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
    public class UpsController : BaseController
    {
        private readonly AssetManagementContext _context;

        public UpsController(AssetManagementContext context) : base(context)
        {
            _context = context;
        }

        // GET: Ups
        public async Task<IActionResult> Index()
        {
            var assetManagementContext = _context.tbl_ictams_ups.Where(u => u.ups_status != "IN").Include(u => u.Brand).Include(u => u.Createdby).Include(u => u.Model).Include(u => u.Status).Include(u => u.Updatedby).Include(u => u.UpsBattStatus).Include(u => u.UpsCondition).Include(u => u.UpsType).Include(u => u.Vendor);
            return View(await assetManagementContext.ToListAsync());
        }
        public async Task<IActionResult> Inactive()
        {
            var assetManagementContext = _context.tbl_ictams_ups.Include(u => u.Status).Include(u => u.Createdby).Where(u => u.ups_status == "IN").Include(u => u.Brand).Include(u => u.Model).Include(u => u.UpsType).Include(u => u.Vendor).Include(u => u.UpsCondition).Include(u => u.UpsBattStatus).Include(u => u.Store);
            return View(await assetManagementContext.ToListAsync());
        }

        // GET: Ups/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Ups == null)
            {
                return NotFound();
            }

            var ups = await _context.Ups
                .Include(u => u.Brand)
                .Include(u => u.Createdby)
                .Include(u => u.Model)
                .Include(u => u.Status)
                .Include(u => u.Updatedby)
                .Include(u => u.UpsBattStatus)
                .Include(u => u.UpsCondition)
                .Include(u => u.UpsType)
                .Include(u => u.Vendor)
                .FirstOrDefaultAsync(m => m.ups_store == id);
            if (ups == null)
            {
                return NotFound();
            }

            return View(ups);
        }

        // GET: Ups/Create
        public IActionResult Create()
        {
            ViewData["ups_store"] = new SelectList(_context.tbl_ictams_stores, "Store_code", "StoreName");
            return View();
        }

        // POST: Ups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ups_store,ups_code,ups_brand,ups_model,ups_type,ups_serial,ups_warranty,ups_validity,ups_iovoltage,ups_dcvoltage,ups_idealload,ups_currentload,ups_vendor,ups_dtinstalled,ups_condition,ups_serviceyrs,ups_lastpmdt,ups_battstatus,ups_battrepcount,ups_unitserve,ups_status,ups_remarks,ups_createdby,ups_createddt,ups_updatedby,ups_updateddt")] Ups ups)
        {
            bool serialExists = await _context.tbl_ictams_ups.AnyAsync(x => x.ups_serial == ups.ups_serial && x.ups_status == "AC");
            if (serialExists)
            {
                TempData["ErrorMessage"] = "This serial number already exists!";
                return RedirectToAction(nameof(Index));
            }
            if (ups.ups_store.Equals(0))
            {
                TempData["AlertMessage"] = "Please select a store!";
                return RedirectToAction("Create");
            }
            if (ups.ups_brand.Equals(0))
            {
                TempData["AlertMessage"] = "Please select a brand!";
                return RedirectToAction("Create");
            }
            if (ups.ups_model.Equals(0))
            {
                TempData["AlertMessage"] = "Please select a model!";
                return RedirectToAction("Create");
            }
            if (ups.ups_type.Equals(0))
            {
                TempData["AlertMessage"] = "Please select a type!";
                return RedirectToAction("Create");
            }
            if (ups.ups_serial.Equals(0))
            {
                TempData["AlertMessage"] = "Please input a serial!";
                return RedirectToAction("Create");
            }
            if (ups.ups_warranty.Equals(0))
            {
                TempData["AlertMessage"] = "Please input a warranty!";
                return RedirectToAction("Create");
            }
            if (ups.ups_validity.Equals(0))
            {
                TempData["AlertMessage"] = "Please input a validity!";
                return RedirectToAction("Create");
            }
            if (ups.ups_iovoltage.Equals(0))
            {
                TempData["AlertMessage"] = "Please input a IO Voltage!";
                return RedirectToAction("Create");
            }
            if (ups.ups_dcvoltage.Equals(0))
            {
                TempData["AlertMessage"] = "Please input a DC Voltage!";
                return RedirectToAction("Create");
            }
            if (ups.ups_idealload.Equals(0))
            {
                TempData["AlertMessage"] = "Please input a Ideal Load!";
                return RedirectToAction("Create");
            }
            if (ups.ups_currentload.Equals(0))
            {
                TempData["AlertMessage"] = "Please input a Current Load!";
                return RedirectToAction("Create");
            }
            if (ups.ups_vendor.Equals(0))
            {
                TempData["AlertMessage"] = "Please select a Vendor!";
                return RedirectToAction("Create");
            }
            if (ups.ups_dtinstalled.Equals(0))
            {
                TempData["AlertMessage"] = "Please select a date!";
                return RedirectToAction("Create");
            }
            if (ups.ups_condition.Equals(0))
            {
                TempData["AlertMessage"] = "Please select a condition!";
                return RedirectToAction("Create");
            }
            if (ups.ups_serviceyrs.Equals(0))
            {
                TempData["AlertMessage"] = "Please input a service years!";
                return RedirectToAction("Create");
            }
            if (ups.ups_lastpmdt.Equals(0))
            {
                TempData["AlertMessage"] = "Please select a date!";
                return RedirectToAction("Create");
            }
            if (ups.ups_battstatus.Equals(0))
            {
                TempData["AlertMessage"] = "Please select a battery status!";
                return RedirectToAction("Create");
            }
            if (ups.ups_battrepcount.Equals(0))
            {
                TempData["AlertMessage"] = "Please input a battery count!";
                return RedirectToAction("Create");
            }
            if (ups.ups_unitserve.Equals(0))
            {
                TempData["AlertMessage"] = "Please input a unit serve!";
                return RedirectToAction("Create");
            }
            if (ups.ups_remarks.Equals(0))
            {
                TempData["AlertMessage"] = "Please input a remarks!";
                return RedirectToAction("Create");
            }
            else
            {
                var ucode = HttpContext.Session.GetString("UserName");
                var paramCode = await _context.tbl_ictams_parameters.Where(p => p.parm_code == "ups_id").MaxAsync(p => p.parm_value);
                var newparamCode = paramCode + 1;

                var param = await _context.tbl_ictams_parameters.FirstOrDefaultAsync(p => p.parm_code == "ups_id");
                param.parm_value = newparamCode;


                ups.ups_status = "AC";
                ups.ups_code = "UPS" + newparamCode.ToString().PadLeft(7, '0');
                ups.ups_createdby = ucode;
                ups.ups_createddt = DateTime.Now;
                _context.Add(ups);
                await _context.SaveChangesAsync();
                // ...
                TempData["SuccessNotification"] = "Successfully added a new UPS!";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Ups/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            ViewData["ups_type"] = new SelectList(_context.tbl_ictams_upstype, "type_id", "type_description");
            ViewData["ups_condition"] = new SelectList(_context.tbl_ictams_upscondition, "condition_id", "condition_description");
            ViewData["ups_battstatus"] = new SelectList(_context.tbl_ictams_upsbattstatus, "status_id", "status_description");

            if (id == null)
            {
                return NotFound();
            }

            var ups = await _context.tbl_ictams_ups
                .Include(u => u.Brand)
                .Include(u => u.Createdby)
                .Include(u => u.Model)
                .Include(u => u.Status)
                .Include(u => u.Updatedby)
                .Include(u => u.UpsBattStatus)
                .Include(u => u.UpsCondition)
                .Include(u => u.UpsType)
                .Include(u => u.Vendor)
                .FirstOrDefaultAsync(m => m.ups_code == id);
            if (ups == null)
            {
                return NotFound();
            }


            return View(ups);
        }

        // POST: Ups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ups_store,ups_code,ups_brand,ups_model,ups_type,ups_serial,ups_warranty,ups_validity,ups_iovoltage,ups_dcvoltage,ups_idealload,ups_currentload,ups_vendor,ups_dtinstalled,ups_condition,ups_serviceyrs,ups_lastpmdt,ups_battstatus,ups_battrepcount,ups_unitserve,ups_status,ups_remarks,ups_createdby,ups_createddt,ups_updatedby,ups_updateddt")] Ups ups)
        {
            if (id != ups.ups_code)
            {
                return NotFound();
            }

            var upsEdit = await _context.tbl_ictams_ups.Where(c => c.ups_code == ups.ups_code).FirstOrDefaultAsync();
            try
            {
                var ucode = HttpContext.Session.GetString("UserName");
                upsEdit.ups_updatedby = ucode;
                upsEdit.ups_updateddt = DateTime.Now;
                upsEdit.ups_status = "AV";
                upsEdit.ups_warranty = ups.ups_warranty;
                upsEdit.ups_validity = ups.ups_validity;
                upsEdit.ups_iovoltage = ups.ups_iovoltage;
                upsEdit.ups_dcvoltage = ups.ups_dcvoltage;
                upsEdit.ups_idealload = ups.ups_idealload;
                upsEdit.ups_currentload = ups.ups_currentload;
                upsEdit.ups_dtinstalled = ups.ups_dtinstalled;
                upsEdit.ups_serviceyrs = ups.ups_serviceyrs;
                upsEdit.ups_lastpmdt = ups.ups_lastpmdt;
                upsEdit.ups_battrepcount = ups.ups_battrepcount;
                upsEdit.ups_unitserve = ups.ups_unitserve;
                upsEdit.ups_remarks = ups.ups_remarks;
                //upsEdit.ups_brand = ups.ups_brand;
                //upsEdit.ups_model = ups.ups_model;
                upsEdit.ups_type = ups.ups_type;
                //upsEdit.ups_vendor = ups.ups_vendor;
                upsEdit.ups_condition = ups.ups_condition;
                upsEdit.ups_battstatus = ups.ups_battstatus;

                await _context.SaveChangesAsync();
                // ...
                TempData["SuccessNotification"] = "Successfully edited!";
                // ...
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UpsExists(ups.ups_code))
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
    

        //DELETE
        public async Task<IActionResult> DeleteAsEdit(string[] selectedIds)
        {
            ViewData["UserStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_name");
            foreach (string id in selectedIds)
            {
                var upID = await _context.tbl_ictams_ups.FindAsync(id);
                if (upID != null)
                {
                    // Update the profile
                    var ucode = HttpContext.Session.GetString("UserName");
                    upID.ups_updatedby = ucode;
                    upID.ups_status = "IN";
                    upID.ups_updateddt = DateTime.Now;

                    _context.Update(upID);
                    await _context.SaveChangesAsync();
                    // ...
                    TempData["SuccessNotification"] = "Successfully deleted!";
                    // ...
                    return RedirectToAction(nameof(Index));
                }
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Retrieve(string[] selectedIds)
        {
            ViewData["UserStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_name");
            foreach (string id in selectedIds)
            {
                var upsCode = await _context.tbl_ictams_ups.FindAsync(id);
                if (upsCode != null)
                {
                    var status = await _context.tbl_ictams_status.FindAsync(upsCode.ups_store);
                    if (status == null)
                    {
                        ModelState.AddModelError("", $"Profile status '{upsCode.ups_code}' does not exist.");
                    }

                    // Update the profile
                    var ucode = HttpContext.Session.GetString("UserName");
                    upsCode.ups_updatedby = ucode;
                    upsCode.ups_status = "AV";
                    upsCode.ups_updateddt = DateTime.Now;

                    _context.Update(upsCode);
                    await _context.SaveChangesAsync();

                    // ...
                    TempData["SuccessNotification"] = "Successfully retrieved!";
                    // ...
                    return RedirectToAction(nameof(Index));
                }
            }

            return RedirectToAction(nameof(Index));
        }


        private bool UpsExists(string id)
        {
          return (_context.Ups?.Any(e => e.ups_code == id)).GetValueOrDefault();
        }
    }
}
