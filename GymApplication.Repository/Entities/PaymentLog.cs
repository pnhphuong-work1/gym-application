using GymApplication.Repository.Abstractions.Entity;

namespace GymApplication.Repository.Entities;

public class PaymentLog : Entity<Guid>, IAuditableEntity
{
    public string? PaymentStatus { get; set; }
    public DateTime PaymentDate { get; set; }
    public long PayOsOrderId { get; set; }
    public Guid UserId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    public virtual ApplicationUser? User { get; set; }
}