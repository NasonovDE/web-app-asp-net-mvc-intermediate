using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace RestApiClient.Models
{
     public class FilmCover
    {


        public int Id { get; set; }
        public Guid Guid { get; set; }
        [Required]
        public byte[] Data { get; set; }

        /// <summary>
        /// Тип файла
        /// </summary>
        [StringLength(100)]
        public string ContentType { get; set; }

        /// <summary>
        /// Дата изменения
        /// </summary>

        public DateTime? DateChanged { get; set; }

        /// <summary>
        /// Имя файла
        /// </summary>


        public string FileName { get; set; }



    }
}