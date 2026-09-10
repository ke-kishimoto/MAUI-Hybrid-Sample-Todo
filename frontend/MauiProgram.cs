using Microsoft.Extensions.Logging;

using Frontend.Data;

namespace Frontend;

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
			});

		builder.Services.AddMauiBlazorWebView();
		builder.Services.AddSingleton<SqliteDatabase>();
		builder.Services.AddSingleton<LocalTodoService>();
		builder.Services.AddSingleton(_ =>
		{
			var configuredUrl = Environment.GetEnvironmentVariable("TODO_API_BASE_URL");
			var baseUrl = string.IsNullOrWhiteSpace(configuredUrl)
				? "http://localhost:8080/api/v1/"
				: configuredUrl.Trim().TrimEnd('/') + "/";

			return new HttpClient
			{
				BaseAddress = new Uri(baseUrl, UriKind.Absolute),
				Timeout = TimeSpan.FromSeconds(15),
			};
		});
		builder.Services.AddSingleton<Sync.TodoApiClient>();
		builder.Services.AddSingleton<Sync.TodoSyncService>();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
