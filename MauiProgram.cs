using CommunityToolkit.Maui;
using i_KimMobileClient.Handlers;
using Microcharts.Maui;
using Microsoft.Extensions.Logging;
using Mopups.Hosting;
using Syncfusion.Maui.Toolkit.Hosting;
using The49.Maui.BottomSheet;

namespace i_KimMobileClient
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseBottomSheet()
                .UseMauiCommunityToolkit()
                .ConfigureSyncfusionToolkit()
                .ConfigureMopups()
                .UseMicrocharts()
                .ConfigureMauiHandlers(handlers =>
                {
#if IOS
                    handlers.AddHandler<Microsoft.Maui.Controls.CollectionView, Microsoft.Maui.Controls.Handlers.Items2.CollectionViewHandler2>();
#endif
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("SegoeUI-Semibold.ttf", "SegoeSemibold");
                    fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);

                    fonts.AddFont("segoeui Regular.ttf", "segoeuiRegular");
                    fonts.AddFont("segoeuib Bold.ttf", "segoeuiBold");
                    fonts.AddFont("seguisb Semibold.ttf", "segoeuiSemibold");
                });
            //builder.UseMauiApp<App>().ConfigureSyncfusionToolkit();

#if DEBUG
            builder.Logging.AddDebug();
            builder.Services.AddLogging(configure => configure.AddDebug());
#endif

            // Global .NET exception
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                if (args.ExceptionObject is Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(
                        "========== UNHANDLED EXCEPTION ==========");
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                    System.Diagnostics.Debug.WriteLine(
                        "==========================================");
                }
            };

            // Exceptions from async Tasks
            TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                System.Diagnostics.Debug.WriteLine(
                    "========== UNOBSERVED TASK EXCEPTION ==========");
                System.Diagnostics.Debug.WriteLine(args.Exception.ToString());
                System.Diagnostics.Debug.WriteLine(
                    "================================================");

                args.SetObserved();
            };

            BoderlessEntryHandler.RemoveBorder();
            return builder.Build();
        }
    }
}
