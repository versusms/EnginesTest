using System;
using System.Web.Mvc;
using EnginesTest.Models;

namespace EnginesTest.Base
{    
    /**
     * Представляет базовый результат на основе базового формата ответа
     */
    public class ApiResult : JsonResult
    {
        public Boolean Success
        {
            get
            {
                return ((ApiResponse)this.Data).Success;
            }
            set
            {
                ((ApiResponse)this.Data).Success = value;
            }
        }

        public String Message
        {
            get
            {
                return ((ApiResponse)this.Data).Message;
            }
            set
            {
                ((ApiResponse)this.Data).Message = value;
            }
        }

        public Object DataSet
        {
            get
            {
                return ((ApiResponse)this.Data).DataSet;
            }
            set
            {
                ((ApiResponse)this.Data).DataSet = value;
            }
        }

        /**
         * Конструкторы
         */
        public ApiResult()
        {
            this.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            this.Data = new ApiResponse();
        }

        public ApiResult(Boolean success, String message)
        {
            this.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            this.Data = new ApiResponse { Success = success, Message = message };
        }

        public ApiResult(Boolean success, Object dataset)
        {
            this.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            this.Data = new ApiResponse { Success = success, DataSet = dataset };
        }

        public ApiResult(Boolean success, String message, Object dataset)
        {
            this.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            this.Data = new ApiResponse { Success = success, Message = message, DataSet = dataset };
        }
    }
}