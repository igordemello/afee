namespace tcc_in305b.Models
{
    public class FaseTreinador
    {
        public int FaseId { get; set; }
        public Fase Fase { get; set; }

        public int TreinadorId { get; set; }
        public Treinador Treinador { get; set; }
    }
}
