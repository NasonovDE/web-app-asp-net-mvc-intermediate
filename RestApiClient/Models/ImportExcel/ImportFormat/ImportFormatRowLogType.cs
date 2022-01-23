using System.ComponentModel.DataAnnotations;

namespace RestApiClient.Models
{
    public enum ImportFormatRowLogType
    {
        [Display(Name = "Успешно")]
        Success = 1,

        [Display(Name = "Ошибка при парсинге строки")]
        ErrorParsed = 2,
    }
}