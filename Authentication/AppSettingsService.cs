namespace TodoAPI.Authentication;

public static class AppSettingsService
{
    public static JwtSettings JwtSettings { get; }

    static AppSettingsService()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath("/home/isaque/RiderProjects/TodoAPI/TodoAPI/")
            .AddJsonFile("appsettings.json")
            .Build();

        JwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>() ?? new JwtSettings();
    }
}