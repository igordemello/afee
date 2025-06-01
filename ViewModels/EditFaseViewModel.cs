using Microsoft.AspNetCore.Mvc.Rendering;

namespace tcc_in305b.ViewModels
{
    public class EditFaseViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Status { get; set; }
        public List<int> TreinadorIds { get; set; }
        public List<SelectListItem>? Treinadores { get; set; }
    }
}
