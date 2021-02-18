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
    
    public class TasksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tasks
        [Authorize(Roles = "Organizator, Admin")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "User, Organizator, Admin")]
        public ActionResult Show(int id)
        {
            Task taskk = db.Tasks.Find(id);
            SetAccessRights(id);
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }
            
            SetAccessRights(id);
            var users = from u in db.Users
                        select u;
            foreach(var u in users)
            {
                if(u.Id == taskk.UserId)
                {
                    ViewBag.NameofUser = u.Email;
                }
            }
            
            return View(taskk);
        }


        //POST: New
        //[HttpPost]
        //public ActionResult New(Task task)
        //{
        //    try
        //    {
        //        db.Tasks.Add(task);
        //        db.SaveChanges();
        //        return Redirect("/Projects/Show/" + task.ProjectId);
        //    }
        //    catch (Exception e)
        //    {
        //        return Redirect("/Projects/Show/" + task.ProjectId);
        //    }
        //}
        //GET: Edit
        [Authorize(Roles = "Organizator, Admin")]
        public ActionResult Edit(int id)
        {
            var taskk = db.Tasks.Find(id);
            taskk.TaskId = id;
            SetAccessRights(id);
            ViewBag.ListOfMembers = GetAllMembers(id);
            return View(taskk);
        }

        //PUT: Edit
        [Authorize(Roles = "Organizator, Admin")]
        [HttpPut]
        public ActionResult Edit(int id, Task requestTask)
        {
            SetAccessRights(id);
            ViewBag.ListOfMembers = GetAllMembers(requestTask.TaskId);

            try
            {
                if (ModelState.IsValid)
                {
                    Task taskk = db.Tasks.Find(id);
                    var idP = taskk.ProjectId;
                    SetAccessRights(id);
                    if (TryUpdateModel(taskk))
                    {
                        taskk.TaskTitle = requestTask.TaskTitle;
                        taskk.TaskDescription = requestTask.TaskDescription;
                        taskk.StartDate = requestTask.StartDate;
                        taskk.EndDate = requestTask.EndDate;
                        taskk.Status = requestTask.Status;
                        taskk.ProjectId = requestTask.ProjectId;
                        db.SaveChanges();
                        TempData["message"] = "Taskul a fost modificat cu succes!";
                        
                    }
                    ViewBag.ListOfMembers = GetAllMembers(id);
                    return Redirect("/Projects/Show/" + idP);
                }
                else
                {
                    
                    return View(requestTask);
                }
            }
            catch (Exception e)
            {
               
                return View(requestTask);
            }
        }

        //[Authorize(Roles = "User, Organizator, Admin")]
        //[HttpPut]
        //[ActionName("EditStatus")]
        //public ActionResult Edit(int id, string S)
        //{
        //    Task taskk = db.Tasks.Find(id);
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {

        //            if (TryUpdateModel(taskk))
        //            {

        //                taskk.Status = S;

        //                db.SaveChanges();
        //                TempData["message"] = "Taskul a fost modificat cu succes!";

        //            }
        //            ViewBag.ListOfMembers = GetAllMembers(id);
        //            return Redirect("/Projects/Show/" + taskk.ProjectId);
        //        }
        //        else
        //        {

        //            return View(taskk);
        //        }
        //    }
        //    catch (Exception e)
        //    {

        //        return View(taskk);
        //    }
        //}

        [HttpPost]
        [Authorize(Roles = "User, Organizator, Admin")]
        public ActionResult Show(Comment comm)
        {
            comm.Date = DateTime.Now;
            comm.UserId = User.Identity.GetUserId();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Comments.Add(comm);
                    db.SaveChanges();
                    TempData["message"] = "Comentariul a fost adaugat!";
                    return Redirect("/Tasks/Show/" + comm.TaskId);
                }

                else
                {
                    Task a = db.Tasks.Find(comm.TaskId);
                    SetAccessRights(comm.TaskId);
                    return View(a);
                }

            }

            catch (Exception e)
            {
                Task a = db.Tasks.Find(comm.TaskId);
                return View(a);
            }

        }

        //DELETE: Delete
        [Authorize(Roles = "Organizator, Admin")]
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Task task = db.Tasks.Find(id);
            db.Tasks.Remove(task);
            db.SaveChanges();
            TempData["message"] = "Taskul cu titlul " + task.TaskTitle + " a fost sters cu succes!";
            return Redirect("/Projects/Show/" + task.ProjectId);
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllMembers(int TaskId)
        {
            var selectList = new List<SelectListItem>();
            var task = db.Tasks.Find(TaskId);
            var p = task.Project;
            var t = db.Teams.Find(p.TeamId);

            var users = from u in db.Users
                        select u;

            foreach (var member in t.Members)
            {
                foreach (var user in users)
                {
                    if (user.Id == member.UserId)
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

        private void SetAccessRights(int id)
        {
            ViewBag.afisareButoane = false;
            ViewBag.schimbaStatus = false;
            ViewBag.esteOrganizator = false;

            var task = db.Tasks.Find(id);
            var project = db.Projects.Find(task.ProjectId);


            if (User.IsInRole("Admin"))
            {
                ViewBag.afisareButoane = true;
            }

            var t = db.Tasks.Find(id);
            if (User.IsInRole("Organizator") && t.Project.UserId == User.Identity.GetUserId())
            {
                ViewBag.afisareButoane = true;
                ViewBag.esteOrganizator = true;
            }

            foreach (var taskk in project.Tasks)
            {
                if (task.UserId == User.Identity.GetUserId() && taskk.TaskId == id)
                        ViewBag.schimbaStatus = true;
            }
            
            

            ViewBag.esteAdmin = User.IsInRole("Admin");
            ViewBag.utilizatorCurent = User.Identity.GetUserId();
        }
    }
}
