namespace tcc_in305b.Models
{
    public class AnaliseNota
    {
        public int AnaliseId { get; set; }
        public Analise Analise { get; set; }

        public int NotaId { get; set; }
        public Nota Nota { get; set; }

        public double? Valor { get; set; }
    }
}
