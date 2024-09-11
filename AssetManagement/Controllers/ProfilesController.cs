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
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using AssetManagement.Utility;

namespace AssetManagement.Controllers
{
    public class ProfilesController : BaseController
    {
        private readonly AssetManagementContext _context;

        public ProfilesController( AssetManagementContext context)
        : base(context) // Pass the 'context' dependency to the base constructor
        {
            _context = context;
        }


        // GET: Profiles
        public async Task<IActionResult> ProfilePartialView(int id)
        {
           
            ViewBag.UserProfile = HttpContext.Session.GetInt32("UserProfile");
            ViewData["UserProfile"] = HttpContext.Session.GetInt32("UserProfile");
            ViewData["ProfileCreated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode");
            ViewData["ProfileStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_name");
            ViewData["ProfileUpdated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode");
            var orifue = _context.tbl_ictams_profiles
                .Where(p => p.Status.status_code == "AC")
                .Include(p => p.CreatedBy)
                .Include(p => p.Status)
                .Include(p => p.UpdatedBy);

            ProfileViewModel profileViewModel = new()
            {
                ProfileList = await orifue.ToListAsync(),
                Profile = new()
                {
                    ProfileId = 0
                }

            };
            if (id > 0)
            {
                var ftable = await _context.tbl_ictams_profiles.FirstOrDefaultAsync(x => x.ProfileId == id);
                if (ftable == null)
                {
                    return NotFound("Profile id not found");
                }

                profileViewModel.Profile = ftable;

            }

            return View(profileViewModel);
        }


        public async Task<IActionResult> Cancel()
        {
            
            return RedirectToAction(nameof(Index));
        }
        // GET: Profiles
        public async Task<IActionResult> Index(int id)
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
                          pa.Module.ModuleTitle == "Profiles" &&  // Adjust the module name as needed
                          pa.ProfileId == userProfile.Value);
                if (!hasOpenAccess)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.UserProfile = HttpContext.Session.GetInt32("UserProfile");
                    ViewData["UserProfile"] = HttpContext.Session.GetInt32("UserProfile");
                    ViewData["ProfileCreated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode");
                    ViewData["ProfileStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_name");
                    ViewData["ProfileUpdated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode");
                    var orifue = _context.tbl_ictams_profiles
                        .Where(p => p.Status.status_code == "AC")
                        .Include(p => p.CreatedBy)
                        .Include(p => p.Status)
                        .Include(p => p.UpdatedBy);

                    ProfileViewModel profileViewModel = new()
                    {
                        ProfileList = await orifue.ToListAsync(),
                        Profile = new()
                        {
                            ProfileId = 0
                        }

                    };
                    if (id > 0)
                    {
                        var ftable = await _context.tbl_ictams_profiles.FirstOrDefaultAsync(x => x.ProfileId == id);
                        if (ftable == null)
                        {
                            return NotFound("Profile id not found");
                        }

                        profileViewModel.Profile = ftable;

                    }

