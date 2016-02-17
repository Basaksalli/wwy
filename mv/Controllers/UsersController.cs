using mv.DataModel;
using mv.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mv.Controllers
{
    public class UsersController : Controller
    {
        DataModel.Model1 ent = new DataModel.Model1();
        // GET: Users
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult Login()
        {
            ViewData["error"] = TempData["error"];
            return View();
        }

        [HttpPost]
        public ActionResult Login(DataModel.Users u)
        {
            var kullanici = (from item in ent.Users
                             where item.Email == u.Email && item.Password == u.Password
                             select item).FirstOrDefault();

            if (kullanici!=null)
            { Session["KullaniciLogin"] = kullanici;
                return RedirectToAction("Index", "ProfilePage");
            }
            else
            {
                TempData["error"] = "Kullanici Bilgileri Hatalı..!";
                return View();

                //Kullanıcı bilgileri hatalı mesajı verilmesi gereklidir.
                
            }
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(Register r)
        {
            Users u = new Users();
            u.Name = r.Name;
            u.Surname = r.Surname;
            u.Email = r.Email;
            u.Password = r.Password;
            u.Phone = r.Phone;
            u.City = r.City;
            u.Country = r.Country;
            u.Gender = Convert.ToBoolean(r.Gender);
            u.Birthdate = r.Birthdate;

            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Profile-image"), fileName);
                    file.SaveAs(path);
                    u.PictureLoc = path;
                }
            }

            ent.Users.Add(u);
            try
            {
                ent.SaveChanges();
            }
            catch (Exception ex)
            {
                string hata = ex.Message;
            }

            return RedirectToAction("Register");

        }

        public ActionResult Edit(int id)
        {
            var user = (from item in ent.Users
                        where item.ID == id
                        select item).FirstOrDefault();
            return View(user);
        }
        [HttpPost]
        public ActionResult Edit(Users user)
        {
            //değişek olan kayda odaklandık.
            var degisen = (from item in ent.Users
                     where item.ID == user.ID
                     select item).FirstOrDefault();

            //odaklandımığız nesnenin propertylerine müdahale edityoruz.

            degisen.Email = user.Email;
            degisen.Password = user.Password;
            degisen.Name = user.Name;
            degisen.Surname = user.Surname;
            degisen.Phone = user.Phone;
            degisen.Country = user.Country;
            degisen.City = user.City;
            degisen.Birthdate = user.Birthdate; 
            degisen.PictureLoc = user.PictureLoc;
            try
            {
                ent.SaveChanges();
                return Redirect("http://localhost:22572/ProfilePage");
                
            }
            catch (SqlException ex)
            {

                string hata = ex.Message;
                return View(user);

            }
        }
    }
}