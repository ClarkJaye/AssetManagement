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
    public class CapacitiesController : BaseController
    {
        private readonly AssetManagementContext _context;

        public CapacitiesController(AssetManagementContext context)
        : base(context) // Pass the 'context' dependency to the base constructor
        {
            _context = context;
        }

        // GET: Capacities
        public async Task<IActionResult> Index()
        {
            int? userProfile = HttpContext.Session.GetInt32("UserProfile");
            if (userProfile.HasValue)
            {

                var hasOpenAccess = await _context.tbl_ictams_profileaccess
          .AnyAsync(pa => pa.OpenAccess == "Y" &&
                          pa.Module.ModuleTitle == "Capacities" &&  // Adjust the module name as needed
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

                    var myData = HttpContext.Session.GetString("name");
                    var lSM_PNContext = _context.tbl_ictams_capacity.Where(Capacity => Capacity.CapacityStatus == "AC");
                    return View(await lSM_PNContext.ToListAsync());
                }
            }

            return RedirectToAction("Index", "Home");

            
        }

        // GET: Capacities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.tbl_ictams_capacity == null)
            {
                return NotFound();
            }

            var capacity = await _context.tbl_ictams_capacity
                .FirstOrDefaultAsync(m => m.CapacityId == id);
            if (capacity == null)
            {
                return NotFound();
            }

            return View(capacity);
        }
        public IActionResult InactiveCapacity()
        {
            var myData1 = HttpContext.Session.GetString("name");
            ViewBag.showprofile = myData1;
            var myData = HttpContext.Session.GetString("name");
            ViewBag.showprofile = myData;
            var inactiveCapacity = _context.tbl_ictams_capacity
                .Where(capacity => capacity.CapacityStatus == "IN")
                .ToList();
            return View(inactiveCapacity);
        }
        public async Task<IActionResult> Activate(int? id)
        {
            var userr = HttpContext.Session.GetString("name");
            var capacity = await _context.tbl_ictams_capacity.FindAsync(id);
            if (capacity == null)
            {
                return NotFound();
            }
            capacity.CapacityUpdatedBy = userr;
            capacity.CapacityUpdatedDate = DateTime.Now;
            capacity.CapacityStatus = "AC"; // Set the status to "Active"

            _context.Entry(capacity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully retrieve a deleted capacity!";
            // ...
            return RedirectToAction(nameof(Index));
        }

        // GET: Capacities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Capacities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CapacityId,CapacityDescription,CapacityStatus,CapacityCreatedBy,CapacityCreatedDate,CapacityUpdatedBy,CapacityUpdatedDate")] Capacity capacity)
        {
            var userrr = HttpContext.Session.GetString("name");
            bool descriptionExists = await _context.tbl_ictams_capacity.AnyAsync(x => x.CapacityDescription == capacity.CapacityDescription);
            if (descriptionExists)
            {
                TempData["ErrorMessage"] = "This description already exists. Please enter a different description.";
                return RedirectToAction(nameof(Index));
            }


            var paramCode = await _context.tbl_ictams_parameters.Where(p => p.parm_code == "cap_id").MaxAsync(p => p.parm_value);
            var newparamCode = paramCode + 1;

            var param = await _context.tbl_ictams_parameters.FirstOrDefaultAsync(p => p.parm_code == "cap_id");
            param.parm_value = newparamCode;


            capacity.CapacityDescription = capacity.CapacityDescription.ToUpper();
            capacity.CapacityStatus = "AC";
            capacity.CapacityId = newparamCode;
            capacity.CapacityCreatedDate = DateTime.Now;
            capacity.CapacityCreatedBy = userrr;
            _context.Add(capacity);
            await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully added a new capacity!";
            // ...
            return RedirectToAction(nameof(Index));
        }

        // GET: Capacities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.tbl_ictams_capacity == null)
            {
                return NotFound();
            }

            var capacity = await _context.tbl_ictams_capacity.FindAsync(id);
            if (capacity == null)
            {
                return NotFound();
            }
            return View(capacity);
        }

        // POST: Capacities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CapacityId,CapacityDescription,CapacityStatus,CapacityCreatedBy,CapacityCreatedDate,CapacityUpdatedBy,CapacityUpdatedDate")] Capacity capacity)
        {
            var userrr = HttpContext.Session.GetString("name");
            bool descriptionExists = await _context.tbl_ictams_capacity.AnyAsync(x => x.CapacityDescription == capacity.CapacityDescription);
            if (descriptionExists)
            {
                TempData["ErrorMessage"] = "This description already exists. Please enter a different description.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    capacity.CapacityDescription = capacity.CapacityDescription.ToUpper();
                    capacity.CapacityUpdatedBy = userrr;
                    capacity.CapacityUpdatedDate = DateTime.Now;
                    _context.Update(capacity);
                    await _context.SaveChangesAsync();
                    // ...
                    TempData["SuccessNotification"] = "Successfully edit a  capacity!";
                    // ...
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CapacityExists(capacity.CapacityId))
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
            return View(capacity);
        }


        // GET: Capacities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.tbl_ictams_capacity == null)
            {
                return NotFound();
            }

            var capacity = await _context.tbl_ictams_capacity
                .FirstOrDefaultAsync(m => m.CapacityId == id);
            if (capacity == null)
            {
                return NotFound();
            }

            return View(capacity);
        }

        // POST: Capacities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userrr = HttpContext.Session.GetString("name");
            if (_context.tbl_ictams_capacity == null)
            {
                return Problem("Entity set 'AssetManagementContext.Brand'  is null.");
            }
            var capacity = await _context.tbl_ictams_capacity.FindAsync(id);
            if (capacity != null)
            {
                capacity.CapacityStatus = "IN";
                capacity.CapacityUpdatedBy = userrr;
                capacity.CapacityUpdatedDate = DateTime.Now;
                _context.tbl_ictams_capacity.Update(capacity);
            }

            await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully delete a capacity!";
            // ...
            return RedirectToAction(nameof(Index));
        }

        private bool CapacityExists(int id)
        {
            return (_context.tbl_ictams_capacity?.Any(e => e.CapacityId == id)).GetValueOrDefault();
        }
    }
}
