using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskManagementAut.Models;

namespace TaskManagementAut.Controllers
{
    public class MembersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Members
        //public ActionResult Index()
        //{
        //    return View();
        //}

        //DELETE: Delete
        [HttpDelete]
        [Authorize(Roles = "Organizator, Admin")]
        public ActionResult Delete(int id)
        {
            Member member = db.Members.Find(id);
            db.Members.Remove(member);
            db.SaveChanges();
            TempData["message"] = "Membrul a fost sters!";
            return RedirectToAction("Index", "Projects");

        }
    }
}