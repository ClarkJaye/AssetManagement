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
    public class HardDisksController :BaseController
    {
        private readonly AssetManagementContext _context;

        public HardDisksController(AssetManagementContext context)
        : base(context) // Pass the 'context' dependency to the base constructor
        {
            _context = context;
        }
        public async Task<IActionResult> HardDiskPartialView()
        {
            var myData = HttpContext.Session.GetString("name");
            var lSM_PNContext = _context.tbl_ictams_hardisk.Where(HD => HD.HDStatus == "AC").Include(CA => CA.Capacity);
            return View(await lSM_PNContext.ToListAsync());
        }
        // GET: HardDisks
        public async Task<IActionResult> Index()
        {
            int? userProfile = HttpContext.Session.GetInt32("UserProfile");
            if (userProfile.HasValue)
            {

                var hasOpenAccess = await _context.tbl_ictams_profileaccess
          .AnyAsync(pa => pa.OpenAccess == "Y" &&
                          pa.Module.ModuleTitle == "Hard Disks" &&  // Adjust the module name as needed
                          pa.ProfileId == userProfile.Value);
                if (!hasOpenAccess)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var ucode = HttpContext.Session.GetString("UserName");

                    var findPass = await _context.tbl_ictams_users.Where(x => x.UserCode == ucode).FirstOrDefaultAsync();

                    var PasswordIsCorrect = BCrypt.Net.BCrypt.Verify("1234", findPass.UserPassword);
                    if (PasswordIsCorrect)
                    {
                        // Show success alert using SweetAlert
                        TempData["AlertType"] = "success";
                        TempData["SuccessMessage"] = "FORCE CHANGE PASSWORD!";
                        return RedirectToAction("ChangePassword", "Users");
                    }

                    var myData = HttpContext.Session.GetString("name");
                    var lSM_PNContext = _context.tbl_ictams_hardisk.Where(HD => HD.HDStatus == "AC").Include(CA => CA.Capacity);
                    return View(await lSM_PNContext.ToListAsync());
                }
            }

            return RedirectToAction("Index", "Home");

        }

        // GET: HardDisks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.tbl_ictams_hardisk == null)
            {
                return NotFound();
            }

            var hardDisk = await _context.tbl_ictams_hardisk
                .FirstOrDefaultAsync(m => m.HDId == id);
            if (hardDisk == null)
            {
                return NotFound();
            }

            return View(hardDisk);
        }
        public IActionResult InactiveHD()
        {
            var myData1 = HttpContext.Session.GetString("name");
            ViewBag.showprofile = myData1;
            var myData = HttpContext.Session.GetString("name");
            ViewBag.showprofile = myData;
            var inactiveHD = _context.tbl_ictams_hardisk
                .Where(hd => hd.HDStatus == "IN")
                .ToList();
            return View(inactiveHD);
        }
        public async Task<IActionResult> Activate(int? id)
        {
            var userr = HttpContext.Session.GetString("name");
            var hd = await _context.tbl_ictams_hardisk.FindAsync(id);
            if (hd == null)
            {
                return NotFound();
            }
            hd.HDUpdatedBy = userr;
            hd.HDUpdatedDate = DateTime.Now;
            hd.HDStatus = "AC"; // Set the status to "Active"

            _context.Entry(hd).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Json(new { success=true });
        }

        // GET: HardDisks/Create
        public IActionResult Create()
        {
            ViewData["HDCapacity"] = new SelectList(_context.tbl_ictams_capacity, "CapacityId", "CapacityDescription");
            return View();
        }

        // POST: HardDisks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HDId,HDDescription,HDCapacity,HDStatus,HDCreatedBy,HDCreatedDate,HDUpdatedBy,HDUpdatedDate")] HardDisk hardDisk)
        {

            var findDescription = await _context.tbl_ictams_hardisk.Where(x => x.HDDescription == hardDisk.HDDescription && x.HDCapacity == hardDisk.HDCapacity).FirstOrDefaultAsync();
            if (findDescription != null)
            {
                TempData["AlertMessage"] = "Hard disk description and capacity already exists!";
                return RedirectToAction(nameof(Index));
            }

            var userrr = HttpContext.Session.GetString("name");
            var paramCode = await _context.tbl_ictams_parameters.Where(p => p.parm_code == "hd_id").MaxAsync(p => p.parm_value);
                var newparamCode = paramCode + 1;

                var param = await _context.tbl_ictams_parameters.FirstOrDefaultAsync(p => p.parm_code == "hd_id");
                param.parm_value = newparamCode;

            

                hardDisk.HDDescription = hardDisk.HDDescription.ToUpper();
                hardDisk.HDStatus = "AC";
                hardDisk.HDId = newparamCode;
                hardDisk.HDCreatedDate = DateTime.Now;
                hardDisk.HDCreatedBy = userrr;
                _context.Add(hardDisk);
                await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully added a new hard disk!";
            // ...
            return RedirectToAction(nameof(Index));

        }

        // GET: HardDisks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["HDCapacity"] = new SelectList(_context.tbl_ictams_capacity, "CapacityId", "CapacityDescription");

            if (id == null || _context.tbl_ictams_hardisk == null)
            {
                return NotFound();
            }

            var hardDisk = await _context.tbl_ictams_hardisk.FindAsync(id);
            if (hardDisk == null)
            {
                return NotFound();
            }
            return View(hardDisk);

        }
      

        // POST: HardDisks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HDId,HDDescription,HDCapacity,HDStatus,HDCreatedBy,HDCreatedDate,HDUpdatedBy,HDUpdatedDate")] HardDisk hardDisk)
        {
            var userrr = HttpContext.Session.GetString("name");

            if (ModelState.IsValid)
            {
                try
                {
                    hardDisk.HDDescription = hardDisk.HDDescription.ToUpper();
                    hardDisk.HDUpdatedBy = userrr;
                    hardDisk.HDUpdatedDate = DateTime.Now;
                    _context.Update(hardDisk);
                    await _context.SaveChangesAsync();
                    // ...
                    TempData["SuccessNotification"] = "Successfully edit a hard disk!";
                    // ...
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HardDiskExists(hardDisk.HDId))
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
            return View(hardDisk);
        }

        // GET: HardDisks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.tbl_ictams_hardisk == null)
            {
                return NotFound();
            }

            var hardDisk = await _context.tbl_ictams_hardisk
                .FirstOrDefaultAsync(m => m.HDId == id);
            if (hardDisk == null)
            {
                return NotFound();
            }

            return View(hardDisk);
        }

        // POST: HardDisks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var findUse = await _context.tbl_ictams_laptopinv.Where(x => x.LTHardisk == id).FirstOrDefaultAsync();
            if (findUse != null)
            {
                TempData["AlertMessage"] = "Cannot be deleted. It is already in use!";
                return RedirectToAction(nameof(Index));
            }



            var userrr = HttpContext.Session.GetString("name");
            if (_context.tbl_ictams_hardisk == null)
            {
                return Problem("Entity set 'AssetManagementContext.HardDisk'  is null.");
            }
            var hardDisk = await _context.tbl_ictams_hardisk.FindAsync(id);
            if (hardDisk != null)
            {
                hardDisk.HDStatus = "IN";
                hardDisk.HDUpdatedBy = userrr;
                hardDisk.HDUpdatedDate = DateTime.Now;
                _context.tbl_ictams_hardisk.Update(hardDisk);
            }
            
            await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully delete a hard disk!";
            // ...
            return RedirectToAction(nameof(Index));
        }

        private bool HardDiskExists(int id)
        {
          return (_context.tbl_ictams_hardisk?.Any(e => e.HDId == id)).GetValueOrDefault();
        }
    }
}
