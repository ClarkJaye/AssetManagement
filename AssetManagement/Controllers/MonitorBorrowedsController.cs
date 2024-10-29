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

namespace AssetManagement.Controllers
{
    public class MonitorBorrowedsController : BaseController
    {
        private readonly AssetManagementContext _context;

        public MonitorBorrowedsController(AssetManagementContext context)
            :base (context)
        {
            _context = context;
        }
        // GET: MonitorInventories
        public async Task<IActionResult> MonitorInvPartialView()
        {

            var assetManagementContext = _context.tbl_ictams_monitorinv
                .Where(x => x.MonitorStatus == "AV")
                .Include(l => l.Createdby)
                .Include(l => l.Model)
                .Include(l => l.Status)
                .Include(l => l.Updatedby);

            return View(await assetManagementContext.ToListAsync());

        }

        // GET: MonitorBorroweds
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
                          pa.Module.ModuleTitle == "MONITOR BORROWEDS" &&  // Adjust the module name as needed
                          pa.ProfileId == userProfile.Value);
                if (!hasOpenAccess)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var assetManagementContext = _context.tbl_ictams_monitorborrowed.Where(x => x.StatusID == "AC").Include(l => l.MonitorDetail.MonitorInventory).Include(l => l.MonitorDetail).Include(l => l.Owner).Include(l => l.Status).Include(l => l.UserCreated).Include(l => l.UserUpdated);
                    return View(await assetManagementContext.ToListAsync());
                }
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Inactive()
        {
            var assetManagementContext = _context.tbl_ictams_monitorborrowed.Where(x => x.StatusID == "RT").Include(l => l.MonitorDetail.MonitorInventory).Include(l => l.Owner).Include(l => l.Status).Include(l => l.UserCreated).Include(l => l.UserUpdated);
            return View(await assetManagementContext.ToListAsync());
        }
        public async Task<IActionResult> BorrowedViews()
        {
            var assetManagementContext = _context.tbl_ictams_monitorborrowed.Where(x => x.StatusID == "RT").Include(l => l.MonitorDetail.MonitorInventory).Include(l => l.Owner).Include(l => l.Status).Include(l => l.UserCreated).Include(l => l.UserUpdated);
            return View(await assetManagementContext.ToListAsync());
        }

        // GET: MonitorBorroweds/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null ||_context.tbl_ictams_monitorborrowed == null)
            {
                return NotFound();
            }

            var monitorBorrowed = await _context.tbl_ictams_monitorborrowed
                .Include(m => m.MonitorDetail)
                .Include(m => m.MonitorDetail.MonitorInventory)
                .Include(m => m.Owner)
                .Include(m => m.Status)
                .Include(m => m.UserCreated)
                .Include(m => m.UserUpdated)
                .FirstOrDefaultAsync(m => m.BorrowedID == id);
            if (monitorBorrowed == null)
            {
                return NotFound();
            }

            return View(monitorBorrowed);
        }

        // GET: MonitorBorroweds/Create
        public IActionResult Create()
        {
            ViewData["SerialNumber"] = new SelectList(_context.tbl_ictams_monitordetails, "SerialNumber", "SerialNumber");
            ViewData["UnitID"] = new SelectList(_context.tbl_ictams_monitorinv, "monitorCode", "monitorCode");
            ViewData["OwnerID"] = new SelectList(_context.tbl_ictams_owners, "OwnerCode", "OwnerCode");
            ViewData["StatusID"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_code");
            ViewData["CreatedBy"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode");
            ViewData["UpdatedBy"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode");
            return View();
        }

        // POST: MonitorBorroweds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BorrowedID,UnitID,SerialNumber,OwnerID,StatusID,DateBorrow,Remark,CreatedBy,DateCreated,UpdatedBy,DateUpdated")] MonitorBorrowed monitorBorrowed)
        {
            var findLaptop = await _context.tbl_ictams_monitorinv
                .Where(x => x.monitorCode == monitorBorrowed.UnitID)
                .FirstOrDefaultAsync();

            var findQuantity = findLaptop.Quantity - findLaptop.AllocatedNo;

            var FindQ = await _context.tbl_ictams_monitorborrowed
                .Where(z => z.StatusID == "AC")
                .CountAsync(x => x.UnitID == monitorBorrowed.UnitID);

            if (FindQ >= findQuantity)
            {
                TempData["AlertMessage"] = "Out of quantity!";
                return RedirectToAction("Create");
            }

            var findOwner = await _context.tbl_ictams_monitorborrowed.Where(x => x.OwnerID == monitorBorrowed.OwnerID && x.StatusID == "AC").FirstOrDefaultAsync();

            if (findOwner != null)
            {
                TempData["AlertMessage"] = "Owner is already exists!";
                return RedirectToAction("Create");
            }

            var paramCode = await _context.tbl_ictams_parameters
                    .Where(p => p.parm_code == "monbr_id")
                    .MaxAsync(p => p.parm_value);
            var newparamCode = paramCode + 1;

            var param = await _context.tbl_ictams_parameters
                .FirstOrDefaultAsync(p => p.parm_code == "monbr_id");
            param.parm_value = newparamCode;

            var ucode = HttpContext.Session.GetString("UserName");
            monitorBorrowed.BorrowedID = "MTB" + newparamCode.ToString().PadLeft(12, '0'); ;
            monitorBorrowed.DateBorrow = DateTime.Now;
            monitorBorrowed.CreatedBy = ucode;
            monitorBorrowed.Remark = monitorBorrowed.Remark.ToUpper();
            monitorBorrowed.DateCreated = DateTime.Now;
            monitorBorrowed.StatusID = "AC";

            _context.Add(monitorBorrowed);
            await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully borrow a monitor!";
            // ...
            return RedirectToAction(nameof(Index));
        }

        // GET: MonitorBorroweds/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.tbl_ictams_monitorborrowed == null)
            {
                return NotFound();
            }

            var laptopBorrowed = await _context.tbl_ictams_monitorborrowed.FindAsync(id);
            if (laptopBorrowed == null)
            {
                return NotFound();
            }

            return View(laptopBorrowed);
        }

        // POST: MonitorBorroweds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("BorrowedID,UnitID,SerialNumber,OwnerID,StatusID,DateBorrow,Remark,CreatedBy,DateCreated,UpdatedBy,DateUpdated")] MonitorBorrowed monitorBorrowed)
        {
            if (id != monitorBorrowed.BorrowedID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(monitorBorrowed);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MonitorBorrowedExists(monitorBorrowed.BorrowedID))
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
            return View(monitorBorrowed);
        }

        // GET: MonitorBorroweds/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.tbl_ictams_monitorborrowed == null)
            {
                return NotFound();
            }

            var monitorBorrowed = await _context.tbl_ictams_monitorborrowed
                .Include(m => m.MonitorDetail)
                .Include(m => m.MonitorDetail.MonitorInventory)
                .Include(m => m.Owner)
                .Include(m => m.Status)
                .Include(m => m.UserCreated)
                .Include(m => m.UserUpdated)
                .FirstOrDefaultAsync(m => m.BorrowedID == id);
            if (monitorBorrowed == null)
            {
                return NotFound();
            }

            return View(monitorBorrowed);
        }

        // POST: MonitorBorroweds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.tbl_ictams_monitorborrowed == null)
            {
                return Problem("Entity set 'AssetManagementContext.MonitorBorrowed'  is null.");
            }
            var ucode = HttpContext.Session.GetString("UserName");
            var laptopBorrowed = await _context.tbl_ictams_monitorborrowed.FindAsync(id);
            if (laptopBorrowed != null)
            {
                laptopBorrowed.DateUpdated = DateTime.Now;
                laptopBorrowed.UpdatedBy = ucode;
                laptopBorrowed.StatusID = "RT";
                _context.tbl_ictams_monitorborrowed.Update(laptopBorrowed);
            }

            await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully return a borrowed monitor!";
            // ...
            return RedirectToAction(nameof(Index));
        }

        private bool MonitorBorrowedExists(string id)
        {
          return (_context.tbl_ictams_monitorborrowed?.Any(e => e.BorrowedID == id)).GetValueOrDefault();
        }
    }
}
