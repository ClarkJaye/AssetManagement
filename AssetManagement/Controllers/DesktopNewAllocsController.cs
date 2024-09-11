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
    public class DesktopNewAllocsController : BaseController
    {
        private readonly AssetManagementContext _context;

        public DesktopNewAllocsController(AssetManagementContext context)
            : base(context)
        {
            _context = context;
        }
        public async Task<IActionResult> seeDetails(string userCODE)
        {
            if (!string.IsNullOrEmpty(userCODE))
            {
                var desktopinventory = await _context.tbl_ictams_desktopinv
                    .Where(x => x.DTStatus == "AV" || x.DTStatus == "CO")
                    .Include(l => l.Brand)
                    .Include(l => l.CPU)
                    .Include(l => l.Createdby)
                    .Include(l => l.HardDisk)
                    .Include(l => l.Level)
                    .Include(l => l.Memory)
                    .Include(l => l.Model)
                    .Include(l => l.GraphicsCard)
                    .Include(l => l.MainBoard)
                    .Include(l => l.OS)
                    .Include(l => l.Status)
                    .Include(l => l.Updatedby)
                    .FirstOrDefaultAsync(x => x.Description == userCODE);

                return View(desktopinventory);
            }

            return View();
        }
        public async Task<IActionResult> Inactive()
        {
            await FindStatus();
            //EXECUTE VIEWS
            var assetManagementContext1 = await _context.tbl_ictams_dtnewalloc.Where(x => x.SecAllocationStatus == "IN")
                .Include(s => s.Createdby)
                .Include(s => s.DesktopAllocation)
                .Include(s => s.Updatedby)
                .Include(s => s.InventoryDetails)
                .Include(s => s.Owner)
                .Include(s => s.Status)
                .Include(s => s.Updatedby).ToListAsync();
            return View(assetManagementContext1);
        }
        public async Task<IActionResult> SecondOwnerViews()
        {
            await FindStatus();
            //EXECUTE VIEWS
            var assetManagementContext1 = await _context.tbl_ictams_dtnewalloc.Where(x => x.SecAllocationStatus == "IN").Include(s => s.Createdby).Include(s => s.DesktopAllocation).Include(s => s.DesktopInventory).Include(s => s.InventoryDetails).Include(s => s.Owner).Include(s => s.Status).Include(s => s.Updatedby).ToListAsync();
            return View(assetManagementContext1);
        }


        // GET: DesktopNewAllocs
        public async Task<IActionResult> Index()
        {
            await FindDesktopAllocationCompleted();
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

                    var assetManagementContext = await _context.tbl_ictams_dtnewalloc.Where(x => x.SecAllocationStatus != "IN").Include(s => s.Createdby).Include(s => s.DesktopAllocation).Include(s => s.InventoryDetails).Include(s => s.DesktopInventory).Include(s => s.Owner).Include(s => s.Status).Include(s => s.Updatedby).ToListAsync();
                    return View(assetManagementContext);
                }
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: DesktopNewAllocs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.tbl_ictams_dtnewalloc == null)
            {
                return NotFound();
            }

            var desktopNewAlloc = await _context.tbl_ictams_dtnewalloc
                .Include(d => d.Createdby)
                .Include(d => d.DesktopAllocation)
                .Include(d => d.DesktopInventory)
                .Include(d => d.InventoryDetails)
                .Include(d => d.Owner)
                .Include(d => d.Status)
                .Include(d => d.Updatedby)
                .FirstOrDefaultAsync(m => m.SecAllocId == id);
            if (desktopNewAlloc == null)
            {
                return NotFound();
            }

            return View(desktopNewAlloc);
        }

        // GET: DesktopNewAllocs/Create
        public IActionResult Create(string id, string ids, string ids2)
        {
            ViewBag.AllocId = id;
            ViewBag.DesktopCode = ids;
            ViewBag.UnitTag = ids2;
            return View();
        }

        // POST: DesktopNewAllocs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SecAllocId,AllocId,SecDesktopCode,UnitTag,SecOwnerCode,SecAllocationStatus,AllocCreated,DateCreated,AllocUpdated,DateUpdated")] DesktopNewAlloc desktopNewAlloc)
        {
            var ucode = HttpContext.Session.GetString("UserName");
            var findOwnerInBorrowed = await _context.tbl_ictams_dtborrowed.Where(x => x.OwnerID == desktopNewAlloc.SecOwnerCode && x.StatusID == "AC").FirstOrDefaultAsync();

            if (findOwnerInBorrowed != null)
            {
                TempData["AlertMessage"] = "The selected owner had borrowed a laptop. Please return it first to reallocate!";
                return RedirectToAction("Create");
            }

            var findFirstOwner = await _context.tbl_ictams_desktopalloc.Where(x => x.AllocId == desktopNewAlloc.AllocId && x.AllocationStatus == "AC").FirstOrDefaultAsync();
            if (findFirstOwner != null)
            {
                findFirstOwner.AllocationStatus = "IN";
                findFirstOwner.AllocUpdated = ucode;
                findFirstOwner.DateUpdated = DateTime.Now;
            }
            var paramCode = await _context.tbl_ictams_parameters
               .Where(p => p.parm_code == "dtas_id")
               .MaxAsync(p => p.parm_value);
            var newparamCode = paramCode + 1;

            var param = await _context.tbl_ictams_parameters
                .FirstOrDefaultAsync(p => p.parm_code == "dtas_id");
            param.parm_value = newparamCode;


            desktopNewAlloc.SecAllocId = "DTAS" + newparamCode.ToString().PadLeft(11, '0');
            desktopNewAlloc.SecAllocationStatus = "AC";
            desktopNewAlloc.DateCreated = DateTime.Now;
            desktopNewAlloc.AllocCreated = ucode;
            _context.Add(desktopNewAlloc);
            await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully transfer to a  second owner!";
            // ...
            return RedirectToAction(nameof(Index));
        }

        // GET: DesktopNewAllocs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.tbl_ictams_dtnewalloc == null)
            {
                return NotFound();
            }

            var secondOwnerAlloc = await _context.tbl_ictams_dtnewalloc.FindAsync(id);
            if (secondOwnerAlloc == null)
            {
                return NotFound();
            }

            return View(secondOwnerAlloc);
        }

        // POST: DesktopNewAllocs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("SecAllocId,AllocId,SecDesktopCode,UnitTag,SecOwnerCode,SecAllocationStatus,AllocCreated,DateCreated,AllocUpdated,DateUpdated")] DesktopNewAlloc desktopNewAlloc)
        {
            if (id != desktopNewAlloc.SecAllocId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(desktopNewAlloc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DesktopNewAllocExists(desktopNewAlloc.SecAllocId))
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
            return View(desktopNewAlloc);
        }

        // GET: DesktopNewAllocs/Delete/5
        public async Task<IActionResult> DeleteAsEdit(string[] selectedIds)
        {
            //ViewData["UserStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_name");
            foreach (string id in selectedIds)
            {
                var dtsecownerID = await _context.tbl_ictams_dtnewalloc.FindAsync(id);
                if (dtsecownerID != null)
                {
                    // Check if the selected profile status exists in the tbl_lsm_status table
                    var status = await _context.tbl_ictams_status.FindAsync(dtsecownerID.SecAllocId);
                    if (status == null)
                    {
                        ModelState.AddModelError("", $"Profile status '{dtsecownerID.SecAllocId}' does not exist.");
                    }

                    // Find the corresponding LaptopCode in tbl_ictams_laptopinv and deduct 1 from AllocatedNo
                    var desktopCode = dtsecownerID.SecDesktopCode;
                    var desktopInv = await _context.tbl_ictams_desktopinv.FirstOrDefaultAsync(a => a.desktopInvCode == desktopCode);
                    if (desktopInv != null)
                    {
                        desktopInv.AllocatedNo -= 1;
                    }

                    // Update the profile
                    var ucode = HttpContext.Session.GetString("UserName");
                    dtsecownerID.AllocUpdated = ucode;
                    dtsecownerID.SecAllocationStatus = "IN";
                    dtsecownerID.DateUpdated = DateTime.Now;

                    _context.Update(dtsecownerID);
                    await _context.SaveChangesAsync();
                    // ...
                    TempData["SuccessNotification"] = "Successfully deleted a second owner allocation!";
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
                var dtsecownerID = await _context.tbl_ictams_dtnewalloc.FindAsync(id);
                if (dtsecownerID != null)
                {
                    // Check if the selected profile status exists in the tbl_lsm_status table
                    var status = await _context.tbl_ictams_status.FindAsync(dtsecownerID.SecAllocId);
                    if (status == null)
                    {
                        ModelState.AddModelError("", $"Profile status '{dtsecownerID.SecAllocId}' does not exist.");
                    }

                    // Find the corresponding LaptopCode in tbl_ictams_laptopinv and deduct 1 from AllocatedNo
                    var desktopCode = dtsecownerID.SecDesktopCode;

                    // Find the corresponding LaptopCode in tbl_ictams_laptopinv
                    var desktopInv = await _context.tbl_ictams_desktopinv.FirstOrDefaultAsync(a => a.desktopInvCode == desktopCode);

                    if (desktopInv != null)
                    {
                        // Check if the Quantity and AllocatedNo are equal
                        if (desktopInv.Quantity == desktopInv.AllocatedNo)
                        {
                            TempData["AlertMessage"] = "Cannot retrieve data. It is already full!";
                            return RedirectToAction("Inactive");
                        }
                        else
                        {
                            // Deduct 1 from AllocatedNo
                            desktopInv.AllocatedNo += 1;
                            // Update the profile
                            var ucode = HttpContext.Session.GetString("UserName");
                            dtsecownerID.AllocUpdated = ucode;
                            dtsecownerID.SecAllocationStatus = "AC";
                            dtsecownerID.DateUpdated = DateTime.Now;

                            _context.Update(dtsecownerID);
                            await _context.SaveChangesAsync();
                            // ...
                            TempData["SuccessNotification"] = "Successfully retrieve a second owner allocation!  ";
                            // ...
                            return RedirectToAction(nameof(Index));
                        }
                    }


                }
            }
            return RedirectToAction(nameof(Index));
        }

        private bool DesktopNewAllocExists(string id)
        {
          return (_context.tbl_ictams_dtnewalloc?.Any(e => e.SecAllocId == id)).GetValueOrDefault();
        }
    }
}
