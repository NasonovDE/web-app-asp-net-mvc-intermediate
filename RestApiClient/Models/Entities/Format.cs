using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestApiClient.Models
{
    public class Format
    {
        /// <summary>
        /// Id
        /// </summary> 
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>    
        [Required]
        [Display(Name = "Название", Order = 5)]
        public string Name { get; set; }

        /// <summary>
        /// Список форматов
        /// </summary> 
        [ScaffoldColumn(false)]
        public virtual ICollection<Film> Films { get; set; }

        /// <summary>
        /// Ключ для создания/изменения записи
        /// </summary>    
        [Required]
        [Display(Name = "Ключ для создания/изменения записи", Order = 120)]
        [UIHint("Password")]
        [NotMapped]
        public string Key { get; set; }
    }
}