-- =============================================================================
-- RankingUp - PostgreSQL Recreation Script (Supabase)
-- Generated from domain entities (Dapper.Contrib mapping)
--
-- Type mapping:
--   int            → INTEGER
--   Guid           → UUID
--   bool           → BOOLEAN
--   DateTimeOffset → TIMESTAMPTZ
--   DateTime       → TIMESTAMP
--   TimeSpan       → TIME
--   decimal        → NUMERIC
--   string         → VARCHAR(n) / TEXT
--
-- Note: PostgreSQL folds unquoted identifiers to lowercase.
--       Dapper accesses columns case-insensitively, so this is transparent.
-- =============================================================================

-- -----------------------------------------------------------------------------
-- Drop tables in reverse dependency order (CASCADE handles FK references)
-- -----------------------------------------------------------------------------
DROP TABLE IF EXISTS RankingQueues   CASCADE;
DROP TABLE IF EXISTS TournamentGames CASCADE;
DROP TABLE IF EXISTS TournamentTeams CASCADE;
DROP TABLE IF EXISTS Tournaments     CASCADE;
DROP TABLE IF EXISTS PlayerSports    CASCADE;
DROP TABLE IF EXISTS PlayerClubs     CASCADE;
DROP TABLE IF EXISTS ClubSports      CASCADE;
DROP TABLE IF EXISTS Players         CASCADE;
DROP TABLE IF EXISTS Clubs           CASCADE;
DROP TABLE IF EXISTS Sports          CASCADE;

-- =============================================================================
-- 1. Sports
--    Inherits: BaseEntity (Id, UUId only — no audit columns)
-- =============================================================================
CREATE TABLE Sports (
    Id          SERIAL       NOT NULL,
    UUId        UUID         NOT NULL,
    Name        VARCHAR(255),
    Description TEXT,

    CONSTRAINT PK_Sports     PRIMARY KEY (Id),
    CONSTRAINT UQ_Sports_UUId UNIQUE (UUId)
);

-- =============================================================================
-- 2. Clubs
--    Inherits: AuditEntity → BaseEntity
-- =============================================================================
CREATE TABLE Clubs (
    Id                  SERIAL       NOT NULL,
    UUId                UUID         NOT NULL,
    -- own columns
    Name                VARCHAR(255),
    Description         TEXT,
    Address             VARCHAR(255),
    AddressNumber       VARCHAR(255),
    AddressComplement   VARCHAR(255),
    AddressDistrict     VARCHAR(255),
    City                VARCHAR(255),
    State               VARCHAR(255),
    Country             VARCHAR(255),
    PostalCode          VARCHAR(255),
    Phone               VARCHAR(255),
    BusinessHourStart   TIME,
    BusinessHourEnd     TIME,
    FacebookUrl         VARCHAR(255),
    InstagramUrl        VARCHAR(255),
    TwitterUrl          VARCHAR(255),
    ImageUrl            VARCHAR(255),
    Email               VARCHAR(255),
    IsActive            BOOLEAN      NOT NULL DEFAULT FALSE,
    -- audit columns (AuditEntity)
    CreateDate          TIMESTAMPTZ  NOT NULL,
    CreatePersonId      INTEGER      NOT NULL,
    UpdateDate          TIMESTAMPTZ  NOT NULL,
    UpdatePersonId      INTEGER      NOT NULL,
    IsDeleted           BOOLEAN      NOT NULL DEFAULT FALSE,
    DeleteDate          TIMESTAMPTZ,
    DeletePersonId      INTEGER,

    CONSTRAINT PK_Clubs      PRIMARY KEY (Id),
    CONSTRAINT UQ_Clubs_UUId UNIQUE (UUId)
);

