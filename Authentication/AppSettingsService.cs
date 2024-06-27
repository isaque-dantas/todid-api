namespace TodoAPI.Authentication;

public static class AppSettingsService
{
    static AppSettingsService()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath("/home/isaque/RiderProjects/TodoAPI/TodoAPI/")
            .AddJsonFile("appsettings.json")
            .Build();

        JwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>() ?? new JwtSettings();
        ConnectionString =
            configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Get<string>() ?? "";
    }

    public static JwtSettings JwtSettings { get; }
    public static string ConnectionString { get; }
}