namespace tcc_in305b.Models
{
    public class TreinoPlayer
    {
        public int PlayerId { get; set; } 
        public Player Player { get; set; } 

        public int TreinoId { get; set; } 
        public Treino Treino { get; set; } 
    }
}
