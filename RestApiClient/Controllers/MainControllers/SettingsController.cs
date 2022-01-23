using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using RestApiClient.Models;

namespace RestApiClient.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly string _key = "123456Qq";

        
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            var db = new KinoAfishaContext();
            var settings = db.Settings.ToList();

            return View(settings);
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public ActionResult Create()
        {
            var setting = new Setting();
            return View(setting);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult Create(Setting model)
        {
            var db = new KinoAfishaContext();
            if (model.Key != _key)
                ModelState.AddModelError("Key", "Ключ для создания/изменения записи указан не верно");
            var setting = db.Settings.FirstOrDefault(x => x.Type == model.Type);
            if (setting != null)
                ModelState.AddModelError("Value", "Данная настройка уже задана");

            if (!ModelState.IsValid)
            {
                var settings = db.Settings.ToList();
                ViewBag.Create = model;
                return View("Index", settings);
            }



            db.Settings.Add(model);
            db.SaveChanges();

            return RedirectPermanent("/Settings/Index");
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int id)
        {
            var db = new KinoAfishaContext();
            var setting = db.Settings.FirstOrDefault(x => x.Id == id);
            if (setting == null)
                return RedirectPermanent("/Settings/Index");

            db.Settings.Remove(setting);
            db.SaveChanges();

            return RedirectPermanent("/Settings/Index");
        }


        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(int id)
        {
            var db = new KinoAfishaContext();
            var setting = db.Settings.FirstOrDefault(x => x.Id == id);
            if (setting == null)
                return RedirectPermanent("/Settings/Index");

            return View(setting);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(Setting model)
        {
            var db = new KinoAfishaContext();
            if (model.Key != _key)
                ModelState.AddModelError("Key", "Ключ для создания/изменения записи указан не верно");
            var setting = db.Settings.FirstOrDefault(x => x.Id == model.Id);
            if (setting == null)
                ModelState.AddModelError("Id", "Настройка не найдена");

            if (!ModelState.IsValid)
            {
                var settings = db.Settings.ToList();
                ViewBag.Create = model;
                return View("Index", settings);
            }

            MappingSetting(model, setting);

            db.Entry(setting).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectPermanent("/Settings/Index");
        }

        private void MappingSetting(Setting sourse, Setting destination)
        {
            destination.Key = sourse.Key;
            destination.Type = sourse.Type;
            destination.Value = sourse.Value;
            
        }
    }
}