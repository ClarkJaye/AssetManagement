﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Data;
using AssetManagement.Models;
using AssetManagement.Utility;

namespace AssetManagement.Controllers
{
    public class InventoryDetailsController : BaseController
    {
        private readonly AssetManagementContext _context;

        public InventoryDetailsController(AssetManagementContext context)
        : base(context) // Pass the 'context' dependency to the base constructor
        {
            _context = context;
        }

        // GET: InventoryDetails
        public async Task<IActionResult> Index()
        {
            var assetManagementContext = _context.tbl_ictams_laptopinvdetails
              .Include(i => i.Createdby)
              .Include(i => i.LaptopInventory)
              .Include(i => i.Status)
              .Include(i => i.Updatedby)
              .Include(i => i.Vendor);
            return View(await assetManagementContext.ToListAsync());
        }
        public async Task<IActionResult> AgingReport()
        {
            return View();
        }
        public async Task<IActionResult> AgingReportDeploy()
        {
            return View();
        }
        // GET: InventoryDetails/Details/5
        public async Task<IActionResult> Details(string code, string serial)
        {
            if (code == null || serial == null || _context.tbl_ictams_laptopinvdetails == null)
            {
                return NotFound();
            }

            var inventoryDetails = await _context.tbl_ictams_laptopinvdetails
                .Include(i => i.Createdby)
                .Include(i => i.LaptopInventory)
                .Include(i => i.Status)
                .Include(i => i.Updatedby)
                .Include(i => i.Vendor)
                 .FirstOrDefaultAsync(m => m.laptoptinvCode == code && m.SerialCode == serial);
            if (inventoryDetails == null)
            {
                return NotFound();
            }

            return View(inventoryDetails);
        }
        public async Task<IActionResult> Get4OldLaptops()
        {
            DateTime fourYearsAgo = DateTime.Now.AddYears(-4);

            DateTime startDate = new DateTime(fourYearsAgo.Year, 1, 1); // Start of the year
            DateTime endDate = new DateTime(fourYearsAgo.Year, 12, 31); // End of the year

            var oldLaptops = await _context.tbl_ictams_laptopinvdetails
                .Where(laptop => laptop.PurchaseDate >= startDate && laptop.PurchaseDate <= endDate)
                .Include(i => i.Createdby)
                .Include(i => i.LaptopInventory).ThenInclude(i => i.Level)
                .Include(i => i.LaptopInventory.Brand)
                .Include(i => i.LaptopInventory.OS)
                .Include(i => i.LaptopInventory.Model)
                .Include(i => i.LaptopInventory.CPU)
                .Include(i => i.LaptopInventory.Memory.Capacity)
                .Include(i => i.LaptopInventory.Status)
                .Include(i => i.LaptopInventory.HardDisk.Capacity)
                .Include(i => i.Updatedby)
                .Include(i => i.Vendor)
                .ToListAsync();

            return View(oldLaptops);
        }
        public async Task<IActionResult> Get4OldLaptopsDeploy()
        {
            DateTime fourYearsAgo = DateTime.Now.AddYears(-4);

            DateTime startDate = new DateTime(fourYearsAgo.Year, 1, 1); // Start of the year
            DateTime endDate = new DateTime(fourYearsAgo.Year, 12, 31); // End of the year

            var oldLaptops = await _context.tbl_ictams_laptopinvdetails
                .Where(laptop => laptop.DeployedDate >= startDate && laptop.DeployedDate <= endDate)
                .Include(i => i.Createdby)
                .Include(i => i.LaptopInventory).ThenInclude(i => i.Level)
                .Include(i => i.LaptopInventory.Brand)
                .Include(i => i.LaptopInventory.OS)
                .Include(i => i.LaptopInventory.Model)
                .Include(i => i.LaptopInventory.CPU)
                .Include(i => i.LaptopInventory.Memory.Capacity)
                .Include(i => i.LaptopInventory.Status)
                .Include(i => i.LaptopInventory.HardDisk.Capacity)
                .Include(i => i.Updatedby)
                .Include(i => i.Vendor)
                .ToListAsync();

            return View(oldLaptops);
        }
        public async Task<IActionResult> FilterLaptops(DateTime? dateFrom, DateTime? dateTo)
        {
            var filteredLaptops = await _context.tbl_ictams_laptopinvdetails
                .Where(laptop => laptop.PurchaseDate >= dateFrom && laptop.PurchaseDate <= dateTo)
                .Include(i => i.Createdby)
                .Include(i => i.LaptopInventory)
                .ThenInclude(i => i.Level)
                .Include(i => i.LaptopInventory.Brand)
                .Include(i => i.LaptopInventory.OS)
                .Include(i => i.LaptopInventory.Model)
                .Include(i => i.LaptopInventory.CPU)
                .Include(i => i.LaptopInventory.Memory.Capacity)
                .Include(i => i.LaptopInventory)
                .Include(i => i.Status)
                .Include(i => i.LaptopInventory.HardDisk.Capacity)
                .Include(i => i.Updatedby)
                .Include(i => i.Vendor)
                .ToListAsync();

            return View("LaptopsView", filteredLaptops);
        }
        public async Task<IActionResult> FilterLaptopsDeploy(DateTime? dateFrom, DateTime? dateTo)
        {
            var filteredLaptops = await _context.tbl_ictams_laptopinvdetails
                .Where(laptop => laptop.DeployedDate <= dateFrom && laptop.DeployedDate >= dateTo)
                .Include(i => i.Createdby)
                .Include(i => i.LaptopInventory)
                .ThenInclude(i => i.Level)
                .Include(i => i.LaptopInventory.Brand)
                .Include(i => i.LaptopInventory.OS)
                .Include(i => i.LaptopInventory.Model)
                .Include(i => i.LaptopInventory.CPU)
                .Include(i => i.LaptopInventory.Memory.Capacity)
                .Include(i => i.LaptopInventory)
                .Include(i => i.Status)
                .Include(i => i.LaptopInventory.HardDisk.Capacity)
                .Include(i => i.Updatedby)
                .Include(i => i.Vendor)
                .ToListAsync();

            return View("LaptopsDeployView", filteredLaptops);
        }


