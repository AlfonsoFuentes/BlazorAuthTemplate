namespace CllientMudBlazor.Services.HttPServives
{
    public static  class StringExtensions
    {
        // ✅ Helper para truncar mensajes largos
        public static string Truncate(this string value, int maxLength) =>
               string.IsNullOrEmpty(value) ? value :
               value.Length <= maxLength ? value : value[..maxLength] + "...";
    }
}
