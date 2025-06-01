namespace tcc_in305b.Models
{
    public class Treino
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int TreinadorId { get; set; } 
        public Treinador Treinador { get; set; }

        public int TreinoTipoId { get; set; }
        public TreinoTipo TreinoTipo { get; set; }

        public DateTime Data { get; set; }  
        public TimeSpan HorarioInicio { get; set; }  
        public TimeSpan HorarioFim { get; set; }    

        public int? EstrategiaId { get; set; }
        public Estrategia Estrategia { get; set; }

        
        public ICollection<TreinoPlayer> TreinoPlayers { get; set; }
        public ICollection<TreinoGrupo> TreinoGrupos { get; set; }

    }
}
