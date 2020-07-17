using System;
using WinSCP;

namespace GerenciadorArquivos.Servico
{
    public class TransferenciaArquivos : BaseServico
    {
        private TransferenciaArquivos() { }

        public TransferenciaArquivos(string Parm_Destination, string Parm_User, string Parm_PWord)
        {
            base.Parm_Destination = Parm_Destination;
            base.Parm_User = Parm_User;
            base.Parm_PWord = Parm_PWord;
        }

        public void Enviar()
        {
            var sessionOptions = GetSessionOptions();

            using (var session = new Session())
            {
                // Connect
                session.SessionLogPath = @"c:\testeftp\WinSCP_Send_File.log";
                session.Open(sessionOptions);
                // Upload files
                var transferOptions = new TransferOptions
                {
                    TransferMode = TransferMode.Binary
                };
                // local   SFTP site
                var transferResult = session.PutFiles(@"c:\testeftp\teste.txt", "/FTP-extranet/solicitacao.v2/img/", false, transferOptions);
                //var transferResult = session.GetFiles( "/FTP-extranet/solicitacao.v2/img", @"c:\testeftp\", false, transferOptions);
                // Throw on any error
                // Print results
                transferResult.Check();
                foreach (TransferEventArgs transfer in transferResult.Transfers)
                    Console.WriteLine("Upload of {0} succeeded", transfer.FileName);
            }
        }
    }
}