using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

namespace MauiSingleInstanceApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .ConfigureLifecycleEvents(lifecycle =>
                {
#if WINDOWS
                    lifecycle.AddWindows(windows =>
                    {
                        windows.OnAppInstanceActivated((sender, e) =>
                            HandleAppActions((e.Data as Windows.ApplicationModel.Activation.LaunchActivatedEventArgs)?.Arguments));
                    });
#endif
                })
                .ConfigureEssentials(essentials =>
                {
                    if (AppActions.Current.IsSupported)
                    {
                        essentials
                            .AddAppAction("appicon", "Present the app again", icon: "appicon")
                            .OnAppAction(HandleAppActions);
                    }
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        // this event will run for old app instances
        private static void HandleAppActions(string? arguments) =>
            HandleAppAction(AppActionsHelper.GetAppActionId(arguments));

        // this event will run for new app instances
        private static void HandleAppActions(AppAction appAction) =>
            HandleAppAction(appAction.Id);

        private static void HandleAppAction(string? appActionId)
        {
            var wasAppAction = appActionId == "appicon";

            if (Application.Current is not App app)
                return;

            foreach (var window in app.Windows)
            {
                if (window.Page is not Page page)
                    continue;

                if (page is Shell shell)
                    page = shell.CurrentPage;

                page.Dispatcher.DispatchAsync(async () =>
                {
                    await Task.Delay(100);
                    await page.DisplayAlert(
                        "Single Instance",
                        wasAppAction ? "This was shown from the App Actions." : "This was shown from a new launch.",
                        "OK");
                });
            }
        }
    }
}
