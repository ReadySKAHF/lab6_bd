using Microsoft.AspNetCore.Mvc;
using App.Models;
using Microsoft.EntityFrameworkCore;
using App.Data;
using Swashbuckle.AspNetCore.Annotations; // Для аннотаций Swagger

namespace App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MechanicsAPIController : ControllerBase
    {
        private readonly AutoRepairWorkshopContext _context;

        public MechanicsAPIController(AutoRepairWorkshopContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получить список всех механиков.
        /// </summary>
        /// <returns>Список всех механиков.</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Получить список механиков", Description = "Возвращает всех механиков, работающих в мастерской.")]
        [SwaggerResponse(200, "Список механиков успешно получен", typeof(IEnumerable<Mechanic>))]
        public async Task<ActionResult<IEnumerable<Mechanic>>> GetMechanics()
        {
            var mechanics = await _context.Mechanics.ToListAsync();
            return Ok(mechanics);
        }

        /// <summary>
        /// Получить механика по ID.
        /// </summary>
        /// <param name="id">ID механика.</param>
        /// <returns>Детальная информация о механике.</returns>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Получить механика по ID", Description = "Возвращает информацию о механике по его уникальному ID.")]
        [SwaggerResponse(200, "Механик найден", typeof(Mechanic))]
        [SwaggerResponse(404, "Механик не найден")]
        public async Task<ActionResult<Mechanic>> GetMechanic(int id)
        {
            var mechanic = await _context.Mechanics.FindAsync(id);

            if (mechanic == null)
            {
                return NotFound();
            }

            return Ok(mechanic);
        }

        /// <summary>
        /// Обновить информацию о механике.
        /// </summary>
        /// <param name="id">ID механика для обновления.</param>
        /// <param name="mechanic">Новые данные механика.</param>
        /// <returns>Статус успешного обновления.</returns>
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Обновить информацию о механике", Description = "Обновляет данные механика по его уникальному ID.")]
        [SwaggerResponse(204, "Механик успешно обновлен")]
        [SwaggerResponse(400, "Некорректный запрос")]
        [SwaggerResponse(404, "Механик не найден")]
        public async Task<IActionResult> PutMechanic(int id, Mechanic mechanic)
        {
            if (id != mechanic.MechanicId)
            {
                return BadRequest();
            }

            _context.Entry(mechanic).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MechanicExists(id))
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
        /// Добавить нового механика.
        /// </summary>
        /// <param name="mechanic">Данные нового механика.</param>
        /// <returns>Созданный механик с присвоенным ID.</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Добавить нового механика", Description = "Создает нового механика в системе.")]
        [SwaggerResponse(201, "Механик успешно создан", typeof(Mechanic))]
        public async Task<ActionResult<Mechanic>> PostMechanic(Mechanic mechanic)
        {
            _context.Mechanics.Add(mechanic);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMechanic), new { id = mechanic.MechanicId }, mechanic);
        }

        /// <summary>
        /// Удалить механика по ID.
        /// </summary>
        /// <param name="id">ID механика для удаления.</param>
        /// <returns>Статус успешного удаления.</returns>
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Удалить механика", Description = "Удаляет механика по его уникальному ID.")]
        [SwaggerResponse(204, "Механик успешно удален")]
        [SwaggerResponse(404, "Механик не найден")]
        public async Task<IActionResult> DeleteMechanic(int id)
        {
            var mechanic = await _context.Mechanics.FindAsync(id);
            if (mechanic == null)
            {
                return NotFound();
            }

            _context.Mechanics.Remove(mechanic);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MechanicExists(int id)
        {
            return _context.Mechanics.Any(e => e.MechanicId == id);
        }
    }
}
