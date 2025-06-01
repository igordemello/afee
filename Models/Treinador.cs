using Microsoft.AspNetCore.Identity;

namespace tcc_in305b.Models
{
    public class Treinador
    {
        public int Id { get; set; }
        public required string Nome { get; set; }

        
        public string UserId { get; set; }
        public IdentityUser User { get; set; }

        public ICollection<ToDoTreinador> ToDoTreinadores { get; set; } 
        public ICollection<TreinoTipo> TreinoTipos { get; set; } 
        public ICollection<FaseTreinador> FaseTreinadores { get; set; }
        public ICollection<Selecao> Selecoes { get; set; } = new List<Selecao>();
        public ICollection<Equipe> Equipes { get; set; } = new List<Equipe>();

        public ICollection<Analise> Analises { get; set; } = new List<Analise>();
        public ICollection<Nota> Notas { get; set; } = new List<Nota>();

        public ICollection<Torneio> Torneios { get; set; } = new List<Torneio>();
    }
}
