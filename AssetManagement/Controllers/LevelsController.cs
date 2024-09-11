using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Data;
using AssetManagement.Models;
using System.Drawing.Drawing2D;
using AssetManagement.Utility;

namespace AssetManagement.Controllers
{
    public class LevelsController : BaseController
    {
        private readonly AssetManagementContext _context;

        public LevelsController(AssetManagementContext context)
        : base(context) // Pass the 'context' dependency to the base constructor
        {
            _context = context;
        }

        public async Task<IActionResult> LevelPartialView()
        {   
            var myData = HttpContext.Session.GetString("name");
            var lSM_PNContext = _context.tbl_ictams_level.Where(Level => Level.LevelStatus == "AC");
            return View(await lSM_PNContext.ToListAsync());
        }

        // GET: Levels
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
                          pa.Module.ModuleTitle == "Levels" &&  // Adjust the module name as needed
                          pa.ProfileId == userProfile.Value);
                if (!hasOpenAccess)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var myData = HttpContext.Session.GetString("name");
                    var lSM_PNContext = _context.tbl_ictams_level.Where(Level => Level.LevelStatus == "AC");
                    return View(await lSM_PNContext.ToListAsync());
                }
            }

            return RedirectToAction("Index", "Home");
        }
        public IActionResult InactiveLevels()
        {
            var myData1 = HttpContext.Session.GetString("name");
            ViewBag.showprofile = myData1;
            var myData = HttpContext.Session.GetString("name");
            ViewBag.showprofile = myData;
            var inactiveLevels = _context.tbl_ictams_level
                .Where(level => level.LevelStatus == "IN")
                .ToList();
            return View(inactiveLevels);
        }
        public async Task<IActionResult> Activate(int? id)
        {
            var userr = HttpContext.Session.GetString("name");
            var level = await _context.tbl_ictams_level.FindAsync(id);
            if (level == null)
            {
                return NotFound();
            }
            level.LevelUpdatedBy = userr;
            level.LevelUpdatedDate = DateTime.Now;
            level.LevelStatus = "AC"; // Set the status to "Active"

            _context.Entry(level).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully retrieve a deleted levels!";
            // ...
            return RedirectToAction(nameof(Index));
        }

        // GET: Levels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.tbl_ictams_level == null)
            {
                return NotFound();
            }

            var level = await _context.tbl_ictams_level
                .FirstOrDefaultAsync(m => m.LevelId == id);
            if (level == null)
            {
                return NotFound();
            }

            return View(level);
        }

        // GET: Levels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Levels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LevelId,LevelDescription,LevelStatus,LevelCreatedBy,LevelCreatedDate,LevelUpdatedBy,LevelUpdatedDate")] Level level)
        {
            var userrr = HttpContext.Session.GetString("name");
			bool descriptionExists = await _context.tbl_ictams_level.AnyAsync(x => x.LevelDescription == level.LevelDescription);
			if (descriptionExists)
			{
				TempData["ErrorMessage"] = "Description already exists. Please enter a different description!";
				return RedirectToAction(nameof(Index));
			}

			var paramCode = await _context.tbl_ictams_parameters.Where(p => p.parm_code == "level_id").MaxAsync(p => p.parm_value);
                var newparamCode = paramCode + 1;

                var param = await _context.tbl_ictams_parameters.FirstOrDefaultAsync(p => p.parm_code == "level_id");
                param.parm_value = newparamCode;

                level.LevelDescription = level.LevelDescription.ToUpper();
                level.LevelStatus = "AC";
                level.LevelId = newparamCode;
                level.LevelCreatedDate = DateTime.Now;
                level.LevelCreatedBy = userrr;
                _context.Add(level);
                await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully added a new levels!";
            // ...
            return RedirectToAction(nameof(Index));
        }

        // GET: Levels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.tbl_ictams_level == null)
            {
                return NotFound();
            }

            var level = await _context.tbl_ictams_level.FindAsync(id);
            if (level == null)
            {
                return NotFound();
            }
            return View(level);
        }

        // POST: Levels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LevelId,LevelDescription,LevelStatus,LevelCreatedBy,LevelCreatedDate,LevelUpdatedBy,LevelUpdatedDate")] Level level)
        {
            var userrr = HttpContext.Session.GetString("name");
            bool descriptionExists = await _context.tbl_ictams_level.AnyAsync(x => x.LevelDescription == level.LevelDescription);
            if (descriptionExists)
            {
                TempData["ErrorMessage"] = "Description already exists. Please enter a different description!";
                return RedirectToAction(nameof(Index));
            }
            if (ModelState.IsValid)
            {
                try
                {
                    level.LevelDescription = level.LevelDescription.ToUpper();
                    level.LevelUpdatedBy = userrr;
                    level.LevelUpdatedDate = DateTime.Now;
                    _context.Update(level);
                    await _context.SaveChangesAsync();
                    // ...
                    TempData["SuccessNotification"] = "Successfully edit a levels!";
                    // ...
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LevelExists(level.LevelId))
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
            return View(level);
        }

        // GET: Levels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.tbl_ictams_level == null)
            {
                return NotFound();
            }

            var level = await _context.tbl_ictams_level
                .FirstOrDefaultAsync(m => m.LevelId == id);
            if (level == null)
            {
                return NotFound();
            }

            return View(level);
        }

        // POST: Levels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var findUse = await _context.tbl_ictams_laptopinv.Where(x=>x.LTLevel == id).FirstOrDefaultAsync();
            if (findUse != null)
            {
                TempData["AlertMessage"] = "Cannot be deleted. It is already in use!";
                return RedirectToAction(nameof(Index));
            }


            var userrr = HttpContext.Session.GetString("name");
            if (_context.tbl_ictams_level == null)
            {
                return Problem("Entity set 'AssetManagementContext.Level'  is null.");
            }
            var level = await _context.tbl_ictams_level.FindAsync(id);
            if (level != null)
            {
                level.LevelStatus = "IN";
                level.LevelUpdatedBy = userrr;
                level.LevelUpdatedDate = DateTime.Now;
                _context.tbl_ictams_level.Update(level);
            }
            
            await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully deleted a levels!";
            // ...
            return RedirectToAction(nameof(Index));
        }

        private bool LevelExists(int id)
        {
          return (_context.tbl_ictams_level?.Any(e => e.LevelId == id)).GetValueOrDefault();
        }
    }
}
