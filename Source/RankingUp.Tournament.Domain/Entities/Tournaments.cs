using Dapper.Contrib.Extensions;
using RankingUp.Core.Domain;

namespace RankingUp.Tournament.Domain.Entities
{
    public class Tournaments : AuditEntity
    {
        public Tournaments(): base(0)
        {

        }

        public Tournaments(string name, string description, string address, string city, string state, string country, string postalCode, string phone
            , DateTime date, TimeSpan businessHourStart, TimeSpan businessHourEnd, bool isActive, int clubId, bool isRanking, bool onlyClubMembers, decimal value
            , decimal memberValue, int matchSameTime, string category, int ownerId) : base(ownerId)
        {
            Name = name;
            Description = description;
            Address = address;
            City = city;
            State = state;
            Country = country;
            PostalCode = postalCode;
            Phone = phone;
            Date = date;
            BusinessHourStart = businessHourStart;
            BusinessHourEnd = businessHourEnd;
            IsActive = isActive;
            ClubId = clubId;
            IsRanking = isRanking;
            OnlyClubMembers = onlyClubMembers;
            Value = value;
            MemberValue = memberValue;
            MatchSameTime = matchSameTime;
            Category = category;
            OwnerId = ownerId;
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Address { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string Country { get; private set; }
        public string PostalCode { get; private set; }
        public string Phone { get; private set; }
        public DateTime Date { get; set; }
        public TimeSpan BusinessHourStart { get; private set; }
        public TimeSpan BusinessHourEnd { get; private set; }
        public bool IsActive { get; private set; }
        public int ClubId { get; private set; }
        public bool IsRanking { get; private set; }
        public bool OnlyClubMembers { get; private set; }
        public decimal Value { get; private set; }
        public decimal MemberValue { get; private set; }
        public int MatchSameTime { get; private set; }
        public string Category { get; private set; }
        public int OwnerId { get; private set; }

        public Club.Domain.Entities.Clubs Club { get;private set; }

        [Computed]
        public string CompleteAddress { get { return $"{Address}, {State} - {City}, {PostalCode}. {Country}"; } }

        public override void Disable(long IdUsuario)
        {
            DeleteDate = UpdateDate = DateTime.Now;
            DeletePersonId = UpdatePersonId = (int)IdUsuario;
            IsActive = false;
        }

        public override void Validate() { }
    }
}
