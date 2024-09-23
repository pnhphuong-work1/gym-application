using GymApplication.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymApplication.Repository.Configuration;

public class CheckLogConfiguration : IEntityTypeConfiguration<CheckLog>
{
    public void Configure(EntityTypeBuilder<CheckLog> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.UserId)
            .IsRequired();

        builder.Property(p => p.CheckStatus)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(p => p.IsDeleted)
            .HasDefaultValue(false);
        
        
    }
}