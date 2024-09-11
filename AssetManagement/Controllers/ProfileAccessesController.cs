using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Data;
using AssetManagement.Models;
using Microsoft.AspNetCore.Authorization;
using AssetManagement.Utility;

namespace AssetManagement.Controllers
{
  
    public class ProfileAccessesController : BaseController
    {
        private readonly AssetManagementContext _context;

        public ProfileAccessesController(AssetManagementContext context)
        : base(context) // Pass the 'context' dependency to the base constructor
        {
            _context = context;
        }

        // GET: ProfileAccesses
        public async Task<IActionResult> Index(int? id)
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
                          pa.Module.ModuleTitle == "Profile Accesses" &&  // Adjust the module name as needed
                          pa.ProfileId == userProfile.Value);
                if (!hasOpenAccess)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var DBContext = _context.tbl_ictams_profileaccess.Include(p => p.CreatedBy).Include(p => p.Module).ThenInclude(m => m.Category).Include(p => p.Profile).Include(p => p.UpdatedBy).Where(p => p.ProfileId == id).Where(x => x.Module.ModuleStatus == "AC");

                    // Retrieve the ProfileName from session
                    string profileName = HttpContext.Session.GetString("ProfileName");

                    // Pass the ProfileName to the view using ViewData
                    ViewData["ProfileName"] = profileName;

                    return View(await DBContext.ToListAsync());
                }
            }

            return RedirectToAction("Index", "Home");
            
        }

        public async Task<IActionResult> Accesslist()
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

            var DBContext = _context.tbl_ictams_profileaccess.Where(x=>x.OpenAccess == "Y").Include(p => p.Module).ThenInclude(m => m.Category).Include(p => p.Profile);
            return View(await DBContext.ToListAsync());
        }

        [HttpPost]
        public IActionResult SetProfileNameSession(string profileName)
        {
            HttpContext.Session.SetString("ProfileName", profileName);
            return Ok();
        }


        // GET: RequestTypes
        public async Task<IActionResult> ModuleShow(int profileid)
        {
            var lSM_PNContext = _context.tbl_ictams_modules.Include(User => User.CreatedBy).Include(Status => Status.Status).Include(User => User.UpdatedBy);
            return PartialView("ModuleShow",await lSM_PNContext.ToListAsync());
        }
        // GET: PromoType
        public async Task<IActionResult> ProfileShow()
        {

            var lSM_PNContext = _context.tbl_ictams_profiles.Include(p => p.CreatedBy).Include(p => p.Status).Include(p => p.UpdatedBy);
            return PartialView("ProfileShow",await lSM_PNContext.ToListAsync());
        }
        

        // GET: ProfileAccesses/Create
        public IActionResult Create()
        {
            ViewData["UserCreated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserFullName");
            ViewData["ModuleId"] = new SelectList(_context.tbl_ictams_modules, "ModuleId", "Module Title");
            ViewData["ProfileId"] = new SelectList(_context.tbl_ictams_profiles, "ProfileId", "ProfileDescription");
            ViewData["UserUpdated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserFullName");
            return PartialView("Create");
        }

        // POST: ProfileAccesses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProfileId,ModuleId,OpenAccess,UserCreated,DateCreated,UserUpdated,DateUpdated")] ProfileAccess profileAccess)
        {
            var userrr = HttpContext.Session.GetString("UserName");

            if (ModelState.IsValid)
            {
                
                profileAccess.UserCreated = userrr;
                _context.Add(profileAccess);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserCreated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode", profileAccess.UserCreated);
            ViewData["ModuleId"] = new SelectList(_context.tbl_ictams_modules, "ModuleId", "ModuleId", profileAccess.ModuleId);
            ViewData["ProfileId"] = new SelectList(_context.tbl_ictams_profiles, "ProfileId", "ProfileDescription", profileAccess.ProfileId);
            ViewData["UserUpdated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode", profileAccess.UserUpdated);
            return View(profileAccess);
        }

        // GET: ProfileAccesses/Edit/5
        public async Task<IActionResult> Edit(int? id,int? id2)
        {
            if (id == null || _context.tbl_ictams_profileaccess == null)
            {
                return NotFound();
            }

            var profileAccess = await _context.tbl_ictams_profileaccess.FindAsync(id,id2);
            if (profileAccess == null)
            {
                return NotFound();
            }
            ViewData["UserCreated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserFullName", profileAccess.UserCreated);
            ViewData["ModuleId"] = new SelectList(_context.tbl_ictams_modules, "ModuleId", "Module Title", profileAccess.ModuleId);
            ViewData["ProfileId"] = new SelectList(_context.tbl_ictams_profiles, "ProfileId", "ProfileDescription", profileAccess.ProfileId);
            ViewData["UserUpdated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserFullName", profileAccess.UserUpdated);
            return PartialView("Edit",profileAccess);
        }

        // GET: ProfileAccesses/Delete/5
        public async Task<IActionResult> Delete(int? id, int? id2)
        {
            if (id == null || _context.tbl_ictams_profileaccess == null)
            {
                return NotFound();
            }

            var profileAccess = await _context.tbl_ictams_profileaccess
                .Include(p => p.CreatedBy)
                .Include(p => p.Module)
                .Include(p => p.Profile)
                .Include(p => p.UpdatedBy)
                .FirstOrDefaultAsync(m => m.ProfileId == id && m.ModuleId == id2);
            if (profileAccess == null)
            {
                return NotFound();
            }

            return View(profileAccess);
        }

        // POST: ProfileAccesses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int id2)
        {
            var userrr = HttpContext.Session.GetString("UserName");

            if (_context.tbl_ictams_profileaccess == null)
            {
                return Problem("Entity set 'LSM_PNContext.tbl_lsm_profileaccess'  is null.");
            }
            var profileAccess = await _context.tbl_ictams_profileaccess.FindAsync(id, id2);
            if (profileAccess != null)
            {
                if (profileAccess.OpenAccess == "N")
                {
                    profileAccess.OpenAccess = "Y"; // Update to "Y" if it was previously "N"
                }
                else if (profileAccess.OpenAccess == "Y")
                {
                    profileAccess.OpenAccess = "N"; // Update to "N" if it was previously "Y" or any other value
                }

                profileAccess.UserUpdated = userrr;
                profileAccess.DateUpdated = DateTime.Now;

                _context.tbl_ictams_profileaccess.Update(profileAccess);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { id = id });

        }

        private bool ProfileAccessExists(int id)
        {
          return (_context.tbl_ictams_profileaccess?.Any(e => e.ProfileId == id)).GetValueOrDefault();
        }
    }
}
