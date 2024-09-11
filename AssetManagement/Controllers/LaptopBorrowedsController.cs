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
using System.Drawing.Drawing2D;

namespace AssetManagement.Controllers
{
    public class LaptopBorrowedsController : BaseController
    {
        private readonly AssetManagementContext _context;

        public LaptopBorrowedsController(AssetManagementContext context)
            : base(context) // Pass the 'context' dependency to the base constructor
        {
            _context = context;
        }


        // GET: LaptopInventories
        public async Task<IActionResult> LapInvPartialView()
        {

            var assetManagementContext = _context.tbl_ictams_laptopinv
                .Where(x => x.LTStatus == "AV")
                .Include(l => l.Brand)
                .Include(l => l.CPU)
                .Include(l => l.Createdby)
                .Include(l => l.HardDisk)
                .Include(l => l.Level)
                .Include(l => l.Memory)
                .ThenInclude(l => l.Capacity)
                .Include(l => l.Model)
                .Include(l => l.OS)
                .Include(l => l.Status)
                .Include(l => l.Updatedby);

            return View(await assetManagementContext.ToListAsync());

        }

        // GET: LaptopBorroweds
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
                          pa.Module.ModuleTitle == "LAPTOP BORROWEDS" &&  // Adjust the module name as needed
                          pa.ProfileId == userProfile.Value);
                if (!hasOpenAccess)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var assetManagementContext = _context.tbl_ictams_ltborrowed.Where(x => x.StatusID == "AC").Include(l => l.LaptopInventory).Include(l => l.Owner).Include(l => l.Status).Include(l => l.UserCreated).Include(l => l.UserUpdated);
                    return View(await assetManagementContext.ToListAsync());
                }
            }

            return RedirectToAction("Index", "Home");

        }


        public async Task<IActionResult> Inactive()
        {
            var assetManagementContext = _context.tbl_ictams_ltborrowed.Where(x => x.StatusID == "RT").Include(l => l.LaptopInventory).Include(l => l.Owner).Include(l => l.Status).Include(l => l.UserCreated).Include(l => l.UserUpdated);
            return View(await assetManagementContext.ToListAsync());
        }

        public async Task<IActionResult> BorrowedViews()
        {
            var assetManagementContext = _context.tbl_ictams_ltborrowed.Where(x => x.StatusID == "RT").Include(l => l.LaptopInventory).Include(l => l.Owner).Include(l => l.Status).Include(l => l.UserCreated).Include(l => l.UserUpdated);
            return View(await assetManagementContext.ToListAsync());
        }

        // GET: LaptopBorroweds/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.tbl_ictams_ltborrowed == null)
            {
                return NotFound();
            }

            var laptopBorrowed = await _context.tbl_ictams_ltborrowed
                .Include(l => l.LaptopInventory)
                .Include(l => l.Owner)
                .Include(l => l.Status)
                .Include(l => l.UserCreated)
                .Include(l => l.UserUpdated)
                .FirstOrDefaultAsync(m => m.BorrowedID == id);
            if (laptopBorrowed == null)
            {
                return NotFound();
            }

            return View(laptopBorrowed);
        }

        // GET: LaptopBorroweds/Create
        public IActionResult Create()
        {
            //ViewBag.Id = id;
            //return View();
            ViewData["SerialNumber"] = new SelectList(_context.tbl_ictams_laptopinvdetails, "SerialCode", "SerialCode");
            ViewData["UnitID"] = new SelectList(_context.tbl_ictams_laptopinv, "laptoptinvCode", "laptoptinvCode");
            ViewData["OwnerID"] = new SelectList(_context.tbl_ictams_owners, "OwnerCode", "OwnerCode");
            ViewData["StatusID"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_code");
            ViewData["CreatedBy"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode");
            ViewData["RTUpdated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode");
            return View();
        }

        // POST: LaptopBorroweds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BorrowedID,UnitID,SerialNumber,OwnerID,StatusID,DateBorrow,Remark,CreatedBy,DateCreated,RTUpdated,DateUpdated")] LaptopBorrowed laptopBorrowed)
        {
            var findLaptop = await _context.tbl_ictams_laptopinv
                .Where(x => x.laptoptinvCode == laptopBorrowed.UnitID)
                .FirstOrDefaultAsync();

            var findQuantity = findLaptop.Quantity - findLaptop.AllocatedNo;

            var FindQ = await _context.tbl_ictams_ltborrowed
                .Where(z => z.StatusID == "AC")
                .CountAsync(x => x.UnitID == laptopBorrowed.UnitID);

            if ( FindQ >= findQuantity  )
            {
                TempData["AlertMessage"] = "Out of quantity!";  
                return RedirectToAction("Create");
            }

            var findOwner = await _context.tbl_ictams_ltborrowed.Where(x=>x.OwnerID == laptopBorrowed.OwnerID && x.StatusID == "AC").FirstOrDefaultAsync();

            if(findOwner != null)
            {
                TempData["AlertMessage"] = "Owner is already exists!";
                return RedirectToAction("Create");
            }

            var paramCode = await _context.tbl_ictams_parameters
                    .Where(p => p.parm_code == "ltbr_id")
                    .MaxAsync(p => p.parm_value);
            var newparamCode = paramCode + 1;

            var param = await _context.tbl_ictams_parameters
                .FirstOrDefaultAsync(p => p.parm_code == "ltbr_id");
            param.parm_value = newparamCode;

            var ucode = HttpContext.Session.GetString("UserName");
            laptopBorrowed.BorrowedID = "LTB" + newparamCode.ToString().PadLeft(12, '0'); ;
            laptopBorrowed.DateBorrow = DateTime.Now;
            laptopBorrowed.CreatedBy = ucode;
            laptopBorrowed.Remark = laptopBorrowed.Remark.ToUpper();
            laptopBorrowed.DateCreated = DateTime.Now;
            laptopBorrowed.StatusID = "AC";

            _context.Add(laptopBorrowed);
                await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully borrow a laptop!";
            // ...
            return RedirectToAction(nameof(Index));


        }

        // GET: LaptopBorroweds/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.tbl_ictams_ltborrowed == null)
            {
                return NotFound();
            }

            var laptopBorrowed = await _context.tbl_ictams_ltborrowed.FindAsync(id);
            if (laptopBorrowed == null)
            {
                return NotFound();
            }

            return View(laptopBorrowed);
        }

        // POST: LaptopBorroweds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("BorrowedID,UnitID,OwnerID,StatusID,DateBorrow,Remark,CreatedBy,DateCreated,RTUpdated,DateUpdated")] LaptopBorrowed laptopBorrowed)
        {
            if (id != laptopBorrowed.BorrowedID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(laptopBorrowed);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LaptopBorrowedExists(laptopBorrowed.BorrowedID))
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
            return View(laptopBorrowed);
        }

        // GET: LaptopBorroweds/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.tbl_ictams_ltborrowed == null)
            {
                return NotFound();
            }

            var laptopBorrowed = await _context.tbl_ictams_ltborrowed
                .Include(l => l.LaptopInventory)
                .Include(l => l.Owner)
                .Include(l => l.Status)
                .Include(l => l.UserCreated)
                .Include(l => l.UserUpdated)
                .FirstOrDefaultAsync(m => m.BorrowedID == id);
            if (laptopBorrowed == null)
            {
                return NotFound();
            }

            return View(laptopBorrowed);
        }

        // POST: LaptopBorroweds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.tbl_ictams_ltborrowed == null)
            {
                return Problem("Entity set 'AssetManagementContext.LaptopBorrowed'  is null.");
            }
            var ucode = HttpContext.Session.GetString("UserName");
            var laptopBorrowed = await _context.tbl_ictams_ltborrowed.FindAsync(id);
            if (laptopBorrowed != null)
            {
                laptopBorrowed.DateUpdated = DateTime.Now;
                laptopBorrowed.RTUpdated = ucode;
                laptopBorrowed.StatusID = "RT";
                _context.tbl_ictams_ltborrowed.Update(laptopBorrowed);
            }
            
            await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully return a borrowed laptop!";
            // ...
            return RedirectToAction(nameof(Index));
        }

        private bool LaptopBorrowedExists(string id)
        {
          return (_context.tbl_ictams_ltborrowed?.Any(e => e.BorrowedID == id)).GetValueOrDefault();
        }
    }
}
