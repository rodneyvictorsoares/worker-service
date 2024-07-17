using System;
using WorkerCambioRestricao.APPLICATION.Services;
using Xunit;

namespace WorkerCambioRestricao.TESTS
{
    public class CriptoServiceHelperTest
    {
        public CriptoServiceHelperTest()
        {
        }

        [Fact]
        public void Criptografar_DeveCriptografar()
        {
            //Arrange
            var txtOrigem = "C:\\SPABANPARA\\APP\\ListaRestricoes\\";

            //Act
            var txtCrypto = CriptoServiceHelper.Criptografar(txtOrigem);

            //Assert
            Assert.NotEmpty(txtCrypto);
            Assert.NotEqual(txtCrypto, txtOrigem);
        }

        [Fact]
        public void Descriptografar_DeveDescriptografar()
        {
            var txtOrigem = "C:\\SPABANPARA\\APP\\ListaRestricoes\\";
            var txtCrypto = CriptoServiceHelper.Criptografar(txtOrigem);
            var txtDecrypted = CriptoServiceHelper.Descriptografar(txtCrypto);

            Assert.Equal(txtOrigem, txtDecrypted);
        }

        [Fact]
        public void Descriptografar_DeveLancarExcecaoParaEntradaInvalida()
        {
            Assert.Throws<FormatException>(() => CriptoServiceHelper.Descriptografar("TextoInvalido"));
        }
    }
}
