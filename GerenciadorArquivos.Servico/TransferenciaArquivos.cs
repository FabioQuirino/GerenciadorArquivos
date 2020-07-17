using System;
using WinSCP;

namespace GerenciadorArquivos.Servico
{
    public class TransferenciaArquivos
    {
        public static bool Enviar(string Parm_Destination, string Parm_User, string Parm_PWord)
        {
            try
            {
                // Setup session options
                var sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Ftp,
                    PortNumber = 990,
                    HostName = Parm_Destination,
                    UserName = Parm_User,
                    Password = Parm_PWord
                };

                using (var session = new Session())
                {
                    // Connect
                    session.SessionLogPath = @"c:\logftp\WinSCP_Send_File.log";
                    session.Open(sessionOptions);
                    // Upload files
                    var transferOptions = new TransferOptions
                    {
                        TransferMode = TransferMode.Binary
                    };
                    // local   SFTP site
                    var transferResult = session.PutFiles(@"c:\testeftp\teste.txt", "//", false, transferOptions);
                    // Throw on any error
                    // Print results
                    transferResult.Check();
                    foreach (TransferEventArgs transfer in transferResult.Transfers)
                        Console.WriteLine("Upload of {0} succeeded", transfer.FileName);
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
                return false;
            }
        }
    }
}