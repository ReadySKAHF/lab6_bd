using Moq;
using Xunit;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using App.Controllers;
using App.Models;
using App.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace TestProject
{
    public class RepairOrdersAPIControllerTests
    {
        private readonly Mock<AutoRepairWorkshopContext> _mockContext;
        private readonly RepairOrdersAPIController _controller;

        public RepairOrdersAPIControllerTests()
        {
            // Мокируем DbContext
            var options = new DbContextOptionsBuilder<AutoRepairWorkshopContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;
            _mockContext = new Mock<AutoRepairWorkshopContext>(options);

            // Инициализируем контроллер
            _controller = new RepairOrdersAPIController(_mockContext.Object);
        }

        #region 1. Получение списка всех заказов на ремонт
        [Fact]
        public async Task GetRepairOrders_ReturnsOkResult_WithListOfRepairOrders()
        {
            // Arrange
            var repairOrders = new List<RepairOrder>
            {
                new RepairOrder { OrderId = 1, OrderDate = new DateOnly(2024, 12, 16) },
                new RepairOrder { OrderId = 2, OrderDate = new DateOnly(2024, 12, 17) }
            };

            _mockContext.Setup(c => c.RepairOrders.ToListAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(repairOrders);

            // Act
            var result = await _controller.GetRepairOrders();

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Xunit.Assert.IsAssignableFrom<IEnumerable<RepairOrder>>(okResult.Value);
            Xunit.Assert.Equal(2, returnValue.Count());
        }
        #endregion

        #region 2. Получение заказа на ремонт по ID
        [Fact]
        public async Task GetRepairOrder_ReturnsOkResult_WithRepairOrder()
        {
            // Arrange
            int orderId = 1;
            var repairOrder = new RepairOrder { OrderId = orderId, OrderDate = new DateOnly(2024, 12, 16) };

            _mockContext.Setup(c => c.RepairOrders
                .FirstOrDefaultAsync(It.IsAny<Func<RepairOrder, bool>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(repairOrder);

            // Act
            var result = await _controller.GetRepairOrder(orderId);

            // Assert
            var okResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Xunit.Assert.IsType<RepairOrder>(okResult.Value);
            Xunit.Assert.Equal(orderId, returnValue.OrderId);
        }
        #endregion

        #region 3. Заказ не найден по ID
        [Fact]
        public async Task GetRepairOrder_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            int invalidOrderId = 999;
            _mockContext.Setup(c => c.RepairOrders
                .FirstOrDefaultAsync(It.IsAny<Func<RepairOrder, bool>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((RepairOrder)null);

            // Act
            var result = await _controller.GetRepairOrder(invalidOrderId);

            // Assert
            Xunit.Assert.IsType<NotFoundResult>(result.Result);
        }
        #endregion

        #region 4. Обновление заказа на ремонт
        [Fact]
        public async Task PutRepairOrder_ReturnsNoContent_WhenOrderIsUpdated()
        {
            // Arrange
            int orderId = 1;
            var repairOrder = new RepairOrder { OrderId = orderId, OrderDate = new DateOnly(2024, 12, 16) };

            _mockContext.Setup(c => c.RepairOrders
                .FirstOrDefaultAsync(It.IsAny<Func<RepairOrder, bool>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(repairOrder);

            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _controller.PutRepairOrder(orderId, repairOrder);

            // Assert
            Xunit.Assert.IsType<NoContentResult>(result);
        }
        #endregion

        #region 5. Обновление заказа с ошибкой
        [Fact]
        public async Task PutRepairOrder_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            int orderId = 1;
            var repairOrder = new RepairOrder { OrderId = orderId, OrderDate = new DateOnly(2024, 12, 16) };

            _mockContext.Setup(c => c.RepairOrders
                .FirstOrDefaultAsync(It.IsAny<Func<RepairOrder, bool>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((RepairOrder)null);

            // Act
            var result = await _controller.PutRepairOrder(orderId, repairOrder);

            // Assert
            Xunit.Assert.IsType<NotFoundResult>(result);
        }
        #endregion

        #region 6. Создание нового заказа на ремонт
        [Fact]
        public async Task PostRepairOrder_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var repairOrder = new RepairOrder { OrderId = 1, OrderDate = new DateOnly(2024, 12, 16) };

            _mockContext.Setup(c => c.AddAsync(It.IsAny<RepairOrder>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _controller.PostRepairOrder(repairOrder);

            // Assert
            var createdResult = Xunit.Assert.IsType<CreatedAtActionResult>(result.Result);
            Xunit.Assert.Equal("GetRepairOrder", createdResult.ActionName);
            Xunit.Assert.Equal(repairOrder.OrderId, createdResult.RouteValues["id"]);
        }
        #endregion

        #region 7. Удаление заказа на ремонт
        [Fact]
        public async Task DeleteRepairOrder_ReturnsNoContent_WhenOrderIsDeleted()
        {
            // Arrange
            int orderId = 1;
            var repairOrder = new RepairOrder { OrderId = orderId, OrderDate = new DateOnly(2024, 12, 16) };

            _mockContext.Setup(c => c.RepairOrders.FindAsync(It.IsAny<int>())).ReturnsAsync(repairOrder);
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _controller.DeleteRepairOrder(orderId);

            // Assert
            Xunit.Assert.IsType<NoContentResult>(result);
        }
        #endregion

        #region 8. Удаление несуществующего заказа
        [Fact]
        public async Task DeleteRepairOrder_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            int invalidOrderId = 999;
            _mockContext.Setup(c => c.RepairOrders.FindAsync(It.IsAny<int>())).ReturnsAsync((RepairOrder)null);

            // Act
            var result = await _controller.DeleteRepairOrder(invalidOrderId);

            // Assert
            Xunit.Assert.IsType<NotFoundResult>(result);
        }
        #endregion

        #region 9. Обновление заказа с некорректными данными
        [Fact]
        public async Task PutRepairOrder_ReturnsBadRequest_WhenIdsDoNotMatch()
        {
            // Arrange
            int orderId = 1;
            var repairOrder = new RepairOrder { OrderId = 2, OrderDate = new DateOnly(2024, 12, 16) };

            // Act
            var result = await _controller.PutRepairOrder(orderId, repairOrder);

            // Assert
            Xunit.Assert.IsType<BadRequestResult>(result);
        }
        #endregion

        #region 10. Добавление услуги в заказ на ремонт
        [Fact]
        public async Task AddServiceToRepairOrder_ReturnsOkResult()
        {
            // Arrange
            var repairOrder = new RepairOrder { OrderId = 1, OrderDate = new DateOnly(2024, 12, 16) };
            var service = new CachedDataService { ServiceId = 1, ServiceName = "Oil Change", Price = 50 };

            _mockContext.Setup(c => c.RepairOrders
                .FirstOrDefaultAsync(It.IsAny<Func<RepairOrder, bool>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(repairOrder);

            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _controller.AddServiceToRepairOrder(repairOrder.OrderId, service.ServiceId);

            // Assert
            Xunit.Assert.IsType<OkResult>(result);
        }
        #endregion
    }
}
