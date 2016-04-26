using SportsStore.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportsStore.WebUI.Controllers
{
    public class UserController : Controller
    {
        SqlConnection db = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SportsStore;Integrated Security=True");
        // GET: User
        public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(UserModel user)
        {
            if (ModelState.IsValid)
            {
                if(CheckIfSame(user.Password, user.Password2))
                {

                }
                else
                {
                    ModelState.AddModelError("", "Passwords do not match!");
                }
            }
            return View(user);
        }
        public ActionResult SignIn()
        {
            return View();
        }
        public ActionResult SignOut()
        {
            return View();
        }
        private bool CheckIfExists(string name, string password)
        {
            //var query = from u in db.
            return true;
        }
        private bool CheckIfSame(string password1, string password2)
        {
            return password1 == password2;
        }
    }
}