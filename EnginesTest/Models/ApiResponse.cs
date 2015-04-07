using System;

namespace EnginesTest.Models
{
    /**
     * Представляет базовый формат JSON-ответа для API
     * Формат:
     * {
     *      Success : <Boolean>,
     *      Message : <String>,
     *      Data    : <JSONObject>
     * }
     */
    public class ApiResponse
    {
        public Boolean Success = false;
        public String Message = "";
        public Object DataSet = new object();
    }
}