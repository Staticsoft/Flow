namespace Staticsoft.TestServer.Lambda;

public class Program
{
    public static void Main(string[] args)
        => CreateHostBuilder(args).Build().Run();

    static IHostBuilder CreateHostBuilder(string[] args)
        => Host
            .CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(builder => builder.UseStartup<LambdaStartup>());
}
