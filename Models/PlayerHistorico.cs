namespace tcc_in305b.Models
{
    public class PlayerHistorico
    {
        public int Id { get; set; } 
        public int PlayerId { get; set; } 
        public Player Player { get; set; } 
        public DateTime DataEvento { get; set; } 
        public string TipoEvento { get; set; } 
        public string? Descricao { get; set; } 
        public string? DadosAlterados { get; set; }
    }
}
