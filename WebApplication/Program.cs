
using ConsoleApp;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebApp;
using WebApp.Services;
using Car = ConsoleApp.Car;


var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

string connection = builder.Configuration.GetConnectionString("SqlServerConnection");
services.AddDbContext<CarDealershipContext>(options => options.UseSqlServer(connection));

services.AddMemoryCache();

services.AddDistributedMemoryCache();
services.AddSession();


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
<button type='button' onclick='location.href=""/searchform1"";'>Форма session</button>
<button type='button' onclick='location.href=""/searchform2"";'>Форма cookie</button>
</BODY>"));
app.Map("/cars", Cars);
app.Map("/emp", Employers);
app.Map("/searchform1", Forms);
app.Map("/searchform2", FormsCookie);

static void Forms(IApplicationBuilder app)
{
    app.Run(async (context) =>
    {
        const string key = "price";

        int price = -1;

        if (context.Request.Method == "POST")
        {
            try
            {
                string s = context.Request.Form[key];
                int.TryParse(s, out price);
                context.Session.Set(key, price);
                Debug.Write(context.Session.GetInt32(key));
            }
            catch (Exception ex) { Debug.WriteLine(ex); }
        }
        else
        {

            if (context.Session.Keys.Contains(key)) Debug.WriteLine("f");

            else
            {
                context.Session.SetInt32(key, 1);
            }
        }


        string strResponse = "<HTML><HEAD><TITLE>Пользователь</TITLE></HEAD>" +
        "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
        "<BODY><FORM action ='/searchform1' method='post'/ >" +
        //$"{context.Request.Method}" +
        "Минимальная зарплата:<BR><INPUT type = 'number' name = 'price' value = " + price + ">" +
        //"<BR>Фамилия:<BR><INPUT type = 'text' name = 'LastName' value = " + form.LastName + " >" +
        "<BR><BR><INPUT type ='submit' value='Сохранить в Session'>" +
        //"<INPUT type ='submit' value='Показать'>" +
        "</FORM>";
        strResponse += "<BR><A href='/'>Главная</A></BODY></HTML>";
        
        
        //price = context.Session.Get<int>(key);
        //user.LastName = context.Request.Query["LastName"];
        
        

        // Асинхронный вывод динамической HTML формы
        await context.Response.WriteAsync(strResponse);
    }
    );
}

static void FormsCookie(IApplicationBuilder app)
{
    app.Run(async (context) =>
    {
        const string key = "name";

        string name = "";
        CookieOptions options = new CookieOptions { Expires = DateTime.Now.AddDays(1) };
        if (context.Request.Method == "POST")
        {
            try
            {
                name = context.Request.Form[key]; 
                context.Response.Cookies.Append(key, name, options);
                
            }
            catch (Exception ex) { Debug.WriteLine(ex); }
        }
        else
        {
            if (context.Request.Cookies.ContainsKey(key)) name = context.Request.Cookies[key];
            else
            {
                context.Response.Cookies.Append(key, "no name", options);
            }
        }

        string strResponse = "<HTML><HEAD><TITLE>Пользователь</TITLE></HEAD>" +
        "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
        "<BODY><FORM action ='/searchform2' method='post'/ >" +
        //$"{context.Request.Method}" +
        "Название машины:<BR><INPUT type = 'text' name = 'name' value = " + name + ">" +
        "<BR><BR><INPUT type ='submit' value='Сохранить в cookie'>" +
        "</FORM>";
        strResponse += "<BR><A href='/'>Главная</A></BODY></HTML>";
        await context.Response.WriteAsync(strResponse);
    }
    );
}


//static void MainPage(IApplicationBuilder app)
//{
//    app.Run(async (context) => { await context.Response.WriteAsync("<BR><A href='/cars'>Машины</A></BR>"); });
//}
static void Cars(IApplicationBuilder app)
{
    app.Run(async (context) =>
    {
        ICachedService<Car> cachedCarsService = context.RequestServices.GetService<ICachedService<Car>>();
        IEnumerable<Car> cars = cachedCarsService.Get("cars20");
        string HtmlString = "<HTML><HEAD><TITLE>Машины</TITLE>  <link href='Styles/style.css' rel='stylesheet'/> </HEAD>" +
        "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
        "<BODY><H1>Список машин</H1>" +
        "<TABLE BORDER=1  class='table'>";
        HtmlString += "<TR>";
        HtmlString += "<TH>регистрационный номер</TH>";
        HtmlString += "<TH>цена</TH>";
        HtmlString += "<TH>год выпуска</TH>";
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
        HtmlString += "<BR><A href='/'>Главная страница</A></BR>";
        HtmlString += "</BODY></HTML>";

        await context.Response.WriteAsync(HtmlString);
    });
}

static void Employers(IApplicationBuilder app)
{
    Position pos;
    app.Run(async (context) =>
    {
        ICachedService<Employee> cachedEmpService = context.RequestServices.GetService<ICachedService<Employee>>();
        IEnumerable<Employee>? employers = cachedEmpService.Get();
        string HtmlString = "<HTML><HEAD><TITLE>Сотрудники</TITLE>  <link href='Styles/style.css' rel='stylesheet'/> </HEAD>" +
        "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
        "<BODY><H1>Список сотрудников</H1>" +
        "<TABLE BORDER=1  class='table'>";
        HtmlString += "<TR>";
        HtmlString += "<TH>возраст</TH>";
        HtmlString += "<TH>имя</TH>";
        HtmlString += "<TH>фамилия</TH>";
        HtmlString += "<TH>должность</TH>";
        HtmlString += "<TH>зарплата</TH>";
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
        HtmlString += "<BR><A href='/'>Главная страница</A></BR>";
        HtmlString += "</BODY></HTML>";

        await context.Response.WriteAsync(HtmlString);
    });
}


app.Map("/info", (appBuilder) =>
{
    appBuilder.Run(async (context) =>
    {
        string strResponse = "<HTML><HEAD><TITLE>Информация</TITLE></HEAD>" +
        "<META http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
        "<BODY><H1>Информация:</H1>";
        strResponse += "<BR> хост: " + context.Request.Host;
        strResponse += "<BR> путь: " + context.Request.PathBase;
        strResponse += "<BR> протокол: " + context.Request.Protocol;
        strResponse += "<BR><A href='/'>Главная страница</A></BODY></HTML>";
        await context.Response.WriteAsync(strResponse);
    });
});

app.Run();
