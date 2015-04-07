using System;
using EnginesTest.Models;

namespace EnginesTest.Base
{
    // Эмулятор датчиков
    public static class Sensors
    {
        private const int maxRPMD = 6500;
        private const int minTorqueD = 3000;
        private const int maxTorqueD = 4000;
        private const double fuelKoefD = 3.26;
        private const double maxCoolantTD = 70;
        private const double maxFuelTD = 900;
        private const double maxExhaustGasTD = 700;
        private const double maxOilPD = 7;
        private const double maxExhaustGasPD = 1.5;

        private const int maxRPMG = 9000;        
        private const int minTorqueG = 5500;        
        private const int maxTorqueG = 7000;        
        private const double fuelKoefG = 4.33;        
        private const double maxCoolantTG = 90.0;
        private const double maxFuelTG = 2000;
        private const double maxExhaustGasTG = 1100;
        private const double maxOilPG = 7;
        private const double maxExhaustGasPG = 1.5;

        public static Measurement GetMeasurements(Int32 number, Engine engine)
        {            
            Measurement result = new Measurement();
            result.Torque = Sensors.Torque(number, engine);
            result.RPM = Sensors.RPM(number, engine);
            result.PowerKWh = Convert.ToDouble(result.Torque) * result.RPM / 9549 + engine.Cylinders * 3;
            result.PowerHP = Convert.ToDouble(result.PowerKWh) * 1.36;
            result.FuelConsumption = Sensors.FuelCompulsion(number, engine);
            result.TCoolant = Sensors.TCoolant(number, engine);
            result.TOil = Sensors.TOil(number, engine);
            result.TFuel = Sensors.TFuel(number, engine);
            result.TExhaustGas = Sensors.TExhaustGas(number, engine);
            result.POil = Sensors.POil(number, engine);
            result.PExhaustGas = Sensors.PExhaustGas(number, engine);
            return result;
        }

        private static Int32 RPM(Int32 number, Engine engine)
        {            
            int result = 0;
            int maxRPM = engine.FuelType == "D" ? maxRPMD : maxRPMG;
            if (number == 0 || number == 30)
            {
                number = 1;
            }
            Random rnd = new Random();
            if (number >= 0 && number <= 20)
            {
                result = maxRPM / 20 * number + rnd.Next(-5, 5);
            }
            else if (number >= 21 && number <= 25)
            {
                result = maxRPM + rnd.Next(-5, 5);
            }
            else if (number >= 26 && number <= 30)
            {
                result = maxRPM - maxRPM / 5 * (number - 25) + rnd.Next(-5, 5);
            }
            else
            {
                result = 0;
            }
            return result < 0 ? 0 : result;
        }

        private static Int32 Torque(Int32 number, Engine engine)
        {            
            int result = 0;
            int tMin = engine.FuelType == "D" ? minTorqueD : minTorqueG;
            int tMax = engine.FuelType == "D" ? maxTorqueD : maxTorqueG;
            int maxRPM = engine.FuelType == "D" ? maxRPMD + 5 : maxRPMG + 5;
            Double RPM = Convert.ToDouble(Sensors.RPM(number, engine));
            Random rnd = new Random();
            if (RPM >= 0 && RPM <= tMin)
            {
                result = (int)Math.Round(Convert.ToDouble(engine.RatedTorque) / tMin * RPM) + rnd.Next(-5, 5);
            }
            else if (RPM > tMin && RPM <= tMax)
            {
                result = engine.RatedTorque + rnd.Next(-5, 5);
            }
            else if (RPM >= tMax && RPM <= maxRPM)
            {
                result = (int)Math.Round(Convert.ToDouble(engine.RatedTorque) - Convert.ToDouble(engine.RatedTorque) / (2 * (maxRPM - tMax)) * (RPM - tMax)) + rnd.Next(-5, 5);
            }
            else
            {
                result = 0;
            }
            return result < 0 ? 0 : result;
        }

        private static Double FuelCompulsion(Int32 number, Engine engine)
        {
            Double result = 0;
            Double maxRPM = engine.FuelType == "D" ? maxRPMD + 5 : maxRPMG + 5;
            Double maxFC = (engine.FuelType == "D" ? fuelKoefD : fuelKoefG) * engine.EngineCapacity;
            Double RPM = Convert.ToDouble(Sensors.RPM(number, engine));
            Random rnd = new Random();
            result = (Math.Sin(RPM * ((Math.PI / 40) / (maxRPM / 20)))) * maxFC + rnd.NextDouble() - 0.5;
            return result < 0 ? 0 : result;
        }

