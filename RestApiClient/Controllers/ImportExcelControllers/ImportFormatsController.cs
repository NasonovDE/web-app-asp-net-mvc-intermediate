using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

using RestApiClient.Models;

namespace KinoAfisha.Controllers
{
    public class ImportFormatsController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var model = new ImportFormatViewModel();
           
            return View(model);
        }

        [HttpPost]
        public ActionResult Import(ImportFormatViewModel model)
        {
            if (model.Key != GetKey())
                ModelState.AddModelError("Key", "Ключ для создания/изменения записи указан не верно");
            if (!ModelState.IsValid)
                return View("Index", model);

            var log = ProceedImport(model);

            return View("Log", log);
        }

        public ActionResult GetExample()
        {
            return File("~/Content/Files/ImportFormatsExample.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ImportFormatsExample.xlsx");
        }

        private ImportFormatLog ProceedImport(ImportFormatViewModel model)
        {
            var startTime = DateTime.Now;

            var workBook = new XLWorkbook(model.FileToImport.InputStream);
            var workSheet = workBook.Worksheet(1);
            var rows = workSheet.RowsUsed().Skip(1).ToList();

            var logs = new List<ImportFormatRowLog>();
            var data = ParseRows(rows, logs);
            ApplyImported(data);

            var successCount = data.Count();
            var failedCount = rows.Count() - successCount;
            var finishTime = DateTime.Now;

            var result = new ImportFormatLog()
            {
                StartImport = startTime,
                EndImport = finishTime,
                SuccessCount = successCount,
                FailedCount = failedCount,
                Logs = logs
            };

            return result;
        }

        private List<ImportFormatData> ParseRows(IEnumerable<IXLRow> rows, List<ImportFormatRowLog> logs)
        {
            var result = new List<ImportFormatData>();
            int index = 1;
            foreach (var row in rows)
            {
                try
                {
                    var data = new ImportFormatData()
                    {
                        Name = ConvertToString(row.Cell("A").GetValue<string>().Trim()),
                       

                    };

                    result.Add(data);
                    logs.Add(new ImportFormatRowLog()
                    {
                        Id = index,
                        Message = $"ОК",
                        Type = ImportFormatRowLogType.Success
                    }); ;

                }
                catch (Exception ex)
                {
                    logs.Add(new ImportFormatRowLog()
                    {
                        Id = index,
                        Message = $"Error: {ex.GetBaseException().Message}",
                        Type = ImportFormatRowLogType.ErrorParsed
                    }); ;
                }

                index++;
            }


            return result;
        }

        private void ApplyImported(List<ImportFormatData> data)
        {
            var db = new KinoAfishaContext();

            foreach (var value in data)
            {
                var model = new Format()
                {
                    Name = value.Name,

                    Key = GetKey()


                };

                db.Formats.Add(model);
                db.SaveChanges();
            }
        }

        private string ConvertToString(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new Exception("Значение не определено");

            var result = HandleInjection(value);

            return result;
        }
        private string HandleInjection(string value)
        {
            var badSymbols = new Regex(@"^[+=@-].*");
            return Regex.IsMatch(value, badSymbols.ToString()) ? string.Empty : value;
        }

        private DateTime? ConvertToDateTime(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            DateTime result = default;

            if (DateTime.TryParse(value, out DateTime temp))
                result = temp;

            if (result == default)
                return null;

            return result;
        }
        private string GetKey()
        {
            var db = new KinoAfishaContext();
            var setting = db.Settings.FirstOrDefault(x => x.Type == SettingType.Password);
            if (setting == null)
                throw new Exception("Setting not found");

            return setting.Value;
        }


    }
}