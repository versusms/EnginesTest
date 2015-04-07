using System.Web.Mvc;
using System.Web.Security;
using EnginesTest.Models;
using EnginesTest.Base;

namespace EnginesTest.Controllers
{
    /**
     * Контроллер для авторизации пользователя
     */
    public class SecurityController : BaseController
    {
        /**
         * Отображение формы авторизации
         */
        [HttpGet]
        public ActionResult Login()
        {
            Response.StatusCode = 401;
            return View(new LoginPageView());
        }

        /**
         * Обработка формы с логином и паролем
         */
        [HttpPost]
        public ActionResult Login(LoginPageView LoginData, string ReturnUrl = "/")
        {
            if (ModelState.IsValid)
            {
                // Проверяем пользователя по логину и паролю
                if (SecurityManager.ValidateUser(LoginData.Login, LoginData.Password))
                {
                    // Если все ок - запоминаем его
                    FormsAuthentication.SetAuthCookie(LoginData.Login, false);
                    return Redirect(ReturnUrl);
                }
                else
                {
                    ModelState.AddModelError("Login", "Login or password is incorrect");
                    ModelState.AddModelError("Password", "Login or password is incorrect");
                }
            }

            return View(LoginData);
        }

        /**
         * Выход пользователя из приложения
         */
        public ActionResult Logout()
        {
            // Разлогинивание пользователя из приложения
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", new { ReturnUrl = "/" });
        }
    }
}
