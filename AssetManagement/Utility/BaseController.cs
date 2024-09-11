using AssetManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Data;
using AssetManagement.Models;
using AssetManagement.Models.View_Model;

namespace AssetManagement.Utility
{
    public class BaseController : Controller
    {
        private readonly AssetManagementContext _context;

        public BaseController(AssetManagementContext context)
        {
            _context = context;
        }

        public override  void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var myData1 = HttpContext.Session.GetString("name");
            ViewBag.showprofile = myData1;
              var myData2 = HttpContext.Session.GetString("profilename");
            ViewBag.showprofilename = myData2;
            int? userProfile = HttpContext.Session.GetInt32("UserProfile");

            if (userProfile.HasValue)
            {
                ViewData["OpenAccessModules"] = new SelectList(_context.tbl_ictams_profileaccess
                    .Include(pa => pa.Module)
                    .Where(pa => pa.OpenAccess == "Y" && pa.Module.ModuleCategory == 1 && pa.ProfileId == userProfile.Value && pa.Module.ModuleStatus == "AC")
                    .Select(pa => new SelectListItem
                    {
                        Value = pa.Module.ModuleId.ToString(),
                        Text = pa.Module.ModuleTitle
                    })
                    .ToList(), "Value", "Text");
            }


            // CATEGORY 2
            if (userProfile.HasValue)
            {
                ViewData["OpenAccessModulesC2"] = new SelectList(_context.tbl_ictams_profileaccess
                    .Include(pa => pa.Module)
                    .Where(pa => pa.OpenAccess == "Y" && pa.Module.ModuleCategory == 2 && pa.ProfileId == userProfile.Value && pa.Module.ModuleStatus == "AC")
                    .Select(pa => new SelectListItem
                    {
                        Value = pa.Module.ModuleId.ToString(),
                        Text = pa.Module.ModuleTitle
                    })
                    .ToList(), "Value", "Text");
            }

            // CATEGORY 3
            if (userProfile.HasValue)
            {
                ViewData["OpenAccessModulesC3"] = new SelectList(_context.tbl_ictams_profileaccess
                    .Include(pa => pa.Module)
                    .Where(pa => pa.OpenAccess == "Y" && pa.Module.ModuleCategory == 3 && pa.ProfileId == userProfile.Value && pa.Module.ModuleStatus == "AC")
                    .Select(pa => new SelectListItem
                    {
                        Value = pa.Module.ModuleId.ToString(),
                        Text = pa.Module.ModuleTitle
                    })
                    .ToList(), "Value", "Text");
            }

            // CATEGORY 4
            if (userProfile.HasValue)
            {
                ViewData["OpenAccessModulesC4"] = new SelectList(_context.tbl_ictams_profileaccess
                    .Include(pa => pa.Module)
                    .Where(pa => pa.OpenAccess == "Y" && pa.Module.ModuleCategory == 4 && pa.ProfileId == userProfile.Value && pa.Module.ModuleStatus == "AC")
                    .Select(pa => new SelectListItem
                    {
                        Value = pa.Module.ModuleId.ToString(),
                        Text = pa.Module.ModuleTitle
                    })
                    .ToList(), "Value", "Text");
            }
			// CATEGORY LAPTOP
			if (userProfile.HasValue)
			{
				ViewData["OpenAccessModulesC5"] = new SelectList(_context.tbl_ictams_profileaccess
					.Include(pa => pa.Module)
					.Where(pa => pa.OpenAccess == "Y" && pa.Module.ModuleCategory == 5 && pa.ProfileId == userProfile.Value && pa.Module.ModuleStatus == "AC")
					.Select(pa => new SelectListItem
					{
						Value = pa.Module.ModuleId.ToString(),
						Text = pa.Module.ModuleTitle
					})
					.ToList(), "Value", "Text");
			}
			// CATEGORY PERIPHERAL
			if (userProfile.HasValue)
			{
				ViewData["OpenAccessModulesC6"] = new SelectList(_context.tbl_ictams_profileaccess
					.Include(pa => pa.Module)
					.Where(pa => pa.OpenAccess == "Y" && pa.Module.ModuleCategory == 6 && pa.ProfileId == userProfile.Value && pa.Module.ModuleStatus == "AC")
					.Select(pa => new SelectListItem
					{
						Value = pa.Module.ModuleId.ToString(),
						Text = pa.Module.ModuleTitle
					})
					.ToList(), "Value", "Text");
			}
			// CATEGORY DESKTOP
			if (userProfile.HasValue)
			{
				ViewData["OpenAccessModulesC7"] = new SelectList(_context.tbl_ictams_profileaccess
					.Include(pa => pa.Module)
					.Where(pa => pa.OpenAccess == "Y" && pa.Module.ModuleCategory == 7 && pa.ProfileId == userProfile.Value && pa.Module.ModuleStatus == "AC")
					.Select(pa => new SelectListItem
					{
						Value = pa.Module.ModuleId.ToString(),
						Text = pa.Module.ModuleTitle
					})
					.ToList(), "Value", "Text");
			}
			// CATEGORY MONITOR
			if (userProfile.HasValue)
			{
				ViewData["OpenAccessModulesC8"] = new SelectList(_context.tbl_ictams_profileaccess
					.Include(pa => pa.Module)
					.Where(pa => pa.OpenAccess == "Y" && pa.Module.ModuleCategory == 8 && pa.ProfileId == userProfile.Value && pa.Module.ModuleStatus == "AC")
					.Select(pa => new SelectListItem
					{
						Value = pa.Module.ModuleId.ToString(),
						Text = pa.Module.ModuleTitle
					})
					.ToList(), "Value", "Text");
			}

