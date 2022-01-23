
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using System.Web.Mvc;

namespace RestApiClient.Models
{
    public class ImportFormatViewModel
    {
        /// <summary>
        /// Id
        /// </summary> 
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }


        [Display(Name = "Файл импорта Excel")]
        [Required(ErrorMessage = "Укажите файл импорта (.xlsx)")]
        public HttpPostedFileBase FileToImport { get; set; }

        [Display(Name = "Пароль для добавления", Order = 10)]
        [Required]
        public string Key { get; set; }
    }
}