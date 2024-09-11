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
    public class SecondOwnerPeripAllocsController : BaseController
    {
        private readonly AssetManagementContext _context;

        public SecondOwnerPeripAllocsController(AssetManagementContext context)
            : base(context)
        {
            _context = context;
        }
        public async Task<IActionResult> Inactive()
        {
            var assetManagementContext1 = await _context.tbl_ictams_ltpsecowner.Where(x => x.SecAllocationStatus == "IN").Include(s => s.CreatedBy).Include(s => s.LaptopPeripAlloc).Include(s => s.LaptopPeripheral).Include(s => s.Owner).Include(s => s.Status).Include(s => s.UpdatedBy).ToListAsync();
            return View(assetManagementContext1);
        }

        public async Task<IActionResult> SecondPeripheralViews()
        {
            var assetManagementContext1 = await _context.tbl_ictams_ltpsecowner.Where(x => x.SecAllocationStatus == "IN").Include(s => s.CreatedBy).Include(s => s.LaptopPeripAlloc).Include(s => s.LaptopPeripheral).Include(s => s.Owner).Include(s => s.Status).Include(s => s.UpdatedBy).ToListAsync();
            return View(assetManagementContext1);
        }

        // GET: SecondOwnerPeripAllocs
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
                          pa.Module.ModuleTitle == "Second Owner Perip Allocs" &&  // Adjust the module name as needed
                          pa.ProfileId == userProfile.Value);
                if (!hasOpenAccess)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var assetManagementContext = await _context.tbl_ictams_ltpsecowner.Where(x => x.SecAllocationStatus == "AC").Include(s => s.CreatedBy).Include(s => s.LaptopPeripAlloc).Include(s => s.LaptopPeripheral).Include(s => s.Owner).Include(s => s.Status).Include(s => s.UpdatedBy).ToListAsync();
                    return View(assetManagementContext);
                }
            }

            return RedirectToAction("Index", "Home");

        }

        public async Task<IActionResult> peripDetails(string userCODE)
        {
            if (!string.IsNullOrEmpty(userCODE))
            {

                var laptopPeripheral = await _context.tbl_ictams_ltperipheral
                .Include(l => l.Brand)
                .Include(l => l.CreatedBy)
                .Include(l => l.DeviceType)
                .Include(l => l.Status)
                .Include(l => l.UpdatedBy)
                .Include(l => l.Vendor)
                .FirstOrDefaultAsync(m => m.PeripheralDescription == userCODE);

                return View(laptopPeripheral);

            }

            return View();
        }

        // GET: SecondOwnerPeripAllocs/Create
        public IActionResult Create(string id, string ids, string id2)
        {

            ViewBag.PeripheralAllocationId = id;
            ViewBag.PeripheralCode = ids;
            ViewBag.DatePurchased = id2;
            return View();
        }

        // POST: SecondOwnerPeripAllocs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SecAllocId,AllocId,SecPeripheralCode,SecOwnerCode,DatePurchased,AgeYears,SecAllocationStatus,AllocCreated,DateCreated,AllocUpdated,DateUpdated")] SecondOwnerPeripAlloc secondOwnerPeripAlloc)
        {

            var findOWNERDevice = await _context.tbl_ictams_ltperipheralalloc.Where(x=>x.OwnerCode == secondOwnerPeripAlloc.SecOwnerCode && x.PeripheralAllocStatus == "AC").FirstOrDefaultAsync();

            if (findOWNERDevice != null)
            {
                TempData["AlertMessage"] = "The owner already has the selected device!";
                return RedirectToAction(nameof(Create));
            }

            var findFirstOwner = await _context.tbl_ictams_ltperipheralalloc.Where(x => x.PeripheralAllocationId == secondOwnerPeripAlloc.AllocId && x.PeripheralAllocStatus == "AC").FirstOrDefaultAsync();
            if (findFirstOwner != null)
            {
                findFirstOwner.PeripheralAllocStatus = "IN";
            }

            //var findOwnerInBorrowed = await _context.tbl_ictams_ltborrowed.Where(x => x.OwnerID == secondOwnerPeripAlloc.SecOwnerCode && x.StatusID == "AC").FirstOrDefaultAsync();

            //if (findOwnerInBorrowed != null)
            //{
            //    TempData["AlertMessage"] = "The selected owner had borrowed a peripheral. Please return it first to reallocate!";
            //    return RedirectToAction("Create");
            //}

            var paramCode = await _context.tbl_ictams_parameters
               .Where(p => p.parm_code == "ltpas_id")
               .MaxAsync(p => p.parm_value);
            var newparamCode = paramCode + 1;

            var param = await _context.tbl_ictams_parameters
                .FirstOrDefaultAsync(p => p.parm_code == "ltpas_id");
            param.parm_value = newparamCode;



            TimeSpan ageDuration = (TimeSpan)(DateTime.Now - secondOwnerPeripAlloc.DatePurchased);

            int totalMonths = (int)Math.Round(ageDuration.TotalDays / (365.25 / 12), 0);

            string ageDescription = $"{totalMonths} months";

            secondOwnerPeripAlloc.AgeYears = decimal.Parse($"{totalMonths / 12}.{totalMonths % 12:00}");

            var ucode = HttpContext.Session.GetString("UserName");
            secondOwnerPeripAlloc.SecAllocId = "LTPA" + newparamCode.ToString().PadLeft(11, '0');
            secondOwnerPeripAlloc.SecAllocationStatus = "AC";
            secondOwnerPeripAlloc.DateCreated = DateTime.Now;
            secondOwnerPeripAlloc.AllocCreated = ucode;
            _context.Add(secondOwnerPeripAlloc);
                await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully transfer to a  second owner!";
            // ...
            return RedirectToAction(nameof(Index));
      
        }

        // GET: SecondOwnerPeripAllocs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null ||  _context.tbl_ictams_ltpsecowner == null)
            {
                return NotFound();
            }

            var secondOwnerPeripAlloc = await _context.tbl_ictams_ltpsecowner.FindAsync(id);
            if (secondOwnerPeripAlloc == null)
            {
                return NotFound();
            }
            return View(secondOwnerPeripAlloc);
        }

        // POST: SecondOwnerPeripAllocs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("SecAllocId,AllocId,SecPeripheralCode,SecOwnerCode,DatePurchased,AgeYears,SecAllocationStatus,AllocCreated,DateCreated,AllocUpdated,DateUpdated")] SecondOwnerPeripAlloc secondOwnerPeripAlloc)
        {
            if (id != secondOwnerPeripAlloc.SecAllocId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(secondOwnerPeripAlloc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SecondOwnerPeripAllocExists(secondOwnerPeripAlloc.SecAllocId))
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
            return View(secondOwnerPeripAlloc);
        }

        // GET: SecondOwnerPeripAllocs/Delete/5
        //DELETE
        public async Task<IActionResult> DeleteAsEdit(string[] selectedIds)
        {
            //ViewData["UserStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_name");
            foreach (string id in selectedIds)
            {
                var ltsecownerID = await _context.tbl_ictams_ltpsecowner.FindAsync(id);
                if (ltsecownerID != null)
                {
                    // Check if the selected profile status exists in the tbl_lsm_status table
                    var status = await _context.tbl_ictams_status.FindAsync(ltsecownerID.SecAllocId);
                    if (status == null)
                    {
                        ModelState.AddModelError("", $"Profile status '{ltsecownerID.SecAllocId}' does not exist.");
                    }

                    // Find the corresponding LaptopCode in tbl_ictams_laptopinv and deduct 1 from AllocatedNo
                    var laptopCode = ltsecownerID.SecPeripheralCode;
                    var laptopInv = await _context.tbl_ictams_ltperipheral.FirstOrDefaultAsync(a => a.PeripheralCode == laptopCode);
                    if (laptopInv != null)
                    {
                        laptopInv.PeripheralAllocation -= 1;
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
                var ltsecownerID = await _context.tbl_ictams_ltpsecowner.FindAsync(id);
                if (ltsecownerID != null)
                {
                    // Check if the selected profile status exists in the tbl_lsm_status table
                    var status = await _context.tbl_ictams_status.FindAsync(ltsecownerID.SecAllocId);
                    if (status == null)
                    {
                        ModelState.AddModelError("", $"Profile status '{ltsecownerID.SecAllocId}' does not exist.");
                    }

                    // Find the corresponding LaptopCode in tbl_ictams_laptopinv and deduct 1 from AllocatedNo
                    var laptopCode = ltsecownerID.SecPeripheralCode;

                    // Find the corresponding LaptopCode in tbl_ictams_laptopinv
                    var laptopInv = await _context.tbl_ictams_ltperipheral.FirstOrDefaultAsync(a => a.PeripheralCode == laptopCode);

                    if (laptopInv != null)
                    {
                        // Check if the Quantity and AllocatedNo are equal
                        if (laptopInv.PeripheralQty == laptopInv.PeripheralAllocation)
                        {
                            TempData["AlertMessage"] = "Cannot retrieve data. It is already full!";
                            return RedirectToAction("Inactive");
                        }
                        else
                        {
                            // Deduct 1 from AllocatedNo
                            laptopInv.PeripheralAllocation += 1;
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
        private bool SecondOwnerPeripAllocExists(string id)
        {
          return (_context.tbl_ictams_ltpsecowner?.Any(e => e.SecAllocId == id)).GetValueOrDefault();
        }
    }
}
