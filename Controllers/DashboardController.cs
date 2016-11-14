using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DashboardApp.Factory;
using aspuserdashboard.Models;
using CryptoHelper;

namespace aspuserdashboard.Controllers
{
    public class DashboardController : Controller
    {
        private readonly DashboardRepository dashboardFactory;
        private readonly MessageRepository messageFactory;
        private readonly CommentRepository commentFactory;
        public DashboardController()
        {
            dashboardFactory = new DashboardRepository();
            messageFactory = new MessageRepository();
            commentFactory = new CommentRepository();
        }
 // Index views::: LandinPage, Login, Registration
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View("Index");
        }
        [HttpGet]
        [Route("Register")]
        public IActionResult Register()
        {
            if(TempData["errors"] != null)
            {
               ViewBag.errors = TempData["errors"];
            }
            return View("Register");
        }
        [HttpGet]
        [Route("Login")]
        public IActionResult Login()
        {
            if(TempData["errors"] != null)
            {
               ViewBag.errors = TempData["errors"];
            }
            return View("Login");
        }
// Post Methods:: Login, Registration
        [HttpPost]
        [Route("registration")]
        public IActionResult Create(User newuser)
        {   
            if(ModelState.IsValid)
            {
                 ViewBag.User_all = dashboardFactory.AllUsers();
                 if(ViewBag.User_all.Count == 0) // Admins
                 {
                     dashboardFactory.Add_Admin(newuser);
                     ViewBag.User_Extracting = dashboardFactory.FindByID();
                     int current_id = ViewBag.User_Extracting.id;
                     HttpContext.Session.SetInt32("current_id", (int) current_id);
                     return RedirectToAction("Dashboard");
                 }
                // Non Admins
                 dashboardFactory.Add(newuser);
                 ViewBag.User_Extracting = dashboardFactory.FindByID();
                 int current_other_id = ViewBag.User_Extracting.id;
                 HttpContext.Session.SetInt32("current_id", (int) current_other_id);
                 return RedirectToAction("Dashboard");
            }
            List<string> temp_errors = new List<string>();
            foreach(var error in ModelState.Values)
            {
                if(error.Errors.Count > 0)
                {
                    temp_errors.Add(error.Errors[0].ErrorMessage);
                }  
            }
            TempData["errors"] = temp_errors;
            return RedirectToAction("Register");
        }
        [HttpPost]
        [RouteAttribute("login")]
        public IActionResult Login(string email, string password)
        {
            List<string> temp_errors = new List<string>();
            if(email == null || password == null)
            {
                temp_errors.Add("Enter Email and Password Fields to Login");
                TempData["errors"] = temp_errors;
                return RedirectToAction("Login");
            }
//Login User Id Extracting query
          User check_user = dashboardFactory.FindEmail(email);
            if(check_user == null)
            {
                temp_errors.Add("Email is not registered");
                TempData["errors"] = temp_errors;
                return RedirectToAction("Login");
            }
            bool correct = Crypto.VerifyHashedPassword((string) check_user.password, password);
            if(correct)
            {
                HttpContext.Session.SetInt32("current_id", check_user.id);
                return RedirectToAction("Dashboard");
            }
            else{
                temp_errors.Add("Password is not matching");
                TempData["errors"] = temp_errors;
                return RedirectToAction("Login");
            }
        }
 // Admin and all users Dashboard start
        [HttpGet]
        [Route("dashboard")]
        public IActionResult Dashboard()
        {
            //on refresh once after logout
            if(HttpContext.Session.GetInt32("current_id") == null)
            {
                return RedirectToAction("Index");
            }
            //Dashboard begins
            ViewBag.User_one = dashboardFactory.CurrentUser((int)HttpContext.Session.GetInt32("current_id"));
             if(ViewBag.User_one.auth_level == 0)
            {
                ViewBag.User_all = dashboardFactory.AllUsers();
                return View("AdminDashboard");
            }
            // if not admin
            ViewBag.User_all = dashboardFactory.AllUsers();
            return View("UserDashboard");
        }
 // Adding new user by admin
        [HttpGet]
        [Route("new")]
        public IActionResult New()
        {
            //on refresh
             if(HttpContext.Session.GetInt32("current_id") == null)
            {
                return RedirectToAction("Index");
            }
            //display
            if(TempData["errors"] != null)
            {
               ViewBag.errors = TempData["errors"];
            }
            ViewBag.User_one = dashboardFactory.CurrentUser((int)HttpContext.Session.GetInt32("current_id"));
            return View("Addnewuser");
        }
        [HttpPost]
        [Route("createNewUser")]
        public IActionResult AddnewUser(User newuser)
        {   
            if(ModelState.IsValid)
            {
                 dashboardFactory.Add(newuser);
                 return RedirectToAction("Dashboard");
            }
            List<string> temp_errors = new List<string>();
            foreach(var error in ModelState.Values)
            {
                if(error.Errors.Count > 0)
                {
                    temp_errors.Add(error.Errors[0].ErrorMessage);
                }  
            }
            TempData["errors"] = temp_errors;
            return RedirectToAction("New");
        }
    //UserEdit Profile-Page
        [HttpGet]
        [Route("edit")]
        public IActionResult Edit()
        {
             //on refresh
             if(HttpContext.Session.GetInt32("current_id") == null)
            {
                return RedirectToAction("Index");
            }
             if(TempData["errors"] != null)
            {
               ViewBag.errors = TempData["errors"];
            }
            ViewBag.User_one = dashboardFactory.CurrentUser((int)HttpContext.Session.GetInt32("current_id"));
            return View("UserEdit");
        }
        [HttpPost]
        [Route("edit")]
        public IActionResult EditUser(string email, string first_name, string last_name , string image, int auth_level, int id)
        {   
            dashboardFactory.Edit(id, email, first_name, last_name, image, auth_level);
            return RedirectToAction("Dashboard");
        }
        [HttpPost]
        [Route("edit_password")]
        public IActionResult Edit_Password(string password, string confirm_password, int id)
        {   
            List<string> temp_errors = new List<string>();
            if(password == null || password.Length < 3)
            {
                temp_errors.Add("Password Field Must be atleast 3 characters");
                TempData["errors"] = temp_errors;
                return RedirectToAction("Edit");
            }
            else if(password != confirm_password)
            {
                temp_errors.Add("Password and Confirm Password fields Must Match");
                TempData["errors"] = temp_errors;
                return RedirectToAction("Edit");
            }
            dashboardFactory.Edit_Password(id, password);
            return RedirectToAction("Dashboard");
        }
        [HttpPost]
        [Route("description")]
        public IActionResult Description(string description)
        {
            ViewBag.User_one = dashboardFactory.CurrentUser((int)HttpContext.Session.GetInt32("current_id"));
            int edit_des_id = ViewBag.User_one.id;
            dashboardFactory.Edit_Description(edit_des_id, description);
            return RedirectToAction("Dashboard");   
        }
        [HttpGet]
        [Route("edit/{id}")]
        public IActionResult Admin_Edit(string id = "")
        {
             //on refresh
             if(HttpContext.Session.GetInt32("current_id") == null)
            {
                return RedirectToAction("Index");
            }
             if(TempData["errors"] != null)
            {
               ViewBag.errors = TempData["errors"];
            }

            ViewBag.Find_Edit_User = dashboardFactory.FindEditProfile(id);
            ViewBag.User_one = dashboardFactory.CurrentUser((int)HttpContext.Session.GetInt32("current_id"));
            return View("AdminEdit");
        }
        [HttpPost]
        [Route("admin_edit_password")]
        public IActionResult Admin_Edit_Password(string password, string confirm_password, int id)
        {   
            ViewBag.id = id;
            List<string> temp_errors = new List<string>();
            if(password == null || password.Length < 3)
            {
                temp_errors.Add("Password Field Must be atleast 3 characters");
                TempData["errors"] = temp_errors;
                Console.WriteLine(id);
                return RedirectToAction("Admin_Edit", new { id = id});
            }
            else if(password != confirm_password)
            {
                temp_errors.Add("Password and Confirm Password fields Must Match");
                TempData["errors"] = temp_errors;
                return RedirectToAction("Admin_Edit", new { id = id});
            }
            dashboardFactory.Edit_Password(id, password);
            return RedirectToAction("Dashboard");
        }
        [HttpGet]
        [Route("delete/{id}")]
        public IActionResult Admin_Delete(string id = "")
        {
            ViewBag.Find_Edit_User = dashboardFactory.FindDeleteProfile(id);
            ViewBag.User_one = dashboardFactory.CurrentUser((int)HttpContext.Session.GetInt32("current_id"));
            return RedirectToAction("Dashboard");
        }
//show user messageboard
        [HttpGet]
        [Route("show/{id}")]
        public IActionResult Show_Board(string id = "")
        {
             //on refresh
             if(HttpContext.Session.GetInt32("current_id") == null)
            {
                return RedirectToAction("Index");
            }
             if(TempData["errors"] != null)
            {
               ViewBag.errors = TempData["errors"];
            }
            Console.WriteLine(id);
            ViewBag.messages = messageFactory.FindAllMessages(id);
            ViewBag.Show_User = dashboardFactory.FindEditProfile(id);
            ViewBag.comments = commentFactory.FindAllComments();
            ViewBag.User_one = dashboardFactory.CurrentUser((int)HttpContext.Session.GetInt32("current_id"));
            return View("Rough");
        }
        [HttpPost]
        [Route("message")]
        public IActionResult Message(Message newmessage)
        {
            int show_id = newmessage.tag_id;
            List<string> temp_errors = new List<string>();
            if(ModelState.IsValid)
            {
                 messageFactory.AddMessage(newmessage);
                 Console.WriteLine("Message is Successfully added");
                 return RedirectToAction("Show_Board", new { id = show_id});
            }
            else
            {
                temp_errors.Add("Message Strength is weak");
                TempData["errors"] = temp_errors;
                return RedirectToAction("Show_Board", new { id = show_id});
            }
        }
        [HttpPost]
        [Route("comment")]
        public IActionResult Comment(Comment newcomment)
        {
            int show_id = newcomment.temp_id;
            List<string> temp_errors = new List<string>();
            if(ModelState.IsValid)
            {
                Console.WriteLine("Comment is Successfully added");
                commentFactory.AddComment(newcomment);
                 return RedirectToAction("Show_Board", new { id = show_id});
            }
            else
            {
                temp_errors.Add("Comment Strength is weak");
                TempData["errors"] = temp_errors;
                return RedirectToAction("Show_Board", new { id = show_id});
            }
        }
// Logout
        [HttpGet]
        [Route("logout")]
         public IActionResult Logout()
         {
             HttpContext.Session.Clear();
             Console.WriteLine("session is" + HttpContext.Session.GetInt32("current_id"));
             return RedirectToAction("Index");

         }
    }

}



