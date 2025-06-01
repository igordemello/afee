using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tcc_in305b.Models
{
    public class Grupo
    {
        public int Id { get; set; }

        public string Nome { get; set; }
        public int TreinadorId { get; set; } 
        public Treinador Treinador { get; set; }
        public string Status { get; set; }
        public DateTime DtCriacao { get; set; } 
        public string Tipo { get; set; }

        public ICollection<Player>? Players { get; set; }
        public ICollection<HistoricoPlayerGrupo>? HistoricoPlayerGrupos { get; set; } = new List<HistoricoPlayerGrupo>();
        public ICollection<AnaliseGrupo> AnaliseGrupos { get; set; } = new List<AnaliseGrupo>();
        public ICollection<AnalisePlayerGrupo> AnalisePlayerGrupos { get; set; } = new List<AnalisePlayerGrupo>();

    }
}
