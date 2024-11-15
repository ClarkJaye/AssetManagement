using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Data;
using AssetManagement.Models;
using AssetManagement.Utility;
using OfficeOpenXml;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AssetManagement.Controllers
{
    public class MonitorAllocationsController : BaseController
    {
        private readonly AssetManagementContext _context;

        public MonitorAllocationsController(AssetManagementContext context)
            :base (context)
        {
            _context = context;
        }
        public async Task<IActionResult> Inactive()
        {
            await FindStatus();

            var assetManagementContext = _context.tbl_ictams_monitoralloc.Include(l => l.Status).Include(l => l.Createdby).Include(l => l.MonitorDetails.MonitorInventory).Where(x => x.AllocationStatus == "IN").Include(l => l.Owner).Include(l => l.Updatedby).Include(l => l.MonitorDetails);
            return View(await assetManagementContext.ToListAsync());
        }
        public async Task<IActionResult> AllocationViews()
        {
            await FindStatus();

            var assetManagementContext = _context.tbl_ictams_monitoralloc.Include(l => l.Status).Include(l => l.Createdby).Include(l => l.MonitorDetails.MonitorInventory).Where(x => x.AllocationStatus == "IN").Include(l => l.Owner).Include(l => l.Updatedby);
            return View(await assetManagementContext.ToListAsync());
        }

        public async Task<IActionResult> ActiveMonitorReport()
        {
            var totalActiveDesktop = await _context.tbl_ictams_monitoralloc.CountAsync(x => x.AllocationStatus == "AC");
            var totalActiveDesktop2 = await _context.tbl_ictams_monitornewalloc.CountAsync(x => x.SecAllocationStatus == "AC");

            ViewBag.TotalActiveDesktop = totalActiveDesktop + totalActiveDesktop2;

            var assetManagementContext = _context.tbl_ictams_monitoralloc.Where(x => x.AllocationStatus != "IN"
            && x.AllocationStatus != "CO")
                        .Include(d => d.Createdby)
                        .Include(d => d.MonitorDetails.MonitorInventory)
                        .Include(d => d.MonitorDetails.MonitorInventory.Model)
                        .Include(d => d.MonitorDetails.Vendor)
                        .Include(d => d.Owner)
                        .Include(d => d.Status)
                        .Include(d => d.Updatedby);
            return View(await assetManagementContext.ToListAsync());
        }

        public async Task<IActionResult> DisposeMonitor()
        {
            var assetManagementContext = _context.tbl_ictams_monitorreturn.Where(x => x.ReturnType.Description == "DISPOSE").Include(d => d.MonitorAllocation).Include(d => d.MonitorAllocation.MonitorDetails.MonitorInventory.Model).Include(d => d.ReturnType).Include(d => d.Status).Include(d => d.UserCreated).Include(d => d.UserUpdated);
            return View(await assetManagementContext.ToListAsync());
        }

        public async Task<IActionResult> ActiveMonitorReportSecond()
        {
            var totalActiveDesktop = await _context.tbl_ictams_monitoralloc.CountAsync(x => x.AllocationStatus == "AC");
            var totalActiveDesktop2 = await _context.tbl_ictams_monitornewalloc.CountAsync(x => x.SecAllocationStatus == "AC");

            ViewBag.TotalActiveDesktop = totalActiveDesktop + totalActiveDesktop2;

            var assetManagementContext = _context.tbl_ictams_monitornewalloc.Where(x => x.SecAllocationStatus != "IN"
            && x.SecAllocationStatus != "CO")
                        .Include(d => d.Createdby)
                        .Include(d => d.MonitorAllocation)
                        .Include(d => d.MonitorAllocation.MonitorDetails.MonitorInventory)
                        .Include(d => d.MonitorAllocation.MonitorDetails.MonitorInventory.Model)
                        .Include(d => d.MonitorDetail.Vendor)
                        .Include(d => d.Owner)
                        .Include(d => d.Status)
                        .Include(d => d.Updatedby);
            return View(await assetManagementContext.ToListAsync());
        }

        public async Task<IActionResult> SixyearsMonitorFirst(string id)
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
                var count = await _context.tbl_ictams_monitoralloc
                .Where(x => x.AllocationStatus == "CO" &&
                            _context.tbl_ictams_owners.Any(a => a.OwnerCode == x.Owner.OwnerCode && a.Department.Dept_name == id) &&
                            _context.tbl_ictams_monitordetails.Any(a => a.SerialNumber == x.SerialNumber && a.PurchaseDate < DateTime.Now.AddMonths(-72)))
                .CountAsync();

                ViewBag.TotalAvailableLaptop = "Total number of" + " " + id + ": " + count;

                var assetManagementContext = _context.tbl_ictams_monitoralloc
                    .Where(x => x.AllocationStatus == "CO" &&
                        _context.tbl_ictams_owners.Any(a => a.OwnerCode == x.Owner.OwnerCode && a.Department.Dept_name == id) &&
                        _context.tbl_ictams_monitordetails.Any(a => a.SerialNumber == x.SerialNumber && a.PurchaseDate < DateTime.Now.AddMonths(-72)))
                    .Include(d => d.Createdby)
                        .Include(d => d.MonitorDetails.MonitorInventory)
                        .Include(d => d.MonitorDetails.MonitorInventory.Model)
                        .Include(d => d.MonitorDetails.Vendor)
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

        public async Task<IActionResult> SixyearsMonitorSecond(string id)
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
                var count = await _context.tbl_ictams_monitornewalloc
                .Where(x => x.SecAllocationStatus == "CO" &&
                            _context.tbl_ictams_owners.Any(a => a.OwnerCode == x.Owner.OwnerCode && a.Department.Dept_name == id) &&
                            _context.tbl_ictams_monitordetails.Any(a => a.SerialNumber == x.SerialNumber && a.PurchaseDate < DateTime.Now.AddMonths(-72)))
                .CountAsync();

                ViewBag.TotalAvailableLaptop = "Total number of" + " " + id + ": " + count;

                var assetManagementContext = _context.tbl_ictams_monitornewalloc
                    .Where(x => x.SecAllocationStatus == "CO" &&
                        _context.tbl_ictams_owners.Any(a => a.OwnerCode == x.Owner.OwnerCode && a.Department.Dept_name == id) &&
                        _context.tbl_ictams_monitordetails.Any(a => a.SerialNumber == x.SerialNumber && a.PurchaseDate < DateTime.Now.AddMonths(-72)))
                    .Include(d => d.Createdby)
                        .Include(d => d.MonitorAllocation)
                        .Include(d => d.MonitorAllocation.MonitorDetails.MonitorInventory)
                        .Include(d => d.MonitorAllocation.MonitorDetails.MonitorInventory.Model)
                        .Include(d => d.MonitorAllocation.MonitorDetails.Vendor)
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

        // GET: MonitorAllocations
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

            await FindStatus();

            int? userProfile = HttpContext.Session.GetInt32("UserProfile");
            if (userProfile.HasValue)
            {

                var hasOpenAccess = await _context.tbl_ictams_profileaccess
          .AnyAsync(pa => pa.OpenAccess == "Y" &&
                          pa.Module.ModuleTitle == "Monitor Allocations" &&  // Adjust the module name as needed
                          pa.ProfileId == userProfile.Value);
                if (!hasOpenAccess)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Count total laptops
                    var totalAlloc = await _context.tbl_ictams_monitordetails.CountAsync(x => x.MonitorStatus == "AC");
                    var totalNotAlloc = await _context.tbl_ictams_monitorinv.SumAsync(x => x.Quantity - x.AllocatedNo);

                    ViewBag.TotalActive = totalAlloc;
                    ViewBag.TotalInactive = totalNotAlloc;

                    var assetManagementContext = _context.tbl_ictams_monitoralloc.Where(x => x.AllocationStatus != "IN")
                        .Include(l => l.Status)
                        .Include(l => l.Createdby)
                        .Include(l => l.MonitorDetails.MonitorInventory)
                        .Include(l => l.Owner)
                        .Include(l => l.MonitorDetails)
                        .Include(l => l.Updatedby);

                    return View(await assetManagementContext.ToListAsync());
                }
            }

            return RedirectToAction("Logout", "Users");
        }
        public async Task<IActionResult> ReturnPartialView()
        {
            await FindStatus();


            var totalActiveLaptops = await _context.tbl_ictams_monitoralloc.CountAsync(x => x.AllocationStatus == "AC");
            var totalInactiveLaptops = await _context.tbl_ictams_monitoralloc.CountAsync(x => x.AllocationStatus == "IN");

            ViewBag.TotalActiveLaptops = totalActiveLaptops;
            ViewBag.TotalInactiveLaptops = totalInactiveLaptops;

            var assetManagementContext = _context.tbl_ictams_monitoralloc.Where(x => x.AllocationStatus == "AC")
                .Include(l => l.Status)
                .Include(l => l.Createdby)
                .Include(l => l.MonitorDetails.MonitorInventory)
                .Include(l => l.Owner)
                .Include(l => l.MonitorDetails)
                .Include(l => l.Updatedby);

            return View(await assetManagementContext.ToListAsync());


        }

        public async Task<IActionResult> MonitorAllocPartialView()
        {
            await FindStatus();


            var totalActiveLaptops = await _context.tbl_ictams_monitoralloc.CountAsync(x => x.AllocationStatus == "AC");
            var totalInactiveLaptops = await _context.tbl_ictams_monitoralloc.CountAsync(x => x.AllocationStatus == "IN");

            ViewBag.TotalActiveLaptops = totalActiveLaptops;
            ViewBag.TotalInactiveLaptops = totalInactiveLaptops;

            var assetManagementContext = _context.tbl_ictams_monitoralloc.Where(x => x.AllocationStatus == "AC")
                .Include(l => l.Status)
                .Include(l => l.Createdby)
                .Include(l => l.MonitorDetails.MonitorInventory)
                .Include(l => l.Owner)
                .Include(l => l.MonitorDetails)
                .Include(l => l.Updatedby);

            return View(await assetManagementContext.ToListAsync());


        }
        // GET: MonitorAllocations/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.tbl_ictams_monitoralloc == null)
            {
                return NotFound();
            }

            var monitorAllocation = await _context.tbl_ictams_monitoralloc
                .Include(m => m.Createdby)
                .Include(m => m.MonitorDetails)
                .Include(m => m.MonitorDetails.MonitorInventory)
                .Include(m => m.Owner)
                .Include(m => m.Status)
                .Include(m => m.Updatedby)
                .FirstOrDefaultAsync(m => m.AllocId == id);
            if (monitorAllocation == null)
            {
                return NotFound();
            }

            return View(monitorAllocation);
        }

        // GET: MonitorAllocations/Create
        public IActionResult Create()
        {
            ViewData["AllocCreated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserFullName");
            ViewData["monitorCode"] = new SelectList(_context.tbl_ictams_monitorinv, "monitorCode", "monitorCode");
            ViewData["SerialNumber"] = new SelectList(_context.tbl_ictams_monitordetails, "SerialNumber", "SerialNumber");
            ViewData["OwnerCode"] = new SelectList(_context.tbl_ictams_owners, "OwnerCode", "OwnerFullName");
            ViewData["AllocationStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_name");
            ViewData["AllocUpdated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode");
            return View();
        }

        // POST: MonitorAllocations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AllocId,monitorCode,OwnerCode,SerialNumber,FixedassetTag,DateDeployed,AllocationStatus,AllocCreated,DateCreated,AllocUpdated,DateUpdated")] MonitorAllocation monitorAllocation)
        {
            bool descriptionExists = await _context.tbl_ictams_monitoralloc.AnyAsync(x => x.SerialNumber == monitorAllocation.SerialNumber
           && x.AllocationStatus == "AC");
            if (descriptionExists)
            {
                TempData["ErrorMessage"] = "This serial number already existss!";
                return RedirectToAction(nameof(Index));
            }

            var availableQuantity = await _context.tbl_ictams_monitorinv
                .Where(x => x.monitorCode == monitorAllocation.monitorCode && x.MonitorStatus == "AV")
                .Select(x => x.Quantity)
                .FirstOrDefaultAsync();

            var allocatedQuantity = await _context.tbl_ictams_monitoralloc.Where(z => z.AllocationStatus == "AC")
                .CountAsync(x => x.monitorCode == monitorAllocation.monitorCode);

            var ownerCodeExist = await _context.tbl_ictams_monitoralloc.Where(x => x.OwnerCode == monitorAllocation.OwnerCode)
                .FirstOrDefaultAsync();


            var ownerExists = await _context.tbl_ictams_monitoralloc.AnyAsync(x => x.OwnerCode == monitorAllocation.OwnerCode && x.DateCreated >= DateTime.Now.AddYears(4));

            var findOwnerInBorrowed = await _context.tbl_ictams_monitorborrowed.Where(x => x.OwnerID == monitorAllocation.OwnerCode && x.StatusID == "AC").FirstOrDefaultAsync();

            if (findOwnerInBorrowed != null)
            {
                TempData["AlertMessage"] = "The owner borrowed a monitor. Kindly return it to reallocate!";
                return RedirectToAction("Create");
            }

            if (monitorAllocation.monitorCode != null)
            {
                var findLaptop = await _context.tbl_ictams_monitorinv
                .Where(x => x.monitorCode == monitorAllocation.monitorCode)
                .FirstOrDefaultAsync();

                var findQuantity = findLaptop.Quantity - findLaptop.AllocatedNo;

                var FindQ = await _context.tbl_ictams_monitorborrowed
                   .Where(z => z.StatusID == "AC")
                   .CountAsync(x => x.UnitID == monitorAllocation.monitorCode);

                if (FindQ == findQuantity)
                {
                    TempData["AlertMessage"] = "The available monitor has been borrowed!";
                    return RedirectToAction("Create");
                }
            }
            else
            {
                if (monitorAllocation.monitorCode == null)
                {
                    TempData["AlertMessage"] = "The monitor cannot be null!";
                    return RedirectToAction("Create");
                }
            }


            if (MonitorAllocationExistsOwner(monitorAllocation.OwnerCode))
            {
                TempData["AlertMessage"] = "The Selected Owner is already allocated with a device or Inventory Monitor! Select another Owner to Allocate with! Need to wait after 4 years!";
                return RedirectToAction("Create");
            }

            if (monitorAllocation.OwnerCode == null)
            {
                TempData["AlertMessage"] = "Owner code cannot be null or Empty!";
                return RedirectToAction("Create");
            }


            if (allocatedQuantity < availableQuantity)
            {
                var paramCode = await _context.tbl_ictams_parameters
                    .Where(p => p.parm_code == "mona_id")
                    .MaxAsync(p => p.parm_value);
                var newparamCode = paramCode + 1;

                var param = await _context.tbl_ictams_parameters
                    .FirstOrDefaultAsync(p => p.parm_code == "mona_id");
                param.parm_value = newparamCode;


                var allocQuantity = await _context.tbl_ictams_monitorinv.Where(a => a.monitorCode == monitorAllocation.monitorCode)
                    .MaxAsync(a => a.AllocatedNo);
                var newallocQuantity = allocQuantity + 1;
                var inv_alloc = await _context.tbl_ictams_monitorinv.FirstOrDefaultAsync(a => a.monitorCode == monitorAllocation.monitorCode);
                inv_alloc.AllocatedNo = newallocQuantity;
                var inv_details = await _context.tbl_ictams_monitordetails
                    .FirstOrDefaultAsync(a => a.SerialNumber == monitorAllocation.SerialNumber);
                inv_details.MonitorStatus = "AC";
                inv_details.DeployedDate = DateTime.Now;


                var ucode = HttpContext.Session.GetString("UserName");
                monitorAllocation.AllocId = "MTA" + newparamCode.ToString().PadLeft(12, '0');
                monitorAllocation.FixedassetTag = monitorAllocation.FixedassetTag.ToUpper();
                monitorAllocation.SerialNumber = monitorAllocation.SerialNumber.ToUpper();
                monitorAllocation.AllocCreated = ucode;
                monitorAllocation.DateCreated = DateTime.Now;
                monitorAllocation.DateDeployed = DateTime.Now;

                _context.Add(monitorAllocation);
                await _context.SaveChangesAsync();
                TempData["SuccessNotification"] = "Successfully added a new allocation!";
                return RedirectToAction(nameof(Index));
            }
            else if (allocatedQuantity >= availableQuantity)
            {
                TempData["AlertMessage"] = "The monitor inventory you selected has been fully allocated!";
                return RedirectToAction("Create");
            }
            if (ownerCodeExist != null)
            {
                TempData["AlertMessage"] = "The Selected Owner already allocated a device!";
                return RedirectToAction("Create");
            }




            return RedirectToAction(nameof(Index));
        }

        // GET: MonitorAllocations/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
          
            ViewData["AllocCreated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode");
            ViewData["SerialNumber"] = new SelectList(_context.Set<MonitorDetail>(), "SerialNumber", "SerialNumber");
            ViewData["monitorCode"] = new SelectList(_context.tbl_ictams_monitorinv, "monitorCode", "monitorCode");
            ViewData["OwnerCode"] = new SelectList(_context.tbl_ictams_owners, "OwnerCode", "OwnerCode");
            ViewData["AllocationStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_code");
            ViewData["AllocUpdated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode");

            if (id == null || _context.tbl_ictams_monitoralloc == null)
            {
                return NotFound();
            }

            var monitorAllocation = await _context.tbl_ictams_monitoralloc.FindAsync(id);
            if (monitorAllocation == null)
            {
                return NotFound();
            }
            return View(monitorAllocation);
        }

        // POST: MonitorAllocations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("AllocId,monitorCode,OwnerCode,SerialNumber,FixedassetTag,DateDeployed,AllocationStatus,AllocCreated,DateCreated,AllocUpdated,DateUpdated")] MonitorAllocation monitorAllocation)
        {
            if (id != monitorAllocation.AllocId)
            {
                return NotFound();
            }

            try
            {
                var ucode = HttpContext.Session.GetString("UserName");
                monitorAllocation.AllocUpdated = ucode;
                monitorAllocation.DateUpdated = DateTime.Now;
                _context.Update(monitorAllocation);
                await _context.SaveChangesAsync();
                // ...
                TempData["SuccessNotification"] = "Successfully edited!";
                // ...
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MonitorAllocationExists(monitorAllocation.AllocId))
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
                var allocationID = await _context.tbl_ictams_monitoralloc.FindAsync(id);
                if (allocationID != null)
                {
                    // Check if the selected profile status exists in the tbl_lsm_status table
                    var status = await _context.tbl_ictams_status.FindAsync(allocationID.AllocationStatus);
                    if (status == null)
                    {
                        ModelState.AddModelError("", $"Profile status '{allocationID.AllocId}' does not exist.");
                    }

                    // Find the corresponding LaptopCode in tbl_ictams_laptopinv and deduct 1 from AllocatedNo
                    var laptopCode = allocationID.monitorCode;
                    var laptopInv = await _context.tbl_ictams_monitorinv.FirstOrDefaultAsync(a => a.monitorCode == laptopCode);
                    if (laptopInv != null)
                    {
                        laptopInv.AllocatedNo -= 1;
                    }

                    // Update the profile
                    var ucode = HttpContext.Session.GetString("UserName");
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

        private bool MonitorAllocationExistsOwner(string id)
        {
            return (_context.tbl_ictams_monitoralloc?.Any(e => e.OwnerCode == id && e.AllocationStatus == "AC")).GetValueOrDefault();
        }

        private bool MonitorAllocationExists(string id)
        {
          return (_context.tbl_ictams_monitoralloc?.Any(e => e.AllocId == id)).GetValueOrDefault();
        }
    }
}
