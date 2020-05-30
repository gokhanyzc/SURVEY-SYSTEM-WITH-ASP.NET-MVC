using Survey_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Survey_System.Controllers;
using Survey_System.Utils;

namespace Survey_System.Controllers
{
    public class QuestionController : BaseController
    {

       // SurveyEntities db = new SurveyEntities();
        public ActionResult Index()
        {
            if(Session["Admin"] == null)
            {
                return RedirectToAction("SignIn", "Login");
            }
            else
            {
                var model = db.Question.ToList();
                return View(model);
            }        
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Question question)
        {

            if (Session["Admin"] == null)
            {
                return RedirectToAction("SignIn", "Login");
            }


            if (question.QuestionLine != null)
            {
                question.CreateBy = NameSurname;
                question.CreateDate = DateTime.Now;
                db.Question.Add(question);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }
                               //BURASIDA ÇALIŞIYOR
        //[HttpPost]

        //public ActionResult Create([Bind(Include = "Id,QuestionLine")] Question question)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        question.CreateBy = "System";
          //      question.CreateDate = DateTime.Now;
        //        db.Question.Add(question);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(question);
        //}


        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Question question = db.Question.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        [HttpPost]
        public ActionResult Edit(Question question)
        {
            db.Entry(question).State = System.Data.Entity.EntityState.Modified;
            db.Entry(question).Property(e => e.CreateBy).IsModified = false;
            db.Entry(question).Property(e => e.CreateDate).IsModified = false;
            question.ModifyBy = NameSurname;
            question.ModifyDate = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return HttpNotFound();
            }
            var question = db.Question.Find(Id);
            db.Question.Remove(question);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}