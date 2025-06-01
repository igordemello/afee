using System.ComponentModel.DataAnnotations;

namespace tcc_in305b.ViewModels
{
    public class CreateTreinadorViewModel
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
    }
}
