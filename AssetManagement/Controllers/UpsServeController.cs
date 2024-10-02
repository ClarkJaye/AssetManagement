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
    public class UpsServeController : BaseController
    {
        private readonly AssetManagementContext _context;

        public UpsServeController(AssetManagementContext context) : base(context)
        {
            _context = context;
        }

        // GET: UpsServes
        public async Task<IActionResult> Index()
        {
            await FindDesktopAllocationCompleted();
            await FindStatus();

            int? userProfile = HttpContext.Session.GetInt32("UserProfile");
            if (userProfile.HasValue)
            {

                var hasOpenAccess = await _context.tbl_ictams_profileaccess
          .AnyAsync(pa => pa.OpenAccess == "Y" &&
                          pa.Module.ModuleTitle == "UPS Serve" && pa.ProfileId == userProfile.Value);
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
                    var assetManagementContext = _context.tbl_ictams_upsserve.Include(u => u.User);
                    return View(await assetManagementContext.ToListAsync());

                }
            }

            return RedirectToAction("Logout", "Users");

        }

        // GET: UpsServes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.tbl_ictams_upsserve == null)
            {
                return NotFound();
            }

            var upsServe = await _context.tbl_ictams_upsserve
				.Include(u => u.UpsCode)
                .Include(u => u.UpsStore)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.UnitNo == id);
            if (upsServe == null)
            {
                return NotFound();
            }

            return View(upsServe);
        }

        // GET: UpsServes/Create
        public IActionResult Create()
        {
            ViewData["UpsServeStore"] = new SelectList(_context.tbl_ictams_ups, "ups_store", "ups_store");
            ViewData["UpsServeCode"] = new SelectList(_context.tbl_ictams_ups, "ups_code", "ups_code");
            ViewData["UnitCreatedBy"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode");
            return View();
        }

        // POST: UpsServes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( UpsServe upsServe )
        {
            var findUPSSERVECODE = await _context.tbl_ictams_upsserve.Where(x => x.UpsServeCode == upsServe.UpsServeCode)
                .FirstOrDefaultAsync();
            if (findUPSSERVECODE != null)
            {
                TempData["AlertMessage"] = "This UPS CODE already exists. Please select a different CODE.";
                return RedirectToAction(nameof(Index));
            }
            var userrr = HttpContext.Session.GetString("UserName");

            var paramCode = await _context.tbl_ictams_parameters.Where(p => p.parm_code == "serve_no").MaxAsync(p => p.parm_value);
            var newparamCode = paramCode + 1;

            var param = await _context.tbl_ictams_parameters.FirstOrDefaultAsync(p => p.parm_code == "serve_no");
            param.parm_value = newparamCode;

            upsServe.UnitCreatedAt = DateTime.Now;
            upsServe.UnitCreatedBy = userrr;
            upsServe.UnitNo = newparamCode;
            upsServe.UpsServeCode = upsServe.UpsServeCode;

            var findStoreCode = await _context.tbl_ictams_ups.Where(x => x.ups_code == upsServe.UpsServeCode).FirstOrDefaultAsync();
            upsServe.UpsServeStore = findStoreCode.ups_store;

            _context.Add(upsServe);
            await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully added!";
            // ...
            return RedirectToAction(nameof(Index));
        }

        // GET: UpsServes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.UpsServe == null)
            {
                return NotFound();
            }

            var upsServe = await _context.UpsServe.FindAsync(id);
            if (upsServe == null)
            {
                return NotFound();
            }
            ViewData["UpsServeStore"] = new SelectList(_context.Ups, "ups_store", "ups_store", upsServe.UpsServeStore);
            ViewData["UpsServeCode"] = new SelectList(_context.Ups, "ups_code", "ups_code", upsServe.UpsServeCode);
            ViewData["UnitCreatedBy"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode", upsServe.UnitCreatedBy);
            return View(upsServe);
        }

        // POST: UpsServes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UpsServeStore,UpsServeCode,UnitNo,UnitServe,UnitCreatedBy,UnitCreatedAt")] UpsServe upsServe)
        {
            if (id != upsServe.UnitNo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(upsServe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UpsServeExists(upsServe.UnitNo))
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
            ViewData["UpsServeStore"] = new SelectList(_context.Ups, "ups_store", "ups_store", upsServe.UpsServeStore);
            ViewData["UpsServeCode"] = new SelectList(_context.Ups, "ups_code", "ups_code", upsServe.UpsServeCode);
            ViewData["UnitCreatedBy"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode", upsServe.UnitCreatedBy);
            return View(upsServe);
        }

        public async Task<IActionResult> Delete(int[] selectedIds)
        {
            foreach (int id in selectedIds)
            {
                var upsserve = await _context.tbl_ictams_upsserve.FindAsync(id);
                if (upsserve != null)
                {
                    _context.Remove(upsserve);
                    await _context.SaveChangesAsync();
                    // ...
                    TempData["SuccessNotification"] = "Successfully deleted!";
                    // ...
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

        private bool UpsServeExists(int id)
        {
          return (_context.UpsServe?.Any(e => e.UnitNo == id)).GetValueOrDefault();
        }
    }
}
