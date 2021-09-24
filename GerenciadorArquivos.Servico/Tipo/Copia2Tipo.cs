using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerenciadorArquivos.Servico.Tipo
{
    public class Copia2Tipo
    {
        public string Aplicacao { get; set; }
        public string Destino { get; set; }
        public string Local { get; set; }
        public string Password { get; set; }
        public string Server { get; set; }
        public string User { get; set; }
        public string[] Excecoes { get; set; }
    }
}
