using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using EnginesTest.Base;
using EnginesTest.Models;
using System.Reflection;

namespace EnginesTest.Controllers
{
    [Authorize(Roles = "testsview,testsmanage")]
    public class MeasurementsController : BaseController
    {
        /** ***************************************************
         * Список измерений или Информация об измерении с <ID>
         * GET: api/measurements/[<ID>]
         *****************************************************/
        [ActionName("Api"), HttpGet]
        [Authorize(Roles = "testsview,testsmanage")]
        public ApiResult Get(int id = 0, Measurement filters = null)
        {
            ApiResult result = new ApiResult();
            try
            {
                IQueryable<Measurement> data = DataBase.Measurements;

                // Информация о измерении с <ID>
                if (id > 0)
                {
                    result = GetEntity(id);
                }
                else
                // Список измерений
                {
                    result = GetList(filters);
                }
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Message = e.ToString();
            }
            return result;
        }

        /**
         * Вспомогательный метод получения списка измерений
         */
        private ApiResult GetList(Measurement filters)
        {
            ApiResult result = new ApiResult();
            try
            {
                IQueryable<Measurement> list = DataBase.Measurements;

                // Фильтрация по параметрам из строки запроса
                Type mType = new Measurement().GetType();
                Type[] exprArgTypes = { list.ElementType };
                foreach (PropertyInfo index in mType.GetProperties())
                {
                    // свойство не виртуальное и пришло значение в форме
                    if (!mType.GetProperty(index.Name).GetGetMethod().IsVirtual && Request.Params.Get(index.Name) != null)
                    {
                        ParameterExpression p = Expression.Parameter(typeof(Measurement), "p");
                        MemberExpression member = Expression.PropertyOrField(p, index.Name);
                        LambdaExpression lambda = Expression.Lambda<Func<Measurement, bool>>(Expression.Equal(member, Expression.Constant(filters.GetType().GetProperty(index.Name).GetValue(filters))), p);
                        MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, list.Expression, lambda);
                        list = (IQueryable<Measurement>)list.Provider.CreateQuery(methodCall);
                    }
                }

                result.DataSet = list.Select(measurement => new
                {
                    Id = measurement.Id,
                    TestId = measurement.TestId,
                    Torque = measurement.Torque,
                    RPM = measurement.RPM,
                    FuelConsumption = measurement.FuelConsumption,
                    TCoolant = measurement.TCoolant,
                    TOil = measurement.TFuel,
                    TExhaustGas = measurement.TExhaustGas,
                    POil = measurement.POil,
                    PExhaustGas = measurement.PExhaustGas,
                    PowerHP = measurement.PowerHP,
                    PowerKWh = measurement.PowerKWh
                });
                result.Success = true;
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Message = e.ToString();
            }
            return result;
        }

        /**
         * Вспомогательный метод получения информации об измерении с <ID>
         */
        private ApiResult GetEntity(int id)
        {
            ApiResult result = new ApiResult();
            try
            {
                result.DataSet = DataBase.Measurements
                    .Where(measurement => measurement.Id == id)
                    .Select(measurement => new
                    {
                        Id = measurement.Id,
                        TestId = measurement.TestId,
                        Torque = measurement.Torque,
                        RPM = measurement.RPM,
                        FuelConsumption = measurement.FuelConsumption,
                        TCoolant = measurement.TCoolant,
                        TOil = measurement.TFuel,
                        TExhaustGas = measurement.TExhaustGas,
                        POil = measurement.POil,
                        PExhaustGas = measurement.PExhaustGas,
                        PowerHP = measurement.PowerHP,
                        PowerKWh = measurement.PowerKWh
                    })
                    .FirstOrDefault();
                // Проверяем существует ли запись в базе
                if (result.DataSet != null)
                {
                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                    result.Message = "Entity not found";
                }
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Message = e.ToString();
            }
            return result;
        }

        /** ***************************************************
         * Добавление нового измерения
         * PUT: api/measurements/
         *****************************************************/
        [ActionName("Api"), HttpPut]
        [Authorize(Roles = "testsmanage")]
        public ApiResult Put(Measurement data)
        {
            ApiResult result = new ApiResult();
            try
            {
                DataBase.Measurements.Add(data);
                DataBase.SaveChanges();

                result.Success = true;
                result.DataSet = data;
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Message = e.ToString();
            }            

            return result;
        }

        /** ***************************************************
         * Редактирование измерения с <ID>
         * POST: api/measurements/<ID>
         *****************************************************/
        [ActionName("Api"), HttpPost]
        [Authorize(Roles = "testsmanage")]
        public ApiResult Post(int id, Measurement data)
        {
            ApiResult result = new ApiResult();
            try
            {
                Measurement Measurement = DataBase.Measurements
                    .Where(measurement => measurement.Id == id)
                    .FirstOrDefault();
                if (Measurement != null)
                {

                    // Небольшой workaround
                    // Нужно обновить только те поля, которые пришли в запросе
                    Type mType = Measurement.GetType();
                    Type dType = data.GetType();
                    foreach (PropertyInfo index in mType.GetProperties())
                    {
                        // свойство не виртуальное и пришло значение в форме
                        if (!mType.GetProperty(index.Name).GetGetMethod().IsVirtual && Request.Form.Get(index.Name) != null)
                        {
                            mType.GetProperty(index.Name).SetValue(Measurement, dType.GetProperty(index.Name).GetValue(data));
                        }
                    }

                    DataBase.Entry(Measurement).CurrentValues.SetValues(Measurement);
                    DataBase.SaveChanges();
                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                    result.Message = "Entity not found";
                }
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Message = e.ToString();
            }
            return result;
        }

        /** ***************************************************
         * Удаление измерения с <ID>
         * DELETE: api/measurements/<ID>
         *****************************************************/
        [ActionName("Api"), HttpDelete]
        [Authorize(Roles = "testsmanage")]
        public ApiResult Delete(int id)
        {
            ApiResult result = new ApiResult();
            try
            {
                Measurement Measurement = DataBase.Measurements
                    .Where(measurement => measurement.Id == id)
                    .FirstOrDefault();
                if (Measurement != null)
                {
                    DataBase.Measurements.Remove(Measurement);
                    DataBase.SaveChanges();
                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                    result.Message = "Entity not found";
                }
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Message = e.ToString();
            }
            return result;
        }
    }
}
