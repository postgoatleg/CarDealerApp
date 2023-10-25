// See https://aka.ms/new-console-template for more information
using ConsoleApp;
//using ConsoleApp.Models;
using System.Linq;



using (CarDealershipContext context = new CarDealershipContext())
{

    var manufacturers = context.Manufacturers.ToList();
    Console.WriteLine("Адреса всех производителей:");
    foreach (var manufacturer in manufacturers)
    {
        Console.WriteLine($"{manufacturer.ManufacturerName} have adress {manufacturer.Adres}");
    }

    var positions = context.Positions.Where(p => p.Salary < 15).ToList();
    Console.WriteLine("Должности с зарплатой меньше 15:");
    foreach (var pos in positions)
    {
        Console.WriteLine($"{pos.PositionName} - have salary {pos.Salary}");
    }

    var workers = from e in context.Employees
                  join pos in context.Positions
                  on e.PositionId equals pos.PositionId
                  select new
                  {
                      e.FirstName,
                      pos.PositionName,
                      pos.Salary
                  };
    workers = workers.Where(w => w.Salary > 85);
    Console.WriteLine("Должности работников:");

    foreach (var worker in workers)
    {
        Console.WriteLine($"{worker.FirstName} have {worker.PositionName} position");
    }
    Console.WriteLine("работники с зарплатой больше 85:");

    foreach (var worker in workers)
    {
        Console.WriteLine($"{worker.FirstName} have {worker.PositionName} position and salary = {worker.Salary}");
    }

    var mf = new Manufacturer() { Adres = "Gomel", Country = "Belarus", ManufacturerName="Selmashop", ExtraDescription="best tractors in Belarus" };
    context.Manufacturers.Add(mf);
    context.SaveChanges();

    var mfToDel = context.Manufacturers.Where(m => m.ManufacturerName == "Selmashop").First();
    context.Manufacturers.Remove(mfToDel);
    var posesToDel = context.Positions.Where( p => p.Salary < 5 );
    context.Positions.RemoveRange(posesToDel);
    context.SaveChanges();

    var posToUpd = context.Positions.Where(p => p.Salary < 10 );
    foreach (var pos in posToUpd)
    {
        pos.Salary += 5;
    }
    context.SaveChanges();

}
