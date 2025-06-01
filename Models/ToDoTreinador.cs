using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace tcc_in305b.Models
{
    public class ToDoTreinador
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Treinador")]
        public int TreinadorId { get; set; }
        public Treinador Treinador { get; set; }

        [Required]
        [MaxLength(200)]
        public string Descricao { get; set; }

        public bool Feito { get; set; } = false;

        [Required]
        [MaxLength(40)]
        public string Nome { get; set; }

        public DateTime DtCriacao { get; set; } = DateTime.Now;
    }
}
