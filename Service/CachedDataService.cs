using Microsoft.EntityFrameworkCore;
using App.Data;
using System.Collections.Generic;
using System;
using App.Models;
using Microsoft.Extensions.Caching.Memory;

namespace App.Service
{
    public class CachedService
    {
        private readonly AutoRepairWorkshopContext _context;
        private readonly IMemoryCache _cache;
        private const int RowCount = 20;

        public CachedService(AutoRepairWorkshopContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _cache = memoryCache;
        }

        // Метод для получения автомобилей с включением связанных данных (Owner и RepairOrders)
        public IEnumerable<Car> GetCars()
        {
            if (!_cache.TryGetValue("Cars", out IEnumerable<Car> cars))
            {
                cars = _context.Cars
                    .Include(c => c.Owner)
                    .Include(c => c.RepairOrders)
                    .Take(RowCount)
                    .ToList();
                _cache.Set("Cars", cars, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(258)
                });
            }
            return cars;
        }

        // Метод для получения ремонтных заказов с включением связанных данных (Car, Mechanic, Status, Payments)
        public IEnumerable<RepairOrder> GetRepairOrders()
        {
            if (!_cache.TryGetValue("RepairOrders", out IEnumerable<RepairOrder> repairOrders))
            {
                repairOrders = _context.RepairOrders
                    .Include(ro => ro.Car)
                    .Include(ro => ro.Mechanic)
                    .Include(ro => ro.Status)
                    .Include(ro => ro.Payments)
                    .Take(RowCount)
                    .ToList();
                _cache.Set("RepairOrders", repairOrders, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(258)
                });
            }
            return repairOrders;
        }

        // Метод для получения механиков с включением связанных данных (CarServices и RepairOrders)
        public IEnumerable<Mechanic> GetMechanics()
        {
            if (!_cache.TryGetValue("Mechanics", out IEnumerable<Mechanic> mechanics))
            {
                mechanics = _context.Mechanics
                    .Include(m => m.CarServices)
                    .Include(m => m.RepairOrders)
                    .Take(RowCount)
                    .ToList();
                _cache.Set("Mechanics", mechanics, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(258)
                });
            }
            return mechanics;
        }

        // Метод для получения услуг с включением связанных данных (CarServices)
        public IEnumerable<CachedDataService> GetServices()
        {
            if (!_cache.TryGetValue("Services", out IEnumerable<CachedDataService> services))
            {
                services = (IEnumerable<CachedDataService>?)_context.Services
                    .Include(s => s.CarServices)
                    .Take(RowCount)
                    .ToList();
                _cache.Set("Services", services, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(258)
                });
            }
            return services;
        }

        // Метод для получения платежей с включением связанных данных (Order)
        public IEnumerable<Payment> GetPayments()
        {
            if (!_cache.TryGetValue("Payments", out IEnumerable<Payment> payments))
            {
                payments = _context.Payments
                    .Include(p => p.Order)
                    .Take(RowCount)
                    .ToList();
                _cache.Set("Payments", payments, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(258)
                });
            }
            return payments;
        }

        // Метод для получения CarService с включением связанных данных (Order, Mechanic, Service)
        public IEnumerable<CarService> GetCarServices()
        {
            if (!_cache.TryGetValue("CarServices", out IEnumerable<CarService> carServices))
            {
                carServices = _context.CarServices
                    .Include(cs => cs.Order)
                    .Include(cs => cs.Mechanic)
                    .Include(cs => cs.Service)
                    .Take(RowCount)
                    .ToList();
                _cache.Set("CarServices", carServices, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(258)
                });
            }
            return carServices;
        }

        // Метод для получения статусов автомобилей с включением связанных данных (RepairOrders)
        public IEnumerable<CarStatus> GetCarStatus()
        {
            if (!_cache.TryGetValue("CarStatus", out IEnumerable<CarStatus> CarStatus))
            {
                CarStatus = _context.CarStatus
                    .Include(cs => cs.RepairOrders)
                    .Take(RowCount)
                    .ToList();
                _cache.Set("CarStatus", CarStatus, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(258)
                });
            }
            return CarStatus;
        }

        // Метод для получения владельцев с включением связанных данных (Cars)
        public IEnumerable<Owner> GetOwners()
        {
            if (!_cache.TryGetValue("Owners", out IEnumerable<Owner> owners))
            {
                owners = _context.Owners
                    .Include(o => o.Cars)
                    .Take(RowCount)
                    .ToList();
                _cache.Set("Owners", owners, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(258)
                });
            }
            return owners;
        }
    }
}