-- =============================================================================
-- 3. Players
--    Inherits: AuditEntity → BaseEntity
--    Note: UserId references an external auth/user system (no FK constraint)
-- =============================================================================
CREATE TABLE Players (
    Id              SERIAL       NOT NULL,
    UUId            UUID         NOT NULL,
    -- own columns
    UserId          INTEGER      NOT NULL,
    Name            VARCHAR(255),
    Description     TEXT,
    Phone           VARCHAR(255),
    -- audit columns (AuditEntity)
    CreateDate      TIMESTAMPTZ  NOT NULL,
    CreatePersonId  INTEGER      NOT NULL,
    UpdateDate      TIMESTAMPTZ  NOT NULL,
    UpdatePersonId  INTEGER      NOT NULL,
    IsDeleted       BOOLEAN      NOT NULL DEFAULT FALSE,
    DeleteDate      TIMESTAMPTZ,
    DeletePersonId  INTEGER,

    CONSTRAINT PK_Players      PRIMARY KEY (Id),
    CONSTRAINT UQ_Players_UUId UNIQUE (UUId)
);

-- =============================================================================
-- 4. ClubSports
--    Inherits: AuditEntity → BaseEntity
--    FK: ClubId → Clubs.Id, SportId → Sports.Id
-- =============================================================================
CREATE TABLE ClubSports (
    Id              SERIAL      NOT NULL,
    UUId            UUID        NOT NULL,
    -- own columns
    ClubId          INTEGER     NOT NULL,
    SportId         INTEGER     NOT NULL,
    -- audit columns (AuditEntity)
    CreateDate      TIMESTAMPTZ NOT NULL,
    CreatePersonId  INTEGER     NOT NULL,
    UpdateDate      TIMESTAMPTZ NOT NULL,
    UpdatePersonId  INTEGER     NOT NULL,
    IsDeleted       BOOLEAN     NOT NULL DEFAULT FALSE,
    DeleteDate      TIMESTAMPTZ,
    DeletePersonId  INTEGER,

    CONSTRAINT PK_ClubSports        PRIMARY KEY (Id),
    CONSTRAINT UQ_ClubSports_UUId   UNIQUE (UUId),
    CONSTRAINT FK_ClubSports_Clubs  FOREIGN KEY (ClubId)  REFERENCES Clubs(Id),
    CONSTRAINT FK_ClubSports_Sports FOREIGN KEY (SportId) REFERENCES Sports(Id)
);

-- =============================================================================
-- 5. PlayerClubs
--    Inherits: AuditEntity → BaseEntity
--    FK: PlayerId → Players.Id, ClubId → Clubs.Id
-- =============================================================================
CREATE TABLE PlayerClubs (
    Id              SERIAL      NOT NULL,
    UUId            UUID        NOT NULL,
    -- own columns
    PlayerId        INTEGER     NOT NULL,
    ClubId          INTEGER     NOT NULL,
    IsActive        BOOLEAN     NOT NULL DEFAULT FALSE,
    -- audit columns (AuditEntity)
    CreateDate      TIMESTAMPTZ NOT NULL,
    CreatePersonId  INTEGER     NOT NULL,
    UpdateDate      TIMESTAMPTZ NOT NULL,
    UpdatePersonId  INTEGER     NOT NULL,
    IsDeleted       BOOLEAN     NOT NULL DEFAULT FALSE,
    DeleteDate      TIMESTAMPTZ,
    DeletePersonId  INTEGER,

    CONSTRAINT PK_PlayerClubs         PRIMARY KEY (Id),
    CONSTRAINT UQ_PlayerClubs_UUId    UNIQUE (UUId),
    CONSTRAINT FK_PlayerClubs_Players FOREIGN KEY (PlayerId) REFERENCES Players(Id),
    CONSTRAINT FK_PlayerClubs_Clubs   FOREIGN KEY (ClubId)   REFERENCES Clubs(Id)
);

-- =============================================================================
-- 6. PlayerSports
--    Inherits: AuditEntity → BaseEntity
--    FK: PlayerId → Players.Id, SportId → Sports.Id
-- =============================================================================
CREATE TABLE PlayerSports (
    Id              SERIAL      NOT NULL,
    UUId            UUID        NOT NULL,
    -- own columns
    PlayerId        INTEGER     NOT NULL,
    SportId         INTEGER     NOT NULL,
    IsActive        BOOLEAN     NOT NULL DEFAULT FALSE,
    -- audit columns (AuditEntity)
    CreateDate      TIMESTAMPTZ NOT NULL,
    CreatePersonId  INTEGER     NOT NULL,
    UpdateDate      TIMESTAMPTZ NOT NULL,
    UpdatePersonId  INTEGER     NOT NULL,
    IsDeleted       BOOLEAN     NOT NULL DEFAULT FALSE,
    DeleteDate      TIMESTAMPTZ,
    DeletePersonId  INTEGER,

    CONSTRAINT PK_PlayerSports         PRIMARY KEY (Id),
    CONSTRAINT UQ_PlayerSports_UUId    UNIQUE (UUId),
    CONSTRAINT FK_PlayerSports_Players FOREIGN KEY (PlayerId) REFERENCES Players(Id),
    CONSTRAINT FK_PlayerSports_Sports  FOREIGN KEY (SportId)  REFERENCES Sports(Id)
);

