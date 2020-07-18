using GerenciadorArquivos.Servico;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GerenciadorArquivos.TesteUnitario
{
    [TestClass]
    public class TransferenciaArquivosUnitTest
    {
        private readonly string password = "423";
        private readonly string server = "srvdfhg.com.br";
        private readonly string user = "FTP-quirino";
        private TransferenciaArquivos transferenciaComLog => new TransferenciaArquivos(server, user, password, true);
        private TransferenciaArquivos transferenciaSemLog => new TransferenciaArquivos(server, user, password, false);

        private string aplicacao => @"solicitacao.v2";
        private string local => $@"\\192.168.104.121\iis-root$\extranet\";
        private string destino => $@"/FTP-fquirino/";

        [TestMethod]
        public void Sincronizar()
        {
            transferenciaSemLog.Sincronizar($"{local}{aplicacao}", string.Concat(destino, aplicacao));
        }

        [TestMethod]
        public void Copiar()
        {
            transferenciaSemLog.Copiar($@"{local}{aplicacao}\",
                $"{destino}/{aplicacao}/", 
                new []
                {
                    @"*",
                });
        }
    }
}