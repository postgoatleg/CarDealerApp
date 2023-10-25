using System;
using System.Collections.Generic;

namespace ConsoleApp;

public partial class Client
{
    public int ClientId { get; set; }

    public string? FamilyName { get; set; }

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public string? Adres { get; set; }

    public string? Telephone { get; set; }

    public string? PassportData { get; set; }

    public int? CarId { get; set; }

    public DateTime? OrderDate { get; set; }

    public DateTime? SaleDate { get; set; }

    public bool? IsSold { get; set; }

    public bool? IsPayed { get; set; }

    public decimal? Rrepayment { get; set; }

    public int? ClientEmployeeId { get; set; }

    public virtual Car? Car { get; set; }

    public virtual Employee? ClientEmployee { get; set; }
}
