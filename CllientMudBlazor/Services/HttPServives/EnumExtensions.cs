using CllientMudBlazor.Services.Enums;
using System.ComponentModel;
using System.Reflection;

namespace CllientMudBlazor.Services.HttPServives
{
    public static class EnumExtensions
    {
        public static string GetDescription(this DashBoardsStartTable value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attributes = field?.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            return attributes?.FirstOrDefault()?.Description ?? value.ToString().Replace('_', ' ');
        }

        // Opcional: método genérico (si más adelante quieres reusarlo con otros enums)
        public static string GetDescription<TEnum>(this TEnum value) where TEnum : struct, Enum
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? value.ToString().Replace('_', ' ');
        }
    }
}