        public async Task<IActionResult> Get6OldLaptops()
        {
            DateTime fourYearsAgo2 = DateTime.Now.AddYears(-6);

            DateTime startDate = new DateTime(fourYearsAgo2.Year, 1, 1); 
            DateTime endDate = new DateTime(fourYearsAgo2.Year, 12, 31); 

            var oldLaptops2 = await _context.tbl_ictams_laptopinvdetails
                .Where(laptop => laptop.PurchaseDate >= startDate && laptop.PurchaseDate <= endDate)
                .Include(i => i.Createdby)
                .Include(i => i.LaptopInventory).ThenInclude(i => i.Level)
                .Include(i => i.LaptopInventory.Brand)
                .Include(i => i.LaptopInventory.OS)
                .Include(i => i.LaptopInventory.Model)
                .Include(i => i.LaptopInventory.CPU)
                .Include(i => i.LaptopInventory.Memory.Capacity)
                .Include(i => i.LaptopInventory.Status)
                .Include(i => i.LaptopInventory.HardDisk.Capacity)
                .Include(i => i.Updatedby)
                .Include(i => i.Vendor)
                .ToListAsync();

            return View(oldLaptops2);
        }
        public async Task<IActionResult> Get6OldLaptopsDeploy()
        {
            DateTime fourYearsAgo2 = DateTime.Now.AddYears(-6);

            DateTime startDate = new DateTime(fourYearsAgo2.Year, 1, 1);
            DateTime endDate = new DateTime(fourYearsAgo2.Year, 12, 31);

            var oldLaptops2 = await _context.tbl_ictams_laptopinvdetails
                .Where(laptop => laptop.DeployedDate >= startDate && laptop.DeployedDate <= endDate)
                .Include(i => i.Createdby)
                .Include(i => i.LaptopInventory).ThenInclude(i => i.Level)
                .Include(i => i.LaptopInventory.Brand)
                .Include(i => i.LaptopInventory.OS)
                .Include(i => i.LaptopInventory.Model)
                .Include(i => i.LaptopInventory.CPU)
                .Include(i => i.LaptopInventory.Memory.Capacity)
                .Include(i => i.LaptopInventory.Status)
                .Include(i => i.LaptopInventory.HardDisk.Capacity)
                .Include(i => i.Updatedby)
                .Include(i => i.Vendor)
                .ToListAsync();

            return View(oldLaptops2);
        }
        public async Task<IActionResult> Get8OldLaptops()       
        {
            DateTime fourYearsAgo3 = DateTime.Now.AddYears(-8);

            DateTime startDate = new DateTime(fourYearsAgo3.Year, 1, 1); 
            DateTime endDate = new DateTime(fourYearsAgo3.Year, 12, 31); 

            var oldLaptops3 = await _context.tbl_ictams_laptopinvdetails
                .Where(laptop => laptop.PurchaseDate >= startDate && laptop.PurchaseDate <= endDate)
                .Include(i => i.Createdby)
                .Include(i => i.LaptopInventory).ThenInclude(i => i.Level)
                .Include(i => i.LaptopInventory.Brand)
                .Include(i => i.LaptopInventory.OS)
                .Include(i => i.LaptopInventory.Model)
                .Include(i => i.LaptopInventory.CPU)
                .Include(i => i.LaptopInventory.Memory.Capacity)
                .Include(i => i.LaptopInventory.Status)
                .Include(i => i.LaptopInventory.HardDisk.Capacity)
                .Include(i => i.Updatedby)
                .Include(i => i.Vendor)
                .ToListAsync();

            return View(oldLaptops3);
        }
        public async Task<IActionResult> Get8OldLaptopsDeploy()
        {
            DateTime fourYearsAgo3 = DateTime.Now.AddYears(-8);

            DateTime startDate = new DateTime(fourYearsAgo3.Year, 1, 1);
            DateTime endDate = new DateTime(fourYearsAgo3.Year, 12, 31);

            var oldLaptops3 = await _context.tbl_ictams_laptopinvdetails
                .Where(laptop => laptop.DeployedDate >= startDate && laptop.DeployedDate <= endDate)
                .Include(i => i.Createdby)
                .Include(i => i.LaptopInventory).ThenInclude(i => i.Level)
                .Include(i => i.LaptopInventory.Brand)
                .Include(i => i.LaptopInventory.OS)
                .Include(i => i.LaptopInventory.Model)
                .Include(i => i.LaptopInventory.CPU)
                .Include(i => i.LaptopInventory.Memory.Capacity)
                .Include(i => i.LaptopInventory.Status)
                .Include(i => i.LaptopInventory.HardDisk.Capacity)
                .Include(i => i.Updatedby)
                .Include(i => i.Vendor)
                .ToListAsync();

            return View(oldLaptops3);
        }
        // GET: InventoryDetails/Create
        public async Task<IActionResult> Create(string id, int typeId)
        {
            var laptop = await _context.tbl_ictams_laptopinv.FirstOrDefaultAsync(x=>x.laptoptinvCode == id);
            ViewBag.Code = id;
            ViewBag.Description = laptop.Description;
            ViewData["LTTypeId"] = typeId;
            ViewData["LTInvCode"] = new SelectList(_context.tbl_ictams_laptopinv, "laptoptinvCode", "Description");
            ViewData["LTVendor"] = new SelectList(_context.tbl_ictams_vendor, "VendorID", "VendorName");
            return View();
        }

