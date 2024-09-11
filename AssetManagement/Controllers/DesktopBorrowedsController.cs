using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Data;
using AssetManagement.Utility;
using AssetManagement.Models;

namespace AssetManagement.Controllers
{
    public class DesktopBorrowedsController : BaseController
    {
        private readonly AssetManagementContext _context;

        public DesktopBorrowedsController(AssetManagementContext context)
            : base(context)
        {
            _context = context;
        }

        // GET: DesktopBorroweds
        public async Task<IActionResult> Index()
        {
            int? userProfile = HttpContext.Session.GetInt32("UserProfile");
            if (userProfile.HasValue)
            {

                var hasOpenAccess = await _context.tbl_ictams_profileaccess
          .AnyAsync(pa => pa.OpenAccess == "Y" &&
                          pa.Module.ModuleTitle == "LAPTOP BORROWEDS" &&  // Adjust the module name as needed
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

                    var assetManagementContext = _context.DesktopBorrowed.Where(x => x.StatusID=="AC").Include(d => d.DesktopInventory).Include(d => d.InventoryDetails).Include(d => d.Owner).Include(d => d.Status).Include(d => d.UserCreated).Include(d => d.UserUpdated);
                    return View(await assetManagementContext.ToListAsync());
                }
            }

            return RedirectToAction("Index", "Home");
            
        }

        public async Task<IActionResult> Inactive()
        {
        var assetManagementContext = _context.DesktopBorrowed.Where(x => x.StatusID=="RT").Include(d => d.DesktopInventory).Include(d => d.InventoryDetails).Include(d => d.Owner).Include(d => d.Status).Include(d => d.UserCreated).Include(d => d.UserUpdated);
        return View(await assetManagementContext.ToListAsync());
 

        }

        public async Task<IActionResult> DeskInvPartialView()
        {

            var assetManagementContext = _context.tbl_ictams_desktopinv
                .Where(x => x.DTStatus == "AV")
                .Include(l => l.Brand)
                .Include(l => l.CPU)
                .Include(l => l.Createdby)
                .Include(l => l.HardDisk)
                .Include(l => l.Level)
                .Include(l => l.Memory)
                .Include(l => l.GraphicsCard)
                .Include(l => l.Model)
                .Include(l => l.MainBoard)
                .Include(l => l.OS)
                .Include(l => l.Status)
                .Include(l => l.Updatedby);

            return View(await assetManagementContext.ToListAsync());

        }

        // GET: DesktopBorroweds/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.DesktopBorrowed == null)
            {
                return NotFound();
            }

            var desktopBorrowed = await _context.DesktopBorrowed
                .Include(d => d.DesktopInventory)
                .Include(d => d.InventoryDetails)
                .Include(d => d.Owner)
                .Include(d => d.Status)
                .Include(d => d.UserCreated)
                .Include(d => d.UserUpdated)
                .FirstOrDefaultAsync(m => m.BorrowedID == id);
            if (desktopBorrowed == null)
            {
                return NotFound();
            }

            return View(desktopBorrowed);
        }

