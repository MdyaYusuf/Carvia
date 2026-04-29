using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Carvia.Features.CuratorItems;

public class CuratorItemConfiguration : IEntityTypeConfiguration<CuratorItem>
{
  public void Configure(EntityTypeBuilder<CuratorItem> builder)
  {
    builder.ToTable("CuratorItems");

    builder.HasKey(ci => ci.Id);

    builder.Property(ci => ci.Id)
      .HasColumnName("Id")
      .IsRequired();

    builder.Property(ci => ci.CreatedDate)
      .HasColumnName("CreatedDate")
      .IsRequired();

    builder.Property(ci => ci.UpdatedDate)
      .HasColumnName("UpdatedDate")
      .IsRequired(false);

    builder.Property(ci => ci.UserNote)
      .HasMaxLength(500)
      .IsRequired(false);

    builder.HasOne(ci => ci.User)
      .WithMany(u => u.CuratorItems)
      .HasForeignKey(ci => ci.UserId)
      .OnDelete(DeleteBehavior.Cascade);

    builder.HasOne(ci => ci.Car)
      .WithMany(c => c.CuratorItems)
      .HasForeignKey(ci => ci.CarId)
      .OnDelete(DeleteBehavior.Cascade);

    builder.HasIndex(ci => new { ci.UserId, ci.CarId }).IsUnique();
  }
}
