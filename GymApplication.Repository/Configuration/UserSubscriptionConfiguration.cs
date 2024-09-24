using GymApplication.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymApplication.Repository.Configuration;

public class UserSubscriptionConfiguration : IEntityTypeConfiguration<UserSubscription>
{
    public void Configure(EntityTypeBuilder<UserSubscription> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.SubscriptionId)
            .IsRequired();
        
        builder.Property(x => x.PaymentId)
            .IsRequired();

        builder.Property(x => x.PaymentPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.WorkoutSteak);
        
        builder.Property(x => x.LongestWorkoutSteak);

        builder.Property(x => x.LastWorkoutDate);
        
        builder.Property(x => x.SubscriptionEndDate)
            .IsRequired();

        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(x => x.CreatedAt)
            .IsRequired();
        
        builder.Property(x => x.UpdatedAt);
        
        builder.Property(x => x.DeletedAt);
    }
}