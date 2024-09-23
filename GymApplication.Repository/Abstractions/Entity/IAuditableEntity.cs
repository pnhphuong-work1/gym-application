namespace GymApplication.Repository.Abstractions.Entity;

public interface IAuditableEntity
{
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}