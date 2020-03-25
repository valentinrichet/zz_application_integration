using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using AiCandidate.Repositories;
using AiCandidate.Repositories.Database;
using AiCandidate.Business;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics;
using System.Reflection;
using System.IO;
using AiCandidate.Options;
using AiCandidate.Services.PasswordHasher;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AiCandidate.Services.JwtGenerator;
using Microsoft.AspNetCore.Authorization;
using AiCandidate.Dbo;
using AiCandidate.Business.Candidates;

namespace AiCandidate
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            IConfigurationSection jwtSection = Configuration.GetSection(Settings.JWT);
            IConfigurationSection swaggerSection = Configuration.GetSection(Settings.SWAGGER);
            JwtOptions jwtOptions = jwtSection.Get<JwtOptions>();
            SwaggerOptions swaggerOptions = swaggerSection.Get<SwaggerOptions>();

            /* Configure Database */
            services.AddDbContext<CandidateContext>(optionsBuilder =>
            {
                optionsBuilder.UseMySql(Configuration.GetConnectionString(Settings.CONNECTION_STRING_URL), x => x.ServerVersion(Configuration.GetConnectionString(Settings.CONNECTION_STRING_VERSION)));
            });
            /* ****************** */

            /* Configure Jwt Authentication */
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
           .AddJwtBearer(x =>
           {
               x.RequireHttpsMetadata = false;
               x.SaveToken = true;
               x.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuerSigningKey = true,
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.IssuerKey)),
                   ValidateIssuer = false,
                   ValidateAudience = false,
               };
           });
            /* **************************** */

            /* Configure Swagger */
            services.AddSwaggerGen(c =>
                 {
                     c.SwaggerDoc("v1", new OpenApiInfo
                     {
                         Version = swaggerOptions.Version,
                         Title = swaggerOptions.Title,
                         Description = swaggerOptions.Description,
                         Contact = new OpenApiContact
                         {
                             Name = swaggerOptions.Contact.Name,
                             Email = swaggerOptions.Contact.Email
                         }
                     });

                     c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                     {
                         Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                         Name = "Authorization",
                         BearerFormat = "JWT",
                         Scheme = "bearer",
                         In = ParameterLocation.Header,
                         Type = SecuritySchemeType.Http
                     });

                     c.AddSecurityRequirement(new OpenApiSecurityRequirement
                     {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                            },
                            new List<string>()
                        }
                     });

                     // Set the comments path for the Swagger JSON and UI.gfdgdfnb !:
                     string xmlFile = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";
                     string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                     c.IncludeXmlComments(xmlPath);
                 });
            /* ***************** */

            /* Register Options */
            services.AddOptions<JwtOptions>().Bind(jwtSection);
            services.AddOptions<HashingOptions>().Bind(Configuration.GetSection("Hash"));
            /* ******************* */

            /* Register Services */
            services.AddScoped<IJwtGenerator, JwtGenerator>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            /* ******************* */

            /* Register Automapper */
            services.AddAutoMapper(typeof(Startup));
            /* ******************* */

            /* Register Repositories */
            services.AddScoped<IRepository<Candidate>, DbRepository<Candidate, CandidateContext>>();
            services.AddScoped<IRepository<Skill>, DbRepository<Skill, CandidateContext>>();
            /* ********************* */

            /* Register Business */
            services.AddScoped<ICandidateBusiness<Candidate>, DbCandidateBusiness>();
            services.AddScoped<IBusiness<Skill>, Business<Skill, IRepository<Skill>>>();
            /* ***************** */

            /* Configure Authorization */
            services.AddAuthorization();
            /* *********************** */

            /* Configure Cors */
            services.AddCors();
            /* ************** */

            /* Configure Controllers */
            services.AddControllers();
            /* ********************* */

            /* Configure InvalidModelStateResponseFactory */
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    ValidationProblemDetails problemDetails = new ValidationProblemDetails(context.ModelState);
                    BadRequestObjectResult result = new BadRequestObjectResult(problemDetails.Errors);
                    return result;
                };
            });
            /* ****************************************** */
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        IExceptionHandlerPathFeature exceptionHandler = context.Features.Get<IExceptionHandlerPathFeature>();
                        logger.LogError(exceptionHandler.Error, exceptionHandler.Path);
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("There was an error with the server, please try again later...");
                    });
                });
            }

            app.UseHttpsRedirection();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                SwaggerOptions swaggerOptions = Configuration.GetSection(Settings.SWAGGER).Get<SwaggerOptions>();

                c.SwaggerEndpoint(swaggerOptions.EndPoint, $"{swaggerOptions.Title} {swaggerOptions.Version}");
            });

            app.UseRouting();

            // Global Cors Policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
