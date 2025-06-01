namespace tcc_in305b.Models
{
    public class EquipePartida
    {
        public int EquipeId { get; set; }
        public Equipe Equipe { get; set; }

        public int PartidaId { get; set; }
        public Partida Partida { get; set; }

        public int? Pontuacao { get; set; }
    }
}
