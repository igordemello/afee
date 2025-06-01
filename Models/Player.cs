using Microsoft.AspNetCore.Identity;
using System.Configuration;

namespace tcc_in305b.Models
{
   
    public class Player
    {
        

        public int Id { get; set; }
        public required string Nome { get; set; }
        public bool? FgCandidato { get; set; }
        public required string Status { get; set; }
        public required string Nickname { get; set; }
        public required string Tag { get; set; }
        public int? GrupoId { get; set; }
        public Grupo Grupo { get; set; }

        public int? EquipeId { get; set; }
        public Equipe Equipe { get; set; }

        public string UserId { get; set; }
        public IdentityUser User { get; set; }
        public int? TreinadorId { get; set; }
        public Treinador Treinador { get; set; }

        public DateTime? DtCriacao { get; set; }

        public string? EloCriacao { get; set; }

        public string? Puuid { get; set; }

        public int? FaseId { get; set; }
        public Fase Fase { get; set; }

        public string? IGL { get; set; }

        public string? Classe { get; set; }
        public DateTime DataNascimento { get; set; }

        public int Idade
        {
            get
            {
                var hoje = DateTime.Today;
                var idade = hoje.Year - DataNascimento.Year;

                if (DataNascimento.Date > hoje.AddYears(-idade))
                {
                    idade--;
                }

                return idade;
            }
        }


        public ICollection<HistoricoPlayerGrupo> HistoricoPlayerGrupos { get; set; } = new List<HistoricoPlayerGrupo>();
        public ICollection<AnalisePlayer> AnalisePlayers { get; set; } = new List<AnalisePlayer>();
        public ICollection<AnalisePlayerGrupo> AnalisePlayerGrupos { get; set; } = new List<AnalisePlayerGrupo>();
        public ICollection<SelecaoPlayer> SelecaoPlayers { get; set; } = new List<SelecaoPlayer>();
        public ICollection<PlayerHistorico> PlayerHistoricos { get; set; } = new List<PlayerHistorico>();



    }
}
