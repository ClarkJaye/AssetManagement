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
    public class BorrowedPeripheralsController : BaseController
    {
        private readonly AssetManagementContext _context;

        public BorrowedPeripheralsController(AssetManagementContext context)
           : base(context) // Pass the 'context' dependency to the base constructor
        {
            _context = context;
        }


        public async Task<IActionResult> LTPeripPartialView()
        {
            var assetManagementContext = _context.tbl_ictams_ltperipheral
                .Where(lp => lp.PeripheralStatus == "AV")
                .Include(l => l.Brand)
                .Include(l => l.CreatedBy)
                .Include(l => l.DeviceType)
                .Include(l => l.Status)
                .Include(l => l.UpdatedBy)
                .Include(l => l.Vendor);

            return View(await assetManagementContext.ToListAsync());
        }


        // GET: BorrowedPeripherals
        public async Task<IActionResult> Index()
        {
            int? userProfile = HttpContext.Session.GetInt32("UserProfile");
            if (userProfile.HasValue)
            {

                var hasOpenAccess = await _context.tbl_ictams_profileaccess
          .AnyAsync(pa => pa.OpenAccess == "Y" &&
                          pa.Module.ModuleTitle == "Borrowed Peripherals" &&  // Adjust the module name as needed
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

                    var assetManagementContext = _context.tbl_ictams_ltpborrowed.Where(x => x.StatusID == "AC").Include(b => b.LaptopPeripheral).Include(b => b.Owner).Include(b => b.Status).Include(b => b.UserCreated).Include(b => b.UserUpdated);
                    return View(await assetManagementContext.ToListAsync());
                }
            }

            return RedirectToAction("Index", "Home");

            
        }

        public async Task<IActionResult> BorrowedPeripheralViews()
        {
            var assetManagementContext = _context.tbl_ictams_ltpborrowed.Where(x => x.StatusID == "RT").Include(b => b.LaptopPeripheral).Include(b => b.Owner).Include(b => b.Status).Include(b => b.UserCreated).Include(b => b.UserUpdated);
            return View(await assetManagementContext.ToListAsync());
        }

        // GET: BorrowedPeripherals/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.tbl_ictams_ltpborrowed == null)
            {
                return NotFound();
            }

            var borrowedPeripherals = await _context.tbl_ictams_ltpborrowed
                .Include(b => b.LaptopPeripheral)
                .Include(b => b.Owner)
                .Include(b => b.Status)
                .Include(b => b.UserCreated)
                .Include(b => b.UserUpdated)
                .FirstOrDefaultAsync(m => m.BorrowedID == id);
            if (borrowedPeripherals == null)
            {
                return NotFound();
            }

            return View(borrowedPeripherals);
        }

        // GET: BorrowedPeripherals/Create
        public IActionResult Create(string id)
        {
            ViewBag.Id = id;
            return View();
        }

        // POST: BorrowedPeripherals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BorrowedID,UnitID,OwnerID,StatusID,DateBorrow,Remark,CreatedBy,DateCreated,RTUpdated,DateUpdated")] BorrowedPeripherals borrowedPeripherals)
        {

            var findLaptop = await _context.tbl_ictams_ltperipheral
                .Where(x => x.PeripheralCode == borrowedPeripherals.UnitID)
                .FirstOrDefaultAsync();

            var findQuantity = findLaptop.PeripheralQty - findLaptop.PeripheralAllocation;

            var FindQ = await _context.tbl_ictams_ltpborrowed
                .Where(z => z.StatusID == "AC")
                .CountAsync(x => x.UnitID == borrowedPeripherals.UnitID);

            if (FindQ >= findQuantity)
            {
                TempData["AlertMessage"] = "Out of Quantity!";
                return RedirectToAction("Create");
            }

            var findOwner = await _context.tbl_ictams_ltpborrowed.Where(x => x.OwnerID == borrowedPeripherals.OwnerID && x.UnitID == borrowedPeripherals.UnitID && x.StatusID == "AC").FirstOrDefaultAsync();

            if (findOwner != null)
            {
                TempData["AlertMessage"] = "Owner and peripheral already exists!";
                return RedirectToAction("Create");
            }


            var paramCode = await _context.tbl_ictams_parameters
                    .Where(p => p.parm_code == "ltpbr_id")
                    .MaxAsync(p => p.parm_value);
            var newparamCode = paramCode + 1;

            var param = await _context.tbl_ictams_parameters
                .FirstOrDefaultAsync(p => p.parm_code == "ltpbr_id");
            param.parm_value = newparamCode;

            var ucode = HttpContext.Session.GetString("UserName");
            borrowedPeripherals.BorrowedID = "LTPB" + newparamCode.ToString().PadLeft(11, '0'); ;
            borrowedPeripherals.DateBorrow = DateTime.Now;
            borrowedPeripherals.CreatedBy = ucode;
            borrowedPeripherals.Remark = borrowedPeripherals.Remark.ToUpper();
            borrowedPeripherals.DateCreated = DateTime.Now;
            borrowedPeripherals.StatusID = "AC";

            _context.Add(borrowedPeripherals);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            
        }

        // GET: BorrowedPeripherals/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.tbl_ictams_ltpborrowed == null)
            {
                return NotFound();
            }

            var borrowedPeripherals = await _context.tbl_ictams_ltpborrowed.FindAsync(id);
            if (borrowedPeripherals == null)
            {
                return NotFound();
            }
            ViewData["UnitID"] = new SelectList(_context.tbl_ictams_laptopinv, "laptoptinvCode", "laptoptinvCode", borrowedPeripherals.UnitID);
            ViewData["OwnerID"] = new SelectList(_context.tbl_ictams_owners, "OwnerCode", "OwnerCode", borrowedPeripherals.OwnerID);
            ViewData["StatusID"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_code", borrowedPeripherals.StatusID);
            ViewData["CreatedBy"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode", borrowedPeripherals.CreatedBy);
            ViewData["RTUpdated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode", borrowedPeripherals.RTUpdated);
            return View(borrowedPeripherals);
        }

        // POST: BorrowedPeripherals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("BorrowedID,UnitID,OwnerID,StatusID,DateBorrow,Remark,CreatedBy,DateCreated,RTUpdated,DateUpdated")] BorrowedPeripherals borrowedPeripherals)
        {
            if (id != borrowedPeripherals.BorrowedID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(borrowedPeripherals);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BorrowedPeripheralsExists(borrowedPeripherals.BorrowedID))
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
            ViewData["UnitID"] = new SelectList(_context.tbl_ictams_laptopinv, "laptoptinvCode", "laptoptinvCode", borrowedPeripherals.UnitID);
            ViewData["OwnerID"] = new SelectList(_context.tbl_ictams_owners, "OwnerCode", "OwnerCode", borrowedPeripherals.OwnerID);
            ViewData["StatusID"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_code", borrowedPeripherals.StatusID);
            ViewData["CreatedBy"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode", borrowedPeripherals.CreatedBy);
            ViewData["RTUpdated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode", borrowedPeripherals.RTUpdated);
            return View(borrowedPeripherals);
        }

        // GET: BorrowedPeripherals/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.tbl_ictams_ltpborrowed == null)
            {
                return NotFound();
            }

            var borrowedPeripherals = await _context.tbl_ictams_ltpborrowed
                .Include(b => b.LaptopPeripheral)
                .Include(b => b.Owner)
                .Include(b => b.Status)
                .Include(b => b.UserCreated)
                .Include(b => b.UserUpdated)
                .FirstOrDefaultAsync(m => m.BorrowedID == id);
            if (borrowedPeripherals == null)
            {
                return NotFound();
            }

            return View(borrowedPeripherals);
        }

        // POST: BorrowedPeripherals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.tbl_ictams_ltpborrowed == null)
            {
                return Problem("Entity set 'AssetManagementContext.tbl_ictams_ltpborrowed'  is null.");
            }
            var borrowedPeripherals = await _context.tbl_ictams_ltpborrowed.FindAsync(id);
            if (borrowedPeripherals != null)
            {
                var ucode = HttpContext.Session.GetString("UserName");
                borrowedPeripherals.DateUpdated = DateTime.Now;
                borrowedPeripherals.RTUpdated = ucode;
                borrowedPeripherals.StatusID = "RT";
                _context.tbl_ictams_ltpborrowed.Update(borrowedPeripherals);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BorrowedPeripheralsExists(string id)
        {
          return (_context.tbl_ictams_ltpborrowed?.Any(e => e.BorrowedID == id)).GetValueOrDefault();
        }
    }
}
