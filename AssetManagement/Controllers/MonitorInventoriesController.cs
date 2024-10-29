using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Data;
using AssetManagement.Models;
using AssetManagement.Utility;

namespace AssetManagement.Controllers
{
    public class MonitorInventoriesController : BaseController
    {
        private readonly AssetManagementContext _context;

        public MonitorInventoriesController(AssetManagementContext context)
        : base(context)
        {
            _context = context;
        }
        public async Task<IActionResult> SeeAll(string id, string ids, string id2)
        {
            ViewBag.Id = id;
            ViewBag.Ids = ids;
            ViewBag.Id2 = id2;
            var assetManagementContext = _context.tbl_ictams_monitordetails.Where(x => x.monitorCode == id).Include(i => i.Createdby).Include(i => i.MonitorInventory).Include(i => i.Status).Include(i => i.Updatedby).Include(i => i.Vendor);
            return View(await assetManagementContext.ToListAsync());
        }
        public async Task<IActionResult> SeeAllSerial(string id)
        {
            var assetManagementContext = _context.tbl_ictams_monitordetails.Where(x => x.monitorCode == id && x.MonitorStatus == "AV").Include(i => i.Createdby).Include(i => i.MonitorInventory).Include(i => i.Status).Include(i => i.Updatedby).Include(i => i.Vendor);
            return View(await assetManagementContext.ToListAsync());
        }

        public async Task<IActionResult> DeletedHistory(string userCODE)
        {

            var assetManagementContext = _context.tbl_ictams_monitoralloc.Include(l => l.Status).Include(l => l.Createdby).Include(l => l.MonitorDetails.MonitorInventory).Where(x => x.AllocationStatus == "IN" && x.monitorCode.Contains(userCODE)).Include(l => l.Owner).Include(l => l.Updatedby).Include(l => l.MonitorDetails);
            return View(await assetManagementContext.ToListAsync());
        }
        public async Task<IActionResult> ReturnDetails(string userCODE)
        {
            if (!string.IsNullOrEmpty(userCODE))
            {
                var assetManagementContext = _context.tbl_ictams_monitorreturn.Where(x => x.MonitorAllocation.monitorCode.Contains(userCODE)).Include(r => r.MonitorAllocation).Include(r => r.ReturnType).Include(r => r.Status).Include(r => r.UserCreated).Include(r => r.UserUpdated);
                return View(await assetManagementContext.ToListAsync());
            }
            return View();
        }
        public async Task<IActionResult> Borrowed(string userCODE)
        {
            if (!string.IsNullOrEmpty(userCODE))
            {
                var assetManagementContext = _context.tbl_ictams_monitorborrowed.Where(x => x.StatusID == "AC" && x.UnitID.Contains(userCODE)).Include(l => l.MonitorDetail.MonitorInventory).Include(l => l.Owner).Include(l => l.Status).Include(l => l.UserCreated).Include(l => l.UserUpdated);
                return View(await assetManagementContext.ToListAsync());
            }
            return View();
        }
        // GET: MonitorAllocations
        public async Task<IActionResult> InventoryDetails(string id, string ids, string id2)
        {
            ViewBag.Id = id;
            ViewBag.Ids = ids;
            ViewBag.Id2 = id2;

            var assetManagementContext = _context.tbl_ictams_monitoralloc.Where(x => x.AllocationStatus == "AC" && x.monitorCode == id).Include(l => l.Status).Include(l => l.Createdby).Include(l => l.MonitorDetails.MonitorInventory).Include(l => l.Owner).Include(l => l.Updatedby).Include(l => l.MonitorDetails);
            return View(await assetManagementContext.ToListAsync());
        }
        public async Task<IActionResult> seeDetails(string userCODE)
        {
            if (!string.IsNullOrEmpty(userCODE))
            {
                var monitorInventory = await _context.tbl_ictams_monitorinv
                .Where(x => x.MonitorStatus == "AV" || x.MonitorStatus == "CO")
                .Include(l => l.Createdby)
                .Include(l => l.Model)
                .Include(l => l.Status)
                .Include(l => l.Updatedby)
                .FirstOrDefaultAsync(x => x.monitorCode == userCODE);
                return View(monitorInventory);
            }
            return View();
        }
        public async Task<IActionResult> InventorySecOwnerDetails(string userCODE)
        {
            if (!string.IsNullOrEmpty(userCODE))
            {
                var assetManagementContext = await _context.tbl_ictams_monitornewalloc.Where(x => x.SecAllocationStatus == "AC" && x.SecMonitorCode.Contains(userCODE)).Include(s => s.Createdby).Include(s => s.MonitorAllocation).Include(s => s.MonitorAllocation.MonitorDetails.MonitorInventory).Include(s => s.Owner).Include(s => s.Status).Include(s => s.Updatedby).Include(s => s.MonitorDetail).ToListAsync();

                return View(assetManagementContext);
            }
            return View();
        }
        // GET: MonitorInventories
        public async Task<IActionResult> MonitorInvPartialView()
        {
            var assetManagementContext = _context.tbl_ictams_monitorinv
                .Where(x => x.MonitorStatus == "AV")
                .Include(l => l.Createdby)
                .Include(l => l.Model)
                .Include(l => l.Status)
                .Include(l => l.Updatedby);

            return View(await assetManagementContext.ToListAsync());

        }
        // GET: MonitorInventories
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

