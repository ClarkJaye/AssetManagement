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
    public class LaptopPeripheralsController : BaseController
    {
        private readonly AssetManagementContext _context;

        public LaptopPeripheralsController(AssetManagementContext context)
        : base(context) // Pass the 'context' dependency to the base constructor
        {
            _context = context;
        }

        public async Task<IActionResult> ReturnDetails(string userCODE)
        {
            if (!string.IsNullOrEmpty(userCODE))
            {
                var assetManagementContext = _context.tbl_ictams_ltpareturn.Where(x => x.LaptopPeripheralAllocation.PeripheralCode.Contains(userCODE)).Include(r => r.LaptopPeripheralAllocation).Include(r => r.ReturnType).Include(r => r.Status).Include(r => r.UserCreated).Include(r => r.UserUpdated);
                return View(await assetManagementContext.ToListAsync());
            }
            return View();
        }
        public async Task<IActionResult> Inactive()
        {
           await FindStatusLTP();
            var assetManagementContext = _context.tbl_ictams_ltperipheral.Where(x => x.PeripheralStatus == "IN").Include(l => l.Brand).Include(l => l.CreatedBy).Include(l => l.DeviceType).Include(l => l.Status).Include(l => l.UpdatedBy).Include(l => l.Vendor);
            return View(await assetManagementContext.ToListAsync());
        }
        // GET: LaptopPeripheralAllocation
        public async Task<IActionResult> InventoryDetails(string id, string ids, string id2)
        {
            ViewBag.Id = id;
            ViewBag.Ids = ids;
            ViewBag.Id2 = id2;

            var assetManagementContext = _context.tbl_ictams_ltperipheralalloc.Where(x => x.PeripheralAllocStatus == "AC" && x.PeripheralCode == id).Include(l => l.Status).Include(l => l.CreatedBy).Include(l => l.LaptopPeripheral).Include(l => l.Owner).Include(l => l.UpdatedBy);
            return View(await assetManagementContext.ToListAsync());
        }
        public async Task<IActionResult> SecOwnerPeripDetails(string userCODE)
        {
            if (!string.IsNullOrEmpty(userCODE))
            {
                var assetManagementContext = await _context.tbl_ictams_ltpsecowner.Where(x => x.SecAllocationStatus == "AC" && x.SecPeripheralCode.Contains(userCODE)).Include(s => s.CreatedBy).Include(s => s.LaptopPeripAlloc).Include(s => s.LaptopPeripheral).Include(s => s.Owner).Include(s => s.Status).Include(s => s.UpdatedBy).ToListAsync();

                return View(assetManagementContext);
            }

            return View();
        }
        public async Task<IActionResult> LTPeripPartialView()
        {
            var assetManagementContext = _context.tbl_ictams_ltperipheral
                .Where(lp => lp.PeripheralStatus == "AV")
                .Include(l => l.Brand)
                .Include(l => l.CreatedBy)
                .Include(l => l.DeviceType)
                .Include(l => l.Status)
                .Include(l => l.UpdatedBy)
                .Include(l => l.Vendor);

            return View(await assetManagementContext.ToListAsync());
        }

        // GET: LaptopPeripherals
        public async Task<IActionResult> Index()
        {
            var ucode = HttpContext.Session.GetString("UserName");

            var findPass = await _context.tbl_ictams_users.Where(x => x.UserCode == ucode).FirstOrDefaultAsync();

            var PasswordIsCorrect = BCrypt.Net.BCrypt.Verify("1234", findPass.UserPassword);
            if (PasswordIsCorrect)
            {
                // Show success alert using SweetAlert
                TempData["AlertType"] = "success";
                TempData["SuccessMessage"] = "Login successfully!";
                return RedirectToAction("ChangePassword", "Users");
            }
            int? userProfile = HttpContext.Session.GetInt32("UserProfile");
            if (userProfile.HasValue)
            {

                var hasOpenAccess = await _context.tbl_ictams_profileaccess
          .AnyAsync(pa => pa.OpenAccess == "Y" &&
                          pa.Module.ModuleTitle == "Laptop Peripherals" &&  // Adjust the module name as needed
                          pa.ProfileId == userProfile.Value);
                if (!hasOpenAccess)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    await FindStatusLTP();
                    // Count total laptops
                    var totallPeripherals = await _context.tbl_ictams_ltperipheral.SumAsync(x => x.PeripheralQty);
                    var countAvailPer = await _context.tbl_ictams_ltperipheral.Where(x => x.PeripheralQty > x.PeripheralAllocation && x.PeripheralStatus == "AV").SumAsync(x => x.PeripheralQty - x.PeripheralAllocation);
                    var totalAllocatedPer = await _context.tbl_ictams_ltperipheral.SumAsync(x => x.PeripheralAllocation);

                    var assetManagementContext = await _context.tbl_ictams_ltperipheral.Where(LP => LP.PeripheralStatus == "AV" || LP.PeripheralStatus == "CO").Include(l => l.Brand).Include(l => l.CreatedBy).Include(l => l.DeviceType).Include(l => l.Status).Include(l => l.UpdatedBy).Include(l => l.Vendor).ToListAsync();

                    ViewBag.TotalPeripherals = totallPeripherals;
                    ViewBag.TotalAvailablePer = countAvailPer;
                    ViewBag.TotalAllocatedPer = totalAllocatedPer;
                    return View(assetManagementContext);
                }
            }

            return RedirectToAction("Index", "Home");

        }

        public async Task<IActionResult> DeleteAsEdit(string peripheralCode)
        {
            ViewData["UserStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_name");

            // Retrieve the peripheral with the specified PeripheralCode
            var peripheral = await _context.tbl_ictams_ltperipheral
                .FirstOrDefaultAsync(p => p.PeripheralCode == peripheralCode);

            if (peripheral == null)
            {
                TempData["AlertMessage"] = "Peripheral not found.";
                return RedirectToAction(nameof(Index));
            }

            // Check if the peripheral is allocated to any owner
            var isAllocated = await _context.tbl_ictams_ltperipheralalloc
                .AnyAsync(x => x.PeripheralCode == peripheralCode);

            if (isAllocated)
            {
                TempData["AlertMessage"] = "Cannot be deleted. It is already allocated to a specific owner!";
                return RedirectToAction(nameof(Index));
            }

            // Update the peripheral details to mark it as 'deleted'
            peripheral.PeripheralUpdatedBy = HttpContext.Session.GetString("UserName");
            peripheral.PeripheralStatus = "IN"; // Mark it as 'IN'
            peripheral.PeripheralUpdatedAt = DateTime.Now;

            _context.Update(peripheral);
            await _context.SaveChangesAsync();

            TempData["SuccessNotification"] = "Successfully deleted!";
            return RedirectToAction(nameof(Index));
        }


        //public async Task<IActionResult> DeleteAsEdit(string[] selectedIds)
        //{
        //    ViewData["UserStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_name");
        //    var peripheralCodes = await _context.tbl_ictams_ltperipheral
        //        .Where(p => selectedIds.Contains(p.PeripheralCode))
        //        .ToListAsync();

        //    if (peripheralCodes.Count == 0)
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }

        //    var ltinvExist = await _context.tbl_ictams_ltperipheralalloc
        //        .AnyAsync(x => selectedIds.Contains(x.PeripheralCode));

        //    if (ltinvExist)
        //    {
        //        TempData["AlertMessage"] = "Cannot be deleted. It is already allocated to the specific owner!";
        //        return RedirectToAction(nameof(Index));
        //    }

        //    foreach (var peripheralCode in peripheralCodes)
        //    {
        //        peripheralCode.PeripheralUpdatedBy = HttpContext.Session.GetString("UserName");
        //        peripheralCode.PeripheralStatus = "IN";
        //        peripheralCode.PeripheralUpdatedAt = DateTime.Now;

        //        _context.Update(peripheralCode);
        //    }

        //    await _context.SaveChangesAsync();

        //    TempData["SuccessNotification"] = "Successfully deleted!";
        //    return RedirectToAction(nameof(Index));
        //}

        public async Task<IActionResult> Retrieve(string[] selectedIds)
        {
            ViewData["UserStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_name");
            var peripheralCodes = await _context.tbl_ictams_ltperipheral
                .Where(p => selectedIds.Contains(p.PeripheralCode))
                .ToListAsync();

            if (peripheralCodes.Count == 0)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var peripheralCode in peripheralCodes)
            {
                peripheralCode.PeripheralUpdatedBy = HttpContext.Session.GetString("UserName");
                peripheralCode.PeripheralStatus = "AV";
                peripheralCode.PeripheralUpdatedAt = DateTime.Now;

                _context.Update(peripheralCode);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessNotification"] = "Successfully retrieved!";
            return RedirectToAction(nameof(Index));
        }


        // GET: LaptopPeripherals/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.tbl_ictams_ltperipheral  == null)
            {
                return NotFound();
            }

            var laptopPeripheral = await _context.tbl_ictams_ltperipheral
                .Include(l => l.Brand)
                .Include(l => l.CreatedBy)
                .Include(l => l.DeviceType)
                .Include(l => l.Status)
                .Include(l => l.UpdatedBy)
                .Include(l => l.Vendor)
                .FirstOrDefaultAsync(m => m.PeripheralCode == id);
            if (laptopPeripheral == null)
            {
                return NotFound();
            }

            return View(laptopPeripheral);
        }
      

        // GET: LaptopPeripherals/Create
        public IActionResult Create()
        {
            ViewData["PeripheralBrand"] = new SelectList(_context.tbl_ictams_brand, "BrandId", "BrandId");
            ViewData["PeripheralCreatedBy"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode");
            ViewData["PeripheralDevice"] = new SelectList(_context.tbl_ictams_devicetype, "DevtypeID", "DevtypeID");
            ViewData["PeripheralStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_code");
            ViewData["PeripheralUpdatedBy"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode");
            ViewData["PeripheralVendor"] = new SelectList(_context.tbl_ictams_vendor, "VendorID", "VendorID");
            return View();
        }

        // POST: LaptopPeripherals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PeripheralCode,PeripheralDescription,PeripheralBrand,PeripheralDevice,PeripheralVendor,PeripheralQty,PeripheralAllocation,PeripheralStatus,PeripheralCreatedBy,PeripheralCreatedAt,PeripheralUpdatedBy,PeripheralUpdatedAt")] LaptopPeripheral laptopPeripheral)
        {
            if (laptopPeripheral.PeripheralBrand.Equals(0))
            {
                TempData["AlertMessage"] = "Please select a brand";
                return View(laptopPeripheral);
            }
            if (laptopPeripheral.PeripheralDevice.Equals(0))
            {
                TempData["AlertMessage"] = "Please select a device!";
                return View(laptopPeripheral);
            }
            if (laptopPeripheral.PeripheralVendor.Equals(0))
            {
                TempData["AlertMessage"] = "Please select a vendor!";
                return View(laptopPeripheral);
            }
            else
            {
                var paramCode = await _context.tbl_ictams_parameters.Where(p => p.parm_code == "ltp_id").MaxAsync(p => p.parm_value);
                var newparamCode = paramCode + 1;
                var param = await _context.tbl_ictams_parameters.FirstOrDefaultAsync(p => p.parm_code == "ltp_id");
                param.parm_value = newparamCode;

                var ucode = HttpContext.Session.GetString("UserName");
                laptopPeripheral.PeripheralCode = "LTP" + newparamCode.ToString().PadLeft(7, '0');
                laptopPeripheral.PeripheralDescription = laptopPeripheral.PeripheralDescription.ToUpper();
                laptopPeripheral.PeripheralCreatedBy = ucode;
                laptopPeripheral.PeripheralCreatedAt = DateTime.Now;
                laptopPeripheral.PeripheralStatus = "AV";

                TempData["SuccessNotification"] = "Successfully added a new device to the inventory!";

                _context.Add(laptopPeripheral);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            
        }

        // GET: LaptopPeripherals/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var laptopPeripheral = await _context.tbl_ictams_ltperipheral
                .Include(lp => lp.Brand)
                .Include(lp => lp.DeviceType)
                .Include(lp => lp.Vendor)
                .FirstOrDefaultAsync(lp => lp.PeripheralCode == id);

            if (laptopPeripheral == null)
            {
                return NotFound();
            }
            return View(laptopPeripheral);
        }

        // POST: LaptopPeripherals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("PeripheralCode,PeripheralDescription,PeripheralBrand,PeripheralDevice,PeripheralVendor,PeripheralQty,PeripheralAllocation,PeripheralStatus,PeripheralCreatedBy,PeripheralCreatedAt,PeripheralUpdatedBy,PeripheralUpdatedAt")] LaptopPeripheral laptopPeripheral)
        {
            if (id != laptopPeripheral.PeripheralCode)
            {
                return NotFound();
            }
            var maxAllocatedNo = await _context.tbl_ictams_ltperipheral
            .Where(x => x.PeripheralCode == id)
            .MaxAsync(x => x.PeripheralAllocation);

            if (laptopPeripheral.PeripheralQty < maxAllocatedNo)
            {
                TempData["AlertMessage"] = "Must not be below the existing quantity!";
                return RedirectToAction("Edit");
            }
            try
            {
                var ucode = HttpContext.Session.GetString("UserName");
                laptopPeripheral.PeripheralUpdatedBy = ucode;
                laptopPeripheral.PeripheralUpdatedAt = DateTime.Now;
                laptopPeripheral.PeripheralStatus = "AV";
                _context.Update(laptopPeripheral);
                 await _context.SaveChangesAsync();

                TempData["SuccessNotification"] = "Successfully edited!";

            }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LaptopPeripheralExists(laptopPeripheral.PeripheralCode))
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

        // GET: LaptopPeripherals/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.tbl_ictams_ltperipheral  == null)
            {
                return NotFound();
            }

            var laptopPeripheral = await _context.tbl_ictams_ltperipheral
                .Include(l => l.Brand)
                .Include(l => l.CreatedBy)
                .Include(l => l.DeviceType)
                .Include(l => l.Status)
                .Include(l => l.UpdatedBy)
                .Include(l => l.Vendor)
                .FirstOrDefaultAsync(m => m.PeripheralCode == id);
            if (laptopPeripheral == null)
            {
                return NotFound();
            }

            return View(laptopPeripheral);
        }

        // POST: LaptopPeripherals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.tbl_ictams_ltperipheral  == null)
            {
                return Problem("Entity set 'AssetManagementContext.LaptopPeripheral'  is null.");
            }
            var laptopPeripheral = await _context.tbl_ictams_ltperipheral.FindAsync(id);
            if (laptopPeripheral != null)
            {
                _context.tbl_ictams_ltperipheral.Remove(laptopPeripheral);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LaptopPeripheralExists(string id)
        {
          return (_context.tbl_ictams_ltperipheral?.Any(e => e.PeripheralCode == id)).GetValueOrDefault();
        }
    }
}
