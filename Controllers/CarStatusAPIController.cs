using Microsoft.AspNetCore.Mvc;
using App.Models;
using Microsoft.EntityFrameworkCore;
using App.Data;
using Swashbuckle.AspNetCore.Annotations; // Подключаем для Swagger

namespace App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarStatusAPIController : ControllerBase
    {
        private readonly AutoRepairWorkshopContext _context;

        public CarStatusAPIController(AutoRepairWorkshopContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получить список всех статусов автомобилей.
        /// </summary>
        /// <returns>Список всех статусов автомобилей.</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Получить список сотрудников", Description = "Возвращает список сотрудников, включая их базовую информацию.")]
        [SwaggerResponse(200, "Список статусов успешно получен", typeof(IEnumerable<CarStatus>))]
        public async Task<ActionResult<IEnumerable<CarStatus>>> GetCarStatuses()
        {
            var carStatuses = await _context.CarStatus.ToListAsync();
            return Ok(carStatuses);
        }

        /// <summary>
        /// Получить статус автомобиля по ID.
        /// </summary>
        /// <param name="id">ID статуса автомобиля.</param>
        /// <returns>Детальная информация о статусе автомобиля.</returns>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Получить статус автомобиля по ID", Description = "Возвращает информацию о статусе автомобиля по его уникальному ID.")]
        [SwaggerResponse(200, "Статус найден", typeof(CarStatus))]
        [SwaggerResponse(404, "Статус не найден")]
        public async Task<ActionResult<CarStatus>> GetCarStatus(int id)
        {
            var carStatus = await _context.CarStatus.FindAsync(id);

            if (carStatus == null)
            {
                return NotFound();
            }

            return Ok(carStatus);
        }

        /// <summary>
        /// Добавить новый статус автомобиля.
        /// </summary>
        /// <param name="carStatus">Данные нового статуса автомобиля.</param>
        /// <returns>Созданный статус автомобиля.</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Добавить новый статус автомобиля", Description = "Добавляет новый статус автомобиля в систему.")]
        [SwaggerResponse(201, "Статус успешно создан", typeof(CarStatus))]
        public async Task<ActionResult<CarStatus>> PostCarStatus(CarStatus carStatus)
        {
            _context.CarStatus.Add(carStatus);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCarStatus), new { id = carStatus.StatusId }, carStatus);
        }

        /// <summary>
        /// Удалить статус автомобиля по ID.
        /// </summary>
        /// <param name="id">ID статуса для удаления.</param>
        /// <returns>Статус успешного удаления.</returns>
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Удалить статус автомобиля", Description = "Удаляет статус автомобиля по его уникальному ID.")]
        [SwaggerResponse(204, "Статус успешно удален")]
        [SwaggerResponse(404, "Статус не найден")]
        public async Task<IActionResult> DeleteCarStatus(int id)
        {
            var carStatus = await _context.CarStatus.FindAsync(id);
            if (carStatus == null)
            {
                return NotFound();
            }

            _context.CarStatus.Remove(carStatus);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
