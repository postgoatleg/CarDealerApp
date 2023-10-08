using System;
using System.Collections.Generic;

namespace ConsoleApp;

public partial class Car
{
    public int CarId { get; set; }

    public string? RegistrationNumber { get; set; }

    public int? ModelId { get; set; }

    public int? ManufacturerId { get; set; }

    public byte[]? Photo { get; set; }

    public int? CarcaseTypeId { get; set; }

    public DateTime? ReleaseYear { get; set; }

    public string? Color { get; set; }

    public string? CarcaseNumber { get; set; }

    public string? EngineNumber { get; set; }

    public string? CarsStats { get; set; }

    public decimal? Price { get; set; }

    public int? SellerEmployeeId { get; set; }

    public virtual CarcaseType? CarcaseType { get; set; }

    public virtual ICollection<CarsEquipment> CarsEquipments { get; set; } = new List<CarsEquipment>();

    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();

    public virtual Manufacturer? Manufacturer { get; set; }

    public virtual CarModel? Model { get; set; }

    public virtual Employee? SellerEmployee { get; set; }
}
