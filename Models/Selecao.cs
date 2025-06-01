namespace tcc_in305b.Models
{
    public class Selecao
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int TreinadorId { get; set; }
        public Treinador? Treinador { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public string? Convidado { get; set; }

        public ICollection<SelecaoPlayer> SelecaoPlayers { get; set; } = new List<SelecaoPlayer>();
        public ICollection<Round> Rounds { get; set; } = new List<Round>();
        public ICollection<Equipe> Equipes { get; set; } = new List<Equipe>();
        public ICollection<AnaliseSelecao> AnaliseSelecoes { get; set; } = new List<AnaliseSelecao>();
    }
}
