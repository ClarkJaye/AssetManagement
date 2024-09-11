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
    public class ReturnPeripheralsController : BaseController
    {
        private readonly AssetManagementContext _context;

        public ReturnPeripheralsController(AssetManagementContext context)
            : base(context) // Pass the 'context' dependency to the base constructor
        {
            _context = context;
        }

        // GET: ReturnPeripherals
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
                          pa.Module.ModuleTitle == "Return Peripherals" &&  // Adjust the module name as needed
                          pa.ProfileId == userProfile.Value);
                if (!hasOpenAccess)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var assetManagementContext = _context.tbl_ictams_ltpareturn.Include(r => r.LaptopPeripheralAllocation).Include(r => r.ReturnType).Include(r => r.Status).Include(r => r.UserCreated).Include(r => r.UserUpdated);
                    return View(await assetManagementContext.ToListAsync());
                }
            }

            return RedirectToAction("Index", "Home");
            
        }

        public async Task<IActionResult> ReturnPeripheralViews()
        {
            var assetManagementContext = _context.tbl_ictams_ltpareturn.Include(r => r.LaptopPeripheralAllocation).Include(r => r.ReturnType).Include(r => r.Status).Include(r => r.UserCreated).Include(r => r.UserUpdated);
            return View(await assetManagementContext.ToListAsync());

        }


        // GET: ReturnPeripherals/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.tbl_ictams_ltpareturn == null)
            {
                return NotFound();
            }

            var returnPeripheral = await _context.tbl_ictams_ltpareturn
                .Include(r => r.LaptopPeripheralAllocation)
                .Include(r => r.ReturnType)
                .Include(r => r.Status)
                .Include(r => r.UserCreated)
                .Include(r => r.UserUpdated)
                .FirstOrDefaultAsync(m => m.ReturnID == id);
            if (returnPeripheral == null)
            {
                return NotFound();
            }

            return View(returnPeripheral);
        }

        // GET: ReturnPeripherals/Create
        public IActionResult Create(string id)
        {
            ViewBag.Id = id;
            return View();
        }

        // POST: ReturnPeripherals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReturnID,AllocID,RTtype,RTStatus,CreatedBy,DateCreated,RTUpdated,DateUpdated")] ReturnPeripheral returnPeripheral)
        {


            var findFirstOwner = await _context.tbl_ictams_ltperipheralalloc.Where(x => x.PeripheralAllocationId == returnPeripheral.AllocID && x.PeripheralAllocStatus == "AC").FirstOrDefaultAsync();
            if (findFirstOwner != null)
            {
                findFirstOwner.PeripheralAllocStatus = "IN";
            }


            var findRType = await _context.tbl_ictams_returntype.Where(x =>x.TypeID == returnPeripheral.RTtype).FirstOrDefaultAsync();

            var allocationID = await _context.tbl_ictams_ltperipheralalloc.FirstOrDefaultAsync(x => x.PeripheralAllocationId == returnPeripheral.AllocID);
            if (findRType != null && findRType.Return_Inv == "N")
            {
                returnPeripheral.RTStatus = "DM";
                allocationID.PeripheralAllocStatus = "IN";
            }
            else
            { 

                if (allocationID != null)
                {
                    var findLT = allocationID.PeripheralCode;
                    var laptopInv = await _context.tbl_ictams_ltperipheral.FirstOrDefaultAsync(a => a.PeripheralCode == findLT);
                    if (laptopInv != null)
                    {
                        laptopInv.PeripheralAllocation -= 1;
                    }
                    allocationID.PeripheralAllocStatus = "IN";
                    returnPeripheral.RTStatus = "AC";
                }
            }



            var paramCode = await _context.tbl_ictams_parameters
                   .Where(p => p.parm_code == "ltparet_id")
                   .MaxAsync(p => p.parm_value);
            var newparamCode = paramCode + 1;

            var param = await _context.tbl_ictams_parameters
                .FirstOrDefaultAsync(p => p.parm_code == "ltparet_id");
            param.parm_value = newparamCode;

            var ucode = HttpContext.Session.GetString("UserName");
            returnPeripheral.ReturnID = "LTPA" + newparamCode.ToString().PadLeft(11, '0');
            returnPeripheral.DateCreated = DateTime.Now;
            returnPeripheral.CreatedBy = ucode;
            

            _context.Add(returnPeripheral);
                await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully return!";
            // ...

            return RedirectToAction(nameof(Index));

     

        }


        // GET: ReturnPeripherals/Create
        public IActionResult CreateSecPeripherals(string id)
        {
            ViewBag.Id = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateSecPeripherals([Bind("ReturnID,AllocID,RTtype,RTStatus,CreatedBy,DateCreated,RTUpdated,DateUpdated")] ReturnPeripheral returnPeripheral)
        {
         

            var findFirstOwner = await _context.tbl_ictams_ltpsecowner.Where(x => x.LaptopPeripAlloc.PeripheralAllocationId == returnPeripheral.AllocID && x.SecAllocationStatus == "AC").FirstOrDefaultAsync();
            if (findFirstOwner != null)
            {
                findFirstOwner.SecAllocationStatus = "IN";
            }


            var findRType = await _context.tbl_ictams_returntype.Where(x => x.TypeID == returnPeripheral.RTtype).FirstOrDefaultAsync();

            var allocationID = await _context.tbl_ictams_ltpsecowner.FirstOrDefaultAsync(x => x.AllocId == returnPeripheral.AllocID);
            if (findRType != null && findRType.Return_Inv == "N")
            {
                returnPeripheral.RTStatus = "DM";
                allocationID.SecAllocationStatus = "IN";
            }
            else
            {
                
                if (allocationID != null)
                {
                    var findLT = allocationID.SecPeripheralCode;
                    var laptopInv = await _context.tbl_ictams_ltperipheral.FirstOrDefaultAsync(a => a.PeripheralCode == findLT);
                    if (laptopInv != null)
                    {
                        laptopInv.PeripheralAllocation -= 1;
                    }
                    allocationID.SecAllocationStatus = "IN";
                    returnPeripheral.RTStatus = "AC";
                }
            }



            var paramCode = await _context.tbl_ictams_parameters
                   .Where(p => p.parm_code == "ltparet_id")
                   .MaxAsync(p => p.parm_value);
            var newparamCode = paramCode + 1;

            var param = await _context.tbl_ictams_parameters
                .FirstOrDefaultAsync(p => p.parm_code == "ltparet_id");
            param.parm_value = newparamCode;

            var ucode = HttpContext.Session.GetString("UserName");
            returnPeripheral.ReturnID = "LTPA" + newparamCode.ToString().PadLeft(11, '0');
            returnPeripheral.DateCreated = DateTime.Now;
            returnPeripheral.CreatedBy = ucode;

            _context.Add(returnPeripheral);
            await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully return!";
            // ...

            return RedirectToAction(nameof(Index));



        }

        // GET: ReturnPeripherals/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            ViewBag.Id = id;
            if (id == null || _context.tbl_ictams_ltpareturn == null)
            {
                return NotFound();
            }

            var returnPeripheral = await _context.tbl_ictams_ltpareturn.FindAsync(id);
            if (returnPeripheral == null)
            {
                return NotFound();
            }
            return View(returnPeripheral);
        }

        // POST: ReturnPeripherals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ReturnID,AllocID,RTtype,RTStatus,CreatedBy,DateCreated,RTUpdated,DateUpdated")] ReturnPeripheral returnPeripheral)
        {
            if (id != returnPeripheral.ReturnID)
            {
                return NotFound();
            }
            var ucode = HttpContext.Session.GetString("UserName");
            returnPeripheral.DateUpdated = DateTime.Now;
            returnPeripheral.RTUpdated = ucode;
            returnPeripheral.RTStatus = "DM";

            _context.Update(returnPeripheral);
                    await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));

        }

        // GET: ReturnPeripherals/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.tbl_ictams_ltpareturn == null)
            {
                return NotFound();
            }

            var returnPeripheral = await _context.tbl_ictams_ltpareturn
                .Include(r => r.LaptopPeripheralAllocation)
                .Include(r => r.ReturnType)
                .Include(r => r.Status)
                .Include(r => r.UserCreated)
                .Include(r => r.UserUpdated)
                .FirstOrDefaultAsync(m => m.ReturnID == id);
            if (returnPeripheral == null)
            {
                return NotFound();
            }

            return View(returnPeripheral);
        }

        // POST: ReturnPeripherals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.tbl_ictams_ltpareturn == null)
            {
                return Problem("Entity set 'AssetManagementContext.tbl_ictams_ltpareturn'  is null.");
            }
            var returnPeripheral = await _context.tbl_ictams_ltpareturn.FindAsync(id);
            if (returnPeripheral != null)
            {
                _context.tbl_ictams_ltpareturn.Remove(returnPeripheral);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReturnPeripheralExists(string id)
        {
          return (_context.tbl_ictams_ltpareturn?.Any(e => e.ReturnID == id)).GetValueOrDefault();
        }
    }
}
