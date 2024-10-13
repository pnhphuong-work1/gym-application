using GymApplication.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymApplication.Repository.Configuration;

public class PaymentLogConfiguration : IEntityTypeConfiguration<PaymentLog>
{
    public void Configure(EntityTypeBuilder<PaymentLog> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.PaymentStatus)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(e => e.PaymentDate)
            .IsRequired();

        builder.HasIndex(e => e.PayOsOrderId)
            .IsUnique();
        
        builder.Property(e => e.UserId)
            .IsRequired();
        
        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);
        
        builder.Property(e => e.CreatedAt)
            .IsRequired();
        
        builder.Property(e => e.UpdatedAt);
        
        builder.Property(e => e.DeletedAt);
        
        builder.HasMany(e => e.UserSubscriptions)
            .WithOne(e => e.Payment)
            .HasForeignKey(e => e.PaymentId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}