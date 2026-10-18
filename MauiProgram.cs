using CarnetDepenses.Services;
using CarnetDepenses.ViewModels;
using Microsoft.Extensions.Logging;

namespace CarnetDepenses;

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
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		// Champs sans soulignement natif : le cadre est dessiné en XAML (Border).
		Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("SansBordure", (h, v) =>
		{
#if WINDOWS
			h.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
#elif ANDROID
			h.PlatformView.BackgroundTintList =
				Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#endif
		});

		Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping("SansBordure", (h, v) =>
		{
#if WINDOWS
			h.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
			h.PlatformView.Background = null;
#elif ANDROID
			h.PlatformView.BackgroundTintList =
				Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#endif
		});

		builder.Services.AddSingleton<IDepenseRepository, SqliteDepenseRepository>();   // UNE SEULE ligne change
		// Vraie API plus tard : remplacer new FakeApiHandler() par rien, soit new HttpClient().
		builder.Services.AddSingleton(new HttpClient(new FakeApiHandler()));
		builder.Services.AddSingleton<IApiService, ApiService>();
		builder.Services.AddSingleton<DepensesViewModel>();
		builder.Services.AddSingleton<MainPage>();

		return builder.Build();
	}
}
