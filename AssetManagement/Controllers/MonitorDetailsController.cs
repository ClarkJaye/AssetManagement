using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Data;
using AssetManagement.Models;

namespace AssetManagement.Controllers
{
    public class MonitorDetailsController : Controller
    {
        private readonly AssetManagementContext _context;

        public MonitorDetailsController(AssetManagementContext context)
        {
            _context = context;
        }

        // GET: MonitorDetails
        public async Task<IActionResult> Index()
        {
            var assetManagementContext = _context.tbl_ictams_monitordetails.Include(i => i.Createdby).Include(i => i.MonitorInventory).Include(i => i.Status).Include(i => i.Updatedby).Include(i => i.Vendor);
            return View(await assetManagementContext.ToListAsync());
        }

        // GET: MonitorDetails/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.tbl_ictams_monitordetails == null)
            {
                return NotFound();
            }

            var inventoryDetails = await _context.tbl_ictams_monitordetails
                .Include(i => i.Createdby)
                .Include(i => i.MonitorInventory)
                .Include(i => i.Status)
                .Include(i => i.Updatedby)
                .Include(i => i.Vendor)
                .FirstOrDefaultAsync(m => m.SerialNumber == id);
            if (inventoryDetails == null)
            {
                return NotFound();
            }

            return View(inventoryDetails);
        }

        // GET: MonitorDetails/Create
        public IActionResult Create(string id)
        {
            ViewBag.Code = id;

            return View();
        }

        // POST: MonitorDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("monitorCode,SerialNumber,PO,Price,MonitorVendor,PurchaseDate,DeployedDate,MonitorStatus,DetailCreated,DateCreated,DetailUpdated,DateUpdated")] MonitorDetail monitorDetail)
        {
            try
            {
                var findSerial = await _context.tbl_ictams_monitordetails.Where(x => x.SerialNumber == monitorDetail.SerialNumber)
                    .FirstOrDefaultAsync();
                if (findSerial != null)
                {
                    TempData["ErrorNotification"] = "Serial Number already Exist!";
                    return RedirectToAction("Index", "MonitorInventories");
                }
                if (monitorDetail.MonitorVendor.Equals(0))
                {
                    TempData["ErrorNotification"] = "Vendor is missing,Please Select a Vendor!";
                    return RedirectToAction("Index", "MonitorInventories");
                }

                var ucode = HttpContext.Session.GetString("UserName");
                monitorDetail.DetailCreated = ucode;
                monitorDetail.SerialNumber = monitorDetail.SerialNumber.ToUpper();
                monitorDetail.PO = monitorDetail.PO.ToUpper();
                monitorDetail.MonitorStatus = "AV";
                monitorDetail.DateCreated = DateTime.Now;
                _context.Add(monitorDetail);

                var maxQuantityLaptop = await _context.tbl_ictams_monitorinv
                 .Where(x => x.monitorCode == monitorDetail.monitorCode)
                 .MaxAsync(x => x.Quantity);

                // Now you can update the database with the incremented quantity
                var laptopToUpdate = await _context.tbl_ictams_monitorinv
                    .FirstOrDefaultAsync(x => x.monitorCode == monitorDetail.monitorCode && x.Quantity == maxQuantityLaptop);

                if (laptopToUpdate != null)
                {
                    laptopToUpdate.Quantity += 1; // Increment the quantity by 1
                    await _context.SaveChangesAsync();
                }


                // ...
                TempData["SuccessNotification"] = "Successfully added a new monitor to inventory";
                // ...
                return RedirectToAction("Index", "MonitorInventories");
            }
            catch (Exception e)
            {
                return View(e.Message);
            }
        }

        // GET: MonitorDetails/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.tbl_ictams_monitordetails == null)
            {
                return NotFound();
            }

            var inventoryDetails = await _context.tbl_ictams_monitordetails.FindAsync(id);
            if (inventoryDetails == null)
            {
                return NotFound();
            }

            return View(inventoryDetails);
        }

        // POST: MonitorDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("monitorCode,SerialNumber,PO,Price,MonitorVendor,PurchaseDate,DeployedDate,MonitorStatus,DetailCreated,DateCreated,DetailUpdated,DateUpdated")] MonitorDetail monitorDetail)
        {
            if (id != monitorDetail.SerialNumber)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(monitorDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MonitorDetailExists(monitorDetail.SerialNumber))
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
            ViewData["DetailCreated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode", monitorDetail.DetailCreated);
            ViewData["monitorCode"] = new SelectList(_context.tbl_ictams_monitorinv, "monitorCode", "monitorCode", monitorDetail.monitorCode);
            ViewData["MonitorStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_code", monitorDetail.MonitorStatus);
            ViewData["DetailUpdated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode", monitorDetail.DetailUpdated);
            ViewData["MonitorVendor"] = new SelectList(_context.tbl_ictams_vendor, "VendorID", "VendorID", monitorDetail.MonitorVendor);
            return View(monitorDetail);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var findSerial = await _context.tbl_ictams_monitordetails
                                           .Where(x => x.SerialNumber == id)
                                           .FirstOrDefaultAsync();

            if (findSerial != null)
            {
                _context.tbl_ictams_monitordetails.Remove(findSerial);

                var maxQuantityLaptop = await _context.tbl_ictams_monitorinv
                                                     .Where(x => x.monitorCode == findSerial.monitorCode)
                                                     .MaxAsync(x => x.Quantity);

                var laptopToUpdate = await _context.tbl_ictams_monitorinv
                                                   .FirstOrDefaultAsync(x => x.monitorCode == findSerial.monitorCode && x.Quantity == maxQuantityLaptop);
                try
                {
                    if (laptopToUpdate != null)
                    {
                        laptopToUpdate.Quantity = Math.Max(0, laptopToUpdate.Quantity - 1);
                        await _context.SaveChangesAsync();
                        // ...
                        TempData["SuccessNotification"] = "Successfully removed a monitor from inventory";
                        // ...
                    }
                }
                catch (Exception)
                {
                    TempData["ErrorNotification"] = "It cannot be deleted since it is already in used!";
                }

            }
            else
            {
                TempData["ErrorNotification"] = "Monitor not found";
            }

            return RedirectToAction("Index", "MonitorInventories");
        }

        private bool MonitorDetailExists(string id)
        {
          return (_context.tbl_ictams_monitordetails?.Any(e => e.SerialNumber == id)).GetValueOrDefault();
        }
    }
}
