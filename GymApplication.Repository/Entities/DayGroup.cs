using GymApplication.Repository.Abstractions.Entity;

namespace GymApplication.Repository.Entities;

public class DayGroup : Entity<Guid>, IAuditableEntity
{
    public string? Group { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}