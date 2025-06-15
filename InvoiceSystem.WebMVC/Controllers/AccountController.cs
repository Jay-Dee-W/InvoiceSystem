using System.Web.Mvc;
using System.Web.SessionState; 

namespace InvoiceSystem.WebMVC.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            if (Session["IsAuthenticated"] != null && (bool)Session["IsAuthenticated"])
            {
                return RedirectToAction("Dashboard", "Invoice");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            // Hardcoded credentials for assessment purposes ONLY
            if (username == "admin" && password == "password")
            {
                Session["IsAuthenticated"] = true;
                Session["Username"] = username; 
                return RedirectToAction("Dashboard", "Invoice");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid username or password.";
                Session["IsAuthenticated"] = false; 
                return View();
            }
        }

        public ActionResult Logout()
        {
            Session.Clear(); 
            Session.Abandon(); 
            return RedirectToAction("Upsert", "Invoice");
        }
    }
}