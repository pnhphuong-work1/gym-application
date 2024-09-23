using GymApplication.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymApplication.Repository.Configuration;

public class DayGroupConfiguration : IEntityTypeConfiguration<DayGroup>
{
    public void Configure(EntityTypeBuilder<DayGroup> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Group)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(e => e.CreatedAt)
            .IsRequired();
        
        builder.Property(e => e.UpdatedAt);
        
        builder.Property(e => e.DeletedAt);
        
        builder.HasMany(dg => dg.Subscriptions)
            .WithOne(s => s.DayGroup)
            .HasForeignKey(s => s.DayGroupId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}