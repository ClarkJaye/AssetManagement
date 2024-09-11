using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Data;
using AssetManagement.Models;
using PagedList;
using System.Web;
using System.Drawing.Printing;
using AssetManagement.Utility;

namespace AssetManagement.Controllers
{
    public class UserDepartmentsController : BaseController
    {
        private readonly AssetManagementContext _context;

        public UserDepartmentsController(AssetManagementContext context)
        : base(context) // Pass the 'context' dependency to the base constructor
        {
            _context = context;
        }


        public async Task<IActionResult> UserDepartmentView(string userCODE)
        {   
            var assetManagementContext = _context.tbl_ictams_userdept.Include(u => u.Department).Include(u => u.User).ToListAsync();
            return View(assetManagementContext);
        }


        // GET: UserDepartments
        public async Task<IActionResult> Index(int? page, string searchString)
        {
            var ucode = HttpContext.Session.GetString("UserName");

            var findPass = await _context.tbl_ictams_users.Where(x => x.UserCode == ucode).FirstOrDefaultAsync();

            var PasswordIsCorrect = BCrypt.Net.BCrypt.Verify("1234", findPass.UserPassword);
            if (PasswordIsCorrect)
            {
                // Show success alert using SweetAlert
                TempData["AlertType"] = "success";
                TempData["SuccessMessage"] = "Login successfully!";
                return RedirectToAction("ChangePassword", "Users");
            }

            int? userProfile = HttpContext.Session.GetInt32("UserProfile");
            if (userProfile.HasValue)
            {

                var hasOpenAccess = await _context.tbl_ictams_profileaccess
          .AnyAsync(pa => pa.OpenAccess == "Y" &&
                          pa.Module.ModuleTitle == "User Departments" &&  // Adjust the module name as needed
                          pa.ProfileId == userProfile.Value);
                if (!hasOpenAccess)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    int pageSize = 10;
                    int pageIndex = 1;
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

                    if (!string.IsNullOrEmpty(searchString))
                    {
                        var query1 = _context.tbl_ictams_userdept.Include(u => u.Department).Include(u => u.User)
                            .Where(x => x.User.UserADLogin.Contains(searchString) || x.Department.Dept_name.Contains(searchString))
                            .ToPagedList(pageIndex, pageSize);
                        return View(query1);
                    }

                    ViewBag.UserProfile = HttpContext.Session.GetInt32("UserProfile");
                    ViewData["UserProfile"] = HttpContext.Session.GetInt32("UserProfile");

                    ViewBag.UserName = HttpContext.Session.GetString("UserName");
                    ViewData["UserName"] = HttpContext.Session.GetString("UserName");

                    var assetManagementContext = _context.tbl_ictams_userdept.Include(u => u.Department).Include(u => u.User).ToPagedList(pageIndex, pageSize);
                    return View(assetManagementContext);
                }
            }

            return RedirectToAction("Index", "Home");

        }


        // GET: UserDepartments
        public async Task<IActionResult> UserDepartmentsViews(int? page, string searchString)
        {
           
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            if (!string.IsNullOrEmpty(searchString))
            {
                var query1 = _context.tbl_ictams_userdept.Include(u => u.Department).Include(u => u.User)
                    .Where(x => x.User.UserADLogin.Contains(searchString) || x.Department.Dept_name.Contains(searchString))
                    .ToPagedList(pageIndex, pageSize);
                return View(query1);
            }

            ViewBag.UserProfile = HttpContext.Session.GetInt32("UserProfile");
            ViewData["UserProfile"] = HttpContext.Session.GetInt32("UserProfile");

            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewData["UserName"] = HttpContext.Session.GetString("UserName");

            var assetManagementContext = _context.tbl_ictams_userdept.Include(u => u.Department).Include(u => u.User).ToPagedList(pageIndex, pageSize);
            return View(assetManagementContext);


        }


        // GET: UserDepartments/Create
        public async Task<IActionResult> Create(string userCODE)
        {

            ViewData["DeptCode"] = new SelectList(_context.tbl_ictams_department.Where(x=>x.Dept_status == "AC"), "Dept_code", "Dept_name");
            ViewData["UserCode"] = new SelectList(_context.tbl_ictams_users.Where(x => x.UserStatus == "AC"), "UserCode", "UserFullName");
            return View();
        }

        // POST: UserDepartments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserCode,DeptCode")] UserDepartment userDepartment)
        {
            if (UserDepartmentExists( userDepartment))
            {
                ModelState.AddModelError("DeptCode", "Sorry, but it's already exists");
                TempData["ErrorMessage"] = "Duplicate department is not allowed!";
            }
            else
            {
                var findStore = await _context.tbl_ictams_department.Where(x => x.Dept_code == userDepartment.DeptCode && x.Dept_status == "AC").FirstOrDefaultAsync();
                if (findStore != null)
                {
                    _context.Add(userDepartment);
                    await _context.SaveChangesAsync();
                    // ...
                    TempData["SuccessNotification"] = "Successfully added a new user department!";
                    // ...
                    return RedirectToAction(nameof(Index));
                }
                   
            }

            ViewData["DeptCode"] = new SelectList(_context.tbl_ictams_department.Where(x => x.Dept_status == "AC"), "Dept_code", "Dept_name");
            ViewData["UserCode"] = new SelectList(_context.tbl_ictams_users.Where(x => x.UserStatus == "AC"), "UserCode", "UserFullName");
            return View();


        }

        // GET: UserStores/Delete/5
        public async Task<IActionResult> Delete(string userCode, int departmentCode)
        {
            if (userCode == null || departmentCode == null)
            {
                return NotFound();
            }

            var userDept = await _context.tbl_ictams_userdept
                .FirstOrDefaultAsync(us => us.UserCode == userCode && us.DeptCode == departmentCode);

            if (userDept == null)
            {
                return NotFound();
            }


            _context.tbl_ictams_userdept.Remove(userDept);
            await _context.SaveChangesAsync();

            // ...
            TempData["SuccessNotification"] = "Successfully delete a user department!";
            // ...
            return RedirectToAction(nameof(Index));
        }


        private bool UserDepartmentExists(UserDepartment  userDepartment)
        {
          return (_context.tbl_ictams_userdept?.Any(e => e.UserCode == userDepartment.UserCode && e.DeptCode == userDepartment.DeptCode)).GetValueOrDefault();
        }
    }
}
