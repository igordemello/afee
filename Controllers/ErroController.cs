using Microsoft.AspNetCore.Mvc;

namespace tcc_in305b.Controllers
{
    public class ErroController : Controller
    {
        [Route("Erro/404")]
        public IActionResult NotFoundPage()
        {
            return View("NotFound");
        }
    }
}
