using GymApplication.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymApplication.Repository.Configuration;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(e => e.Price)
            .HasPrecision(18, 2)
            .IsRequired();
        
        builder.Property(e => e.DayGroupId)
            .IsRequired();

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);
        
        builder.Property(e => e.CreatedAt)
            .IsRequired();
        
        builder.Property(e => e.UpdatedAt);
        
        builder.Property(e => e.DeletedAt);
        
        builder.HasMany(e => e.UserSubscriptions)
            .WithOne(e => e.Subscription)
            .HasForeignKey(e => e.SubscriptionId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}