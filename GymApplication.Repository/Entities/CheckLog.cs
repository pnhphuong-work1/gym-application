using GymApplication.Repository.Abstractions.Entity;

namespace GymApplication.Repository.Entities;

public class CheckLog : Entity<Guid>, IAuditableEntity
{
    public Guid UserId { get; set; }
    public Guid? CheckInId { get; set; }
    public Guid UserSubscriptionId { get; set; }
    public string? CheckStatus { get; set; }
    public TimeOnly? WorkoutTime { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    public virtual ApplicationUser? User { get; set; }
    public virtual UserSubscription? UserSubscription { get; set; }
}