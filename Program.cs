using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using QLDEducationalWeatherDataAPI.Models.Repositories;
using QLDEducationalWeatherDataAPI.Services;
using QLDEducationalWeatherDataAPI.Settings;
using System.Reflection;

namespace QLDEducationalWeatherDataAPI
{
    /// <summary>
    /// Represents the entry point of the program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main method where the program execution begins.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            //Generates the full file path of our XML file based upon the project directory location and the file's name.
            //The file's name will be our project name with a .xml extension added to it.
            var filepath = Path.Combine(AppContext.BaseDirectory, "QLDEducationalWeatherDataAPI.xml");

            //Adds swagger to our project. The options set inside the method call configure addtional
            //parameters to allow for better Swagger API documentation.
            builder.Services.AddSwaggerGen(s =>
            {
                //Sets up the headings and version data in our Swagger Docs.
                s.SwaggerDoc("v1", new OpenApiInfo { Title = "Educational Weather Data System", Version = "v1" });
                //Tells swagger to read our XML comments and tranfer some of the contents into the API docs.
                s.IncludeXmlComments(filepath);

                s.AddSecurityDefinition("apiKey", new OpenApiSecurityScheme
                {
                    Description = @"API Key to manage user access. 
                                        Enter you provided key into the Box below.",
                    Name = "apiKey",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                s.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            //Reference the UI component we added above to store the key.
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "apiKey"
                            },
                            //Tells it what to call the key parameter and where to put it for each request.
                            Name = "apiKey",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });

            // Sets up our options pattern which maps the settings for the mongo connection string to the mongo settings
            // class whe the program first starts up. This makes access faster than reading directly from the file
            // when we need the connetion string details.
            builder.Services.Configure<MongoConnectionSettings>(builder.Configuration.GetSection("ConnString"));
            // Add our connection builder class to the service container so we can access it from wherever we need it.
            builder.Services.AddScoped<MongoConnectionBuilder>();
            //Adds the repository class to the services by providing the
            //interface so that we can request it using the interface type
            //instead of the class name. This makes it easier to change this
            //class for an alternatie (SQL) without changing all the rest of our code.
            builder.Services.AddScoped<IWeatherRepository, WeatherRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            //This AddCORS method is used to help us configure a Cross-Origin Resource Sharing (CORS) policies.
            builder.Services.AddCors(c =>
            {
                //AddPOLICY method is used for us to define a CORS policy like "GOOGLE" and you can add more different policy if its required.
                c.AddPolicy("Google", p =>
                {
                    //This WithOrigins method specifies the allowed communication origins for requests.
                    p.WithOrigins("https://www.google.com", "https://www.google.com.au");
                    //This AllowAnyHeader method allows any header to be sent with the request.
                    p.AllowAnyHeader();
                    //This WithMethods method specifies the allowed HTTP methods for sending allowed requests.
                    p.WithMethods("GET", "POST", "PUT", "DELETE");
                });
                //You can still add more policy in this section using the same method from the above.
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseCors("Google");

            app.MapControllers();

            app.Run();
        }
    }
}
