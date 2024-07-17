using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkerCambioRestricao.APPLICATION.Services;
using WorkerCambioRestricao.INFRA.Repositories;

namespace WorkerCambioRestricao.WORK
{
    public class WorkerService : BackgroundService
    {
        private readonly ILogger<WorkerService> _logger;
        private readonly IRestricaoService _restricaoService;
        //private readonly string _decryptPath;

        public WorkerService(ILogger<WorkerService> logger, IRestricaoService restricaoService/*, IConfiguration configuration*/)
        {
            _logger = logger;
            _restricaoService = restricaoService;
            //_decryptPath = CriptoServiceHelper.Descriptografar(configuration["DiretorioCSV"]);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _restricaoService.ProcessarCSV();
                _logger.LogInformation("Servico em execucao...");
                Thread.Sleep(TimeSpan.FromHours(24));
            }
        }
    }
}
