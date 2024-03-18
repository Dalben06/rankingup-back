namespace RankingUp.Tournament.Domain.Enums
{
    public enum SignalrRankingEventType
    {
        PlayerAdded = 1,
        PlayerRemoved = 2,
        GameCreated = 3,
        GameFinished = 4,
        RankingStarted = 5,
        RankingFinished = 6,
        GameUpdated = 7,
        RankingTable = 8,
    }
}
