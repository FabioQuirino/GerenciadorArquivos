using System;
using System.Linq;
using WinSCP;

namespace GerenciadorArquivos.Servico
{
    public class TransferenciaArquivos : BaseServico
    {
        private TransferenciaArquivos() { }

        public TransferenciaArquivos(string host, string user, string password, bool geralog)
        {
            base.Host = host;
            base.User = user;
            base.Password = password;
            base.GeraLog = geralog;
        }

        public void Sincronizar(string local, string remoto)
        {
            base.padrao = new[] { "*" };
            using (var session = GetNewSession())
            {
                var transferResult = session.SynchronizeDirectories(SynchronizationMode.Remote, local, remoto, true, options: TransferOptions);
                transferResult.Check();
            }
        }

        public void Copiar(string local, string remoto, string[] arquivos)
        {
            using (var session = GetNewSession())
            {
                Backup(session, local, remoto, arquivos);

                foreach (var arquivo in arquivos)
                {
                    var arquivoLocal = $"{local}{arquivo}";
                    var arquivoRemoto = $"{remoto}{arquivo.Replace(@"\", "/")}";
                    var transferResult = session.PutFiles(arquivoLocal, arquivoRemoto);
                    transferResult.Check();
                }
            }
        }

        public void Backup(string[] arquivos, string local, string remoto)
        {
            using (var sessionFtp = GetNewSession())
            {
                Backup(sessionFtp, local, remoto, arquivos);
            }
        }


        private void Backup(Session sessionFtp, string local, string remoto, string[] arquivos)
        {
            var dataHoraAtual = DateTime.Now.ToString("yyyyMMddHHmmss");
            var diretorioBackup = $"backup-{dataHoraAtual}";

            foreach (var arquivo in arquivos)
            {
                try
                {
                    var arquivoRemoto = $@"{remoto}\{arquivo}".Replace(@"\", "/").Replace(@"\\", @"\");
                    var arquivoRemotoBackup = $@"{remoto}\{diretorioBackup}\{arquivo}".Replace(@"\", "/").Replace(@"\\", @"\");
                    var remotoSemArquivo = arquivoRemotoBackup.Replace(arquivoRemotoBackup.Split('/').Last(), "");

                    if (!sessionFtp.FileExists(remotoSemArquivo))
                    {
                        sessionFtp.CreateDirectory(remotoSemArquivo);
                    }

                    sessionFtp.MoveFile(arquivoRemoto, arquivoRemotoBackup);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}