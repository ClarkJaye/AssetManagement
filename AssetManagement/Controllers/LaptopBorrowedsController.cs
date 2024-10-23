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
using AssetManagement.Models.View_Model;

namespace AssetManagement.Controllers
{
    public class LaptopBorrowedsController : BaseController
    {
        private readonly AssetManagementContext _context;

        public LaptopBorrowedsController(AssetManagementContext context)
            : base(context)
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

        public async Task<IActionResult> OwnerPartialView()
        {
            var assetManagementContext = _context.tbl_ictams_owners
             .Where(p => p.Status.status_code == "AC" && !_context.tbl_ictams_laptopalloc.Any(a => a.OwnerCode == p.OwnerCode && a.AllocationStatus == "AC"))
             .Where(p => p.Status.status_code == "AC" && !_context.tbl_ictams_ltnewalloc.Any(a => a.SecOwnerCode == p.OwnerCode && a.SecAllocationStatus == "AC"))
             .Include(o => o.Location)
             .Include(o => o.Createdby)
             .Include(o => o.Department)
             .Include(o => o.Status)
             .Include(o => o.Store)
             .Include(o => o.Updatedby);

            // Get the owners who have an active laptop borrowed
            var ownersWithActiveBorrow = await _context.tbl_ictams_ltborrowed
                                                 .Where(b => b.StatusID == "AC")
                                                 .Select(b => b.OwnerID)
                                                 .ToListAsync();

            // Filter out owners who have already borrowed an active laptop
            var availableOwners = assetManagementContext.Where(o => !ownersWithActiveBorrow.Contains(o.OwnerCode)).ToListAsync();

            OwnerViewModel ownerViewModel = new()
            {
                OwnerList = await availableOwners
            };

            return View(ownerViewModel);
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
                    var assetManagementContext = _context.tbl_ictams_ltborrowed.Where(x => x.StatusID == "AC").Include(l => l.LaptopInventoryDetails.LaptopInventory).Include(l => l.Department).Include(l => l.Owner).Include(l => l.Status).Include(l => l.UserCreated).Include(l => l.UserUpdated);
                    return View(await assetManagementContext.ToListAsync());
                }
            }

