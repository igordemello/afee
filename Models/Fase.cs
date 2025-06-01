namespace tcc_in305b.Models
{
    public class Fase
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public ICollection<FaseTreinador> FaseTreinadores { get; set; }
        public string Status { get; set; }

        public ICollection<Player> Players { get; set; }
    }
}
