using Microsoft.AspNetCore.Mvc;
using App.Models;
using Microsoft.EntityFrameworkCore;
using App.Data;
using Swashbuckle.AspNetCore.Annotations; // Подключаем для Swagger

namespace App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsAPIController : ControllerBase
    {
        private readonly AutoRepairWorkshopContext _context;

        public CarsAPIController(AutoRepairWorkshopContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получить список всех автомобилей.
        /// </summary>
        /// <returns>Список автомобилей с владельцами и заказами на ремонт.</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Получить список автомобилей", Description = "Возвращает список всех автомобилей с владельцами и статусами заказов на ремонт.")]
        [SwaggerResponse(200, "Список автомобилей успешно получен", typeof(IEnumerable<Car>))]
        public async Task<ActionResult<IEnumerable<Car>>> GetCars()
        {
            var cars = await _context.Cars
                .Take(10)
                .Include(c => c.Owner)
                .Include(c => c.RepairOrders)
                    .ThenInclude(ro => ro.Status)
                .ToListAsync();

            return Ok(cars);
        }

        /// <summary>
        /// Получить автомобиль по ID.
        /// </summary>
        /// <param name="id">ID автомобиля.</param>
        /// <returns>Детальная информация об автомобиле.</returns>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Получить автомобиль по ID", Description = "Возвращает информацию о конкретном автомобиле по его ID.")]
        [SwaggerResponse(200, "Автомобиль найден", typeof(Car))]
        [SwaggerResponse(404, "Автомобиль не найден")]
        public async Task<ActionResult<Car>> GetCar(int id)
        {
            var car = await _context.Cars
                .Include(c => c.Owner)
                .Include(c => c.RepairOrders)
                    .ThenInclude(ro => ro.Status)
                .FirstOrDefaultAsync(c => c.CarId == id);

            if (car == null)
            {
                return NotFound();
            }

            return Ok(car);
        }

        /// <summary>
        /// Обновить информацию о автомобиле.
        /// </summary>
        /// <param name="id">ID автомобиля для обновления.</param>
        /// <param name="car">Обновленные данные автомобиля.</param>
        /// <returns>Статус успешного выполнения.</returns>
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Обновить автомобиль", Description = "Обновляет данные об автомобиле по его ID.")]
        [SwaggerResponse(204, "Автомобиль успешно обновлен")]
        [SwaggerResponse(400, "Некорректные данные запроса")]
        [SwaggerResponse(404, "Автомобиль не найден")]
        public async Task<IActionResult> PutCar(int id, Car car)
        {
            if (id != car.CarId)
            {
                return BadRequest();
            }

            _context.Entry(car).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Создать новый автомобиль.
        /// </summary>
        /// <param name="car">Данные для нового автомобиля.</param>
        /// <returns>Созданный автомобиль с присвоенным ID.</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Создать новый автомобиль", Description = "Создает новый автомобиль и сохраняет его в базе данных.")]
        [SwaggerResponse(201, "Автомобиль успешно создан", typeof(Car))]
        public async Task<ActionResult<Car>> PostCar(Car car)
        {
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCar), new { id = car.CarId }, car);
        }

        /// <summary>
        /// Удалить автомобиль по ID.
        /// </summary>
        /// <param name="id">ID автомобиля для удаления.</param>
        /// <returns>Статус успешного удаления.</returns>
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Удалить автомобиль", Description = "Удаляет автомобиль по его ID.")]
        [SwaggerResponse(204, "Автомобиль успешно удален")]
        [SwaggerResponse(404, "Автомобиль не найден")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.CarId == id);
        }
    }
}
