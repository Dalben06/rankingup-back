-- Stable local reference and demo data. Safe to run repeatedly.

INSERT INTO Sports (UUId, Name, Description)
VALUES
    ('bec6d441-53aa-42aa-b420-2ef3f18868d2', 'Table Tennis', 'Table tennis'),
    ('0a8385a6-356f-4de3-a5a4-6ebd1beff72b', 'Magic', 'Magic: The Gathering')
ON CONFLICT (UUId) DO UPDATE
SET Name = EXCLUDED.Name,
    Description = EXCLUDED.Description;

INSERT INTO Clubs (
    UUId, Name, Description, Address, AddressNumber, AddressComplement,
    AddressDistrict, City, State, Country, PostalCode, Phone,
    BusinessHourStart, BusinessHourEnd, Email, IsActive,
    CreateDate, CreatePersonId, UpdateDate, UpdatePersonId, IsDeleted)
VALUES (
    '11111111-1111-4111-8111-111111111111',
    'RankingUp Demo Club',
    'Local Docker development club',
    'Rua do Esporte', '100', 'Quadra A', 'Centro',
    'Sao Paulo', 'SP', 'Brasil', '01000-000', '5511999990000',
    '08:00:00', '22:00:00', 'demo@rankingup.local', TRUE,
    CURRENT_TIMESTAMP, 1, CURRENT_TIMESTAMP, 1, FALSE)
ON CONFLICT (UUId) DO UPDATE
SET Name = EXCLUDED.Name,
    Description = EXCLUDED.Description,
    IsActive = TRUE,
    IsDeleted = FALSE,
    DeleteDate = NULL,
    DeletePersonId = NULL,
    UpdateDate = CURRENT_TIMESTAMP,
    UpdatePersonId = 1;

INSERT INTO Players (
    UUId, UserId, Name, Description, Phone,
    CreateDate, CreatePersonId, UpdateDate, UpdatePersonId, IsDeleted)
VALUES
    ('22222222-2222-4222-8222-222222222221', 101, 'Demo Player One', 'Local demo player', '5511999990001', CURRENT_TIMESTAMP, 1, CURRENT_TIMESTAMP, 1, FALSE),
    ('22222222-2222-4222-8222-222222222222', 102, 'Demo Player Two', 'Local demo player', '5511999990002', CURRENT_TIMESTAMP, 1, CURRENT_TIMESTAMP, 1, FALSE)
ON CONFLICT (UUId) DO UPDATE
SET Name = EXCLUDED.Name,
    Description = EXCLUDED.Description,
    Phone = EXCLUDED.Phone,
    IsDeleted = FALSE,
    DeleteDate = NULL,
    DeletePersonId = NULL,
    UpdateDate = CURRENT_TIMESTAMP,
    UpdatePersonId = 1;

INSERT INTO ClubSports (
    UUId, ClubId, SportId,
    CreateDate, CreatePersonId, UpdateDate, UpdatePersonId, IsDeleted)
SELECT
    '33333333-3333-4333-8333-333333333331', Clubs.Id, Sports.Id,
    CURRENT_TIMESTAMP, 1, CURRENT_TIMESTAMP, 1, FALSE
FROM Clubs
JOIN Sports ON Sports.UUId = 'bec6d441-53aa-42aa-b420-2ef3f18868d2'
WHERE Clubs.UUId = '11111111-1111-4111-8111-111111111111'
ON CONFLICT (UUId) DO UPDATE
SET IsDeleted = FALSE,
    DeleteDate = NULL,
    DeletePersonId = NULL,
    UpdateDate = CURRENT_TIMESTAMP,
    UpdatePersonId = 1;

INSERT INTO PlayerClubs (
    UUId, PlayerId, ClubId, IsActive,
    CreateDate, CreatePersonId, UpdateDate, UpdatePersonId, IsDeleted)
SELECT
    relation.UUId, Players.Id, Clubs.Id, TRUE,
    CURRENT_TIMESTAMP, 1, CURRENT_TIMESTAMP, 1, FALSE
FROM (VALUES
    ('44444444-4444-4444-8444-444444444441'::UUID, '22222222-2222-4222-8222-222222222221'::UUID),
    ('44444444-4444-4444-8444-444444444442'::UUID, '22222222-2222-4222-8222-222222222222'::UUID)
) AS relation(UUId, PlayerUUId)
JOIN Players ON Players.UUId = relation.PlayerUUId
JOIN Clubs ON Clubs.UUId = '11111111-1111-4111-8111-111111111111'
ON CONFLICT (UUId) DO UPDATE
SET IsActive = TRUE,
    IsDeleted = FALSE,
    DeleteDate = NULL,
    DeletePersonId = NULL,
    UpdateDate = CURRENT_TIMESTAMP,
    UpdatePersonId = 1;

INSERT INTO PlayerSports (
    UUId, PlayerId, SportId, IsActive,
    CreateDate, CreatePersonId, UpdateDate, UpdatePersonId, IsDeleted)
SELECT
    relation.UUId, Players.Id, Sports.Id, TRUE,
    CURRENT_TIMESTAMP, 1, CURRENT_TIMESTAMP, 1, FALSE
FROM (VALUES
    ('55555555-5555-4555-8555-555555555551'::UUID, '22222222-2222-4222-8222-222222222221'::UUID),
    ('55555555-5555-4555-8555-555555555552'::UUID, '22222222-2222-4222-8222-222222222222'::UUID)
) AS relation(UUId, PlayerUUId)
JOIN Players ON Players.UUId = relation.PlayerUUId
JOIN Sports ON Sports.UUId = 'bec6d441-53aa-42aa-b420-2ef3f18868d2'
ON CONFLICT (UUId) DO UPDATE
SET IsActive = TRUE,
    IsDeleted = FALSE,
    DeleteDate = NULL,
    DeletePersonId = NULL,
    UpdateDate = CURRENT_TIMESTAMP,
    UpdatePersonId = 1;
