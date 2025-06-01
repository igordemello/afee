using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

public class CreateFaseViewModel
{
    [Required]
    public string Nome { get; set; }

    [Required]
    public string Status { get; set; }

    [Required(ErrorMessage = "Por favor, selecione ao menos um treinador.")]
    public List<int> TreinadorIds { get; set; } = new List<int>();

    public IEnumerable<SelectListItem>? Treinadores { get; set; }
}