using Serilog;
using WorkerCambioRestricao.APPLICATION.Services;
using WorkerCambioRestricao.INFRA.Repositories;


namespace WorkerCambioRestricao.WORK
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, configuration) =>
                {
                    configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var configuration = hostContext.Configuration;

                    string logPath = Environment.GetEnvironmentVariable("LogPath") ?? "/logs/WORKCAMBIORESTRICAOTRACE.txt";

                    Log.Logger = new LoggerConfiguration()
                        .WriteTo.File(logPath)
                        .CreateLogger();

                    services.AddLogging(builder =>
                    {
                        builder.AddSerilog();
                    });

                    string connection = configuration.GetConnectionString("Default");
                    string decryptConnection = CriptoServiceHelper.Descriptografar(connection);
                    
                    services.AddSingleton<IRestricoesRepository>(_ => new RestricoesRepository(decryptConnection));
                    //services.AddTransient<IRestricoesRepository>(_ => new RestricoesRepository(decryptConnection));

                    services.AddTransient<IFtpClientWrapper>(provider =>
                    {
                        var repo = provider.GetRequiredService<IRestricoesRepository>();
                        string ftpServer = repo.ObterParametroCambio(3);
                        string ftpUser = repo.ObterParametroCambio(4);
                        string ftpPassword = repo.ObterParametroCambio(5);
                        return new FtpClientWrapper(ftpServer, ftpUser, ftpPassword);
                    });

                    services.AddTransient<RestricaoService>();
                    services.AddTransient<IRestricaoService, RestricaoService>();

                    //services.AddSingleton(provider => CriptoServiceHelper.Descriptografar(provider.GetRequiredService<IConfiguration>()["DiretorioCSV"]));

                    services.AddHostedService<WorkerService>(); 
                })
                .UseWindowsService();

        public static void StopApplication()
        {
            Console.WriteLine("Saindo da aplicação.");
            return;
        }
    }
}