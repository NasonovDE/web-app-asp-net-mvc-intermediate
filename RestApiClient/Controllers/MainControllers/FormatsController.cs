using RestApiClient.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Rotativa;
using Common.Extentions;
using System.Xml.Serialization;
using System.Xml;
using ClosedXML.Excel;

namespace RestApiClient.Controllers
{
    [Authorize]
    public class FormatsController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            KinoAfishaContext db = new KinoAfishaContext();
            return View(db.Set<Format>());

        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public ActionResult Create()
        {
            var format = new Format();
            return View(format);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult Create(Format model)
        {


            var db = new KinoAfishaContext();
            if (!ModelState.IsValid)
            {

                var formats = db.Set<Format>();
                ViewBag.Create = model;
                return View("Index", formats);
            }
            if (!ModelState.IsValid)
                return View(model);

            if (model.Key != GetKey())
                ModelState.AddModelError("Key", "Ключ для создания/изменения записи указан не верно");
            
            db.Formats.Add(model);
            db.SaveChanges();

            return RedirectPermanent("/Formats/Index");
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int id)
        {
            var db = new KinoAfishaContext();
            var format = db.Formats.FirstOrDefault(x => x.Id == id);
            if (format == null)
                return RedirectPermanent("/Formats/Index");

            db.Formats.Remove(format);
            db.SaveChanges();

            return RedirectPermanent("/Formats/Index");
        }


        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(int id)
        {
            var db = new KinoAfishaContext();
            var format = db.Formats.FirstOrDefault(x => x.Id == id);
            if (format == null)
                return RedirectPermanent("/Formats/Index");

            return View(format);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(Format model)
        {
            var db = new KinoAfishaContext();
            if (!ModelState.IsValid)
            {

                var formats = db.Set<Format>();
                ViewBag.Create = model;
                return View("Index", formats);
            }

            var format = db.Formats.FirstOrDefault(x => x.Id == model.Id);
            if (model.Key != GetKey())
                ModelState.AddModelError("Key", "Ключ для создания/изменения записи указан не верно");
            if (format == null)
                ModelState.AddModelError("Id", "Формат не найден");
          
            


            MappingNationality(model, format);

            db.Entry(format).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectPermanent("/Formats/Index");
        }

        private void MappingNationality(Format sourse, Format destination)
        {
            destination.Name = sourse.Name;
            destination.Key = sourse.Key;
        }
        [HttpGet]
        public ActionResult GetXlsx()
        {
            var db = new KinoAfishaContext();
            var values = db.Formats.ToList();

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Data");


            ws.Cell("A" + 1).Value = "Id";
            ws.Cell("B" + 1).Value = "Название формата";
           
            int row = 2;
            foreach (var value in values)
            {
                ws.Cell("A" + row).Value = value.Id;
                ws.Cell("B" + row).Value = value.Name;

                row++;
            };
            var rngHead = ws.Range("A1:L" + 1);
            rngHead.Style.Fill.BackgroundColor = XLColor.AshGrey;

            var rngTable = ws.Range("A1:L" + 100);
            rngTable.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            rngTable.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            ws.Columns().AdjustToContents();



            using (MemoryStream stream = new MemoryStream())
            {
                wb.SaveAs(stream);
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Formats.xlsx");
            }



        }
        private string GetKey()
        {
            var db = new KinoAfishaContext();
            var setting = db.Settings.FirstOrDefault(x => x.Type == SettingType.Password);
            if (setting == null)
                throw new Exception("Setting not found");

            return setting.Value;
        }
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public ActionResult GetXml()
        {
            var db = new KinoAfishaContext();
            var clients = db.Formats.ToList().Select(x => new XmlFormat()
            {
                Id = x.Id,
                Name = x.Name,
                
            }).ToList();

            XmlSerializer xml = new XmlSerializer(typeof(List<XmlFormat>));
            var ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var ms = new MemoryStream();
            xml.Serialize(ms, clients, ns);
            ms.Position = 0;

            return File(new MemoryStream(ms.ToArray()), "application/xml");
        }
       
    }
}