namespace tcc_in305b.Models
{
    public class Partida
    {
        public int Id { get; set; }
        public int? RoundId { get; set; }
        public Round Round { get; set; }
        public int? ChaveamentoId { get; set; }
        public Chaveamento Chaveamento { get; set; }
        public DateTime Data { get; set; }
        public string Tipo { get; set; }

        public ICollection<EquipePartida> EquipePartidas { get; set; } = new List<EquipePartida>();
    }
}
