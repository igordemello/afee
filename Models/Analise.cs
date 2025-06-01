namespace tcc_in305b.Models
{
    public class Analise
    {
        public int Id { get; set; }
        public int TreinadorId { get; set; }
        public Treinador Treinador { get; set; }
        public DateTime Data { get; set; }
        public string Descricao { get; set; }

        public ICollection<AnaliseNota> AnaliseNotas { get; set; } = new List<AnaliseNota>();
        public ICollection<AnalisePlayer> AnalisePlayers { get; set; } = new List<AnalisePlayer>();
        public ICollection<AnaliseGrupo> AnaliseGrupos { get; set; } = new List<AnaliseGrupo>();
        public ICollection<AnalisePlayerGrupo> AnalisePlayerGrupos { get; set; } = new List<AnalisePlayerGrupo>();
        public ICollection<AnaliseSelecao> AnaliseSelecoes { get; set; } = new List<AnaliseSelecao>();
    }
}
