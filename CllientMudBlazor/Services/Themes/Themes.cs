using MudBlazor;
using MudBlazor.Utilities;

namespace CllientMudBlazor.Themes;

public static class AppTheme
{
    public static MudTheme Create()
    {
        var paletteLight = new PaletteLight()
        {
            Primary = new MudColor("#e8dff5"),
           

            Secondary = new MudColor("#ddedea"),      // 🌿 Menta (Secondary)
          

            Tertiary = new MudColor("#fcf4dd"),       // 🍑 Melocotón (Tertiary)
           

            Success = new MudColor("#ddedea"),        // = Secondary
            Warning = new MudColor("#fcf4dd"),        // = Tertiary
            Error = new MudColor("#fce1e4"),
            Info = new MudColor("#daeaf6"),

            Background = new MudColor("#fdfcfc"),
            Surface = new MudColor("#ffffff"),
            TextPrimary = new MudColor("#2d3748"),
            TextSecondary = new MudColor("#718096"),
            AppbarBackground = new MudColor("#f8f6fb"),
            AppbarText = new MudColor("#2d3748") // gris oscuro (slate-700)
        };

        var paletteDark = new PaletteDark();
        paletteDark.Primary = new MudColor("#818cf8");
        paletteDark.Success = new MudColor("#34d399");
        paletteDark.Warning = new MudColor("#fbbf24");
        paletteDark.Error = new MudColor("#f87171");
        paletteDark.Info = new MudColor("#60a5fa");
        paletteDark.Background = new MudColor("#0f172a");
        paletteDark.Surface = new MudColor("#1e293b");
        paletteDark.TextPrimary = new MudColor("#f1f5f9");
        paletteDark.TextSecondary = new MudColor("#94a3b8");

        return new MudTheme
        {
            PaletteLight = paletteLight,
            PaletteDark = paletteDark,
            LayoutProperties = new LayoutProperties
            {
                DefaultBorderRadius = "6px",
                DrawerWidthLeft = "240px",
                DrawerWidthRight = "300px",
                AppbarHeight = "32px"  // ← Añade esta línea
            } ,
            Typography = new Typography
            {
                Default = new DefaultTypography { FontFamily = ["Inter", "Segoe UI"] },
                H1 = new H1Typography { FontSize = "2rem", FontWeight = "600" },
                H2 = new H2Typography { FontSize = "1.5rem", FontWeight = "600" }
            }
        };
    }
}