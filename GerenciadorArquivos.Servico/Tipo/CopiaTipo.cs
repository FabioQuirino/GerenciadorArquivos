using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerenciadorArquivos.Servico.Tipo
{
    public class CopiaTipo
    {
        public string[] Arquivos { get; set; }
        public string De { get; set; }
        public string Para { get; set; }
        public string Aplicacao { get; set; }
        public bool GeraBackup { get; set; }
    }
}
