using ETicaretAPI.Application;
using ETicaretAPI.Application.Validators.Product;
using ETicaretAPI.Infrastructure;
using ETicaretAPI.Infrastructure.Filters;
using ETicaretAPI.Infrastructure.Services.Storage.Azure;
using ETicaretAPI.Infrastructure.Services.Storage.Local;
using ETicaretAPI.Persistence;
using ETicaretAPI.Persistence.Contexts;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Serilog.Core;
using Serilog;
using NpgsqlTypes;
using Serilog.Sinks.PostgreSQL;
using Serilog.Context;
using ETicaretAPI.API.Configurations.ColumnWriters;
using Microsoft.AspNetCore.HttpLogging;
using ETicaretAPI.API.Extensions;
using ETicaretAPI.SignalR;
using ETicaretAPI.API.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpContextAccessor();//Client'tan gelen request neticvesinde oluþturulan HttpContext nesnesine katmanlardaki class'lar üzerinden(busineess logic) eriþebilmemizi saðlayan bir servistir. //baskets
builder.Services.AddPersistanceServices();
builder.Services.AddInfrastructureServices();
builder.Services.AddApplicationServices(); // media
builder.Services.AddSignalRServices(); //SignalR

//builder.Services.AddStorage<LocalStorage>();
builder.Services.AddStorage<AzureStorage>();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.WithOrigins("http://localhost:4200", "https://localhost:4200").AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

//serilog yapılandırması
Logger log = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt")
    .WriteTo.PostgreSQL(builder.Configuration.GetConnectionString("PostgreSQL"), "logs", needAutoCreateTable: true,
    columnOptions: new Dictionary<string, ColumnWriterBase>
        {
            {"message", new RenderedMessageColumnWriter(NpgsqlDbType.Text)},
            {"message_template", new MessageTemplateColumnWriter(NpgsqlDbType.Text)},
            {"level", new LevelColumnWriter(true , NpgsqlDbType.Varchar)},
            {"time_stamp", new TimestampColumnWriter(NpgsqlDbType.Timestamp)},
            {"exception", new ExceptionColumnWriter(NpgsqlDbType.Text)},
            {"log_event", new LogEventSerializedColumnWriter(NpgsqlDbType.Json)},
            {"user_name", new UsernameColumnWriter()}
        })
    .WriteTo.Seq(builder.Configuration["Seq:ServerURL"])
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .CreateLogger();

builder.Host.UseSerilog(log); //serilog
builder.Services.AddHttpLogging(logging =>//serilog
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestHeaders.Add("sec-ch-ua"); //kullancıya dair bütün bilgileri getirir
    logging.MediaTypeOptions.AddText("application/javascript");
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});

//fluentvalidation kullanımının sağlanması
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
    options.Filters.Add<RolePermissionFilter>();
})
    .AddFluentValidation(configuration => configuration.RegisterValidatorsFromAssemblyContaining<CreateProductValidator>())
    .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//JWT DOĞRULAMASI
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Admin", options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateAudience = true, // Oluşturulacak token değerini  kimlerin/hangi originlerin/sitelerin kullanıcı belirlediğimizdeğerdir. --> www.bilmemne.com
            ValidateIssuer = true, //Oluşturulacak token değerini kimin dağıttığını ifade edeceğimiz alandır. --> www.myapi.com
            ValidateLifetime = true, //Oluşturulacak token değerinin süresini kontrol edecek olan doğrulamadır.
            ValidateIssuerSigningKey = true,  //Üretilecek token değerinin uygulamamıza ait bir değer olduğunu ifade eden security key verisinin doğrulanmasıdır.

            ValidAudience = builder.Configuration["Token:Audience"],
            ValidIssuer = builder.Configuration["Token:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"])),
            LifetimeValidator = ( notBefore,  expires,  securityToken,  validationParameters) => expires !=null ? expires > DateTime.UtcNow : false, // tokeinin süresi bittiği andatoken kapanacak
            
            NameClaimType = ClaimTypes.Name //JWT Üzerinden Nameclaime karşılık gelen değeri user.identiy.name proptan elde edebiliriz
        
        };
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.ConfigureExceptionHandler<Program>(app.Services.GetRequiredService<ILogger<Program>>());

app.UseStaticFiles();//angular
app.UseSerilogRequestLogging();//serilog //bundan sonraki bilgiler loglanacak üstteki loglanmayacak
app.UseHttpLogging(); //serilog
app.UseCors(); //angular

app.UseHttpsRedirection();

app.UseAuthentication(); // brysel eklendi Authentication için
app.UseAuthorization();
app.Use(async (context, next) =>  //serilog ayarlaması
{

    var username = context.User?.Identity?.IsAuthenticated != null || true ? context.User.Identity.Name : null;
    LogContext.PushProperty("user_name", username);
    await next();
});

app.MapControllers();
app.MapHubs();

app.Run();
