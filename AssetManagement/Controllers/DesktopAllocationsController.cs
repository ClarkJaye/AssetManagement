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
    public class DesktopAllocationsController : BaseController
    {
        private readonly AssetManagementContext _context;

        public DesktopAllocationsController(AssetManagementContext context)
            : base(context)
        {
            _context = context;
        }

        // GET: DesktopAllocations
        public async Task<IActionResult> Index()
        {
            await FindDesktopAllocationCompleted();
            await FindStatus();

            int? userProfile = HttpContext.Session.GetInt32("UserProfile");
            if (userProfile.HasValue)
            {

                var hasOpenAccess = await _context.tbl_ictams_profileaccess
          .AnyAsync(pa => pa.OpenAccess == "Y" &&
                          pa.Module.ModuleTitle == "Desktop Allocations" && pa.ProfileId == userProfile.Value);
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

                    // Count total laptops
                    var totalDesk = await _context.DesktopInventory.SumAsync(x => x.Quantity);
                    var totalAllocLaps = await _context.tbl_ictams_desktopalloc.CountAsync(x => x.AllocationStatus == "AC");
                    var totalNotAllocLaps = await _context.DesktopInventory.SumAsync(x => x.Quantity - x.AllocatedNo);

                  

                    ViewBag.TotalDekstop = totalDesk;
                    ViewBag.TotalAllocatedDesktop = totalAllocLaps;
                    ViewBag.TotalNotAllocatedDesktop= totalNotAllocLaps;

                    var assetManagementContext = _context.tbl_ictams_desktopalloc.Where(x=>x.AllocationStatus != "IN")
                        .Include(d => d.Createdby)
                        .Include(d => d.DesktopInventory)
                        .Include(d => d.DesktopInventoryDetail)
                        .Include(d => d.Owner)
                        .Include(d => d.Status)
                        .Include(d => d.Updatedby);
                    return View(await assetManagementContext.ToListAsync());
                }
            }

            return RedirectToAction("Logout", "Users");

        }

        public async Task<IActionResult> DisposeDesktop()
        {
            var assetManagementContext = _context.tbl_ictams_dtareturn.Where(x => x.ReturnType.Description == "DISPOSE").Include(d => d.DesktopAllocation).Include(d => d.ReturnType).Include(d => d.Status).Include(d => d.UserCreated).Include(d => d.UserUpdated);
            return View(await assetManagementContext.ToListAsync());
        }



        public async Task<IActionResult> ActiveDesktopReport()
        {
            var totalActiveDesktop = await _context.tbl_ictams_desktopalloc.CountAsync(x => x.AllocationStatus == "AC");
            var totalActiveDesktop2 = await _context.tbl_ictams_dtnewalloc.CountAsync(x => x.SecAllocationStatus == "AC");

            ViewBag.TotalActiveDesktop = totalActiveDesktop + totalActiveDesktop2;

            var assetManagementContext = _context.tbl_ictams_desktopalloc.Where(x => x.AllocationStatus != "IN"
            && x.AllocationStatus != "CO")
                        .Include(d => d.Createdby)
                        .Include(d => d.DesktopInventory)
                        .Include(d => d.DesktopInventoryDetail)
                        .Include(d => d.Owner)
                        .Include(d => d.Status)
                        .Include(d => d.Updatedby);
            return View(await assetManagementContext.ToListAsync());
        }

        public async Task<IActionResult> ActiveDesktopReportSecond()
        {
            var totalActiveDesktop = await _context.tbl_ictams_desktopalloc.CountAsync(x => x.AllocationStatus == "AC");
            var totalActiveDesktop2 = await _context.tbl_ictams_dtnewalloc.CountAsync(x => x.SecAllocationStatus == "AC");

            ViewBag.TotalActiveDesktop = totalActiveDesktop + totalActiveDesktop2;

            var assetManagementContext = _context.tbl_ictams_dtnewalloc.Where(x => x.SecAllocationStatus != "IN"
            && x.SecAllocationStatus != "CO")
                        .Include(d => d.Createdby)
                        .Include(d => d.DesktopInventory)
                        .Include(d => d.DesktopAllocation)
                        .Include(d => d.DesktopInventory.MainBoard)
                        .Include(d => d.DesktopInventory.GraphicsCard)
                        .Include(d => d.DesktopInventory.Level)
                        .Include(d => d.DesktopInventory.Brand)
                        .Include(d => d.InventoryDetails)
                        .Include(d => d.Owner)
                        .Include(d => d.Status)
                        .Include(d => d.Updatedby);
            return View(await assetManagementContext.ToListAsync());
        }

        public async Task<IActionResult> SixyearsDesktop(string id)
        {
            await FindDesktopAllocationCompleted();
            await FindStatus();
            ViewBag.Id = id;

            if (id == null)
            {

                return View();
            }
            else
            {
                var count = await _context.tbl_ictams_desktopalloc
                .Where(x => x.AllocationStatus == "CO" &&
                            _context.tbl_ictams_owners.Any(a => a.OwnerCode == x.Owner.OwnerCode && a.Department.Dept_name == id) &&
                            _context.tbl_ictams_desktopinvdetails.Any(a => a.unitTag == x.UnitTag && a.PurchaseDate < DateTime.Now.AddMonths(-72)))
                .CountAsync(); 

                ViewBag.TotalAvailableLaptop = "Total number of" + " " + id + ": " + count;

                var assetManagementContext = _context.tbl_ictams_desktopalloc
                    .Where(x => x.AllocationStatus == "CO" &&
                        _context.tbl_ictams_owners.Any(a => a.OwnerCode == x.Owner.OwnerCode && a.Department.Dept_name == id) &&
                        _context.tbl_ictams_desktopinvdetails.Any(a => a.unitTag == x.UnitTag && a.PurchaseDate < DateTime.Now.AddMonths(-72)))
                    .Include(d => d.Createdby)
                        .Include(d => d.DesktopInventory)
                        .Include(d => d.DesktopInventory.MainBoard)
                        .Include(d => d.DesktopInventory.GraphicsCard)
                        .Include(d => d.DesktopInventory.Level)
                        .Include(d => d.DesktopInventory.Brand)
                        .Include(d => d.DesktopInventoryDetail)
                        .Include(d => d.Owner)
                        .Include(d => d.Status)
                        .Include(d => d.Updatedby);

                var laptopAllocations = await assetManagementContext.ToListAsync();

                // Check if there are any laptop allocations that meet the criteria
                if (laptopAllocations.Any())
                {
                    return View(laptopAllocations);
                }
                else
                {
                    // Set id to null and return a view with id as null
                    id = null;
                    ViewBag.Id = id; // Update ViewBag.Id to reflect the change
                    return View(); // Return the view with id as null
                }
            }
        }

        public async Task<IActionResult> SixyearsDesktopSecond(string id)
        {
            await FindDesktopAllocationCompleted();
            await FindStatus();
            ViewBag.Id = id;

            if (id == null)
            {

                return View();
            }
            else
            {
                var count = await _context.tbl_ictams_dtnewalloc
                .Where(x => x.SecAllocationStatus == "CO" &&
                            _context.tbl_ictams_owners.Any(a => a.OwnerCode == x.Owner.OwnerCode && a.Department.Dept_name == id) &&
                            _context.tbl_ictams_desktopinvdetails.Any(a => a.unitTag == x.UnitTag && a.PurchaseDate < DateTime.Now.AddMonths(-72)))
                .CountAsync();

                ViewBag.TotalAvailableLaptop = "Total number of" + " " + id + ": " + count;

                var assetManagementContext = _context.tbl_ictams_dtnewalloc
                    .Where(x => x.SecAllocationStatus == "CO" &&
                        _context.tbl_ictams_owners.Any(a => a.OwnerCode == x.Owner.OwnerCode && a.Department.Dept_name == id) &&
                        _context.tbl_ictams_desktopinvdetails.Any(a => a.unitTag == x.UnitTag && a.PurchaseDate < DateTime.Now.AddMonths(-72)))
                        .Include(d => d.Createdby)
                        .Include(d => d.DesktopInventory)
                        .Include(d => d.DesktopAllocation)
                        .Include(d => d.DesktopInventory.MainBoard)
                        .Include(d => d.DesktopInventory.GraphicsCard)
                        .Include(d => d.DesktopInventory.Level)
                        .Include(d => d.DesktopInventory.Brand)
                        .Include(d => d.InventoryDetails)
                        .Include(d => d.Owner)
                        .Include(d => d.Status)
                        .Include(d => d.Updatedby);

                var laptopAllocations = await assetManagementContext.ToListAsync();

                // Check if there are any laptop allocations that meet the criteria
                if (laptopAllocations.Any())
                {
                    return View(laptopAllocations);
                }
                else
                {
                    // Set id to null and return a view with id as null
                    id = null;
                    ViewBag.Id = id; // Update ViewBag.Id to reflect the change
                    return View(); // Return the view with id as null
                }
            }
        }


        public async Task<IActionResult> ReturnPartialView()
        {
            await FindStatus();


            var totalActiveLaptops = await _context.tbl_ictams_desktopalloc.CountAsync(x => x.AllocationStatus == "AC");
            var totalInactiveLaptops = await _context.tbl_ictams_desktopalloc.CountAsync(x => x.AllocationStatus == "IN");

            ViewBag.TotalActiveLaptops = totalActiveLaptops;
            ViewBag.TotalInactiveLaptops = totalInactiveLaptops;

            var assetManagementContext = _context.tbl_ictams_desktopalloc.Where(x => x.AllocationStatus == "AC")
                .Include(l => l.Status)
                .Include(l => l.Createdby)
                .Include(l => l.DesktopInventory)
                .Include(l => l.Owner)
                .Include(l => l.DesktopInventoryDetail)
                .Include(l => l.Updatedby);

            return View(await assetManagementContext.ToListAsync());


        }
        public async Task<IActionResult> DeskAllocPartialView()
        {
            await FindStatus();


            var totalActiveLaptops = await _context.tbl_ictams_desktopalloc.CountAsync(x => x.AllocationStatus == "AC");
            var totalInactiveLaptops = await _context.tbl_ictams_desktopalloc.CountAsync(x => x.AllocationStatus == "IN");

            ViewBag.TotalActiveLaptops = totalActiveLaptops;
            ViewBag.TotalInactiveLaptops = totalInactiveLaptops;
                        var assetManagementContext = _context.tbl_ictams_desktopalloc.Where(x => x.AllocationStatus != "IN")
                .Include(l => l.Status)
                .Include(l => l.Createdby)
                .Include(l => l.DesktopInventory)
                .Include(l => l.Owner)
                .Include(l => l.DesktopInventoryDetail)
                .Include(l => l.Updatedby);

            return View(await assetManagementContext.ToListAsync());


        }


        public async Task<IActionResult> Inactive()
        {
            await FindStatus();

            var assetManagementContext = _context.tbl_ictams_desktopalloc.Where(x=>x.AllocationStatus == "IN").Include(d => d.Createdby).Include(d => d.DesktopInventory).Include(d => d.DesktopInventoryDetail).Include(d => d.Owner).Include(d => d.Status).Include(d => d.Updatedby);
            return View(await assetManagementContext.ToListAsync());

        }

        // GET: DesktopAllocations/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.tbl_ictams_desktopalloc == null)
            {
                return NotFound();
            }

            var desktopAllocation = await _context.tbl_ictams_desktopalloc
                .Include(d => d.Createdby)
                .Include(d => d.DesktopInventory)
                .Include(d => d.DesktopInventoryDetail)
                .Include(d => d.Owner)
                .Include(d => d.Status)
                .Include(d => d.Updatedby)
                .FirstOrDefaultAsync(m => m.AllocId == id);
            if (desktopAllocation == null)
            {
                return NotFound();
            }

            return View(desktopAllocation);
        }

        // GET: DesktopAllocations/Create
        public IActionResult Create()
        {
            ViewData["AllocationStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_code");
            ViewData["AllocCreated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode");
            ViewData["DesktopCode"] = new SelectList(_context.tbl_ictams_desktopinv, "desktopInvCode", "desktopInvCode");
            ViewData["OwnerCode"] = new SelectList(_context.tbl_ictams_owners, "OwnerCode", "OwnerCode");
            ViewData["AllocUpdated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode");
            return View();
        }

        // POST: DesktopAllocations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AllocId,DesktopCode,OwnerCode,UnitTag,FixedassetTag,ComputerName,DateDeployed,AllocationStatus,AllocCreated,DateCreated,AllocUpdated,DateUpdated")] DesktopAllocation desktopAllocation)
        {

            bool descriptionExists = await _context.tbl_ictams_desktopalloc.AnyAsync(x => x.UnitTag == desktopAllocation.UnitTag
            && x.AllocationStatus == "AC");
            if (descriptionExists)
            {
                TempData["ErrorMessage"] = "This serial number already existss!";
                return RedirectToAction(nameof(Index));
            }

            var availableQuantity = await _context.tbl_ictams_desktopinv
               .Where(x => x.desktopInvCode == desktopAllocation.DesktopCode && x.DTStatus == "AV")
               .Select(x => x.Quantity)
               .FirstOrDefaultAsync();

            var allocatedQuantity = await _context.tbl_ictams_desktopalloc.Where(z => z.AllocationStatus == "AC")
                .CountAsync(x => x.DesktopCode == desktopAllocation.DesktopCode);

            var ownerCodeExist = await _context.tbl_ictams_desktopalloc.Where(x => x.OwnerCode == desktopAllocation.OwnerCode)
                .FirstOrDefaultAsync();


            var ownerExists = await _context.tbl_ictams_desktopalloc.AnyAsync(x => x.OwnerCode == desktopAllocation.OwnerCode && x.DateCreated >= DateTime.Now.AddYears(4));

            var findOwnerInBorrowed = await _context.tbl_ictams_dtborrowed.Where(x => x.OwnerID == desktopAllocation.OwnerCode && x.StatusID == "AC").FirstOrDefaultAsync();
            if (findOwnerInBorrowed != null)
            {
                TempData["AlertMessage"] = "The owner borrowed a desktop. Kindly return it to reallocate!";
                return RedirectToAction("Create");
            }

            if (desktopAllocation.DesktopCode != null)
            {
                var findLaptop = await _context.tbl_ictams_desktopinv
                .Where(x => x.desktopInvCode == desktopAllocation.DesktopCode)
                .FirstOrDefaultAsync();

                var findQuantity = findLaptop.Quantity - findLaptop.AllocatedNo;

                var FindQ = await _context.tbl_ictams_dtborrowed
                   .Where(z => z.StatusID == "AC")
                   .CountAsync(x => x.UnitID == desktopAllocation.DesktopCode);

                if (FindQ == findQuantity)
                {
                    TempData["AlertMessage"] = "The available desktop has been borrowed!";
                    return RedirectToAction("Create");
                }
            }
            else
            {
                if (desktopAllocation.DesktopCode == null)
                {
                    TempData["AlertMessage"] = "The Desktop cannot be null!";
                    return RedirectToAction("Create");
                }
            }

            if (DesktopAllocationExistsComputerName(desktopAllocation.ComputerName))
            {
                TempData["AlertMessage"] = "Computer name cannot be duplicated!";
                return RedirectToAction("Create");
            }

            if (DesktopAllocationExistsOwner(desktopAllocation.OwnerCode))
            {
                TempData["AlertMessage"] = "The Selected Owner is already allocated with a device or Inventory Desktop! Select another Owner to Allocate with! Need to wait after 4 years!";
                return RedirectToAction("Create");
            }

            if (desktopAllocation.OwnerCode == null)
            {
                TempData["AlertMessage"] = "Owner code cannot be null or Empty!";
                return RedirectToAction("Create");
            }

            if (allocatedQuantity < availableQuantity)
            {
                var paramCode = await _context.tbl_ictams_parameters
                    .Where(p => p.parm_code == "dta_id")
                    .MaxAsync(p => p.parm_value);
                var newparamCode = paramCode + 1;

                var param = await _context.tbl_ictams_parameters
                    .FirstOrDefaultAsync(p => p.parm_code == "dta_id");
                param.parm_value = newparamCode;


                var allocQuantity = await _context.tbl_ictams_desktopinv.Where(a => a.desktopInvCode == desktopAllocation.DesktopCode)
                    .MaxAsync(a => a.AllocatedNo);
                var newallocQuantity = allocQuantity + 1;
                var inv_alloc = await _context.tbl_ictams_desktopinv.FirstOrDefaultAsync(a => a.desktopInvCode == desktopAllocation.DesktopCode);
                inv_alloc.AllocatedNo = newallocQuantity;
                var inv_details = await _context.tbl_ictams_desktopinvdetails
                    .FirstOrDefaultAsync(a => a.unitTag == desktopAllocation.UnitTag);
                inv_details.DTStatus = "AC";
                inv_details.DeployedDate = DateTime.Now;


                var ucode = HttpContext.Session.GetString("UserName");
                desktopAllocation.AllocId = "DTA" + newparamCode.ToString().PadLeft(12, '0');
                desktopAllocation.FixedassetTag = desktopAllocation.FixedassetTag.ToUpper();
                desktopAllocation.ComputerName = desktopAllocation.ComputerName.ToUpper();
                desktopAllocation.UnitTag = desktopAllocation.UnitTag.ToUpper();
                desktopAllocation.AllocCreated = ucode;
                desktopAllocation.DateCreated = DateTime.Now;
                desktopAllocation.DateDeployed = DateTime.Now;
                desktopAllocation.AllocationStatus = "AC";

                _context.Add(desktopAllocation);
                await _context.SaveChangesAsync();

                // ...
                TempData["SuccessNotification"] = "Successfully added a new allocation!";
                // ...
                return RedirectToAction(nameof(Index));
            }
            else if (allocatedQuantity >= availableQuantity)
            {
                TempData["AlertMessage"] = "The Desktop inventory you selected has been fully allocated!";
                return RedirectToAction("Create");
            }
            if (ownerCodeExist != null)
            {
                TempData["AlertMessage"] = "The Selected Owner already allocated a Desktop!";
                return RedirectToAction("Create");
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: DesktopAllocations/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            ViewData["AllocationStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_code");
            ViewData["AllocCreated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode");
            ViewData["DesktopCode"] = new SelectList(_context.tbl_ictams_desktopinv, "desktopInvCode", "desktopInvCode");
            ViewData["OwnerCode"] = new SelectList(_context.tbl_ictams_owners, "OwnerCode", "OwnerCode");
            ViewData["AllocUpdated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode");
            if (id == null || _context.tbl_ictams_desktopalloc == null)
            {
                return NotFound();

            }

            var desktopAllocation = await _context.tbl_ictams_desktopalloc.FindAsync(id);
            if (desktopAllocation == null)
            {
                return NotFound();
            }
            return View(desktopAllocation);
        }

        // POST: DesktopAllocations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("AllocId,DesktopCode,OwnerCode,UnitTag,FixedassetTag,ComputerName,DateDeployed,AllocationStatus,AllocCreated,DateCreated,AllocUpdated,DateUpdated")] DesktopAllocation desktopAllocation)
        {
            if (id != desktopAllocation.AllocId)
            {
                return NotFound();
            }

            try
            {
                var ucode = HttpContext.Session.GetString("UserName");
                desktopAllocation.AllocUpdated = ucode;
                desktopAllocation.DateUpdated = DateTime.Now;
                _context.Update(desktopAllocation);
                await _context.SaveChangesAsync();
                // ...
                TempData["SuccessNotification"] = "Successfully edited!";
                // ...
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DesktopAllocationExists(desktopAllocation.AllocId))
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



        // GET: DesktopAllocations/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.tbl_ictams_desktopalloc == null)
            {
                return NotFound();
            }

            var desktopAllocation = await _context.tbl_ictams_desktopalloc
                .Include(d => d.Createdby)
                .Include(d => d.DesktopInventory)
                .Include(d => d.DesktopInventoryDetail)
                .Include(d => d.Owner)
                .Include(d => d.Status)
                .Include(d => d.Updatedby)
                .FirstOrDefaultAsync(m => m.AllocId == id);
            if (desktopAllocation == null)
            {
                return NotFound();
            }

            return View(desktopAllocation);
        }
        //DELETE
        public async Task<IActionResult> DeleteAsEdit(string[] selectedIds)
        {
            ViewData["UserStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_name");
            foreach (string id in selectedIds)
            {
                var allocationID = await _context.tbl_ictams_desktopalloc.FindAsync(id);
                if (allocationID != null)
                {
                    // Check if the selected profile status exists in the tbl_lsm_status table
                    var status = await _context.tbl_ictams_status.FindAsync(allocationID.AllocationStatus);
                    if (status == null)
                    {
                        ModelState.AddModelError("", $"Profile status '{allocationID.AllocId}' does not exist.");
                    }

                    // Find the corresponding LaptopCode in tbl_ictams_laptopinv and deduct 1 from AllocatedNo
                    var desktopCode = allocationID.DesktopCode;
                    var desktopInv = await _context.tbl_ictams_desktopinv.FirstOrDefaultAsync(a => a.desktopInvCode == desktopCode);
                    if (desktopInv != null)
                    {
                        desktopInv.AllocatedNo -= 1;
                    }
                    var ucode = HttpContext.Session.GetString("UserName");

                    var InvDetails = await _context.tbl_ictams_desktopinvdetails.FirstOrDefaultAsync(a => a.unitTag == allocationID.UnitTag);
                    InvDetails.UpdatedDate = DateTime.Now;
                    InvDetails.Updated = ucode;
                    InvDetails.DTStatus = "AV";

                    // Update the profile
                    allocationID.AllocUpdated = ucode;
                    allocationID.AllocationStatus = "IN";
                    allocationID.DateUpdated = DateTime.Now;

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

        // POST: DesktopAllocations/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(string id)
        //{
        //    if (_context.tbl_ictams_desktopalloc == null)
        //    {
        //        return Problem("Entity set 'AssetManagementContext.DesktopAllocation'  is null.");
        //    }
        //    var desktopAllocation = await _context.tbl_ictams_desktopalloc.FindAsync(id);
        //    if (desktopAllocation != null)
        //    {
        //        _context.tbl_ictams_desktopalloc.Remove(desktopAllocation);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool DesktopAllocationExistsOwner(string id)
        //{
        //    return (_context.tbl_ictams_desktopalloc?.Any(e => e.OwnerCode == id && e.AllocationStatus == "AC")).GetValueOrDefault();
        //}

        //private bool DesktopAllocationExistsComputerName(string id)
        //{
        //    return (_context.tbl_ictams_desktopalloc?.Any(e => e.ComputerName == id && e.AllocationStatus == "AC")).GetValueOrDefault();
        //}

        //private bool DesktopAllocationExists(string id)
        //{
        //  return (_context.tbl_ictams_desktopalloc?.Any(e => e.AllocId == id)).GetValueOrDefault();
        //}
        private bool DesktopAllocationExists(string id)
        {
            return (_context.tbl_ictams_desktopalloc?.Any(e => e.AllocId == id)).GetValueOrDefault();
        }

        private bool DesktopAllocationExistsOwner(string id)
        {
            return (_context.tbl_ictams_desktopalloc?.Any(e => e.OwnerCode == id && e.AllocationStatus == "AC")).GetValueOrDefault();
        }

        private bool DesktopAllocationExistsComputerName(string id)
        {
            return (_context.tbl_ictams_desktopalloc?.Any(e => e.ComputerName == id && e.AllocationStatus == "AC")).GetValueOrDefault();
        }
    }
}
