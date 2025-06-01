namespace tcc_in305b.ViewModels
{
    public class CriarAnaliseViewModel
    {
        public string Descricao { get; set; }
        public string Tipo { get; set; } // Player, Grupo, PlayerEmGrupo
        public int AlvoId { get; set; } // ID do Player ou Grupo
        public int? GrupoId { get; set; } // Apenas para PlayerEmGrupo
        public List<NotaViewModel> Notas { get; set; } = new List<NotaViewModel>();
    }

    public class NotaViewModel
    {
        public int NotaId { get; set; }
        public double? Valor { get; set; }
    }
}
