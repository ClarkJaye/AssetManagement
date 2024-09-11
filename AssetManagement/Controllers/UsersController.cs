using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Data;
using AssetManagement.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using AssetManagement.Utility;
using Microsoft.Extensions.Options;
using System.DirectoryServices.AccountManagement;

namespace AssetManagement.Controllers
{
    [AllowAnonymous]
    public class UsersController : BaseController
    {
        private readonly AssetManagementContext _context;
        private readonly LDAPSettings _ldapSettings;

        public UsersController(AssetManagementContext context, IOptions<LDAPSettings> ldapSettings)
            : base(context)
        {
            _context = context;
            _ldapSettings = ldapSettings.Value;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ADLogin(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return Json(new { success = false, message = "Please enter your AD Account credentials!" });
            }

            using (var context = new PrincipalContext(ContextType.Domain, null, _ldapSettings.Path))
            {
                try
                {
                    if (!context.ValidateCredentials(username, password))
                    {
                        return Json(new { success = false, message = "Invalid credentials. Please verify your username and password." });
                    }

                    var uppercaseUsername = username.ToUpper();
                    var userAD = await _context.tbl_ictams_users
                        .Include(u => u.Profile)
                        .FirstOrDefaultAsync(u => u.UserADLogin == uppercaseUsername && u.UserStatus == "AC");

                    if (userAD == null)
                    {
                        var inactiveUser = await _context.tbl_ictams_users
                            .FirstOrDefaultAsync(u => u.UserADLogin == uppercaseUsername && u.UserStatus == "IN");

                        if (inactiveUser != null)
                        {
                            return Json(new { success = false, message = "This account is deactivated by the admin." });
                        }
                        return Json(new { success = false, message = "You're not registered in this application!" });
                    }

                    // Populate dropdowns and session details
                    var openAccessModules = await _context.tbl_ictams_profileaccess
                        .Where(pa => pa.OpenAccess == "Y")
                        .Select(pa => new SelectListItem
                        {
                            Value = pa.ModuleId.ToString(),
                            Text = pa.Module.ModuleTitle
                        })
                        .ToListAsync();

                    var myStore = await _context.tbl_user_stores
                        .Include(u => u.Store)
                        .Include(u => u.User)
                        .Select(pa => new SelectListItem
                        {
                            Value = pa.StoreCode.ToString(),
                            Text = pa.Store.StoreName
                        })
                        .ToListAsync();

                    ViewBag.myStore = myStore;
                    ViewBag.OpenAccessModules = openAccessModules;

                    HttpContext.Session.SetInt32("UserProfile", (int)userAD.UserProfile);
                    HttpContext.Session.SetString("UserName", userAD.UserCode);
                    HttpContext.Session.SetString("name", userAD.UserFullName);
                    HttpContext.Session.SetString("profilename", userAD.Profile.ProfileName);

                    return Json(new { success = true, message = "Successfully signed in!", userName = userAD.UserFullName });
                }
                catch (Exception ex)
                {
                    // Log the error here if necessary
                    return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return Json(new { success = false, message = "Username and password are required!" });
            }

            var uppercaseUsername = username.ToUpper();

            // Fetch both active and inactive users in one query to minimize database calls
            var user = await _context.tbl_ictams_users.Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.UserCode == uppercaseUsername);

            if (user == null)
            {
                //return Json(new { success = false, message = "First time login? Obtain permission from the Admin!" });
                return Json(new { success = false, message = "Invalid credentials. Please verify your username and password." });
            }

            // Check if the password matches
            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(password.ToUpper(), user.UserPassword);

            if (!isPasswordCorrect)
            {
                return Json(new { success = false, message = "Username and password are incorrect!" });
            }

            // Check if user is inactive
            if (user.UserStatus == "IN")
            {
                return Json(new { success = false, message = "This account is deactivated by the admin!" });
            }

            // User is active and password is correct, proceed with login
            var openAccessModules = await _context.tbl_ictams_profileaccess
                .Where(pa => pa.OpenAccess == "Y")
                .Select(pa => new SelectListItem
                {
                    Value = pa.ModuleId.ToString(),
                    Text = pa.Module.ModuleTitle
                })
                .ToListAsync();

            var myStore = await _context.tbl_user_stores
                .Include(u => u.Store)
                .Include(u => u.User)
                .Select(pa => new SelectListItem
                {
                    Value = pa.StoreCode.ToString(),
                    Text = pa.Store.StoreName
                })
                .ToListAsync();

            ViewBag.myStore = myStore;
            ViewBag.OpenAccessModules = openAccessModules;

            // Setting session variables
            HttpContext.Session.SetInt32("UserProfile", (int)user.UserProfile);
            HttpContext.Session.SetString("UserName", user.UserCode);
            HttpContext.Session.SetString("name", user.UserFullName);
            HttpContext.Session.SetString("profilename", user.Profile.ProfileName);

            if (password == "1234")
            {
                TempData["AlertType"] = "success";
                TempData["SuccessMessage"] = "Login successful! Please change your password.";
                return RedirectToAction(nameof(ChangePassword));
            }

            TempData["UserName"] = user.UserFullName;
            return Json(new { success = true, message = "Successfully signed in!", userName = user.UserFullName });
        }


