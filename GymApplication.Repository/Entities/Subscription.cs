using GymApplication.Repository.Abstractions.Entity;

namespace GymApplication.Repository.Entities;

public class Subscription : Entity<Guid>, IAuditableEntity
{
    public string? Name { get; set; }
    public TimeOnly? TotalWorkoutTime { get; set; }
    public decimal Price { get; set; }
    public Guid DayGroupId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int TotalMonth { get; set; }
    
    public virtual DayGroup? DayGroup { get; set; }
    public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
}