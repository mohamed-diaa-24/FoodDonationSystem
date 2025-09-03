using FoodDonationSystem.API.Seed;

namespace FoodDonationSystem.API.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication UseSwaggerDocumentation(this WebApplication app)
        {

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Qoot API V1");
                c.RoutePrefix = "swagger";
                c.DefaultModelsExpandDepth(-1);
                c.DefaultModelExpandDepth(-1);
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                c.DisplayRequestDuration();
            });


            return app;
        }

        public static WebApplication UseSecurityHeaders(this WebApplication app)
        {
            app.Use(async (context, next) =>
            {

                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
                context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");

                context.Response.Headers.Remove("Server");

                await next();
            });

            return app;
        }


        public static WebApplication UseRequestLogging(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.Use(async (context, next) =>
                {
                    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

                    logger.LogInformation("Request: {Method} {Path} at {Time}",
                        context.Request.Method,
                        context.Request.Path,
                        DateTime.UtcNow);

                    await next();

                    logger.LogInformation("Response: {StatusCode} for {Method} {Path}",
                        context.Response.StatusCode,
                        context.Request.Method,
                        context.Request.Path);
                });
            }

            return app;
        }


        public static WebApplication UseGlobalExceptionHandler(this WebApplication app)
        {
            app.Use(async (context, next) =>
            {
                try
                {
                    await next();
                }
                catch (Exception ex)
                {
                    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An unhandled exception occurred");

                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";

                    var response = new
                    {
                        error = "A server error has occurred",
                        message = app.Environment.IsDevelopment() ? ex.Message : "Please try again later.",
                        statusCode = 500
                    };

                    await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
                }
            });

            return app;
        }


        public static WebApplication UseCorsPolicy(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseCors("AllowAll");
            }
            else
            {
                app.UseCors("Production");
            }

            return app;
        }

        public static WebApplication UseAuthenticationAndAuthorization(this WebApplication app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
            return app;
        }


        public static WebApplication UseApiEndpoints(this WebApplication app)
        {
            app.MapControllers();
            // Add API info endpoint
            app.MapGet("/", () => new
            {
                name = "Qoot API",
                version = "1.0.0",
                description = "Food Donation Management System",
                documentation = "/swagger"
            }).ExcludeFromDescription();

            return app;
        }

        public static async Task<WebApplication> SeedDataAsync(this WebApplication app)
        {
            try
            {
                var logger = app.Services.GetRequiredService<ILogger<Program>>();
                await DataSeeder.SeedRoles(app.Services, logger);

                logger.LogInformation("Data seeding completed successfully");
            }
            catch (Exception ex)
            {
                var logger = app.Services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while seeding data");
            }

            return app;
        }


        public static async Task<WebApplication> ConfigurePipelineAsync(this WebApplication app)
        {

            app.UseGlobalExceptionHandler();

            app.UseStaticFiles();

            app.UseSecurityHeaders();


            app.UseSwaggerDocumentation();


            app.UseRequestLogging();


            app.UseHttpsRedirection();


            app.UseCorsPolicy();


            app.UseAuthenticationAndAuthorization();


            app.UseApiEndpoints();


            await app.SeedDataAsync();

            return app;
        }
    }
}
