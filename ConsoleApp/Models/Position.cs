using System;
using System.Collections.Generic;

namespace ConsoleApp;

public partial class Position
{
    public int PositionId { get; set; }

    public string? PositionName { get; set; }

    public decimal? Salary { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
