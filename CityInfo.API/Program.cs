using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using CityInfo.API;
using CityInfo.API.Filters;
using CityInfo.API.Services;
using CityInfo.API.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;


// Configuer Logging using 3rd party(serilog)
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("Log/logs.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

//-----------------------------------------------------------------------
 
builder.Host.UseSerilog();
// Add console logging provider
//builder.Logging.ClearProviders(); // logging providers already configured internally when calling CreateBuilder
//builder.Logging.AddConsole();

// Add services to the container.
//-----------------------------------------------------------------------

builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
    //----------------------------------Add The Filter Validator-------------------------------------

    options.Filters.Add<ValidatorFilter>();
})
.AddNewtonsoftJson()
.AddXmlDataContractSerializerFormatters();
//----------------------------------Fluent Validation-------------------------------------

builder.Services.AddFluentValidationAutoValidation();

//-----------------------------------------------------------------------


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//-----------------------------------------------------------------------


builder.Services.AddSingleton<FileExtensionContentTypeProvider>();
builder.Services.AddProblemDetails();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<ICityInfoRepositiory, CityInfoRepositiory>();
builder.Services.AddDbContext<CityInfoContext>
    (options => options.UseSqlite(builder
    .Configuration["ConnectionStrings:CityInfoConnectionString"]));

//-----------------------------------------------------------------------



builder.Services.AddAuthentication("Bearer").AddJwtBearer(options =>

   options.TokenValidationParameters = new()
   {
       ValidateIssuer = true,
       ValidateAudience = true,
       ValidateIssuerSigningKey = true,
       ValidAudience = builder.Configuration["Authentication:Audience"],
       ValidIssuer = builder.Configuration["Authentication:ISSuer"],
       IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(builder.Configuration["Authentication:SercretKey"]))
   }
    );
//-----------------------------------------------------------------------



builder.Services.AddApiVersioning(setupAction =>
{
    setupAction.ReportApiVersions = true;
    setupAction.AssumeDefaultVersionWhenUnspecified = true;
    setupAction.DefaultApiVersion = new ApiVersion(1, 0);
}).AddMvc().AddApiExplorer(
    setupAction =>
    {
        setupAction.SubstituteApiVersionInUrl = true;
    });
//--------------------------------Fluent Validation---------------------------------------
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
//builder.Services.AddMvc()
//.AddFluentValidation(mvc => 
//mvc.RegisterValidatorsFromAssemblyContaining<Startup>());

//-----------------------------------------------------------------------

var apiVersionDescriptionProvider = builder.Services.BuildServiceProvider()
    .GetRequiredService<IApiVersionDescriptionProvider>();

builder.Services.AddSwaggerGen(setupAction =>
{
    // one foreach version
    foreach (var des in apiVersionDescriptionProvider.ApiVersionDescriptions)
    {
        setupAction.SwaggerDoc(
            $"{des.GroupName}", new()
            {
                Title = "City Info API",
                Version = des.ApiVersion.ToString(),
                Description = "Througth this API you can access all cities!"
            });
    }

    //include authentication

    setupAction.AddSecurityDefinition("API Authentication", new()
    {
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        Description = "Input a valid Token to be authenticated"
    });


    // to make it send the token in the Auth Header
    setupAction.AddSecurityRequirement(
        new()
        {
          {     new()
                {
                    Reference = new OpenApiReference()
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "API Authentication"
                    }
                },
               new List<string>()
          }
        }
        );

    // include xml comments in the open api spec
    var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
    setupAction.IncludeXmlComments(xmlCommentsFullPath);
});
//-----------------------------------------------------------------------







//builder.Services.AddProblemDetails(options =>
//{
//    options.CustomizeProblemDetails = ctx =>
//    {
//        ctx.ProblemDetails.Extensions.Add("Server", Environment.MachineName);
//    };
//});

// Custom Service

#if DEBUG 
builder.Services.AddTransient<IMailService, LocalMailService>();
#else 
builder.Services.AddTransient<IMailService,CloudMailService>();
#endif
//-----------------------------------------------------------------------

builder.Services.AddSingleton<CityDataStore>();
builder.Services.AddAuthorization(
    options => options.AddPolicy("MustBeFromCity",
     p =>
     {
         p.RequireAuthenticatedUser();
         p.RequireClaim("city", "Antwerp");
     }
    )
    );

builder.Services.Configure<ForwardedHeadersOptions>(options =>
options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
ForwardedHeaders.XForwardedProto
);

//-----------------------------------------------------------------------
//-----------------------------------------------------------------------

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    // app.UseDeveloperExceptionPage();
    app.UseExceptionHandler();
}
//-----------------------------------------------------------------------

app.UseForwardedHeaders();
//-----------------------------------------------------------------------

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(
        setupAction =>
        {
            var descriptions = app.DescribeApiVersions();
            foreach (var desc in descriptions)
            {
                setupAction.SwaggerEndpoint(
                   $"/swagger/{desc.GroupName}/swagger.json",
                     desc.GroupName.ToUpperInvariant()
                    );
            }
        });
}
//-----------------------------------------------------------------------

app.UseHttpsRedirection();



app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

//app.MapControllers();
app.Run(

    async (httpContext) => await httpContext.Response.WriteAsync("HELLO")

    );

app.Run();
