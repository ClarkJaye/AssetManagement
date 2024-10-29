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
    public class SecondOwnerAllocsController : BaseController
    {
        private readonly AssetManagementContext _context;

        public SecondOwnerAllocsController(AssetManagementContext context) 
           : base(context)
        {
            _context = context;
        }


        public async Task<IActionResult> seeDetails(string userCODE)
        {
            if (!string.IsNullOrEmpty(userCODE))
            {
                var laptopInventory = await _context.tbl_ictams_laptopinv
                    .Where(x => x.LTStatus == "AV" || x.LTStatus == "CO")
                    .Include(l => l.Brand)
                    .Include(l => l.CPU)
                    .Include(l => l.Createdby)
                    .Include(l => l.HardDisk.Capacity)
                    .Include(l => l.Level)
                    .Include(l => l.Memory)
                    .Include(l => l.Model)
                    .Include(l => l.OS)
                    .Include(l => l.Status)
                    .Include(l => l.Updatedby)
                    .FirstOrDefaultAsync(x => x.Description == userCODE);

                return View(laptopInventory);
            }

            return View();
        }



        public async Task<IActionResult> Inactive()
        {
            await FindStatus();
            //EXECUTE VIEWS
            var assetManagementContext1 = await _context.tbl_ictams_ltnewalloc.Where(x => x.SecAllocationStatus == "IN")
                .Include(s => s.Createdby)
                .Include(s => s.LaptopAllocation)
                .Include(s => s.Updatedby)
                .Include(s => s.LaptopInventoryDetails)
                .Include(s => s.Owner).Include(s => s.Status).Include(s => s.Updatedby).ToListAsync();
            return View( assetManagementContext1);
        }

        public async Task<IActionResult> SecondOwnerViews()
        {
            await FindStatus();
            //EXECUTE VIEWS
            var assetManagementContext1 = await _context.tbl_ictams_ltnewalloc.Where(x => x.SecAllocationStatus == "IN").Include(s => s.Createdby).Include(s => s.LaptopAllocation).Include(s => s.LaptopInventoryDetails.LaptopInventory).Include(s => s.LaptopInventoryDetails).Include(s => s.Owner).Include(s => s.Status).Include(s => s.Updatedby).ToListAsync();
            return View(assetManagementContext1);
        }

        // GET: SecondOwnerAllocs
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
                          pa.Module.ModuleTitle == "Second Owner Allocs" &&  // Adjust the module name as needed
                          pa.ProfileId == userProfile.Value);
                if (!hasOpenAccess)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    await FindStatus();

                    var assetManagementContext = await _context.tbl_ictams_ltnewalloc
                        .Where(x => x.SecAllocationStatus == "AC")
                        .Include(s => s.LaptopAllocation)
                        .Include(s => s.LaptopAllocation.LaptopInventoryDetails)
                        .Include(s => s.LaptopAllocation.LaptopInventoryDetails.LaptopInventory)
                        .Include(s => s.Owner)
                        .Include(s => s.Status)
                        .Include(s => s.Createdby)
                        .Include(s => s.Updatedby).ToListAsync();

                    return View(assetManagementContext);
                }
            }

            return RedirectToAction("Index", "Home");

        }

        // GET: SecondOwnerAllocs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.tbl_ictams_ltnewalloc == null)
            {
                return NotFound();
            }

            var secondOwnerAlloc = await _context.tbl_ictams_ltnewalloc
                .Include(s => s.LaptopAllocation)
                .Include(s => s.LaptopAllocation.LaptopInventoryDetails)
                .Include(s => s.LaptopAllocation.LaptopInventoryDetails.LaptopInventory)
                .Include(s => s.Owner)
                .Include(s => s.Status)
                .Include(s => s.Createdby)
                .Include(s => s.Updatedby)
                .FirstOrDefaultAsync(m => m.SecAllocId == id);

            if (secondOwnerAlloc == null)
            {
                return NotFound();
            }

            return View(secondOwnerAlloc);
        }

        // GET: SecondOwnerAllocs/Create
        public IActionResult Create(string id, string ids, string ids2)
        {

            ViewBag.AllocId = id;
            ViewBag.LaptopCode = ids;
            ViewBag.Serial = ids2;
            return View();
        }

        // POST: SecondOwnerAllocs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SecAllocId,AllocId,SecLaptopCode,SerialNumber,SecOwnerCode,AgeYears,SecAllocationStatus,AllocCreated,DateCreated,AllocUpdated,DateUpdated")] SecondOwnerAlloc secondOwnerAlloc)
        {
            var ucode = HttpContext.Session.GetString("UserName");
            var findOwnerInBorrowed = await _context.tbl_ictams_ltborrowed.Where(x => x.OwnerID == secondOwnerAlloc.SecOwnerCode && x.StatusID == "AC").FirstOrDefaultAsync();

            if (findOwnerInBorrowed != null)
            {
                TempData["AlertMessage"] = "The selected owner had borrowed a laptop. Please return it first to reallocate!";
                return RedirectToAction("Create");
            }

            var findFirstOwner = await _context.tbl_ictams_laptopalloc.Where(x=>x.AllocId == secondOwnerAlloc.AllocId && x.AllocationStatus == "AC").FirstOrDefaultAsync();
            if(findFirstOwner != null)
            {
                findFirstOwner.AllocationStatus = "IN";
                findFirstOwner.AllocUpdated = ucode;
                findFirstOwner.DateUpdated = DateTime.Now;
            }
                var paramCode = await _context.tbl_ictams_parameters
                   .Where(p => p.parm_code == "ltas_id")
                   .MaxAsync(p => p.parm_value);
                var newparamCode = paramCode + 1;

                var param = await _context.tbl_ictams_parameters
                    .FirstOrDefaultAsync(p => p.parm_code == "ltas_id");
                param.parm_value = newparamCode;

                
                secondOwnerAlloc.SecAllocId = "LTAS" + newparamCode.ToString().PadLeft(11, '0');
                secondOwnerAlloc.SecAllocationStatus = "AC";
                secondOwnerAlloc.DateCreated = DateTime.Now;
                secondOwnerAlloc.AllocCreated = ucode;
                _context.Add(secondOwnerAlloc);
                await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully transfer to a  second owner!";
            // ...
            return RedirectToAction(nameof(Index));

        }

        // GET: SecondOwnerAllocs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.tbl_ictams_ltnewalloc == null)
            {
                return NotFound();
            }

            var secondOwnerAlloc = await _context.tbl_ictams_ltnewalloc.FindAsync(id);
            if (secondOwnerAlloc == null)
            {
                return NotFound();
            }
           
            return View(secondOwnerAlloc);
        }

        // POST: SecondOwnerAllocs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("SecAllocId,AllocId,SecLaptopCode,SecOwnerCode,DatePurchase,AgeYears,SecAllocationStatus,AllocCreated,DateCreated,AllocUpdated,DateUpdated")] SecondOwnerAlloc secondOwnerAlloc)
        {
            if (id != secondOwnerAlloc.SecAllocId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(secondOwnerAlloc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SecondOwnerAllocExists(secondOwnerAlloc.SecAllocId))
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
            return View(secondOwnerAlloc);
        }

        // GET: SecondOwnerAllocs/Delete/5
        //DELETE
        public async Task<IActionResult> DeleteAsEdit(string[] selectedIds)
        {
            //ViewData["UserStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_name");
            foreach (string id in selectedIds)
            {
                var ltsecownerID = await _context.tbl_ictams_ltnewalloc.FindAsync(id);
                if (ltsecownerID != null)
                {
                    // Check if the selected profile status exists in the tbl_lsm_status table
                    var status = await _context.tbl_ictams_status.FindAsync(ltsecownerID.SecAllocId);
                    if (status == null)
                    {
                        ModelState.AddModelError("", $"Profile status '{ltsecownerID.SecAllocId}' does not exist.");
                    }

                    // Find the corresponding LaptopCode in tbl_ictams_laptopinv and deduct 1 from AllocatedNo
                    var laptopCode = ltsecownerID.SecLaptopCode;
                    var laptopInv = await _context.tbl_ictams_laptopinv.FirstOrDefaultAsync(a => a.laptoptinvCode == laptopCode);
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
                var ltsecownerID = await _context.tbl_ictams_ltnewalloc.FindAsync(id);
                if (ltsecownerID != null)
                {
                    // Check if the selected profile status exists in the tbl_lsm_status table
                    var status = await _context.tbl_ictams_status.FindAsync(ltsecownerID.SecAllocId);
                    if (status == null)
                    {
                        ModelState.AddModelError("", $"Profile status '{ltsecownerID.SecAllocId}' does not exist.");
                    }

                    // Find the corresponding LaptopCode in tbl_ictams_laptopinv and deduct 1 from AllocatedNo
                    var laptopCode = ltsecownerID.SecLaptopCode;

                    // Find the corresponding LaptopCode in tbl_ictams_laptopinv
                    var laptopInv = await _context.tbl_ictams_laptopinv.FirstOrDefaultAsync(a => a.laptoptinvCode == laptopCode);

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
                            TempData["SuccessNotification"] = "Successfully retrieve a second owner allocation!  ";
                            // ...
                            return RedirectToAction(nameof(Index));
                        }
                    }


                }
            }
            return RedirectToAction(nameof(Index));
        }

        private bool SecondOwnerAllocExists(string id)
        {
          return (_context.tbl_ictams_ltnewalloc?.Any(e => e.SecAllocId == id)).GetValueOrDefault();
        }
    }
}
