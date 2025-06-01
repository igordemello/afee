using tcc_in305b.Models;

namespace tcc_in305b.ViewModels
{

    public class PlayerViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
    }

    public class FaseDetalhesViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Status { get; set; }
        public List<string> Treinadores { get; set; }
        public List<PlayerViewModel> Players { get; set; }
    }

    
}
