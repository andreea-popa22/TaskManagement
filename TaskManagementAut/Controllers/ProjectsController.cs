using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskManagement.Models;
using TaskManagementAut;
using TaskManagementAut.Models;
using static TaskManagement.Models.Project;


namespace TaskManagement.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private int _perPage = 3;

        // GET: Projects
        [Authorize(Roles = "User, Organizator, Admin")]
        public ActionResult Index()
        {
            var projects = db.Projects.Include("Team").Include("User").OrderBy(a => a.ProjectDate); 
            var search = "";

            if(Request.Params.Get("search") != null)
            {
                search = Request.Params.Get("search").Trim();
                List<int> projectsIds = db.Projects.Where(
                    at => at.ProjectTitle.Contains(search)
                    || at.ProjectDescription.Contains(search)
                    ).Select(a => a.ProjectId).ToList();

                List<int> tasksIds = db.Tasks.Where(c => c.TaskDescription.Contains(search))
                    .Select(t => t.ProjectId).ToList();

                List<int> mergedIds = projectsIds.Union(tasksIds).ToList();

                projects = db.Projects.Where(p => mergedIds.Contains(p.ProjectId)).Include("Team").Include("User").OrderBy(a => a.ProjectDate);


            }

            var totalItems = projects.Count();
            var currentPage = Convert.ToInt32(Request.Params.Get("page"));

            var offset = 0;

            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * this._perPage;
            }

            var paginatedProjects = projects.Skip(offset).Take(this._perPage);

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            ViewBag.total = totalItems;
            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)this._perPage);
            ViewBag.Projects = paginatedProjects;
            ViewBag.SearchString = search;

            return View();
        }

        //Get: Show
        [Authorize(Roles = "User, Organizator, Admin")]
        public ActionResult Show(int id)
        {

            var project = db.Projects.Find(id);

            //var projects = db.Projects.Include("Team");

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }
            SetAccessRights(id);
            ViewBag.ListOfUsers = GetAllUsers();
            ViewBag.ListOfMembers = GetAllMembers(id);
            return View(project);
        }

        [HttpPost]
        [Authorize(Roles = "Organizator, Admin")]
        //[ActionName("AdaugareTask")]
        public ActionResult Show(Task taskk)
        {
            //am comm pt a asigna un task taskk.UserId = User.Identity.GetUserId();
            SetAccessRights(taskk.ProjectId);
            ViewBag.ListOfMembers = GetAllMembers(taskk.ProjectId);
            ViewBag.ListOfUsers = GetAllUsers();

            try
            {
                if (ModelState.IsValid)
                {
                    db.Tasks.Add(taskk);
                    db.SaveChanges();
                    TempData["message"] = "Taskul a fost adaugat!";
                    return Redirect("/Projects/Show/" + taskk.ProjectId);
                }

                else
                {

                    Project a = db.Projects.Find(taskk.ProjectId);
                    
                    return View(a);

                }

            }

            catch (Exception e)
            {
                Project a = db.Projects.Find(taskk.ProjectId);
                SetAccessRights(a.ProjectId);
                return View(a);
            }

        }

        [HttpPost]
        [Authorize(Roles = "Organizator, Admin")]
        [ActionName("AfisareMembrii")]
        public ActionResult Show(int id, Member member)
        {
            Project a = db.Projects.Find(id);
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
                    return Redirect("/Projects/Show/" + id);
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
            Project project = new Project();
            project.Teamm = GetAllTeams();
            //project.Teamm = GetAllTeams();
            project.UserId = User.Identity.GetUserId();

            return View(project);
        }

        //POST: New
        [Authorize(Roles = "User, Organizator, Admin")]
        [HttpPost]
        public ActionResult New(Project project)
        {
            project.UserId = User.Identity.GetUserId();
            
            try
            {
                
                if (ModelState.IsValid)
                {
                    db.Projects.Add(project);
                    if(User.IsInRole("User"))
                    {
                        ApplicationDbContext context = new ApplicationDbContext();
                        var UserManager = new UserManager<ApplicationUser>(new Microsoft.AspNet.Identity.EntityFramework.UserStore<ApplicationUser>(context));
                        UserManager.RemoveFromRole(project.UserId, "User");
                        UserManager.AddToRole(project.UserId, "Organizator");
                    }
                    db.SaveChanges();
                    TempData["message"] = "Proiectul a fost adaugat!";
                    return RedirectToAction("Index");
                }
                else
                {
                    project.Teamm = GetAllTeams();
                    return View(project);
                }

            }
            catch (Exception e)
            {
                project.Teamm = GetAllTeams();
                return View(project);
            }
        }
        //GET: Edit
        [Authorize(Roles = "Organizator, Admin")]
        public ActionResult Edit(int id)
        {
            var project = db.Projects.Find(id);
            project.Teamm = GetAllTeams();
            if (project.UserId == User.Identity.GetUserId() || User.IsInRole("Admin") )
            {
                return View(project);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa modificati proiectul!";
                return RedirectToAction("Index");
            }
        }

        //PUT: Edit
        [HttpPut]
        [Authorize(Roles = "Organizator, Admin")]
        public ActionResult Edit(int id, Project requestProject)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    Project project = db.Projects.Find(id);
                    if (project.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
                    {
                        if (TryUpdateModel(project))
                        {
                            project.ProjectTitle = requestProject.ProjectTitle;
                            project.ProjectDescription = requestProject.ProjectDescription;
                            project.ProjectDate = requestProject.ProjectDate;
                            project.TeamId = requestProject.TeamId;
                            db.SaveChanges();
                            TempData["message"] = "Proiectul a fost modificat cu succes!";

                        }
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["message"] = "Nu aveti dreptul sa stergeti articolul!";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    requestProject.Teamm = GetAllTeams();
                    return View(requestProject);
                }
            }
            catch (Exception e)
            {
                requestProject.Teamm = GetAllTeams();
                return View(requestProject);
            }
        }

        //DELETE: Delete
        [HttpDelete]
        [Authorize(Roles = "Organizator, Admin")]
        public ActionResult Delete(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
            db.SaveChanges();
            TempData["message"] = "Proiectul cu titlul " + project.ProjectTitle + " a fost sters cu succes!";
            return RedirectToAction("Index");
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllTeams()
        {
            var selectList = new List<SelectListItem>();
            var teams = from t in db.Teams
                             select t;
            foreach (var team in teams)
            {
                selectList.Add(new SelectListItem
                {
                    Value = team.TeamId.ToString(),
                    Text = team.TeamName.ToString()
                });
            }
            return selectList;
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllMembers(int ProjectId)
        {
            var selectList = new List<SelectListItem>();
            var p = db.Projects.Find(ProjectId);
            var t = db.Teams.Find(p.TeamId);

            var users = from u in db.Users
                        select u;

            foreach (var member in t.Members)
            {
                foreach(var user in users)
                {
                    if(user.Id == member.UserId)
                    {
                        selectList.Add(new SelectListItem
                        {
                            Value = member.UserId.ToString(),
                            Text = user.Email.ToString()
                        });
                    }
                }
                
            }
            return selectList;
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

        private void SetAccessRights(int ProjectId)
        {
            var project = db.Projects.Find(ProjectId);
            var team = db.Teams.Find(project.TeamId);

            ViewBag.afisareTaskuri = false;

            foreach (var member in team.Members)
            {
                if (User.Identity.GetUserId() == member.UserId)
                    ViewBag.afisareTaskuri = true;
                
            }

            ViewBag.afisareButoane = false;
            if (User.IsInRole("Organizator") && project.UserId == User.Identity.GetUserId())
            {
                ViewBag.afisareButoane = true;
            }

            ViewBag.esteAdmin = User.IsInRole("Admin");
            ViewBag.utilizatorCurent = User.Identity.GetUserId();
        }
    }
}
