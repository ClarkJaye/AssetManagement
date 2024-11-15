using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Data;
using AssetManagement.Models;
using AssetManagement.Utility;

namespace AssetManagement.Controllers
{
    public class MonitorDetailsController : BaseController
    {
        private readonly AssetManagementContext _context;

        public MonitorDetailsController(AssetManagementContext context) : base(context)
        {
            _context = context;
        }

        // GET: MonitorDetails
        public async Task<IActionResult> Index()
        {
            var assetManagementContext = _context.tbl_ictams_monitordetails
                .Include(i => i.MonitorInventory)
                .Include(i => i.Status)
                .Include(i => i.Updatedby)
                .Include(i => i.Createdby)
                .Include(i => i.Vendor);
            return View(await assetManagementContext.ToListAsync());
        }

        // GET: MonitorDetails/Details/5
        public async Task<IActionResult> Details(string code, string serial)
        {
            if (code == null || serial == null || _context.tbl_ictams_monitordetails == null)
            {
                return NotFound();
            }

            var inventoryDetails = await _context.tbl_ictams_monitordetails
                .Include(i => i.Createdby)
                .Include(i => i.MonitorInventory)
                .Include(i => i.Status)
                .Include(i => i.Updatedby)
                .Include(i => i.Vendor)
                .FirstOrDefaultAsync(m => m.monitorCode == code && m.SerialNumber == serial);
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
            ViewData["MTInvCode"] = new SelectList(_context.tbl_ictams_monitorinv, "monitorCode", "monitorCode");
            ViewData["MTVendor"] = new SelectList(_context.tbl_ictams_vendor, "VendorID", "VendorName");
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
        public async Task<IActionResult> Edit(string code, string serial)
        {
            if (code == null || serial == null || _context.tbl_ictams_monitordetails == null)
            {
                return NotFound();
            }

            var inventoryDetails = await _context.tbl_ictams_monitordetails.FirstOrDefaultAsync(m => m.monitorCode == code && m.SerialNumber == serial);
            if (inventoryDetails == null)
            {
                return NotFound();
            }

            ViewData["MonitorVendor"] = new SelectList(_context.tbl_ictams_vendor, "VendorID", "VendorName", inventoryDetails.MonitorVendor);
            ViewData["MonitorStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_name", inventoryDetails.MonitorStatus);
            return View(inventoryDetails);
        }

        // POST: MonitorDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("monitorCode,SerialNumber,PO,Price,MonitorVendor,PurchaseDate,DeployedDate,MonitorStatus,DetailCreated,DateCreated,DetailUpdated,DateUpdated")] MonitorDetail monitorDetail)
        {
            try
            {
                // Retrieve the existing record to update
                var existingDetail = await _context.tbl_ictams_monitordetails
                    .FirstOrDefaultAsync(c => c.monitorCode == monitorDetail.monitorCode && c.SerialNumber == monitorDetail.SerialNumber);

                if (existingDetail == null)
                {
                    TempData["ErrorNotification"] = "Updated Failed";
                    return NotFound();
                }

                var ucode = HttpContext.Session.GetString("UserName");
                // Update specific properties
                existingDetail.PO = monitorDetail.PO;
                existingDetail.Price = monitorDetail.Price;
                existingDetail.MonitorVendor = monitorDetail.MonitorVendor;
                existingDetail.PurchaseDate = monitorDetail.PurchaseDate;
                existingDetail.DeployedDate = monitorDetail.DeployedDate;
                existingDetail.MonitorStatus = monitorDetail.MonitorStatus;
                existingDetail.DateUpdated = DateTime.Now;
                existingDetail.DetailUpdated = ucode;

                TempData["SuccessNotification"] = "Successfully Udpdated";

                _context.Update(existingDetail);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MonitorDetailExists(monitorDetail.monitorCode, monitorDetail.SerialNumber))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            //ViewData["MonitorStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "VendorName", monitorDetail.MonitorStatus);
            //ViewData["MonitorVendor"] = new SelectList(_context.tbl_ictams_vendor, "VendorID", "status_name", monitorDetail.MonitorVendor);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string code, string serial)
        {
            var findSerial = await _context.tbl_ictams_monitordetails
                                           .Where(x => x.monitorCode == code && x.SerialNumber == serial)
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
                        var ucode = HttpContext.Session.GetString("UserName");
                        laptopToUpdate.Quantity = laptopToUpdate.Quantity - 1;
                        //findSerial.MonitorStatus = "IN";
                        //findSerial.DateUpdated = DateTime.Now;
                        //findSerial.DetailUpdated = ucode;
                        //_context.tbl_ictams_monitordetails.Update(findSerial);

                        _context.tbl_ictams_monitorinv.Update(laptopToUpdate);
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

        private bool MonitorDetailExists(string code, string serial)
        {
          return (_context.tbl_ictams_monitordetails?.Any(e => e.monitorCode == code && e.SerialNumber == serial)).GetValueOrDefault();
        }
    }
}