                    return View(profileViewModel);
                }
            }

            return RedirectToAction("Index", "Home");

        }

        public async Task<IActionResult> ProfileViews(int id)
        {


            var orifue = _context.tbl_ictams_profiles
                .Where(p => p.Status.status_code == "IN")
                .Include(p => p.CreatedBy)
                .Include(p => p.Status)
                .Include(p => p.UpdatedBy);

            ProfileViewModel profileViewModel = new()
            {
                ProfileList = await orifue.ToListAsync(),
                Profile = new()
                {
                    ProfileId = 0
                }

            };
            return View(profileViewModel);
        }


        //INACTIVE TO BE RETRIEVE
        public async Task<IActionResult> InActive(int id)
        {
           

            var orifue = _context.tbl_ictams_profiles
                .Where(p => p.Status.status_code == "IN")
                .Include(p => p.CreatedBy)
                .Include(p => p.Status)
                .Include(p => p.UpdatedBy);

            ProfileViewModel profileViewModel = new()
            {
                ProfileList = await orifue.ToListAsync(),
                Profile = new()
                {
                    ProfileId = 0
                }

            };
            return View(profileViewModel);
        }

        public async Task<IActionResult> SetAccessProfile()
        {


            var orifue = _context.tbl_ictams_profiles
                .Where(p => p.Status.status_code == "AC")
                .Include(p => p.CreatedBy)
                .Include(p => p.Status)
                .Include(p => p.UpdatedBy);

            ProfileViewModel profileViewModel = new()
            {
                ProfileList = await orifue.ToListAsync(),
                Profile = new()
                {
                    ProfileId = 0
                }

            };
            return View(profileViewModel);
        }


        // CREATE UPDATE ROUTE
        public async Task<IActionResult> CreateUpdate([Bind("ProfileId,ProfileName,ProfileDescription,ProfileStatus,ProfileCreated,ProfileDtCreated")] Profile profile)
        {

            if (profile.ProfileId > 0)
            {

                if (!ProfileExists(profile.ProfileId))
                {
                    return NotFound("Profile id not found");
                }
                    try
                    {
					    var ftable = await _context.tbl_ictams_profiles.FirstOrDefaultAsync(x => x.ProfileId == profile.ProfileId);
					    var ucode = HttpContext.Session.GetString("UserName");
                        ftable.ProfileName = profile.ProfileName.ToUpper();
                        ftable.ProfileDescription = profile.ProfileDescription;
					    ftable.ProfileUpdated = ucode;
					    ftable.ProfileDtUpdated = DateTime.Now;
						//_context.Update(profile);
                        await _context.SaveChangesAsync();
                    // ...
                    TempData["SuccessNotification"] = "Successfully edit a  profile!";
                    // ...
                }
                catch (DbUpdateConcurrencyException)
                    {
                        if (!ProfileExists(profile.ProfileId))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }

            }
            else
            {
                var existName = await _context.tbl_ictams_profiles.AnyAsync(p=>p.ProfileName == profile.ProfileName);
                if (existName)
                {
                    TempData["ErrorMessage"] = "Profile name already exists!";
                    return RedirectToAction(nameof(Index));
                }
                if(profile.ProfileDescription == null || profile.ProfileName == null)
                {
                    ModelState.AddModelError("ProfileDescription", "Sorry but it's required");
                    TempData["ErrorMessage1"] = "Please input a name!";
                }
                else
                {
                    profile.ProfileId = 0;

                    var paramCode = await _context.tbl_ictams_parameters.Where(p => p.parm_code == "profile_id").MaxAsync(p => p.parm_value);
                    var newparamCode = paramCode + 1;

                    var param = await _context.tbl_ictams_parameters.FirstOrDefaultAsync(p => p.parm_code == "profile_id");
                    param.parm_value = newparamCode;
 

                    profile.ProfileId = newparamCode;
                    var ucode = HttpContext.Session.GetString("UserName");
                    profile.ProfileDtCreated = DateTime.Now;
                    profile.ProfileCreated = ucode;
                    _context.Add(profile);
                    await _context.SaveChangesAsync();
                    // ...
                    TempData["SuccessNotification"] = "Successfully added a new profile!";
                    // ...
                }


            }

            return RedirectToAction(nameof(Index));
        }

    

        // DELETE AS EDIT
 
        public async Task<IActionResult> DeleteAsEdit(int[] selectedIds)
        {
            ViewData["ProfileStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_name");
            foreach (int id in selectedIds)
            {      
                var profile = await _context.tbl_ictams_profiles.FindAsync(id);
                var profileExist = await _context.tbl_ictams_users.FirstOrDefaultAsync(x => x.UserProfile == id);
                if (profile != null)
                {
                    // Check if the selected profile status exists in the tbl_lsm_status table
                    var status = await _context.tbl_ictams_status.FindAsync(profile.ProfileStatus);
                    if (status == null)
                    {
                        ModelState.AddModelError("", $"Profile status '{profile.ProfileStatus}' does not exist.");
                    }
                    if(profileExist != null)
                    {
                        ModelState.AddModelError("UserProfile", "Profile is in used");
                        TempData["ErrorMessage"] = "Profile in use!";
                    }
                    else
                    {
                        // Update the profile
                        var ucode = HttpContext.Session.GetString("UserName");
                        profile.ProfileUpdated = ucode;
                        profile.ProfileStatus = "IN";
                        profile.ProfileDtUpdated = DateTime.Now;

                        _context.Update(profile);
                        await _context.SaveChangesAsync();
                        // ...
                        TempData["SuccessNotification"] = "Successfully delete a  profile!";
                        // ...
                    }



                }
                
            }

            return RedirectToAction("Index");
        }


        // Retrieve Deleted Profile
        [HttpPost]
        public async Task<IActionResult> Retrieve(int[] selectedIds)
        {
            ViewData["ProfileStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_name");
            foreach (int id in selectedIds)
            {
                var profile = await _context.tbl_ictams_profiles.FindAsync(id);
                if (profile != null)
                {
                    // Check if the selected profile status exists in the tbl_lsm_status table
                    var status = await _context.tbl_ictams_status.FindAsync(profile.ProfileStatus);
                    if (status == null)
                    {
                        ModelState.AddModelError("", $"Profile status '{profile.ProfileStatus}' does not exist.");
                    }
					var ucode = HttpContext.Session.GetString("UserName");

                    // Update the profile
                    profile.ProfileUpdated = ucode;
					profile.ProfileStatus = "AC";
                    profile.ProfileDtUpdated = DateTime.Now;

                    _context.Update(profile);
                    await _context.SaveChangesAsync();
                    // ...
                    TempData["SuccessNotification"] = "Successfully retrieve a deleted profile!";
                    // ...
                }
            }

            return RedirectToAction("Index");
        }



        private bool ProfileExists(int id)
        {
          return (_context.tbl_ictams_profiles?.Any(e => e.ProfileId == id)).GetValueOrDefault();
        }
    }
}