-- =============================================================================
-- 7. Tournaments
--    Inherits: AuditEntity → BaseEntity
--    FK: ClubId → Clubs.Id
--    Excluded ([Computed] / [Write(false)]): IsStart, IsFinish, IsSingle, Distance
-- =============================================================================
CREATE TABLE Tournaments (
    Id                      SERIAL          NOT NULL,
    UUId                    UUID            NOT NULL,
    -- own columns
    Name                    VARCHAR(255),
    Description             TEXT,
    Address                 VARCHAR(100),
    AddressNumber           VARCHAR(10),
    AddressComplement       VARCHAR(255),
    AddressDistrict         VARCHAR(255),
    City                    VARCHAR(255),
    State                   VARCHAR(50),
    Country                 VARCHAR(50),
    PostalCode              VARCHAR(30),
    Phone                   VARCHAR(50),
    Email                   VARCHAR(255),
    EventDate               TIMESTAMP,
    EventHourStart          TIME,
    EventHourEnd            TIME,
    IsActive                BOOLEAN         NOT NULL DEFAULT FALSE,
    ClubId                  INTEGER         NOT NULL,
    IsRanking               BOOLEAN         NOT NULL DEFAULT FALSE,
    OnlyClubMembers         BOOLEAN         NOT NULL DEFAULT FALSE,
    Price                   NUMERIC(10,2)   NOT NULL DEFAULT 0.00,
    MemberPrice             NUMERIC(10,2)   NOT NULL DEFAULT 0.00,
    MatchSameTime           INTEGER         NOT NULL DEFAULT 0,
    Category                VARCHAR(255),
    OwnerId                 INTEGER         NOT NULL,
    Latitude                NUMERIC(10,6)   NOT NULL DEFAULT 0.000000,
    Longitude               NUMERIC(10,6)   NOT NULL DEFAULT 0.000000,
    AutoQueue               BOOLEAN         NOT NULL DEFAULT FALSE,
    HasNotificationToPlayer BOOLEAN         NOT NULL DEFAULT FALSE,
    StartDate               TIMESTAMP,
    FinishDate              TIMESTAMP,
    -- audit columns (AuditEntity)
    CreateDate              TIMESTAMPTZ     NOT NULL,
    CreatePersonId          INTEGER         NOT NULL,
    UpdateDate              TIMESTAMPTZ     NOT NULL,
    UpdatePersonId          INTEGER         NOT NULL,
    IsDeleted               BOOLEAN         NOT NULL DEFAULT FALSE,
    DeleteDate              TIMESTAMPTZ,
    DeletePersonId          INTEGER,

    CONSTRAINT PK_Tournaments       PRIMARY KEY (Id),
    CONSTRAINT UQ_Tournaments_UUId  UNIQUE (UUId),
    CONSTRAINT FK_Tournaments_Clubs FOREIGN KEY (ClubId) REFERENCES Clubs(Id)
);