        private static Double TCoolant(Int32 number, Engine engine)
        {
            Double result = 0;            
            Double maxT = engine.FuelType == "D" ? maxCoolantTD : maxCoolantTG;  
            Double RPM = Convert.ToDouble(Sensors.RPM(number, engine));
            Random rnd = new Random();
            result = number < 21
                ? (Math.Sin(number * ((Math.PI / 20) / (30 / 20)) - Math.PI / 2) + 1) * (maxT / 6) + maxT * 2 / 3 + rnd.NextDouble() - 0.5 + engine.EngineCapacity * 1.5
                : maxT + rnd.NextDouble() - 0.5 + engine.EngineCapacity * 1.5;
            return result < 0 ? 0 : result;
        }

        private static Double TOil(Int32 number, Engine engine)
        {
            Double result = 0;
            Double maxT = engine.FuelType == "D" ? maxCoolantTD : maxCoolantTG;
            Double RPM = Convert.ToDouble(Sensors.RPM(number, engine));
            Random rnd = new Random();
            result = number < 21
                ? (Math.Sin(number * ((Math.PI / 20) / (30 / 20)) - Math.PI / 2) + 1) * (maxT / 8) + maxT * 3 / 4 + rnd.NextDouble() - 0.5 + engine.EngineCapacity * 1.5 + 13 + rnd.Next(-1, 1)
                : maxT + rnd.NextDouble() - 0.5 + engine.EngineCapacity * 1.5 + 13 + rnd.Next(-1, 1);
            return result < 0 ? 0 : result;
        }

        private static Double TFuel(Int32 number, Engine engine)
        {
            Double result = 0;
            Double maxT = engine.FuelType == "D" ? maxFuelTD : maxFuelTG;
            Double RPM = Convert.ToDouble(Sensors.RPM(number, engine));
            Random rnd = new Random();
            result = number < 24
                ? (Math.Sin(number * (Math.PI / 23) - Math.PI / 2) + 1) * (maxT / 8 * 3) + maxT / 4 + rnd.NextDouble() - 0.5 + engine.EngineCapacity * 1.5 + rnd.Next(-1, 1)
                : (Math.Sin(number * (Math.PI / 8) - Math.PI / 2) + 1) * (maxT / 8 * 3) + maxT / 4 + rnd.NextDouble() - 0.5 + engine.EngineCapacity * 1.5 + rnd.Next(-1, 1);
            return result < 0 ? 0 : result;
        }

        private static Double TExhaustGas(Int32 number, Engine engine)
        {
            Double result = 0;
            Double maxT = engine.FuelType == "D" ? maxExhaustGasTD : maxExhaustGasTG;            
            Random rnd = new Random();
            result = number < 24
                ? (Math.Sin(number * (Math.PI / 23) - Math.PI / 2) + 1) * (maxT / 8 * 3) + maxT / 4 + rnd.NextDouble() - 0.5 + engine.EngineCapacity * 1.5 + rnd.Next(-1, 1)
                : (Math.Sin(number * (Math.PI / 8) - Math.PI / 2) + 1) * (maxT / 8 * 3) + maxT / 4 + rnd.NextDouble() - 0.5 + engine.EngineCapacity * 1.5 + rnd.Next(-1, 1);
            return result < 0 ? 0 : result;
        }

        private static Double POil(Int32 number, Engine engine)
        {
            Double result = 0;
            Double maxP = engine.FuelType == "D" ? maxOilPD : maxOilPG;
            Double RPM = Convert.ToDouble(Sensors.RPM(number, engine));
            Random rnd = new Random();
            result = result = number < 24
                ? (Math.Sin(number * (Math.PI / 23) - Math.PI / 2) + 1) * (maxP / 8 * 3) + maxP / 4
                : (Math.Sin(number * (Math.PI / 8) - Math.PI / 2) + 1) * (maxP / 8 * 3) + maxP / 4;
            return result < 0 ? 0 : result;
        }

        private static Double PExhaustGas(Int32 number, Engine engine)
        {
            Double result = 0;
            Double maxP = engine.FuelType == "D" ? maxExhaustGasPD : maxExhaustGasPG;
            Double RPM = Convert.ToDouble(Sensors.RPM(number, engine));
            Random rnd = new Random();
            result = result = number < 24
                ? (Math.Sin(number * (Math.PI / 23) - Math.PI / 2) + 1) * (maxP / 6 * 3) + maxP / 3
                : (Math.Sin(number * (Math.PI / 8) - Math.PI / 2) + 1) * (maxP / 6 * 3) + maxP / 3;
            return result < 0 ? 0 : result;
        }
    }
}