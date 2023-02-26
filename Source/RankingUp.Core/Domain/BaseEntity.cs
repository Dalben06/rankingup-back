using Dapper.Contrib.Extensions;
using RankingUp.Core.DataAnnotations;
using System;

namespace RankingUp.Core.Domain
{
    public abstract class BaseEntity : Notifiable
    {
        [Key]
        public int Id { get; set; }
        [ExplicitKey]
        [OnlyInsert]
        public Guid UUId { get; set; }


        public BaseEntity()
        {
            UUId = UUId == Guid.Empty ? Guid.NewGuid() : UUId;
        }

        public override bool Equals(object obj)
        {
            var compareTo = obj as BaseEntity;

            if (ReferenceEquals(this, compareTo)) return true;
            if (ReferenceEquals(null, compareTo)) return false;

            return Id.Equals(compareTo.Id);
        }

        public static bool operator ==(BaseEntity a, BaseEntity b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(BaseEntity a, BaseEntity b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return (GetType().GetHashCode() * 907) + Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"{GetType().Name} [Id={Id}] [UUId={UUId}]";
        }

        public T DeepClone<T>() where T : BaseEntity
        {
            T newEntity = (T)this.MemberwiseClone();
            newEntity.Id = 0;
            newEntity.UUId = Guid.NewGuid();
            return newEntity;
        }

        public abstract void Disable(long IdUsuario);
        public abstract void Validate();
    }
}
