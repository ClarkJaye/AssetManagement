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
    public class DesktopInventoryDetailsController : BaseController
    {
        private readonly AssetManagementContext _context;

        public DesktopInventoryDetailsController(AssetManagementContext context)
        : base(context)
        {
            _context = context;
        }

        // GET: DesktopInventoryDetails
        public async Task<IActionResult> Index()
        {
            var assetManagementContext = _context.tbl_ictams_desktopinvdetails.Include(d => d.Createdby).Include(d => d.Status).Include(d => d.Updatedby).Include(d => d.Vendor);
            return View(await assetManagementContext.ToListAsync());
        }

        // GET: DesktopInventoryDetails/Details/5
        public async Task<IActionResult> Details(string code, string unitTag)
        {
            if (code == null || unitTag == null || _context.tbl_ictams_desktopinvdetails == null)
            {
                return NotFound();
            }

            var desktopInventoryDetail = await _context.tbl_ictams_desktopinvdetails
                .Include(d => d.Createdby)
                .Include(d => d.Status)
                .Include(d => d.Updatedby)
                .Include(d => d.Vendor)
                .FirstOrDefaultAsync(m => m.desktopInvCode == code && m.unitTag == unitTag);
            if (desktopInventoryDetail == null)
            {
                return NotFound();
            }

            return View(desktopInventoryDetail);
        }
        public async Task<IActionResult> SeeAllSerial(string id)
        {
            var assetManagementContext = _context.tbl_ictams_desktopinvdetails.Where(x => x.desktopInvCode == id && x.DTStatus == "AV").Include(i => i.Createdby).Include(i => i.DesktopInventory).Include(i => i.Status).Include(i => i.Updatedby).Include(i => i.Vendor);
            return View(await assetManagementContext.ToListAsync());
        }

        // GET: DesktopInventoryDetails/Create
        public IActionResult Create(string id)
        {
            ViewBag.Code = id;
            ViewData["DTInvCode"] = new SelectList(_context.tbl_ictams_desktopinv, "desktopInvCode", "desktopInvCode");
            ViewData["DTVendor"] = new SelectList(_context.tbl_ictams_vendor, "VendorID", "VendorName");
            return View();
        }

        // POST: DesktopInventoryDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("desktopInvCode,unitTag,ComputerName,PO,Price,DTDVendor,PurchaseDate,DeployedDate,DTStatus,Created,DateCreated,Updated,UpdatedDate")] DesktopInventoryDetail desktopInventoryDetail)
        {
            try
            {
                var findunitTag = await _context.tbl_ictams_desktopinvdetails.Where(x => x.desktopInvCode == desktopInventoryDetail.desktopInvCode && x.unitTag == desktopInventoryDetail.unitTag)
                    .FirstOrDefaultAsync();
                if (findunitTag != null)
                {
                    TempData["ErrorNotification"] = "Unit Tag already Exist!";
                    return RedirectToAction(nameof(Index));
                }
                if (desktopInventoryDetail.DTDVendor.Equals(0))
                {
                    TempData["ErrorNotification"] = "Vendor is missing,Please Select a Vendor!";
                    return RedirectToAction(nameof(Index));
                }

                var ucode = HttpContext.Session.GetString("UserName");
                desktopInventoryDetail.Created = ucode;
                desktopInventoryDetail.unitTag = desktopInventoryDetail.unitTag.ToUpper();
                desktopInventoryDetail.PO = desktopInventoryDetail.PO.ToUpper();
                desktopInventoryDetail.DTStatus = "AV";
                desktopInventoryDetail.ComputerName = desktopInventoryDetail.ComputerName;
                desktopInventoryDetail.DateCreated = DateTime.Now;
                _context.Add(desktopInventoryDetail);

                var maxQuantityLaptop = await _context.tbl_ictams_desktopinv
                 .Where(x => x.desktopInvCode == desktopInventoryDetail.desktopInvCode)
                 .MaxAsync(x => x.Quantity);

                // Now you can update the database with the incremented quantity
                var laptopToUpdate = await _context.tbl_ictams_desktopinv
                    .FirstOrDefaultAsync(x => x.desktopInvCode == desktopInventoryDetail.desktopInvCode && x.Quantity == maxQuantityLaptop);

                if (laptopToUpdate != null)
                {
                    laptopToUpdate.Quantity += 1; // Increment the quantity by 1
                    await _context.SaveChangesAsync();
                }

                // ...
                TempData["SuccessNotification"] = "Successfully added a new desktop to inventory";
                // ...
                return RedirectToAction("Index", "DesktopInventories");
            }
            catch (Exception e)
            {
                return View(e.Message);
            }
        }

        // GET: DesktopInventoryDetails/Edit/5
        public async Task<IActionResult> Edit(string code, string unitTag)
        {
            if (code == null || unitTag == null || _context.tbl_ictams_desktopinvdetails == null)
            {
                return NotFound();
            }

            var desktopInventoryDetail = await _context.tbl_ictams_desktopinvdetails.Where(c=> c.desktopInvCode == code && c.unitTag == unitTag).FirstOrDefaultAsync();
            if (desktopInventoryDetail == null)
            {
                return NotFound();
            }
            ViewData["Created"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode", desktopInventoryDetail.Created);
            ViewData["DTStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_name", desktopInventoryDetail.DTStatus);
            ViewData["Updated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode", desktopInventoryDetail.Updated);
            ViewData["DTDVendor"] = new SelectList(_context.tbl_ictams_vendor, "VendorID", "VendorName", desktopInventoryDetail.DTDVendor);
            return View(desktopInventoryDetail);
        }

        // POST: DesktopInventoryDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("desktopInvCode,unitTag,PO,Price,DTDVendor,PurchaseDate,DeployedDate,DTStatus,Created,DateCreated,Updated,UpdatedDate")] DesktopInventoryDetail desktopInventoryDetail)
        {

            try
            {
                // Retrieve the existing record to update
                var existingDetail = await _context.tbl_ictams_desktopinvdetails
                    .FirstOrDefaultAsync(c => c.desktopInvCode == desktopInventoryDetail.desktopInvCode && c.unitTag == desktopInventoryDetail.unitTag);

                if (existingDetail == null)
                {
                    TempData["ErrorNotification"] = "Updated Failed";
                    return NotFound();
                }
                var ucode = HttpContext.Session.GetString("UserName");
                // Update specific properties
                existingDetail.PO = desktopInventoryDetail.PO;
                existingDetail.Price = desktopInventoryDetail.Price;
                existingDetail.DTDVendor = desktopInventoryDetail.DTDVendor;
                existingDetail.PurchaseDate = desktopInventoryDetail.PurchaseDate;
                existingDetail.DeployedDate = desktopInventoryDetail.DeployedDate;
                existingDetail.DTStatus = desktopInventoryDetail.DTStatus;
                existingDetail.Updated = ucode;
                existingDetail.UpdatedDate = DateTime.Now;

                TempData["SuccessNotification"] = "Successfully Updated";
                // Save changes
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DesktopInventoryDetailExists(desktopInventoryDetail.desktopInvCode, desktopInventoryDetail.unitTag))
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
        public async Task<IActionResult> Delete(string code, string unitTag)
        {
            var findSerial = await _context.tbl_ictams_desktopinvdetails
                                           .Where(x => x.desktopInvCode == code && x.unitTag == unitTag)
                                           .FirstOrDefaultAsync();

            if (findSerial != null)
            {
                var maxQuantityLaptop = await _context.tbl_ictams_desktopinv
                                                     .Where(x => x.desktopInvCode == findSerial.desktopInvCode)
                                                     .MaxAsync(x => x.Quantity);

                var laptopToUpdate = await _context.tbl_ictams_desktopinv
                                                   .FirstOrDefaultAsync(x => x.desktopInvCode == findSerial.desktopInvCode && x.Quantity == maxQuantityLaptop);
                try
                {
                    if (laptopToUpdate != null)
                    {
                        var ucode = HttpContext.Session.GetString("UserName");
                        laptopToUpdate.Quantity = laptopToUpdate.Quantity - 1;
                        findSerial.DTStatus = "IN";
                        findSerial.UpdatedDate = DateTime.Now;
                        findSerial.Updated = ucode;
                        await _context.SaveChangesAsync();
                        // ...
                        TempData["SuccessNotification"] = "Successfully removed a laptop from inventory";
                        // ...
                    }
                    else
                    {
                        TempData["ErrorNotification"] = "No laptops found in inventory";
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

            return RedirectToAction("Index", "DesktopInventories");
        }

        private bool DesktopInventoryDetailExists(string code, string unitTag)
        {
          return (_context.tbl_ictams_desktopinvdetails?.Any(e => e.desktopInvCode == code && e.unitTag == unitTag)).GetValueOrDefault();
        }
    }
}
