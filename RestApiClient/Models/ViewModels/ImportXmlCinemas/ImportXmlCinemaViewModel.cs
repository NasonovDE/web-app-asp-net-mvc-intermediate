using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RestApiClient.Models
{
    public class ImportXmlCinemaViewModel
    {
        /// <summary>
        /// Id
        /// </summary> 
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }


        [Display(Name = "Файл импорта xml", Order = 20)]
        [Required(ErrorMessage = "Укажите файл импорта (.xml)")]
        public HttpPostedFileBase FileToImport { get; set; }
        [Display(Name = "Пароль для добавления", Order = 10)]
        [Required]
        public string Key { get; set; }
    }
}