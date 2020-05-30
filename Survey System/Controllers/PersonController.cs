using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Survey_System.Models;
using Survey_System.Utils;

namespace Survey_System.Controllers
{
    public class PersonController : BaseController
    {
       // SurveyEntities db = new SurveyEntities();  bunu artık BaseControllera taşıyabiliriz.
        public ActionResult Index()
        {
            if(Session["Admin"] == null)
            {
                return RedirectToAction("SignIn", "Login");
            }
            else
            {
                var model = db.Person.ToList();
                return View(model);
            }          
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Person person , string Answer_1)
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("SignIn", "Login");
            }

            if (!ModelState.IsValid)
            {
                return View("Create");
            }

            if (person.NameSurname != null)
            {
                person.CreateBy = NameSurname;
                person.CreateDate = DateTime.Now;
                if(Answer_1 == Constants.AnswerType.Yes)
                {
                    person.IsAdmin = true;
                }
                else
                {
                    person.IsAdmin = false;
                }
                db.Person.Add(person);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Person person = db.Person.Find(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            return View(person);
        }

        [HttpPost]
        public ActionResult Edit(Person person, string Answer_1)
        {

            //var model = db.Person.Find(person.Id);
            //model.NameSurname = person.NameSurname;
            //model.Code = person.Code;
            //model.Password = person.Password;
            //model.CreateBy = person.CreateBy;
            //model.CreateDate = person.CreateDate;
            db.Entry(person).State = System.Data.Entity.EntityState.Modified;
            db.Entry(person).Property(e => e.CreateBy).IsModified = false;
            db.Entry(person).Property(e => e.CreateDate).IsModified = false;
            person.ModifyBy = NameSurname;
            person.ModifyDate = DateTime.Now;
            if (Answer_1 == Constants.AnswerType.Yes)
            {
                person.IsAdmin = true;
            }
            else
            {
                person.IsAdmin = false;
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return HttpNotFound();
            }
            var person = db.Person.Find(Id);
            db.Person.Remove(person);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
     

    }
}