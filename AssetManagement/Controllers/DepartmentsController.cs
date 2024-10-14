using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Data;
using AssetManagement.Models;
using AssetManagement.Utility;

namespace AssetManagement.Controllers
{
    public class DepartmentsController : BaseController
    {
        private readonly AssetManagementContext _context;

        public DepartmentsController(AssetManagementContext context)
         : base(context)
        {
            _context = context;
        }

        // GET: Vendors  
        public async Task<IActionResult> DepartmentPartialView()
        {
            var assetManagementContext = _context.tbl_ictams_department.Where(x => x.Dept_status == "AC").Include(v => v.Status);
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
                    pa.Module.ModuleTitle == "Departments" &&  
                    pa.ProfileId == userProfile.Value);
                if (!hasOpenAccess)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    await FindStatus();
                    var myData = HttpContext.Session.GetString("UserName");
                    var lSM_PNContext = _context.tbl_ictams_department.Where(dept => dept.Dept_status == "AC").Include(d => d.Status);
                    return View(await lSM_PNContext.ToListAsync());
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> DepartmentViews()
        {
            await FindStatus();
            var myData = HttpContext.Session.GetString("UserName");
            var lSM_PNContext = _context.tbl_ictams_department.Where(Vendor => Vendor.Dept_status == "AC").Include(v => v.Status);
            return View(await lSM_PNContext.ToListAsync());
        }

        public IActionResult Inactive()
        {
            var myData = HttpContext.Session.GetString("UserName");
            ViewBag.showprofile = myData;
            var InactiveDate = _context.tbl_ictams_department
                .Where(dept => dept.Dept_status == "IN")
                .ToList();
            return View(InactiveDate);
        }
        public async Task<IActionResult> Activate(int? id)
        {
            var userr = HttpContext.Session.GetString("UserName");
            var dept = await _context.tbl_ictams_department.FindAsync(id);
            if (dept == null)
            {
                return NotFound();
            }
            dept.DUpdateby = userr;
            dept.DateUpdated = DateTime.Now;
            dept.Dept_status = "AC"; // Set the status to "Active"

            _context.Entry(dept).State = EntityState.Modified;
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
        public async Task<IActionResult> Create([Bind("Dept_code,Dept_name,Dept_status")] Department dept)
        {

            var findDept = await _context.tbl_ictams_department.Where(x => x.Dept_code == dept.Dept_code).FirstOrDefaultAsync();
            if (findDept != null)
            {
                TempData["SuccessNotification"] = "Department already exists!";
                return RedirectToAction(nameof(Index));
            }

            var userrr = HttpContext.Session.GetString("UserName");

            var paramCode = await _context.tbl_ictams_parameters.Where(p => p.parm_code == "dept_id").MaxAsync(p => p.parm_value);
            var newparamCode = paramCode + 1;

            var param = await _context.tbl_ictams_parameters.FirstOrDefaultAsync(p => p.parm_code == "dept_id");
            param.parm_value = newparamCode;


            dept.Dept_code = newparamCode;
            dept.Dept_name = dept.Dept_name.ToUpper();
            dept.Dept_status = "AC";
            dept.DateCreated = DateTime.Now;
            dept.DCreatedby = userrr;
            _context.Add(dept);
            await _context.SaveChangesAsync();
            TempData["SuccessNotification"] = "Department updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Vendors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.tbl_ictams_department == null)
            {
                return NotFound();
            }

            var dept = await _context.tbl_ictams_department.FindAsync(id);
            if (dept == null)
            {
                return NotFound();
            }
            return View(dept);
        }
        // POST: Departments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Dept_code,Dept_name,Dept_status")] Department dept)
        {
            var username = HttpContext.Session.GetString("UserName");

            if (!DepartmentExists(dept.Dept_code))
            {
                TempData["ErrorMessage"] = "Department not found!";
                return NotFound();
            }

            var existingDept = await _context.tbl_ictams_department.FindAsync(dept.Dept_code);
            if (existingDept == null)
            {
                TempData["ErrorMessage"] = "Department not found!";
                return NotFound();
            }

            try
            {
                // Update only the fields that should be editable
                existingDept.Dept_name = dept.Dept_name?.ToUpper();
                existingDept.DUpdateby = username;
                existingDept.DateUpdated = DateTime.Now;

                _context.Update(existingDept);
                await _context.SaveChangesAsync();
                TempData["SuccessNotification"] = "Department updated successfully!";
            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["ErrorMessage"] = "Error updating department due to concurrency issue!";
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Vendors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.tbl_ictams_department == null)
            {
                return NotFound();
            }
            var dept = await _context.tbl_ictams_department
                .FirstOrDefaultAsync(m => m.Dept_code == id);
            if (dept == null)
            {
                return NotFound();
            }
            return View(dept);
        }

        // POST: Vendors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userrr = HttpContext.Session.GetString("UserName");
            if (_context.tbl_ictams_department == null)
            {
                return Problem("Entity set 'AssetManagementContext.Departments'  is null.");
            }
            var dept = await _context.tbl_ictams_department.FindAsync(id);
            if (dept != null)
            {
                dept.Dept_status = "IN";
                dept.DUpdateby = userrr;
                dept.DateUpdated = DateTime.Now;
                _context.tbl_ictams_department.Update(dept);
            }
            await _context.SaveChangesAsync();
            TempData["SuccessNotification"] = "Department Deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool DepartmentExists(int id)
        {
            return (_context.tbl_ictams_department?.Any(e => e.Dept_code == id)).GetValueOrDefault();
        }
    }
}

