﻿
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using System.Web.Mvc;

namespace RestApiClient.Models
{
    public class ImportFormatLog
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Display(Name = "Импорт начат в")]
        [UIHint("TextReadOnly")]
        public DateTime StartImport { get; set; }

        [Display(Name = "Импорт закончен в")]
        [UIHint("TextReadOnly")]
        public DateTime EndImport { get; set; }


        [Display(Name = "Распознанных строк")]
        [UIHint("TextReadOnly")]
        public int SuccessCount { get; set; }

        [Display(Name = "Не распознанных строк")]
        [UIHint("TextReadOnly")]
        public int FailedCount { get; set; }

        [Display(Name = "Отчет")]
        [UIHint("Logs")]
        public List<ImportFormatRowLog> Logs { get; set; }
    }
}