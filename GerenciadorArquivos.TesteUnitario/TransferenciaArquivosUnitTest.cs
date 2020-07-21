using System;
using System.Collections.Generic;
using System.Linq;
using GerenciadorArquivos.Servico;
using GerenciadorArquivos.Servico.Tipo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GerenciadorArquivos.TesteUnitario
{
    [TestClass]
    public class TransferenciaArquivosUnitTest
    {
        private readonly string password = "T1ft84yIN@Uu%rp";
        private readonly string server = "srvahint.com.br";
        private readonly string user = "FTP-quirino";

        private string aplicacao => @"solicitacao.v2";
        private string local => $@"\\192.168.104.121\iis-root$\extranet";
        private string destino => $@"/FTP-fquirino/";

        [TestMethod]
        public void ProcessarAtualizacaoServidor()
        {
            var processamento = new TransferenciaTipo()
            {
                Transferencias = new List<CopiaTipo>()
                {
                    new CopiaTipo() { Aplicacao = "solicitacao.v2", Arquivos = new []{ "*" }, De = @"\\192.168.104.121\iis-root$\novaintranet", Para = "/FTP-fquirino/novaintranet" },
                    new CopiaTipo() { Aplicacao = "solicitacao.v2", Arquivos = new []{ "*" }, De = @"\\192.168.104.121\iis-root$\extranet", Para = "/FTP-fquirino/extranet" },
                    new CopiaTipo() { Aplicacao = "solicitacao", Arquivos = new []{ "*" }, De = @"\\192.168.104.121\iis-root$\api-corporativo", Para = "/FTP-fquirino/api-corporativo" },
                }
            };

            foreach (var transferencia in processamento.Transferencias)
            {
                var transf = new TransferenciaArquivos(server, user, password, false)
                {
                    GeraBackup = transferencia.GeraBackup
                };
                if (transferencia.Arquivos.All(x => x.Equals("*")))
                {
                    transf.CopiarTudo(transferencia.Aplicacao, transferencia.De, transferencia.Para);
                }
                else
                {
                    transf.Copiar(transferencia.Aplicacao, transferencia.De, transferencia.Para, transferencia.Arquivos);
                }
            }
        }

        [TestMethod]
        public void CopiarTudo()
        {
            var transferencia = new TransferenciaArquivos(server, user, password, false);
            transferencia.CopiarTudo(aplicacao, local, destino);
            Console.WriteLine(string.Join("\n", transferencia.Resultado));
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Copiar()
        {
            var transferencia = new TransferenciaArquivos(server, user, password, false);

            transferencia.Copiar(aplicacao, $@"{local}\{aplicacao}\",
                $"{destino}/{aplicacao}/",
                new[]
                {
                    "touch-icon-iphone.png",
                    @"scripts\zonasul.js",
                    @"bin\roslyn\vbc.exe.config",
                });

            Console.WriteLine(string.Join("\n", transferencia.Resultado));
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Listar()
        {
            var aplicacao = "solicitacao.v2";
            var local = @"\\192.168.104.121\iis-root$\extranet";
            var arquivos = TransferenciaArquivos.ListarLocal(aplicacao, local);
            Assert.IsTrue(arquivos.Length > 0, "Não foi possível encontrar arquivos");
        }
    }
}