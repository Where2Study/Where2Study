using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Where2Study.Models;
using System.Web.Script.Serialization;
using System.Threading;

namespace Where2Study.Controllers
{
    public class CRUDController : BaseController
    {
        w2sRepository repository = new w2sRepository();

        /* Metode:
         * getContinents()
         * get countries()
         * getCities()
         * getFaculties()
         * 
         * Ove metode primaju varijablu id koja dolazi iz parametara rute (pogledaj Global.asax.cs -> Default route).
         * Za jezik ove metode odabiru jezik na kojem korisnik trenutno pretražuje stranicu.
         */

        public string getContinents()
        {
            SiteLanguages.GetAllLanguages();
            var currentLanguage = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;

            var db = new w2sDataContext();
            var queue = from k in db.kontinent_teksts
                        from j in db.jeziks
                        where j.kratica == currentLanguage && k.id_jezik == j.id
                        select k.tekst;
           /* Where2Study.Models.w2sRepository.GetAllContinents();
            var queue = Where2Study.Models.w2sRepository.AllContinents.ToList();*/
            
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string ret = serializer.Serialize(queue.ToArray());

            return ret;
        }

        public string getCountries(string id)
        {
            if(id == null) return "[]";

            SiteLanguages.GetAllLanguages();
            var currentLanguage = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;

            var db = new w2sDataContext();
            var queue = from kt in db.kontinent_teksts
                        from d in db.drzavas
                        from dt in db.drzava_teksts
                        from j in db.jeziks
                        where kt.tekst == id && d.id_kontinent == kt.id_kontinent && dt.id_jezik == j.id && d.id == dt.id_drzava && j.kratica == currentLanguage
                        select dt.naziv;

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string ret = serializer.Serialize(queue);

            return ret;
        }

        public string getCities(string id)
        {
            if(id == null) return "[]";

            SiteLanguages.GetAllLanguages();
            var currentLanguage = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;

            var db = new w2sDataContext();
            var queue = from dt in db.drzava_teksts
                        from c in db.grads
                        from ct in db.grad_teksts
                        from j in db.jeziks
                        where dt.naziv == id  && j.kratica == currentLanguage && ct.id_jezik == j.id && ct.id_grad == c.id && c.id_drzava == dt.id_drzava
                        select ct.naziv;

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string ret = serializer.Serialize(queue);

            return ret;
        }

        public string getFaculties(string id)
        {
            if (id == null) return "[]";

            SiteLanguages.GetAllLanguages();
            var currentLanguage = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;

            var db = new w2sDataContext();
            var queue = from gt in db.grad_teksts
                        from s in db.sveucilistes
                        from st in db.sveuciliste_teksts
                        from j in db.jeziks
                        where gt.naziv == id && j.kratica == currentLanguage && st.id_jezik == j.id && st.id_sveuciliste == s.id && s.id_grad == gt.id_grad
                        select st.naziv;

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string ret = serializer.Serialize(queue);

            return ret;
        }

        [Authorize]
        public ActionResult Index(fakultet faculty)
        {
            if (ModelState.IsValid)
            {
               /* try
                {
                    UpdateModel(faculty);
                    repository.Add(faculty);
                    repository.Save();
                    return RedirectToAction("Details", new { id = faculty.id });
                }
                catch
                {
                    // ModelState.AddRuleViolations(faculty.GetRuleViolations());
                }*/
            }
            return View();
        }

        public ActionResult Details(int id)
        {

            fakultet_tekst faculty = repository.Get_fakultet_tekst(id);

            /*if (facultyy == null)
                return View("NotFound");
            else*/
            return View(faculty);
        }

        public ActionResult Create()
        {
            fakultet_tekst faculty = new fakultet_tekst();

            return View(faculty);
        }

        [AcceptVerbs(HttpVerbs.Post), ValidateInput(false)]
        public ActionResult Create(fakultet_tekst faculty)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    UpdateModel(faculty);
                    repository.Add(faculty);
                    repository.Save();
                    return RedirectToAction("Details", new { id = faculty.id });
                }
                catch
                {
                     ModelState.AddRuleViolations(faculty.GetRuleViolations());
                }
            }
            return View(faculty);
        }

        public ActionResult Edit(int id)
        {
            fakultet_tekst faculty = repository.Get_fakultet_tekst(id);

            return View(faculty);
        }

        [AcceptVerbs(HttpVerbs.Post), ValidateInput(false)]
        public ActionResult Edit(int id, FormCollection formvalues)
        {
            fakultet_tekst faculty = repository.Get_fakultet_tekst(id);

            /*faculty.naziv = Request.Form["tekst"];
            faculty.id_fakultet = int.Parse(Request.Form["id_fakultet"]);
            faculty.id_jezik = int.Parse(Request.Form["id_jezik"]);*/

            try
            {
                UpdateModel(faculty);
                repository.Save();
                return RedirectToAction("Details", new { id = faculty.id });
            }
            catch
            {
                ModelState.AddRuleViolations(faculty.GetRuleViolations());
                return View(faculty);
            }
        }

        public ActionResult Delete(int id)
        {
            fakultet_tekst faculty = repository.Get_fakultet_tekst(id);

            if (faculty == null)
                return View("NotFound");
            else
                return View(faculty);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete(int id, FormCollection collection)
        {
            fakultet_tekst faculty = repository.Get_fakultet_tekst(id);
            if (faculty == null)
                return View("NotFound");
            repository.Delete(faculty);
            repository.Save();
            {
                return View("Deleted");
            }
        }
    }
}
