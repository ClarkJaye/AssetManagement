using AssetManagement.Data;
using AssetManagement.Models;
using ExternalLogin.Extensions;
using ExternalLogin.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Controllers
{
    public class ExternalController : Controller
    {
        private readonly IExternalLoginService externalLoginService;
        private readonly AssetManagementContext context;

        public ExternalController(
            IExternalLoginService externalLoginService,
            AssetManagementContext context)
        {
            this.externalLoginService = externalLoginService;
            this.context = context;
        }


        [HttpGet]
        public async Task<IActionResult> Login(string token) // make sure to add the parameter in the endpoint
        {
            var ipaddress = HttpContext.IpAddress();
            var usercode = string.Empty;

            if (externalLoginService.TryValidateToken(token, ipaddress, out usercode))
                return await AuthenticateUser(usercode); // if validated, this is where you setup the user session 


            return Redirect(externalLoginService.PortalUrl);
        }

        public IActionResult Logout()
        {
            return Redirect(externalLoginService.PortalUrl);
        }

        // sample app user session setup 
        private async Task<IActionResult> AuthenticateUser(string usercode)
        {
            var uppercaseUsername = usercode.ToUpper();

            // Fetch both active and inactive users in one query to minimize database calls
            var user = await context.tbl_ictams_users.Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.UserCode == uppercaseUsername);


            if (user != null)
            {
                // Check if user is inactive
                if (user.UserStatus == "IN")
                {
                    return Json(new { success = false, message = "This account is deactivated by the admin!" });
                }

                // User is active and password is correct, proceed with login
                var openAccessModules = await context.tbl_ictams_profileaccess
                    .Where(pa => pa.OpenAccess == "Y")
                    .Select(pa => new SelectListItem
                    {
                        Value = pa.ModuleId.ToString(),
                        Text = pa.Module.ModuleTitle
                    })
                    .ToListAsync();

                var myStore = await context.tbl_user_stores
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

                TempData["UserName"] = user.UserFullName;

                return RedirectToAction("Index", "Home");
            }
            return Redirect(externalLoginService.PortalUrl);
        }



    }
}
