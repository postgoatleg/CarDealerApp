using System;
using System.Collections.Generic;

namespace ConsoleApp;

public partial class CarsEquipment
{
    public int CarEquipmentId { get; set; }

    public int? CarId { get; set; }

    public int? EquipmentId { get; set; }

    public virtual Car? Car { get; set; }

    public virtual ExtraEquipment? Equipment { get; set; }
}
