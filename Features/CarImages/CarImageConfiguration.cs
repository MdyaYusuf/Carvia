using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Carvia.Features.CarImages;

public class CarImageConfiguration : IEntityTypeConfiguration<CarImage>
{
  public void Configure(EntityTypeBuilder<CarImage> builder)
  {
    builder.ToTable("CarImages");

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

    builder.Property(ci => ci.Url)
      .HasMaxLength(500)
      .IsRequired();

    builder.Property(ci => ci.AltText)
      .HasMaxLength(200)
      .IsRequired(false);

    builder.Property(ci => ci.DisplayOrder)
      .IsRequired();

    builder.HasOne(ci => ci.Car)
      .WithMany(c => c.Images)
      .HasForeignKey(ci => ci.CarId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
