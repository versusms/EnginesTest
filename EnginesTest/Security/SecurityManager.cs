using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using EnginesTest.Models;

namespace EnginesTest.Security
{
    /**
     * Класс работы с пользователями - Менеджер безопасности
     */
    public class SecurityManager
    {
        private DataModelContainer DataBase = new DataModelContainer();

        // Проверка поользователя по логину и паролю
        public bool ValidateUser(string username, string password)
        {
            String PassHash = HashPassword(password);
            User User = DataBase.Users
                .Where(user => user.Login == username && user.Password == PassHash && user.Deleted == false)
                .Select(user => user)
                .FirstOrDefault();

            return User != null;
        }

        // Получение информации о пользователе по его логину
        public User GetUserInfo(string username)
        {
            User User = DataBase.Users
                .Where(user => user.Login == username)
                .Select(user => user)
                .FirstOrDefault();

            return User;
        }

        // Определение ролей пользователя по его логину
        public string[] GetRolesForUser(string username)
        {
            string[] result = {};
            List<String> roles = new List<String>();

            try
            {
                User User = DataBase.Users
                    .Where(user => user.Login == username)
                    .Select(user => user)
                    .FirstOrDefault();

                if (User != null)
                {
                    // Авторизованный пользователь
                    roles.Add("user");

                    // Доступ к просмотру тестов
                    if (User.SecurityTV)
                    {
                        roles.Add("testsview");
                    }

                    // Доступ к управлению тестами
                    if (User.SecurityTM)
                    {
                        roles.Add("testsmanage");
                    }

                    // Доступ к управлению пользователями и должностями
                    if (User.SecurityUM)
                    {
                        roles.Add("usersmanage");
                    }
                    result = roles.ToArray<String>();
                }
            }
            catch (Exception e) {}

            return result;
        }

        // Генерация MD5-хеша пароля
        // т.к. хранить пароли пользователей в базе в открытом виде - не есть хорошо
        public string HashPassword(string password)
        {
            UnicodeEncoding encoding = new UnicodeEncoding();
            HashAlgorithm hash = HashAlgorithm.Create("MD5");

            if (password != null && hash != null && encoding != null)
            {
                byte[] valueToHash = new byte[encoding.GetByteCount(password)];
                byte[] binaryPassword = encoding.GetBytes(password);
                binaryPassword.CopyTo(valueToHash, 0);
                byte[] hashValue = hash.ComputeHash(valueToHash);
                string hashedPassword = "";
                foreach (byte hexdigit in hashValue)
                {
                    hashedPassword += hexdigit.ToString("X2", CultureInfo.InvariantCulture.NumberFormat);
                }

                return hashedPassword;
            }

            return null;
        } 
    }
}