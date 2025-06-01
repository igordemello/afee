namespace tcc_in305b.Models
{
    public class AnaliseSelecao
    {
        public int AnaliseId { get; set; }
        public Analise Analise { get; set; }

        public int SelecaoId { get; set; }
        public Selecao Selecao { get; set; }
    }
}
