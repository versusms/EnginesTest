using System;
using System.Linq;
using System.Web.Mvc;
using EnginesTest.Base;
using EnginesTest.Models;

namespace EnginesTest.Controllers
{
    [Authorize(Roles = "testsmanage")]
    public class SensorsController : BaseController
    {

        /**
         * Проверка доступности датчиков, отвечает true на 3-ий запрос
         */
        public ApiResult Check(int number)
        {
            ApiResult result = new ApiResult();
            result.Success = number == 3;
            result.Message = number == 3 ? "Sensors are online" : "Sensors are offline";            
            return result;
        }

        /**
         * Эмуляция получения данных с датчиков
         * 
         * FuelType = "G";
         * RatedTorque = 1000;
         * EngineCapacity = 8.5;
         * Cylinders = 12;
         */
        public ApiResult Measurements(int number, int engineId)
        {
            ApiResult result = new ApiResult();
            try
            {
                Engine engine = DataBase.Engines.Where(e => e.Id == engineId).Select(e => e).FirstOrDefault();
                if (engine == null)
                {
                    engine = new Engine();
                }
                result.Success = true;                
                result.DataSet = Sensors.GetMeasurements(number, engine);
            }
            catch
            {
                result.Success = true;
                result.Message = "Sensors are offline";
            }
            return result;
        }

    }
}
