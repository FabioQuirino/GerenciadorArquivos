using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GerenciadorArquivos.Servico;
using GerenciadorArquivos.Servico.Tipo;
using Microsoft.SqlServer.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace GerenciadorArquivos.TesteUnitario
{
    [TestClass]
    public class TransferenciaArquivosUnitTest
    {
        private readonly string password = "T1frp";
        private readonly string server = "srvahint01.com.br";
        private readonly string user = "FTP-fquirino";

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
                    new CopiaTipo() { Aplicacao = "AtendimentoFracionado", Arquivos = new []{ "*" }, De = @"\\192.168.104.121\iis-root$\novaintranet", Para = "/FTP-fquirino/novaintranet" },
                    // new CopiaTipo() { Aplicacao = "solicitacao.v2", Arquivos = new []{ "*" }, De = @"\\192.168.104.121\iis-root$\extranet", Para = "/FTP-fquirino/extranet" },
                    // new CopiaTipo() { Aplicacao = "solicitacao", Arquivos = new []{ "*" }, De = @"\\192.168.104.121\iis-root$\api-corporativo", Para = "/FTP-fquirino/api-corporativo" },
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
                    transf.CopiarTudo(transferencia.Aplicacao, transferencia.De, transferencia.Para, null);
                }
                else
                {
                    transf.Copiar(transferencia.Aplicacao, transferencia.De, transferencia.Para, transferencia.Arquivos, null);
                }
            }
        }

        [TestMethod]
        public void CopiarTudo()
        {
            var transfApi = new Copia2Tipo()
            {
                Password = "Wp",
                Server = "sapwsv01.com.br",
                User = "FTP-fquirino",
                Local = $@"\\192.168.104.121\c$\INTERNET\api-corporativo",
                Destino = $@"/FTP-fquirino/API/",
            };

            System.Diagnostics.Debug.WriteLine("----------");
            System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(transfApi, new JsonSerializerSettings() { Formatting = Formatting.None, NullValueHandling = NullValueHandling.Ignore }));

            var transfIntra = new Copia2Tipo()
            {
                Password = "Ftuys",
                Server = "sapint01.com.br",
                User = "FTP-service",
                Local = $@"\\192.168.104.121\iis-root$\novaintranet",
                Destino = $@"/FTP-service/novaintranet/"
            };

            System.Diagnostics.Debug.WriteLine("----------");
            System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(transfIntra, new JsonSerializerSettings() { Formatting = Formatting.None, NullValueHandling = NullValueHandling.Ignore }));

            var transfExtra = new Copia2Tipo()
            {
                Password = "#b@55.",
                Server = "sapweb02.com.br",
                User = "FTP-extranet",
                Local = $@"\\192.168.104.121\c$\internet\extranet",
                Destino = $@"/FTP-extranet/",
            };

            System.Diagnostics.Debug.WriteLine("----------");
            System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(transfExtra, new JsonSerializerSettings() { Formatting = Formatting.None, NullValueHandling = NullValueHandling.Ignore }));

            var transfApiExtra = new Copia2Tipo()
            {
                Password = "P9&bz",
                Server = "sapweb02.com.br",
                User = "FTP-extranet",
                Local = $@"\\192.168.104.121\c$\INTERNET\api-hmlg",
                Destino = $@"/FTP-fquirino/",
            };

            System.Diagnostics.Debug.WriteLine("----------");
            System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(transfApiExtra, new JsonSerializerSettings() { Formatting = Formatting.None, NullValueHandling = NullValueHandling.Ignore }));

            var objetos = new List<Copia2Tipo>();

            transfApiExtra = new Copia2Tipo() { Excecoes = new[] { "abc", @"bin\teste" } };
            objetos.Add(transfApiExtra);

            transfApiExtra = new Copia2Tipo() { Excecoes = new[] { "*" } };
            objetos.Add(transfApiExtra);

            JsonConvert.SerializeObject(objetos);

            for (int i = 0; i < objetos.Count; i++)
            {
                var transf = objetos.ElementAt(i);
                var transferencia = new TransferenciaArquivos(transf.Server, transf.User, transf.Password, false);
                transferencia.GeraBackup = true;
                transferencia.CopiarTudo(transf.Aplicacao, transf.Local, transf.Destino, transf.Excecoes);
            }

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
                },
                null);

            Console.WriteLine(string.Join("\n", transferencia.Resultado));
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Copiar2()
        {
            var transferencia = new TransferenciaArquivos(server, user, password, false);
            var from = $@"\\192.168.104.121\iis-root$\extranet\solicitacao.v2";
            var to = $"/FTP-fquirino/extranet/solicitacao.v2";

            transferencia.Copiar(from,
                to,
                new[]
                {
                    "touch-icon-iphone.png",
                    @"scripts\zonasul.js"
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

        [TestMethod]
        public void VerificarVersao()
        {
            var result = new Servico.GerenciadorArquivos().VerificarVersao("sapint01.zonasul.com.br", "FTP-service", "Ftupg7Serv1c&@S%ys");
            Console.Write(string.Join("\n", result));
            Assert.IsTrue(true);
        }
    }
}