            await FindAllocationCompleted();
            int? userProfile = HttpContext.Session.GetInt32("UserProfile");
            if (userProfile.HasValue)
            {

                var hasOpenAccess = await _context.tbl_ictams_profileaccess
          .AnyAsync(pa => pa.OpenAccess == "Y" &&
                          pa.Module.ModuleTitle == "Monitor Inventories" &&  
                          pa.ProfileId == userProfile.Value);
                if (!hasOpenAccess)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    await FindStatus();
                    var countAvailLT = await _context.tbl_ictams_monitorinv.Where(x => x.Quantity > x.AllocatedNo && x.MonitorStatus == "AV").SumAsync(x => x.Quantity - x.AllocatedNo);
                    var totalAllocatedLaptops = await _context.tbl_ictams_monitorinv.SumAsync(x => x.AllocatedNo);

                    var assetManagementContext = _context.tbl_ictams_monitorinv.Where(x => x.MonitorStatus == "AV" || x.MonitorStatus == "CO").Include(l => l.Createdby).Include(l => l.Model).Include(l => l.Status).Include(l => l.Updatedby);

                    ViewBag.TotalAvailableLaptops = countAvailLT;
                    ViewBag.TotalAllocatedLaptops = totalAllocatedLaptops;

                    return View(await assetManagementContext.ToListAsync());
                }
            }

            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> RetrieveRow(string monitorCode)
        {
            if (string.IsNullOrEmpty(monitorCode))
            {
                // Handle the case where the laptopCode parameter is empty or null
                return BadRequest();
            }

            var monitorInventory = await _context.tbl_ictams_monitorinv
                .Where(x => x.MonitorStatus == "AV" || x.MonitorStatus == "CO")
                .Include(l => l.Createdby)
                .Include(l => l.Model)
                .Include(l => l.Status)
                .Include(l => l.Updatedby)
                .FirstOrDefaultAsync(x => x.Description == monitorCode);

            if (monitorInventory == null)
            {
                // Handle the case where the laptop with the given laptopCode is not found
                return NotFound();
            }

            return View("~/Views/MonitorInventories/RetrieveRow.cshtml", monitorInventory);

        }
        public async Task<IActionResult> Inactive()
        {
            await FindStatus();
            var assetManagementContext = _context.tbl_ictams_monitorinv.Where(x => x.MonitorStatus == "IN").Include(l => l.Createdby).Include(l => l.Model).Include(l => l.Status).Include(l => l.Updatedby);
            return View(await assetManagementContext.ToListAsync());
        }

