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
    public class LaptopPeripheralAllocationsController : BaseController
    {
        private readonly AssetManagementContext _context;

        public LaptopPeripheralAllocationsController(AssetManagementContext context)
        : base(context) // Pass the 'context' dependency to the base constructor
        {
            _context = context;
        }
    
        public async Task<IActionResult> Inactive()
        {
            await FindStatusLTP();
            var assetManagementContext = _context.tbl_ictams_ltperipheralalloc.Where(l => l.PeripheralAllocStatus == "IN").Include(l => l.CreatedBy).Include(l => l.LaptopPeripheral).Include(l => l.Owner).Include(l => l.Status).Include(l => l.UpdatedBy);
            return View(await assetManagementContext.ToListAsync());

        }

        public async Task<IActionResult> PeripheralViews()
        {
            await FindStatusLTP();
            var assetManagementContext = _context.tbl_ictams_ltperipheralalloc.Where(l => l.PeripheralAllocStatus == "IN").Include(l => l.CreatedBy).Include(l => l.LaptopPeripheral).Include(l => l.Owner).Include(l => l.Status).Include(l => l.UpdatedBy);
            return View(await assetManagementContext.ToListAsync());

        }

        // GET: LaptopPeripheralAllocations
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
                          pa.Module.ModuleTitle == "Laptop Peripheral Allocations" &&  // Adjust the module name as needed
                          pa.ProfileId == userProfile.Value);
                if (!hasOpenAccess)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    await FindStatusLTP();

                    var totalPeripherals = await _context.tbl_ictams_ltperipheral.SumAsync(x => x.PeripheralQty);
                    var totalActive = await _context.tbl_ictams_ltperipheralalloc.CountAsync(x => x.PeripheralAllocStatus == "AC");
                    var totalNotAlloc = await _context.tbl_ictams_ltperipheral.SumAsync(x => x.PeripheralQty - x.PeripheralAllocation);

                    ViewBag.TotalPer = totalPeripherals;
                    ViewBag.TotalAllocate = totalActive;
                    ViewBag.TotalNotAllocate = totalNotAlloc;

                    var assetManagementContext = _context.tbl_ictams_ltperipheralalloc.Where(l => l.PeripheralAllocStatus == "AC").Include(l => l.CreatedBy).Include(l => l.LaptopPeripheral).Include(l => l.Owner).Include(l => l.Status).Include(l => l.UpdatedBy);
                    return View(await assetManagementContext.ToListAsync());
                }
            }

            return RedirectToAction("Index", "Home");
            
        }

        public async Task<IActionResult> ReturnPeripPartialView()
        {
            await FindStatusLTP();
            var totalActiveLaptops = await _context.tbl_ictams_ltperipheralalloc.CountAsync(x => x.PeripheralAllocStatus == "AC");
            var totalInactiveLaptops = await _context.tbl_ictams_ltperipheralalloc.CountAsync(x => x.PeripheralAllocStatus == "IN");

            ViewBag.TotalActiveLaptops = totalActiveLaptops;
            ViewBag.TotalInactiveLaptops = totalInactiveLaptops;
            var assetManagementContext = _context.tbl_ictams_ltperipheralalloc.Where(l => l.PeripheralAllocStatus == "AC").Include(l => l.CreatedBy).Include(l => l.LaptopPeripheral).Include(l => l.Owner).Include(l => l.Status).Include(l => l.UpdatedBy);
            return View(await assetManagementContext.ToListAsync());
        }

        public async Task<IActionResult> LapPeripAllocPartialView()
        {
            await FindStatusLTP();
            var totalActiveLaptops = await _context.tbl_ictams_ltperipheralalloc.CountAsync(x => x.PeripheralAllocStatus == "AC");
            var totalInactiveLaptops = await _context.tbl_ictams_ltperipheralalloc.CountAsync(x => x.PeripheralAllocStatus == "IN");

            ViewBag.TotalActiveLaptops = totalActiveLaptops;
            ViewBag.TotalInactiveLaptops = totalInactiveLaptops;
            var assetManagementContext = _context.tbl_ictams_ltperipheralalloc.Where(l => l.PeripheralAllocStatus == "AC").Include(l => l.CreatedBy).Include(l => l.LaptopPeripheral).Include(l => l.Owner).Include(l => l.Status).Include(l => l.UpdatedBy);
            return View(await assetManagementContext.ToListAsync());
        }

        // GET: LaptopPeripheralAllocations/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.tbl_ictams_ltperipheralalloc == null)
            {
                return NotFound();
            }

            var laptopPeripheralAllocation = await _context.tbl_ictams_ltperipheralalloc
                .Include(l => l.CreatedBy)
                .Include(l => l.LaptopPeripheral)
                .Include(l => l.Owner)
                .Include(l => l.Status)
                .Include(l => l.UpdatedBy)
                .FirstOrDefaultAsync(m => m.PeripheralAllocationId == id);
            if (laptopPeripheralAllocation == null)
            {
                return NotFound();
            }

            return View(laptopPeripheralAllocation);
        }
        public async Task<IActionResult> DeleteAsEdit(string[] selectedIds)
        {
            ViewData["UserStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_name");
            foreach (string id in selectedIds)
            {
                var allocationID = await _context.tbl_ictams_ltperipheralalloc.FindAsync(id);
                if (allocationID != null)
                {
                    // Check if the selected profile status exists in the tbl_lsm_status table
                    var status = await _context.tbl_ictams_status.FindAsync(allocationID.PeripheralAllocationId);
                    if (status == null)
                    {
                        ModelState.AddModelError("", $"Profile status '{allocationID.PeripheralAllocationId}' does not exist.");
                    }

                    // Find the corresponding LaptopCode in tbl_ictams_laptopinv and deduct 1 from AllocatedNo
                    var laptopCode = allocationID.PeripheralCode;
                    var laptopInv = await _context.tbl_ictams_ltperipheral.FirstOrDefaultAsync(a => a.PeripheralCode == laptopCode);
                    if (laptopInv != null)
                    {
                        laptopInv.PeripheralAllocation -= 1;
                    }

                    // Update the profile
                    var ucode = HttpContext.Session.GetString("UserName");
                    allocationID.PeripheralAllocUpdatedBy = ucode;
                    allocationID.PeripheralAllocStatus = "IN";
                    allocationID.PeripheralAllocUpdateAt = DateTime.Now;

                    _context.Update(allocationID);
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
                var allocationID = await _context.tbl_ictams_ltperipheralalloc.FindAsync(id);
                if (allocationID != null)
                {
                    // Check if the selected profile status exists in the tbl_lsm_status table
                    var status = await _context.tbl_ictams_status.FindAsync(allocationID.PeripheralAllocationId);
                    if (status == null)
                    {
                        ModelState.AddModelError("", $"Profile status '{allocationID.PeripheralAllocationId}' does not exist.");
                    }

                    var laptopCode = allocationID.PeripheralCode;

                    // Find the corresponding LaptopCode in tbl_ictams_laptopinv
                    var laptopInv = await _context.tbl_ictams_ltperipheral.FirstOrDefaultAsync(a => a.PeripheralCode == laptopCode);

                    if (laptopInv != null)
                    {
                        // Check if the Quantity and AllocatedNo are equal
                        if (laptopInv.PeripheralQty == laptopInv.PeripheralAllocation && laptopInv.PeripheralStatus == "CO")
                        {
                            TempData["AlertMessage"] = "Cannot retrieve. It is already completed!";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            // Deduct 1 from AllocatedNo
                            laptopInv.PeripheralAllocation += 1;
                        }
                    }

                    // Update the profile
                    var ucode = HttpContext.Session.GetString("UserName");
                    allocationID.PeripheralAllocUpdatedBy = ucode;
                    allocationID.PeripheralAllocStatus = "AC";
                    allocationID.PeripheralAllocUpdateAt = DateTime.Now;

                    _context.Update(allocationID);
                    await _context.SaveChangesAsync();

                    // ...
                    TempData["SuccessNotification"] = "Successfully retrieved!  ";
                    // ...
                    return RedirectToAction(nameof(Index));
                }
            }

            return RedirectToAction(nameof(Index));
        }


        // GET: LaptopPeripheralAllocations/Create
        public IActionResult Create()
        {
            ViewData["PeripheralAllocCreatedBy"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode");
            ViewData["PeripheralCode"] = new SelectList(_context.tbl_ictams_ltperipheral, "PeripheralCode", "PeripheralCode");
            ViewData["OwnerCode"] = new SelectList(_context.tbl_ictams_owners, "OwnerCode", "OwnerCode");
            ViewData["PeripheralAllocStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_code");
            ViewData["PeripheralAllocUpdatedBy"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode");
            return View();
        }

        // POST: LaptopPeripheralAllocations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PeripheralAllocationId,PeripheralCode,OwnerCode,SerialNumber,FixedAssetTag,DatePurchased,AgeYears,PONumber,PeripheralAllocStatus,PeripheralAllocCreatedBy,PeripheralAllocCreatedAt,PeripheralAllocUpdatedBy,PeripheralAllocUpdateAt,UnitPrice")] LaptopPeripheralAllocation laptopPeripheralAllocation)
        {
            bool descriptionExists = await _context.tbl_ictams_ltperipheralalloc.AnyAsync(x => x.SerialNumber == laptopPeripheralAllocation.SerialNumber && x.PeripheralAllocStatus == "AC");
            if (descriptionExists)
            {
                TempData["ErrorMessage"] = "Serial number already exists!";
                return RedirectToAction(nameof(Index));
            }

            var findOWNERDevice = await _context.tbl_ictams_ltperipheralalloc .Where(x => x.OwnerCode == laptopPeripheralAllocation.OwnerCode && x.PeripheralCode == laptopPeripheralAllocation.PeripheralCode && x.Status.status_code == "AC").FirstOrDefaultAsync();

            if (findOWNERDevice != null)
            {
                TempData["AlertMessage"] = "The owner already have this peripheral device!";
                return RedirectToAction("Create");
            }


            var findOWNERDevice2 = await _context.tbl_ictams_ltpsecowner.Where(x => x.SecOwnerCode == laptopPeripheralAllocation.OwnerCode && x.SecPeripheralCode == laptopPeripheralAllocation.PeripheralCode && x.Status.status_code == "AC").FirstOrDefaultAsync();

            if (findOWNERDevice2 != null)
            {
                TempData["AlertMessage"] = "The owner already have this peripheral device!";
                return RedirectToAction("Create");
            }

            //var findOwnerInBorrowed = await _context.tbl_ictams_ltpborrowed.Where(x => x.OwnerID == laptopPeripheralAllocation.OwnerCode && x.StatusID == "AC").FirstOrDefaultAsync();

            //if (findOwnerInBorrowed != null)
            //{
            //    TempData["AlertMessage"] = "The owner borrowed a laptop. Kindly return it to reallocate!";
            //    return RedirectToAction("Create");
            //}

            if (laptopPeripheralAllocation.PeripheralCode !=null)
            {
                var findLaptop = await _context.tbl_ictams_ltperipheral
               .Where(x => x.PeripheralCode == laptopPeripheralAllocation.PeripheralCode)
               .FirstOrDefaultAsync();

                var findQuantity = findLaptop.PeripheralQty - findLaptop.PeripheralAllocation;

                var FindQ = await _context.tbl_ictams_ltpborrowed
                   .Where(z => z.StatusID == "AC")
                   .CountAsync(x => x.UnitID == laptopPeripheralAllocation.PeripheralCode);


                if (FindQ == findQuantity)
                {
                    TempData["AlertMessage"] = "The available peripheral is already borrowed";
                    return RedirectToAction("Create");
                }
            }
            else
            {
                TempData["AlertMessage"] = "The peripheral code is cannot be null!";
                return RedirectToAction("Create");
            }

           


            var availableQuantity = await _context.tbl_ictams_ltperipheral
                .Where(x => x.PeripheralCode == laptopPeripheralAllocation.PeripheralCode && x.PeripheralStatus == "AV")
                .Select(x => x.PeripheralQty)
                .FirstOrDefaultAsync();



            var allocatedQuantity = await _context.tbl_ictams_ltperipheralalloc.Where(z => z.PeripheralAllocStatus == "AC")
                .CountAsync(x => x.PeripheralCode == laptopPeripheralAllocation.PeripheralCode);

            var ownerCodeExist = await _context.tbl_ictams_ltperipheralalloc.Where(x => x.OwnerCode == laptopPeripheralAllocation.OwnerCode)
                .FirstOrDefaultAsync();


            var ownerExists = await _context.tbl_ictams_ltperipheralalloc.AnyAsync(x => x.OwnerCode == laptopPeripheralAllocation.OwnerCode && x.PeripheralCode == laptopPeripheralAllocation.PeripheralCode);

            if (laptopPeripheralAllocation.DatePurchased > DateTime.Now)
            {
                TempData["AlertMessage"] = "Invalid date purchase!";
                return RedirectToAction("Create");
            }

            if (laptopPeripheralAllocation.PONumber == null)
            {
                TempData["AlertMessage"] = "PO number cannot be null or Empty!";
                return RedirectToAction("Create");
            }

            if (laptopPeripheralAllocation.OwnerCode == null)
            {
                TempData["AlertMessage"] = "Owner code cannot be null or Empty!";
                return RedirectToAction("Create");
            }
            if(laptopPeripheralAllocation.UnitPrice == 0)
            {
                TempData["AlertMessage"] = "Unit price value cannot be zero!";
                return RedirectToAction("Create");
            }
            if (laptopPeripheralAllocation.UnitPrice < 0)
            {
                TempData["AlertMessage"] = "Unit price value cannot be negative value!";
                return RedirectToAction("Create");
            }

                if (allocatedQuantity < availableQuantity)
            {
                var paramCode = await _context.tbl_ictams_parameters
                    .Where(p => p.parm_code == "ltpa_id")
                    .MaxAsync(p => p.parm_value);
                var newparamCode = paramCode + 1;

                var param = await _context.tbl_ictams_parameters
                    .FirstOrDefaultAsync(p => p.parm_code == "ltpa_id");
                param.parm_value = newparamCode;


                var allocQuantity = await _context.tbl_ictams_ltperipheral.Where(a => a.PeripheralCode == laptopPeripheralAllocation.PeripheralCode)
                    .MaxAsync(a => a.PeripheralAllocation);
                var newallocQuantity = allocQuantity + 1;
                var inv_alloc = await _context.tbl_ictams_ltperipheral.FirstOrDefaultAsync(a => a.PeripheralCode == laptopPeripheralAllocation.PeripheralCode);
                inv_alloc.PeripheralAllocation = newallocQuantity;


                TimeSpan ageDuration = (TimeSpan)(DateTime.Now - laptopPeripheralAllocation.DatePurchased);

                int totalMonths = (int)Math.Round(ageDuration.TotalDays / (365.25 / 12), 0);

                string ageDescription = $"{totalMonths} months";

                laptopPeripheralAllocation.AgeYears = decimal.Parse($"{totalMonths / 12}.{totalMonths % 12:00}");

                var ucode = HttpContext.Session.GetString("UserName");
                laptopPeripheralAllocation.PeripheralAllocationId = "LTPA" + newparamCode.ToString().PadLeft(11, '0');
                laptopPeripheralAllocation.FixedAssetTag = laptopPeripheralAllocation.FixedAssetTag.ToUpper();
                laptopPeripheralAllocation.SerialNumber =  laptopPeripheralAllocation.SerialNumber.ToUpper();
                laptopPeripheralAllocation.PeripheralAllocCreatedBy  = ucode;
                laptopPeripheralAllocation.PeripheralAllocCreatedAt = DateTime.Now;

                _context.Add(laptopPeripheralAllocation);
                await _context.SaveChangesAsync();

                // ...
                TempData["SuccessNotification"] = "Successfully added a new allocation!";
                // ...
                return RedirectToAction(nameof(Index));
            }
            else if (allocatedQuantity >= availableQuantity)
            {
                TempData["AlertMessage"] = "The laptop peripheral you selected is already fully allocated!";
                return RedirectToAction("Create");
            }
            if (ownerCodeExist != null)
            {
                TempData["AlertMessage"] = "The selected owner is already allocated!";
                return RedirectToAction("Create");
            }

            return RedirectToAction(nameof(Index));



        }

        // GET: LaptopPeripheralAllocations/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.tbl_ictams_ltperipheralalloc == null)
            {
                return NotFound();
            }

            var laptopPeripheralAllocation = await _context.tbl_ictams_ltperipheralalloc.FindAsync(id);
            if (laptopPeripheralAllocation == null)
            {
                return NotFound();
            }
            ViewData["PeripheralAllocCreatedBy"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode", laptopPeripheralAllocation.PeripheralAllocCreatedBy);
            ViewData["PeripheralCode"] = new SelectList(_context.tbl_ictams_ltperipheral, "PeripheralCode", "PeripheralCode", laptopPeripheralAllocation.PeripheralCode);
            ViewData["OwnerCode"] = new SelectList(_context.tbl_ictams_owners, "OwnerCode", "OwnerCode", laptopPeripheralAllocation.OwnerCode);
            ViewData["PeripheralAllocStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_code", laptopPeripheralAllocation.PeripheralAllocStatus);
            ViewData["PeripheralAllocUpdatedBy"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode", laptopPeripheralAllocation.PeripheralAllocUpdatedBy);
            return View(laptopPeripheralAllocation);
        }

        // POST: LaptopPeripheralAllocations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("PeripheralAllocationId,PeripheralCode,OwnerCode,SerialNumber,FixedAssetTag,DatePurchased,AgeYears,PONumber,PeripheralAllocStatus,PeripheralAllocCreatedBy,PeripheralAllocCreatedAt,PeripheralAllocUpdatedBy,PeripheralAllocUpdateAt,UnitPrice")] LaptopPeripheralAllocation laptopPeripheralAllocation)
        {
            if (id != laptopPeripheralAllocation.PeripheralAllocationId)
            {
                return NotFound();
            }
                try
                {
                var ucode = HttpContext.Session.GetString("UserName");
                laptopPeripheralAllocation.PeripheralAllocUpdatedBy = ucode;
                laptopPeripheralAllocation.PeripheralAllocUpdateAt = DateTime.Now;
                _context.Update(laptopPeripheralAllocation);
                 await _context.SaveChangesAsync();
                // ...
                TempData["SuccessNotification"] = "Successfully edited!";
                // ...
            }
            catch (DbUpdateConcurrencyException)
                {
                    if (!LaptopPeripheralAllocationExists(laptopPeripheralAllocation.PeripheralAllocationId))
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

        // GET: LaptopPeripheralAllocations/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.tbl_ictams_ltperipheralalloc == null)
            {
                return NotFound();
            }

            var laptopPeripheralAllocation = await _context.tbl_ictams_ltperipheralalloc
                .Include(l => l.CreatedBy)
                .Include(l => l.LaptopPeripheral)
                .Include(l => l.Owner)
                .Include(l => l.Status)
                .Include(l => l.UpdatedBy)
                .FirstOrDefaultAsync(m => m.PeripheralAllocationId == id);
            if (laptopPeripheralAllocation == null)
            {
                return NotFound();
            }

            return View(laptopPeripheralAllocation);
        }

        // POST: LaptopPeripheralAllocations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.tbl_ictams_ltperipheralalloc == null)
            {
                return Problem("Entity set 'AssetManagementContext.LaptopPeripheralAllocation'  is null.");
            }
            var laptopPeripheralAllocation = await _context.tbl_ictams_ltperipheralalloc.FindAsync(id);
            if (laptopPeripheralAllocation != null)
            {
                _context.tbl_ictams_ltperipheralalloc.Remove(laptopPeripheralAllocation);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LaptopPeripheralAllocationExists(string id)
        {
          return (_context.tbl_ictams_ltperipheralalloc?.Any(e => e.PeripheralAllocationId == id)).GetValueOrDefault();
        }
        private bool LaptopPeripheralAllocationExistsOwner(string id)
        {
            return (_context.tbl_ictams_ltperipheralalloc?.Any(e => e.OwnerCode == id && e.PeripheralAllocStatus == "AC")).GetValueOrDefault();
        }
    }
}
