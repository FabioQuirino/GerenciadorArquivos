using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WinSCP;

namespace GerenciadorArquivos.Servico
{
    public class GerenciadorArquivos : BaseServico
    {
        public List<string> VerificarVersao(string host, string user, string password)
        {
            base.SetPermission(host, user, password);
            List<string> arquivos = new List<string>();
            var nomeArquivo1 = new string[] { "ZonaSul.Framework.Web.dll" };
            var nomeArquivo2 = new string[] { "ZonaSul.Mvc.Autenticacao.dll" };
            string diretorioLocal = @"c:\tempVerifica";
            using (var session = GetNewSession())
            {
                var diretorios = session
                    .ListDirectory(@"/FTP-service/novaintranet")
                    .Files
                    .Where(x => x.IsDirectory && !x.Name.Equals(".."));

                foreach (RemoteFileInfo diretorio in diretorios)
                {
                    try
                    {
                        
                        var arquivoZsFramework = session
                            .ListDirectory($@"/FTP-service/novaintranet/{diretorio.Name}/bin")
                            .Files
                            .FirstOrDefault(x => !x.IsDirectory && nomeArquivo1.Contains(x.Name));

                        if (arquivoZsFramework == null)
                        {
                            arquivoZsFramework = session
                                .ListDirectory($@"/FTP-service/novaintranet/{diretorio.Name}/bin")
                                .Files
                                .FirstOrDefault(x => !x.IsDirectory && nomeArquivo2.Contains(x.Name));
                        }

                        var ultAtualizacao = session
                            .ListDirectory($@"/FTP-service/novaintranet/{diretorio.Name}/bin")
                            .Files
                            .Where(x => !x.IsDirectory)
                            .OrderByDescending(x => x.LastWriteTime)
                            .FirstOrDefault();

                        if (arquivoZsFramework != null)
                        {
                            var resultFila = session.GetFileToDirectory($@"/FTP-service/novaintranet/{diretorio.Name}/bin/{arquivoZsFramework.Name}", diretorioLocal, options: new TransferOptions() {OverwriteMode = OverwriteMode.Overwrite});
                            FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(string.Concat(diretorioLocal,"\\", arquivoZsFramework.Name));
                            arquivos.Add($"{diretorio.Name};{arquivoZsFramework.Name};{myFileVersionInfo.FileVersion};{System.IO.File.GetLastWriteTime(string.Concat(diretorioLocal, "\\", arquivoZsFramework.Name)):dd/MM/yyyy HH:mm:ss};{ultAtualizacao?.Name};{ultAtualizacao?.LastWriteTime:dd/MM/yyyy HH:mm:ss}");
                        }
                        else
                        {
                            arquivos.Add($"{diretorio.Name};não");
                        }
                    }
                    catch (Exception e)
                    {
                    }
                }
            }

            return arquivos;
        }

    }
}
