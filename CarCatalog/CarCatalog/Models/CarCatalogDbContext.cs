using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace CarCatalog.Models;

public partial class CarCatalogDbContext : DbContext
{
    public CarCatalogDbContext()
    {
    }

    public CarCatalogDbContext(DbContextOptions<CarCatalogDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Car> Cars { get; set; }

    public virtual DbSet<Model> Models { get; set; }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Reservation> Reservations { get; set; }    

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;database=car_catalog_db;user=root;password=root", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.41-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("brands");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Country)
                .HasMaxLength(50)
                .HasColumnName("country");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Car>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("cars");

            entity.HasIndex(e => e.CreatedBy, "created_by");

            entity.HasIndex(e => e.ModelId, "model_id");

            entity.HasIndex(e => e.Vin, "vin").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.HasMany(c => c.Reservations)
                    .WithOne(r => r.Car)
                    .HasForeignKey(r => r.CarId)
                    .OnDelete(DeleteBehavior.Cascade);
            entity.Property(e => e.Color)
                .HasMaxLength(30)
                .HasColumnName("color");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Mileage).HasColumnName("mileage");
            entity.Property(e => e.ModelId).HasColumnName("model_id");
            entity.Property(e => e.PhotoUrl)
                .HasMaxLength(500)
                .HasColumnName("photo_url");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'available'")
                .HasColumnType("enum('available','sold','reserved')")
                .HasColumnName("status");
            entity.Property(e => e.Vin)
                .HasMaxLength(17)
                .HasColumnName("vin");
            entity.Property(e => e.Year).HasColumnName("year");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Cars)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("cars_ibfk_2");

            entity.HasOne(d => d.Model).WithMany(p => p.Cars)
                .HasForeignKey(d => d.ModelId)
                .HasConstraintName("cars_ibfk_1");
        });

        modelBuilder.Entity<Model>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("models");

            entity.HasIndex(e => e.BrandId, "brand_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BrandId).HasColumnName("brand_id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.YearFrom).HasColumnName("year_from");
            entity.Property(e => e.YearTo).HasColumnName("year_to");

            entity.HasOne(d => d.Brand).WithMany(p => p.Models)
                .HasForeignKey(d => d.BrandId)
                .HasConstraintName("models_ibfk_1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("email");

            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("password_hash");

            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasColumnName("role")
                .HasConversion<string>();

            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");

            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");

            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");

            entity.Property(e => e.AvatarUrl)
                .HasMaxLength(500)
                .HasColumnName("avatar_url");

            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");

            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.ToTable("reservations");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.CarId)
                .HasColumnName("car_id")
                .IsRequired();

            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            entity.Property(e => e.StartDate)
                .IsRequired()
                .HasColumnType("date")
                .HasColumnName("start_date");

            entity.Property(e => e.EndDate)
                .IsRequired()
                .HasColumnType("date")
                .HasColumnName("end_date");

            entity.Property(e => e.TotalPrice)
                .IsRequired()
                .HasColumnType("decimal(10,2)")
                .HasColumnName("total_price");

            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("pending")
                .HasColumnName("status");

            entity.Property(e => e.Comment)
                .HasColumnType("text")
                .HasColumnName("comment");

            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp")
                .HasColumnName("updated_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnUpdate();

            entity.HasOne(r => r.Car)
                .WithMany(c => c.Reservations)
                .HasForeignKey(r => r.CarId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(r => r.CarId, "idx_reservations_car_id");
            entity.HasIndex(r => r.UserId, "idx_reservations_user_id");
            entity.HasIndex(r => r.Status, "idx_reservations_status");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
