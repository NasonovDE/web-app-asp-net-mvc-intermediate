using System.ComponentModel.DataAnnotations;

namespace RestApiClient.Models
{
    public enum SettingType
    {
        [Display(Name = "Пароль")]
        Password = 1,
    }
}