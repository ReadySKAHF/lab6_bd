using App.Models;
using App;
using App.Service;

namespace App.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AutoRepairWorkshopContext db)
        {
            db.Database.EnsureCreated();

            if (db.Cars.Any())
            {
                return; // Exit if data already exists
            }

            Random randObj = new(1);

            // Seed Owners
            string[] ownerNames = { "John Doe", "Jane Smith", "Robert Johnson", "Michael Brown", "William Davis" };

            foreach (var ownerName in ownerNames)
            {
                string licenseNumber;


                do
                {
                    licenseNumber = "DLN_" + randObj.Next(1000, 9999);
                } while (db.Owners.Any(o => o.DriverLicenseNumber == licenseNumber)); 

                db.Owners.Add(new Owner
                {
                    FullName = ownerName,
                    DriverLicenseNumber = licenseNumber,
                    Address = "Address_" + ownerName,
                    Phone = "+12345678" + randObj.Next(10, 99)
                });
            }
            db.SaveChanges();



            // Seed Car Statuses
            string[] statusNames = { "Available", "In Service", "Under Repair", "Out of Service" };

            foreach (var statusName in statusNames)
            {
                if (!db.CarStatus.Any(cs => cs.StatusName == statusName))
                {
                    db.CarStatus.Add(new CarStatus
                    {
                        StatusName = statusName
                    });
                }
            }
            db.SaveChanges();


            // Seed Mechanics
            string[] mechanicNames = { "Alex Black", "Chris Green", "Sam White", "Taylor Brown", "Morgan Gray" };
            for (int i = 0; i < mechanicNames.Length; i++)
            {
                db.Mechanics.Add(new Mechanic
                {
                    FullName = mechanicNames[i],
                    Qualification = "Level " + (i + 1),
                    ExperienceYears = randObj.Next(1, 10),
                    Salary = randObj.Next(3000, 7000)
                });
            }
            db.SaveChanges();

            // Seed Services
            string[] serviceNames = { "Pididy kids Oil", "Tire Replacement", "Brake Check", "Engine Diagnostics", "Battery Replacement" };
            foreach (var serviceName in serviceNames)
            {
                db.Services.Add(new CachedDataService
                {
                    ServiceName = serviceName,
                    Price = randObj.Next(100, 500)
                });
            }
            db.SaveChanges();

            // Seed Cars
            var owners = db.Owners.ToList();
            var carStatus = db.CarStatus.ToList(); 
            string[] carBrands = { "Toyota", "Ford", "BMW", "Mercedes", "Audi" };
            HashSet<string> usedLicensePlates = new HashSet<string>(); 

            for (int i = 0; i < 50; i++)
            {
                string licensePlate;

                // Генерируем уникальный LicensePlate
                do
                {
                    licensePlate = "LP_" + randObj.Next(1000, 9999);
                } while (!usedLicensePlates.Add(licensePlate)); 

                db.Cars.Add(new Car
                {
                    LicensePlate = licensePlate,
                    Brand = carBrands[randObj.Next(carBrands.Length)],
                    Power = randObj.Next(100, 400),
                    Color = "Color_" + i,
                    YearOfProduction = 2000 + randObj.Next(23),
                    ChassisNumber = "Chassis_" + Guid.NewGuid().ToString().Substring(0, 8),
                    EngineNumber = "Engine_" + Guid.NewGuid().ToString().Substring(0, 8),
                    DateReceived = DateOnly.FromDateTime(DateTime.Now.AddDays(-randObj.Next(30))),
                    OwnerId = owners[randObj.Next(owners.Count)].OwnerId,
                    StatusId = carStatus[randObj.Next(carStatus.Count)].StatusId
                });
            }
            db.SaveChanges();


            // Seed Repair Orders
            var cars = db.Cars.ToList();
            var mechanics = db.Mechanics.ToList();
            var carStatusList = db.CarStatus.ToList(); 

            for (int i = 0; i < 30; i++)
            {
                db.RepairOrders.Add(new RepairOrder
                {
                    CarId = cars[randObj.Next(cars.Count)].CarId,
                    MechanicId = mechanics[randObj.Next(mechanics.Count)].MechanicId,
                    OrderDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-randObj.Next(30, 60))),
                    StatusId = carStatusList[randObj.Next(carStatusList.Count)].StatusId
                });
            }
            db.SaveChanges();

            // Seed Payments
            var repairOrders = db.RepairOrders.ToList();
            for (int i = 0; i < 20; i++)
            {
                db.Payments.Add(new Payment
                {
                    OrderId = repairOrders[randObj.Next(repairOrders.Count)].OrderId,
                    PaymentDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-randObj.Next(1, 20))),
                    Amount = randObj.Next(100, 1000),
                    Employee = "Employee_" + i
                });
            }
            db.SaveChanges();

            // Seed Car Services
            var services = db.Services.ToList();
            for (int i = 0; i < 50; i++)
            {
                db.CarServices.Add(new CarService
                {
                    OrderId = repairOrders[randObj.Next(repairOrders.Count)].OrderId,
                    ServiceId = services[randObj.Next(services.Count)].ServiceId,
                    MechanicId = mechanics[randObj.Next(mechanics.Count)].MechanicId,
                    WorkDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-randObj.Next(1, 30)))
                });
            }
            db.SaveChanges();
        }
    }
}
