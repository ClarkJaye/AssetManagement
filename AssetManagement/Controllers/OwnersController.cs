using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Data;
using AssetManagement.Models;
using AssetManagement.Models.View_Model;
using AssetManagement.Utility;
using Microsoft.IdentityModel.Tokens;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AssetManagement.Controllers
{
    public class OwnersController : BaseController
    {
        private readonly AssetManagementContext _context;

        public OwnersController(AssetManagementContext context)
        : base(context) // Pass the 'context' dependency to the base constructor
        {
            _context = context;
        }
        public async Task<IActionResult> OwnerPartialView()
        {   
            var assetManagementContext = _context.tbl_ictams_owners
             .Where(p => p.Status.status_code == "AC" && !_context.tbl_ictams_laptopalloc.Any(a => a.OwnerCode == p.OwnerCode && a.AllocationStatus == "AC"))
             .Where(p => p.Status.status_code == "AC" && !_context.tbl_ictams_ltnewalloc.Any(a => a.SecOwnerCode == p.OwnerCode && a.SecAllocationStatus == "AC"))
             .Include(o => o.Store)
             .Include(o => o.Location)
             .Include(o => o.Createdby)
             .Include(o => o.Department)
             .Include(o => o.Status)
             .Include(o => o.Updatedby);

            

            OwnerViewModel ownerViewModel = new()
            {
                OwnerList = await assetManagementContext.ToListAsync()
            };

            return View(ownerViewModel);
        }

        public async Task<IActionResult> DesktopOwnerPartialView()
        {
            var assetManagementContext = _context.tbl_ictams_owners
             .Where(p => p.Status.status_code == "AC" && !_context.tbl_ictams_desktopalloc.Any(a => a.OwnerCode == p.OwnerCode && a.AllocationStatus == "AC"))
             .Where(p => p.Status.status_code == "AC" && !_context.tbl_ictams_dtnewalloc.Any(a => a.SecOwnerCode == p.OwnerCode && a.SecAllocationStatus == "AC"))
             .Include(o => o.Location)
             .Include(o => o.Store)
             .Include(o => o.Createdby)
             .Include(o => o.Department)
             .Include(o => o.Status)
             .Include(o => o.Updatedby);

            OwnerViewModel ownerViewModel = new()
            {
                OwnerList = await assetManagementContext.ToListAsync()
            };

            return View(ownerViewModel);
        }

        public async Task<IActionResult> MonitorOwnerPartialView()
        {
            var assetManagementContext = _context.tbl_ictams_owners
             .Where(p => p.Status.status_code == "AC" && !_context.tbl_ictams_monitoralloc.Any(a => a.OwnerCode == p.OwnerCode && a.AllocationStatus == "AC"))
              .Where(p => p.Status.status_code == "AC" && !_context.tbl_ictams_monitornewalloc.Any(a => a.SecOwnerCode == p.OwnerCode && a.SecAllocationStatus == "AC"))
             .Include(o => o.Location)
             .Include(o => o.Store)
             .Include(o => o.Createdby)
             .Include(o => o.Department)
             .Include(o => o.Status)
             .Include(o => o.Updatedby);

            OwnerViewModel ownerViewModel = new()
            {
                OwnerList = await assetManagementContext.ToListAsync()
            };

            return View(ownerViewModel);
        }

        public async Task<IActionResult> OwnerViews()
        {

            //var assetManagementContext = _context.tbl_ictams_owners
            // .Where(p => p.Status.status_code == "IN")
            // .Include(o => o.Location)
            // .Include(o => o.Createdby)
            // .Include(o => o.Department)
            // .Include(o => o.Status)
            // .Include(o => o.Updatedby);

            var assetManagementContext = _context.tbl_ictams_owners
                        .Include(o => o.Location)
                        .Include(o => o.Createdby).Include(o => o.Department).Include(o => o.Store)
                        .Include(o => o.Status).Include(o => o.Updatedby);

            OwnerViewModel ownerViewModel = new()
            {
                OwnerList = await assetManagementContext.ToListAsync()
            };

            return View(ownerViewModel);
        }

        public async Task<IActionResult> OwnerPartialViewBorrowed(string id, string searchString)
        {

            var assetManagementContext = _context.tbl_ictams_owners
             .Where(p => p.Status.status_code == "AC" && !_context.tbl_ictams_laptopalloc.Any(a => a.OwnerCode == p.OwnerCode && a.AllocationStatus == "AC"))
             .Where(p => p.Status.status_code == "AC" && !_context.tbl_ictams_ltnewalloc.Any(a => a.SecOwnerCode == p.OwnerCode && a.SecAllocationStatus == "AC"))
             .Where(p => p.Status.status_code == "AC" && !_context.tbl_ictams_ltborrowed.Any(a => a.OwnerID == p.OwnerCode && a.Status.status_code == "AC"))
             .Include(o => o.Location)
             .Include(o => o.Createdby)
             .Include(o => o.Department)
             .Include(o => o.Status)
             .Include(o => o.Updatedby);

            OwnerViewModel ownerViewModel = new()
            {
                OwnerList = await assetManagementContext.ToListAsync()
            };

            return View(ownerViewModel);
        }

        public async Task<IActionResult> PeripheralOwner(string id, string searchString)
        {

            var assetManagementContext = _context.tbl_ictams_owners
             .Include(o => o.Location)
             .Include(o => o.Createdby)
             .Include(o => o.Department)
             .Include(o => o.Status)
             .Include(o => o.Updatedby);



            OwnerViewModel ownerViewModel = new()
            {
                OwnerList = await assetManagementContext.ToListAsync()
            };

            return View(ownerViewModel);
        }



        public async Task<IActionResult> LTPOwner(string id, string searchString)
        {

            var assetManagementContext = _context.tbl_ictams_owners
             .Where(p => p.Status.status_code == "AC")
             .Include(o => o.Location)
             .Include(o => o.Createdby)
             .Include(o => o.Department)
             .Include(o => o.Status)
             .Include(o => o.Updatedby);



            OwnerViewModel ownerViewModel = new()
            {
                OwnerList = await assetManagementContext.ToListAsync()
            };

            return View(ownerViewModel);
        }

        // GET: Owners
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
                          pa.Module.ModuleTitle == "Owners" &&  // Adjust the module name as needed
                          pa.ProfileId == userProfile.Value);
                if (!hasOpenAccess)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // VIEW DATA

                    ViewData["OwnerCreated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode");
                    ViewData["OwnerDept"] = new SelectList(_context.tbl_ictams_department, "Dept_code", "Dept_name");
                    ViewData["OwnerStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_name");
                    ViewData["Owner_updated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode");
                    ViewData["OwnerLocation"] = new SelectList(_context.tbl_ictams_location, "LocationCode", "Description");
                    ViewData["OwnerOffice"] = new SelectList(_context.tbl_ictams_stores, "Store_code", "StoreName");

                    var assetManagementContext = _context.tbl_ictams_owners
                        .Where(p => p.Status.status_code == "AC")
                        .Include(o => o.Location)
                        .Include(o => o.Createdby).Include(o => o.Department).Include(o => o.Store)
                        .Include(o => o.Status).Include(o => o.Updatedby);

                    OwnerViewModel ownerViewModel = new()
                    {
                        OwnerList = await assetManagementContext.ToListAsync()
                    };

                    ViewData["ad"] = 0;
                    return View(ownerViewModel);
                }
            }

            return RedirectToAction("Index", "Home");

        }

        [HttpGet]
        public async Task<IActionResult> EditOwner(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var owner = await _context.tbl_ictams_owners
                    .Include(o => o.Location)
                    .Include(o => o.Createdby)
                    .Include(o => o.Department)
                    .Include(o => o.Store)
                    .Include(o => o.Status)
                    .Include(o => o.Updatedby)
                    .FirstOrDefaultAsync(x => x.OwnerCode == id);

                if (owner == null)
                {
                    return Json(new { success = false, message = "Owner not found." });
                }       
                return Json(new { success = true, data = owner });
            }

            return Json(new { success = false, message = "Invalid request." });
        }

        [HttpGet]
        public async Task<IActionResult> InActive()
        {
            var assetManagement = _context.tbl_ictams_owners
                   .Where(p => p.Status.status_code == "IN")
                   .Include(o => o.Location)
                   .Include(o => o.Createdby).Include(o => o.Department).Include(o => o.Store)
                   .Include(o => o.Status).Include(o => o.Updatedby);
            OwnerViewModel ownerViewModel2 = new()
            {
                OwnerList = await assetManagement.ToListAsync()
            };
            return View(ownerViewModel2);
        }



        public IActionResult Cancel()
        {
            ViewData["ad"] = 0;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> CreateOwner([Bind("OwnerCode,OwnerFullName,OwnerDept,OwnerLocation,OwnerOffice,OwnerStatus,OwnerCreated,DateCreated,Owner_updated,DateUpdated")] Owner owner)
        {
            var ucode = HttpContext.Session.GetString("UserName");

            if (owner.OwnerCode == null || owner.OwnerFullName == null || owner.OwnerDept == null || owner.OwnerLocation == null || owner.OwnerOffice == null)
            {
                return Json(new { success = false, message = "Fill up all the required fields"});
            }
            if (_context.tbl_ictams_owners.Any(e => e.OwnerFullName == owner.OwnerFullName))
            {
                return Json(new { success = false, message = "Full name already exists. Cannot be duplicated!" });
            }

            owner.OwnerCreated = ucode;
            owner.DateCreated = DateTime.Now;

            _context.Add(owner);
            await _context.SaveChangesAsync();

            TempData["SuccessNotification"] = "New owner added successfully!";
            return Json(new { success = true });

        }


        [HttpPost]
        public async Task<IActionResult> UpdateOwner([Bind("OwnerCode,OwnerFullName,OwnerDept,OwnerLocation,OwnerOffice,OwnerStatus,OwnerCreated,DateCreated,Owner_updated,DateUpdated")] Owner owner)
        {
                if (owner.OwnerCode == null || owner.OwnerFullName == null)
                {
                    return Json(new { success = false, message = "Fill up all the required fields" });
                }

                var ucode = HttpContext.Session.GetString("UserName");

                var existingOwner = await _context.tbl_ictams_owners.FirstOrDefaultAsync(x => x.OwnerCode == owner.OwnerCode);
                if (existingOwner == null)
                {
                    return Json(new { success = false, message = "Owner code does not exist!" });
                }

                var duplicateNameOwner = await _context.tbl_ictams_owners
                    .FirstOrDefaultAsync(x => x.OwnerFullName == owner.OwnerFullName && x.OwnerCode != owner.OwnerCode);
                if (duplicateNameOwner != null)
                {
                    return Json(new { success = false, message = "Full name already exists. Cannot be duplicated!" });
                }

                // Check for active allocations preventing updates
                var activeAlloc = await _context.tbl_ictams_laptopalloc.FirstOrDefaultAsync(x => x.OwnerCode == owner.OwnerCode && x.Status.status_code == "AC");
                var activePerip = await _context.tbl_ictams_ltperipheralalloc.FirstOrDefaultAsync(x => x.OwnerCode == owner.OwnerCode && x.Status.status_code == "AC");

                if (activeAlloc != null || activePerip != null)
                {
                    return Json(new { success = false, message = "Owner cannot be edited due to active allocations!" });
                }

                // Update owner information
                existingOwner.OwnerDept = owner.OwnerDept;
                existingOwner.OwnerFullName = owner.OwnerFullName;
                existingOwner.OwnerOffice = owner.OwnerOffice;
                existingOwner.OwnerLocation = owner.OwnerLocation;
                existingOwner.Owner_updated = ucode;
                existingOwner.DateUpdated = DateTime.Now;

                await _context.SaveChangesAsync();
                TempData["SuccessNotification"] = "Owner updated successfully!";
                return Json(new { success = true, message = "Owner updated successfully!" });
            
        }

        //// CREATE UPDATE
        //[HttpPost]
        //public async Task<IActionResult> CreateUpdate([Bind("OwnerCode,OwnerFullName,OwnerDept,OwnerLocation,OwnerOffice,OwnerStatus,OwnerCreated,DateCreated,Owner_updated,DateUpdated")] Owner owner, int idCheck)
        //{
        //    if (string.IsNullOrEmpty(owner.OwnerCode) || string.IsNullOrEmpty(owner.OwnerFullName) || owner?.OwnerDept == null || owner?.OwnerLocation == null || string.IsNullOrEmpty(owner.OwnerOffice))
        //    {
        //        return Json(new { success = false, message = "Please ensure all required fields are not empty." });
        //    }

        //    var ucode = HttpContext.Session.GetString("UserName");

        //    if (idCheck > 0)
        //    {
        //        var existingOwner = await _context.tbl_ictams_owners.FirstOrDefaultAsync(x => x.OwnerCode == owner.OwnerCode);
        //        var existingOwnerByName = await _context.tbl_ictams_owners.FirstOrDefaultAsync(x => x.OwnerFullName == owner.OwnerFullName);

        //        if (existingOwner == null)
        //        {
        //            return Json(new { success = false, message = "Owner code does not exist!" });
        //        }

        //        if (existingOwnerByName != null && existingOwnerByName.OwnerFullName == owner.OwnerFullName)
        //        {
        //            return Json(new { success = false, message = "Full name already exists. Cannot be duplicated!" });
        //        }

        //        var activeAlloc = await _context.tbl_ictams_laptopalloc.FirstOrDefaultAsync(x => x.OwnerCode == owner.OwnerCode && x.Status.status_code == "AC");
        //        var activePerip = await _context.tbl_ictams_ltperipheralalloc.FirstOrDefaultAsync(x => x.OwnerCode == owner.OwnerCode && x.Status.status_code == "AC");

        //        if (activeAlloc != null || activePerip != null)
        //        {
        //            return Json(new { success = false, message = "Owner cannot be edited due to active allocations!" });
        //        }

        //        existingOwner.OwnerDept = owner.OwnerDept;
        //        existingOwner.OwnerFullName = owner.OwnerFullName;
        //        existingOwner.OwnerOffice = owner.OwnerOffice;
        //        existingOwner.OwnerLocation = owner.OwnerLocation;
        //        existingOwner.Owner_updated = ucode;
        //        existingOwner.DateUpdated = DateTime.Now;

        //        await _context.SaveChangesAsync();
        //        return Json(new { success = true, message = "Owner updated successfully!" });
        //    }
        //    else
        //    {
        //        if (_context.tbl_ictams_owners.Any(e => e.OwnerCode == owner.OwnerCode || e.OwnerFullName == owner.OwnerFullName))
        //        {
        //            return Json(new { success = false, message = "Owner code or full name already exists!" });
        //        }

        //        owner.OwnerCreated = ucode;
        //        owner.DateCreated = DateTime.Now;

        //        _context.Add(owner);
        //        await _context.SaveChangesAsync();

        //        return Json(new { success = true, message = "New owner added successfully!" });
        //    }
        //}


        public async Task<IActionResult> DeleteAsEdit(string[] selectedIds)
        {
            ViewData["UserStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_name");
            foreach (string id in selectedIds)
            {
                var owner = await _context.tbl_ictams_owners.FirstOrDefaultAsync(f => f.OwnerCode == id && f.OwnerStatus == "AC");
                var findLTA = await _context.tbl_ictams_laptopalloc.FirstOrDefaultAsync(x => x.OwnerCode == id);
                var ownerIN = await _context.tbl_ictams_owners.FirstOrDefaultAsync(f => f.OwnerCode == id && f.OwnerStatus == "IN");
                if (owner != null)
                {
                    var status = await _context.tbl_ictams_status.FindAsync(owner.OwnerStatus );

                    if (status != null)
                    {
                        if (findLTA != null)
                        {
                            TempData["AlertMessage"] = "Owner  is already allocated with a device";
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            // Update the profile
                            var ucode = HttpContext.Session.GetString("UserName");
                            owner.Owner_updated = ucode;
                            owner.OwnerStatus = "IN";
                            owner.DateUpdated = DateTime.Now;

                            _context.Update(owner);
                            await _context.SaveChangesAsync();
                            // ...
                            TempData["SuccessNotification"] = "Successfully InActive a owner!";
                            // ...
                            return RedirectToAction(nameof(Index));
                        }

                    }
                }
                if (ownerIN != null)
                {
                    var statusIN = await _context.tbl_ictams_status.FindAsync(ownerIN.OwnerStatus);
                    // Update the profile
                    if (statusIN != null)
                    {
                        var ucode = HttpContext.Session.GetString("UserName");
                        ownerIN.Owner_updated = ucode;
                        ownerIN.OwnerStatus = "AC";
                        ownerIN.DateUpdated = DateTime.Now;

                        _context.Update(ownerIN);
                        await _context.SaveChangesAsync();
                        // ...
                        TempData["SuccessNotification"] = "Successfully InActive a deleted owner!";
                        // ...
                        return RedirectToAction(nameof(Index));
                    } 
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Retrieve(string OwnerCode)
        {
            ViewData["UserStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_name");
            var ownerIN = await _context.tbl_ictams_owners.FirstOrDefaultAsync(f => f.OwnerCode == OwnerCode && f.OwnerStatus == "IN");
            if (ownerIN != null)
            {
                var statusIN = await _context.tbl_ictams_status.FindAsync(ownerIN.OwnerStatus);
                // Update the profile
                if (statusIN != null)
                {
                    var ucode = HttpContext.Session.GetString("UserName");
                    ownerIN.Owner_updated = ucode;
                    ownerIN.OwnerStatus = "AC";
                    ownerIN.DateUpdated = DateTime.Now;
                    _context.Update(ownerIN);
                    await _context.SaveChangesAsync();
                    TempData["SuccessNotification"] = "Successfully retrieve a InActive owner!";
                    return RedirectToAction(nameof(Index));
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool OwnerExists(Owner owner )
        {
          return (_context.tbl_ictams_owners?.Any(e => e.OwnerCode == owner.OwnerCode || e.OwnerFullName == owner.OwnerFullName)).GetValueOrDefault();
        }
    }
}
