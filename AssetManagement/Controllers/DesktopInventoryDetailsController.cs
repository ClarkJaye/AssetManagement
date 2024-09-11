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
    public class DesktopInventoryDetailsController : Controller
    {
        private readonly AssetManagementContext _context;

        public DesktopInventoryDetailsController(AssetManagementContext context)
        {
            _context = context;
        }

        // GET: DesktopInventoryDetails
        public async Task<IActionResult> Index()
        {
            var assetManagementContext = _context.tbl_ictams_desktopinvdetails.Where(x=>x.DTStatus == "AV").Include(d => d.Createdby).Include(d => d.Status).Include(d => d.Updatedby).Include(d => d.Vendor);
            return View(await assetManagementContext.ToListAsync());
        }

        // GET: DesktopInventoryDetails/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.tbl_ictams_desktopinvdetails == null)
            {
                return NotFound();
            }

            var desktopInventoryDetail = await _context.tbl_ictams_desktopinvdetails
                .Include(d => d.Createdby)
                .Include(d => d.Status)
                .Include(d => d.Updatedby)
                .Include(d => d.Vendor)
                .FirstOrDefaultAsync(m => m.unitTag == id);
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
            return View();
        }

        // POST: DesktopInventoryDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("desktopInvCode,unitTag,PO,Price,DTDVendor,PurchaseDate,DeployedDate,DTStatus,Created,DateCreated,Updated,UpdatedDate")] DesktopInventoryDetail desktopInventoryDetail)
        {
            try
            {
                var findunitTag = await _context.tbl_ictams_desktopinvdetails.Where(x => x.unitTag == desktopInventoryDetail.unitTag)
                    .FirstOrDefaultAsync();
                if (findunitTag != null)
                {
                    TempData["ErrorNotification"] = "Serial Number already Exist!";
                    return RedirectToAction("Index", "DesktopInventories");
                }
                if (desktopInventoryDetail.DTDVendor.Equals(0))
                {
                    TempData["ErrorNotification"] = "Vendor is missing,Please Select a Vendor!";
                    return RedirectToAction("Index", "DesktopInventories");
                }

                var ucode = HttpContext.Session.GetString("UserName");
                desktopInventoryDetail.Created = ucode;
                desktopInventoryDetail.unitTag = desktopInventoryDetail.unitTag.ToUpper();
                desktopInventoryDetail.PO = desktopInventoryDetail.PO.ToUpper();
                desktopInventoryDetail.DTStatus = "AV";
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
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.tbl_ictams_desktopinvdetails == null)
            {
                return NotFound();
            }

            var desktopInventoryDetail = await _context.tbl_ictams_desktopinvdetails.FindAsync(id);
            if (desktopInventoryDetail == null)
            {
                return NotFound();
            }
            ViewData["Created"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode", desktopInventoryDetail.Created);
            ViewData["DTStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_code", desktopInventoryDetail.DTStatus);
            ViewData["Updated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode", desktopInventoryDetail.Updated);
            ViewData["DTDVendor"] = new SelectList(_context.tbl_ictams_vendor, "VendorID", "VendorID", desktopInventoryDetail.DTDVendor);
            return View(desktopInventoryDetail);
        }

        // POST: DesktopInventoryDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("desktopInvCode,unitTag,PO,Price,DTDVendor,PurchaseDate,DeployedDate,DTStatus,Created,DateCreated,Updated,UpdatedDate")] DesktopInventoryDetail desktopInventoryDetail)
        {
            if (id != desktopInventoryDetail.unitTag)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(desktopInventoryDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DesktopInventoryDetailExists(desktopInventoryDetail.unitTag))
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
            return View(desktopInventoryDetail);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var findSerial = await _context.tbl_ictams_desktopinvdetails
                                           .Where(x => x.unitTag == id)
                                           .FirstOrDefaultAsync();

            if (findSerial != null)
            {
                _context.tbl_ictams_desktopinvdetails.Remove(findSerial);

                var maxQuantityLaptop = await _context.tbl_ictams_desktopinv
                                                     .Where(x => x.desktopInvCode == findSerial.desktopInvCode)
                                                     .MaxAsync(x => x.Quantity);

                var laptopToUpdate = await _context.tbl_ictams_desktopinv
                                                   .FirstOrDefaultAsync(x => x.desktopInvCode == findSerial.desktopInvCode && x.Quantity == maxQuantityLaptop);
                try
                {
                    if (laptopToUpdate != null)
                    {
                        laptopToUpdate.Quantity = Math.Max(0, laptopToUpdate.Quantity - 1);
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

        private bool DesktopInventoryDetailExists(string id)
        {
          return (_context.tbl_ictams_desktopinvdetails?.Any(e => e.unitTag == id)).GetValueOrDefault();
        }
    }
}
