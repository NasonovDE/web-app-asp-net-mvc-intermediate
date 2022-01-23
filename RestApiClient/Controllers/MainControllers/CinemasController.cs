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
    public class CinemasController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            KinoAfishaContext db = new KinoAfishaContext();
            return View(db.Set<Cinema>());
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public ActionResult Create()
        {
            var cinema = new Cinema();
            return View(cinema);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult Create(Cinema model)
        {
          
            var db = new KinoAfishaContext();
            if (!ModelState.IsValid)
            {

                var formats = db.Set<Cinema>();
                ViewBag.Create = model;
                return View("Index", formats);
            }
            if (model.Key != GetKey())
                ModelState.AddModelError("Key", "Ключ для создания/изменения записи указан не верно");

            db.Cinemas.Add(model);
            db.SaveChanges();

            return RedirectPermanent("/Cinemas/Index");
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int id)
        {
            var db = new KinoAfishaContext();
            var cinema = db.Cinemas.FirstOrDefault(x => x.Id == id);
            if (cinema == null)
                return RedirectPermanent("/Cinemas/Index");

            db.Cinemas.Remove(cinema);
            db.SaveChanges();

            return RedirectPermanent("/Cinemas/Index");
        }


        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(int id)
        {
            var db = new KinoAfishaContext();
            var cinema = db.Cinemas.FirstOrDefault(x => x.Id == id);
            if (cinema == null)
                return RedirectPermanent("/Cinemas/Index");

            return View(cinema);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(Cinema model)
        {
            var db = new KinoAfishaContext();
            if (!ModelState.IsValid)
            {

                var formats = db.Set<Cinema>();
                ViewBag.Create = model;
                return View("Index", formats);
            }
            var cinema = db.Cinemas.FirstOrDefault(x => x.Id == model.Id);
            if (cinema == null)
                ModelState.AddModelError("Id", "Кинотеатр не найден");
            if (model.Key != GetKey())
                ModelState.AddModelError("Key", "Ключ для создания/изменения записи указан не верно");

            if (!ModelState.IsValid)
                return View(model);

            MappingFilm(model, cinema, db);

            db.Entry(cinema).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectPermanent("/Cinemas/Index");
        }

        private void MappingFilm(Cinema sourse, Cinema destination, KinoAfishaContext db)
        {

            destination.CinemaPlace = sourse.CinemaPlace;
            destination.NumberOfBilets = sourse.NumberOfBilets;
            destination.QRcode = sourse.QRcode;
            destination.Key = sourse.Key;

        }
        [HttpGet]
        public ActionResult GetXlsx()
        {
            var db = new KinoAfishaContext();
            var values = db.Cinemas.ToList();

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Data");


            ws.Cell("A" + 1).Value = "Id";
            ws.Cell("B" + 1).Value = "Название кинотеатра";
            ws.Cell("C" + 1).Value = "Количество мест";
            ws.Cell("D" + 1).Value = "Требование QR кода";

            int row = 2;
            foreach (var value in values)
            {
                ws.Cell("A" + row).Value = value.Id;
                ws.Cell("B" + row).Value = value.CinemaPlace;
                ws.Cell("C" + row).Value = value.NumberOfBilets;
                ws.Cell("D" + row).Value = value.QRcode;

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
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Cinemas.xlsx");
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
            var clients = db.Cinemas.ToList().Select(x => new XmlCinema()
            {
                Id = x.Id,
                CinemaPlace = x.CinemaPlace,
                NumberOfBilets = x.NumberOfBilets,
                QRcode = x.QRcode
                

            }).ToList();

            XmlSerializer xml = new XmlSerializer(typeof(List<XmlCinema>));
            var ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var ms = new MemoryStream();
            xml.Serialize(ms, clients, ns);
            ms.Position = 0;

            return File(new MemoryStream(ms.ToArray()), "application/xml");
        }

    }
}