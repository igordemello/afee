using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace tcc_in305b.Models
{
    public class TreinoTipo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O código é obrigatório.")]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A cor é obrigatória.")]
        [RegularExpression("^#(?:[0-9a-fA-F]{3}){1,2}$", ErrorMessage = "Formato de cor inválido. Use hexadecimal.")]
        public string CorHex { get; set; }

        [Required(ErrorMessage = "O campo TreinadorId é obrigatório.")]
        public int TreinadorId { get; set; }

        public Treinador Treinador { get; set; }

        
        public ICollection<Treino>? Treinos { get; set; }
    }
}
