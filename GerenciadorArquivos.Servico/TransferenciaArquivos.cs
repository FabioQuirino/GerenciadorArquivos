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

        public void CopiarTudo(string aplicacao, string local, string remoto, string[] excecoes)
        {
            var arquivos = ListarLocal(aplicacao, local);
            Copiar(aplicacao, local, remoto, arquivos, excecoes);
        }

        public void Copiar(string fromAppdir, string toAppDir, string[] arquivos)
        {
            using (var session = GetNewSession())
            {
                foreach (var arquivo in arquivos)
                {
                    var arquivoLocal = $@"{fromAppdir}\{arquivo}";
                    var arquivoRemoto = $@"{toAppDir}\{arquivo.Replace(@"\", "/")}";

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
            }
        }

        public void Copiar(string aplicacao, string local, string remoto, string[] arquivos, string[] excecoes)
        {
            using (var session = GetNewSession())
            {
                Backup(session, aplicacao, local, remoto, arquivos, excecoes);
                Resultado.Add("Cópia dos arquivos");

                if (excecoes != null && excecoes.Any())
                {
                    arquivos = arquivos.Where(x => !excecoes.Any(y => y.ToLower().Equals(x.ToLower()))).ToArray();
                }

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

        private void Backup(Session sessionFtp, string aplicacao, string local, string remoto, string[] arquivos, string[] excecoes)
        {
            if (!GeraBackup)
            {
                return;
            }

            Resultado.Add($"Processamento do backup ------------------ ");
            var dataHoraAtual = DateTime.Now.ToString("yyyyMMddHHmmss");

            var diretorioBackup = $"{aplicacao}-bkp-{dataHoraAtual}";
            string diretorioTemporario = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(diretorioTemporario);
            sessionFtp.CreateDirectory($@"{remoto}/{diretorioBackup}");

            foreach (var arquivo in arquivos.Where(x => !excecoes.Any(y => y.ToLower().Equals(x.ToLower()))).ToArray())
            {
                var arquivoRemoto = $@"{remoto}\{aplicacao}\{arquivo}".Replace(@"\", "/").Replace(@"\\", @"\");
                var arquivoRemotoBackup = $@"{remoto}/{diretorioBackup}\{arquivo}".Replace(@"\", "/").Replace(@"\\", @"\");
                
                // var arquivoRemotoBackup = $@"{remoto}/{aplicacao}/{diretorioBackup}\{arquivo}".Replace(@"\", "/").Replace(@"\\", @"\");
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
            /*
            var result = sessionFtp.SynchronizeDirectories(SynchronizationMode.Local, diretorioTemporario, $@"{remoto}/{aplicacao}/{diretorioBackup}\", false);
            result.Check();

            var remove = sessionFtp.RemoveFiles($@"{remoto}/{aplicacao}/{diretorioBackup}\");
            remove.Check();
            ArquivoBkpZip = $@"c:\teste\{aplicacao}-{base.Host}-bkp-{dataHoraAtual}.zip";
            ZipFile.CreateFromDirectory(diretorioTemporario + @"\", ArquivoBkpZip, CompressionLevel.Optimal, false);
            */
            Resultado.Add($"Fim do processamento do backup ------------------ ");
        }
    }
}