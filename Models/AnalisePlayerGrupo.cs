namespace tcc_in305b.Models
{
    public class AnalisePlayerGrupo
    {
        public int AnaliseId { get; set; }
        public Analise Analise { get; set; }

        public int GrupoId { get; set; }
        public Grupo Grupo { get; set; }

        public int PlayerId { get; set; }
        public Player Player { get; set; }

        public string Formacao { get; set; }

    }
}
