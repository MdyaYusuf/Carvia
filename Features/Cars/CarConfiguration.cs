using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Carvia.Features.Cars;

public class CarConfiguration : IEntityTypeConfiguration<Car>
{
  public void Configure(EntityTypeBuilder<Car> builder)
  {
    builder.ToTable("Cars");

    builder.HasKey(c => c.Id);

    builder.Property(c => c.Id)
      .HasColumnName("Id")
      .IsRequired();

    builder.Property(c => c.CreatedDate)
      .HasColumnName("CreatedDate")
      .IsRequired();

    builder.Property(c => c.UpdatedDate)
      .HasColumnName("UpdatedDate")
      .IsRequired(false);

    builder.Property(c => c.Make)
      .HasMaxLength(100)
      .IsRequired();

    builder.Property(c => c.Model)
      .HasMaxLength(100)
      .IsRequired();

    builder.Property(c => c.Year)
      .IsRequired();

    builder.Property(c => c.Price)
      .HasPrecision(18, 2)
      .IsRequired();

    builder.Property(c => c.ImageUrl)
      .HasMaxLength(500)
      .IsRequired();

    builder.Property(c => c.Description)
      .HasMaxLength(2000)
      .IsRequired();

    builder.HasOne(c => c.Category)
      .WithMany(cat => cat.Cars)
      .HasForeignKey(c => c.CategoryId)
      .OnDelete(DeleteBehavior.Restrict);

    builder.HasMany(c => c.Images)
      .WithOne(i => i.Car)
      .HasForeignKey(i => i.CarId)
      .OnDelete(DeleteBehavior.Cascade);

    builder.HasMany(c => c.CuratorItems)
      .WithOne(ci => ci.Car)
      .HasForeignKey(ci => ci.CarId)
      .OnDelete(DeleteBehavior.Cascade);

    builder.HasIndex(c => c.Make);
  }
}
