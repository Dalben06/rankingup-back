using Dapper.Contrib.Extensions;
using RankingUp.Club.Domain.Entities;
using RankingUp.Core.DataAnnotations;
using RankingUp.Core.Domain;

namespace RankingUp.Tournament.Domain.Entities
{
    [Table("Tournaments")]
    public class Tournaments : AuditEntity
    {
        public Tournaments() : base(0)
        {

        }

        public Tournaments(string name, string description, string address, string addressNumber
            , string addressComplement, string addressDistrict, string city, string state, string country
            , string postalCode, string phone, string email, DateTime eventDate, string eventHourStart, string eventHourEnd
            , bool isActive, bool isRanking, bool onlyClubMembers, decimal price, decimal memberPrice
            , int matchSameTime, string category, int ownerId, decimal latitude
            , decimal longitude, bool autoQueue, bool hasNotificationToPlayer) : base(ownerId)
        {

            TimeSpan.TryParse(eventHourStart, out var startHour);
            TimeSpan.TryParse(eventHourEnd, out var endHour);

            Name = name;
            Description = description;
            Address = address;
            AddressNumber = addressNumber;
            AddressComplement = addressComplement;
            AddressDistrict = addressDistrict;
            City = city;
            State = state;
            Country = country;
            PostalCode = postalCode;
            Phone = phone;
            Email = email;
            EventDate = eventDate;
            EventHourStart = startHour;
            EventHourEnd = endHour;
            IsActive = isActive;
            IsRanking = isRanking;
            OnlyClubMembers = onlyClubMembers;
            Price = price;
            MemberPrice = memberPrice;
            MatchSameTime = matchSameTime;
            Category = category;
            OwnerId = ownerId;
            Latitude = latitude;
            Longitude = longitude;
            AutoQueue = autoQueue;
            HasNotificationToPlayer = hasNotificationToPlayer;
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Address { get; private set; }
        public string AddressNumber { get; private set; }
        public string AddressComplement { get; private set; }
        public string AddressDistrict { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string Country { get; private set; }
        public string PostalCode { get; private set; }
        public string Phone { get; private set; }
        public string Email { get; private set; }
        public DateTime EventDate { get; private set; }
        public TimeSpan EventHourStart { get; private set; }
        public TimeSpan EventHourEnd { get; private set; }
        public bool IsActive { get; private set; }
        public int ClubId { get; private set; }
        public bool IsRanking { get; private set; }
        public bool OnlyClubMembers { get; private set; }
        public decimal Price { get; private set; }
        public decimal MemberPrice { get; private set; }
        public int MatchSameTime { get; private set; }
        public string Category { get; private set; }
        public int OwnerId { get; private set; }
        public decimal Latitude { get; private set; }
        public decimal Longitude { get; private set; }
        public bool AutoQueue { get; private set; }
        public bool HasNotificationToPlayer { get; private set; }

        [OnlyUpdate]
        public DateTime? StartDate { get; private set; }
        [OnlyUpdate]
        public DateTime? FinishDate { get; private set; }

        [Computed]
        public bool IsStart { get => StartDate.HasValue; }
        [Computed]
        public bool IsFinish { get => FinishDate.HasValue; }


        // TODO CRIAR NO BD
        [Computed]
        public bool IsSingle { get; private set; }


        [Computed]
        public Club.Domain.Entities.Clubs Club { get; set; }

        [Computed]
        public string CompleteAddress { get { return $"{Address}, {State} - {City}, {PostalCode}. {Country}"; } }

        [Write(false)]
        public decimal? Distance { get; set; }


        public void SetClub(Clubs clubs)
        {
            if (clubs == null) return;

            this.Club = clubs;
            this.ClubId = clubs.Id;
        }

        public void StartEvent(int UserId)
        {
            this.StartDate = DateTime.Now;
            this.UpdateDate = DateTime.Now;
            this.UpdatePersonId = UserId;
        }
        public void FinishEvent(int UserId)
        {
            this.FinishDate = DateTime.Now;
            this.UpdateDate = DateTime.Now;
            this.UpdatePersonId = UserId;
        }

        public void SetClubInfoInRank()
        {
            if (this.Club == null) return;

            this.Address = this.Club.Address;
            this.AddressNumber = this.Club.AddressNumber;
            this.AddressDistrict = this.Club.AddressDistrict;
            this.AddressComplement = this.Club.AddressComplement;
            this.State = this.Club.State;
            this.City = this.Club.City;
            this.Country = this.Club.Country;
            this.PostalCode = this.Club.PostalCode;

            this.Phone = this.Club.Phone;
            this.Email = this.Club.Email;
        }

        public override void Disable(long IdUsuario)
        {
            DeleteDate = UpdateDate = DateTime.Now;
            DeletePersonId = UpdatePersonId = (int)IdUsuario;
            IsDeleted= true;
            IsActive = false;
        }

        public override void Validate() 
        {
            if (string.IsNullOrEmpty(Address) || Address.Length > 100) this.AddNotification("O endereço é obrigatorio e deve ter menos de 100 caracteres");
            if (string.IsNullOrEmpty(AddressNumber) || AddressNumber.Length > 10) this.AddNotification("O numero do endereço é obrigatorio e deve ter menos de 10 caracteres");
            if (string.IsNullOrEmpty(State) || State.Length > 50) this.AddNotification("O estado é obrigatorio e deve ter menos de 100 caracteres");
            if (string.IsNullOrEmpty(Country) || Country.Length > 50) this.AddNotification("O Pais é obrigatorio e deve ter menos de 50 caracteres");
            if (string.IsNullOrEmpty(Phone) || Phone.Length > 50) this.AddNotification("O telefone é obrigatorio e deve ter menos de 50 caracteres");
            if (string.IsNullOrEmpty(PostalCode) || PostalCode.Length > 30) this.AddNotification("O codigo postal é obrigatorio e deve ter menos de 30 caracteres");

        }
    }
}