        // POST: InventoryDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("laptoptinvCode,ComputerName,SerialCode,PO,Price,LTDVendor,ServiceYear,ComputerName,PurchaseDate,DeployedDate,LTStatus,Created,DateCreated")] InventoryDetails inventoryDetails)
        {
            try
            {
                var findSerial = await _context.tbl_ictams_laptopinvdetails.Where(x => x.SerialCode == inventoryDetails.SerialCode)
                    .FirstOrDefaultAsync();
                if (findSerial != null)
                {
                    TempData["ErrorNotification"] = "Serial Number already Exist!";
                    return RedirectToAction("Index", "LaptopInventories");
                }
                if (inventoryDetails.LTDVendor.Equals(0))
                {
                    TempData["ErrorNotification"] = "Vendor is missing,Please Select a Vendor!";
                    return RedirectToAction("Index", "LaptopInventories");
                }

                var ucode = HttpContext.Session.GetString("UserName");
                inventoryDetails.Created = ucode;
                inventoryDetails.SerialCode = inventoryDetails.SerialCode.ToUpper();
                inventoryDetails.PO = inventoryDetails.PO.ToUpper();
                inventoryDetails.LTStatus = "AV";
                inventoryDetails.ComputerName = inventoryDetails.ComputerName;
                inventoryDetails.DateCreated = DateTime.Now;
                _context.Add(inventoryDetails);

                var maxQuantityLaptop = await _context.tbl_ictams_laptopinv
                 .Where(x => x.laptoptinvCode == inventoryDetails.laptoptinvCode)
                 .MaxAsync(x => x.Quantity);

                // Now you can update the database with the incremented quantity
                var laptopToUpdate = await _context.tbl_ictams_laptopinv
                    .FirstOrDefaultAsync(x => x.laptoptinvCode == inventoryDetails.laptoptinvCode && x.Quantity == maxQuantityLaptop);

                if (laptopToUpdate != null)
                {
                    laptopToUpdate.Quantity += 1; // Increment the quantity by 1
                    await _context.SaveChangesAsync();
                }


                // ...
                TempData["SuccessNotification"] = "Successfully added a new laptop to inventory";
                // ...
                return RedirectToAction("Index", "LaptopInventories");
            }
            catch (Exception e)
            {
                return View(e.Message);
            }

        }

