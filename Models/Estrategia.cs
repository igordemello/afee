using System.ComponentModel.DataAnnotations;

namespace tcc_in305b.Models
{
    public class Estrategia
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descrição { get; set; }
        public int TreinadorId { get; set; } 
        public Treinador Treinador { get; set; }
        public ICollection<EstrategiaGrupo> EstrategiaGrupos { get; set; }

    }
}
