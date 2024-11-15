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
using System.Drawing;

namespace AssetManagement.Controllers
{
    public class MonitorNewAllocsController : BaseController
    {
        private readonly AssetManagementContext _context;

        public MonitorNewAllocsController(AssetManagementContext context)
            : base(context)
        {
            _context = context;
        }
        public async Task<IActionResult> seeDetails(string code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                var monitorInventory = await _context.tbl_ictams_monitorinv
                    .Where(x => x.MonitorStatus == "AV" || x.MonitorStatus == "CO")
                    .Include(l => l.Createdby)
                    .Include(l => l.Model)
                    .Include(l => l.Status)
                    .Include(l => l.Updatedby)
                    .FirstOrDefaultAsync(x => x.monitorCode == code);

                return View(monitorInventory);
            }

            return View();
        }
        public async Task<IActionResult> Inactive()
        {
            await FindStatus();
            //EXECUTE VIEWS
            var assetManagementContext1 = await _context.tbl_ictams_monitornewalloc.Where(x => x.SecAllocationStatus == "IN")
                .Include(s => s.Createdby)
                .Include(s => s.MonitorAllocation)
                .ThenInclude(s => s.MonitorDetails.MonitorInventory)
                .Include(s => s.Updatedby)
                .Include(s => s.MonitorDetail)
                .Include(s => s.Owner).Include(s => s.Status).Include(s => s.Updatedby).ToListAsync();
            return View(assetManagementContext1);
        }
        public async Task<IActionResult> SecondOwnerViews()
        {
            await FindStatus();
            //EXECUTE VIEWS
            var assetManagementContext1 = await _context.tbl_ictams_monitornewalloc.Where(x => x.SecAllocationStatus == "IN").Include(s => s.Createdby).Include(s => s.MonitorAllocation).Include(s => s.MonitorAllocation.MonitorDetails.MonitorInventory).Include(s => s.MonitorDetail).Include(s => s.Owner).Include(s => s.Status).Include(s => s.Updatedby).ToListAsync();
            return View(assetManagementContext1);
        }

