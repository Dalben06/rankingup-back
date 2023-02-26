using Dapper.Contrib.Extensions;
using RankingUp.Core.Domain;
using RankingUp.Sport.Domain.Entities;

namespace RankingUp.Club.Domain.Entities
{
    public sealed class Club : AuditEntity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Address { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string Country { get; private set; }
        public string Phone { get; private set; }
        public string PostalCode { get; private set; }
        public TimeSpan BusinessHourStart { get; private set; }
        public TimeSpan BusinessHourEnd { get; private set; }
        public string FacebookUrl { get; private set; }
        public string InstagramUrl { get; private set; }
        public string TwitterUrl { get; private set; }
        public string ImageUrl { get; private set; }
        public bool IsActive { get; private set; }
        [Computed]
        public string CompleteAddress { get {return $"{Address}, {State} - {City}, {PostalCode}. {Country}"; } }

        public Club(): base(0)
        {

        }
        public Club(string name, string description, string address, string city, string state,
            string country, string phone, string postalCode,
            string businessHourStart, string businessHourEnd, 
            string facebookUrl, string instagramUrl, string twitterUrl, string imageUrl, bool isActive, int UserId): base(UserId)
        {
            TimeSpan.TryParse(businessHourStart, out var startHour);
            TimeSpan.TryParse(businessHourEnd, out var endHour);

            Name = name;
            Description = description;
            Address = address;
            City = city;
            State = state;
            Country = country;
            Phone = phone;
            PostalCode = postalCode;
            BusinessHourStart = startHour;
            BusinessHourEnd = endHour;
            FacebookUrl = facebookUrl;
            InstagramUrl = instagramUrl;
            TwitterUrl = twitterUrl;
            ImageUrl = imageUrl;
            IsActive = isActive;

            Validate();
        }


        [Computed]
        public IEnumerable<Sports> Sports { get; set; }

        public override void Disable(long IdUsuario)
        {
            this.IsDeleted = true;
            this.DeleteDate = this.UpdateDate = DateTime.UtcNow;
            this.DeletePersonId = this.UpdatePersonId = (int)IdUsuario;
        }

        public override void Validate()
        {
            return;
        }

    }
}
