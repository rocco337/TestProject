using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        [BasicAuthentication]
        public ActionResult Auth()
        {
            return View("Index");
        }

        public ActionResult Data(Model model)
        {
            return Json(model.ids, JsonRequestBehavior.AllowGet);
        }
    }

    public class Model
    {
        public int[] ids { get; set; }
    }

    public class BasicAuthenticationAttribute : ActionFilterAttribute
    {
        private string Username = "admin";
        private string Password = "1234";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var req = filterContext.HttpContext.Request;
            var auth = req.Headers["Authorization"];
            if (!String.IsNullOrEmpty(auth))
            {
                var cred = System.Text.ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(auth.Substring(6))).Split(':');
                var user = new { Name = cred[0], Pass = cred[1] };
                if (user.Name == Username && user.Pass == Password) return;
            }
            var res = filterContext.HttpContext.Response;
            res.StatusCode = 401;
            res.AddHeader("WWW-Authenticate", String.Format("Basic realm=\"{0}\"", "Admin"));
            res.End();
        }
    }
}