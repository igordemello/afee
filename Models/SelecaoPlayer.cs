namespace tcc_in305b.Models
{
    public class SelecaoPlayer
    {
        public int SelecaoId { get; set; }
        public Selecao Selecao { get; set; }

        public int PlayerId { get; set; }
        public Player Player { get; set; }
    }
}
