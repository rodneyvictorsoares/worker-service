using System;
using System.IO;
using System.Linq;
using FluentFTP;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using WorkerCambioRestricao.APPLICATION.Services;
using WorkerCambioRestricao.DOMAIN.Entities;
using WorkerCambioRestricao.INFRA.Repositories;
using Xunit;

namespace WorkerCambioRestricao.TESTS
{
    public class RestricaoServiceTests
    {
        private readonly Mock<IRestricoesRepository> _mockRepo;
        private readonly Mock<ILogger<RestricaoService>> _mockLogger;
        private readonly Mock<IFtpClientWrapper> _mockFtpClient;
        private readonly RestricaoService _service;

        public RestricaoServiceTests()
        {
            _mockRepo = new Mock<IRestricoesRepository>();
            _mockLogger = new Mock<ILogger<RestricaoService>>();
            _mockFtpClient = new Mock<IFtpClientWrapper>();
            _service = new RestricaoService(_mockRepo.Object, _mockLogger.Object, _mockFtpClient.Object);
        }

        [Fact]
        public void ProcessarCSV_DeveProcessarArquivoCorretamente()
        {
            // Arrange
            _mockRepo.Setup(r => r.ObterParametroCambio(1)).Returns("/path/to/dir");
            _mockRepo.Setup(r => r.ObterParametroCambio(2)).Returns("arquivo.csv");
            _mockRepo.Setup(r => r.ObterParametroCambio(3)).Returns("ftp.server");
            _mockRepo.Setup(r => r.ObterParametroCambio(4)).Returns("ftpUser");
            _mockRepo.Setup(r => r.ObterParametroCambio(5)).Returns("ftpPassword");
            _mockRepo.Setup(r => r.InserirRegistros(It.IsAny<IEnumerable<RestricoesCSV>>()));

            var tempPath = Path.Combine(Path.GetTempPath(), "arquivo.csv");
            File.WriteAllText(tempPath, "CPFCNPJ;NOME;FONTE_INFORMACAO\n123456789;John Doe;Source");

            _mockFtpClient.Setup(f => f.Connect());
            _mockFtpClient.Setup(f => f.FileExists(It.IsAny<string>())).Returns(true);
            _mockFtpClient.Setup(f => f.DownloadFile(It.IsAny<string>(), It.IsAny<string>()));
            _mockFtpClient.Setup(f => f.UploadFile(It.IsAny<string>(), It.IsAny<string>()));
            _mockFtpClient.Setup(f => f.DeleteFile(It.IsAny<string>()));

            // Act
            _service.ProcessarCSV();

            // Assert
            _mockRepo.Verify(r => r.InserirRegistros(It.IsAny<IEnumerable<RestricoesCSV>>()), Times.Once);
        }

        [Fact]
        public void ProcessarCSV_DeveLogarErroParaArquivoInexistente()
        {
            // Arrange
            _mockRepo.Setup(r => r.ObterParametroCambio(1)).Returns("/path/to/dir");
            _mockRepo.Setup(r => r.ObterParametroCambio(2)).Returns("arquivo_inexistente.csv");
            _mockRepo.Setup(r => r.ObterParametroCambio(3)).Returns("ftp.server");
            _mockRepo.Setup(r => r.ObterParametroCambio(4)).Returns("ftpUser");
            _mockRepo.Setup(r => r.ObterParametroCambio(5)).Returns("ftpPassword");

            var tempPath = Path.Combine(Path.GetTempPath(), "arquivo_inexistente.csv");
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }

            _mockFtpClient.Setup(f => f.Connect());
            _mockFtpClient.Setup(f => f.FileExists(It.IsAny<string>())).Returns(false);

            // Act
            _service.ProcessarCSV();

            // Assert
            _mockLogger.Verify(l => l.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Warning),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Não foi encontrado arquivo para a data atual")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public void ProcessarCSVAsync_ErroAoInserirnaBase()
        {
            //Arrage
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string connectionStringMock = CriptoServiceHelper.Descriptografar(configuration.GetConnectionString("Default"));
            var mockRepository = new Mock<IRestricoesRepository>();

            var mockRestricoes = new List<RestricoesCSV> { new RestricoesCSV
            {
                CPFCNPJ = null,
                FONTE_INFORMACAO = null,
                NOME = null
            } };

            var restricoesMockRepository = new RestricoesRepository(connectionStringMock);

            // Act & Assert
            Assert.Throws<Exception>(() => restricoesMockRepository.InserirRegistros(mockRestricoes));
        }
    }
}