        // GET: MonitorNewAllocs
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
                          pa.Module.ModuleTitle == "Second Owner Allocs" &&  // Adjust the module name as needed
                          pa.ProfileId == userProfile.Value);
                if (!hasOpenAccess)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    await FindStatus();

                    var assetManagementContext = await _context.tbl_ictams_monitornewalloc.Where(x => x.SecAllocationStatus == "AC")
                        .Include(s => s.MonitorAllocation)
                        .Include(s => s.MonitorAllocation.MonitorDetails)
                        .Include(s => s.MonitorAllocation.MonitorDetails.MonitorInventory)
                        .Include(s => s.Owner)
                        .Include(s => s.Status)
                        .Include(s => s.Createdby)
                        .Include(s => s.Updatedby).ToListAsync();
                    return View(assetManagementContext);
                }
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: MonitorNewAllocs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.tbl_ictams_monitornewalloc == null)
            {
                return NotFound();
            }

            var secondOwnerAlloc = await _context.tbl_ictams_monitornewalloc
                .Include(s => s.Createdby)
                .Include(s => s.MonitorAllocation.MonitorDetails.MonitorInventory)
                .Include(s => s.MonitorAllocation.MonitorDetails)
                .Include(s => s.Owner)
                .Include(s => s.Status)
                .Include(s => s.Updatedby)
                .FirstOrDefaultAsync(m => m.SecAllocId == id);
            if (secondOwnerAlloc == null)
            {
                return NotFound();
            }

            return View(secondOwnerAlloc);
        }

        // GET: MonitorNewAllocs/Create
        public IActionResult Create(string id, string ids, string ids2)
        {
            ViewBag.AllocId = id;
            ViewBag.SecMonitorCode = ids;
            ViewBag.SerialNumber = ids2;
            return View();
        }

        // POST: MonitorNewAllocs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SecAllocId,AllocId,SecMonitorCode,SerialNumber,SecOwnerCode,SecAllocationStatus,AllocCreated,DateCreated,AllocUpdated,DateUpdated")] MonitorNewAlloc monitorNewAlloc)
        {
            var ucode = HttpContext.Session.GetString("UserName");
            var findOwnerInBorrowed = await _context.tbl_ictams_monitorborrowed.Where(x => x.OwnerID == monitorNewAlloc.SecOwnerCode && x.StatusID == "AC").FirstOrDefaultAsync();

            if (findOwnerInBorrowed != null)
            {
                TempData["AlertMessage"] = "The selected owner had borrowed a monitor. Please return it first to reallocate!";
                return RedirectToAction("Create");
            }

            var findFirstOwner = await _context.tbl_ictams_monitoralloc.Where(x => x.AllocId == monitorNewAlloc.AllocId && x.AllocationStatus == "AC").FirstOrDefaultAsync();
            if (findFirstOwner != null)
            {
                findFirstOwner.AllocationStatus = "IN";
                findFirstOwner.AllocUpdated = ucode;
                findFirstOwner.DateUpdated = DateTime.Now;
            }
            var paramCode = await _context.tbl_ictams_parameters
               .Where(p => p.parm_code == "monas_id")
               .MaxAsync(p => p.parm_value);
            var newparamCode = paramCode + 1;

            var param = await _context.tbl_ictams_parameters
                .FirstOrDefaultAsync(p => p.parm_code == "monas_id");
            param.parm_value = newparamCode;


            monitorNewAlloc.SecAllocId = "MTAS" + newparamCode.ToString().PadLeft(11, '0');
            monitorNewAlloc.SecAllocationStatus = "AC";
            monitorNewAlloc.DateCreated = DateTime.Now;
            monitorNewAlloc.AllocCreated = ucode;
            _context.Add(monitorNewAlloc);
            await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully transfer to a  new allocation!";
            // ...
            return RedirectToAction(nameof(Index));
        }

        // GET: MonitorNewAllocs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.tbl_ictams_monitoralloc == null)
            {
                return NotFound();
            }

            var secondOwnerAlloc = await _context.tbl_ictams_monitoralloc.FindAsync(id);
            if (secondOwnerAlloc == null)
            {
                return NotFound();
            }

            return View(secondOwnerAlloc);
        }

        // POST: MonitorNewAllocs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("SecAllocId,AllocId,SecMonitorCode,SerialNumber,SecOwnerCode,SecAllocationStatus,AllocCreated,DateCreated,AllocUpdated,DateUpdated")] MonitorNewAlloc monitorNewAlloc)
        {
            if (id != monitorNewAlloc.SecAllocId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(monitorNewAlloc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MonitorNewAllocExists(monitorNewAlloc.SecAllocId))
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
            return View(monitorNewAlloc);
        }

        // GET: MonitorNewAllocs/Delete/5
        //DELETE
        public async Task<IActionResult> DeleteAsEdit(string[] selectedIds)
        {
            //ViewData["UserStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_name");
            foreach (string id in selectedIds)
            {
                var ltsecownerID = await _context.tbl_ictams_monitornewalloc.FindAsync(id);
                if (ltsecownerID != null)
                {
                    // Check if the selected profile status exists in the tbl_lsm_status table
                    var status = await _context.tbl_ictams_status.FindAsync(ltsecownerID.SecAllocId);
                    if (status == null)
                    {
                        ModelState.AddModelError("", $"Profile status '{ltsecownerID.SecAllocId}' does not exist.");
                    }

                    // Find the corresponding LaptopCode in tbl_ictams_laptopinv and deduct 1 from AllocatedNo
                    var laptopCode = ltsecownerID.SecMonitorCode;
                    var laptopInv = await _context.tbl_ictams_monitorinv.FirstOrDefaultAsync(a => a.monitorCode == laptopCode);
                    if (laptopInv != null)
                    {
                        laptopInv.AllocatedNo -= 1;
                    }

                    // Update the profile
                    var ucode = HttpContext.Session.GetString("UserName");
                    ltsecownerID.AllocUpdated = ucode;
                    ltsecownerID.SecAllocationStatus = "IN";
                    ltsecownerID.DateUpdated = DateTime.Now;

                    _context.Update(ltsecownerID);
                    await _context.SaveChangesAsync();
                    // ...
                    TempData["SuccessNotification"] = "Successfully deleted a new allocation!";
                    // ...
                    return RedirectToAction(nameof(Index));
                }
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Retrieve(string[] selectedIds)
        {
            //ViewData["UserStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_name");
            foreach (string id in selectedIds)
            {
                var ltsecownerID = await _context.tbl_ictams_monitornewalloc.FindAsync(id);
                if (ltsecownerID != null)
                {
                    // Check if the selected profile status exists in the tbl_lsm_status table
                    var status = await _context.tbl_ictams_status.FindAsync(ltsecownerID.SecAllocId);
                    if (status == null)
                    {
                        ModelState.AddModelError("", $"Profile status '{ltsecownerID.SecAllocId}' does not exist.");
                    }

                    // Find the corresponding LaptopCode in tbl_ictams_laptopinv and deduct 1 from AllocatedNo
                    var laptopCode = ltsecownerID.SecMonitorCode;

                    // Find the corresponding LaptopCode in tbl_ictams_laptopinv
                    var laptopInv = await _context.tbl_ictams_monitorinv.FirstOrDefaultAsync(a => a.monitorCode == laptopCode);

                    if (laptopInv != null)
                    {
                        // Check if the Quantity and AllocatedNo are equal
                        if (laptopInv.Quantity == laptopInv.AllocatedNo)
                        {
                            TempData["AlertMessage"] = "Cannot retrieve data. It is already full!";
                            return RedirectToAction("Inactive");
                        }
                        else
                        {
                            // Deduct 1 from AllocatedNo
                            laptopInv.AllocatedNo += 1;
                            // Update the profile
                            var ucode = HttpContext.Session.GetString("UserName");
                            ltsecownerID.AllocUpdated = ucode;
                            ltsecownerID.SecAllocationStatus = "AC";
                            ltsecownerID.DateUpdated = DateTime.Now;

                            _context.Update(ltsecownerID);
                            await _context.SaveChangesAsync();
                            // ...
                            TempData["SuccessNotification"] = "Successfully retrieve a new allocation!  ";
                            // ...
                            return RedirectToAction(nameof(Index));
                        }
                    }


                }
            }
            return RedirectToAction(nameof(Index));
        }


        private bool MonitorNewAllocExists(string id)
        {
          return (_context.tbl_ictams_monitornewalloc?.Any(e => e.SecAllocId == id)).GetValueOrDefault();
        }
    }
}
