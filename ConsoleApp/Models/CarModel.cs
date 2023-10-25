using System;
using System.Collections.Generic;

namespace ConsoleApp;

public partial class CarModel
{
    public int ModelId { get; set; }

    public string? ModelName { get; set; }

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
}
