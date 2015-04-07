using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EnginesTest.Models
{
    // Класс, описывающий поля для формы авторизации
    // и правила проверки значений полей
    public partial class LoginPageView
    {
        [DisplayName("Login")]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Please enter Login")]
        public string Login { get; set; }

        [DisplayName("Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Please enter Password")]
        public string Password { get; set; }
    }
}