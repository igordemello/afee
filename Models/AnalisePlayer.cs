namespace tcc_in305b.Models
{
    public class AnalisePlayer
    {
        public int AnaliseId { get; set; }
        public Analise Analise { get; set; }

        public int PlayerId { get; set; }
        public Player Player { get; set; }
    }
}
