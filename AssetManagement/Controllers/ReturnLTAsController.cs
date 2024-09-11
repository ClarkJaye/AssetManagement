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
using AssetManagement.Models.View_Model;

namespace AssetManagement.Controllers
{
    public class ReturnLTAsController : BaseController
    {
        private readonly AssetManagementContext _context;

        public ReturnLTAsController(AssetManagementContext context)
             : base(context) // Pass the 'context' dependency to the base constructor
        {
            _context = context;
        }

        // GET: ReturnLTAs
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
                          pa.Module.ModuleTitle == "Return LTAS" &&  // Adjust the module name as needed
                          pa.ProfileId == userProfile.Value);
                if (!hasOpenAccess)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var assetManagementContext = _context.tbl_ictams_ltareturn.Include(r => r.LaptopAllocation).Include(r => r.ReturnType).Include(r => r.Status).Include(r => r.UserCreated).Include(r => r.UserUpdated);
                    return View(await assetManagementContext.ToListAsync());
                }
            }

            return RedirectToAction("Index", "Home");

        }

        public async Task<IActionResult> ReturnLTAViews()
        {
            var assetManagementContext = _context.tbl_ictams_ltareturn.Include(r => r.LaptopAllocation).Include(r => r.ReturnType).Include(r => r.Status).Include(r => r.UserCreated).Include(r => r.UserUpdated);
            return View(await assetManagementContext.ToListAsync());

        }

        // GET: ReturnLTAs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.tbl_ictams_ltareturn == null)
            {
                return NotFound();
            }

            var returnLTA = await _context.tbl_ictams_ltareturn
                .Include(r => r.LaptopAllocation)
                .Include(r => r.ReturnType)
                .Include(r => r.Status)
                .Include(r => r.UserCreated)
                .Include(r => r.UserUpdated)
                .FirstOrDefaultAsync(m => m.ReturnID == id);
            if (returnLTA == null)
            {
                return NotFound();
            }

            return View(returnLTA);
        }

        // GET: ReturnLTAs/Create
        public IActionResult Create(string id)
        {
            ViewBag.Id = id;
            return View();
        }

        // POST: ReturnLTAs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReturnID,AllocID,RTtype,RTStatus,CreatedBy,DateCreated,RTUpdated,DateUpdated")] ReturnLTA returnLTA)
        {
            var ucode = HttpContext.Session.GetString("UserName");

            var findFirstOwner = await _context.tbl_ictams_laptopalloc.Where(x => x.AllocId == returnLTA.AllocID && x.AllocationStatus == "AC").FirstOrDefaultAsync();
            if (findFirstOwner != null)
            {
                findFirstOwner.AllocationStatus = "IN";
            }

            var findRType = await _context.tbl_ictams_returntype.Where(x =>x.TypeID == returnLTA.RTtype).FirstOrDefaultAsync();
           

            var allocationID = await _context.tbl_ictams_laptopalloc.FirstOrDefaultAsync(x => x.AllocId == returnLTA.AllocID);


            if (findRType != null && findRType.Return_Inv == "N")
                {
                    returnLTA.RTStatus = "DM";
                    allocationID.AllocationStatus = "IN";
                }
            else 
            {
                    
                if (allocationID != null)
                {
                    var findLT = allocationID.LaptopCode;
                    var laptopInv = await _context.tbl_ictams_laptopinv.FirstOrDefaultAsync(a => a.laptoptinvCode == findLT);
                    var InvDetails = await _context.tbl_ictams_laptopinvdetails.FirstOrDefaultAsync(a => a.SerialCode == allocationID.SerialNumber);
                    if (laptopInv != null)
                    {
                        laptopInv.AllocatedNo -= 1;
                    }
                    allocationID.AllocationStatus = "IN";
                    InvDetails.UpdatedDate = DateTime.Now;
                    InvDetails.Updated = ucode;
                    InvDetails.LTStatus = "AV";
                    returnLTA.RTStatus = "AC";
                }
            }


            var paramCode = await _context.tbl_ictams_parameters
                   .Where(p => p.parm_code == "ltaret_id")
                   .MaxAsync(p => p.parm_value);
            var newparamCode = paramCode + 1;

            var param = await _context.tbl_ictams_parameters
                .FirstOrDefaultAsync(p => p.parm_code == "ltaret_id");
            param.parm_value = newparamCode;


            returnLTA.ReturnID = "LTAR" + newparamCode.ToString().PadLeft(11, '0');
            returnLTA.DateCreated = DateTime.Now;
            returnLTA.CreatedBy = ucode;
            

            _context.Add(returnLTA);
                await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully return!";
            // ...
            return RedirectToAction(nameof(Index));

        }


        public IActionResult CreateSecReturn(string id)
        {
            ViewBag.Id = id;
            return View();
        }   
        [HttpPost]
        public async Task<IActionResult> CreateSecReturn([Bind("ReturnID,AllocID,RTtype,RTStatus,CreatedBy,DateCreated,RTUpdated,DateUpdated")] ReturnLTA returnLTA )
        {
            var ucode = HttpContext.Session.GetString("UserName");

            var findFirstOwner = await _context.tbl_ictams_ltnewalloc   .Where(x => x.AllocId == returnLTA.AllocID && x.SecAllocationStatus == "AC").FirstOrDefaultAsync();
            if (findFirstOwner != null)
            {
                findFirstOwner.SecAllocationStatus = "IN";
            }

            var findRType = await _context.tbl_ictams_returntype.Where(x => x.TypeID == returnLTA.RTtype).FirstOrDefaultAsync();


            var allocationID = await _context.tbl_ictams_laptopalloc.FirstOrDefaultAsync(x => x.AllocId == returnLTA.AllocID);


            if (findRType != null && findRType.Return_Inv == "N")
            {
                returnLTA.RTStatus = "DM";
                allocationID.AllocationStatus = "IN";
            }
            else
            {

                if (allocationID != null)
                {
                    var findLT = allocationID.LaptopCode;
                    var InvDetails = await _context.tbl_ictams_laptopinvdetails.FirstOrDefaultAsync(a => a.SerialCode == allocationID.SerialNumber);
                    var laptopInv = await _context.tbl_ictams_laptopinv.FirstOrDefaultAsync(a => a.laptoptinvCode == findLT);
                    if (laptopInv != null)
                    {
                        laptopInv.AllocatedNo -= 1;
                    }
                    InvDetails.LTStatus = "AV";
                    InvDetails.UpdatedDate = DateTime.Now;
                    InvDetails.Updated = ucode;
                    allocationID.AllocationStatus = "IN";
                    returnLTA.RTStatus = "AC";
                }
            }



            var paramCode = await _context.tbl_ictams_parameters
                   .Where(p => p.parm_code == "ltaret_id")
                   .MaxAsync(p => p.parm_value);
            var newparamCode = paramCode + 1;

            var param = await _context.tbl_ictams_parameters
                .FirstOrDefaultAsync(p => p.parm_code == "ltaret_id");
            param.parm_value = newparamCode;


            
            returnLTA.ReturnID = "LTAR" + newparamCode.ToString().PadLeft(11, '0');
            returnLTA.DateCreated = DateTime.Now;
            returnLTA.CreatedBy = ucode;
            _context.Add(returnLTA);
            await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully return!";
            // ...
            return RedirectToAction("Index", "SecondOwnerAllocs");

        }
        // GET: ReturnLTAs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            ViewBag.Id = id;
            if (id == null || _context.tbl_ictams_ltareturn == null)
            {
                return NotFound();
            }

            var returnLTA = await _context.tbl_ictams_ltareturn.FindAsync(id);
            if (returnLTA == null)
            {
                return NotFound();
            }
            return View(returnLTA);
        }

        // POST: ReturnLTAs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ReturnID,AllocID,RTtype,RTStatus,CreatedBy,DateCreated,RTUpdated,DateUpdated")] ReturnLTA returnLTA)
        {
            if (id != returnLTA.ReturnID)
            {
                return NotFound();
            }


            var ucode = HttpContext.Session.GetString("UserName");
            returnLTA.DateUpdated = DateTime.Now;
            returnLTA.RTUpdated = ucode;
            returnLTA.RTStatus = "AC";

            _context.Update(returnLTA);
                    await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));

        }

        // GET: ReturnLTAs/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.tbl_ictams_ltareturn == null)
            {
                return NotFound();
            }

            var returnLTA = await _context.tbl_ictams_ltareturn
                .Include(r => r.LaptopAllocation)
                .Include(r => r.ReturnType)
                .Include(r => r.Status)
                .Include(r => r.UserCreated)
                .Include(r => r.UserUpdated)
                .FirstOrDefaultAsync(m => m.ReturnID == id);
            if (returnLTA == null)
            {
                return NotFound();
            }

            return View(returnLTA);
        }

        // POST: ReturnLTAs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.ReturnLTA == null)
            {
                return Problem("Entity set 'AssetManagementContext.ReturnLTA'  is null.");
            }
            var returnLTA = await _context.ReturnLTA.FindAsync(id);
            if (returnLTA != null)
            {
                _context.ReturnLTA.Remove(returnLTA);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReturnLTAExists(string id)
        {
          return (_context.tbl_ictams_ltareturn?.Any(e => e.ReturnID == id)).GetValueOrDefault();
        }
    }
}
