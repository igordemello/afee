namespace tcc_in305b.Models
{
    public class EquipeTorneio
    {
        public int EquipeId { get; set; }
        public Equipe Equipe { get; set; }

        public int TorneioId { get; set;}
        public Torneio Torneio { get; set; }

    }
}
