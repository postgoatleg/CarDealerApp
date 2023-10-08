using System;
using System.Collections.Generic;

namespace ConsoleApp;

public partial class ExtraEquipment
{
    public int ExtraEquipmentId { get; set; }

    public string? EquipmentName { get; set; }

    public string? EquipmentStats { get; set; }

    public decimal? Price { get; set; }

    public virtual ICollection<CarsEquipment> CarsEquipments { get; set; } = new List<CarsEquipment>();
}
