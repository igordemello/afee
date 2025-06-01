namespace tcc_in305b.Models
{
    public class Torneio
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Tipo { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public string? Convidado { get; set; }
        public int QuantidadeEquipes { get; set; }
        public int? TreinadorId {  get; set; }
        public Treinador? Treinador { get; set; }


        public ICollection<Chaveamento> Chaveamentos { get; set; } = new List<Chaveamento>();
        public ICollection<EquipeTorneio> EquipeTorneios { get; set; } = new List<EquipeTorneio>();

    }
}
