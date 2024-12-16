using Microsoft.AspNetCore.Mvc;
using App.Models;
using Microsoft.EntityFrameworkCore;
using App.Data;
using Swashbuckle.AspNetCore.Annotations; // Для аннотаций Swagger

namespace App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsAPIController : ControllerBase
    {
        private readonly AutoRepairWorkshopContext _context;

        public PaymentsAPIController(AutoRepairWorkshopContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получить список всех платежей.
        /// </summary>
        /// <returns>Список всех платежей, включая информацию о заказах.</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Получить список всех платежей", Description = "Возвращает список всех платежей, включая связанные с ними заказы.")]
        [SwaggerResponse(200, "Список платежей успешно получен", typeof(IEnumerable<Payment>))]
        public async Task<ActionResult<IEnumerable<Payment>>> GetPayments()
        {
            var payments = await _context.Payments
                .Include(p => p.Order) // Включаем заказ, с которым связан платеж
                .ToListAsync();

            return Ok(payments);
        }

        /// <summary>
        /// Получить платеж по ID.
        /// </summary>
        /// <param name="id">ID платежа.</param>
        /// <returns>Детальная информация о платеже с привязанным заказом.</returns>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Получить платеж по ID", Description = "Возвращает информацию о платеже по его уникальному ID. Также включает информацию о связанном заказе.")]
        [SwaggerResponse(200, "Платеж найден", typeof(Payment))]
        [SwaggerResponse(404, "Платеж не найден")]
        public async Task<ActionResult<Payment>> GetPayment(int id)
        {
            var payment = await _context.Payments
                .Include(p => p.Order) // Включаем заказ
                .FirstOrDefaultAsync(p => p.PaymentId == id);

            if (payment == null)
            {
                return NotFound();
            }

            return Ok(payment);
        }

        /// <summary>
        /// Создать новый платеж.
        /// </summary>
        /// <param name="payment">Данные нового платежа.</param>
        /// <returns>Созданный платеж с присвоенным ID.</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Создать новый платеж", Description = "Создает новый платеж в системе и привязывает его к заказу.")]
        [SwaggerResponse(201, "Платеж успешно создан", typeof(Payment))]
        public async Task<ActionResult<Payment>> PostPayment(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPayment), new { id = payment.PaymentId }, payment);
        }

        /// <summary>
        /// Удалить платеж по ID.
        /// </summary>
        /// <param name="id">ID платежа для удаления.</param>
        /// <returns>Статус успешного удаления.</returns>
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Удалить платеж", Description = "Удаляет платеж по его уникальному ID.")]
        [SwaggerResponse(204, "Платеж успешно удален")]
        [SwaggerResponse(404, "Платеж не найден")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
