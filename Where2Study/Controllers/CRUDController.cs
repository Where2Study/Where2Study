using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Where2Study.Models;
using System.Web.Script.Serialization;
using System.Threading;
using System.Collections.Specialized;

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

        [Authorize, AcceptVerbs(HttpVerbs.Get)]
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
        //public ActionResult Create(kontinent_tekst continent, drzava_tekst country_text, grad_tekst city_text, fakultet faculty, fakultet_tekst faculty_text, sveuciliste university, sveuciliste_tekst university_text)
        public ActionResult Index(Faculty facultyEntry)
        //public ActionResult Create(Faculty facultyEntry)
        {
            if (ModelState.IsValid)
            {
                //NameValueCollection nvc = Request.Form;
                var db = new w2sDataContext(); 
                var currentLanguage = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
                var clId=0;
                kontinent_tekst continent = new kontinent_tekst();
                continent.tekst = facultyEntry.Continent;

                drzava country = new drzava();
                drzava_tekst country_text = new drzava_tekst();
                country_text.naziv = facultyEntry.Country;
                country_text.opis = "";

                grad city = new grad();
                grad_tekst city_text = new grad_tekst();
                city_text.naziv = facultyEntry.City;
                city_text.opis = "";

                sveuciliste university = new sveuciliste();
                university.adresa_sveucilista = ""; 
                university.broj_telefona = null;
                university.web = "";                           

                sveuciliste_tekst university_text = new sveuciliste_tekst();
                university_text.naziv = facultyEntry.University;
                university_text.opis = "";

                fakultet faculty = new fakultet();
                faculty.adresa_fakulteta = facultyEntry.Address;
                faculty.broj_telefona = facultyEntry.Phone;
                faculty.web = facultyEntry.WebSite;
                faculty.slika = facultyEntry.Photo;
                
                fakultet_tekst faculty_text = new fakultet_tekst();
                faculty_text.naziv = facultyEntry.Title;
                faculty_text.opis = facultyEntry.Description;


                //sveuciliste university = new sveuciliste();
                //fakultet faculty = new fakultet();

                var languages = from j in db.jeziks select j;
                foreach(var item in languages) if(currentLanguage == item.kratica) clId=item.id;    
                bool countryBool=false, cityBool=false, universityBool=false, facultyBool=false;
                var countries = from d in db.drzavas
                            from dt in db.drzava_teksts
                            where d.id == dt.id_drzava
                            select dt;
                foreach(var item in countries)
                {
                    if (country_text.naziv == item.naziv)
                        {
                            countryBool = true;
                            country.id = item.id;
                            break;
                        }
                }
                if (countryBool==false)
                    try
                    {
                        var continents = from kt in db.kontinent_teksts
                                         from k in db.kontinents
                                         where k.id==kt.id_kontinent && kt.id_jezik==clId
                                         select kt;
                        foreach(var item in continents) if(continent.tekst == item.tekst) country.id_kontinent=item.id_kontinent;
                        UpdateModel(country);
                        repository.Add(country);
                        repository.Save();

                        country_text.id_drzava = country.id;
                        country_text.id_jezik = clId;
                        country_text.opis = "";
                        UpdateModel(country_text);
                        repository.Add(country_text);
                        repository.Save();
                        //return RedirectToAction("Details", new { id = country_text.id });
                    }
                    catch
                    {
                        ModelState.AddRuleViolations(country_text.GetRuleViolations());
                    }

                var cities = from g in db.grads
                             from gt in db.grad_teksts
                             where g.id == gt.id_grad
                             select gt;
                foreach(var item in cities)
                {
                    if (city_text.naziv == item.naziv)
                        {
                            cityBool = true;
                            city.id = item.id;
                            break;
                        }
                }
                if (cityBool==false)
                    try
                    {
                        city.id_drzava=country.id;                    
                        UpdateModel(city);
                        repository.Add(city);
                        repository.Save();

                        city_text.id_grad = city.id;
                        city_text.id_jezik = clId;
                        city_text.opis = "";
                        UpdateModel(city_text);
                        repository.Add(city_text);
                        repository.Save();
                    }
                    catch
                    {
                        ModelState.AddRuleViolations(city_text.GetRuleViolations());
                    }


                var universities = from s in db.sveucilistes
                                   from st in db.sveuciliste_teksts
                                   where s.id == st.id_sveuciliste
                                   select st;
                foreach(var item in universities)
                {
                    if (university_text.naziv == item.naziv) 
                    {
                        universityBool = true;
                        university.id = item.id;
                        break;
                    }

                }
                if (universityBool==false)
                    try
                    {
                        university.id_grad=city.id;
                        university.adresa_sveucilista="";
                        university.broj_telefona="";
                        university.web="";
                        UpdateModel(university);
                        repository.Add(university);
                        repository.Save();

                        university_text.id_sveuciliste = university.id;
                        university_text.id_jezik = clId;
                        university_text.opis = "";
                        UpdateModel(university_text);
                        repository.Add(university_text);
                        repository.Save();
                    }
                    catch
                    {
                        ModelState.AddRuleViolations(university_text.GetRuleViolations());
                    }

                var faculties = from f in db.fakultets
                                from ft in db.fakultet_teksts
                                where f.id == ft.id_fakultet
                                select ft;
                foreach(var item in faculties)
                {
                if (faculty_text.naziv == item.naziv) facultyBool = true;
                }
                if (facultyBool==false)
                    try
                    {
                        faculty.id_grad = city.id;
                        
                        faculty.id_sveuciliste = university.id;
                        faculty.slika = "";
                        UpdateModel(faculty);
                        repository.Add(faculty);
                        repository.Save();

                        faculty_text.id_fakultet = faculty.id;
                        faculty_text.id_jezik = clId;
                        UpdateModel(faculty_text);
                        repository.Add(faculty_text);
                        repository.Save();
                        return RedirectToAction("Details", "Menu" ,new { id = faculty.id });
                   }
                   catch
                   {
                        ModelState.AddRuleViolations(faculty_text.GetRuleViolations());
                   }
               }
            return View(facultyEntry);
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
