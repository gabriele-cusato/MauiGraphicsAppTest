using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Maui;
using UraniumUI;
using MauiAppGraphicsTest.ViewModels;
using MauiAppGraphicsTest.Views;
using MauiAppGraphicsTest.Services;
using MauiAppGraphicsTest.Controls;

namespace MauiAppGraphicsTest;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit() // Include le animazioni
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<MainPageViewModel>();
        builder.Services.AddSingleton<FieraDataService>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
