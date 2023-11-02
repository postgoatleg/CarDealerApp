using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp;

public partial class CarDealershipContext : DbContext
{
    public CarDealershipContext()
    {
    }

    public CarDealershipContext(DbContextOptions<CarDealershipContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Car> Cars { get; set; }

    public virtual DbSet<CarModel> CarModels { get; set; }

    public virtual DbSet<CarcaseType> CarcaseTypes { get; set; }

    public virtual DbSet<CarsEquipment> CarsEquipments { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<ExtraEquipment> ExtraEquipments { get; set; }

    public virtual DbSet<Manufacturer> Manufacturers { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<ViewEmployeeAndPosition> ViewEmployeeAndPositions { get; set; }
    private string conStrOffline = "Server=localhost\\SQLEXPRESS;Database=CarDealership;Trusted_Connection=True;TrustServerCertificate=True";

    private string conStrOnline = "Data Source=SQL6031.site4now.net;Initial Catalog=db_aa090f_cardealership;User Id=db_aa090f_cardealership_admin;Password=zxcpass123";
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer(conStrOffline);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Car>(entity =>
        {
            entity.HasKey(e => e.CarId).HasName("PK__Cars__68A0340E99F3F89C");

            entity.Property(e => e.CarId).HasColumnName("CarID");
            entity.Property(e => e.CarcaseNumber).HasMaxLength(50);
            entity.Property(e => e.CarsStats).HasMaxLength(50);
            entity.Property(e => e.Color).HasMaxLength(50);
            entity.Property(e => e.EngineNumber).HasMaxLength(50);
            entity.Property(e => e.Photo).HasColumnType("image");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RegistrationNumber).HasMaxLength(20);
            entity.Property(e => e.ReleaseYear).HasColumnType("date");

            entity.HasOne(d => d.CarcaseType).WithMany(p => p.Cars)
                .HasForeignKey(d => d.CarcaseTypeId)
                .HasConstraintName("FK_Cars_CarcaseTypes");

            entity.HasOne(d => d.Manufacturer).WithMany(p => p.Cars)
                .HasForeignKey(d => d.ManufacturerId)
                .HasConstraintName("FK_Cars_Manufacturers");

            entity.HasOne(d => d.Model).WithMany(p => p.Cars)
                .HasForeignKey(d => d.ModelId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Cars_CarModels");

            entity.HasOne(d => d.SellerEmployee).WithMany(p => p.Cars)
                .HasForeignKey(d => d.SellerEmployeeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Cars_Employees");
        });

        modelBuilder.Entity<CarModel>(entity =>
        {
            entity.HasKey(e => e.ModelId).HasName("PK__CarModel__E8D7A1CC4914F59A");

            entity.Property(e => e.ModelId).HasColumnName("ModelID");
            entity.Property(e => e.ModelName).HasMaxLength(50);
        });

        modelBuilder.Entity<CarcaseType>(entity =>
        {
            entity.HasKey(e => e.CarcaseTypeId).HasName("PK__CarcaseT__DF470C9005A065A2");

            entity.Property(e => e.CarcaseTypeId).HasColumnName("CarcaseTypeID");
            entity.Property(e => e.TypeDescription).HasMaxLength(50);
            entity.Property(e => e.TypeName).HasMaxLength(50);
        });

        modelBuilder.Entity<CarsEquipment>(entity =>
        {
            entity.HasKey(e => e.CarEquipmentId).HasName("PK__CarsEqui__9A7D4868FC8281F9");

            entity.Property(e => e.CarEquipmentId).HasColumnName("CarEquipmentID");

            entity.HasOne(d => d.Car).WithMany(p => p.CarsEquipments)
                .HasForeignKey(d => d.CarId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_CarsEquipments_Cars");

            entity.HasOne(d => d.Equipment).WithMany(p => p.CarsEquipments)
                .HasForeignKey(d => d.EquipmentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_CarsEquipments_ExtraEquipments");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.ClientId).HasName("PK__Clients__E67E1A04461EBDF4");

            entity.Property(e => e.ClientId).HasColumnName("ClientID");
            entity.Property(e => e.Adres).HasMaxLength(20);
            entity.Property(e => e.FamilyName).HasMaxLength(20);
            entity.Property(e => e.Name).HasMaxLength(20);
            entity.Property(e => e.OrderDate).HasColumnType("date");
            entity.Property(e => e.PassportData).HasMaxLength(50);
            entity.Property(e => e.Rrepayment).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SaleDate).HasColumnType("date");
            entity.Property(e => e.Surname).HasMaxLength(20);
            entity.Property(e => e.Telephone)
                .HasMaxLength(12)
                .IsUnicode(false);

            entity.HasOne(d => d.Car).WithMany(p => p.Clients)
                .HasForeignKey(d => d.CarId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Clients_Cars");

            entity.HasOne(d => d.ClientEmployee).WithMany(p => p.Clients)
                .HasForeignKey(d => d.ClientEmployeeId)
                .HasConstraintName("FK_Clients_Employees");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04FF1323FB488");

            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.PositionId).HasColumnName("PositionID");

            entity.HasOne(d => d.Position).WithMany(p => p.Employees)
                .HasForeignKey(d => d.PositionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Employees_Positions");
        });

        modelBuilder.Entity<ExtraEquipment>(entity =>
        {
            entity.HasKey(e => e.ExtraEquipmentId).HasName("PK__ExtraEqu__325C35DBDEDCE8CA");

            entity.Property(e => e.ExtraEquipmentId).HasColumnName("ExtraEquipmentID");
            entity.Property(e => e.EquipmentName).HasMaxLength(50);
            entity.Property(e => e.EquipmentStats).HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.HasKey(e => e.ManufacturerId).HasName("PK__Manufact__357E5CA1614190F2");

            entity.Property(e => e.ManufacturerId).HasColumnName("ManufacturerID");
            entity.Property(e => e.Adres).HasMaxLength(50);
            entity.Property(e => e.Country).HasMaxLength(50);
            entity.Property(e => e.ExtraDescription).HasMaxLength(50);
            entity.Property(e => e.ManufacturerName).HasMaxLength(50);
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasKey(e => e.PositionId).HasName("PK__Position__60BB9A59D317328C");

            entity.Property(e => e.PositionId).HasColumnName("PositionID");
            entity.Property(e => e.PositionName).HasMaxLength(50);
            entity.Property(e => e.Salary).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<ViewEmployeeAndPosition>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_EmployeeAndPositions");

            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.PositionId).HasColumnName("PositionID");
            entity.Property(e => e.PositionName).HasMaxLength(50);
            entity.Property(e => e.Salary).HasColumnType("decimal(18, 2)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
