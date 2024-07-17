using Xunit;
using WorkerCambioRestricao.DOMAIN.Entities;

namespace WorkerCambioRestricao.TESTS
{
    public class RestricoesCSVTests
    {
        [Fact]
        public void RestricoesCSV_Creation_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var cpfCnpj = "123456789";
            var nome = "John Doe";
            var fonteInformacao = "Source";

            // Act
            var restricao = new RestricoesCSV
            {
                CPFCNPJ = cpfCnpj,
                NOME = nome,
                FONTE_INFORMACAO = fonteInformacao
            };

            // Assert
            Assert.Equal(cpfCnpj, restricao.CPFCNPJ);
            Assert.Equal(nome, restricao.NOME);
            Assert.Equal(fonteInformacao, restricao.FONTE_INFORMACAO);
        }
    }
}
