namespace tcc_in305b.Models
{
    public class EstrategiaGrupo
    {
        public int GrupoId { get; set; } 
        public Grupo Grupo { get; set; } 

        public int EstrategiaId { get; set; } 
        public Estrategia Estrategia { get; set; } 
    }
}