        // GET: Users
        public async Task<IActionResult> Index()
        {
            int? userProfile = HttpContext.Session.GetInt32("UserProfile");
            if (userProfile.HasValue)
            {
                var hasOpenAccess = await _context.tbl_ictams_profileaccess
          .AnyAsync(pa => pa.OpenAccess == "Y" &&
                          pa.Module.ModuleTitle == "Users" &&  // Adjust the module name as needed
                          pa.ProfileId == userProfile.Value);
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
                    var lSM_PNContext = _context.tbl_ictams_users.Where(p => p.Status.status_code == "AC").Include(Status => Status.Status).Include(Profile => Profile.Profile);
                    return View(await lSM_PNContext.ToListAsync());
                }
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Inactive()
        {
            var lSM_PNContext = _context.tbl_ictams_users.Where(p => p.Status.status_code == "IN").Include(Status => Status.Status).Include(Profile => Profile.Profile);
            return View(await lSM_PNContext.ToListAsync());
        }

        public async Task<IActionResult> UserViews()
        {
            var lSM_PNContext = _context.tbl_ictams_users.Where(p => p.Status.status_code == "IN").Include(Status => Status.Status).Include(Profile => Profile.Profile);
            return View(await lSM_PNContext.ToListAsync());
        }

        public IActionResult ResetPassword(string id)
        {
            // Check if the user exists with the given id
            var user = _context.tbl_ictams_users.FirstOrDefault(u => u.UserCode == id);
            if (user == null)
            {
                return NotFound();
            }
            // Reset the user's password to a default value (e.g., "1234")
            user.UserPassword = BCrypt.Net.BCrypt.HashPassword("1234");

            _context.SaveChanges();
            // Redirect back to the user list or any other desired page
            return RedirectToAction("Index");
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        public IActionResult UserChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string username, string newpassword, string conpassword)
        {
            if (ModelState.IsValid)
            {
                var ucode = HttpContext.Session.GetString("UserName");
                var UserAD = await _context.tbl_ictams_users.FirstOrDefaultAsync(u => u.UserCode == ucode);

                if (UserAD != null)
                {
                    if (newpassword == conpassword)
                    {
                        string uPass = BCrypt.Net.BCrypt.HashPassword(conpassword);
                        UserAD.UserPassword = uPass;
                        UserAD.UserUpdated = ucode;
                        UserAD.UserDateUpdated = DateTime.Now;
                        _context.Update(UserAD);
                        await _context.SaveChangesAsync();
                        TempData["AlertMessage1"] = "Successfully change the password, Login your account again.";
                        return RedirectToAction(nameof(Login));
                    }
                    else
                    {
                        TempData["AlertMessage1"] = "Password and confirm password don't match!";
                        return RedirectToAction(nameof(ChangePassword));
                    }
                }
                else
                {
                    TempData["AlertMessage"] = "Incorrect username";
                    return RedirectToAction(nameof(ChangePassword));
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UserChangePassword(string username, string newpassword, string conpassword, string curpassword)
        {
            if (ModelState.IsValid)
            {
                var UserAD = await _context.tbl_ictams_users.FirstOrDefaultAsync(u => u.UserCode == username);
                if (UserAD != null)
                {
                    string passwordHash = BCrypt.Net.BCrypt.HashPassword(curpassword);
                    var PasswordIsCorrect = BCrypt.Net.BCrypt.Verify(curpassword, UserAD.UserPassword);
                    if (PasswordIsCorrect)
                    {
                        if (newpassword == conpassword)
                        {
                            var ucode = HttpContext.Session.GetString("UserName");
                            string uPass = BCrypt.Net.BCrypt.HashPassword(conpassword);
                            UserAD.UserPassword = uPass;
                            UserAD.UserUpdated = ucode;
                            UserAD.UserDateUpdated = DateTime.Now;
                            _context.Update(UserAD);
                            await _context.SaveChangesAsync();
                            TempData["AlertMessage1"] = "Successfully change the password";
                            return RedirectToAction(nameof(Login));
                        }
                        else
                        {
                            TempData["AlertMessage1"] = "Password and confirm password don't match!";
                            return RedirectToAction(nameof(UserChangePassword));
                        }
                    }
                    else
                    {
                        TempData["AlertMessage2"] = "Old password is incorrect!";
                        return RedirectToAction(nameof(UserChangePassword));
                    }
                }
                else
                {
                    TempData["AlertMessage"] = "Incorrect username";
                    return RedirectToAction(nameof(UserChangePassword));
                }
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }

        public IActionResult Create()
        {
            ViewData["UserStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_name");
            ViewData["UserProfile"] = new SelectList(_context.tbl_ictams_profiles.Where(x => x.ProfileStatus == "AC"), "ProfileId", "ProfileName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserCode,UserPassword,UserADLogin,UserFullName,UserProfile,UserStatus,UserCreated,UserDateCreated,UserUpdated,UserDateUpdated")] User user)
        {
            if (user.UserProfile.Equals(0))
            {
                TempData["AlertMessage"] = "User profile cannot be null";
                return RedirectToAction(nameof(Create));
            }
            var findCode = await _context.tbl_ictams_users.Where(x => x.UserCode == user.UserCode).FirstOrDefaultAsync();
            if (findCode != null)
            {
                TempData["AlertMessage"] = "User code already exists!";
                return RedirectToAction(nameof(Create));
            }
            var findName = await _context.tbl_ictams_users.Where(x => x.UserCode == user.UserCode && x.UserFullName == user.UserFullName).FirstOrDefaultAsync();
            if (findName != null)
            {
                TempData["AlertMessage"] = "User code and full name already exists!";
                return RedirectToAction(nameof(Create));
            }
            var findFname = await _context.tbl_ictams_users.Where(x => x.UserFullName == user.UserFullName).FirstOrDefaultAsync();
            if (findFname != null)
            {
                TempData["AlertMessage"] = "Full name already exists!";
                return RedirectToAction(nameof(Create));
            }
            var findAD = await _context.tbl_ictams_users.Where(x => x.UserADLogin == user.UserADLogin).FirstOrDefaultAsync();
            if (findAD != null)
            {
                TempData["AlertMessage"] = "User adlogin already exists!";
                return RedirectToAction(nameof(Create));
            }
            var ucode = HttpContext.Session.GetString("UserName");
            user.UserCreated = ucode;
            user.UserDateCreated = DateTime.Now;
            string uPass = BCrypt.Net.BCrypt.HashPassword(user.UserPassword);
            user.UserPassword = uPass;
            _context.Add(user);
            await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully added a new user!";
            // ...
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.tbl_ictams_users == null)
            {
                return NotFound();
            }
            var user = await _context.tbl_ictams_users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["UserStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_code");
            ViewData["UserProfile"] = new SelectList(_context.tbl_ictams_profiles.Where(x => x.ProfileStatus == "AC"), "ProfileId", "ProfileName");
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, [Bind("UserCode,UserPassword,UserADLogin,UserFullName,UserProfile,UserStatus,UserCreated,UserDateCreated,UserUpdated,UserDateUpdated")] User user)
        {
            if (id != user.UserCode)
            {
                return NotFound();
            }
            try
            {
                var findUserExistName = await _context.tbl_ictams_users.Where(x => x.UserFullName == user.UserFullName).FirstOrDefaultAsync();
                var findUserExist = await _context.tbl_ictams_users.Where(x => x.UserCode == user.UserCode && x.Status.status_code == "AC" && x.UserFullName == user.UserFullName).FirstOrDefaultAsync();
                if (findUserExist != null)
                {
                    TempData["AlertMessage"] = "Duplicate Entry: User Full Name Already Present. The provided user full name already exists in the system. Please ensure each user's full name is unique.";
                    return RedirectToAction(nameof(Edit));
                }
                if (findUserExistName != null)
                {
                    TempData["AlertMessage"] = "Duplicate Entry: User Full Name Already Present. The provided user full name already exists in the system. Please ensure each user's full name is unique.";
                    return RedirectToAction(nameof(Edit));
                }
                var ucode = HttpContext.Session.GetString("UserName");
                user.UserUpdated = ucode;
                user.UserDateUpdated = DateTime.Now;
                _context.Update(user);
                await _context.SaveChangesAsync();
                // ...
                TempData["SuccessNotification"] = "Successfully update a  user!";
                // ...
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(user.UserCode))
                {

                }
                else
                {
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteAsEdit(string[] selectedIds)
        {
            ViewData["UserStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_name");
            foreach (string id in selectedIds)
            {
                var user = await _context.tbl_ictams_users.FindAsync(id);
                if (user != null)
                {
                    // Check if the selected profile status exists in the tbl_lsm_status table
                    var status = await _context.tbl_ictams_status.FindAsync(user.UserStatus);
                    if (status == null)
                    {
                        ModelState.AddModelError("", $"Profile status '{user.UserStatus}' does not exist.");
                    }
                    // Update the profile
                    var ucode = HttpContext.Session.GetString("UserName");
                    user.UserUpdated = ucode;
                    user.UserStatus = "IN";
                    user.UserDateUpdated = DateTime.Now;

                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    // ...
                    TempData["SuccessNotification"] = "Successfully delete a  user!";
                    // ...
                }
            }

            return RedirectToAction(nameof(Index));
        }

        //public async Task<IActionResult> Retrieve(string[] selectedIds)
        //{
        //    ViewData["UserStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_name");
        //    foreach (string id in selectedIds)
        //    {
        //        var profile = await _context.tbl_ictams_users.FindAsync(id);
        //        if (profile != null)
        //        {
        //            // Check if the selected profile status exists in the tbl_lsm_status table
        //            var status = await _context.tbl_ictams_status.FindAsync(profile.UserStatus);
        //            if (status == null)
        //            {
        //                ModelState.AddModelError("", $"Profile status '{profile.UserStatus}' does not exist.");
        //            }
        //            // Update the profile
        //            var ucode = HttpContext.Session.GetString("UserName");
        //            profile.UserUpdated = ucode;
        //            profile.UserStatus = "AC";
        //            profile.UserDateUpdated = DateTime.Now;

        //            _context.Update(profile);
        //            await _context.SaveChangesAsync();
        //            // ...
        //            TempData["SuccessNotification"] = "Successfully retrieve a  user!";
        //            // ...
        //        }
        //    }
        //    return RedirectToAction("Index");
        //}

        [HttpPost]
        public async Task<IActionResult> Retrieve(string userCode)
        {
            ViewData["UserStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_name");
            var profile = await _context.tbl_ictams_users.FindAsync(userCode);
            if (profile != null)
            {
                var status = await _context.tbl_ictams_status.FindAsync(profile.UserStatus);
                if (status == null)
                {
                    ModelState.AddModelError("", $"Profile status '{profile.UserStatus}' does not exist.");
                }
                else
                {
                    // Update the profile
                    var ucode = HttpContext.Session.GetString("UserName");
                    profile.UserUpdated = ucode;
                    profile.UserStatus = "AC";
                    profile.UserDateUpdated = DateTime.Now;

                    _context.Update(profile);
                    await _context.SaveChangesAsync();

                    TempData["SuccessNotification"] = "Successfully retrieved the user!";
                }
            }

            //return RedirectToAction("Index");

            return RedirectToAction("Inactive", "Users");
        }

        private bool UserExists(string id)
        {
            return (_context.tbl_ictams_users?.Any(e => e.UserCode == id)).GetValueOrDefault();
        }
    }
}
