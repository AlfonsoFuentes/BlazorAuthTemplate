using Server.Interfaces.EndPoints;

using Scalar.AspNetCore;

using Server.Services;

namespace Server.Services
{
    public static class AppBuilder
    {
        public static WebApplication UseApp(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors("AllowBlazorWasm");
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapEndPoint();
            app.UseEndpoints();

            return app;
        }
        internal static IApplicationBuilder MapEndPoint(this WebApplication app)
        {
            try
            {
                var TransientServices = app.Services;
                IEnumerable<IEndPoint> endpoints = app.Services.GetRequiredService<IEnumerable<IEndPoint>>();



                foreach (IEndPoint row in endpoints)
                {
                    row.MapEndPoint(app);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }


            return app;
        }

        internal static IApplicationBuilder UseEndpoints(this IApplicationBuilder app)
        {

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");

            });
            return app;
        }

    }
}