			var ucode = HttpContext.Session.GetString("UserName");
            if (ucode != null)
            {
                ViewData["myStore"] = new SelectList(_context.tbl_user_stores
                    .Include(pa => pa.Store)
                    .Where(pa => pa.UserCode == ucode )
                    .Select(pa => new SelectListItem
                    {
                        Value = pa.Store.Store_code.ToString(),
                        Text = pa.Store.StoreName
                    })
                    .ToList(), "Value", "Text");

                ViewData["myDept"] = new SelectList(_context.tbl_ictams_userdept
                  .Include(pa => pa.Department)
                  .Where(pa => pa.UserCode == ucode)
                  .Select(pa => new SelectListItem
                  {
                      Value = pa.Department.Dept_code.ToString(),
                      Text = pa.Department.Dept_name
                  })
                  .ToList(), "Value", "Text");
            }


            ViewBag.UserProfile = HttpContext.Session.GetInt32("UserProfile");
            ViewData["UserProfile"] = HttpContext.Session.GetInt32("UserProfile");

            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewData["UserName"] = HttpContext.Session.GetString("UserName");

            TempData["Welcome"] = "Welcome!";

        }
        protected async Task FindStatus()
        {
            var laptopsToUpdate = await _context.tbl_ictams_laptopinv
       .Where(x => x.Quantity == x.AllocatedNo && x.LTStatus == "AV")
       .ToListAsync();

            foreach (var laptop in laptopsToUpdate)
            {
                laptop.LTStatus = "CO";
            }

            var laptopsToUpdateAV = await _context.tbl_ictams_laptopinv
                .Where(x => x.Quantity != x.AllocatedNo && x.LTStatus == "CO")
                .ToListAsync();

            foreach (var laptop in laptopsToUpdateAV)
            {
                laptop.LTStatus = "AV";
            }

            var desktopToUpdate = await _context.tbl_ictams_desktopinv
       .Where(x => x.Quantity == x.AllocatedNo && x.DTStatus == "AV")
       .ToListAsync();

            foreach (var laptop in desktopToUpdate)
            {
                laptop.DTStatus = "CO";
            }

            var desktopToUpdateAV = await _context.tbl_ictams_desktopinv
       .Where(x => x.Quantity != x.AllocatedNo && x.DTStatus == "CO")
       .ToListAsync();

            foreach (var laptop in desktopToUpdateAV)
            {
                laptop.DTStatus = "AV";
            }

            await _context.SaveChangesAsync();

        }


        protected async Task FindStatusLTP()
        {
            var laptopsToUpdate = await _context.tbl_ictams_ltperipheral
       .Where(x => x.PeripheralQty == x.PeripheralAllocation && x.PeripheralStatus == "AV")
       .ToListAsync();

            foreach (var laptop in laptopsToUpdate)
            {
                laptop.PeripheralStatus = "CO";
            }

            var laptopsToUpdateAV = await _context.tbl_ictams_ltperipheral
                .Where(x => x.PeripheralQty != x.PeripheralAllocation && x.PeripheralStatus == "CO")
       .ToListAsync();

            foreach (var laptop in laptopsToUpdateAV)
            {
                laptop.PeripheralStatus = "AV";
            }

            await _context.SaveChangesAsync();

        }

