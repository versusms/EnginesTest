using System;
using EnginesTest.Security;

namespace EnginesTest.Security
{
    /**
     * Класс для обработки определения ролей пользователей в системе
     * Можно переопределить только этот метод, так как именно он используется при фильтрации
     */
    public class RoleProvider : System.Web.Security.RoleProvider
    {
        // Менеджер безопасности
        SecurityManager SecurityManager = new SecurityManager();

        /**
         * Метод возвращает список ролей для пользователя по его логину
         */
        public override string[] GetRolesForUser(string username)
        {
            // Вызываем "наш" метод определения ролей пользователя
            return SecurityManager.GetRolesForUser(username);
        }
        /**
         * Остальные методы для данного примера можно не реализовывать
         */
        #region Абстрактные методы, реализация не обязательна

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}