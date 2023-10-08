using System;
using System.Collections.Generic;

namespace ConsoleApp;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public int? Age { get; set; }

    public int? PositionId { get; set; }

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();

    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();

    public virtual Position? Position { get; set; }
}
