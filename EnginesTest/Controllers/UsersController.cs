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
    [Authorize(Roles = "user")]
    public class UsersController : BaseController
    {
        /** ***************************************************
         * Список пользоателей или Информация о пользователе с <ID>
         * GET: api/users/[<ID>]
         *****************************************************/
        [ActionName("Api"), HttpGet]
        public ApiResult Get(int id = 0, User filters = null)
        {
            ApiResult result = new ApiResult();

            //             
            try
            {
                IQueryable<User> data = DataBase.Users;

                // Информация о пользователе с <ID>
                if (id > 0)
                {
                    // Пользователи без SecurityUM могут получать информацию только о себе
                    if (!SecurityManager.GetUserInfo(HttpContext.User.Identity.Name).SecurityUM &&
                        SecurityManager.GetUserInfo(HttpContext.User.Identity.Name).Id != id)
                    {
                        Response.StatusCode = 401;
                    }
                    else
                    {
                        result = GetEntity(id);
                    }
                }
                else
                // Список пользователей
                {
                    // Пользователи без SecurityUM не могутсмотреть список пользователей
                    if (!SecurityManager.GetUserInfo(HttpContext.User.Identity.Name).SecurityUM)
                    {
                        Response.StatusCode = 401;
                    }
                    else
                    {
                        result = GetList(filters);
                    }
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
         * Вспомогательный метод получения списка пользователей
         */
        private ApiResult GetList(User filters)
        {
            ApiResult result = new ApiResult();
            try
            {
                IQueryable<User> list = DataBase.Users;

                // Фильтрация по параметрам из строки запроса
                Type uType = new User().GetType();
                Type[] exprArgTypes = { list.ElementType };
                foreach (PropertyInfo index in uType.GetProperties())
                {
                    // свойство не виртуальное и пришло значение в форме
                    if (!uType.GetProperty(index.Name).GetGetMethod().IsVirtual && Request.Params.Get(index.Name) != null)
                    {
                        ParameterExpression p = Expression.Parameter(typeof(User), "p");
                        MemberExpression member = Expression.PropertyOrField(p, index.Name);
                        LambdaExpression lambda = Expression.Lambda<Func<User, bool>>(Expression.Equal(member, Expression.Constant(filters.GetType().GetProperty(index.Name).GetValue(filters))), p);
                        MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, list.Expression, lambda);
                        list = (IQueryable<User>)list.Provider.CreateQuery(methodCall);
                    }
                }

                result.DataSet = list.Select(user => new
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    MiddleName = (user.MiddleName != null ? user.MiddleName : ""),
                    Phone = (user.Phone != null ? user.Phone : ""),
                    Email = (user.Email != null ? user.Email : ""),
                    JobTitleId = user.JobTitleId,
                    JobTitle = user.JobTitle.Title,
                    ProfileImage = (user.ProfileImage != null ? user.ProfileImage : "noprofile.jpg"),                 
                    Login = user.Login,
                    Deleted = user.Deleted
                }).OrderBy(user => user.LastName).ThenBy(user => user.FirstName);
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
         * Вспомогательный метод получения информации о пользователе с <ID>
         */
        private ApiResult GetEntity(int id)
        {
            ApiResult result = new ApiResult();
            try
            {
                result.DataSet = DataBase.Users
                    .Where(user => user.Id == id)
                    .Select(user => new
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        MiddleName = user.MiddleName,
                        Phone = user.Phone,
                        Email = user.Email,
                        JobTitleId = user.JobTitleId,
                        JobTitle = user.JobTitle.Title,
                        ProfileImage = user.ProfileImage,
                        SecurityUM = user.SecurityUM,
                        SecurityTV = user.SecurityTV,
                        SecurityTM = user.SecurityTM,
                        Login = user.Login,
                        Deleted = user.Deleted
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
         * Добавление нового пользователя
         * PUT: api/users/
         *****************************************************/
        [ActionName("Api"), HttpPut]
        [Authorize(Roles = "usersmanage")]
        public ApiResult Put(User data)
        {
            // TODO: JS: Проверка на существование
            ApiResult result = new ApiResult();
            try
            {
                // Выставим флаг на всякий случай, нет смысла добавлять сразу удаленную запись
                data.Deleted = false;
                // Шифруем пароль если пришел
                if (data.Password != null)
                {
                    data.Password = SecurityManager.HashPassword(data.Password);
                }
                DataBase.Users.Add(data);
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
         * Редактирование пользователя с <ID>
         * POST: api/users/<ID>
         *****************************************************/
        [ActionName("Api"), HttpPost]
        public ApiResult Post(int id, User data)
        {
            ApiResult result = new ApiResult();
            try
            {
                // Пользователи без SecurityUM могут менять только свой профиль
                if (!SecurityManager.GetUserInfo(HttpContext.User.Identity.Name).SecurityUM &&
                        SecurityManager.GetUserInfo(HttpContext.User.Identity.Name).Id != id)
                {
                    Response.StatusCode = 401;
                }
                else
                {
                    // Нельзя самому себе менять доступ или должность через профиль
                    if (!SecurityManager.GetUserInfo(HttpContext.User.Identity.Name).SecurityUM &&
                        (Request.Form.Get("JobTitleId") != null || Request.Form.Get("SecurityTM") != null || Request.Form.Get("SecurityTV") != null || Request.Form.Get("SecurityUM") != null))
                    {
                        Response.StatusCode = 401;
                    }
                    else
                    {
                        User User = DataBase.Users
                            .Where(user => user.Id == id)
                            .FirstOrDefault();
                        if (User != null)
                        {
                            // Если загружается новая картинка - сохраняем ее и обновляем данные
                            if (data != null && data.ProfileImage != User.ProfileImage)
                            {
                                // проверка на существование файла и удаление текущего изображения
                                if (User.ProfileImage != null && System.IO.File.Exists(HttpRuntime.AppDomainAppPath + "/Content/Images/ProfileImages/" + User.ProfileImage))
                                {
                                    System.IO.File.Delete(HttpRuntime.AppDomainAppPath + "/Content/Images/ProfileImages/" + User.ProfileImage);
                                }
                            }
                            // Шифруем пароль если пришел
                            if (data.Password != null)
                            {
                                data.Password = SecurityManager.HashPassword(data.Password);
                            }
                            // Небольшой workaround
                            // Нужно обновить только те поля, которые пришли в запросе
                            Type uType = User.GetType();
                            Type dType = data.GetType();
                            foreach (PropertyInfo index in uType.GetProperties())
                            {
                                // свойство не виртуальное и пришло значение в форме
                                if (!uType.GetProperty(index.Name).GetGetMethod().IsVirtual && Request.Form.Get(index.Name) != null)
                                {
                                    uType.GetProperty(index.Name).SetValue(User, dType.GetProperty(index.Name).GetValue(data));
                                }
                            }

                            DataBase.Entry(User).CurrentValues.SetValues(User);
                            DataBase.SaveChanges();
                            result.Success = true;
                            result.DataSet = GetEntity(User.Id).DataSet;
                        }
                        else
                        {
                            result.Success = false;
                            result.Message = "Entity not found";
                        }
                    }
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
         * Удаление пользователя с <ID>
         * DELETE: api/users/<ID>
         *****************************************************/
        [ActionName("Api"), HttpDelete]
        [Authorize(Roles = "usersmanage")]
        public ApiResult Delete(int id)
        {
            ApiResult result = new ApiResult();
            try
            {
                User User = DataBase.Users
                    .Where(user => user.Id == id)
                    .FirstOrDefault();
                if (User != null)
                {
                    User.Deleted = true;
                    DataBase.Entry(User).CurrentValues.SetValues(User);
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
         * загрузка изображения для профиля пользователя
         * POST: users/uploadprofileimage/
         *****************************************************/
        [HttpPost]
        public ApiResult UploadProfileImage(HttpPostedFileBase ProfileImage = null)
        {
            ApiResult result = new ApiResult();
            try
            {
                // Если загружается новая картинка - сохраняем ее
                if (ProfileImage != null)
                {
                    if (ProfileImage.ContentType == "image/jpeg" && ProfileImage.ContentLength <= 512000)
                    {
                        byte[] ImageContent = new byte[ProfileImage.ContentLength];
                        ProfileImage.InputStream.Read(ImageContent, 0, ProfileImage.ContentLength);
                        // Генерируем уникальное имя файла, чтобы они не перезатирали друг друга
                        string UniqueFileName = Guid.NewGuid().ToString("n") + ".jpg";
                        System.IO.File.WriteAllBytes(HttpRuntime.AppDomainAppPath + "/Content/Images/ProfileImages/" + UniqueFileName, ImageContent);                        
                        result.Success = true;
                        result.DataSet = UniqueFileName;
                    }
                    else
                    {
                        result.Success = false;
                        result.Message = "Profile image must be in JPG-format & less than 500Kb";
                        return result;
                    }
                }
                else
                {
                    result.Success = false;
                    result.Message = "No image was uploaded";
                    return result;
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
