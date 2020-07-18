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
            base.padrao = arquivos;
            using (var session = GetNewSession())
            {
                // var transferResult = session.PutFiles(local, remoto, options: TransferOptions);
                foreach (var arquivo in arquivos)
                {
                    var transferResult = session.PutFiles($"{local}{arquivo}", $"{remoto}{arquivo.Replace(@"\", "/")}");
                    transferResult.Check();
                }
            }
        }
    }
}