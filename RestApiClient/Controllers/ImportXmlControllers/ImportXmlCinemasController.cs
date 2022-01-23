using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using RestApiClient.Models;

namespace RestApiClient.Controllers
{
    [Authorize]
    public class ImportXmlCinemasController : Controller
    {
        //private readonly string _key = "123456Qq";
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            var model = new ImportXmlCinemaViewModel();
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult Import(ImportXmlCinemaViewModel model)
        {
            if (model.Key != GetKey())
               ModelState.AddModelError("Key", "Ключ для создания/изменения записи указан не верно");
            if (!ModelState.IsValid)
                return View("Index", model);
            
            var file = new byte[model.FileToImport.InputStream.Length];
            model.FileToImport.InputStream.Read(file, 0, (int)model.FileToImport.InputStream.Length);

            XmlSerializer xml = new XmlSerializer(typeof(List<XmlCinema>));
            var clients = (List<XmlCinema>)xml.Deserialize(new MemoryStream(file));
            var db = new KinoAfishaContext();

            foreach (var client in clients)
            {
                db.Cinemas.Add(new Cinema()
                {
                   
                    CinemaPlace = client.CinemaPlace,
                    NumberOfBilets = client.NumberOfBilets,
                    QRcode = client.QRcode,
                    Key = GetKey()
                }) ;

                db.SaveChanges();
            }

            return RedirectPermanent("/Cinemas/Index");
        }

        public ActionResult GetExample()
        {
            return File("~/Content/Files/ImportXmlCinemasExample.xml", "application/xml", "ImportXmlCinemasExample.xml");
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