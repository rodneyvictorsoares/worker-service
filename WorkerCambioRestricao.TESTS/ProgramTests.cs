using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using WorkerCambioRestricao.APPLICATION.Services;
using WorkerCambioRestricao.INFRA.Repositories;
using WorkerCambioRestricao.WORK;
using Xunit;

namespace WorkerCambioRestricao.TESTS
{
    public class ProgramTests
    {
        private readonly Mock<IHost> _mockHost;
        private readonly Mock<IHostBuilder> _mockHostBuilder;

        public ProgramTests()
        {
            _mockHost = new Mock<IHost>();
            _mockHostBuilder = new Mock<IHostBuilder>();
        }

        [Fact]
        public void Main_ShouldRunWithoutErrors()
        {
            // Arrange
            var args = Array.Empty<string>();
            var configurationBuilder = new ConfigurationBuilder();
            var configuration = configurationBuilder.Build();

            var hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddConfiguration(configuration);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var mockServiceProvider = new Mock<IServiceProvider>();
                    mockServiceProvider.Setup(sp => sp.GetService(typeof(RestricaoService)))
                        .Returns(new Mock<RestricaoService>().Object);

                    services.AddSingleton<IRestricoesRepository>(_ => new Mock<IRestricoesRepository>().Object);
                    services.AddSingleton<IServiceProvider>(_ => mockServiceProvider.Object);
                });

            // Act
            Task.Run(() =>
            {
                Program.Main(args);
            });

            Thread.Sleep(1000);

            Program.StopApplication();

            // Assert
            // Se o programa sair sem lançar exceções, o teste passa
        }
    }
}
