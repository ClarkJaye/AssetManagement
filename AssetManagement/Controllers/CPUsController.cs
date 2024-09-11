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
    public class CPUsController : BaseController
    {
        private readonly AssetManagementContext _context;

        public CPUsController(AssetManagementContext context)
        : base(context) // Pass the 'context' dependency to the base constructor
        {
            _context = context;
        }

        public async Task<IActionResult> CPUPartialView()
        {
            var myData = HttpContext.Session.GetString("name");
            var lSM_PNContext = _context.tbl_ictams_cpu.Where(CPU => CPU.CPUStatus == "AC");
            return View(await lSM_PNContext.ToListAsync());
        }

        // GET: CPUs
        public async Task<IActionResult> Index()
        {
            int? userProfile = HttpContext.Session.GetInt32("UserProfile");
            if (userProfile.HasValue)
            {

                var hasOpenAccess = await _context.tbl_ictams_profileaccess
          .AnyAsync(pa => pa.OpenAccess == "Y" &&
                          pa.Module.ModuleTitle == "CPUS" &&  // Adjust the module name as needed
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
                    var lSM_PNContext = _context.tbl_ictams_cpu.Where(CPU => CPU.CPUStatus == "AC");
                    return View(await lSM_PNContext.ToListAsync());
                }
            }

            return RedirectToAction("Index", "Home");
           
        }

        // GET: CPUs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.tbl_ictams_cpu == null)
            {
                return NotFound();
            }

            var cPU = await _context.tbl_ictams_cpu
                .FirstOrDefaultAsync(m => m.CPUId == id);
            if (cPU == null)
            {
                return NotFound();
            }

            return View(cPU);
        }
        public IActionResult InactiveCPUs()
        {
            var myData1 = HttpContext.Session.GetString("name");
            ViewBag.showprofile = myData1;
            var myData = HttpContext.Session.GetString("name");
            ViewBag.showprofile = myData;
            var inactiveCPU = _context.tbl_ictams_cpu
                .Where(CPU => CPU.CPUStatus == "IN")
                .ToList();
            return View(inactiveCPU);
        }
        public async Task<IActionResult> Activate(int? id)
        {
            var userr = HttpContext.Session.GetString("name");
            var cpu = await _context.tbl_ictams_cpu.FindAsync(id);
            if (cpu == null)
            {
                return NotFound();
            }
            cpu.CPUUpdatedBy = userr;
            cpu.CPUUpdatedDate = DateTime.Now;
            cpu.CPUStatus = "AC"; // Set the status to "Active"

            _context.Entry(cpu).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // ...
            TempData["SuccessNotification"] = "Successfully retrieve a deleted cpu!";
            // ...
            return RedirectToAction(nameof(Index));
        }

        // GET: CPUs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CPUs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CPUId,CPUDescription,CPUStatus,CPUCreatedBy,CPUCreatedDate,CPUUpdatedBy,CPUUpdatedDate")] CPU cPU)
        {
            var userrr = HttpContext.Session.GetString("name");
            bool descriptionExists = await _context.tbl_ictams_cpu.AnyAsync(x => x.CPUDescription == cPU.CPUDescription);
            if (descriptionExists)
            {
                TempData["ErrorMessage"] = "Description already exists. Please enter a different description!";
                return RedirectToAction(nameof(Index));
            }

            var paramCode = await _context.tbl_ictams_parameters.Where(p => p.parm_code == "cpu_id").MaxAsync(p => p.parm_value);
                var newparamCode = paramCode + 1;

                var param = await _context.tbl_ictams_parameters.FirstOrDefaultAsync(p => p.parm_code == "cpu_id");
                param.parm_value = newparamCode;



                cPU.CPUDescription = cPU.CPUDescription.ToUpper();
                cPU.CPUStatus = "AC";
                cPU.CPUId = newparamCode;
                cPU.CPUCreatedDate = DateTime.Now;
                cPU.CPUCreatedBy = userrr;
                _context.Add(cPU);
                await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully added a new cpu!";
            // ...
            return RedirectToAction(nameof(Index));
        }

        // GET: CPUs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.tbl_ictams_cpu == null)
            {
                return NotFound();
            }

            var cPU = await _context.tbl_ictams_cpu.FindAsync(id);
            if (cPU == null)
            {
                return NotFound();
            }
            return View(cPU);
        }

        // POST: CPUs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CPUId,CPUDescription,CPUStatus,CPUCreatedBy,CPUCreatedDate,CPUUpdatedBy,CPUUpdatedDate")] CPU cPU)
        {
            var userrr = HttpContext.Session.GetString("name");
            bool descriptionExists = await _context.tbl_ictams_cpu.AnyAsync(x => x.CPUDescription == cPU.CPUDescription);
            if (descriptionExists)
            {
                TempData["ErrorMessage"] = "Description already exists. Please enter a different description!";
                return RedirectToAction(nameof(Index));
            }
            if (ModelState.IsValid)
            {
                try
                {
                    cPU.CPUDescription = cPU.CPUDescription.ToUpper();
                    cPU.CPUUpdatedBy = userrr;
                    cPU.CPUUpdatedDate = DateTime.Now;
                    _context.Update(cPU);
                    await _context.SaveChangesAsync();
                    // ...
                    TempData["SuccessNotification"] = "Successfully edit a cpu!";
                    // ...
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CPUExists(cPU.CPUId))
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
            return View(cPU);
        }

        // GET: CPUs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.tbl_ictams_cpu == null)
            {
                return NotFound();
            }

            var cPU = await _context.tbl_ictams_cpu
                .FirstOrDefaultAsync(m => m.CPUId == id);
            if (cPU == null)
            {
                return NotFound();
            }

            return View(cPU);
        }

        // POST: CPUs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var findUse = await _context.tbl_ictams_laptopinv.Where(x => x.LTcpu == id).FirstOrDefaultAsync();
            if (findUse != null)
            {
                TempData["AlertMessage"] = "Cannot be deleted. It is already in use!";
                return RedirectToAction(nameof(Index));
            }


            var userrr = HttpContext.Session.GetString("name");
            if (_context.tbl_ictams_cpu == null)
            {
                return Problem("Entity set 'AssetManagementContext.CPU'  is null.");
            }
            var cPU = await _context.tbl_ictams_cpu.FindAsync(id);
            if (cPU != null)
            {
                cPU.CPUStatus = "IN";
                cPU.CPUUpdatedBy = userrr;
                cPU.CPUUpdatedDate = DateTime.Now;
                _context.tbl_ictams_cpu.Update(cPU);
            }
            
            await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully delete a cpu!";
            // ...
            return RedirectToAction(nameof(Index));
        }

        private bool CPUExists(int id)
        {
          return (_context.tbl_ictams_cpu?.Any(e => e.CPUId == id)).GetValueOrDefault();
        }
    }
}
