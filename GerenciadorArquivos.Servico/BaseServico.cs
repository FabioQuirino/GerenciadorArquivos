using WinSCP;

namespace GerenciadorArquivos.Servico
{
   public class BaseServico
    {
        protected SessionOptions GetSessionOptions()
        {
            var sessionOptions = new SessionOptions
            {
                Protocol = Protocol.Ftp,
                PortNumber = 990,
                HostName = Parm_Destination,
                UserName = Parm_User,
                Password = Parm_PWord,
                FtpSecure = FtpSecure.Implicit
            };
            return sessionOptions;
        }

        protected string Parm_PWord { get; set; }

        protected string Parm_User { get; set; }

        protected string Parm_Destination { get; set; }
    }
}