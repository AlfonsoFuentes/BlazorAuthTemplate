using Shared.Attributtes;
using Shared.Enums.BudgetCategorys;
using System.ComponentModel;
using System.Reflection;

namespace Shared.ExtensionsMethods
{
    public static class ExtensionMethods
    {
        // ✅ Versión mejorada
        public static string GetIcon<TEnum>(this TEnum value) where TEnum : struct, Enum
        {
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString());

            if (memberInfo.Length > 0)
            {
                var attributes = memberInfo[0].GetCustomAttributes(typeof(UiIconAttribute), false);
                if (attributes.Length > 0)
                {
                    return ((UiIconAttribute)attributes[0]).IconClass;
                }
            }
            return ""; // Retorna vacío si no hay icono
        }

        public static string GetDescription<TEnum>(this TEnum value) where TEnum : struct, Enum
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? value.ToString();
        }
        public static string ToCurrencyCulture(this decimal value)
        {
            return value.ToString("C0", new System.Globalization.CultureInfo("en-US"));
        }
        public static string ToCurrencyCulture(this double value)
        {
            return value.ToString("C0", new System.Globalization.CultureInfo("en-US"));
        }
        public static string GetLetter(this BudgetCategory value)
        {
            return GetAttribute(value)?.Letter ?? "?";
        }

        public static string GetDescription(this BudgetCategory value)
        {
            return GetAttribute(value)?.Name ?? value.ToString();
        }

        public static string ToUiString(this BudgetCategory value)
        {
            var attr = GetAttribute(value);
            return attr != null ? $"{attr.Letter}-{attr.Name}" : value.ToString();
        }

        // Helper genérico para extraer el atributo
        private static BudgetMetadataAttribute? GetAttribute(BudgetCategory value)
        {
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString());
            if (memberInfo.Length > 0)
            {
                var attributes = memberInfo[0].GetCustomAttributes(typeof(BudgetMetadataAttribute), false);
                if (attributes.Length > 0)
                {
                    return (BudgetMetadataAttribute)attributes[0];
                }
            }
            return null;
        }
        public static bool IsSpecialCalculation(this BudgetCategory category)
        {
            return category == BudgetCategory.Tax ||
                   category == BudgetCategory.Engineering ||
                   category == BudgetCategory.Contingency;
        }
    }
}
