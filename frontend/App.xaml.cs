namespace Frontend;

using Frontend.Data;
using Microsoft.Extensions.Logging;

public partial class App : Application
{
	private readonly SqliteDatabase database;
	private readonly ILogger<App> logger;

	public App(SqliteDatabase database, ILogger<App> logger)
	{
		InitializeComponent();
		this.database = database;
		this.logger = logger;
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		var window = new Window(new MainPage()) { Title = "Frontend" };
		window.Created += OnWindowCreated;

		return window;
	}

	private async void OnWindowCreated(object? sender, EventArgs e)
	{
		if (sender is Window window)
		{
			window.Created -= OnWindowCreated;
		}

		try
		{
			await database.InitializeAsync();
			logger.LogInformation("SQLite database initialized successfully.");
		}
		catch (Exception exception)
		{
			logger.LogError(exception, "Failed to initialize the SQLite database.");
		}
	}
}
