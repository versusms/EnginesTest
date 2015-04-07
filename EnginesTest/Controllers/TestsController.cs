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
    public class TestsController : BaseController
    {
        /** ***************************************************
         * Список тестов или Информация о тесте с <ID>
         * GET: api/tests/[<ID>]
         *****************************************************/
        [ActionName("Api"), HttpGet]
        [Authorize(Roles = "testsview,testsmanage")]
        public ApiResult Get(int id = 0, Test filters = null)
        {
            ApiResult result = new ApiResult();
            try
            {
                IQueryable<Test> data = DataBase.Tests;

                // Информация о тесте с <ID>
                if (id > 0)
                {
                    result = GetEntity(id);
                }
                else
                // Список тестов
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
         * Вспомогательный метод получения списка тестов
         */
        private ApiResult GetList(Test filters)
        {
            ApiResult result = new ApiResult();
            try
            {
                IQueryable<Test> list = DataBase.Tests;

                // Фильтрация по параметрам из строки запроса
                Type tType = new Test().GetType();
                Type[] exprArgTypes = { list.ElementType };
                foreach (PropertyInfo index in tType.GetProperties())
                {
                    // свойство не виртуальное и пришло значение в форме
                    if (!tType.GetProperty(index.Name).GetGetMethod().IsVirtual && Request.Params.Get(index.Name) != null)
                    {
                        ParameterExpression p = Expression.Parameter(typeof(Test), "p");
                        MemberExpression member = Expression.PropertyOrField(p, index.Name);
                        LambdaExpression lambda = Expression.Lambda<Func<Test, bool>>(Expression.Equal(member, Expression.Constant(filters.GetType().GetProperty(index.Name).GetValue(filters))), p);
                        MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, list.Expression, lambda);
                        list = (IQueryable<Test>)list.Provider.CreateQuery(methodCall);
                    }
                }

                result.DataSet = list.Select(test => new
                {
                    Id = test.Id,
                    UID = test.UID,
                    DateTime = test.DateTime,
                    EngineId = test.EngineId,
                    UserId = test.UserId,
                    User = test.User.LastName + " " + test.User.FirstName,
                    TIncomingAir = test.TIncomingAir,
                    PBarometric = test.PBarometric
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
         * Вспомогательный метод получения информации о тесте с <ID>
         */
        private ApiResult GetEntity(int id)
        {
            ApiResult result = new ApiResult();
            try
            {
                result.DataSet = DataBase.Tests
                    .Where(test => test.Id == id)
                    .Select(test => new
                    {
                        Id = test.Id,
                        UID = test.UID,
                        DateTime = test.DateTime,
                        EngineId = test.EngineId,
                        UserId = test.UserId,
                        User = test.User.LastName + " " + test.User.FirstName,
                        TIncomingAir = test.TIncomingAir,
                        PBarometric = test.PBarometric
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
         * Добавление нового теста
         * PUT: api/tests/
         *****************************************************/
        [ActionName("Api"), HttpPut]
        [Authorize(Roles = "testsmanage")]
        public ApiResult Put(Test data)
        {
            ApiResult result = new ApiResult();
            try
            {
                DataBase.Tests.Add(data);
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
         * Редактирование теста с <ID>
         * POST: api/tests/<ID>
         *****************************************************/
        [ActionName("Api"), HttpPost]
        [Authorize(Roles = "testsmanage")]
        public ApiResult Post(int id, Test data)
        {
            ApiResult result = new ApiResult();
            try
            {
                Test Test = DataBase.Tests
                    .Where(test => test.Id == id)
                    .FirstOrDefault();
                if (Test != null)
                {

                    // Небольшой workaround
                    // Нужно обновить только те поля, которые пришли в запросе
                    Type tType = Test.GetType();
                    Type dType = data.GetType();
                    foreach (PropertyInfo index in tType.GetProperties())
                    {
                        // свойство не виртуальное и пришло значение в форме
                        if (!tType.GetProperty(index.Name).GetGetMethod().IsVirtual && Request.Form.Get(index.Name) != null)
                        {
                            tType.GetProperty(index.Name).SetValue(Test, dType.GetProperty(index.Name).GetValue(data));
                        }
                    }

                    DataBase.Entry(Test).CurrentValues.SetValues(Test);
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
         * Удаление теста с <ID>
         * DELETE: api/tests/<ID>
         *****************************************************/
        [ActionName("Api"), HttpDelete]
        [Authorize(Roles = "testsmanage")]
        public ApiResult Delete(int id)
        {
            ApiResult result = new ApiResult();
            try
            {
                Test Test = DataBase.Tests
                    .Where(test => test.Id == id)
                    .FirstOrDefault();
                if (Test != null)
                {
                    DataBase.Tests.Remove(Test);
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
