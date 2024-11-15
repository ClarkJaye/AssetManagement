using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Data;
using AssetManagement.Utility;
using AssetManagement.Models;

namespace AssetManagement.Controllers
{
    public class ReturnMTAsController : BaseController
    {
        private readonly AssetManagementContext _context;

        public ReturnMTAsController(AssetManagementContext context)
            :base (context)
        {
            _context = context;
        }

        // GET: ReturnMTAs
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
                          pa.Module.ModuleTitle == "Return MTAS" &&  // Adjust the module name as needed
                          pa.ProfileId == userProfile.Value);
                if (!hasOpenAccess)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var assetManagementContext = _context.tbl_ictams_monitorreturn
                        .Include(r => r.MonitorAllocation)
                        .Include(r => r.MonitorAllocation.MonitorDetails)
                        .Include(r => r.MonitorAllocation.MonitorDetails.MonitorInventory)
                        .Include(r => r.ReturnType)
                        .Include(r => r.Status)
                        .Include(r => r.UserCreated)
                        .Include(r => r.UserUpdated);
                    return View(await assetManagementContext.ToListAsync());
                }
            }

                return RedirectToAction("Index", "Home");
            }

        public async Task<IActionResult> ReturnMTAViews()
        {
            var assetManagementContext = _context.tbl_ictams_monitorreturn.Include(r => r.MonitorAllocation).Include(r => r.ReturnType).Include(r => r.Status).Include(r => r.UserCreated).Include(r => r.UserUpdated);
            return View(await assetManagementContext.ToListAsync());

        }
        // GET: ReturnMTAs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null ||  _context.tbl_ictams_monitorreturn == null)
            {
                return NotFound();
            }

            var returnMTA = await _context.tbl_ictams_monitorreturn
                .Include(r => r.MonitorAllocation)
                .Include(r => r.ReturnType)
                .Include(r => r.Status)
                .Include(r => r.UserCreated)
                .Include(r => r.UserUpdated)
                .FirstOrDefaultAsync(m => m.ReturnID == id);
            if (returnMTA == null)
            {
                return NotFound();
            }

            return View(returnMTA);
        }

        // GET: ReturnMTAs/Create
        public async Task<IActionResult> Create(string id)
     {
            var item = await _context.tbl_ictams_monitoralloc.Include(x=>x.MonitorDetails)
                .FirstOrDefaultAsync(i => i.AllocId == id);

            ViewData["allocID"] = item.AllocId;  
            ViewData["monitorCode"] = item.MonitorDetails.monitorCode;
            ViewData["monitorSerial"] = item.MonitorDetails.SerialNumber;
            ViewData["MTReturnType"] = new SelectList(_context.tbl_ictams_returntype, "TypeID", "Description");

            return View();
        }

        // POST: ReturnMTAs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReturnID,AllocID,RTtype,RTStatus,CreatedBy,DateCreated,RTUpdated,DateUpdated")] ReturnMTA returnMTA)
        {
            var ucode = HttpContext.Session.GetString("UserName");

            var findFirstOwner = await _context.tbl_ictams_monitoralloc.Where(x => x.AllocId == returnMTA.AllocID && x.AllocationStatus == "AC").FirstOrDefaultAsync();
            if (findFirstOwner != null)
            {
                findFirstOwner.AllocationStatus = "IN";
            }

            var findRType = await _context.tbl_ictams_returntype.Where(x => x.TypeID == returnMTA.RTtype).FirstOrDefaultAsync();


            var allocationID = await _context.tbl_ictams_monitoralloc.FirstOrDefaultAsync(x => x.AllocId == returnMTA.AllocID);


            if (findRType != null && findRType.Return_Inv == "N")
            {
                returnMTA.RTStatus = "DM";
                allocationID.AllocationStatus = "IN";
            }
            else
            {

                if (allocationID != null)
                {
                    var findLT = allocationID.monitorCode;
                    var laptopInv = await _context.tbl_ictams_monitorinv.FirstOrDefaultAsync(a => a.monitorCode == findLT);
                    var InvDetails = await _context.tbl_ictams_monitordetails.FirstOrDefaultAsync(a => a.SerialNumber == allocationID.SerialNumber);
                    if (laptopInv != null)
                    {
                        laptopInv.AllocatedNo -= 1;
                    }
                    allocationID.AllocationStatus = "IN";
                    InvDetails.DateUpdated = DateTime.Now;
                    InvDetails.DetailUpdated = ucode;
                    InvDetails.MonitorStatus = "AV";
                    returnMTA.RTStatus = "AC";
                }
            }


            var paramCode = await _context.tbl_ictams_parameters
                   .Where(p => p.parm_code == "monaret_id")
                   .MaxAsync(p => p.parm_value);
            var newparamCode = paramCode + 1;

            var param = await _context.tbl_ictams_parameters
                .FirstOrDefaultAsync(p => p.parm_code == "monaret_id");
            param.parm_value = newparamCode;


            returnMTA.ReturnID = "MTAR" + newparamCode.ToString().PadLeft(11, '0');
            returnMTA.DateCreated = DateTime.Now;
            returnMTA.CreatedBy = ucode;


            _context.Add(returnMTA);
            await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully return!";
            // ...
            return RedirectToAction(nameof(Index));
        }
        public IActionResult CreateSecReturn(string id)
        {
            ViewBag.Id = id;
            ViewData["MTReturnType"] = new SelectList(_context.tbl_ictams_returntype, "TypeID", "Description");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateSecReturn([Bind("ReturnID,AllocID,RTtype,RTStatus,CreatedBy,DateCreated,RTUpdated,DateUpdated")] ReturnMTA returnMTA)
        {
            var ucode = HttpContext.Session.GetString("UserName");

            var findFirstOwner = await _context.tbl_ictams_monitornewalloc.Where(x => x.AllocId == returnMTA.AllocID && x.SecAllocationStatus == "AC").FirstOrDefaultAsync();
            if (findFirstOwner != null)
            {
                findFirstOwner.SecAllocationStatus = "IN";
            }

            var findRType = await _context.tbl_ictams_returntype.Where(x => x.TypeID == returnMTA.RTtype).FirstOrDefaultAsync();


            var allocationID = await _context.tbl_ictams_monitoralloc.FirstOrDefaultAsync(x => x.AllocId == returnMTA.AllocID);


            if (findRType != null && findRType.Return_Inv == "N")
            {
                returnMTA.RTStatus = "DM";
                allocationID.AllocationStatus = "IN";
            }
            else
            {

                if (allocationID != null)
                {
                    var findLT = allocationID.monitorCode;
                    var InvDetails = await _context.tbl_ictams_monitordetails.FirstOrDefaultAsync(a => a.SerialNumber == allocationID.SerialNumber);
                    var laptopInv = await _context.tbl_ictams_monitorinv.FirstOrDefaultAsync(a => a.monitorCode == findLT);
                    if (laptopInv != null)
                    {
                        laptopInv.AllocatedNo -= 1;
                    }
                    InvDetails.MonitorStatus = "AV";
                    InvDetails.DateUpdated = DateTime.Now;
                    InvDetails.DetailUpdated = ucode;
                    allocationID.AllocationStatus = "IN";
                    returnMTA.RTStatus = "AC";
                }
            }



            var paramCode = await _context.tbl_ictams_parameters
                   .Where(p => p.parm_code == "monaret_id")
                   .MaxAsync(p => p.parm_value);
            var newparamCode = paramCode + 1;

            var param = await _context.tbl_ictams_parameters
                .FirstOrDefaultAsync(p => p.parm_code == "monaret_id");
            param.parm_value = newparamCode;



            returnMTA.ReturnID = "MTAR" + newparamCode.ToString().PadLeft(11, '0');
            returnMTA.DateCreated = DateTime.Now;
            returnMTA.CreatedBy = ucode;
            _context.Add(returnMTA);
            await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully return!";
            // ...
            return RedirectToAction("Index", "MonitorNewAllocs");

        }
        // GET: ReturnMTAs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            ViewBag.Id = id;
            ViewData["MTReturnType"] = new SelectList(_context.tbl_ictams_returntype, "TypeID", "Description");
            if (id == null || _context.tbl_ictams_monitorreturn == null)
            {
                return NotFound();
            }

            var returnMTA = await _context.tbl_ictams_monitorreturn.FindAsync(id);
            if (returnMTA == null)
            {
                return NotFound();
            }
            return View(returnMTA);
        }

        // POST: ReturnMTAs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ReturnID,AllocID,RTtype,RTStatus,CreatedBy,DateCreated,RTUpdated,DateUpdated")] ReturnMTA returnMTA)
        {
            if (id != returnMTA.ReturnID)
            {
                return NotFound();
            }


            var ucode = HttpContext.Session.GetString("UserName");
            returnMTA.DateUpdated = DateTime.Now;
            returnMTA.RTUpdated = ucode;
            returnMTA.RTStatus = "AC";

            _context.Update(returnMTA);
            await _context.SaveChangesAsync();
            TempData["SuccessNotification"] = "Successfully updated!";
            return RedirectToAction(nameof(Index));
        }

        // GET: ReturnMTAs/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.tbl_ictams_monitorreturn == null)
            {
                return NotFound();
            }

            var returnMTA = await _context.tbl_ictams_monitorreturn
                .Include(r => r.MonitorAllocation)
                .Include(r => r.ReturnType)
                .Include(r => r.Status)
                .Include(r => r.UserCreated)
                .Include(r => r.UserUpdated)
                .FirstOrDefaultAsync(m => m.ReturnID == id);
            if (returnMTA == null)
            {
                return NotFound();
            }

            return View(returnMTA);
        }

        // POST: ReturnMTAs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.tbl_ictams_monitorreturn == null)
            {
                return Problem("Entity set 'AssetManagementContext.ReturnMTA'  is null.");
            }
            var returnMTA = await _context.tbl_ictams_monitorreturn.FindAsync(id);
            if (returnMTA != null)
            {
                _context.tbl_ictams_monitorreturn.Remove(returnMTA);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReturnMTAExists(string id)
        {
          return (_context.tbl_ictams_monitorreturn?.Any(e => e.ReturnID == id)).GetValueOrDefault();
        }   
    }
}
