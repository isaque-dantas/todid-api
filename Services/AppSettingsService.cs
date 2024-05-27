namespace TodoAPI.Services;
using Models;

public static class AppSettingsService
{
    public static JwtSettings JwtSettings { get; }

    static AppSettingsService()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetParent(".")!.ToString())
            .AddJsonFile("appsettings.json")
            .Build();

        JwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>() ?? new JwtSettings();
    }
}