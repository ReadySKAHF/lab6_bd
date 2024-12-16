using Microsoft.AspNetCore.Mvc;
using App.Models;
using Microsoft.EntityFrameworkCore;
using App.Data;
using Swashbuckle.AspNetCore.Annotations; // Подключаем для Swagger

namespace App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarServicesAPIController : ControllerBase
    {
        private readonly AutoRepairWorkshopContext _context;

        public CarServicesAPIController(AutoRepairWorkshopContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получить список всех услуг по обслуживанию автомобилей.
        /// </summary>
        /// <returns>Список услуг по обслуживанию автомобилей с механиками и типами услуг.</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Получить список всех услуг по обслуживанию автомобилей", Description = "Возвращает все записи об услугах, предоставленных для автомобилей, включая механиков и типы услуг.")]
        [SwaggerResponse(200, "Список услуг успешно получен", typeof(IEnumerable<CarService>))]
        public async Task<ActionResult<IEnumerable<CarService>>> GetCarServices()
        {
            var carServices = await _context.CarServices
                .Include(cs => cs.Service)     // Включаем информацию о типе услуги
                .Include(cs => cs.Mechanic)    // Включаем информацию о механике
                .ToListAsync();

            return Ok(carServices);
        }

        /// <summary>
        /// Получить услугу по обслуживанию автомобиля по ID.
        /// </summary>
        /// <param name="id">ID услуги по обслуживанию автомобиля.</param>
        /// <returns>Детальная информация о конкретной услуге.</returns>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Получить услугу по обслуживанию автомобиля по ID", Description = "Возвращает информацию об услуге по обслуживанию автомобиля по его уникальному ID.")]
        [SwaggerResponse(200, "Услуга найдена", typeof(CarService))]
        [SwaggerResponse(404, "Услуга не найдена")]
        public async Task<ActionResult<CarService>> GetCarService(int id)
        {
            var carService = await _context.CarServices
                .Include(cs => cs.Service)     // Включаем информацию о типе услуги
                .Include(cs => cs.Mechanic)    // Включаем информацию о механике
                .FirstOrDefaultAsync(cs => cs.CarServiceId == id);

            if (carService == null)
            {
                return NotFound();
            }

            return Ok(carService);
        }

        /// <summary>
        /// Добавить новую услугу по обслуживанию автомобиля.
        /// </summary>
        /// <param name="carService">Данные новой услуги по обслуживанию.</param>
        /// <returns>Созданная услуга с присвоенным ID.</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Создать новую услугу по обслуживанию автомобиля", Description = "Добавляет новую услугу по обслуживанию автомобиля в базу данных.")]
        [SwaggerResponse(201, "Услуга успешно создана", typeof(CarService))]
        public async Task<ActionResult<CarService>> PostCarService(CarService carService)
        {
            _context.CarServices.Add(carService);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCarService), new { id = carService.CarServiceId }, carService);
        }

        /// <summary>
        /// Удалить услугу по обслуживанию автомобиля по ID.
        /// </summary>
        /// <param name="id">ID услуги для удаления.</param>
        /// <returns>Статус успешного удаления.</returns>
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Удалить услугу по обслуживанию автомобиля", Description = "Удаляет услугу по обслуживанию автомобиля по его ID.")]
        [SwaggerResponse(204, "Услуга успешно удалена")]
        [SwaggerResponse(404, "Услуга не найдена")]
        public async Task<IActionResult> DeleteCarService(int id)
        {
            var carService = await _context.CarServices.FindAsync(id);
            if (carService == null)
            {
                return NotFound();
            }

            _context.CarServices.Remove(carService);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
