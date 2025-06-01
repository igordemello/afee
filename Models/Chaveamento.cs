namespace tcc_in305b.Models
{
    public class Chaveamento
    {
        public int Id { get; set; }
        public int TorneioId { get; set; }
        public Torneio Torneio { get; set; }
        public int? AnteriorId { get; set; }
        public Chaveamento Anterior { get; set; }
        public string Nome {  get; set; }
        public DateTime Data { get; set; }

        public ICollection<Chaveamento> Proximos { get; set; }
        public ICollection<Partida> Partidas { get; set; } = new List<Partida>();
    }
}
