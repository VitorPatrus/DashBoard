using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BI.Sistemas.Domain.Novo
{
    public class GeracaoTime
    {
        public int GeracaoId { get; set; }
        public Geracao Geracao { get; set; }
        public int Id { get; set; }
        public int ConclusaoCards { get; set; }
        public int SpEntregues { get; set; }
        public int BacklogTotal { get; set; }
        public int BacklogNovo { get; set; }
        public int Wip { get; set; }
        public int Bug { get; set; }
        public int LeadTime { get; set; }
        public int CicleTime { get; set; }

        public int GetId()
        {
            return Geracao.Id;
        }
    }
}

