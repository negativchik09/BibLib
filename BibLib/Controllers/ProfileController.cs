using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibLib.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        // GET
        public IActionResult Profile()
        {
            return View();
        }
    }
}