using GymApplication.Repository.Abstractions.Entity;
using Microsoft.AspNetCore.Identity;

namespace GymApplication.Repository.Entities;

public class ApplicationUser : IdentityUser<Guid>, IAuditableEntity
{
    public string? FullName { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
    public virtual ICollection<PaymentLog> Payments { get; set; } = new List<PaymentLog>();
    public virtual ICollection<CheckLog> CheckLogs { get; set; } = new List<CheckLog>();
}