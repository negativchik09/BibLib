using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BibLib.Controllers
{
    public class Home
    {
        public IActionResult Index()
        {
            return View("Index");
        }
    }
}