        // GET: MonitorInventories/Create
        public IActionResult Create()
        {
            ViewData["MonitorCreatedBy"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode");
            ViewData["MonitorModel"] = new SelectList(_context.tbl_ictams_model, "ModelId", "ModelId");
            ViewData["MonitorStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_code");
            ViewData["MonitorUpdatedBy"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode");
            return View();
        }

        // POST: MonitorInventories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("monitorCode,Description,MonitorModel,Quantity,AllocatedNo,MonitorStatus,MonitorCreatedBy,DateCreated,MonitorUpdatedBy,DateUpdated")] MonitorInventory monitorInventory)
        {
            if (monitorInventory.MonitorModel.Equals(0))
            {
                TempData["AlertMessage"] = "Please select a model!";
                return RedirectToAction("Create");
            }
            else
            {
                var paramCode = await _context.tbl_ictams_parameters.Where(p => p.parm_code == "mon_id").MaxAsync(p => p.parm_value);
                var newparamCode = paramCode + 1;
                var param = await _context.tbl_ictams_parameters.FirstOrDefaultAsync(p => p.parm_code == "mon_id");
                param.parm_value = newparamCode;


                var ucode = HttpContext.Session.GetString("UserName");
                monitorInventory.monitorCode = "MT" + newparamCode.ToString().PadLeft(8, '0');
                monitorInventory.Description = monitorInventory.Description.ToUpper();
                monitorInventory.MonitorCreatedBy = ucode;
                monitorInventory.DateCreated = DateTime.Now;
                monitorInventory.MonitorStatus = "AV";
                _context.Add(monitorInventory);
                await _context.SaveChangesAsync();
                // ...
                TempData["SuccessNotification"] = "Successfully added a new device to inventory";
                // ...
                return RedirectToAction(nameof(Index));

            }

        }
        // GET: MonitorInventories/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var monitorInventory = await _context.tbl_ictams_monitorinv
              .Include(x => x.Model)
              .FirstOrDefaultAsync(x => x.monitorCode == id);

            if (monitorInventory == null)
            {
                return NotFound();
            }

            ViewData["Mmodel"] = new SelectList(_context.tbl_ictams_model, "ModelId", "ModelDescription", monitorInventory.Model);
            return View(monitorInventory);
        }

        // POST: MonitorInventories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("monitorCode,Description,MonitorModel,Quantity,AllocatedNo,MonitorStatus,MonitorCreatedBy,DateCreated,MonitorUpdatedBy,DateUpdated")] MonitorInventory monitorInventory)
        {

            var record = await _context.tbl_ictams_monitorinv.Where(x => x.monitorCode == monitorInventory.monitorCode).FirstOrDefaultAsync();
            if (record.Quantity >= 1)
            {
                TempData["AlertMessage"] = "You can't edit this!";
                return RedirectToAction("Edit");
            }
            try
            {
                var ucode = HttpContext.Session.GetString("UserName");
                record.MonitorUpdatedBy = ucode;
                record.DateUpdated = DateTime.Now;
                record.MonitorStatus = "AV";
                record.MonitorModel = monitorInventory.MonitorModel;
                record.Description = monitorInventory.Description;
                await _context.SaveChangesAsync();
                // ...
                TempData["SuccessNotification"] = "Successfully edited!";
                // ...

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MonitorInventoryExists(monitorInventory.monitorCode))
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

        public async Task<IActionResult> DeleteAsEdit(string[] selectedIds)
        {
            ViewData["UserStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_name");
            foreach (string id in selectedIds)
            {
                var inventoryCode = await _context.tbl_ictams_monitorinv.FindAsync(id);
                var mtinvExist = await _context.tbl_ictams_monitoralloc.FirstOrDefaultAsync(x => x.monitorCode == id);
                if (inventoryCode != null)
                {
                    // Check if the selected profile status exists in the tbl_lsm_status table
                    var status = await _context.tbl_ictams_status.FindAsync(inventoryCode.MonitorStatus);
                    if (status == null)
                    {
                        ModelState.AddModelError("", $"Profile status '{inventoryCode.monitorCode}' does not exist.");
                    }
                    if (mtinvExist != null)
                    {
                        TempData["AlertMessage"] = "Deletion Not Permitted: Device Already Allocated. The selected device cannot be deleted as it is currently allocated in the Monitor Allocation system. This device is already in use and its allocation status prevents its removal at this time.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        // Update the profile
                        var ucode = HttpContext.Session.GetString("UserName");
                        inventoryCode.MonitorUpdatedBy = ucode;
                        inventoryCode.MonitorStatus = "IN";
                        inventoryCode.DateUpdated = DateTime.Now;

                        _context.Update(inventoryCode);
                        await _context.SaveChangesAsync();
                        // ...
                        TempData["SuccessNotification"] = "Successfully deleted!";
                        // ...
                        return RedirectToAction(nameof(Index));
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Retrieve(string[] selectedIds)
        {
            ViewData["UserStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_name");
            foreach (string id in selectedIds)
            {
                var inventoryCode = await _context.tbl_ictams_monitorinv.FindAsync(id);
                if (inventoryCode != null)
                {
                    // Check if the selected profile status exists in the tbl_lsm_status table
                    var status = await _context.tbl_ictams_status.FindAsync(inventoryCode.monitorCode);
                    if (status == null)
                    {
                        ModelState.AddModelError("", $"Profile status '{inventoryCode.monitorCode}' does not exist.");
                    }

                    // Update the profile
                    var ucode = HttpContext.Session.GetString("UserName");
                    inventoryCode.MonitorUpdatedBy = ucode;
                    inventoryCode.MonitorStatus = "AV";
                    inventoryCode.DateUpdated = DateTime.Now;

                    _context.Update(inventoryCode);
                    await _context.SaveChangesAsync();

                    // ...
                    TempData["SuccessNotification"] = "Successfully retrieved!";
                    // ...
                    return RedirectToAction(nameof(Index));
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool MonitorInventoryExists(string id)
        {
          return (_context.tbl_ictams_monitorinv?.Any(e => e.monitorCode == id)).GetValueOrDefault();
        }
    }
}
