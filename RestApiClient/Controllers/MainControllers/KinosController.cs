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

    public class KinosController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            KinoAfishaContext db = new KinoAfishaContext();
            return View(db.Set<Kino>());
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public ActionResult Create()
        {
            var kino = new Kino();
            return View(kino);

        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult Create(Kino model)
        {
            var db = new KinoAfishaContext();
            if (!ModelState.IsValid)
            {

                var formats = db.Set<Kino>();
                ViewBag.Create = model;
                return View("Index", formats);
            }
            if (model.Key != GetKey())
                ModelState.AddModelError("Key", "Ключ для создания/изменения записи указан не верно");





            if (model.FilmIds != null && model.FilmIds.Any())
            {
                var film = db.Films.Where(s => model.FilmIds.Contains(s.Id)).ToList();
                model.Films = film;
            }
            if (model.CinemaIds != null && model.CinemaIds.Any())
            {
                var cinema = db.Cinemas.Where(s => model.CinemaIds.Contains(s.Id)).ToList();
                model.Cinemas = cinema;
            }



            db.Kinos.Add(model);
            db.SaveChanges();


            return RedirectPermanent("/Kinos/Index");
        }


        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int id)
        {
            var db = new KinoAfishaContext();
            var kino = db.Kinos.FirstOrDefault(x => x.Id == id);
            if (kino == null)
                return RedirectPermanent("/Kinos/Index");

            db.Kinos.Remove(kino);
            db.SaveChanges();

            return RedirectPermanent("/Kinos/Index");
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(int id)
        {
            var db = new KinoAfishaContext();
            var kino = db.Kinos.FirstOrDefault(x => x.Id == id);

            if (kino == null)
                return RedirectPermanent("/Kinos/Index");

            return View(kino);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(Kino model)
        {

            var db = new KinoAfishaContext();

            if (!ModelState.IsValid)
            {

                var formats = db.Set<Kino>();
                ViewBag.Create = model;
                return View("Index", formats);
            }
            var kino = db.Kinos.FirstOrDefault(x => x.Id == model.Id);



            if (kino == null)
            {
                ModelState.AddModelError("Id", "кино не найдено");
            }
            if (model.Key != GetKey())
                ModelState.AddModelError("Key", "Ключ для создания/изменения записи указан не верно");

            if (!ModelState.IsValid)
                return View(model);

            MappingKino(model, kino, db);

            db.Entry(kino).State = EntityState.Modified;
            db.SaveChanges();


            return RedirectPermanent("/Kinos/Index");
        }


        private void MappingKino(Kino sourse, Kino destination, KinoAfishaContext db)
        {

            destination.Price = sourse.Price;
            destination.FilmIds = sourse.FilmIds;
            destination.NextArrivalDate = sourse.NextArrivalDate;
            destination.KinoTime = sourse.KinoTime;
            destination.Key = sourse.Key;


            if (destination.Films != null)
                destination.Films.Clear();

            if (sourse.FilmIds != null && sourse.FilmIds.Any())
                destination.Films = db.Films.Where(s => sourse.FilmIds.Contains(s.Id)).ToList();

            if (destination.Cinemas != null)
                destination.Cinemas.Clear();

            if (sourse.CinemaIds != null && sourse.CinemaIds.Any())
                destination.Cinemas = db.Cinemas.Where(s => sourse.CinemaIds.Contains(s.Id)).ToList();


        }

        [HttpGet]
        public ActionResult GetXlsx()
        {
            var db = new KinoAfishaContext();
            var values = db.Kinos.ToList();

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Data");


            ws.Cell("A" + 1).Value = "Id";
            ws.Cell("B" + 1).Value = "Название фильма";
            ws.Cell("C" + 1).Value = "Место показа";
            ws.Cell("D" + 1).Value = "Дата сеанса";
            ws.Cell("E" + 1).Value = "Время сеанса";
            ws.Cell("F" + 1).Value = "Цена";


            int row = 2;
            foreach (var value in values)
            {
                ws.Cell("A" + row).Value = value.Id;
                ws.Cell("B" + row).Value = string.Join(", ", value.Films.Select(y => $"{y.NameFilm}"));
                ws.Cell("C" + row).Value = string.Join(", ", value.Cinemas.Select(y => $"{y.CinemaPlace}"));
                ws.Cell("D" + row).Value = value.NextArrivalDate;
                ws.Cell("E" + row).Value = value.KinoTime.Value.ToString("HH:mm");
                ws.Cell("F" + row).Value = value.Price;
                
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
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Kinos.xlsx");
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
            var clients = db.Kinos.ToList().Select(x => new XmlKino()
            {
                Id = x.Id,
                //Films = x.Films.Select(y => new XmlFilm() { NameFilm = y.NameFilm, Id = y.Id }).ToList(),
                Price = x.Price,
                //Cinemas = x.Cinemas.Select(y => new XmlCinema() { CinemaPlace = y.CinemaPlace, Id = y.Id }).ToList(),
                NextArrivalDate = x.NextArrivalDate,
                KinoTime = x.KinoTime,
               


            }).ToList();

            XmlSerializer xml = new XmlSerializer(typeof(List<XmlKino>));
            var ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var ms = new MemoryStream();
            xml.Serialize(ms, clients, ns);
            ms.Position = 0;

            return File(new MemoryStream(ms.ToArray()), "application/xml");
        }
    }
}
