namespace CllientMudBlazor.Services.NotificationServices
{
   
    // ProjectNotificationService.cs
    public class ProjectNotificationService
    {
        // ✅ Usa Func<Task> para permitir async/await seguro
        public event Func<Task> OnProjectsChanged = () => Task.CompletedTask;

        // Opción 1: Síncrono y seguro — fire-and-forget con captura de excepción
        public void NotifyProjectsChanged()
        {
            _ = OnProjectsChanged().ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    // Opcional: loguea aquí si tienes ILogger
                    // _logger?.LogError(t.Exception, "Error in OnProjectsChanged");
                }
            }, TaskScheduler.Default);
        }

        // Opción 2 (alternativa): asíncrono explícito (útil si algún suscriptor es crítico)
        // public async Task NotifyProjectsChangedAsync()
        // {
        //     await OnProjectsChanged();
        // }
    }
}
