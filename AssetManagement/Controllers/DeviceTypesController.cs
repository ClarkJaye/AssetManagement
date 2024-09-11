using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Data;
using AssetManagement.Models;
using System.Drawing.Drawing2D;
using AssetManagement.Utility;

namespace AssetManagement.Controllers
{
    public class DeviceTypesController : BaseController
    {
        private readonly AssetManagementContext _context;

        public DeviceTypesController(AssetManagementContext context)
            :base(context)
        {
            _context = context;
        }

        // GET: DeviceTypes
        public async Task<IActionResult> Index()
     
        {
            int? userProfile = HttpContext.Session.GetInt32("UserProfile");
            if (userProfile.HasValue)
            {

                var hasOpenAccess = await _context.tbl_ictams_profileaccess
          .AnyAsync(pa => pa.OpenAccess == "Y" &&
                          pa.Module.ModuleTitle == "Device Types" &&  // Adjust the module name as needed
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

                    var myData = HttpContext.Session.GetString("UserName");
                    var lSM_PNContext = _context.tbl_ictams_devicetype.Where(DeviceType => DeviceType.DevtypeStatus == "AC");
                    return View(await lSM_PNContext.ToListAsync());
                }
            }

            return RedirectToAction("Index", "Home");

           
        }
        public async Task<IActionResult> DevicePartialView()
        {

            var lSM_PNContext = _context.tbl_ictams_devicetype.Where(DeviceType => DeviceType.DevtypeStatus == "AC");
            return View(await lSM_PNContext.ToListAsync());
        }

        // GET: DeviceTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.tbl_ictams_devicetype == null)
            {
                return NotFound();
            }

            var deviceType = await _context.tbl_ictams_devicetype
                .FirstOrDefaultAsync(m => m.DevtypeID == id);
            if (deviceType == null)
            {
                return NotFound();
            }

            return View(deviceType);
        }
        public IActionResult InactiveDevTypes()
        {
            var myData = HttpContext.Session.GetString("name");
            ViewBag.showprofile = myData;
            var InactiveDevTypes = _context.tbl_ictams_devicetype
                .Where(deviceType => deviceType.DevtypeStatus == "IN")
                .ToList();
            return View(InactiveDevTypes);
        }
        public async Task<IActionResult> Activate(int? id)
        {
            var userr = HttpContext.Session.GetString("UserName");
            var deviceType = await _context.tbl_ictams_devicetype.FindAsync(id);
            if (deviceType == null)
            {
                return NotFound();
            }
            deviceType.DevtypeUpdateby = userr;
            deviceType.DateUpdated = DateTime.Now;
            deviceType.DevtypeStatus = "AC"; // Set the status to "Active"

            _context.Entry(deviceType).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully retrieve a deleted device type!";
            // ...
            return RedirectToAction(nameof(Index));
        }

        // GET: DeviceTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DeviceTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DevtypeID,DevtypeDescription,DevtypeStatus,DevtypeCreatedby,DateCreated,DevtypeUpdateby,DateUpdated")] DeviceType deviceType)
        {
            var userrr = HttpContext.Session.GetString("UserName");

            var paramCode = await _context.tbl_ictams_parameters.Where(p => p.parm_code == "dev_id").MaxAsync(p => p.parm_value);
            var newparamCode = paramCode + 1;

            var param = await _context.tbl_ictams_parameters.FirstOrDefaultAsync(p => p.parm_code == "dev_id");
            param.parm_value = newparamCode;

            bool descriptionExists = await _context.tbl_ictams_devicetype.AnyAsync(x => x.DevtypeDescription == deviceType.DevtypeDescription);
            if (descriptionExists)
            {
                TempData["ErrorMessage"] = "Description already exists. Please enter a different description!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                deviceType.DevtypeDescription = deviceType.DevtypeDescription.ToUpper();
                deviceType.DevtypeStatus = "AC";
                deviceType.DevtypeID = newparamCode;
                deviceType.DateCreated = DateTime.Now;
                deviceType.DevtypeCreatedby = userrr;
                _context.Add(deviceType);
                await _context.SaveChangesAsync();
                // ...
                TempData["SuccessNotification"] = "Successfully added a new device type!";
                // ...
                return RedirectToAction(nameof(Index));
            }

            
            return RedirectToAction(nameof(Index));

        }

        // GET: DeviceTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.tbl_ictams_devicetype == null)
            {
                return NotFound();
            }

            var deviceType = await _context.tbl_ictams_devicetype.FindAsync(id);
            if (deviceType == null)
            {
                return NotFound();
            }
            return View(deviceType);
        }

        // POST: DeviceTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DevtypeID,DevtypeDescription,DevtypeStatus,DevtypeCreatedby,DateCreated,DevtypeUpdateby,DateUpdated")] DeviceType deviceType)
        {
            var userrr = HttpContext.Session.GetString("UserName");
            bool descriptionExists = await _context.tbl_ictams_devicetype.AnyAsync(x => x.DevtypeDescription == deviceType.DevtypeDescription);
            if (descriptionExists)
            {
                TempData["ErrorMessage"] = "Description already exists. Please enter a different description!";
                return RedirectToAction(nameof(Index));
            }


            try
                {
                deviceType.DevtypeDescription = deviceType.DevtypeDescription.ToUpper();
                    deviceType.DevtypeUpdateby = userrr;
                    deviceType.DateUpdated = DateTime.Now;
                    _context.Update(deviceType);
                    await _context.SaveChangesAsync();
                // ...
                TempData["SuccessNotification"] = "Successfully edit a device type!";
                // ...
                return RedirectToAction(nameof(Index));
            }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeviceTypeExists(deviceType.DevtypeID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            
            return View(deviceType);
        }

        // GET: DeviceTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.tbl_ictams_devicetype == null)
            {
                return NotFound();
            }

            var deviceType = await _context.tbl_ictams_devicetype
                .FirstOrDefaultAsync(m => m.DevtypeID == id);
            if (deviceType == null)
            {
                return NotFound();
            }

            return View(deviceType);
        }

        // POST: DeviceTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var findUse = await _context.tbl_ictams_ltperipheral .Where(x => x.PeripheralDevice == id).FirstOrDefaultAsync();
            if (findUse != null)
            {
                TempData["AlertMessage"] = "Cannot be deleted. It is already in use!";
                return RedirectToAction(nameof(Index));
            }


            var userrr = HttpContext.Session.GetString("UserName");
            if (_context.tbl_ictams_devicetype == null)
            {
                return Problem("Entity set 'AssetManagementContext.DeviceTypes'  is null.");
            }
            var deviceType = await _context.tbl_ictams_devicetype.FindAsync(id);
            if (deviceType != null)
            {
                deviceType.DevtypeStatus = "IN";
                deviceType.DevtypeUpdateby = userrr;
                deviceType.DateUpdated = DateTime.Now;
                _context.tbl_ictams_devicetype.Update(deviceType);
                // ...
                TempData["SuccessNotification"] = "Successfully delete a device type!";
                // ...
                return RedirectToAction(nameof(Index));
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DeviceTypeExists(int id)
        {
            return (_context.tbl_ictams_devicetype?.Any(e => e.DevtypeID == id)).GetValueOrDefault();
        }
    }
}

