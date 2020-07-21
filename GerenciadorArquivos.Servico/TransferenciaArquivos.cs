using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
            base.GeraLogFtp = geralog;
            base.Resultado = new List<string>();
            base.GeraBackup = true;
        }

        public void CopiarTudo(string aplicacao, string local, string remoto)
        {
            var arquivos = ListarLocal(aplicacao, local);
            Copiar(aplicacao, local, remoto, arquivos);
        }

        public void Copiar(string aplicacao, string local, string remoto, string[] arquivos)
        {
            using (var session = GetNewSession())
            {
                Backup(session, aplicacao, local, remoto, arquivos);
                Resultado.Add("Cópia dos arquivos");

                foreach (var arquivo in arquivos)
                {
                    var arquivoLocal = $@"{local}\{aplicacao}\{arquivo}";
                    var arquivoRemoto = $@"{remoto}/{aplicacao}/{arquivo.Replace(@"\", "/")}";

                    try
                    {
                        var transferResult = session.PutFiles(arquivoLocal, arquivoRemoto);
                        transferResult.Check();
                        Resultado.Add($"OK - de: {arquivoLocal} para: {arquivoRemoto}");
                    }
                    catch (Exception e)
                    {
                        Resultado.Add($"FALHA - de: {arquivoRemoto} para: {arquivoRemoto}, erro: {e.Message}");
                    }
                }

                Resultado.Add("Fim cópia dos arquivos");
            }
        }

        public static string[] ListarLocal(string aplicacao, string local)
        {
            var itens = new DirectoryInfo($@"{local}\{aplicacao}\")
                .GetFiles("*", SearchOption.AllDirectories)
                .Select(x => x.FullName).ToArray();
            itens = itens.Select(x => x.Replace($@"{local}\{aplicacao}\", "")).ToArray();
            return itens;
        }

        private void Backup(Session sessionFtp, string aplicacao, string local, string remoto, string[] arquivos)
        {
            if (!GeraBackup)
            {
                return;
            }

            Resultado.Add($"Processamento do backup ------------------ ");
            var dataHoraAtual = DateTime.Now.ToString("yyyyMMddHHmmss");
            var diretorioBackup = $"backup-{dataHoraAtual}";

            string diretorioTemporario = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(diretorioTemporario);

            foreach (var arquivo in arquivos)
            {
                var arquivoRemoto = $@"{remoto}\{aplicacao}\{arquivo}".Replace(@"\", "/").Replace(@"\\", @"\");
                var arquivoRemotoBackup = $@"{remoto}/{aplicacao}/{diretorioBackup}\{arquivo}".Replace(@"\", "/").Replace(@"\\", @"\");
                var remotoSemArquivo = arquivoRemotoBackup.Replace(arquivoRemotoBackup.Split('/').Last(), "");

                try
                {
                    if (!sessionFtp.FileExists(remotoSemArquivo))
                    {
                        sessionFtp.CreateDirectory(remotoSemArquivo);
                    }

                    sessionFtp.MoveFile(arquivoRemoto, arquivoRemotoBackup);
                    Resultado.Add($"OK - de: {arquivoRemoto} para: {arquivoRemotoBackup}");
                }
                catch (Exception e)
                {
                    Resultado.Add($"FALHA - de: {arquivoRemoto} para: {arquivoRemotoBackup}, erro: {e.Message}");
                }
            }

            var result = sessionFtp.SynchronizeDirectories(SynchronizationMode.Local, diretorioTemporario, $@"{remoto}/{aplicacao}/{diretorioBackup}\", false);
            result.Check();

            var remove = sessionFtp.RemoveFiles($@"{remoto}/{aplicacao}/{diretorioBackup}\");
            remove.Check();
            ArquivoBkpZip = $@"c:\teste\transf-bkp-{dataHoraAtual}.zip";
            ZipFile.CreateFromDirectory(diretorioTemporario + @"\", ArquivoBkpZip, CompressionLevel.Optimal, false);

            Resultado.Add($"Fim do processamento do backup ------------------ ");
        }
    }
}