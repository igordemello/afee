namespace tcc_in305b.Models
{
    public class Nota
    {
        public int Id { get; set; }
        public int TreinadorId { get; set; }
        public Treinador Treinador { get; set; }
        public string Nome { get; set; }

        public ICollection<AnaliseNota>? AnaliseNotas { get; set; } = new List<AnaliseNota>();
        
    }
}
