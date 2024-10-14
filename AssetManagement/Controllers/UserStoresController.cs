    using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Data;
using AssetManagement.Models;
using AssetManagement.Models.View_Model;
using PagedList;
using System.Web;
using System.Drawing.Printing;
using AssetManagement.Utility;

namespace AssetManagement.Controllers
{
 
    public class UserStoresController : BaseController
    {
        private readonly AssetManagementContext _context;

        public UserStoresController(AssetManagementContext context)
        : base(context) // Pass the 'context' dependency to the base constructor
        {
            _context = context;
        }


        public async Task<IActionResult> UserStoreView(string userCODE)
        {   
            if (!string.IsNullOrEmpty(userCODE))
            {
                var assetManagementContext1 = await _context.tbl_user_stores.Where(x => x.User.UserCode.Contains(userCODE)).Include(u => u.Store).Include(u => u.User).ToListAsync();
                return View(assetManagementContext1);
            }
            var assetManagementContext = await _context.tbl_user_stores.Include(u => u.Store).Include(u => u.User).ToListAsync();
            return View(assetManagementContext);
        }


        public async Task<JsonResult> GetStoresDrop(string userCODE)
        {
            // Get the list of stores that user assigned
            var userAssigned = await _context.tbl_user_stores
                .Where(ud => ud.UserCode == userCODE)
                .Select(ud => ud.StoreCode)
                .ToListAsync();

            // Get the list of stores that the user is not assigned and order by StoreName A-Z
            var available = await _context.tbl_ictams_stores
                .Where(dept => !userAssigned.Contains(dept.Store_code) && dept.StoreStatus == "AC")
                .Select(d => new { d.Store_code, d.StoreName })
                .OrderBy(d => d.StoreName)  
                .ToListAsync();


            return Json(available);
        }

        public async Task<IActionResult> GetUserStores(string userCODE)
        {
            var userAssigned = await _context.tbl_user_stores
                .Where(ud => ud.UserCode == userCODE)
                .Select(ud => ud.StoreCode)
                .ToListAsync();

            var available = await _context.tbl_ictams_stores
                .Where(dept => userAssigned.Contains(dept.Store_code) && dept.StoreStatus == "AC")
                .ToListAsync();

            return PartialView("_UserStoresViewPartial", available);
        }


        // GET: UserStores

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
                          pa.Module.ModuleTitle == "User Stores" &&  // Adjust the module name as needed
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
                        var query1 = _context.tbl_user_stores
                            .Include(u => u.Store)
                            .Include(u => u.User)
                            .Where(x => x.User.UserADLogin.Contains(searchString) || x.Store.StoreName.Contains(searchString))
                            .ToPagedList(pageIndex, pageSize);
                        return View(query1);
                    }
                    var assetManagementContext = _context.tbl_user_stores.Include(u => u.Store).Include(u => u.User).ToPagedList(pageIndex, pageSize);
                    return View(assetManagementContext);
                }
            }

            return RedirectToAction("Index", "Home");

        }

        public async Task<IActionResult> UserStoreViews(int? page, string searchString)
        {

                int pageSize = 10;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

                if (!string.IsNullOrEmpty(searchString))
                {
                    var query1 = _context.tbl_user_stores
                        .Include(u => u.Store)
                        .Include(u => u.User)
                        .Where(x => x.User.UserADLogin.Contains(searchString) || x.Store.StoreName.Contains(searchString))
                        .ToPagedList(pageIndex, pageSize);
                    return View(query1);
                }
                var assetManagementContext = _context.tbl_user_stores.Include(u => u.Store).Include(u => u.User).ToPagedList(pageIndex, pageSize);
                return View(assetManagementContext);

        }

        // GET: UserStores/Create
        public IActionResult Create()
        {
            ViewData["StoreCode"] = new SelectList(_context.tbl_ictams_stores, "Store_code", "Store_code");
            ViewData["UserCode"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserFullName");
            return View();
        }

        // POST: UserStores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserCode,StoreCode")] UserStore userStore)
        {
            if(userStore != null)
            {
                if (UserStoreExists(userStore))
                {
                    ModelState.AddModelError("UserCode", "Sorry, but it's already exists");
                    TempData["ErrorMessage"] = "Duplicate stores are not allowed!";
                    return RedirectToAction(nameof(PartialStoreView));
                }
                if (userStore.UserCode == null || userStore.StoreCode == null)
                {
                    ModelState.AddModelError("ProfileDescription", "Sorry, but it's required");
                    TempData["ErrorMessage1"] = "Please select a store!";
                    return RedirectToAction(nameof(PartialStoreView));
                }
                else
                {
                    _context.Add(userStore);
                    await _context.SaveChangesAsync();
                    // ...
                    TempData["SuccessNotification"] = "Successfully added a new user store!";
                    // ...
                    return RedirectToAction(nameof(Index));
                }
            }
            
            ViewData["UserCode"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserFullName");
            var userStore1 = _context.tbl_ictams_stores.Where(u => u.Status.status_code == "AC").Include(u => u.Status);
            UserStoreViewModel userStoreViewModel = new()
            {
                StoreList = await userStore1.ToListAsync()
            };

            return View("PartialStoreView", userStoreViewModel);

        }

        // USER STORE PARTIAL VIEW

        public async Task<IActionResult> PartialStoreView(string userCODE)
        {
            //ViewData["UserCode"] = new SelectList(_context.tbl_ictams_users.Where(x => x.UserStatus == "AC"), "UserCode", "UserFullName");
            //var userStore = _context.tbl_ictams_stores.Where(u => u.Status.status_code == "AC").Include(u => u.Status);

            //UserStoreViewModel userStoreViewModel = new()
            //{
            //    StoreList = await userStore.ToListAsync()
            //};

            ViewData["StoreCode"] = new SelectList(_context.tbl_ictams_stores, "Store_code", "Store_code");
            ViewData["UserCode"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserFullName");

            return View("PartialStoreView");
        }

        // GET: UserStores/Delete/5
        public async Task<IActionResult> Delete(string userCode, string storeCode)
        {
            if (userCode == null || storeCode == null)
            {
                return NotFound();
            }

            var userStore = await _context.tbl_user_stores
                .FirstOrDefaultAsync(us => us.UserCode == userCode && us.StoreCode == storeCode);

            if (userStore == null)
            {
                return NotFound();
            }

            _context.tbl_user_stores.Remove(userStore);
            await _context.SaveChangesAsync();

            // ...
            TempData["SuccessNotification"] = "Successfully remove a user store!";
            // ...
            return RedirectToAction(nameof(Index));
        
    }


        private bool UserStoreExists(UserStore userStore)
        {
          return (_context.tbl_user_stores?.Any(e => e.UserCode == userStore.UserCode && e.StoreCode == userStore.StoreCode)).GetValueOrDefault();
        }
    }
}
