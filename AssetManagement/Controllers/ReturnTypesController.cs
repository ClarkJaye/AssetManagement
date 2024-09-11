using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Data;
using AssetManagement.Models;
using AssetManagement.Utility;
using System.Drawing.Drawing2D;

namespace AssetManagement.Controllers
{
    public class ReturnTypesController : BaseController
    {
        private readonly AssetManagementContext _context;

        public ReturnTypesController(AssetManagementContext context)
            : base(context) // Pass the 'context' dependency to the base constructor
        {
            _context = context;
        }

        // GET: ReturnTypes
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

                var hasOpenAccess = await _context.tbl_ictams_profileaccess
          .AnyAsync(pa => pa.OpenAccess == "Y" &&
                          pa.Module.ModuleTitle == "Return Types" &&  // Adjust the module name as needed
                          pa.ProfileId == userProfile.Value);
                if (!hasOpenAccess)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var assetManagementContext = _context.tbl_ictams_returntype.Where(x => x.Status.status_code == "AC").Include(r => r.Status).Include(r => r.UserCreated).Include(r => r.UserUpdated);
                    return View(await assetManagementContext.ToListAsync());
                }
            }

            return RedirectToAction("Index", "Home");

        }

        public async Task<IActionResult> ReturnPartialView()
        {
            var assetManagementContext = _context.tbl_ictams_returntype.Where(x => x.Status.status_code == "AC").Include(r => r.Status).Include(r => r.UserCreated).Include(r => r.UserUpdated);
            return View(await assetManagementContext.ToListAsync());
        }

        // GET: ReturnTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.tbl_ictams_returntype == null)
            {
                return NotFound();
            }

            var returnType = await _context.tbl_ictams_returntype
                .Include(r => r.Status)
                .Include(r => r.UserCreated)
                .Include(r => r.UserUpdated)
                .FirstOrDefaultAsync(m => m.TypeID == id);
            if (returnType == null)
            {
                return NotFound();
            }

            return View(returnType);
        }

        // GET: ReturnTypes/Create
        public IActionResult Create()
        {
            ViewData["Return_Status"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_code");
            ViewData["CreatedBy"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode");
            ViewData["RTUpdated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode");
            return View();
        }

        // POST: ReturnTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>
        Create([Bind("TypeID,Description,Return_Inv,Return_Status,CreatedBy,DateCreated,RTUpdated,DateUpdated")] ReturnType returnType)
        {

            bool descriptionExists = await _context.tbl_ictams_returntype.AnyAsync(x => x.Description == returnType.Description);
            if (descriptionExists)
            {
                TempData["ErrorMessage"] = "Description already exists. Please enter a different description!";
                return RedirectToAction(nameof(Index));
            }

            var paramCode = await _context.tbl_ictams_parameters.Where(p => p.parm_code == "ret_id").MaxAsync(p => p.parm_value);
            var newparamCode = paramCode + 1;

            var param = await _context.tbl_ictams_parameters.FirstOrDefaultAsync(p => p.parm_code == "ret_id");
            param.parm_value = newparamCode;


            var userrr = HttpContext.Session.GetString("UserName");

            returnType.Description = returnType.Description.ToUpper();
            returnType.Return_Inv = returnType.Return_Inv.ToUpper();
            returnType.TypeID = newparamCode;
            returnType.DateCreated = DateTime.Now;
            returnType.CreatedBy = userrr;
            _context.Add(returnType);
                await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully added a new return type!";
            // ...
            return RedirectToAction(nameof(Index));
        }

        // GET: ReturnTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.tbl_ictams_returntype == null)
            {
                return NotFound();
            }

            var returnType = await _context.tbl_ictams_returntype.FindAsync(id);
            if (returnType == null)
            {
                return NotFound();
            }
            return View(returnType);
        }
    

        // POST: ReturnTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TypeID,Description,Return_Inv,Return_Status,CreatedBy,DateCreated,RTUpdated,DateUpdated")] ReturnType returnType)
        {
       
    
            var userrr = HttpContext.Session.GetString("UserName");
  
                    returnType.RTUpdated = userrr;
                    returnType.DateUpdated = DateTime.Now;
                    _context.Update(returnType);
                    await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully edit a return type!";
            // ...
            return RedirectToAction(nameof(Index));

        }


        // GET: ReturnTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.tbl_ictams_returntype == null)
            {
                return NotFound();
            }

            var returnType = await _context.tbl_ictams_returntype
                .Include(r => r.Status)
                .Include(r => r.UserCreated)
                .Include(r => r.UserUpdated)
                .FirstOrDefaultAsync(m => m.TypeID == id);
            if (returnType == null)
            {
                return NotFound();
            }

            return View(returnType);
        }

        // POST: ReturnTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.tbl_ictams_returntype == null)
            {
                return Problem("Entity set 'AssetManagementContext.tbl_ictams_returntype'  is null.");
            }
            var returnType = await _context.tbl_ictams_returntype.FindAsync(id);
            if (returnType != null)
            {
                _context.tbl_ictams_returntype.Remove(returnType);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReturnTypeExists(int id)
        {
          return (_context.tbl_ictams_returntype?.Any(e => e.TypeID == id)).GetValueOrDefault();
        }
    }
}