-- =============================================================================
-- 8. TournamentTeams
--    Inherits: AuditEntity → BaseEntity
--    FK: TournamentId → Tournaments.Id, TeamId → Players.Id
-- =============================================================================
CREATE TABLE TournamentTeams (
    Id              SERIAL      NOT NULL,
    UUId            UUID        NOT NULL,
    -- own columns
    TournamentId    INTEGER     NOT NULL,
    TeamId          INTEGER     NOT NULL,
    IsActive        BOOLEAN     NOT NULL DEFAULT FALSE,
    -- audit columns (AuditEntity)
    CreateDate      TIMESTAMPTZ NOT NULL,
    CreatePersonId  INTEGER     NOT NULL,
    UpdateDate      TIMESTAMPTZ NOT NULL,
    UpdatePersonId  INTEGER     NOT NULL,
    IsDeleted       BOOLEAN     NOT NULL DEFAULT FALSE,
    DeleteDate      TIMESTAMPTZ,
    DeletePersonId  INTEGER,

    CONSTRAINT PK_TournamentTeams              PRIMARY KEY (Id),
    CONSTRAINT UQ_TournamentTeams_UUId         UNIQUE (UUId),
    CONSTRAINT FK_TournamentTeams_Tournaments  FOREIGN KEY (TournamentId) REFERENCES Tournaments(Id),
    CONSTRAINT FK_TournamentTeams_Players      FOREIGN KEY (TeamId)       REFERENCES Players(Id)
);

-- =============================================================================
-- 9. TournamentGames
--    Inherits: AuditEntity → BaseEntity
--    FK: TournamentId → Tournaments.Id
--        TeamOneId    → TournamentTeams.Id
--        TeamTwoId    → TournamentTeams.Id
--    Excluded ([Computed]): Winner, Loser, WinnerPoints, LoserPoints
-- =============================================================================
CREATE TABLE TournamentGames (
    Id                  SERIAL      NOT NULL,
    UUId                UUID        NOT NULL,
    -- own columns
    TournamentId        INTEGER     NOT NULL,
    TeamOneId           INTEGER     NOT NULL,
    TeamTwoId           INTEGER     NOT NULL,
    TeamOneGamePoints   INTEGER     NOT NULL DEFAULT 0,
    TeamTwoGamePoints   INTEGER     NOT NULL DEFAULT 0,
    IsFinished          BOOLEAN     NOT NULL DEFAULT FALSE,
    -- audit columns (AuditEntity)
    CreateDate          TIMESTAMPTZ NOT NULL,
    CreatePersonId      INTEGER     NOT NULL,
    UpdateDate          TIMESTAMPTZ NOT NULL,
    UpdatePersonId      INTEGER     NOT NULL,
    IsDeleted           BOOLEAN     NOT NULL DEFAULT FALSE,
    DeleteDate          TIMESTAMPTZ,
    DeletePersonId      INTEGER,

    CONSTRAINT PK_TournamentGames              PRIMARY KEY (Id),
    CONSTRAINT UQ_TournamentGames_UUId         UNIQUE (UUId),
    CONSTRAINT FK_TournamentGames_Tournaments  FOREIGN KEY (TournamentId) REFERENCES Tournaments(Id),
    CONSTRAINT FK_TournamentGames_TeamOne      FOREIGN KEY (TeamOneId)    REFERENCES TournamentTeams(Id),
    CONSTRAINT FK_TournamentGames_TeamTwo      FOREIGN KEY (TeamTwoId)    REFERENCES TournamentTeams(Id)
);

-- =============================================================================
-- 10. RankingQueues
--     Inherits: AuditEntity → BaseEntity
--     FK: TournamentId → Tournaments.Id, TeamId → TournamentTeams.Id
-- =============================================================================
CREATE TABLE RankingQueues (
    Id              SERIAL      NOT NULL,
    UUId            UUID        NOT NULL,
    -- own columns
    TournamentId    INTEGER     NOT NULL,
    TeamId          INTEGER     NOT NULL,
    -- audit columns (AuditEntity)
    CreateDate      TIMESTAMPTZ NOT NULL,
    CreatePersonId  INTEGER     NOT NULL,
    UpdateDate      TIMESTAMPTZ NOT NULL,
    UpdatePersonId  INTEGER     NOT NULL,
    IsDeleted       BOOLEAN     NOT NULL DEFAULT FALSE,
    DeleteDate      TIMESTAMPTZ,
    DeletePersonId  INTEGER,

    CONSTRAINT PK_RankingQueues                  PRIMARY KEY (Id),
    CONSTRAINT UQ_RankingQueues_UUId             UNIQUE (UUId),
    CONSTRAINT FK_RankingQueues_Tournaments      FOREIGN KEY (TournamentId) REFERENCES Tournaments(Id),
    CONSTRAINT FK_RankingQueues_TournamentTeams  FOREIGN KEY (TeamId)       REFERENCES TournamentTeams(Id)
);
