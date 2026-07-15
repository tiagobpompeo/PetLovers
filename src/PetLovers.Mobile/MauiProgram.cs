using Microsoft.Extensions.Logging;
using PetLovers.Mobile.Services;
using PetLovers.Mobile.ViewModels;
using PetLovers.Mobile.Views;

namespace PetLovers.Mobile;

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

		builder.Services.AddSingleton<ApiService>();
		builder.Services.AddTransient<PetsViewModel>();
		builder.Services.AddTransient<PetFormViewModel>();
		builder.Services.AddTransient<PetsPage>();
		builder.Services.AddTransient<PetFormPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
