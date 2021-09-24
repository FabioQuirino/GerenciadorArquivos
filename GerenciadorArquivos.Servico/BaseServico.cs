using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
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
        public string ArquivoLogFtp { get; set; }
        public string ArquivoBkpZip { get; set; }
        protected bool GeraLogFtp { get; set; }
        public bool GeraBackup { get; set; }
        public List<string> Resultado { get; set; }

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

            if (GeraLogFtp)
            {
                ArquivoLogFtp = $@"c:\testeftp\ftp-session-{DateTime.Now:yyyyMMddHHmmss}.log";
                session.SessionLogPath = ArquivoLogFtp;
            }
            session.Open(sessionOptions);
            return session;
        }

        protected void SetPermission(string host, string user, string password)
        {
            Host = host;
            User = user;
            Password = password;
        }
    }
}