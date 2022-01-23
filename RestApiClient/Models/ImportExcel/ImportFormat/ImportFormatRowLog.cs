
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using System.Web.Mvc;

namespace RestApiClient.Models
{
    public class ImportFormatRowLog
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public ImportFormatRowLogType Type { get; set; }
    }
}
//E:\СОХРАНИТЬ\КнАГУ\Кожин\ИТ\web-app-asp-net-mvc-intermediate\web-app-asp-net-mvc-intermediate\RestApiClient\Content\Files