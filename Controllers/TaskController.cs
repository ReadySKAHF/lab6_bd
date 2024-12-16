using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers
{
    public class CarsController : Controller
    {
        // Метод для отображения страницы управления автомобилями
        [HttpGet]
        public IActionResult Index()
        {
            return View();  // Возвращаем представление Index.cshtml
        }
    }
}
