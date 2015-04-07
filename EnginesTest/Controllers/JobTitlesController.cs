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
    [Authorize(Roles = "usersmanage")]
    public class JobTitlesController : BaseController
    {
        /** ***************************************************
         * Список должностей или Информация о должности с <ID>
         * GET: api/jobtitles/[<ID>]
         *****************************************************/
        [ActionName("Api"), HttpGet]
        public ApiResult Get(int id = 0, JobTitle filters = null)
        {
            ApiResult result = new ApiResult();
            try
            {
                IQueryable<JobTitle> data = DataBase.JobTitles;

                // Информация о должности с <ID>
                if (id > 0)
                {
                    result = GetEntity(id);
                }
                else
                // Список должностей
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
         * Вспомогательный метод получения списка должностей
         */
        private ApiResult GetList(JobTitle filters)
        {
            ApiResult result = new ApiResult();
            try
            {
                IQueryable<JobTitle> list = DataBase.JobTitles;

                // Фильтрация по параметрам из строки запроса
                Type jtType = new JobTitle().GetType();
                Type[] exprArgTypes = { list.ElementType };
                foreach (PropertyInfo index in jtType.GetProperties())
                {
                    // свойство не виртуальное и пришло значение в форме
                    if (!jtType.GetProperty(index.Name).GetGetMethod().IsVirtual && Request.Params.Get(index.Name) != null)
                    {
                        ParameterExpression p = Expression.Parameter(typeof(JobTitle), "p");
                        MemberExpression member = Expression.PropertyOrField(p, index.Name);
                        LambdaExpression lambda = Expression.Lambda<Func<JobTitle, bool>>(Expression.Equal(member, Expression.Constant(filters.GetType().GetProperty(index.Name).GetValue(filters))), p);
                        MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, list.Expression, lambda);
                        list = (IQueryable<JobTitle>)list.Provider.CreateQuery(methodCall);
                    }
                }

                result.DataSet = list.Select(jobtitle => new
                {
                    Id = jobtitle.Id,
                    Title = jobtitle.Title,
                    Deleted = jobtitle.Deleted,
                    UsersCount = jobtitle.Users.Count()
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
         * Вспомогательный метод получения информации о должности с <ID>
         */
        private ApiResult GetEntity(int id)
        {
            ApiResult result = new ApiResult();
            try
            {
                result.DataSet = DataBase.JobTitles
                    .Where(jobtitle => jobtitle.Id == id)
                    .Select(jobtitle => new
                    {
                        Id = jobtitle.Id,
                        Title = jobtitle.Title,
                        Deleted = jobtitle.Deleted
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
         * Добавление новой должности
         * PUT: api/jobtitles/
         *****************************************************/
        [ActionName("Api"), HttpPut]
        public ApiResult Put(JobTitle data)
        {
            ApiResult result = new ApiResult();
            try
            {
                // Выставим флаг на всякий случай, нет смысла добавлять сразу удаленную запись
                data.Deleted = false;
                DataBase.JobTitles.Add(data);
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
         * Редактирование должности с <ID>
         * POST: api/jobtitles/<ID>
         *****************************************************/
        [ActionName("Api"), HttpPost]
        public ApiResult Post(int id, JobTitle data)
        {
            ApiResult result = new ApiResult();
            try
            {
                JobTitle JobTitle = DataBase.JobTitles
                    .Where(jobtitle => jobtitle.Id == id)
                    .FirstOrDefault();
                if (JobTitle != null)
                {

                    // Небольшой workaround
                    // Нужно обновить только те поля, которые пришли в запросе
                    Type jtType = JobTitle.GetType();
                    Type dType = data.GetType();
                    foreach (PropertyInfo index in jtType.GetProperties())
                    {
                        // свойство не виртуальное и пришло значение в форме
                        if (!jtType.GetProperty(index.Name).GetGetMethod().IsVirtual && Request.Form.Get(index.Name) != null)
                        {
                            jtType.GetProperty(index.Name).SetValue(JobTitle, dType.GetProperty(index.Name).GetValue(data));
                        }
                    }

                    DataBase.Entry(JobTitle).CurrentValues.SetValues(JobTitle);
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
         * Удаление должности с <ID>
         * DELETE: api/jobtitles/<ID>
         *****************************************************/
        [ActionName("Api"), HttpDelete]
        public ApiResult Delete(int id)
        {
            ApiResult result = new ApiResult();
            try
            {
                JobTitle JobTitle = DataBase.JobTitles
                    .Where(jobtitle => jobtitle.Id == id)
                    .FirstOrDefault();
                if (JobTitle != null)
                {
                    JobTitle.Deleted = true;
                    DataBase.Entry(JobTitle).CurrentValues.SetValues(JobTitle);
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
