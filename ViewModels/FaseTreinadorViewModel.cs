using tcc_in305b.Models;

namespace tcc_in305b.ViewModels
{
    public class FaseTreinadorViewModel
    {
        public IEnumerable<Fase> Fases { get; set; }
        public IEnumerable<FaseTreinador> FaseTreinadores { get; set; }
        public List<FaseViewModel> FaseDetalhes { get; set; }
        public IEnumerable<Treinador> Treinadores { get; set; }
    }
}
