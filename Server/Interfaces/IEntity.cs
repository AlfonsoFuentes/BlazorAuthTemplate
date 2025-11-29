using System.Security.Cryptography;

namespace Server.Interfaces
{
    public interface IEntity : ISoftDeletable
    {
        Guid Id { get; set; }
        bool IsTennanted { get; }
        string? CreatedBy { get; set; }
        DateTime CreatedOn { get; set; }
        DateTime? LastModifiedOn { get; set; }
        string? LastModifiedBy { get; set; }
        int Order { get; set; }
    }
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }

        DateTime? DeletedOnUtc { get; set; }
    }
    public abstract class Entity : IEntity
    {
        public Guid Id { get; set; }
        public virtual bool IsTennanted => false;
        public string? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public int Order { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOnUtc { get; set; }
    }
    public interface ITennant
    {
        string TenantId { get; set; }
    }
}
