namespace tcc_in305b.Models
{
    public class Equipe
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int TreinadorId { get; set; }
        public Treinador? Treinador { get; set; }
        public string? Status { get; set; }
        public DateTime DtCriacao { get; set; }
        public string? Tipo { get; set; }

        public int? SelecaoId { get; set; }
        public Selecao? Selecao { get; set; }

        public ICollection<Player>? Players { get; set; }
        public ICollection<EquipePartida> EquipePartidas { get; set; } = new List<EquipePartida>();

        public ICollection<EquipeTorneio> EquipeTorneios { get; set; } = new List<EquipeTorneio>();



    }
}
