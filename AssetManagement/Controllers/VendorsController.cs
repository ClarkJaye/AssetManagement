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
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AssetManagement.Controllers
{
    public class VendorsController : BaseController
    {
        private readonly AssetManagementContext _context;

        public VendorsController(AssetManagementContext context)
         : base(context)
        {
            _context = context;
        }

        // GET: Vendors  
        public async Task<IActionResult> VendorsPartialView()
        {
            var assetManagementContext = _context.tbl_ictams_vendor.Where(x => x.VendorStatus == "AC").Include(v => v.Createdby).Include(v => v.Status).Include(v => v.Updatedby);
            return View(await assetManagementContext.ToListAsync());
        }

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
                          pa.Module.ModuleTitle == "Vendors" &&  // Adjust the module name as needed
                          pa.ProfileId == userProfile.Value);
                if (!hasOpenAccess)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    await FindStatus();
                    var myData = HttpContext.Session.GetString("UserName");
                    var lSM_PNContext = _context.tbl_ictams_vendor.Where(Vendor => Vendor.VendorStatus == "AC").Include(x => x.Createdby).Include(x => x.Status);
                    return View(await lSM_PNContext.ToListAsync());
                }
            }

            return RedirectToAction("Index", "Home");

        }

        public async Task<IActionResult> VendorsViews()
        {
            await FindStatus();
            var myData = HttpContext.Session.GetString("UserName");
            var lSM_PNContext = _context.tbl_ictams_vendor.Where(Vendor => Vendor.VendorStatus == "IN").Include(x => x.Createdby).Include(x => x.Status);
            return View(await lSM_PNContext.ToListAsync());

        }

        // GET: Vendors/Details/5

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.tbl_ictams_vendor == null)
            {
                return NotFound();
            }

            var vendor = await _context.tbl_ictams_vendor
                .FirstOrDefaultAsync(m => m.VendorID == id);
            if (vendor == null)
            {
                return NotFound();
            }

            return View(vendor);
        }
        public IActionResult InactiveVendor()
        {
            var myData = HttpContext.Session.GetString("UserName");
            ViewBag.showprofile = myData;
            var InactiveDevTypes = _context.tbl_ictams_vendor
                .Where(vendor => vendor.VendorStatus == "IN")
                .ToList();
            return View(InactiveDevTypes);
        }
        public async Task<IActionResult> Activate(int? id)
        {
            var userr = HttpContext.Session.GetString("UserName");
            var vendor = await _context.tbl_ictams_vendor.FindAsync(id);
            if (vendor == null)
            {
                return NotFound();
            }
            vendor.VUpdateby = userr;
            vendor.DateUpdated = DateTime.Now;
            vendor.VendorStatus = "AC"; // Set the status to "Active"

            _context.Entry(vendor).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // GET: Vendors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Vendors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VendorID,VendorName,VendorAddress,VendorStatus,VCreatedby,DateCreated,VUpdateby,DateUpdated")] Vendor vendor)
        {

            var findName = await _context.tbl_ictams_vendor.Where(x => x.VendorName == vendor.VendorName && x.VendorAddress == vendor.VendorAddress).FirstOrDefaultAsync();
            if (findName != null)
            {
                TempData["AlertMessage"] = "Vendor name and address already exists!";
                return RedirectToAction(nameof(Index));
            }

            var userrr = HttpContext.Session.GetString("UserName");

            var paramCode = await _context.tbl_ictams_parameters.Where(p => p.parm_code == "vendor_id").MaxAsync(p => p.parm_value);
            var newparamCode = paramCode + 1;

            var param = await _context.tbl_ictams_parameters.FirstOrDefaultAsync(p => p.parm_code == "vendor_id");
            param.parm_value = newparamCode;

            await _context.SaveChangesAsync();

            vendor.VendorName = vendor.VendorName.ToUpper();
            vendor.VendorAddress = vendor.VendorAddress.ToUpper();
            vendor.VendorStatus = "AC";
            vendor.VendorID = newparamCode;
            vendor.DateCreated = DateTime.Now;
            vendor.VCreatedby = userrr;
            _context.Add(vendor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        // GET: Vendors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.tbl_ictams_vendor == null)
            {
                return NotFound();
            }

            var vendor = await _context.tbl_ictams_vendor.FindAsync(id);
            if (vendor == null)
            {
                return NotFound();
            }
            return View(vendor);
        }

        // POST: Vendors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VendorID,VendorName,VendorAddress,VendorStatus,VCreatedby,DateCreated,VUpdateby,DateUpdated")] Vendor vendor)
        {
            var userrr = HttpContext.Session.GetString("UserName");


            try
            {
                vendor.VendorName = vendor.VendorName.ToUpper();
                vendor.VendorAddress = vendor.VendorAddress.ToUpper();
                vendor.VUpdateby = userrr;
                vendor.DateUpdated = DateTime.Now;
                _context.Update(vendor);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VendorExists(vendor.VendorID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));

            return View(vendor);
        }

        // GET: Vendors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.tbl_ictams_vendor == null)
            {
                return NotFound();
            }

            var vendor = await _context.tbl_ictams_vendor
                .FirstOrDefaultAsync(m => m.VendorID == id);
            if (vendor == null)
            {
                return NotFound();
            }

            return View(vendor);
        }

        // POST: Vendors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userrr = HttpContext.Session.GetString("UserName");
            if (_context.tbl_ictams_vendor == null)
            {
                return Problem("Entity set 'AssetManagementContext.Vendor'  is null.");
            }
            var deviceType = await _context.tbl_ictams_vendor.FindAsync(id);
            if (deviceType != null)
            {
                deviceType.VendorStatus = "IN";
                deviceType.VUpdateby = userrr;
                deviceType.DateUpdated = DateTime.Now;
                _context.tbl_ictams_vendor.Update(deviceType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VendorExists(int id)
        {
            return (_context.tbl_ictams_vendor?.Any(e => e.VendorID == id)).GetValueOrDefault();
        }
    }
}

