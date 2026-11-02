#!/usr/bin/env bash
set -euo pipefail

BASE_URL="${BASE_URL:-http://localhost:8080}"
CLUB_UUID="${CLUB_UUID:-11111111-1111-4111-8111-111111111111}"
PLAYER_ONE="${PLAYER_ONE:-22222222-2222-4222-8222-222222222221}"
PLAYER_TWO="${PLAYER_TWO:-22222222-2222-4222-8222-222222222222}"
STAMP="$(date +%s)"

api() {
  curl --fail-with-body --silent --show-error "$@"
}

field() {
  python3 -c "import json,sys; print(json.load(sys.stdin)$1)"
}

echo "==> health"
api "$BASE_URL/health/ready" >/dev/null

echo "==> seeded reference data"
SPORTS=$(api "$BASE_URL/api/Sport/GetAll" | field "['data'].__len__()")
CLUBS=$(api "$BASE_URL/api/Club/GetAll" | field "['data'].__len__()")
PLAYERS=$(api "$BASE_URL/api/Player/GetAll" | field "['data'].__len__()")
[ "$SPORTS" -ge 2 ]
[ "$CLUBS" -ge 1 ]
[ "$PLAYERS" -ge 2 ]
echo "sports=$SPORTS clubs=$CLUBS players=$PLAYERS"

echo "==> create, start, and populate ranking"
RANKING_UUID=$(api --request POST "$BASE_URL/api/Ranking/CreateRanking" \
  --header "Content-Type: application/json" \
  --data "{\"name\":\"Smoke $STAMP\",\"description\":\"Automated smoke verification\",\"sameInformationClub\":true,\"clubUUId\":\"$CLUB_UUID\",\"eventDate\":\"2026-11-20T18:00:00Z\",\"eventHourStart\":\"18:00\",\"eventHourEnd\":\"22:00\",\"onlyClubMembers\":true,\"price\":20,\"memberPrice\":10,\"matchSameTime\":1,\"latitude\":-23.5505,\"longitude\":-46.6333,\"autoQueue\":false,\"hasNotificationToPlayer\":true}" \
  | field "['data']['uuId']")

api --request POST "$BASE_URL/api/Ranking/StartRanking/$RANKING_UUID" >/dev/null

for PLAYER in "$PLAYER_ONE" "$PLAYER_TWO"; do
  api --request POST "$BASE_URL/api/Ranking/AddPlayer" \
    --header "Content-Type: application/json" \
    --data "{\"playerUUId\":\"$PLAYER\",\"tournamentUUId\":\"$RANKING_UUID\"}" >/dev/null
done

QUEUE_COUNT=0
for _ in $(seq 1 20); do
  QUEUE_COUNT=$(api "$BASE_URL/api/Ranking/GetPlayersOnQueue/$RANKING_UUID" | field "['data'].__len__()")
  [ "$QUEUE_COUNT" -ge 2 ] && break
  sleep 0.5
done
[ "$QUEUE_COUNT" -ge 2 ]

echo "==> create game from queue"
GAME_UUID=$(api --request POST "$BASE_URL/api/Ranking/CreateGameUsingQueue/$RANKING_UUID" | field "['data']['uuId']")

echo "==> finish game and ranking"
api --request PUT "$BASE_URL/api/Ranking/UpdateGame" \
  --header "Content-Type: application/json" \
  --data "{\"teamOneGamePoints\":11,\"teamTwoGamePoints\":5,\"isFinished\":true,\"uuId\":\"$GAME_UUID\"}" >/dev/null

api --request POST "$BASE_URL/api/Ranking/FinishRanking/$RANKING_UUID" >/dev/null

echo "==> verify persisted results"
FINISHED=$(api "$BASE_URL/api/Ranking/GetRanking/$RANKING_UUID" | field "['data']['isFinish']")
POINTS=$(api "$BASE_URL/api/Ranking/GetRanking/$RANKING_UUID" | field "['data']['games'][0]['titleGamePoints']")
WINS=$(api "$BASE_URL/api/Ranking/GetRankingPlayers/$RANKING_UUID" | python3 -c "import json,sys; print(sum(1 for team in json.load(sys.stdin)['data'] if team['wins'] == 1))")
FILTERED=$(api --request POST "$BASE_URL/api/Ranking/GetByFilter" \
  --header "Content-Type: application/json" \
  --data '{"isRanking":true,"onlyFinished":true,"currentPage":1,"itensPerPage":100}' \
  | python3 -c "import json,sys; print(any(item['uuId'] == '$RANKING_UUID' for item in json.load(sys.stdin)['data']['datasource']))")

[ "$FINISHED" = "True" ]
[ "$POINTS" = "11 - 5" ]
[ "$WINS" -ge 1 ]
[ "$FILTERED" = "True" ]

echo "smoke OK: ranking=$RANKING_UUID game=$GAME_UUID"