        // GET: InventoryDetails/Edit/5
        public async Task<IActionResult> Edit(string code, string serial)
        {
            if (code == null || serial == null || _context.tbl_ictams_laptopinvdetails == null)
            {
                return NotFound();
            }
            var inventoryDetails = await _context.tbl_ictams_laptopinvdetails.Where(c => c.laptoptinvCode == code && c.SerialCode == serial).FirstOrDefaultAsync();
            if (inventoryDetails == null)
            {
                return NotFound();
            }
            ViewData["Created"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode", inventoryDetails.Created);
            ViewData["DTStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_name", inventoryDetails.LTStatus);
            ViewData["Updated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode", inventoryDetails.Updated);
            ViewData["LTVendor"] = new SelectList(_context.tbl_ictams_vendor, "VendorID", "VendorName", inventoryDetails.LTDVendor);
            return View(inventoryDetails);
        }

        // POST: InventoryDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("laptoptinvCode,SerialCode,ComputerName,PO,Price,LTDVendor,PurchaseDate,DeployedDate,LTStatus,Created,DateCreated,Updated,UpdatedDate")] InventoryDetails inventoryDetails)
        {
            try
            {
                var existingDetail = await _context.tbl_ictams_laptopinvdetails
                   .FirstOrDefaultAsync(c => c.laptoptinvCode == inventoryDetails.laptoptinvCode && c.SerialCode == inventoryDetails.SerialCode);

                if (existingDetail == null)
                {
                    TempData["ErrorNotification"] = "Updated Failed";
                    return NotFound();
                }
                var ucode = HttpContext.Session.GetString("UserName");
                // Update specific properties
                existingDetail.PO = inventoryDetails.PO;
                existingDetail.Price = inventoryDetails.Price;
                existingDetail.LTDVendor = inventoryDetails.LTDVendor;
                existingDetail.PurchaseDate = inventoryDetails.PurchaseDate;
                existingDetail.DeployedDate = inventoryDetails.DeployedDate;
                existingDetail.LTStatus = inventoryDetails.LTStatus;
                existingDetail.Updated = ucode;
                existingDetail.UpdatedDate = DateTime.Now;

                TempData["SuccessNotification"] = "Successfully Updated";
                // Save changes
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InventoryDetailsExists(inventoryDetails.laptoptinvCode, inventoryDetails.SerialCode))
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


        [HttpPost]
        public async Task<IActionResult> Delete(string code, string serial)
        {
            var findSerial = await _context.tbl_ictams_laptopinvdetails
                                           .Where(x => x.laptoptinvCode == code && x.SerialCode == serial)
                                           .FirstOrDefaultAsync();

            if (findSerial != null)
            {
                _context.tbl_ictams_laptopinvdetails.Remove(findSerial);

                var maxQuantityLaptop = await _context.tbl_ictams_laptopinv
                                                     .Where(x => x.laptoptinvCode == findSerial.laptoptinvCode)
                                                     .MaxAsync(x => x.Quantity);

                var laptopToUpdate = await _context.tbl_ictams_laptopinv
                                                   .FirstOrDefaultAsync(x => x.laptoptinvCode == findSerial.laptoptinvCode && x.Quantity == maxQuantityLaptop);
                try
                {
                    if (laptopToUpdate != null)
                    {
                        laptopToUpdate.Quantity = laptopToUpdate.Quantity - 1;
                        await _context.SaveChangesAsync();
                        TempData["SuccessNotification"] = "Successfully removed a laptop from inventory";
                    }
                }
                catch (Exception)
                {
                    TempData["ErrorNotification"] = "It cannot be deleted since it is already in used!";
                }
            }
            else
            {
                TempData["ErrorNotification"] = "Laptop not found";
            }

            return RedirectToAction("Index", "LaptopInventories");
        }

        private bool InventoryDetailsExists(string code, string serial)
        {
            return (_context.tbl_ictams_laptopinvdetails?.Any(e => e.laptoptinvCode == code && e.SerialCode == serial)).GetValueOrDefault();
        }
    }
}
