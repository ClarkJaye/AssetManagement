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

namespace AssetManagement.Controllers
{
    public class UpsPMController : BaseController
    {
        private readonly AssetManagementContext _context;

        public UpsPMController(AssetManagementContext context) : base(context)
        {
            _context = context;
        }

        // GET: UpsPMs
        public async Task<IActionResult> Index()
        {
            await FindDesktopAllocationCompleted();
            await FindStatus();

            int? userProfile = HttpContext.Session.GetInt32("UserProfile");
            if (userProfile.HasValue)
            {

                var hasOpenAccess = await _context.tbl_ictams_profileaccess
          .AnyAsync(pa => pa.OpenAccess == "Y" &&
                          pa.Module.ModuleTitle == "UPS PM" && pa.ProfileId == userProfile.Value);
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
                    var assetManagementContext = await _context.tbl_ictams_upspm
                        .Include(u => u.User).ToListAsync();
                    return View(assetManagementContext);

                }
            }

            return RedirectToAction("Logout", "Users");

        }

        // GET: UpsBatteryReps/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.tbl_ictams_upspm == null)
            {
                return NotFound();
            }

            var upsPm = await _context.tbl_ictams_upspm.FindAsync(id);
            if (upsPm == null)
            {
                return NotFound();
            }
            ViewData["UpsPMStore"] = new SelectList(_context.tbl_ictams_ups.Select(u => u.ups_store).Distinct(), upsPm.UpsPMStore);
            ViewData["UpsPMCode"] = new SelectList(_context.tbl_ictams_ups.Where(u => u.ups_store == upsPm.UpsPMStore), "ups_code", "ups_code", upsPm.UpsPMCode);
            ViewData["PMCreatedBy"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode", upsPm.PMCreatedBy);
            return View(upsPm);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpsPM upsPm)
        {
            if (upsPm.PMNO == 0)
            {
                return NotFound();
            }

            // Check if the UpsPMStore and UpsPMCode exist in the related table
            var validStore = await _context.tbl_ictams_ups.AnyAsync(u => u.ups_store == upsPm.UpsPMStore);
            var validCode = await _context.tbl_ictams_ups.AnyAsync(u => u.ups_code == upsPm.UpsPMCode && u.ups_store == upsPm.UpsPMStore);

            if (!validStore)
            {
                ModelState.AddModelError("UpsPMStore", "Selected UPS Store does not exist.");
            }

            if (!validCode)
            {
                ModelState.AddModelError("UpsPMCode", "Selected UPS Code does not exist or is not related to the selected store.");
            }

            if (!ModelState.IsValid)
            {
                // Re-populate the select lists and return the view with errors
                ViewData["UpsPMStore"] = new SelectList(_context.tbl_ictams_ups.Select(u => u.ups_store).Distinct(), upsPm.UpsPMStore);
                ViewData["UpsPMCode"] = new SelectList(_context.tbl_ictams_ups.Where(u => u.ups_store == upsPm.UpsPMStore), "ups_code", "ups_code", upsPm.UpsPMCode);
                ViewData["PMCreatedBy"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode", upsPm.PMCreatedBy);
                return View(upsPm);
            }

            try
            {
                _context.Update(upsPm);
                TempData["SuccessNotification"] = "Successfully updated!";
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UpsPMExists(upsPm.PMNO))
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



        // GET: UpsBatteryReps/GetUpsCodesByStore
        public async Task<JsonResult> GetUpsCodesByStore(string upsStore)
        {
            var upsCodes = await _context.tbl_ictams_ups
                .Where(u => u.ups_store == upsStore)
                .Select(u => new { u.ups_code })
                .ToListAsync();

            return Json(upsCodes);
        }


        // GET: UpsPMs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.tbl_ictams_upspm == null)
            {
                return NotFound();
            }

            var upsPM = await _context.tbl_ictams_upspm
                .Include(u => u.UpsCode)
                .Include(u => u.UpsStore)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.PMNO == id);
            if (upsPM == null)
            {
                return NotFound();
            }

            return View(upsPM);
        }

        // GET: UpsPMs/Create
        public IActionResult Create()
        {
            ViewData["UpsPMCode"] = new SelectList(_context.tbl_ictams_ups, "ups_code", "ups_code");
            ViewData["UpsPMStore"] = new SelectList(_context.tbl_ictams_ups, "ups_store", "ups_store");
            ViewData["PMCreatedBy"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode");
            return View();
        }

        // POST: UpsPMs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( UpsPM upsPM)
        {
            var findUPSPMCODE = await _context.tbl_ictams_upspm.Where(x => x.UpsPMCode == upsPM.UpsPMCode)
                .FirstOrDefaultAsync();
            if (findUPSPMCODE != null)
            {
                TempData["AlertMessage"] = "This UPS CODE already exists. Please select a different CODE.";
                return RedirectToAction(nameof(Index));
            }

            var userrr = HttpContext.Session.GetString("UserName");

            var paramCode = await _context.tbl_ictams_parameters.Where(p => p.parm_code == "upspm_no").MaxAsync(p => p.parm_value);
            var newparamCode = paramCode + 1;

            var param = await _context.tbl_ictams_parameters.FirstOrDefaultAsync(p => p.parm_code == "upspm_no");
            param.parm_value = newparamCode;

            upsPM.PMDate = upsPM.PMDate;
            upsPM.PMNO = newparamCode;
            upsPM.PMCreatedBy = userrr;
            upsPM.PMCreatedAt = DateTime.Now;
            upsPM.UpsPMCode = upsPM.UpsPMCode;

            var findStoreCode = await _context.tbl_ictams_ups.Where(x => x.ups_code == upsPM.UpsPMCode).FirstOrDefaultAsync();
            upsPM.UpsPMStore = findStoreCode.ups_store;

            _context.Add(upsPM);
            await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully added a new UPS Preventive Maintenance!";
            // ...
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int[] selectedIds)
        {
            foreach (int id in selectedIds)
            {
                var upspm = await _context.tbl_ictams_upspm.FindAsync(id);
                if (upspm != null)
                {
                    _context.Remove(upspm);
                    await _context.SaveChangesAsync();
                    // ...
                    TempData["SuccessNotification"] = "Successfully delete a  UPS Preventive Maintenance!";
                    // ...
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool UpsPMExists(int id)
        {
          return (_context.tbl_ictams_upspm?.Any(e => e.PMNO == id)).GetValueOrDefault();
        }
    }
}
