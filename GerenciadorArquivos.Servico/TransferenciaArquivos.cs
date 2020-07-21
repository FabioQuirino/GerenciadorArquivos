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
            base.GeraLog = geralog;
            base.Resultado = new List<string>();
        }

        public void CopiarTudo(string local, string remoto)
        {
            var arquivos = new DirectoryInfo(local).GetFiles("*", SearchOption.AllDirectories).Select(x => x.FullName.Replace(local, "")).ToArray();
            Copiar(local, remoto, arquivos);
        }

        public void Copiar(string local, string remoto, string[] arquivos)
        {
            using (var session = GetNewSession())
            {
                Backup(session, local, remoto, arquivos);
                Resultado.Add("Cópia dos arquivos");

                foreach (var arquivo in arquivos)
                {
                    var arquivoLocal = $"{local}{arquivo}";
                    var arquivoRemoto = $"{remoto}{arquivo.Replace(@"\", "/")}";

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

        private void Backup(Session sessionFtp, string local, string remoto, string[] arquivos)
        {
            Resultado.Add($"Processamento do backup ------------------ ");
            var dataHoraAtual = DateTime.Now.ToString("yyyyMMddHHmmss");
            var diretorioBackup = $"backup-{dataHoraAtual}";

            string diretorioTemporario = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(diretorioTemporario);

            foreach (var arquivo in arquivos)
            {
                var arquivoRemoto = $@"{remoto}\{arquivo}".Replace(@"\", "/").Replace(@"\\", @"\");
                var arquivoRemotoBackup = $@"{remoto}\{diretorioBackup}\{arquivo}".Replace(@"\", "/").Replace(@"\\", @"\");
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

            var result = sessionFtp.SynchronizeDirectories(SynchronizationMode.Local, diretorioTemporario, $@"{remoto}/{diretorioBackup}\", false);
            result.Check();

            var remove = sessionFtp.RemoveFiles($@"{remoto}/{diretorioBackup}\");
            remove.Check();

            ZipFile.CreateFromDirectory(diretorioTemporario + @"\", $@"c:\teste\{DateTime.Now:yyyyMMddHHmmss}.zip", CompressionLevel.Optimal, true);

            Resultado.Add($"Fim do processamento do backup ------------------ ");
        }
    }
}