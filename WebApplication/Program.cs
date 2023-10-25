
using ConsoleApp;
using Microsoft.AspNetCore.Html;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using WebApp.Services;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
// внедрение зависимости для доступа к БД с использованием EF
string connection = builder.Configuration.GetConnectionString("SqlServerConnection");
services.AddDbContext<CarDealershipContext>(options => options.UseSqlServer(connection));
// добавление кэширования
services.AddMemoryCache();

// добавление поддержки сессии
services.AddDistributedMemoryCache();

services.AddSession();

// внедрение зависимости CachedTanksService
services.AddScoped<ICachedService<Car>, CachedCarsService>();
services.AddScoped<ICachedService<Employee>, CachedEmployersService>();
var app = builder.Build();
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Environment.CurrentDirectory, "Styles")),
    RequestPath = "/Styles"
});
app.UseSession();

app.Map("/", async (context) => await context.Response.WriteAsync(@"
<HEAD>
<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>   
<link href='Styles/style.css' rel='stylesheet'/> 
</HEAD>
<BODY id='main-body'>
<button type='button' onclick='location.href=""/cars"";'>Машины</button>
<button type='button' onclick='location.href=""/emp"";'>Сотрудники</button>
</BODY>"));
app.Map("/cars", Cars);
app.Map("/emp", Employers);


static void MainPage(IApplicationBuilder app)
{
    app.Run(async (context) => { await context.Response.WriteAsync("<BR><A href='/cars'>Машины</A></BR>"); });
}
static void Cars(IApplicationBuilder app)
{
    app.Run(async (context) =>
    {
        //обращение к сервису
        ICachedService<Car> cachedCarsService = context.RequestServices.GetService<ICachedService<Car>>();
        IEnumerable<Car> cars = cachedCarsService.Get("cars20");
        string HtmlString = "<HTML><HEAD><TITLE>Машины</TITLE>  <link href='Styles/style.css' rel='stylesheet'/> </HEAD>" +
        "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
        "<BODY><H1>Список машин</H1>" +
        "<TABLE BORDER=1  class='table'>";
        HtmlString += "<TR>";
        HtmlString += "<TH>Регистрационный номер</TH>";
        HtmlString += "<TH>Цена</TH>";
        HtmlString += "<TH>Год выпуска</TH>";
        HtmlString += "</TR>";
        foreach (var car in cars)
        {
            HtmlString += "<TR>";
            HtmlString += "<TD>" + car.RegistrationNumber + "</TD>";
            HtmlString += "<TD>" + car.Price + "</TD>";
            HtmlString += "<TD>" + ((DateTime)car.ReleaseYear).ToString("Y") + "</TD>";
            HtmlString += "</TR>";
        }
        HtmlString += "</TABLE>";
        HtmlString += "<BR><A href='/'>Главная</A></BR>";
        HtmlString += "</BODY></HTML>";

        // Вывод данных
        await context.Response.WriteAsync(HtmlString);
    });
}

static void Employers(IApplicationBuilder app)
{
    Position pos;
    app.Run(async (context) =>
    {
        //обращение к сервису
        ICachedService<Employee> cachedEmpService = context.RequestServices.GetService<ICachedService<Employee>>();
        IEnumerable<Employee>? employers = cachedEmpService.Get();
        string HtmlString = "<HTML><HEAD><TITLE>Сотрудники</TITLE>  <link href='Styles/style.css' rel='stylesheet'/> </HEAD>" +
        "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
        "<BODY><H1>Список сотрудников</H1>" +
        "<TABLE BORDER=1  class='table'>";
        HtmlString += "<TR>";
        HtmlString += "<TH>Возраст</TH>";
        HtmlString += "<TH>Имя</TH>";
        HtmlString += "<TH>Фамилия</TH>";
        HtmlString += "<TH>Должность</TH>";
        HtmlString += "<TH>Зарплата</TH>";
        HtmlString += "</TR>";
        foreach (var emp in employers)
        {
            pos = cachedEmpService.GetPos((int)emp.PositionId);
            HtmlString += "<TR>";
            HtmlString += "<TD>" + emp.Age + "</TD>";
            HtmlString += "<TD>" + emp.FirstName + "</TD>";
            HtmlString += "<TD>" + emp.LastName + "</TD>";
            HtmlString += "<TD>" + pos.PositionName + "</TD>";
            HtmlString += "<TD>" + pos.Salary + "</TD>";
            HtmlString += "</TR>";
        }
        HtmlString += "</TABLE>";
        HtmlString += "<BR><A href='/'>Главная</A></BR>";
        HtmlString += "</BODY></HTML>";

        // Вывод данных
        await context.Response.WriteAsync(HtmlString);
    });
}


app.Map("/info", (appBuilder) =>
{
    appBuilder.Run(async (context) =>
    {
        // Формирование строки для вывода 
        string strResponse = "<HTML><HEAD><TITLE>Информация</TITLE></HEAD>" +
        "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
        "<BODY><H1>Информация:</H1>";
        strResponse += "<BR> Сервер: " + context.Request.Host;
        strResponse += "<BR> Путь: " + context.Request.PathBase;
        strResponse += "<BR> Протокол: " + context.Request.Protocol;
        strResponse += "<BR><A href='/'>Главная</A></BODY></HTML>";
        // Вывод данных
        await context.Response.WriteAsync(strResponse);
    });
});

app.Run();
