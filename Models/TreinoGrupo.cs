namespace tcc_in305b.Models
{
    public class TreinoGrupo
    {
        public int GrupoId { get; set; } 
        public Grupo Grupo { get; set; } 

        public int TreinoId { get; set; } 
        public Treino Treino { get; set; } 
    }
}
