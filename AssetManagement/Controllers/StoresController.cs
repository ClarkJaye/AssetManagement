using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Data;
using AssetManagement.Models;
using AssetManagement.Utility;

namespace AssetManagement.Controllers
{
    public class StoresController : BaseController
    {
        private readonly AssetManagementContext _context;

        public StoresController(AssetManagementContext context)
         : base(context)
        {
            _context = context;
        }

        // GET: Vendors  
        public async Task<IActionResult> StoresPartialView()
        {
            var assetManagementContext = _context.tbl_ictams_stores.Where(x => x.StoreStatus == "AC").Include(v => v.Status);
            return View(await assetManagementContext.ToListAsync());
        }

        public async Task<IActionResult> Index()
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

                var hasOpenAccess = await _context.tbl_ictams_profileaccess.AnyAsync(pa => pa.OpenAccess == "Y" &&
                    pa.Module.ModuleTitle == "Stores" &&  
                    pa.ProfileId == userProfile.Value);
                if (!hasOpenAccess)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    await FindStatus();
                    var myData = HttpContext.Session.GetString("UserName");
                    var lSM_PNContext = _context.tbl_ictams_stores.Where(dept => dept.StoreStatus == "AC").Include(d => d.Status);
                    return View(await lSM_PNContext.ToListAsync());
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> StoreViews()
        {
            await FindStatus();
            var myData = HttpContext.Session.GetString("UserName");
            var lSM_PNContext = _context.tbl_ictams_stores.Where(Vendor => Vendor.StoreStatus == "AC").Include(v => v.Status);
            return View(await lSM_PNContext.ToListAsync());
        }

        public IActionResult Inactive()
        {
            var myData = HttpContext.Session.GetString("UserName");
            ViewBag.showprofile = myData;
            var InactiveDate = _context.tbl_ictams_stores
                .Where(str => str.StoreStatus == "IN")
                .ToList();
            return View(InactiveDate);
        }
        public async Task<IActionResult> Activate(string? id)
        {
            var userr = HttpContext.Session.GetString("UserName");
            var str = await _context.tbl_ictams_stores.FindAsync(id);
            if (str == null)
            {
                return NotFound();
            }
            str.SUpdateby = userr;
            str.DateUpdated = DateTime.Now;
            str.StoreStatus = "AC"; // Set the status to "Active"

            _context.Entry(str).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            TempData["SuccessNotification"] = "Successfully Retreived!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Vendors/Create
        public IActionResult Create()
        {
            return View();
        }

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Store_code,StoreName,StoreStatus")] Store str)
        {

            var findDept = await _context.tbl_ictams_stores.Where(x => x.Store_code == str.Store_code).FirstOrDefaultAsync();
            if (findDept != null)
            {
                TempData["AlertMessage"] = "Store already exists!";
                return RedirectToAction(nameof(Index));
            }

            var userrr = HttpContext.Session.GetString("UserName");

            str.Store_code = str.Store_code;
            str.StoreName = str.StoreName.ToUpper();
            str.StoreStatus = "AC";
            str.DateCreated = DateTime.Now;
            str.SCreatedby = userrr;
            _context.Add(str);
            await _context.SaveChangesAsync();
            TempData["SuccessNotification"] = "Store updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Vendors/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null || _context.tbl_ictams_stores == null)
            {
                return NotFound();
            }

            var str = await _context.tbl_ictams_stores.FindAsync(id);
            if (str == null)
            {
                return NotFound();
            }
            return View(str);
        }
        // POST: Departments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Store_code,StoreName,StoreStatus")] Store str)
        {
            var username = HttpContext.Session.GetString("UserName");

            if (!StoreExists(str.Store_code))
            {
                TempData["ErrorMessage"] = "Store not found!";
                return NotFound();
            }

            var existingDept = await _context.tbl_ictams_stores.FindAsync(str.Store_code);
            if (existingDept == null)
            {
                TempData["ErrorMessage"] = "Store not found!";
                return NotFound();
            }

            try
            {
                // Update only the fields that should be editable
                existingDept.StoreName = str.StoreName?.ToUpper();
                existingDept.SUpdateby = username;
                existingDept.DateUpdated = DateTime.Now;

                _context.Update(existingDept);
                await _context.SaveChangesAsync();
                TempData["SuccessNotification"] = "Store updated successfully!";
            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["ErrorMessage"] = "Error updating Store due to concurrency issue!";
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Stores/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null || _context.tbl_ictams_stores == null)
            {
                return NotFound();
            }
            var dept = await _context.tbl_ictams_stores
                .FirstOrDefaultAsync(m => m.Store_code == id);
            if (dept == null)
            {
                return NotFound();
            }
            return View(dept);
        }

        // POST: Stores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var userrr = HttpContext.Session.GetString("UserName");
            if (_context.tbl_ictams_stores == null)
            {
                return Problem("Entity set 'AssetManagementContext.Store'  is null.");
            }
            var str = await _context.tbl_ictams_stores.FindAsync(id);
            if (str != null)
            {
                str.StoreStatus = "IN";
                str.SUpdateby = userrr;
                str.DateUpdated = DateTime.Now;
                _context.tbl_ictams_stores.Update(str);
            }
            await _context.SaveChangesAsync();
            TempData["SuccessNotification"] = "Store Deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool StoreExists(string id)
        {
            return (_context.tbl_ictams_stores?.Any(e => e.Store_code == id)).GetValueOrDefault();
        }
    }
}

