using Microsoft.AspNetCore.Mvc;
using App.Models;
using Microsoft.EntityFrameworkCore;
using App.Data;
using Swashbuckle.AspNetCore.Annotations; // Для аннотаций Swagger

namespace App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepairOrdersAPIController : ControllerBase
    {
        private readonly AutoRepairWorkshopContext _context;

        public RepairOrdersAPIController(AutoRepairWorkshopContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получить список всех заказов на ремонт.
        /// </summary>
        /// <returns>Список всех заказов на ремонт с информацией о машине, механике и услугах.</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Получить список всех заказов на ремонт", Description = "Возвращает список всех заказов на ремонт с деталями о машине, механике, статусе и услугах.")]
        [SwaggerResponse(200, "Список заказов успешно получен", typeof(IEnumerable<RepairOrder>))]
        public async Task<ActionResult<IEnumerable<RepairOrder>>> GetRepairOrders()
        {
            var repairOrders = await _context.RepairOrders
                .Include(ro => ro.Car) // Включаем информацию о машине
                .Include(ro => ro.Mechanic) // Включаем информацию о механике
                .Include(ro => ro.Status) // Включаем статус заказа
                .Include(ro => ro.CarServices) // Включаем услуги
                    .ThenInclude(cs => cs.Service) // Включаем информацию об услугах
                .ToListAsync();

            return Ok(repairOrders);
        }

        /// <summary>
        /// Получить заказ на ремонт по ID.
        /// </summary>
        /// <param name="id">ID заказа на ремонт.</param>
        /// <returns>Детали заказа на ремонт, включая машину, механика, статус и услуги.</returns>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Получить заказ на ремонт по ID", Description = "Возвращает информацию о заказе на ремонт по его уникальному ID, включая машину, механика, статус и услуги.")]
        [SwaggerResponse(200, "Заказ на ремонт найден", typeof(RepairOrder))]
        [SwaggerResponse(404, "Заказ на ремонт не найден")]
        public async Task<ActionResult<RepairOrder>> GetRepairOrder(int id)
        {
            var repairOrder = await _context.RepairOrders
                .Include(ro => ro.Car) // Включаем информацию о машине
                .Include(ro => ro.Mechanic) // Включаем информацию о механике
                .Include(ro => ro.Status) // Включаем статус
                .Include(ro => ro.CarServices) // Включаем услуги
                    .ThenInclude(cs => cs.Service) // Включаем услугу
                .FirstOrDefaultAsync(ro => ro.OrderId == id);

            if (repairOrder == null)
            {
                return NotFound();
            }

            return Ok(repairOrder);
        }

        /// <summary>
        /// Обновить заказ на ремонт.
        /// </summary>
        /// <param name="id">ID заказа на ремонт.</param>
        /// <param name="repairOrder">Новые данные для обновления заказа.</param>
        /// <returns>Статус успешного обновления.</returns>
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Обновить заказ на ремонт", Description = "Обновляет информацию о заказе на ремонт по его уникальному ID.")]
        [SwaggerResponse(204, "Заказ на ремонт успешно обновлен")]
        [SwaggerResponse(400, "Некорректный запрос")]
        [SwaggerResponse(404, "Заказ на ремонт не найден")]
        public async Task<IActionResult> PutRepairOrder(int id, RepairOrder repairOrder)
        {
            if (id != repairOrder.OrderId)
            {
                return BadRequest();
            }

            _context.Entry(repairOrder).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RepairOrderExists(id))
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
        /// Создать новый заказ на ремонт.
        /// </summary>
        /// <param name="repairOrder">Данные для нового заказа на ремонт.</param>
        /// <returns>Созданный заказ с присвоенным ID.</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Создать новый заказ на ремонт", Description = "Создает новый заказ на ремонт в системе.")]
        [SwaggerResponse(201, "Заказ на ремонт успешно создан", typeof(RepairOrder))]
        public async Task<ActionResult<RepairOrder>> PostRepairOrder(RepairOrder repairOrder)
        {
            _context.RepairOrders.Add(repairOrder);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRepairOrder), new { id = repairOrder.OrderId }, repairOrder);
        }

        /// <summary>
        /// Удалить заказ на ремонт по ID.
        /// </summary>
        /// <param name="id">ID заказа на ремонт.</param>
        /// <returns>Статус успешного удаления.</returns>
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Удалить заказ на ремонт", Description = "Удаляет заказ на ремонт по его уникальному ID.")]
        [SwaggerResponse(204, "Заказ на ремонт успешно удален")]
        [SwaggerResponse(404, "Заказ на ремонт не найден")]
        public async Task<IActionResult> DeleteRepairOrder(int id)
        {
            var repairOrder = await _context.RepairOrders.FindAsync(id);
            if (repairOrder == null)
            {
                return NotFound();
            }

            _context.RepairOrders.Remove(repairOrder);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RepairOrderExists(int id)
        {
            return _context.RepairOrders.Any(e => e.OrderId == id);
        }
    }
}
