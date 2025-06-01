using System.ComponentModel.DataAnnotations;

namespace tcc_in305b.ViewModels
{
    public class CreatePlayerViewModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
        public string Nickname { get; set; }

        [Required]
        public string Tag { get; set; }

        public string Status { get; set; } = "Candidato";

        [Required]
        public DateTime DataNascimento { get; set; }

        [Required]
        public string Classe { get; set; }

        [Required]
        public string IGL { get; set; }
    }

}
