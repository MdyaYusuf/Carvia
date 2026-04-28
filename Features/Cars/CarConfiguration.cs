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
      .HasColumnName("Make")
      .HasMaxLength(100)
      .IsRequired();

    builder.Property(c => c.Model)
      .HasColumnName("Model")
      .HasMaxLength(100)
      .IsRequired();

    builder.Property(c => c.Year)
      .HasColumnName("Year")
      .IsRequired();

    builder.Property(c => c.Price)
      .HasColumnName("Price")
      .HasPrecision(18, 2)
      .IsRequired();

    builder.Property(c => c.ImageUrl)
      .HasColumnName("ImageUrl")
      .HasMaxLength(500)
      .IsRequired();

    builder.Property(c => c.Description)
      .HasColumnName("Description")
      .HasMaxLength(2000)
      .IsRequired();
  }
}
