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
    public class EnginesController : BaseController
    {
        /** ***************************************************
         * Список двигателей или Информация о двигателе с <ID>
         * GET: api/engines/[<ID>]
         *****************************************************/
        [ActionName("Api"), HttpGet]
        [Authorize(Roles = "testsview,testsmanage")]
        public ApiResult Get(int id = 0, Engine filters = null)
        {
            ApiResult result = new ApiResult();
            try
            {
                IQueryable<Engine> data = DataBase.Engines;

                // Информация о двигателе с <ID>
                if (id > 0)
                {
                    result = GetEntity(id);
                }
                else
                // Список двигателей
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
         * Вспомогательный метод получения списка двигателей
         */
        private ApiResult GetList(Engine filters)
        {
            ApiResult result = new ApiResult();
            try
            {
                IQueryable<Engine> list = DataBase.Engines;

                // Фильтрация по параметрам из строки запроса
                Type eType = new Engine().GetType();
                Type[] exprArgTypes = { list.ElementType };
                foreach (PropertyInfo index in eType.GetProperties())
                {
                    // свойство не виртуальное и пришло значение в форме
                    if (!eType.GetProperty(index.Name).GetGetMethod().IsVirtual && Request.Params.Get(index.Name) != null)
                    {
                        ParameterExpression p = Expression.Parameter(typeof(Engine), "p");
                        MemberExpression member = Expression.PropertyOrField(p, index.Name);
                        LambdaExpression lambda = Expression.Lambda<Func<Engine, bool>>(Expression.Equal(member, Expression.Constant(filters.GetType().GetProperty(index.Name).GetValue(filters))), p);
                        MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, list.Expression, lambda);
                        list = (IQueryable<Engine>)list.Provider.CreateQuery(methodCall);
                    }
                }
                
                result.DataSet = list.Select(engine => new
                {
                    Id = engine.Id,
                    UID = engine.UID,
                    Model = engine.Model,
                    SerialNumber = engine.SerialNumber,
                    Configuration = engine.Configuration,
                    Cylinders = engine.Cylinders,
                    EngineCapacity = engine.EngineCapacity,
                    ValversPerCylinder = engine.ValversPerCylinder,
                    FuelType = engine.FuelType,
                    RatedTorque = engine.RatedTorque,
                    TestsCount = engine.Tests.Count()
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
         * Вспомогательный метод получения информации о двигателе с <ID>
         */
        private ApiResult GetEntity(int id)
        {
            ApiResult result = new ApiResult();
            try
            {
                result.DataSet = DataBase.Engines
                    .Where(engine => engine.Id == id)
                    .Select(engine => new
                    {
                        Id = engine.Id,
                        UID = engine.UID,
                        Model = engine.Model,
                        SerialNumber = engine.SerialNumber,
                        Configuration = engine.Configuration,
                        Cylinders = engine.Cylinders,
                        EngineCapacity = engine.EngineCapacity,
                        ValversPerCylinder = engine.ValversPerCylinder,
                        FuelType = engine.FuelType,
                        RatedTorque = engine.RatedTorque
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
         * Добавление нового двигателя
         * PUT: api/engines/
         *****************************************************/
        [ActionName("Api"), HttpPut]
        [Authorize(Roles = "testsmanage")]
        public ApiResult Put(Engine data)
        {
            ApiResult result = new ApiResult();
            try
            {
                DataBase.Engines.Add(data);
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
         * Редактирование двигателя с <ID>
         * POST: api/engines/<ID>
         *****************************************************/
        [ActionName("Api"), HttpPost]
        [Authorize(Roles = "testsmanage")]
        public ApiResult Post(int id, Engine data)
        {
            ApiResult result = new ApiResult();
            try
            {
                Engine Engine = DataBase.Engines
                    .Where(engine => engine.Id == id)
                    .FirstOrDefault();
                if (Engine != null)
                {

                    // Небольшой workaround
                    // Нужно обновить только те поля, которые пришли в запросе
                    Type eType = Engine.GetType();
                    Type dType = data.GetType();
                    foreach (PropertyInfo index in eType.GetProperties())
                    {
                        // свойство не виртуальное и пришло значение в форме
                        if (!eType.GetProperty(index.Name).GetGetMethod().IsVirtual && Request.Form.Get(index.Name) != null)
                        {
                            eType.GetProperty(index.Name).SetValue(Engine, dType.GetProperty(index.Name).GetValue(data));
                        }
                    }

                    DataBase.Entry(Engine).CurrentValues.SetValues(Engine);
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
         * Удаление двигателя с <ID>
         * DELETE: api/engines/<ID>
         *****************************************************/
        [ActionName("Api"), HttpDelete]
        [Authorize(Roles = "testsmanage")]
        public ApiResult Delete(int id)
        {
            ApiResult result = new ApiResult();
            try
            {
                Engine Engine = DataBase.Engines
                    .Where(engine => engine.Id == id)
                    .FirstOrDefault();
                if (Engine != null)
                {
                    DataBase.Engines.Remove(Engine);
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
