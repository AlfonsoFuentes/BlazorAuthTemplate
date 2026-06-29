using Scalar.AspNetCore;
using Server.Interfaces.EndPoints;
using Server.Services;
using System.Net;

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
     
            app.MapStaticAssets();

            app.UseRouting();
            app.UseCors("AllowBlazorWasm");
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapEndPoint();
            app.MapRazorPages();
            app.MapFallbackToFile("index.html");
            app.MapControllers();
            return app;
        }
        internal static IApplicationBuilder MapEndPoint(this WebApplication app)
        {
            // ✅ Todos los minimal endpoints requieren autenticación por defecto.
            // Los controllers de autenticación (Login/Register) usan [AllowAnonymous] y no se ven afectados.
            var apiGroup = app.MapGroup("").RequireAuthorization();

            // Usamos un Scope para asegurar la resolución de servicios
            using (var scope = app.Services.CreateScope())
            {
                var endpoints = scope.ServiceProvider.GetServices<IEndPoint>();

                foreach (var endpoint in endpoints)
                {
                    #if DEBUG
                    // 🛠️ MODO DESARROLLO (Debug)
                    // Aquí SI tenemos Try-Catch para que puedas inspeccionar el error
                    try
                    {
                        endpoint.MapEndPoint(apiGroup);
                    }
                    catch (Exception ex)
                    {
                        // Este comando hace que Visual Studio se detenga aquí AUTOMÁTICAMENTE
                        // como si hubieras puesto un Breakpoint rojo (Punto de interrupción).
                        // Es genial porque te lleva directo a la línea del error.
                        System.Diagnostics.Debugger.Break();

                        // Puedes inspeccionar 'msg' pasando el mouse por encima
                        string msg = ex.Message;
                        Console.WriteLine($"Error mapping {endpoint.GetType().Name}: {msg}");

                        // Opcional: Si quieres que siga intentando con los otros endpoints
                        // no pongas 'throw'. Si quieres que pare, pon 'throw'.
                    }
                    #else
                        // 🚀 MODO PRODUCCIÓN (Release)
                        // Aquí NO hay Try-Catch. Si falla, la aplicación se detiene inmediatamente.
                        // Esto es vital para que el servidor sepa que el despliegue falló.
                        endpoint.MapEndPoint(apiGroup);
                    #endif
                }
            }

            return app;
        }

        internal static IApplicationBuilder UseEndpoints(this IApplicationBuilder app)
        {

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
             

            });
            return app;
        }

    }
}