        // GET: DesktopBorroweds/Create
        public IActionResult Create()
        {
            ViewData["UnitID"] = new SelectList(_context.tbl_ictams_desktopinv, "desktopInvCode", "desktopInvCode");
            ViewData["UnitTag"] = new SelectList(_context.tbl_ictams_desktopinvdetails, "unitTag", "unitTag");
            ViewData["OwnerID"] = new SelectList(_context.tbl_ictams_owners, "OwnerCode", "OwnerCode");
            ViewData["StatusID"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_code");
            ViewData["CreatedBy"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode");
            ViewData["RTUpdated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode");
            return View();
        }

        // POST: DesktopBorroweds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BorrowedID,UnitID,UnitTag,OwnerID,StatusID,DateBorrow,Remark,CreatedBy,DateCreated,RTUpdated,DateUpdated")] DesktopBorrowed desktopBorrowed)
        {
            var findLaptop = await _context.tbl_ictams_desktopinv
                .Where(x => x.desktopInvCode == desktopBorrowed.UnitID)
                .FirstOrDefaultAsync();

            var findQuantity = findLaptop.Quantity - findLaptop.AllocatedNo;

            var FindQ = await _context.tbl_ictams_dtborrowed
                .Where(z => z.StatusID == "AC")
                .CountAsync(x => x.UnitID == desktopBorrowed.UnitID);

            if (FindQ >= findQuantity)
            {
                TempData["AlertMessage"] = "Out of quantity!";
                return RedirectToAction("Create");
            }

            var findOwner = await _context.tbl_ictams_dtborrowed.Where(x => x.OwnerID == desktopBorrowed.OwnerID && x.StatusID == "AC").FirstOrDefaultAsync();

            if (findOwner != null)
            {
                TempData["AlertMessage"] = "Owner is already exists!";
                return RedirectToAction("Create");
            }

            var paramCode = await _context.tbl_ictams_parameters
                    .Where(p => p.parm_code == "dtbr_id")
                    .MaxAsync(p => p.parm_value);
            var newparamCode = paramCode + 1;

            var param = await _context.tbl_ictams_parameters
                .FirstOrDefaultAsync(p => p.parm_code == "dtbr_id");
            param.parm_value = newparamCode;

            var ucode = HttpContext.Session.GetString("UserName");
            desktopBorrowed.BorrowedID = "DTB" + newparamCode.ToString().PadLeft(12, '0'); ;
            desktopBorrowed.DateBorrow = DateTime.Now;
            desktopBorrowed.CreatedBy = ucode;
            desktopBorrowed.Remark = desktopBorrowed.Remark.ToUpper();
            desktopBorrowed.DateCreated = DateTime.Now;
            desktopBorrowed.StatusID = "AC";

            _context.Add(desktopBorrowed);
            await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully borrow a desktop!";
            // ...
            return RedirectToAction(nameof(Index));
        }

        // GET: DesktopBorroweds/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.DesktopBorrowed == null)
            {
                return NotFound();
            }

            var desktopBorrowed = await _context.DesktopBorrowed.FindAsync(id);
            if (desktopBorrowed == null)
            {
                return NotFound();
            }
            return View(desktopBorrowed);
        }

        // POST: DesktopBorroweds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("BorrowedID,UnitID,UnitTag,OwnerID,StatusID,DateBorrow,Remark,CreatedBy,DateCreated,RTUpdated,DateUpdated")] DesktopBorrowed desktopBorrowed)
        {
            if (id != desktopBorrowed.BorrowedID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(desktopBorrowed);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DesktopBorrowedExists(desktopBorrowed.BorrowedID))
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
            return View(desktopBorrowed);
        }

        // GET: DesktopBorroweds/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.DesktopBorrowed == null)
            {
                return NotFound();
            }

            var desktopBorrowed = await _context.DesktopBorrowed
                .Include(d => d.DesktopInventory)
                .Include(d => d.InventoryDetails)
                .Include(d => d.Owner)
                .Include(d => d.Status)
                .Include(d => d.UserCreated)
                .Include(d => d.UserUpdated)
                .FirstOrDefaultAsync(m => m.BorrowedID == id);
            if (desktopBorrowed == null)
            {
                return NotFound();
            }

            return View(desktopBorrowed);
        }

        // POST: DesktopBorroweds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.tbl_ictams_dtborrowed == null)
            {
                return Problem("Entity set 'AssetManagementContext.LaptopBorrowed'  is null.");
            }
            var ucode = HttpContext.Session.GetString("UserName");
            var desktopBorrowed = await _context.tbl_ictams_dtborrowed.FindAsync(id);
            if (desktopBorrowed != null)
            {
                desktopBorrowed.DateUpdated = DateTime.Now;
                desktopBorrowed.RTUpdated = ucode;
                desktopBorrowed.StatusID = "RT";
                _context.tbl_ictams_dtborrowed.Update(desktopBorrowed);
                await _context.SaveChangesAsync();
                // ...
                TempData["SuccessNotification"] = "Successfully return a borrowed Desktop!";
                // ...
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));


        }

        private bool DesktopBorrowedExists(string id)
        {
          return (_context.DesktopBorrowed?.Any(e => e.BorrowedID == id)).GetValueOrDefault();
        }
    }
}
