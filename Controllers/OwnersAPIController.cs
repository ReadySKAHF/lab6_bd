using Microsoft.AspNetCore.Mvc;
using App.Models;
using Microsoft.EntityFrameworkCore;
using App.Data;
using Swashbuckle.AspNetCore.Annotations; // Для аннотаций Swagger

namespace App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnersAPIController : ControllerBase
    {
        private readonly AutoRepairWorkshopContext _context;

        public OwnersAPIController(AutoRepairWorkshopContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получить список всех владельцев.
        /// </summary>
        /// <returns>Список всех владельцев.</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Получить список владельцев", Description = "Возвращает список всех владельцев автомобилей в мастерской.")]
        [SwaggerResponse(200, "Список владельцев успешно получен", typeof(IEnumerable<Owner>))]
        public async Task<ActionResult<IEnumerable<Owner>>> GetOwners()
        {
            var owners = await _context.Owners.ToListAsync();
            return Ok(owners);
        }

        /// <summary>
        /// Получить владельца по ID.
        /// </summary>
        /// <param name="id">ID владельца.</param>
        /// <returns>Детальная информация о владельце.</returns>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Получить владельца по ID", Description = "Возвращает информацию о владельце по его уникальному ID. Включает список автомобилей этого владельца.")]
        [SwaggerResponse(200, "Владелец найден", typeof(Owner))]
        [SwaggerResponse(404, "Владелец не найден")]
        public async Task<ActionResult<Owner>> GetOwner(int id)
        {
            var owner = await _context.Owners
                .Include(o => o.Cars) // Включаем автомобили владельца
                .FirstOrDefaultAsync(o => o.OwnerId == id);

            if (owner == null)
            {
                return NotFound();
            }

            return Ok(owner);
        }

        /// <summary>
        /// Обновить информацию о владельце.
        /// </summary>
        /// <param name="id">ID владельца для обновления.</param>
        /// <param name="owner">Новые данные владельца.</param>
        /// <returns>Статус успешного обновления.</returns>
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Обновить информацию о владельце", Description = "Обновляет данные владельца по его уникальному ID.")]
        [SwaggerResponse(204, "Владелец успешно обновлен")]
        [SwaggerResponse(400, "Некорректный запрос")]
        [SwaggerResponse(404, "Владелец не найден")]
        public async Task<IActionResult> PutOwner(int id, Owner owner)
        {
            if (id != owner.OwnerId)
            {
                return BadRequest();
            }

            _context.Entry(owner).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OwnerExists(id))
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
        /// Добавить нового владельца.
        /// </summary>
        /// <param name="owner">Данные нового владельца.</param>
        /// <returns>Созданный владелец с присвоенным ID.</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Добавить нового владельца", Description = "Создает нового владельца в системе.")]
        [SwaggerResponse(201, "Владелец успешно создан", typeof(Owner))]
        public async Task<ActionResult<Owner>> PostOwner(Owner owner)
        {
            _context.Owners.Add(owner);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOwner), new { id = owner.OwnerId }, owner);
        }

        /// <summary>
        /// Удалить владельца по ID.
        /// </summary>
        /// <param name="id">ID владельца для удаления.</param>
        /// <returns>Статус успешного удаления.</returns>
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Удалить владельца", Description = "Удаляет владельца по его уникальному ID.")]
        [SwaggerResponse(204, "Владелец успешно удален")]
        [SwaggerResponse(404, "Владелец не найден")]
        public async Task<IActionResult> DeleteOwner(int id)
        {
            var owner = await _context.Owners.FindAsync(id);
            if (owner == null)
            {
                return NotFound();
            }

            _context.Owners.Remove(owner);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OwnerExists(int id)
        {
            return _context.Owners.Any(e => e.OwnerId == id);
        }
    }
}
