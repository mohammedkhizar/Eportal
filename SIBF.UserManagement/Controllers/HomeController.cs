using SIBF.UserManagement.Api;
using SIBF.UserManagement.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using X.PagedList;

namespace SIBF.UserManagement.Controllers
{
    public class HomeController : Controller
    {
        private static readonly int PAGE_SIZE = 20;
        private IUserAccountService _accountService;
        
        public HomeController(IUserAccountService accountService)
        {
            this._accountService = accountService;
        }

        [Authorize]
        public ActionResult Index(int? Page, string SortBy = "Username", bool Ascending = true)
        {
            //List<MembershipUser> allUsers = GetAllUsers();
            //MembershipUserModel model = new MembershipUserModel();
            //model.Users = allUsers;
            ////var pageNumber = Page ?? 1;
            //var pageNumber = Page ?? 1;
            //model.UsersPage = model.Users.ToPagedList(pageNumber, 10);
            //switch (SortBy)
            //{
            //    case "Username":
            //        if (Ascending)
            //        {
            //            model.UsersPage = allUsers.OrderBy
            //                (u => u.Username).ToPagedList(pageNumber, PAGE_SIZE);
            //        }else
            //        {
            //            model.UsersPage = allUsers.OrderByDescending
            //                (u => u.Username).ToPagedList(pageNumber, PAGE_SIZE);
            //        }
            //        break;
            //    case "CreationDate":
            //        if (Ascending)
            //        {
            //            model.UsersPage = allUsers.OrderBy
            //                (u => u.CreationDate).ToPagedList(pageNumber, PAGE_SIZE);
            //        }else
            //        {
            //            model.UsersPage = allUsers.OrderByDescending
            //                (u => u.CreationDate).ToPagedList(pageNumber, PAGE_SIZE);
            //        }
            //        break;
            //    case "Email":
            //        if (Ascending)
            //        {
            //            model.UsersPage = allUsers.OrderBy
            //                (u => u.Email).ToPagedList(pageNumber, PAGE_SIZE);
            //        }
            //        else
            //        {
            //            model.UsersPage = allUsers.OrderByDescending
            //                (u => u.Email).ToPagedList(pageNumber, PAGE_SIZE);
            //        }
            //        break;
            //    case "IsLockedout":
            //        if (Ascending)
            //        {
            //            model.UsersPage = allUsers.OrderBy
            //                (u => u.IsLockedout).ToPagedList(pageNumber, PAGE_SIZE);
            //        }
            //        else
            //        {
            //            model.UsersPage = allUsers.OrderByDescending
            //                (u => u.IsLockedout).ToPagedList(pageNumber, PAGE_SIZE);
            //        }
            //        break;
            //    case "CreatedBy":
            //        if (Ascending)
            //        {
            //            model.UsersPage = allUsers.OrderBy
            //                (u => u.CreatedBy).ToPagedList(pageNumber, PAGE_SIZE);
            //        }
            //        else
            //        {
            //            model.UsersPage = allUsers.OrderByDescending
            //                (u => u.CreatedBy).ToPagedList(pageNumber, PAGE_SIZE);
            //        }
            //        break;                
            //}
            //model.SortAscending = Ascending;
            return View();
        }

        [Authorize]
        public ActionResult Details(string Username)
        {
            if (string.IsNullOrEmpty(Username))
                return RedirectToAction("Index", "Home");
            MembershipUser user = GetAllUsers().Find(u => u.Username == Username);
            return View(user);
        }

        [HttpGet]
        [Authorize]
        public ActionResult Edit(string Username)
        {
            if (string.IsNullOrEmpty(Username))
                return RedirectToAction("Index", "Home");
            MembershipUser user = GetAllUsers().Find(u => u.Username == Username);
            return View(user);
        }

        [HttpGet]
        [Authorize]
        public ActionResult Edit(MembershipUser member)
        {
            if (string.IsNullOrEmpty(member.Username))
                return RedirectToAction("Index", "Home");

            MembershipUser user = GetAllUsers().Find(u => u.Username == member.Username);
            return View(user);
        }

        public ActionResult LockUnlock(string Username, bool IsLock)
        {
            if (!string.IsNullOrEmpty(Username))
            {
                _accountService.UnlockUser(Username, IsLock);
            }
            return RedirectToAction("Index", "Home");
        }

        private List<MembershipUser> GetAllUsers()
        {
            return _accountService.GetAllUsers();
        }

        public ActionResult Employees(int? Page, string SortBy = "Username", bool Ascending = true)
        {

            List<MembershipUser> allUsers = GetAllUsers();
            MembershipUserModel model = new MembershipUserModel();
            model.Users = allUsers;
            //var pageNumber = Page ?? 1;
            var pageNumber = Page ?? 1;
            model.UsersPage = model.Users.ToPagedList(pageNumber, 10);
            switch (SortBy)
            {
                case "Username":
                    if (Ascending)
                    {
                        model.UsersPage = allUsers.OrderBy
                            (u => u.Username).ToPagedList(pageNumber, PAGE_SIZE);
                    }
                    else
                    {
                        model.UsersPage = allUsers.OrderByDescending
                            (u => u.Username).ToPagedList(pageNumber, PAGE_SIZE);
                    }
                    break;
                case "CreationDate":
                    if (Ascending)
                    {
                        model.UsersPage = allUsers.OrderBy
                            (u => u.CreationDate).ToPagedList(pageNumber, PAGE_SIZE);
                    }
                    else
                    {
                        model.UsersPage = allUsers.OrderByDescending
                            (u => u.CreationDate).ToPagedList(pageNumber, PAGE_SIZE);
                    }
                    break;
                case "Email":
                    if (Ascending)
                    {
                        model.UsersPage = allUsers.OrderBy
                            (u => u.Email).ToPagedList(pageNumber, PAGE_SIZE);
                    }
                    else
                    {
                        model.UsersPage = allUsers.OrderByDescending
                            (u => u.Email).ToPagedList(pageNumber, PAGE_SIZE);
                    }
                    break;
                case "IsLockedout":
                    if (Ascending)
                    {
                        model.UsersPage = allUsers.OrderBy
                            (u => u.IsLockedout).ToPagedList(pageNumber, PAGE_SIZE);
                    }
                    else
                    {
                        model.UsersPage = allUsers.OrderByDescending
                            (u => u.IsLockedout).ToPagedList(pageNumber, PAGE_SIZE);
                    }
                    break;
                case "CreatedBy":
                    if (Ascending)
                    {
                        model.UsersPage = allUsers.OrderBy
                            (u => u.CreatedBy).ToPagedList(pageNumber, PAGE_SIZE);
                    }
                    else
                    {
                        model.UsersPage = allUsers.OrderByDescending
                            (u => u.CreatedBy).ToPagedList(pageNumber, PAGE_SIZE);
                    }
                    break;
            }
            model.SortAscending = Ascending;
            return View(model);
        }
        
    }
}