        protected async Task FindDesktopAllocationCompleted()
        {
            ////6 YEARS DESKTOP
            //var ownerDesktop = await _context.tbl_ictams_desktopalloc.Where(x => x.DesktopInventoryDetail.PurchaseDate < DateTime.Now.AddYears(-6) && x.AllocationStatus == "AC" && x.DesktopInventoryDetail.DTStatus == "CO" && x.UnitTag == x.DesktopInventoryDetail.unitTag).ToListAsync();

            //foreach (var laptop in ownerDesktop)
            //{
            //    laptop.AllocationStatus = "CO";
            //}

            ////6 YEARS DESKTOP
            //var ownerDesktopNew = await _context.tbl_ictams_dtnewalloc.Where(x => x.InventoryDetails.PurchaseDate < DateTime.Now.AddYears(-6) && x.SecAllocationStatus == "AC" && x.InventoryDetails.DTStatus == "CO" && x.UnitTag == x.InventoryDetails.unitTag).ToListAsync();

            //foreach (var laptop in ownerDesktopNew)
            //{
            //    laptop.SecAllocationStatus = "CO";
            //}

            ////6 years Monitor
            //var ownerMonitor = await _context.tbl_ictams_monitoralloc.Where(x => x.MonitorDetails.PurchaseDate < DateTime.Now.AddYears(-6) && x.AllocationStatus == "AC" && x.MonitorDetails.MonitorStatus == "CO" && x.SerialNumber == x.MonitorDetails.SerialNumber).ToListAsync();

            //foreach (var laptop in ownerMonitor)
            //{
            //    laptop.AllocationStatus = "CO";
            //}

            ////6 years Monitor
            //var ownerMonitorNew = await _context.tbl_ictams_monitornewalloc.Where(x => x.MonitorDetail.PurchaseDate < DateTime.Now.AddYears(-6) && x.SecAllocationStatus == "AC" && x.MonitorDetail.MonitorStatus == "CO" && x.SerialNumber == x.MonitorDetail.SerialNumber).ToListAsync();

            //foreach (var laptop in ownerMonitorNew)
            //{
            //    laptop.SecAllocationStatus = "CO";
            //}


            //await _context.SaveChangesAsync();
        }

        protected async Task FindAllocationCompleted()
        {
            //var ownerLaptop = await _context.tbl_ictams_laptopalloc.Where(x => x.InventoryDetails.PurchaseDate < DateTime.Now.AddMonths(-48)).ToListAsync();

            //foreach(var laptop in ownerLaptop)
            //{
            //    laptop.AllocationStatus = "CO";
            //}
            ////Laptop Inventory Details Completed
            //var laptopInvDetails = await _context.tbl_ictams_laptopinvdetails.Where(x => x.PurchaseDate < DateTime.Now.AddMonths(-48)).ToListAsync();

            //foreach (var laptopInvD in laptopInvDetails)
            //{
            //    laptopInvD.LTStatus = "CO";
            //}

            ////Desktop Inventory Details Completed
            //var DesktopInvDetails = await _context.tbl_ictams_desktopinvdetails.Where(x => x.PurchaseDate < DateTime.Now.AddYears(-6)).ToListAsync();

            //foreach (var DeskInvD in DesktopInvDetails)
            //{
            //    DeskInvD.DTStatus = "CO";
            //}

            ////Monitor Inventory Details Completed
            //var MonitorInvDetails = await _context.tbl_ictams_monitordetails.Where(x => x.PurchaseDate < DateTime.Now.AddYears(-6)).ToListAsync();

            //foreach (var MonitorInvD in MonitorInvDetails)
            //{
            //    MonitorInvD.MonitorStatus = "CO";
            //}


            //var ownerLaptopSecond = await _context.tbl_ictams_ltnewalloc.Where(x => x.InventoryDetails.PurchaseDate < DateTime.Now.AddMonths(-48)).ToListAsync();

            //foreach (var laptop in ownerLaptopSecond)
            //{
            //    laptop.SecAllocationStatus = "CO";
            //}

            //await _context.SaveChangesAsync();
        }

        public async Task<IActionResult> DefaultPassword()
        {
            var ucode = HttpContext.Session.GetString("UserName");

            var findPass = await _context.tbl_ictams_users.Where(x=>x.UserCode == ucode).FirstOrDefaultAsync();

            var PasswordIsCorrect = BCrypt.Net.BCrypt.Verify("1234", findPass.UserPassword);
            if (PasswordIsCorrect)
            {
                // Show success alert using SweetAlert
                TempData["AlertType"] = "success";
                TempData["SuccessMessage"] = "Login successfully!";
                return RedirectToAction("ChangePassword", "Users");
            }
            return RedirectToAction("ChangePassword", "Users");
        }
    }
}
