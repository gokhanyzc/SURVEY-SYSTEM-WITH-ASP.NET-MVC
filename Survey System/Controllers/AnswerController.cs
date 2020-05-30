using Survey_System.Models;
using Survey_System.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace Survey_System.Controllers
{
    public class AnswerController : BaseController
    {
        
        public ActionResult Index()
        {
            var model = db.Answer.Where(i => i.UserCode == UserCode).ToList();  //herkes kendi cevapladıgı anketleri görecek.
            return View(model);
        }
     
        public ActionResult Create(string Code)
        {
            if(Code == null)
            {
                List<SelectListItem> personList = (from person in db.Person
                                                   where person.Code != UserCode  //bu satır sayesinde giriş yapan anketi düzenleyen kişidir.
                                                   select new SelectListItem
                                                   {
                                                       Text = person.NameSurname,
                                                       Value = person.Code.ToString()
                                                   }).ToList();
                ViewBag.Person = new SelectList(personList.OrderBy(m => m.Text), "Value", "Text"); //text'e göre sıralama yapacak.

                var questionModel = db.Question.ToList(); //soruları modelle create sayfasına göndermiş oluruz.
                return View(questionModel);
            }
            else
            {
                CalculateScore(Code);
                return RedirectToAction("Index");
            }
       
        }
        public void CalculateScore(string code)
        {
            double answeryes = 0, answerno = 0 ,scoreresult=0;
            var answer = db.Answer.FirstOrDefault(m => m.PersonCode == code && m.UserCode == UserCode); //anketimizi aldık.
            var answerLine = db.AnswerLine.Where(m => m.AnswerId == answer.Id).ToList();

            foreach (var item in answerLine)
            {
                if(item.Answer == Constants.AnswerType.Yes)
                {
                    answeryes++;
                }
                else
                {
                    answerno++;
                }
            }
            scoreresult = (answeryes / (answeryes + answerno)) * 100; //yüzdelik işlemi verir.
            if (scoreresult > 79)
            {
                answer.Iscomplete = true;
            }
            else
            {
                answer.Iscomplete = false;
            }
            answer.Score = scoreresult.ToString();
            db.SaveChanges();
        }

        public String SendData(AnswerModel answerModel)
        {

            int? month = DateTime.Now.Month;
            var model = db.Answer.FirstOrDefault(m => m.PersonCode == answerModel.Code && m.UserCode == UserCode && m.CreateDate.Value.Month == month);

            if(model != null)
            {
                SaveAnswerLine(answerModel.Question, answerModel.Answer, model.Id);
            }
            else
            {
                Answer answer = new Answer();
                answer.PersonCode = answerModel.Code;
                answer.PersonName = answerModel.NameSurname;
                answer.UserCode = UserCode;
                answer.CreateDate = DateTime.Now;
                answer.CreateBy = NameSurname;

                db.Answer.Add(answer);
                db.SaveChanges();
               
            }
            return "True";
        }
        public void SaveAnswerLine(string question, string answer,int AnswerId)
        {
            //EVET ise ve daha sonra HAYIR cevabını vermişse.
            var model = db.AnswerLine.FirstOrDefault(m => m.AnswerId == AnswerId && m.Question == question); //aynı ıd ye sahip aynı soru varsa
            if(model != null)
            {
                model.Answer = answer; //ilgili kaydın sadece cevabı değiştirilecek.
                db.SaveChanges();
            }
            else
            {
                AnswerLine answerLine = new AnswerLine();
                answerLine.AnswerId = AnswerId;
                answerLine.Question = question;
                answerLine.Answer = answer;

                db.AnswerLine.Add(answerLine);
                db.SaveChanges();
            }        
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Answer answer_details = db.Answer.Find(id);
            if (answer_details == null)
            {
                return HttpNotFound();
            }
            return View(answer_details);
        }

        public ActionResult Detail(int? Id)
        {
            var model = db.AnswerLine.Where(i => i.AnswerId == Id).ToList();
            return View(model);
        }

        #region
        public ActionResult DownloadFile(string fileName, string dokuman_name, string dokuman_no)
        {

            if (!String.IsNullOrEmpty(fileName))
            {
                string filePath;
                string type = System.IO.Path.GetExtension(fileName);
                string dokuman_download_name = dokuman_name + "-" + dokuman_no;
                var reg = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(Path.GetExtension(fileName).ToLower());

                string contentType = "application/unknown";
                if (reg != null)
                {
                    string registryContentType = reg.GetValue("Content Type") as string;

                    if (!String.IsNullOrWhiteSpace(registryContentType))
                    {
                        contentType = registryContentType;
                    }

                }
                filePath = Path.Combine(Server.MapPath("~/DokumanUploadPath"), fileName);
                return File(filePath, contentType, dokuman_download_name + "." + type);
            }
            else
                return null;
        }
        #endregion

    }
}