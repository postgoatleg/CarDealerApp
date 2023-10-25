
using ConsoleApp;
using Microsoft.AspNetCore.Html;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using WebApp.Services;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
// ��������� ����������� ��� ������� � �� � �������������� EF
string connection = builder.Configuration.GetConnectionString("SqlServerConnection");
services.AddDbContext<CarDealershipContext>(options => options.UseSqlServer(connection));
// ���������� �����������
services.AddMemoryCache();

// ���������� ��������� ������
services.AddDistributedMemoryCache();

services.AddSession();

// ��������� ����������� CachedTanksService
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
<button type='button' onclick='location.href=""/cars"";'>������</button>
<button type='button' onclick='location.href=""/emp"";'>����������</button>
</BODY>"));
app.Map("/cars", Cars);
app.Map("/emp", Employers);


static void MainPage(IApplicationBuilder app)
{
    app.Run(async (context) => { await context.Response.WriteAsync("<BR><A href='/cars'>������</A></BR>"); });
}
static void Cars(IApplicationBuilder app)
{
    app.Run(async (context) =>
    {
        //��������� � �������
        ICachedService<Car> cachedCarsService = context.RequestServices.GetService<ICachedService<Car>>();
        IEnumerable<Car> cars = cachedCarsService.Get("cars20");
        string HtmlString = "<HTML><HEAD><TITLE>������</TITLE>  <link href='Styles/style.css' rel='stylesheet'/> </HEAD>" +
        "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
        "<BODY><H1>������ �����</H1>" +
        "<TABLE BORDER=1  class='table'>";
        HtmlString += "<TR>";
        HtmlString += "<TH>��������������� �����</TH>";
        HtmlString += "<TH>����</TH>";
        HtmlString += "<TH>��� �������</TH>";
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
        HtmlString += "<BR><A href='/'>�������</A></BR>";
        HtmlString += "</BODY></HTML>";

        // ����� ������
        await context.Response.WriteAsync(HtmlString);
    });
}

static void Employers(IApplicationBuilder app)
{
    Position pos;
    app.Run(async (context) =>
    {
        //��������� � �������
        ICachedService<Employee> cachedEmpService = context.RequestServices.GetService<ICachedService<Employee>>();
        IEnumerable<Employee>? employers = cachedEmpService.Get();
        string HtmlString = "<HTML><HEAD><TITLE>����������</TITLE>  <link href='Styles/style.css' rel='stylesheet'/> </HEAD>" +
        "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
        "<BODY><H1>������ �����������</H1>" +
        "<TABLE BORDER=1  class='table'>";
        HtmlString += "<TR>";
        HtmlString += "<TH>�������</TH>";
        HtmlString += "<TH>���</TH>";
        HtmlString += "<TH>�������</TH>";
        HtmlString += "<TH>���������</TH>";
        HtmlString += "<TH>��������</TH>";
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
        HtmlString += "<BR><A href='/'>�������</A></BR>";
        HtmlString += "</BODY></HTML>";

        // ����� ������
        await context.Response.WriteAsync(HtmlString);
    });
}


app.Map("/info", (appBuilder) =>
{
    appBuilder.Run(async (context) =>
    {
        // ������������ ������ ��� ������ 
        string strResponse = "<HTML><HEAD><TITLE>����������</TITLE></HEAD>" +
        "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
        "<BODY><H1>����������:</H1>";
        strResponse += "<BR> ������: " + context.Request.Host;
        strResponse += "<BR> ����: " + context.Request.PathBase;
        strResponse += "<BR> ��������: " + context.Request.Protocol;
        strResponse += "<BR><A href='/'>�������</A></BODY></HTML>";
        // ����� ������
        await context.Response.WriteAsync(strResponse);
    });
});

app.Run();
