using System;
using System.Web.Mvc;
using EnginesTest.Models;
using EnginesTest.Security;
using System.Collections.Generic;

namespace EnginesTest.Base
{
    /**
     * Базовый контроллер с контекстом подключения к БД и Менеджером безопасности
     */
    public class BaseController : Controller
    {
        // Контекст подключения к БД
        protected DataModelContainer DataBase = new DataModelContainer();

        // Менеджер безопасности
        protected SecurityManager SecurityManager = new SecurityManager(); 
    }
}
