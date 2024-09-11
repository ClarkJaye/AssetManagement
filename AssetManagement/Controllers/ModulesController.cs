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

namespace LSM_PN.Controllers
{
    public class ModulesController : BaseController
    {
        private readonly AssetManagementContext _context;

        public ModulesController(AssetManagementContext context)
        : base(context) // Pass the 'context' dependency to the base constructor
        {
            _context = context;
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

                var hasOpenAccess = await _context.tbl_ictams_profileaccess
          .AnyAsync(pa => pa.OpenAccess == "Y" &&
                          pa.Module.ModuleTitle == "Modules" &&  // Adjust the module name as needed
                          pa.ProfileId == userProfile.Value);
                if (!hasOpenAccess)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var lSM_PNContext = _context.tbl_ictams_modules.Where(p => p.Status.status_code == "AC").Include(Category => Category.Category).Include(User => User.CreatedBy).Include(Status => Status.Status).Include(User => User.UpdatedBy);
                    return View(await lSM_PNContext.ToListAsync());
                }
            }

            return RedirectToAction("Index", "Home");

        }

        public async Task<IActionResult> Inactive()
        {   
            var lSM_PNContext = _context.tbl_ictams_modules.Where(p => p.Status.status_code == "IN").Include(Category => Category.Category).Include(User => User.CreatedBy).Include(Status => Status.Status).Include(User => User.UpdatedBy);
            return View(await lSM_PNContext.ToListAsync());
        }




        // GET: Modules/Create
        public IActionResult Create()
        {
            ViewData["ModuleCreated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode");
            ViewData["ModuleStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_name");
            ViewData["ModuleUpdated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode");
            ViewData["ModuleCategory"] = new SelectList(_context.tbl_ictams_category, "category_id", "category_name");
            return View();
        }

        // POST: Modules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ModuleTitle,ModuleCategory,ModuleStatus,ModuleCreated,ModuleDtCreated,ModuleUpdated,ModuleDtUpdated")] Module @module)
        {
            bool titleExists = await _context.tbl_ictams_modules.AnyAsync(x => x.ModuleTitle == @module.ModuleTitle);
            if (titleExists)
            {
                TempData["ErrorMessage"] = "This title already exists. Please enter a different title for this module";
                return RedirectToAction(nameof(Index));
            }
            int nextModuleId = 1;
			var maxModuleId = await _context.tbl_ictams_modules.MaxAsync(c => (int?)c.ModuleId);
			if (maxModuleId != null)
			{
				nextModuleId = maxModuleId.Value + 1;
			}
            var ucode = HttpContext.Session.GetString("UserName");
            @module.ModuleCreated = ucode;
            @module.ModuleDtCreated = DateTime.Now;
            @module.ModuleId = nextModuleId;
			_context.Add(@module);
			await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully added a new module!";
            // ...
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.tbl_ictams_modules == null)
            {
                return NotFound();
            }

            var @module = await _context.tbl_ictams_modules.FindAsync(id);
            if (@module == null)
            {
                return NotFound();
            }
            ViewData["ModuleCategory"] = new SelectList(_context.tbl_ictams_category, "category_id", "category_name", @module.ModuleCategory);
            ViewData["ModuleCreated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode", @module.ModuleCreated);
            ViewData["ModuleStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_name", @module.ModuleStatus);
            ViewData["ModuleUpdated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode", @module.ModuleUpdated);

            return View(@module);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ModuleId,ModuleTitle,ModuleStatus,ModuleCategory,ModuleCreated,ModuleDtCreated,ModuleUpdated,ModuleDtUpdated")] Module @module)
        {
            if (id != @module.ModuleId)
            {
                return NotFound();
            }

            
                try
                {
                    var ftable = await _context.tbl_ictams_modules.FirstOrDefaultAsync(x => x.ModuleId == @module.ModuleId);
                    var ucode = HttpContext.Session.GetString("UserName");
                    ftable.ModuleStatus = @module.ModuleStatus;
                    ftable.ModuleTitle = @module.ModuleTitle;
                    ftable.ModuleCategory = @module.ModuleCategory;
                    ftable.ModuleUpdated = ucode;
                    ftable.ModuleDtUpdated = DateTime.Now;

                    await _context.SaveChangesAsync();
                // ...
                TempData["SuccessNotification"] = "Successfully edit a module!";
                // ...
                return RedirectToAction(nameof(Index));
            }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModuleExists(@module.ModuleId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                
                
            }
			return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> DeleteAsEdit(int[] selectedIds)
        {
            ViewData["ModuleStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_name");
            foreach (int id in selectedIds)
            {
                var user = await _context.tbl_ictams_modules.FindAsync(id);
                if (user != null)
                {
                    // Check if the selected profile status exists in the tbl_lsm_status table
                    var status = await _context.tbl_ictams_status.FindAsync(user.ModuleStatus);

                        if (status == null)
                        {
                            ModelState.AddModelError("", $"Profile status '{user.ModuleStatus}' does not exist.");
                        }

                    // Update the profile
                    var ucode = HttpContext.Session.GetString("UserName");
                    user.ModuleUpdated = ucode;
                    user.ModuleStatus = "IN";
                    user.ModuleDtUpdated = DateTime.Now;

                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    // ...
                    TempData["SuccessNotification"] = "Successfully delete a module!";
                    // ...
                    return RedirectToAction(nameof(Index));

                }
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Retrieve(int[] selectedIds)
        {
            ViewData["ModuleStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_name");
            foreach (int id in selectedIds)
            {
                var module = await _context.tbl_ictams_modules.FindAsync(id);
                if (module != null)
                {
                    // Check if the selected profile status exists in the tbl_lsm_status table
                    var status = await _context.tbl_ictams_status.FindAsync(module.ModuleStatus);
                    if (status == null)
                    {
                        ModelState.AddModelError("", $"Profile status '{module.ModuleStatus}' does not exist.");
                    }

                    // Update the profile
                    var ucode = HttpContext.Session.GetString("UserName");
                    module.ModuleUpdated = ucode;
                    module.ModuleStatus = "AC";
                    module.ModuleDtUpdated = DateTime.Now;

                    _context.Update(module);
                    await _context.SaveChangesAsync();
                    // ...
                    TempData["SuccessNotification"] = "Successfully retrieve a deleted module!";
                    // ...
                    return RedirectToAction(nameof(Index));

                }
            }

            return RedirectToAction("Index");
        }



        private bool ModuleExists(int id)
        {
          return (_context.tbl_ictams_modules?.Any(e => e.ModuleId == id)).GetValueOrDefault();
        }
    }
}
