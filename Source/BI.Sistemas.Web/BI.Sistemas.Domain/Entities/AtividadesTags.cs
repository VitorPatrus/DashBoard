using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BI.Sistemas.Domain.Novo
{
    public class AtividadesTags
    {
        private Dictionary<string, int> atividades;

        public AtividadesTags(Dictionary<string, int> atividades)
        {
            this.atividades = atividades;
        }
    }
}
