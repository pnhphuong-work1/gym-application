using GymApplication.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymApplication.Repository.Configuration;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.FullName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.DateOfBirth)
            .IsRequired();

        builder.Property(e => e.IsDeleted)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt);

        builder.Property(e => e.DeletedAt);

        builder.HasMany(u => u.UserSubscriptions)
            .WithOne(us => us.User)
            .HasForeignKey(us => us.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasMany(u => u.Payments)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.NoAction);
            
        builder.HasMany(u => u.CheckLogs)
            .WithOne(cl => cl.User)
            .HasForeignKey(cl => cl.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}