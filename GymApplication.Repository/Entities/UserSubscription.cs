using GymApplication.Repository.Abstractions.Entity;

namespace GymApplication.Repository.Entities;

public class UserSubscription : Entity<Guid>, IAuditableEntity
{
    public Guid UserId { get; set; }
    public Guid SubscriptionId { get; set; }
    public Guid PaymentId { get; set; }
    public decimal PaymentPrice { get; set; }
    public int WorkoutSteak { get; set; }
    public int LongestWorkoutSteak { get; set; }
    public DateTime LastWorkoutDate { get; set; }
    public DateTime SubscriptionEndDate { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    public virtual ApplicationUser? User { get; set; }
    public virtual Subscription? Subscription { get; set; }
    public virtual PaymentLog? Payment { get; set; }
}