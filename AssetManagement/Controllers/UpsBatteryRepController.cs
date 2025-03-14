﻿using System;
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
    public class UpsBatteryRepController : BaseController
    {
        private readonly AssetManagementContext _context;

        public UpsBatteryRepController(AssetManagementContext context) : base(context)
        {
            _context = context;
        }

        // GET: UpsBatteryReps
        public async Task<IActionResult> Index()
        {
            await FindDesktopAllocationCompleted();
            await FindStatus();

            int? userProfile = HttpContext.Session.GetInt32("UserProfile");
            if (userProfile.HasValue)
            {

                var hasOpenAccess = await _context.tbl_ictams_profileaccess
          .AnyAsync(pa => pa.OpenAccess == "Y" &&
                          pa.Module.ModuleTitle == "UPS Battery Rep" && pa.ProfileId == userProfile.Value);
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
                    var assetManagementContext = _context.tbl_ictams_upsbattrep.Include(u => u.User);
                    return View(await assetManagementContext.ToListAsync());

                }
            }

            return RedirectToAction("Logout", "Users");
        }

        // GET: UpsBatteryReps/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.tbl_ictams_upsbattrep == null)
            {
                return NotFound();
            }

            var upsBatteryRep = await _context.tbl_ictams_upsbattrep
                .Include(u => u.UpsCode)
                .Include(u => u.UpsStore)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.BatteryRepNo == id);
            if (upsBatteryRep == null)
            {
                return NotFound();
            }

            return View(upsBatteryRep);
        }

        // GET: UpsBatteryReps/Create
        public IActionResult Create()
        {
            ViewData["UpsBattCode"] = new SelectList(_context.tbl_ictams_ups, "ups_code", "ups_code");
            ViewData["UpsBattStore"] = new SelectList(_context.tbl_ictams_ups, "ups_store", "ups_store");
            return View();
        }

        // POST: UpsBatteryReps/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( UpsBatteryRep upsBatteryRep )
        {
            var findUPSBATTCODE = await _context.tbl_ictams_upsbattrep.Where(x => x.UpsBattCode == upsBatteryRep.UpsBattCode)
                .FirstOrDefaultAsync();
            if (findUPSBATTCODE != null)
            {
                TempData["AlertMessage"] = "This UPS CODE already exists. Please select a different CODE.";
                return RedirectToAction(nameof(Index));
            }
            var userrr = HttpContext.Session.GetString("UserName");

            var paramCode = await _context.tbl_ictams_parameters.Where(p => p.parm_code == "upsbattrep").MaxAsync(p => p.parm_value);
            var newparamCode = paramCode + 1;

            var param = await _context.tbl_ictams_parameters.FirstOrDefaultAsync(p => p.parm_code == "upsbattrep");
            param.parm_value = newparamCode;

            upsBatteryRep.BatteryRepCreatedAt = DateTime.Now;
            upsBatteryRep.BatteryRepCreatedBy = userrr;
            upsBatteryRep.BatteryRepNo = newparamCode;
            upsBatteryRep.UpsBattCode = upsBatteryRep.UpsBattCode;
            upsBatteryRep.BatteryRepRemarks = upsBatteryRep.BatteryRepRemarks;
            upsBatteryRep.BatteryRepDate = upsBatteryRep.BatteryRepDate;

            var findStoreCode = await _context.tbl_ictams_ups.Where(x => x.ups_code == upsBatteryRep.UpsBattCode).FirstOrDefaultAsync();
            upsBatteryRep.UpsBattStore = findStoreCode.ups_store;

            _context.Add(upsBatteryRep);
            await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully added a new UPS Battery Replacement!";
            // ...
            return RedirectToAction(nameof(Index));
        }

        // GET: UpsBatteryReps/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.tbl_ictams_upsbattrep == null)
            {
                return NotFound();
            }

            var upsBatteryRep = await _context.tbl_ictams_upsbattrep.FindAsync(id);
            if (upsBatteryRep == null)
            {
                return NotFound();
            }
            ViewData["UpsBattStore"] = new SelectList(_context.tbl_ictams_ups.Select(u => u.ups_store).Distinct(), upsBatteryRep.UpsBattCode);
            ViewData["UpsBattCode"] = new SelectList(_context.tbl_ictams_ups.Where(u => u.ups_store == upsBatteryRep.UpsBattStore), "ups_code", "ups_code", upsBatteryRep.UpsBattCode);
            ViewData["BatteryRepCreatedBy"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode", upsBatteryRep.BatteryRepCreatedBy);
            return View(upsBatteryRep);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpsBatteryRep upsBatteryRep)
        {
            if (upsBatteryRep.BatteryRepNo == 0)
            {
                return NotFound();
            }

            // Check if the UpsPMStore and UpsPMCode exist in the related table
            var validStore = await _context.tbl_ictams_ups.AnyAsync(u => u.ups_store == upsBatteryRep.UpsBattStore);
            var validCode = await _context.tbl_ictams_ups.AnyAsync(u => u.ups_code == upsBatteryRep.UpsBattCode && u.ups_store == upsBatteryRep.UpsBattStore);

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
                ViewData["UpsBattStore"] = new SelectList(_context.tbl_ictams_ups.Select(u => u.ups_store).Distinct(), upsBatteryRep.UpsBattStore);
                ViewData["UpsBattCode"] = new SelectList(_context.tbl_ictams_ups.Where(u => u.ups_store == upsBatteryRep.UpsBattStore), "ups_code", "ups_code", upsBatteryRep.UpsBattCode);
                ViewData["BatteryRepCreatedBy"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode", upsBatteryRep.BatteryRepCreatedBy);
                return View(upsBatteryRep);
            }

            try
            {
                _context.Update(upsBatteryRep);
                TempData["SuccessNotification"] = "Successfully updated!";
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UpsBatteryRepExists(upsBatteryRep.BatteryRepNo))
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


        public async Task<IActionResult> Delete(int[] selectedIds)
        {
            foreach (int id in selectedIds)
            {
                var upsserve = await _context.tbl_ictams_upsbattrep.FindAsync(id);
                if (upsserve != null)
                {
                    _context.Remove(upsserve);
                    await _context.SaveChangesAsync();
                    // ...
                    TempData["SuccessNotification"] = "Successfully delete a  UPS Battery Replacement!";
                    // ...
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool UpsBatteryRepExists(int id)
        {
          return (_context.tbl_ictams_upsbattrep?.Any(e => e.BatteryRepNo == id)).GetValueOrDefault();
        }
    }
}
