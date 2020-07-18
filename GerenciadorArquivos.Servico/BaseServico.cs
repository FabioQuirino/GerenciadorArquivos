using System;
using System.Net;
using WinSCP;

namespace GerenciadorArquivos.Servico
{
    public class BaseServico
    {
        protected string[] padrao { get; set; }

        protected string[] excecoes => new string[]
        {
            "web.config",
            "web.*.config",
            "*.pdb",
            "*.less"
        };

        protected TransferOptions TransferOptions => new TransferOptions
        {
            TransferMode = TransferMode.Automatic,
            FileMask = $"{string.Join(";", padrao)}|{string.Join(";", excecoes)}"
        };

        protected string Password { get; set; }
        protected string User { get; set; }
        protected string Host { get; set; }
        protected bool GeraLog { get; set; }

        protected SessionOptions GetSessionOptions()
        {
            var sessionOptions = new SessionOptions
            {
                Protocol = Protocol.Ftp,
                PortNumber = 990,
                HostName = Host,
                UserName = User,
                Password = Password,
                FtpSecure = FtpSecure.Implicit
            };

            return sessionOptions;
        }

        protected Session GetNewSession()
        {
            var sessionOptions = GetSessionOptions();
            var session = new Session();

            if (GeraLog)
            {
                session.SessionLogPath = $@"c:\testeftp\ftp-session-{DateTime.Now:yyyyMMddHHmmss}.log";
            }
            session.Open(sessionOptions);
            return session;
        }
    }
}