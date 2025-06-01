namespace tcc_in305b.Models
{
    public class Round
    {
        public int Id { get; set; }
        public int SelecaoId { get; set; }
        public Selecao Selecao { get; set; }
        public DateTime Data { get; set; }
        public ICollection<Partida>? Partidas { get; set; } = new List<Partida>();
    }
}
