using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EnginesTest.Base;
using EnginesTest.Models;
using System.Data.Entity.Infrastructure;

namespace EnginesTest.Controllers
{
    public class InitController : BaseController
    {
        public ActionResult Index()
        {
            // Clear Database
            DataBase.Database.ExecuteSqlCommand("DELETE FROM [dbo].[JobTitles]");
            DataBase.Database.ExecuteSqlCommand("DELETE FROM [dbo].[Engines]");
            DataBase.SaveChanges();

            // Add Job Titles
            JobTitle AdminJT = new JobTitle { Title = "Administrator" };            
            JobTitle EngineerJT = new JobTitle { Title = "Engineer" };
            JobTitle ManagerJT = new JobTitle { Title = "Manager" };
            JobTitle ViceManagerJT = new JobTitle { Title = "Vice Manager", Deleted = true };
            DataBase.JobTitles.Add(AdminJT);
            DataBase.JobTitles.Add(EngineerJT);
            DataBase.JobTitles.Add(ManagerJT);
            DataBase.JobTitles.Add(ViceManagerJT);
            DataBase.SaveChanges();

            // Add users
            // Admin
            DataBase.Users.Add(new User { FirstName = "Admin", LastName = "Admin", JobTitleId = AdminJT.Id, SecurityTM = true, SecurityTV = true, SecurityUM = true, Login = "root", Email = "admin@enginestest.com", Password = SecurityManager.HashPassword("123456789"), ProfileImage = "bc7499ae44c2446a8066fdf5fc639a66.jpg", Phone = "+7(978)527-45-93" });
            // Engineers
            User u1 = new User { FirstName = "Artemy", LastName = "Voloshin", JobTitleId = EngineerJT.Id, SecurityTM = true, SecurityTV = true, SecurityUM = false, Login = "a.voloshin", Email = "a.voloshin@enginestest.com", Password = SecurityManager.HashPassword("12345"), ProfileImage = "2d411823860740789b051c4cb06a1e7a.jpg", Phone = "+7(921)465-78-93" };
            User u2 = new User { FirstName = "Viktor", LastName = "Vasil'ev", JobTitleId = EngineerJT.Id, SecurityTM = false, SecurityTV = false, SecurityUM = false, Login = "v.vasilev", Email = "v.vasilev@enginestest.com", Password = SecurityManager.HashPassword("12345"), ProfileImage = "4ba6843506b3472fb9c634cc0d4187a8.jpg", Phone = "+7(941)256-87-41", Deleted = true };
            User u3 = new User { FirstName = "Marina", LastName = "Lipkina", JobTitleId = EngineerJT.Id, SecurityTM = true, SecurityTV = true, SecurityUM = false, Login = "m.lipkina", Email = "m.lipkina@enginestest.com", Password = SecurityManager.HashPassword("12345"), ProfileImage = "76fff4a5987c4976959bb4341a57b216.jpg", Phone = "+7(934)125-68-49" };
            User u4 = new User { FirstName = "Ksenia", LastName = "Il'ina", JobTitleId = EngineerJT.Id, SecurityTM = true, SecurityTV = true, SecurityUM = false, Login = "k.ilina", Email = "k.ilina@enginestest.com", Password = SecurityManager.HashPassword("12345"), ProfileImage = "6e9973f891cc4ad2b25f2d38b4279922.jpg", Phone = "+7(954)123-56-78" };
            DataBase.Users.Add(u1);
            DataBase.Users.Add(u2);
            DataBase.Users.Add(u3);
            DataBase.Users.Add(u4);            
            // Managers
            DataBase.Users.Add(new User { FirstName = "Lilia", LastName = "Ponomareva", JobTitleId = ManagerJT.Id, SecurityTM = false, SecurityTV = true, SecurityUM = false, Login = "l.ponomareva", Email = "l.ponomareva@enginestest.com", Password = SecurityManager.HashPassword("12345"), ProfileImage = "e40c3e3c3c344ceea05f4cb30617aa03.jpg", Phone = "+(7921)654-91-56" });
            DataBase.Users.Add(new User { FirstName = "Martin", LastName = "Peres", JobTitleId = ManagerJT.Id, SecurityTM = false, SecurityTV = true, SecurityUM = false, Login = "m.peres", Email = "m.peres@enginestest.com", Password = SecurityManager.HashPassword("12345"), ProfileImage = "8b44231db2a848d4bc5ad18b3776a679.jpg", Phone = "+7(932)849-56-21" });
            DataBase.Users.Add(new User { FirstName = "Vlad", LastName = "Chastin", JobTitleId = ManagerJT.Id, SecurityTM = false, SecurityTV = false, SecurityUM = false, Login = "v.chastin", Email = "v.chastin@enginestest.com", Password = SecurityManager.HashPassword("12345"), ProfileImage = "e7dac6daf7554dcc86f75be4a43e9c59.jpg", Phone = "+7(974)659-65-13", Deleted = true });
            DataBase.Users.Add(new User { FirstName = "Nikolay", LastName = "Dudko", JobTitleId = ManagerJT.Id, SecurityTM = false, SecurityTV = true, SecurityUM = false, Login = "n.dudko", Email = "n.dudko@enginestest.com", Password = SecurityManager.HashPassword("12345"), Phone = "+7(954)849-56-12" });
            // ViceManagers
            DataBase.Users.Add(new User { FirstName = "Sergey", LastName = "Panin", JobTitleId = ViceManagerJT.Id, SecurityTM = false, SecurityTV = false, SecurityUM = false, Login = "s.panin", Email = "s.panin@enginestest.com", Password = SecurityManager.HashPassword("12345"), Phone = "+7(988)745-21-45", Deleted = true });
            DataBase.SaveChanges();

            // Add Engines
            Engine e1 = new Engine { UID = Guid.NewGuid().ToString("n").ToUpper(), Model = "W4G8008212", SerialNumber = "CVN21354WN", Configuration = "W", ValversPerCylinder = 4, FuelType = "G", RatedTorque = 800, EngineCapacity = 8.2, Cylinders = 12 };
            Engine e2 = new Engine { UID = Guid.NewGuid().ToString("n").ToUpper(), Model = "I4G280306", SerialNumber = "KJSDFF3472", Configuration = "I", ValversPerCylinder = 4, FuelType = "G", RatedTorque = 280, EngineCapacity = 3.0, Cylinders = 6 };
            Engine e3 = new Engine { UID = Guid.NewGuid().ToString("n").ToUpper(), Model = "V4D600608", SerialNumber = "ASDWCC4545", Configuration = "V", ValversPerCylinder = 4, FuelType = "D", RatedTorque = 600, EngineCapacity = 6.0, Cylinders = 8 };
            Engine e4 = new Engine { UID = Guid.NewGuid().ToString("n").ToUpper(), Model = "I2D190306", SerialNumber = "HJ89IOG80P", Configuration = "I", ValversPerCylinder = 2, FuelType = "D", RatedTorque = 190, EngineCapacity = 3.0, Cylinders = 6 };
            DataBase.Engines.Add(e1);
            DataBase.Engines.Add(e2);
            DataBase.Engines.Add(e3);
            DataBase.Engines.Add(e4);
            DataBase.SaveChanges();

            // Add Tests
            Test t1 = new Test { UID = Guid.NewGuid().ToString("n").ToUpper(), DateTime = new DateTime(2015, 01, 12, 12, 47, 07), EngineId = e1.Id, UserId = u1.Id, TIncomingAir = 12, PBarometric = 101.1 };
            Test t2 = new Test { UID = Guid.NewGuid().ToString("n").ToUpper(), DateTime = new DateTime(2015, 01, 13, 14, 23, 08), EngineId = e1.Id, UserId = u1.Id, TIncomingAir = 14, PBarometric = 105.9 };
            Test t3 = new Test { UID = Guid.NewGuid().ToString("n").ToUpper(), DateTime = new DateTime(2015, 01, 14, 10, 00, 01), EngineId = e1.Id, UserId = u2.Id, TIncomingAir = 18, PBarometric = 103.3 };
            Test t4 = new Test { UID = Guid.NewGuid().ToString("n").ToUpper(), DateTime = new DateTime(2015, 01, 15, 19, 05, 54), EngineId = e2.Id, UserId = u1.Id, TIncomingAir = 20, PBarometric = 90.5 };
            Test t5 = new Test { UID = Guid.NewGuid().ToString("n").ToUpper(), DateTime = new DateTime(2015, 01, 13, 16, 22, 32), EngineId = e2.Id, UserId = u3.Id, TIncomingAir = 15, PBarometric = 120.8 };
            Test t6 = new Test { UID = Guid.NewGuid().ToString("n").ToUpper(), DateTime = new DateTime(2015, 01, 16, 11, 21, 04), EngineId = e2.Id, UserId = u4.Id, TIncomingAir = 10, PBarometric = 110.2 };
            Test t7 = new Test { UID = Guid.NewGuid().ToString("n").ToUpper(), DateTime = new DateTime(2015, 01, 18, 15, 51, 21), EngineId = e3.Id, UserId = u3.Id, TIncomingAir = 8, PBarometric = 107.9 };
            Test t8 = new Test { UID = Guid.NewGuid().ToString("n").ToUpper(), DateTime = new DateTime(2015, 03, 01, 11, 45, 32), EngineId = e3.Id, UserId = u4.Id, TIncomingAir = 12, PBarometric = 106.3 };
            Test t9 = new Test { UID = Guid.NewGuid().ToString("n").ToUpper(), DateTime = new DateTime(2015, 03, 26, 17, 08, 31), EngineId = e3.Id, UserId = u2.Id, TIncomingAir = 17, PBarometric = 102.4 };
            Test t10 = new Test { UID = Guid.NewGuid().ToString("n").ToUpper(), DateTime = new DateTime(2015, 03, 15, 16, 23, 41), EngineId = e4.Id, UserId = u1.Id, TIncomingAir = 13, PBarometric = 111.4 };
            DataBase.Tests.Add(t1);
            DataBase.Tests.Add(t2);
            DataBase.Tests.Add(t3);
            DataBase.Tests.Add(t4);
            DataBase.Tests.Add(t5);
            DataBase.Tests.Add(t6);
            DataBase.Tests.Add(t7);
            DataBase.Tests.Add(t8);
            DataBase.Tests.Add(t9);
            DataBase.Tests.Add(t10);
            DataBase.SaveChanges();

            // Add Measurements
            int i = 0;
            Measurement m = new Measurement();
            for (i = 0; i <= 30; i++)
            {
                m = Sensors.GetMeasurements(i, e1);
                m.TestId = t1.Id;
                DataBase.Measurements.Add(m);
            }
            for (i = 0; i <= 30; i++)
            {
                m = Sensors.GetMeasurements(i, e1);
                m.TestId = t2.Id;
                DataBase.Measurements.Add(m);
            }
            for (i = 0; i <= 30; i++)
            {
                m = Sensors.GetMeasurements(i, e1);
                m.TestId = t3.Id;
                DataBase.Measurements.Add(m);
            }
            for (i = 0; i <= 30; i++)
            {
                m = Sensors.GetMeasurements(i, e2);
                m.TestId = t4.Id;
                DataBase.Measurements.Add(m);
            }
            for (i = 0; i <= 30; i++)
            {
                m = Sensors.GetMeasurements(i, e2);
                m.TestId = t5.Id;
                DataBase.Measurements.Add(m);
            }
            for (i = 0; i <= 30; i++)
            {
                m = Sensors.GetMeasurements(i, e2);
                m.TestId = t6.Id;
                DataBase.Measurements.Add(m);
            }
            for (i = 0; i <= 30; i++)
            {
                m = Sensors.GetMeasurements(i, e3);
                m.TestId = t7.Id;
                DataBase.Measurements.Add(m);
            }
            for (i = 0; i <= 30; i++)
            {
                m = Sensors.GetMeasurements(i, e3);
                m.TestId = t8.Id;
                DataBase.Measurements.Add(m);
            }
            for (i = 0; i <= 30; i++)
            {
                m = Sensors.GetMeasurements(i, e3);
                m.TestId = t9.Id;
                DataBase.Measurements.Add(m);
            }
            for (i = 0; i <= 30; i++)
            {
                m = Sensors.GetMeasurements(i, e4);
                m.TestId = t10.Id;
                DataBase.Measurements.Add(m);
            }
            DataBase.SaveChanges();
            return View();
        }
    }
}
