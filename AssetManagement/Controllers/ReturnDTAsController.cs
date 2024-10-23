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
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

namespace AssetManagement.Controllers
{
    public class ReturnDTAsController : BaseController
    {
        private readonly AssetManagementContext _context;

        public ReturnDTAsController(AssetManagementContext context)
            : base(context)
        {
            _context = context;
        }

        // GET: ReturnDTAs
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
                          pa.Module.ModuleTitle == "Return DTAS" &&
                          pa.ProfileId == userProfile.Value);
                if (!hasOpenAccess)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var assetManagementContext = _context.tbl_ictams_dtareturn.Include(r => r.DesktopAllocation).Include(r => r.ReturnType).Include(r => r.Status).Include(r => r.UserCreated).Include(r => r.UserUpdated);
                    return View(await assetManagementContext.ToListAsync());
                }
            }

            return RedirectToAction("Index", "Home");

        }
        public async Task<IActionResult> ReturnDTAViews()
        {
            var assetManagementContext = _context.tbl_ictams_dtareturn.Include(r => r.DesktopAllocation).Include(r => r.ReturnType).Include(r => r.Status).Include(r => r.UserCreated).Include(r => r.UserUpdated);
            return View(await assetManagementContext.ToListAsync());

        }

        // GET: DesktopReturns/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.tbl_ictams_dtareturn == null)
            {
                return NotFound();
            }

            var desktopReturn = await _context.tbl_ictams_dtareturn
                .Include(d => d.DesktopAllocation)
                .Include(d => d.ReturnType)
                .Include(d => d.Status)
                .Include(d => d.UserCreated)
                .Include(d => d.UserUpdated)
                .FirstOrDefaultAsync(m => m.ReturnID == id);
            if (desktopReturn == null)
            {
                return NotFound();
            }

            return View(desktopReturn);
        }

        // GET: DesktopReturns/Create
        public IActionResult Create(string id)
        {
            ViewBag.Id = id;
            return View();
        }


        // POST: DesktopReturns/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReturnID,AllocID,RTtype,RTStatus,CreatedBy,DateCreated,RTUpdated,DateUpdated")] ReturnDTA desktopReturn)
        {
            var ucode = HttpContext.Session.GetString("UserName");

            var findFirstOwner = await _context.tbl_ictams_desktopalloc.Where(x => x.AllocId == desktopReturn.AllocID && x.AllocationStatus == "AC").FirstOrDefaultAsync();
            if (findFirstOwner != null)
            {
                findFirstOwner.AllocationStatus = "IN";
            }

            var findRType = await _context.tbl_ictams_returntype.Where(x => x.TypeID == desktopReturn.RTtype).FirstOrDefaultAsync();


            var allocationID = await _context.tbl_ictams_desktopalloc.FirstOrDefaultAsync(x => x.AllocId == desktopReturn.AllocID);


            if (findRType != null && findRType.Return_Inv == "N")
            {
                desktopReturn.RTStatus = "DM";
                allocationID.AllocationStatus = "IN";

                var findLT = allocationID.DesktopCode;
                var laptopInv = await _context.tbl_ictams_desktopinv.FirstOrDefaultAsync(a => a.desktopInvCode == findLT);
                var InvDetails = await _context.tbl_ictams_desktopinvdetails.FirstOrDefaultAsync(a => a.desktopInvCode == allocationID.DesktopCode && a.unitTag == allocationID.UnitTag);
                if (laptopInv != null)
                {
                    laptopInv.Quantity -= 1;
                    laptopInv.AllocatedNo -= 1;
                }
                InvDetails.UpdatedDate = DateTime.Now;
                InvDetails.Updated = ucode;
                InvDetails.DTStatus = "DM";
            }
            else
            {

                if (allocationID != null)
                {
                    var findLT = allocationID.DesktopCode;
                    var laptopInv = await _context.tbl_ictams_desktopinv.FirstOrDefaultAsync(a => a.desktopInvCode == findLT);
                    var InvDetails = await _context.tbl_ictams_desktopinvdetails.FirstOrDefaultAsync(a => a.unitTag == allocationID.UnitTag);
                    if (laptopInv != null)
                    {
                        laptopInv.AllocatedNo -= 1;
                    }
                    allocationID.AllocationStatus = "IN";
                    InvDetails.UpdatedDate = DateTime.Now;
                    InvDetails.Updated = ucode;
                    InvDetails.DTStatus = "AV";
                    desktopReturn.RTStatus = "AC";
                }
            }


            var paramCode = await _context.tbl_ictams_parameters
                   .Where(p => p.parm_code == "dtaret_id")
                   .MaxAsync(p => p.parm_value);
            var newparamCode = paramCode + 1;

            var param = await _context.tbl_ictams_parameters
                .FirstOrDefaultAsync(p => p.parm_code == "dtaret_id");
            param.parm_value = newparamCode;


            desktopReturn.ReturnID = "DTAR" + newparamCode.ToString().PadLeft(11, '0');
            desktopReturn.DateCreated = DateTime.Now;
            desktopReturn.CreatedBy = ucode;


            _context.Add(desktopReturn);
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
        public async Task<IActionResult> CreateSecReturn([Bind("ReturnID,AllocID,RTtype,RTStatus,CreatedBy,DateCreated,RTUpdated,DateUpdated")] ReturnDTA desktopReturn)
        {
            var ucode = HttpContext.Session.GetString("UserName");

            var findFirstOwner = await _context.tbl_ictams_dtnewalloc.Where(x => x.AllocId == desktopReturn.AllocID && x.SecAllocationStatus == "AC").FirstOrDefaultAsync();
            if (findFirstOwner != null)
            {
                findFirstOwner.SecAllocationStatus = "IN";
            }

            var findRType = await _context.tbl_ictams_returntype.Where(x => x.TypeID == desktopReturn.RTtype).FirstOrDefaultAsync();


            var allocationID = await _context.tbl_ictams_desktopalloc.FirstOrDefaultAsync(x => x.AllocId == desktopReturn.AllocID);


            if (findRType != null && findRType.Return_Inv == "N")
            {
                desktopReturn.RTStatus = "DM";
                allocationID.AllocationStatus = "IN";
            }
            else
            {

                if (allocationID != null)
                {
                    var findLT = allocationID.DesktopCode;
                    var InvDetails = await _context.tbl_ictams_desktopinvdetails.FirstOrDefaultAsync(a => a.unitTag == allocationID.UnitTag);
                    var laptopInv = await _context.tbl_ictams_desktopinv.FirstOrDefaultAsync(a => a.desktopInvCode == findLT);
                    if (laptopInv != null)
                    {
                        laptopInv.AllocatedNo -= 1;
                    }
                    InvDetails.DTStatus = "AV";
                    InvDetails.UpdatedDate = DateTime.Now;
                    InvDetails.Updated = ucode;
                    allocationID.AllocationStatus = "IN";
                    desktopReturn.RTStatus = "AC";
                }
            }



            var paramCode = await _context.tbl_ictams_parameters
                   .Where(p => p.parm_code == "dtaret_id")
                   .MaxAsync(p => p.parm_value);
            var newparamCode = paramCode + 1;

            var param = await _context.tbl_ictams_parameters
                .FirstOrDefaultAsync(p => p.parm_code == "dtaret_id");
            param.parm_value = newparamCode;



            desktopReturn.ReturnID = "DTAR" + newparamCode.ToString().PadLeft(11, '0');
            desktopReturn.DateCreated = DateTime.Now;
            desktopReturn.CreatedBy = ucode;
            _context.Add(desktopReturn);
            await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully return!";
            // ...
            return RedirectToAction("Index", "DesktopNewAllocs");

        }

        // GET: DesktopReturns/Edit/5
        public async Task<IActionResult> Edit(string id)
        {

            ViewBag.Id = id;
            if (id == null || _context.tbl_ictams_dtareturn == null)
            {
                return NotFound();
            }

            var desktopReturn = await _context.tbl_ictams_dtareturn.FindAsync(id);
            if (desktopReturn == null)
            {
                return NotFound();
            }
            return View(desktopReturn);
        }

        // POST: DesktopReturns/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ReturnID,AllocID,RTtype,RTStatus,CreatedBy,DateCreated,RTUpdated,DateUpdated")] ReturnDTA desktopReturn)
        {
            if (id != desktopReturn.ReturnID)
            {
                return NotFound();
            }


            var ucode = HttpContext.Session.GetString("UserName");
            desktopReturn.DateUpdated = DateTime.Now;
            desktopReturn.RTUpdated = ucode;
            desktopReturn.RTStatus = "AC";

            _context.Update(desktopReturn);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: DesktopReturns/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.tbl_ictams_dtareturn == null)
            {
                return NotFound();
            }

            var desktopReturn = await _context.tbl_ictams_dtareturn
                .Include(d => d.DesktopAllocation)
                .Include(d => d.ReturnType)
                .Include(d => d.Status)
                .Include(d => d.UserCreated)
                .Include(d => d.UserUpdated)
                .FirstOrDefaultAsync(m => m.ReturnID == id);
            if (desktopReturn == null)
            {
                return NotFound();
            }

            return View(desktopReturn);
        }

        // POST: DesktopReturns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.tbl_ictams_dtareturn == null)
            {
                return Problem("Entity set 'AssetManagementContext.tbl_ictams_dtareturn'  is null.");
            }
            var desktopReturn = await _context.tbl_ictams_dtareturn.FindAsync(id);
            if (desktopReturn != null)
            {
                _context.tbl_ictams_dtareturn.Remove(desktopReturn);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DesktopReturnExists(string id)
        {
            return (_context.tbl_ictams_dtareturn?.Any(e => e.ReturnID == id)).GetValueOrDefault();
        }
       }
    }
