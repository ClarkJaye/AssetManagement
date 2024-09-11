using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Data;
using AssetManagement.Models;
using AssetManagement.Models.View_Model;
using AssetManagement.Utility;

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

        public async Task<IActionResult> OwnerViews(string id, string searchString)
        {

            var assetManagementContext = _context.tbl_ictams_owners
             .Where(p => p.Status.status_code == "IN")
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
        public async Task<IActionResult> Index(string id, string searchString)
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

                    if (!string.IsNullOrEmpty(id))
                    {
                        var ownerCode = await _context.tbl_ictams_owners.FirstOrDefaultAsync(x => x.OwnerCode == id);

                        ownerViewModel.Owner = ownerCode;
                        ViewData["ad"] = 1;
                        return View(ownerViewModel);
                    }

                    if (!string.IsNullOrEmpty(searchString))
                    {
                        var assetManagement = _context.tbl_ictams_owners
                      .Where(p => p.Status.status_code == searchString)
                      .Include(o => o.Location)
                      .Include(o => o.Createdby).Include(o => o.Department).Include(o => o.Store)
                      .Include(o => o.Status).Include(o => o.Updatedby);
                        OwnerViewModel ownerViewModel2 = new()
                        {
                            OwnerList = await assetManagement.ToListAsync()
                        };
                        return View(ownerViewModel2);
                    }
                    ViewData["ad"] = 0;
                    return View(ownerViewModel);
                }
            }

            return RedirectToAction("Index", "Home");

        }


        public async Task<IActionResult> Cancel()
        {
            ViewData["ad"] = 0;
            return RedirectToAction(nameof(Index));
        }
        // CREATE UPDATE
        public async Task<IActionResult> CreateUpdate([Bind("OwnerCode,OwnerFullName,OwnerDept,OwnerLocation,OwnerOffice,OwnerStatus,OwnerCreated,DateCreated,Owner_updated,DateUpdated")] Owner owner, int idCheck)
        {
            if (idCheck > 0)
            {
                if(OwnerExists(owner))
                {
                    var findAlloc = await _context.tbl_ictams_laptopalloc
                        .Where(x => x.OwnerCode == owner.OwnerCode && x.Status.status_code == "AC").FirstOrDefaultAsync();
                    var findPerip = await _context.tbl_ictams_ltperipheralalloc
                        .Where(x => x.OwnerCode == owner.OwnerCode && x.Status.status_code == "AC").FirstOrDefaultAsync();
                    var ownderFind = await _context.tbl_ictams_owners.FirstOrDefaultAsync(x => x.OwnerCode == owner.OwnerCode);
                    var ownderFind2 = await _context.tbl_ictams_owners.FirstOrDefaultAsync(x => x.OwnerFullName == owner.OwnerFullName);
                    var ucode = HttpContext.Session.GetString("UserName");
                    if (findAlloc != null)
                    {
                        TempData["AlertMessage1"] = "This owner can't be edit!";
                        return RedirectToAction(nameof(Index));
                    }else if (findPerip != null)
                    {
                        TempData["AlertMessage1"] = "This owner can't be edit!";
                        return RedirectToAction(nameof(Index));
                    }
                    else if (ownderFind == null)
                    {
                        TempData["AlertMessage1"] = "Owner Code can't be edit!";
                        return RedirectToAction(nameof(Index));
                    }

                    else if (ownderFind.OwnerFullName == owner.OwnerFullName)
                    {
                        TempData["AlertMessage1"] = "Fullname already exists, Cannot be duplicated!";
                        return RedirectToAction(nameof(Index));
                    }
                    else if (ownderFind2!=null)
                    {
                        TempData["AlertMessage1"] = "Fullname already exists, Cannot be duplicated!";
                        return RedirectToAction(nameof(Index));
                    }

                    ownderFind.OwnerDept = owner.OwnerDept;
                    ownderFind.OwnerFullName = owner.OwnerFullName;
                    ownderFind.OwnerFullName = owner.OwnerFullName;
                    ownderFind.OwnerOffice = owner.OwnerOffice;
                    ownderFind.OwnerLocation = owner.OwnerLocation;
                    ownderFind.Owner_updated = ucode;
                    ownderFind.DateUpdated = DateTime.Now;

                    await _context.SaveChangesAsync();
                    // ...
                    TempData["SuccessNotification"] = "Successfully edit  new owner!";
                    // ...
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["AlertMessage1"] = "Owner code cannot be edit";
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                if (OwnerExists(owner))
                {
                    TempData["AlertMessage1"] = "Owner code or fullname already exists!";
                    return RedirectToAction(nameof(Index));
                }
                if (owner.OwnerCode == null)
                {
                    TempData["AlertMessage1"] = "Owner code `should be filled!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var ucode = HttpContext.Session.GetString("UserName");

                    owner.OwnerCreated = ucode;
                    owner.DateCreated = DateTime.Now;
                    _context.Add(owner);
                    await _context.SaveChangesAsync();
                    // ...
                    TempData["SuccessNotification"] = "Successfully added a new owner!";
                    // ...
                    return RedirectToAction(nameof(Index));
                }
                

            }
            return RedirectToAction(nameof(Index));
        }




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
                            TempData["AlertMessage1"] = "Owner  is already allocated with a device";
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
                            TempData["SuccessNotification"] = "Successfully delete a owner!";
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
                        TempData["SuccessNotification"] = "Successfully retrieve a deleted owner!";
                        // ...
                        return RedirectToAction(nameof(Index));
                    } 
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
