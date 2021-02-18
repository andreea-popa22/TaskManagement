using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskManagement.Models;
using TaskManagementAut.Models;
using static TaskManagement.Models.Project;


namespace TaskManagement.Controllers
{
    [Authorize(Roles = "User, Organizator, Admin")]
    public class TeamsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Teams
        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }
            var teams = from team in db.Teams
                             orderby team.TeamName
                             select team;
            ViewBag.Teams = teams;
            
            return View();
        }

        //Get: Show
        [Authorize(Roles = "User, Organizator, Admin")]
        public ActionResult Show(int id)
        {
            Team team = db.Teams.Find(id);
            ViewBag.ListOfTasks = GetAllTasks(id);
            SetAccessRights(id);
            ViewBag.ListOfUsers = GetAllUsers();
            return View(team);
        }

        [HttpPost]
        [Authorize(Roles = "Organizator, Admin")]
        [ActionName("AfisareMembrii")]
        public ActionResult Show(int id, Member member)
        {
            Team a = db.Teams.Find(id);
            try
            {
                if (ModelState.IsValid)
                {
                    if (a.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
                    {
                        if (db.Members.Any(ob => ob.TeamId == member.TeamId && ob.UserId == member.UserId))
                        {
                            TempData["message"] = "Membrul face deja parte din echipa!";
                        }
                        else
                        {
                            db.Members.Add(member);
                            //member.Tasks = a.Tasks.ToList();

                            //a.Tasks.CopyTo('member.Tasks');
                            db.SaveChanges();
                            TempData["message"] = "Membrul a fost adaugat in echipa!";
                        }
                    }
                    else
                    {
                        TempData["message"] = "Nu puteti adauga membrii!";
                    }
                    return Redirect("/Teams/Show/" + id);
                }
                else
                {
                    SetAccessRights(id);
                    ViewBag.ListOfUsers = GetAllUsers();
                    return View(a);
                }
            }
            catch (Exception e)
            {
                SetAccessRights(id);
                ViewBag.ListOfUsers = GetAllUsers();
                return View(a);
            }

        }


        //Get: New
        [Authorize(Roles = "User, Organizator, Admin")]
        public ActionResult New()
        {
            return View();
        }

        //POST: New
        [HttpPost]
        [Authorize(Roles = "User, Organizator, Admin")]
        public ActionResult New(Team team)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Teams.Add(team);
                    db.SaveChanges();
                    TempData["message"] = "Echipa a fost adaugata!";
                    if (User.IsInRole("Admin"))
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("New", "Projects");
                    }
                        
                }
                else
                {
                    return View(team);
                }
                    
            }
            catch (Exception e)
            {
                return View(team);
            }
        }
        //GET: Edit
        [Authorize(Roles = "Organizator, Admin")]
        public ActionResult Edit(int id)
        {
            Team team = db.Teams.Find(id);
            return View(team);
        }

        //PUT: Edit
        [HttpPut]
        [Authorize(Roles = "Organizator, Admin")]
        public ActionResult Edit(int id, Team requestTeam)
        {
            try
            {
                Team team = db.Teams.Find(id);
                if (TryUpdateModel(team))
                {
                    team.TeamName = requestTeam.TeamName;
                    db.SaveChanges();
                    TempData["message"] = "Echipa a fost modificata!";
                    return RedirectToAction("Index");
                }
                return View(requestTeam);
            }
            catch (Exception e)
            {
                return View(requestTeam);
            }
        }

        //DELETE: Delete
        [HttpDelete]
        [Authorize(Roles = "Organizator, Admin")]
        public ActionResult Delete(int id)
        {
            Team team = db.Teams.Find(id);
            db.Teams.Remove(team);
            TempData["message"] = "Echipa cu numele " + team.TeamName + " a fost stearsa cu succes!";
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [NonAction]
        public virtual List<Task> GetAllTasks(int TeamId)
        {
            var selectlist = new List<Task>();

            var team = db.Teams.Find(TeamId);

            foreach (var project in team.Projects)
            {
                foreach (var task in project.Tasks)
                {
                    selectlist.Add(task);
                }
            }
            return selectlist;
        }

        private void SetAccessRights(int TeamId)
        {
            ViewBag.afisareButoane = false;
            ViewBag.afisareTaskuri = false;
            var team = db.Teams.Find(TeamId);

            foreach(var m in team.Members)
            {
                if (User.Identity.GetUserId() == m.UserId)
                {
                    ViewBag.afisareTaskuri = true;
                }
            }

            foreach (var p in team.Projects)
            {
                if (User.Identity.GetUserId() == p.UserId  && User.IsInRole("Organizator"))
                {
                    ViewBag.afisareTaskuri = true;
                    ViewBag.afisareButoane = true;
                }
            }
                
            ViewBag.esteAdmin = User.IsInRole("Admin");
            ViewBag.utilizatorCurent = User.Identity.GetUserId();
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllUsers()
        {
            var selectList = new List<SelectListItem>();
            var users = from u in db.Users
                        select u;
            foreach (var user in users)
            {
                selectList.Add(new SelectListItem
                {
                    Value = user.Id.ToString(),
                    Text = user.Email.ToString()
                });
            }
            return selectList;
        }
    }
}