using Survey_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Survey_System.Utils
{
    //bu method controllerlarda çalışacak ve doğrulanmamışsa giriş yapmasına izin verilmeyecek.
    //Diğer controllera değil direk login ekranına yönlendirmeyi sağlar.
  
    public class BaseController : Controller
    {
        public SurveyEntities db = new SurveyEntities();
        public string UserCode { get; set; }
        public string NameSurname { get; set; }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["Code"] == null)
            {
                filterContext.Result = new RedirectResult("/Login/SignIn");
            }
            else
            {
                UserCode = Session["Code"].ToString(); //diğer sayfalarda sürekli ToString() yapmaya gerek kalmayacak.
                NameSurname = Session["NameSurname"].ToString();
            }
            base.OnActionExecuting(filterContext);
        }
    }
}