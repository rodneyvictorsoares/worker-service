using Microsoft.Extensions.Logging;
using System.Data;
using System.Globalization;
using WorkerCambioRestricao.DOMAIN.Entities;
using WorkerCambioRestricao.INFRA.Repositories;
using FluentFTP;

namespace WorkerCambioRestricao.APPLICATION.Services
{
    public class RestricaoService : IRestricaoService
    {
        private readonly ILogger<RestricaoService> _logger;
        private readonly IRestricoesRepository _repositorio;
        private readonly IFtpClientWrapper _ftpClient;

        public RestricaoService(IRestricoesRepository repositorio, ILogger<RestricaoService> logger, IFtpClientWrapper ftpClient)
        {
            _repositorio = repositorio;
            _logger = logger;
            _ftpClient = ftpClient;
        }

        public void ProcessarCSV()
        {
            
            _logger.LogInformation("-------------------------------------------------------------------------------------");
            _logger.LogInformation(DateTime.Now.ToString("D") + ":");
            _logger.LogInformation(" ");
            _logger.LogInformation("Iniciando o Worker..." + DateTime.Now.ToString());

            string path = _repositorio.ObterParametroCambio(1);
            string nomeArquivo = _repositorio.ObterParametroCambio(2);
            var diretorioRemoto = Path.Combine(path, nomeArquivo);

            string ftpServer = _repositorio.ObterParametroCambio(3);
            string ftpUsername = _repositorio.ObterParametroCambio(4);
            string ftpPassword = _repositorio.ObterParametroCambio(5);
            string localPath = Path.Combine(Path.GetTempPath(), nomeArquivo);

            _ftpClient.Connect();

            if (!_ftpClient.FileExists(diretorioRemoto))
            {
                _logger.LogWarning("Não foi encontrado arquivo para a data atual...Finalizando" + DateTime.Now.ToString());
                _logger.LogInformation("Cancelando o Worker..." + DateTime.Now.ToString());
                return;
            }

            _ftpClient.DownloadFile(localPath, diretorioRemoto);

            var registros = File.ReadAllLines(localPath)
                .Skip(1)
                .Select(l => l.Split(';'))
                .Where(c => c.Length >= 3)
                .Select(c => new RestricoesCSV
                {
                    CPFCNPJ = c[0].Replace("\"", ""),
                    NOME = c[1].Replace("\"", ""),
                    FONTE_INFORMACAO = c[2].Replace("\"", "")
                });

            _repositorio.InserirRegistros(registros);
            
            registros = null;

            var NovoNomeArquivo = $"Lista{DateTime.Now.ToString("yyyyMMdd")}.csv";
            var localNovoArquivo = Path.Combine(Path.GetTempPath(), NovoNomeArquivo);

            File.Move(localPath, localNovoArquivo);

            _ftpClient.UploadFile(localNovoArquivo, Path.Combine(path, NovoNomeArquivo));
            _ftpClient.DeleteFile(diretorioRemoto);

            if (File.Exists(localPath))
            {
                File.Delete(localPath);
            }
            if (File.Exists(localNovoArquivo))
            {
                File.Delete(localNovoArquivo);
            }

            _logger.LogInformation("Lista de Restrições Atualizada..." + DateTime.Now.ToString());
            _logger.LogInformation("Finalizando o Worker..." + DateTime.Now.ToString());
        }
    }
}