            return RedirectToAction("Index", "Home");

        }


        public async Task<IActionResult> Inactive()
        {
            var assetManagementContext = _context.tbl_ictams_ltborrowed.Where(x => x.StatusID == "RT").Include(l => l.LaptopInventoryDetails.LaptopInventory).Include(l => l.Department).Include(l => l.Owner).Include(l => l.Status).Include(l => l.UserCreated).Include(l => l.UserUpdated);
            return View(await assetManagementContext.ToListAsync());
        }

        public async Task<IActionResult> BorrowedViews()
        {
            var assetManagementContext = _context.tbl_ictams_ltborrowed.Where(x => x.StatusID == "RT").Include(l => l.LaptopInventoryDetails.LaptopInventory).Include(l => l.Owner).Include(l => l.Status).Include(l => l.UserCreated).Include(l => l.UserUpdated);
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
                .Include(l => l.LaptopInventoryDetails.LaptopInventory)
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
        public async Task<IActionResult> Create(string id)
        {
            var activeDept = await _context.tbl_ictams_department
                                            .Where(l => l.Dept_status != "IN")
                                            .ToListAsync();

            ViewData["LTDept"] = new SelectList(activeDept, "Dept_code", "Dept_name");

            var laptopInventory = await _context.tbl_ictams_laptopinv.FirstOrDefaultAsync(l => l.laptoptinvCode == id);

            if (laptopInventory == null)
            {
                return NotFound();
            }

            var laptop = await _context.tbl_ictams_laptopinvdetails
                                        .FirstOrDefaultAsync(ld => ld.laptoptinvCode == laptopInventory.laptoptinvCode);

            var laptopBorrowed = new LaptopBorrowed
            {
                UnitID = laptop?.laptoptinvCode ?? string.Empty
            };

            return View(laptopBorrowed);
        }

        // POST: LaptopBorroweds/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UnitID,SerialNumber,OwnerID,Deptment_Code,ComputerName,DateBorrow,Remark,Expected_return")] LaptopBorrowed laptopBorrowed)
        {
            var findLaptop = await _context.tbl_ictams_laptopinv.FirstOrDefaultAsync(x => x.laptoptinvCode == laptopBorrowed.UnitID);
            if (findLaptop == null)
            {
                TempData["ErrorNotification"] = "Laptop not found!";
                return RedirectToAction("Create");
            }

            var availableQuantity = findLaptop.Quantity - findLaptop.AllocatedNo;

            var borrowedCount = await _context.tbl_ictams_ltborrowed
                                        .Where(z => z.StatusID == "AC" && z.UnitID == laptopBorrowed.UnitID)
                                        .CountAsync();

            //if (borrowedCount >= availableQuantity)
            if(availableQuantity == 0)
            {
                TempData["AlertMessage"] = "Out of quantity!";
                return RedirectToAction("Create");
            }

            var existingBorrow = await _context.tbl_ictams_ltborrowed
                                            .Where(x => x.OwnerID == laptopBorrowed.OwnerID && x.StatusID == "AC")
                                            .FirstOrDefaultAsync();
            if (existingBorrow != null)
            {
                TempData["AlertMessage"] = "Owner has already borrowed a laptop!";
                return RedirectToAction("Create");
            }

            var paramCode = await _context.tbl_ictams_parameters
                                          .Where(p => p.parm_code == "ltbr_id")
                                          .MaxAsync(p => p.parm_value);
            var newparamCode = paramCode + 1;
            var param = await _context.tbl_ictams_parameters.FirstOrDefaultAsync(p => p.parm_code == "ltbr_id");
            param.parm_value = newparamCode;

            var ucode = HttpContext.Session.GetString("UserName");
            laptopBorrowed.BorrowedID = "LTB" + newparamCode.ToString().PadLeft(12, '0');
            laptopBorrowed.ComputerName = laptopBorrowed.ComputerName?.ToUpper();
            laptopBorrowed.DateBorrow = DateTime.Now;
            laptopBorrowed.DateCreated = DateTime.Now;
            laptopBorrowed.Remark = laptopBorrowed.Remark?.ToUpper();
            laptopBorrowed.CreatedBy = ucode;
            laptopBorrowed.StatusID = "AC";  // Active
            

            var inv_alloc = await _context.tbl_ictams_laptopinv.FirstOrDefaultAsync(a => a.laptoptinvCode == laptopBorrowed.UnitID);
            if (inv_alloc != null)
            {
                inv_alloc.AllocatedNo++;
            }

            var inv_details = await _context.tbl_ictams_laptopinvdetails
                                        .FirstOrDefaultAsync(a => a.SerialCode == laptopBorrowed.SerialNumber);
            if (inv_details != null)
            {
                inv_details.LTStatus = "AC";
                inv_details.DeployedDate = DateTime.Now;
            }

            _context.Add(laptopBorrowed);
            await _context.SaveChangesAsync();

            TempData["SuccessNotification"] = "Successfully borrowed a laptop!";
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
                .Include(l => l.LaptopInventoryDetails.LaptopInventory)
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
            if (id == null)
            {
                TempData["AlertMessage"] = "Id not found!";
                return RedirectToAction(nameof(Index));
            }

            if (_context.tbl_ictams_ltborrowed == null)
            {
                return Problem("Entity set 'AssetManagementContext.LaptopBorrowed' is null.");
            }

            var ucode = HttpContext.Session.GetString("UserName");
            var laptopBorrowed = await _context.tbl_ictams_ltborrowed.FindAsync(id);

            if (laptopBorrowed == null)
            {
                TempData["AlertMessage"] = "Laptop borrow record not found!";
                return RedirectToAction(nameof(Index));
            }

            laptopBorrowed.DateUpdated = DateTime.Now;
            laptopBorrowed.RTUpdated = ucode; 
            laptopBorrowed.StatusID = "RT";  
            _context.tbl_ictams_ltborrowed.Update(laptopBorrowed);

            var inv_alloc = await _context.tbl_ictams_laptopinv
                .FirstOrDefaultAsync(a => a.laptoptinvCode == laptopBorrowed.UnitID);

            if (inv_alloc != null)
            {
                inv_alloc.AllocatedNo = inv_alloc.AllocatedNo - 1;  
            }

            var inv_details = await _context.tbl_ictams_laptopinvdetails
                .FirstOrDefaultAsync(a => a.SerialCode == laptopBorrowed.SerialNumber);

            if (inv_details != null)
            {
                inv_details.LTStatus = "AV";      
                inv_details.DeployedDate = DateTime.Now; 
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            TempData["SuccessNotification"] = "Successfully returned the borrowed laptop!";
            return RedirectToAction(nameof(Index));
        }


        private bool LaptopBorrowedExists(string id)
        {
          return (_context.tbl_ictams_ltborrowed?.Any(e => e.BorrowedID == id)).GetValueOrDefault();
        }
    }
}
