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
    public class FilmsController : Controller
    {

        [HttpGet]
        public ActionResult Index()
        {
            KinoAfishaContext db = new KinoAfishaContext();
            return View(db.Set<Film>());
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]

        public ActionResult Create()
        {
            var film = new Film();
            return View(film);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]

        public ActionResult Create(Film model)
        {
            var db = new KinoAfishaContext();
            if (!ModelState.IsValid)
            {

                var formats = db.Set<Film>();
                ViewBag.Create = model;
                return View("Index", formats);
            }
            if (model.Key != GetKey())
                ModelState.AddModelError("Key", "Ключ для создания/изменения записи указан не верно");

            if (model.FormatIds != null && model.FormatIds.Any())
            {
                var nationality = db.Formats.Where(s => model.FormatIds.Contains(s.Id)).ToList();
                model.Formats = nationality;
            }

            if (model.FilmCoverFile != null)
            {
                var data = new byte[model.FilmCoverFile.ContentLength];
                model.FilmCoverFile.InputStream.Read(data, 0, model.FilmCoverFile.ContentLength);

                model.FilmCover = new FilmCover()
                {
                    Guid = Guid.NewGuid(),
                    DateChanged = DateTime.Now,
                    Data = data,
                    ContentType = model.FilmCoverFile.ContentType,
                    FileName = model.FilmCoverFile.FileName
                };
            }





            db.Films.Add(model);
            db.SaveChanges();

            return RedirectPermanent("/Films/Index");
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]

        public ActionResult Delete(int id)
        {
            var db = new KinoAfishaContext();
            var film = db.Films.FirstOrDefault(x => x.Id == id);
            if (film == null)
                return RedirectPermanent("/Films/Index");

            db.Films.Remove(film);
            db.SaveChanges();

            return RedirectPermanent("/Films/Index");
        }


        [HttpGet]
        [Authorize(Roles = "Administrator")]

        public ActionResult Edit(int id)
        {
            var db = new KinoAfishaContext();
            var film = db.Films.FirstOrDefault(x => x.Id == id);
            if (film == null)
                return RedirectPermanent("/Films/Index");

            return View(film);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]

        public ActionResult Edit(Film model)
        {
            var db = new KinoAfishaContext();
            if (!ModelState.IsValid)
            {

                var formats = db.Set<Film>();
                ViewBag.Create = model;
                return View("Index", formats);
            }
            var film = db.Films.FirstOrDefault(x => x.Id == model.Id);
            if (film == null)
                ModelState.AddModelError("Id", "Фильм не найден");
            if (model.Key != GetKey())
                ModelState.AddModelError("Key", "Ключ для создания/изменения записи указан не верно");




            if (!ModelState.IsValid)
                return View(model);

            MappingFilm(model, film, db);

            db.Entry(film).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectPermanent("/Films/Index");
        }

        private void MappingFilm(Film sourse, Film destination, KinoAfishaContext db)
        {

            destination.NameFilm = sourse.NameFilm;
            destination.FilmYears = sourse.FilmYears;
            destination.FilmAllActors = sourse.FilmAllActors;
            destination.FilmDescription = sourse.FilmDescription;
            destination.FilmDop = sourse.FilmDop;
            destination.Key = sourse.Key;

            if (destination.Formats != null)
                destination.Formats.Clear();

            if (sourse.FormatIds != null && sourse.FormatIds.Any())
                destination.Formats = db.Formats.Where(s => sourse.FormatIds.Contains(s.Id)).ToList();


            if (sourse.FilmCoverFile != null)
            {
                var image = db.FilmCovers.FirstOrDefault(x => x.Id == sourse.Id);
                if (image != null)
                    db.FilmCovers.Remove(image);

                var data = new byte[sourse.FilmCoverFile.ContentLength];
                sourse.FilmCoverFile.InputStream.Read(data, 0, sourse.FilmCoverFile.ContentLength);

                destination.FilmCover = new FilmCover()
                {
                    Guid = Guid.NewGuid(),
                    DateChanged = DateTime.Now,
                    Data = data,
                    ContentType = sourse.FilmCoverFile.ContentType,
                    FileName = sourse.FilmCoverFile.FileName
                };
            }
        }
        [HttpGet]
        public ActionResult GetImage(int id)
        {
            var db = new KinoAfishaContext();
            var image = db.FilmCovers.FirstOrDefault(x => x.Id == id);
            if (image == null)
            {
                FileStream fs = System.IO.File.OpenRead(Server.MapPath(@"~/Content/Images/not-foto.png"));
                byte[] fileData = new byte[fs.Length];
                fs.Read(fileData, 0, (int)fs.Length);
                fs.Close();

                return File(new MemoryStream(fileData), "image/jpeg");
            }

            return File(new MemoryStream(image.Data), image.ContentType);
        }

        [HttpGet]
       
        public ActionResult Pdf(int id)
        {
            var db = new KinoAfishaContext();
            var client = db.Films.FirstOrDefault(x => x.Id == id);
            if (client == null)
                return RedirectPermanent("/Film/Index");

            var pdf = new ViewAsPdf("Pdf", client);
            var data = pdf.BuildFile(this.ControllerContext);


            return File(new MemoryStream(data), "application/pdf", "detail.pdf");
        }

        [HttpGet]
        
        public ActionResult Detail(int id)
        {
            var db = new KinoAfishaContext();
            var client = db.Films.FirstOrDefault(x => x.Id == id);
            if (client == null)
                return RedirectPermanent("/Films/Index");

            return View(client);

        }

        [HttpGet]
        public ActionResult GetXlsx()
        {
            var db = new KinoAfishaContext();
            var values = db.Films.ToList();

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Data");


            ws.Cell("A" + 1).Value = "Id";
            ws.Cell("B" + 1).Value = "Название фильма";
            ws.Cell("C" + 1).Value = "Возрастное ограничение";
           

            int row = 2;
            foreach (var value in values)
            {
                ws.Cell("A" + row).Value = value.Id;
                ws.Cell("B" + row).Value = value.NameFilm;
                ws.Cell("C" + row).Value = value.FilmYears;
               

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
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Films.xlsx");
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
            var clients = db.Films.ToList().Select(x => new XmlFilm()
            {
                Id = x.Id,
                NameFilm = x.NameFilm,
                FilmYears = x.FilmYears,
                XmlFormats = x.Formats.Select(y => new XmlFormat() { Name = y.Name, Id = y.Id }).ToList(),
                FilmCover = x.FilmCover == null ? null : new Models.XmlFilmCover()
                {
                    ContentType = x.FilmCover.ContentType,
                    FileName = x.FilmCover.FileName,
                    Data = Convert.ToBase64String(x.FilmCover.Data)
                }
            }).ToList();

            XmlSerializer xml = new XmlSerializer(typeof(List<XmlFilm>));
            var ns = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var ms = new MemoryStream();
            xml.Serialize(ms, clients, ns);
            ms.Position = 0;

            return File(new MemoryStream(ms.ToArray()), "application/xml");
        }



    }
}