using System;
using GerenciadorArquivos.Servico;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GerenciadorArquivos.TesteUnitario
{
    [TestClass]
    public class TransferenciaArquivosUnitTest
    {
        [TestMethod]
        public void teste1()
        {
            string server = string.Empty;
            string user = string.Empty;
            string password = string.Empty;
            new TransferenciaArquivos(server, user, password).Enviar();
        }
    }
}
