
# Projeto Worker Service

![.NET](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=dot-net&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![xUnit](https://img.shields.io/badge/xUnit-5E8C8C?style=for-the-badge&logo=xunit&logoColor=white)
![MoQ](https://img.shields.io/badge/MoQ-5C2D91?style=for-the-badge&logo=microsoft&logoColor=white)
![Fine Code Coverage](https://img.shields.io/badge/Fine_Code_Coverage-282C34?style=for-the-badge&logo=visual-studio&logoColor=5C2D91)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)

## Visão Geral

O **WorkerCambioRestricao** é um serviço de trabalho em segundo plano desenvolvido com .NET 6.0, projetado para executar tarefas periódicas relacionadas a restrições de câmbio. O projeto inclui testes unitários abrangentes e utiliza ferramentas de cobertura de código para garantir a qualidade do código.

## Tecnologias Utilizadas

- **.NET 6.0**: Framework moderno e de alto desempenho para a construção de aplicações cross-platform.
- **C#**: Linguagem de programação principal utilizada.
- **xUnit**: Framework de testes unitários.
- **MoQ**: Framework de mocking para .NET, utilizado nos testes unitários.
- **Fine Code Coverage**: Ferramenta para análise de cobertura de código.

## Funcionalidades

1. **Processamento em Segundo Plano**: O serviço de trabalho executa tarefas em segundo plano periodicamente.
2. **Log**: Registra informações em intervalos regulares.
3. **Testes Unitários**: Testes abrangentes para garantir o funcionamento correto do serviço.
4. **Cobertura de Código**: Utilização da ferramenta Fine Code Coverage para analisar a cobertura dos testes.

## Estrutura do Projeto

```plaintext
WorkerCambioRestricao/
├── WorkerCambioRestricao/
│   ├── bin/
│   │   ├── Debug/
│   │   │   ├── net6.0/
│   │   │       ├── (arquivos binários compilados)
│   │   ├── Release/
│   │       ├── net6.0/
│   │           ├── (arquivos binários compilados para release)
│   ├── obj/
│   │   ├── Debug/
│   │   │   ├── net6.0/
│   │   │       ├── (arquivos intermediários de compilação)
│   │   ├── Release/
│   │       ├── net6.0/
│   │           ├── (arquivos intermediários de compilação para release)
│   ├── Properties/
│   │   ├── launchSettings.json
│   │   ├── PublishProfiles/
│   │       ├── FolderProfile.pubxml
│   │       ├── FolderProfile.pubxml.user
│   ├── Program.cs
│   ├── Worker.cs
│   ├── appsettings.json
├── WorkerCambioRestricao.APPLICATION/
│   ├── Services/
│   │   ├── ServiceInterface.cs
│   │   ├── ServiceImplementation.cs
│   ├── Interfaces/
│   │   ├── IExampleInterface.cs
├── WorkerCambioRestricao.DOMAIN/
│   ├── Entities/
│   │   ├── ExampleEntity.cs
│   ├── Repositories/
│   │   ├── IExampleRepository.cs
│   ├── Services/
│   │   ├── IExampleService.cs
├── WorkerCambioRestricao.INFRASTRUCTURE/
│   ├── Data/
│   │   ├── ApplicationDbContext.cs
│   ├── Migrations/
│   │   ├── (arquivos de migração)
├── WorkerCambioRestricao.TESTS/
│   ├── bin/
│   │   ├── Debug/
│   │   │   ├── net6.0/
│   │   │       ├── (arquivos binários compilados para testes)
│   ├── obj/
│   │   ├── Debug/
│   │   │   ├── net6.0/
│   │   │       ├── (arquivos intermediários de compilação para testes)
│   ├── WorkerTests.cs
├── fine-code-coverage/
│   ├── build-output/
│   │   ├── (arquivos relacionados à cobertura de código)
├── README.md

```

## Detalhes dos Arquivos

### Program.cs

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WorkerCambioRestricao
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                });
    }
}
```

### Worker.cs

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WorkerCambioRestricao
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
```

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

### WorkerTests.cs

```csharp
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using WorkerCambioRestricao;
using Xunit;

namespace WorkerCambioRestricao.TESTS
{
    public class WorkerTests
    {
        private readonly Mock<ILogger<Worker>> _loggerMock;
        private readonly Worker _worker;

        public WorkerTests()
        {
            _loggerMock = new Mock<ILogger<Worker>>();
            _worker = new Worker(_loggerMock.Object);
        }

        [Fact]
        public async Task Worker_LogsInformation()
        {
            // Arrange
            var stoppingToken = new CancellationTokenSource().Token;

            // Act
            await _worker.StartAsync(stoppingToken);
            await Task.Delay(1500);  // Wait to ensure the worker runs at least once
            await _worker.StopAsync(stoppingToken);

            // Assert
            _loggerMock.Verify(
                x => x.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()),
                Times.AtLeastOnce
            );
        }
    }
}
```

## Como Executar o Projeto

1. Clone o repositório:
    ```bash
    git clone <URL do repositório>
    ```

2. Navegue até o diretório do projeto:
    ```bash
    cd WorkerCambioRestricao
    ```

3. Restaure as dependências:
    ```bash
    dotnet restore
    ```

4. Execute o projeto:
    ```bash
    dotnet run
    ```

5. Execute os testes:
    ```bash
    dotnet test
    ```

## Contribuição

Sinta-se à vontade para contribuir com o projeto. Por favor, siga as diretrizes de contribuição e mantenha um estilo de código consistente.

## Licença

Este projeto é licenciado sob os termos da licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

---

Feito com ❤️ por Rodney Victor
