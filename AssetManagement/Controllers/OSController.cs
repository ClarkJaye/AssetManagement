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
    public class OSController : BaseController
    {
        private readonly AssetManagementContext _context;

        public OSController(AssetManagementContext context)
        : base(context) // Pass the 'context' dependency to the base constructor
        {
            _context = context;
        }
        public async Task<IActionResult> OSPartialView()
        {   
            var myData = HttpContext.Session.GetString("name");
            var lSM_PNContext = _context.tbl_ictams_os.Where(Brand => Brand.OSStatus == "AC");
            return View(await lSM_PNContext.ToListAsync());
        }

        // GET: OS
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
                          pa.Module.ModuleTitle == "OS" &&  // Adjust the module name as needed
                          pa.ProfileId == userProfile.Value);
                if (!hasOpenAccess)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var myData = HttpContext.Session.GetString("name");
                    var lSM_PNContext = _context.tbl_ictams_os.Where(Brand => Brand.OSStatus == "AC");
                    return View(await lSM_PNContext.ToListAsync());
                }
            }

            return RedirectToAction("Index", "Home");

            
        }

        // GET: OS/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.tbl_ictams_os == null)
            {
                return NotFound();
            }

            var os = await _context.tbl_ictams_os
                .FirstOrDefaultAsync(m => m.OSId == id);
            if (os == null)
            {
                return NotFound();
            }

            return View(os);
        }
        public IActionResult InactiveOs()
        {
            var myData1 = HttpContext.Session.GetString("name");
            ViewBag.showprofile = myData1;
            var myData = HttpContext.Session.GetString("name");
            ViewBag.showprofile = myData;
            var inactiveOS = _context.tbl_ictams_os
                .Where(brand => brand.OSStatus == "IN")
                .ToList();
            return View(inactiveOS);
        }
        public async Task<IActionResult> Activate(int? id)
        {
            var userr = HttpContext.Session.GetString("name");
            var os = await _context.tbl_ictams_os.FindAsync(id);
            if (os == null)
            {
                return NotFound();
            }
            os.OSUpdatedBy = userr;
            os.OSUpdatedDate = DateTime.Now;
            os.OSStatus = "AC"; // Set the status to "Active"

            _context.Entry(os).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully retrieve a deleted laptop OS!";
            // ...
            return RedirectToAction(nameof(Index));
        }


        // GET: OS/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: OS/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OSId,OSDescription,OSStatus,OSCreatedBy,OSCreatedDate,OSUpdatedBy,OSUpdatedDate")] OS oS)
        {
            var userrr = HttpContext.Session.GetString("name");
            bool descriptionExists = await _context.tbl_ictams_os.AnyAsync(x => x.OSDescription == oS.OSDescription);
            if (descriptionExists)
            {
                TempData["ErrorMessage"] = "Description already exists. Please enter a different description!";
                return RedirectToAction(nameof(Index));
            }


            var paramCode = await _context.tbl_ictams_parameters.Where(p => p.parm_code == "os_id").MaxAsync(p => p.parm_value);
            var newparamCode = paramCode + 1;

            var param = await _context.tbl_ictams_parameters.FirstOrDefaultAsync(p => p.parm_code == "os_id");
            param.parm_value = newparamCode;

            oS.OSDescription = oS.OSDescription.ToUpper();
            oS.OSStatus = "AC";
            oS.OSId = newparamCode;
            oS.OSCreatedDate = DateTime.Now;
            oS.OSCreatedBy = userrr;
            _context.Add(oS);
            await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully added a new laptop OS!";
            // ...
            return RedirectToAction(nameof(Index));

        }

        // GET: OS/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.tbl_ictams_os == null)
            {
                return NotFound();
            }

            var oS = await _context.tbl_ictams_os.FindAsync(id);
            if (oS == null)
            {
                return NotFound();
            }
            return View(oS);
        }

        // POST: OS/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OSId,OSDescription,OSStatus,OSCreatedBy,OSCreatedDate,OSUpdatedBy,OSUpdatedDate")] OS oS)
        {
            var userrr = HttpContext.Session.GetString("name");
            bool descriptionExists = await _context.tbl_ictams_os.AnyAsync(x => x.OSDescription == oS.OSDescription);
            if (descriptionExists)
            {
                TempData["ErrorMessage"] = "Description already exists. Please enter a different description!";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    oS.OSDescription = oS.OSDescription.ToUpper();
                    oS.OSUpdatedBy = userrr;
                    oS.OSUpdatedDate = DateTime.Now;
                    _context.Update(oS);
                    await _context.SaveChangesAsync();
                    // ...
                    TempData["SuccessNotification"] = "Successfully edit a laptop OS!";
                    // ...
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OSExists(oS.OSId))
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
            return View(oS);
        }

        // GET: OS/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.tbl_ictams_os == null)
            {
                return NotFound();
            }

            var oS = await _context.tbl_ictams_os
                .FirstOrDefaultAsync(m => m.OSId == id);
            if (oS == null)
            {
                return NotFound();
            }

            return View(oS);
        }

        // POST: OS/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var findUse = await _context.tbl_ictams_laptopinv.Where(x => x.LTOS == id).FirstOrDefaultAsync();
            if (findUse != null)
            {
                TempData["AlertMessage"] = "Cannot be deleted. It is already in use!";
                return RedirectToAction(nameof(Index));
            }


            var userrr = HttpContext.Session.GetString("name");
            if (_context.tbl_ictams_os == null)
            {
                return Problem("Entity set 'AssetManagementContext.Brand'  is null.");
            }
            var oS = await _context.tbl_ictams_os.FindAsync(id);
            if (oS != null)
            {
                oS.OSStatus = "IN";
                oS.OSUpdatedBy = userrr;
                oS.OSUpdatedDate = DateTime.Now;
                _context.tbl_ictams_os.Update(oS);
            }

            await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully delete a laptop OS!";
            // ...
            return RedirectToAction(nameof(Index));
        }

        private bool OSExists(int id)
        {
            return (_context.tbl_ictams_os?.Any(e => e.OSId == id)).GetValueOrDefault();
        }
    }
}
