# MauiSingleInstanceApp

A sample app showing a single instance .NET MAUI for Windows.

The main code is in the WinUI App.xaml.cs file:

```cs
public App()
{
    var singleInstance = AppInstance.FindOrRegisterForKey("SingleInstanceApp");
    if (!singleInstance.IsCurrent)
    {
        // this is another instance

        // 1. activate the first instance
        var currentInstance = AppInstance.GetCurrent();
        var args = currentInstance.GetActivatedEventArgs();
        singleInstance.RedirectActivationToAsync(args).GetAwaiter().GetResult();

        // 2. close this instance
        Process.GetCurrentProcess().Kill();
        return;
    }

    // this is the first instance

    // 1. register for future activation
    singleInstance.Activated += OnAppInstanceActivated;

    // 2. continue with normal startup
    this.InitializeComponent();
}

private void OnAppInstanceActivated(object? sender, AppActivationArguments e)
{
    // handle the old app being loaded
}
```
