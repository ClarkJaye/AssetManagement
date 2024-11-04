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
    public class DesktopInventoriesController : BaseController
    {
        private readonly AssetManagementContext _context;

        public DesktopInventoriesController(AssetManagementContext context)
            : base(context)
        {
            _context = context;
        }


        public async Task<IActionResult> SeeAllUnitTag(string id)
        {
            var assetManagementContext = _context.tbl_ictams_desktopinvdetails.Where(x => x.desktopInvCode == id && x.DTStatus == "AV").Include(d => d.Createdby).Include(i => i.DesktopInventory).Include(d => d.Status).Include(d => d.Updatedby).Include(d => d.Vendor);
            return View(await assetManagementContext.ToListAsync());
        }

        public async Task<IActionResult> SeeAll(string id, string ids, string id2)
        {
            ViewBag.Id = id;
            ViewBag.Ids = ids;
            ViewBag.Id2 = id2;
            var assetManagementContext = _context.tbl_ictams_desktopinvdetails.Where(x=>x.desktopInvCode == id && x.DTStatus != "IN").Include(d => d.Createdby).Include(d => d.Status).Include(d => d.Updatedby).Include(d => d.Vendor);
            return View(await assetManagementContext.ToListAsync());
        }


        // GET: DesktopInventories/DeskInvPartialView
        public async Task<IActionResult> DeskInvPartialView(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("Invalid desktop inventory code.");
            }

            var assetManagementContext = _context.tbl_ictams_desktopinvdetails
                .Where(x => x.desktopInvCode == code && x.DTStatus == "AV")
                .OrderByDescending(x => x.desktopInvCode)
                .Include(l => l.DesktopInventory)
                .Include(l => l.Vendor)
                .Include(l => l.Createdby);

            // Return the partial view with the filtered data
            return PartialView(await assetManagementContext.ToListAsync());
        }


        // GET: DesktopInventories/DeskInvenPartialView
        public async Task<IActionResult> DeskInvenPartialView(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("Invalid desktop inventory code.");
            }

            var assetManagementContext = _context.tbl_ictams_desktopinvdetails
                .Where(x => x.desktopInvCode == code && x.DTStatus == "AV")
                .Include(l => l.Createdby);

            // Return the partial view with the filtered data
            return PartialView(await assetManagementContext.ToListAsync());
        }



        public JsonResult GetUnitNumbers(string code)
        {
            // Get serial numbers from inventory details that are available and not already allocated
            var unitTags = _context.tbl_ictams_desktopinvdetails
                .Where(l => l.desktopInvCode == code && l.DTStatus == "AV")
                .Where(l => !_context.DesktopAllocation
                    .Any(alloc => alloc.UnitTag == l.unitTag && alloc.DesktopCode == code && alloc.AllocationStatus != "AC" && alloc.AllocationStatus != "IN"))
                .Select(l => new { l.unitTag })
                .ToList();

            return Json(unitTags);
        }

        public JsonResult GetComputerName(string code, string unitTag)
        {
            var computerName = _context.tbl_ictams_desktopinvdetails
                .Where(l => l.desktopInvCode == code && l.unitTag == unitTag)
                .Select(l => l.ComputerName)
                .FirstOrDefault();

            return Json(computerName);
        }



        // GET: DesktopInventories
        public async Task<IActionResult> seeDetails(string userCODE)
        {
            if (!string.IsNullOrEmpty(userCODE))
            {
                var laptopInventory = await _context.tbl_ictams_desktopinv
                .Where(x => x.DTStatus == "AV" || x.DTStatus == "CO")
                .Include(l => l.Brand)
                .Include(l => l.CPU)
                .Include(l => l.Createdby)
                .Include(l => l.HardDisk)
                .Include(l => l.Level)
                .Include(l => l.Memory)
                .Include(l => l.GraphicsCard)
                .Include(l => l.MainBoard)
                .Include(l => l.Model)
                .Include(l => l.OS)
                .Include(l => l.Status)
                .Include(l => l.Updatedby)
                .FirstOrDefaultAsync(x => x.desktopInvCode == userCODE);
                return View(laptopInventory);
            }
            return View();
        }
        public async Task<IActionResult> Index()
        {
            await FindAllocationCompleted();
            int? userProfile = HttpContext.Session.GetInt32("UserProfile");
            if (userProfile.HasValue)
            {

                var hasOpenAccess = await _context.tbl_ictams_profileaccess
          .AnyAsync(pa => pa.OpenAccess == "Y" &&
                          pa.Module.ModuleTitle == "Laptop Inventories" &&  // Adjust the module name as needed
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

                    await FindStatus();
                    // Count total laptops
                    var totalDekstop = await _context.tbl_ictams_desktopinv.SumAsync(x => x.Quantity);
                    // Count available laptops (where Quantity > AllocatedNo and status is 'AV')
                    var countAvailDT = await _context.tbl_ictams_desktopinv
                        .Where(x => x.Quantity > x.AllocatedNo && x.DTStatus == "AV")
                        .SumAsync(x => x.Quantity - x.AllocatedNo);

                    // Count total allocated laptops
                    var totalAllocatedDT = await _context.tbl_ictams_desktopinv.SumAsync(x => x.AllocatedNo);

                    var assetManagementContext = _context.tbl_ictams_desktopinv.Where(x => x.Status.status_code != "IN").Include(d => d.Brand).Include(d => d.CPU).Include(d => d.Createdby).Include(d => d.GraphicsCard).Include(d => d.HardDisk).Include(d => d.Level).Include(d => d.MainBoard).Include(d => d.Memory).Include(d => d.Model).Include(d => d.OS).Include(d => d.Status).Include(d => d.Updatedby);

                    ViewBag.TotalDekstop = totalDekstop;
                    ViewBag.TotalAvailableDT = countAvailDT;
                    ViewBag.TotalAllocatedDT= totalAllocatedDT;

                    return View(await assetManagementContext.ToListAsync());
                }
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Inactive()
        {

            await FindStatus();
            var assetManagementContext = _context.tbl_ictams_desktopinv.Where(x => x.DTStatus == "IN").Include(d => d.Brand).Include(d => d.CPU).Include(d => d.Createdby).Include(d => d.GraphicsCard).Include(d => d.HardDisk).Include(d => d.Level).Include(d => d.MainBoard).Include(d => d.Memory).Include(d => d.Model).Include(d => d.OS).Include(d => d.Status).Include(d => d.Updatedby);
            return View(await assetManagementContext.ToListAsync());
        }

        // GET: DesktopInventories/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.DesktopInventory == null)
            {
                return NotFound();
            }

            var desktopInventory = await _context.tbl_ictams_desktopinv
                .Include(d => d.Brand)
                .Include(d => d.CPU)
                .Include(d => d.Createdby)
                .Include(d => d.GraphicsCard)
                .Include(d => d.HardDisk)
                .Include(d => d.Level)
                .Include(d => d.MainBoard)
                .Include(d => d.Memory)
                .Include(d => d.Model)
                .Include(d => d.OS)
                .Include(d => d.Status)
                .Include(d => d.Updatedby)
                .FirstOrDefaultAsync(m => m.desktopInvCode == id);
            if (desktopInventory == null)
            {
                return NotFound();
            }

            return View(desktopInventory);
        }

        // GET: DesktopAllocations
        public async Task<IActionResult> InventoryDetails(string id, string ids, string id2)
        {
            ViewBag.Id = id;
            ViewBag.Ids = ids;
            ViewBag.Id2 = id2;

            var assetManagementContext = _context.tbl_ictams_desktopalloc.Where(x => x.AllocationStatus == "AC" && x.DesktopCode == id).Include(l => l.Status).Include(l => l.Createdby).Include(l => l.InventoryDetails.DesktopInventory).Include(l => l.Owner).Include(l => l.Updatedby).Include(l => l.InventoryDetails);
            return View(await assetManagementContext.ToListAsync());
        }
 
        public async Task<IActionResult> DeletedHistory(string userCODE)
        {

            var assetManagementContext = _context.tbl_ictams_desktopalloc.Include(l => l.Status).Include(l => l.Createdby).Include(l => l.InventoryDetails.DesktopInventory).Where(x => x.AllocationStatus == "IN" && x.DesktopCode.Contains(userCODE)).Include(l => l.Owner).Include(l => l.Updatedby).Include(l => l.InventoryDetails);
            return View(await assetManagementContext.ToListAsync());
        }
        public async Task<IActionResult> ReturnDetails(string userCODE)
        {
            if (!string.IsNullOrEmpty(userCODE))
            {
                var assetManagementContext = _context.tbl_ictams_dtareturn.Where(x => x.DesktopAllocation.DesktopCode.Contains(userCODE)).Include(r => r.DesktopAllocation).Include(r => r.ReturnType).Include(r => r.Status).Include(r => r.UserCreated).Include(r => r.UserUpdated);
                return View(await assetManagementContext.ToListAsync());
            }
            return View();
        }
        public async Task<IActionResult> Borrowed(string userCODE)
        {
            if (!string.IsNullOrEmpty(userCODE))
            {
                var assetManagementContext = _context.tbl_ictams_dtborrowed.Where(x => x.StatusID == "AC" && x.UnitID.Contains(userCODE)).Include(l => l.InventoryDetails).Include(l => l.Owner).Include(l => l.Status).Include(l => l.UserCreated).Include(l => l.UserUpdated);
                return View(await assetManagementContext.ToListAsync());
            }
            return View();
        }
        public async Task<IActionResult> InventorySecOwnerDetails(string userCODE)
        {
            if (!string.IsNullOrEmpty(userCODE))
            {
                var assetManagementContext = await _context.tbl_ictams_dtnewalloc.Where(x => x.SecAllocationStatus == "AC" && x.SecDesktopCode.Contains(userCODE)).Include(s => s.Createdby).Include(s => s.DesktopAllocation).Include(s => s.InventoryDetails).Include(s => s.Owner).Include(s => s.Status).Include(s => s.Updatedby).Include(s => s.InventoryDetails).ToListAsync();

                return View(assetManagementContext);
            }
            return View();
        }
        public async Task<IActionResult> RetrieveRow(string desktopCode)
        {
            if (string.IsNullOrEmpty(desktopCode))
            {
                // Handle the case where the desktopCode parameter is empty or null
                return BadRequest();
            }

            var desktopInventory = _context.tbl_ictams_desktopinv
                .Where(x => x.DTStatus == "AV" || x.DTStatus == "CO")
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
                .Include(l => l.Updatedby)
                .FirstOrDefaultAsync(x => x.Description == desktopCode);

            if (desktopInventory == null)
            {
                // Handle the case where the laptop with the given desktopCode is not found
                return NotFound();
            }

            return View("~/Views/DesktopInventories/RetrieveRow.cshtml", desktopInventory);

        }

        // GET: DesktopInventories/Create
        public IActionResult Create()
        {
            PopulateDropDowns();
            return View();
    
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("desktopInvCode,Description,DTLevel,DTBrand,DTModel,DTMBoard,DTcpu,DTgraphics,DTHardisk,DTMemory,DTOS,Quantity,AllocatedNo,DTStatus,DTCreated,DateCreated,DTUpdated,DateUpdated")] DesktopInventory desktopInventory)
        {
            // Manual validations for dropdown selections
            if (desktopInventory.DTLevel == 0)
            {
                TempData["AlertMessage"] = "Please select a level!";
                return View(desktopInventory);
            }
            if (desktopInventory.DTBrand == 0)
            {
                TempData["AlertMessage"] = "Please select a brand!";
                return View(desktopInventory);
            }
            if (desktopInventory.DTcpu == 0)
            {
                TempData["AlertMessage"] = "Please select a CPU!";
                return View(desktopInventory);
            }
            if (desktopInventory.DTModel == 0)
            {
                TempData["AlertMessage"] = "Please select a model!";
                return View(desktopInventory);
            }
            if (desktopInventory.DTMemory == 0)
            {
                TempData["AlertMessage"] = "Please select a memory!";
                return View(desktopInventory);
            }
            if (desktopInventory.DTOS == 0)
            {
                TempData["AlertMessage"] = "Please select an OS!";
                return View(desktopInventory);
            }
            if (desktopInventory.DTHardisk == 0)
            {
                TempData["AlertMessage"] = "Please select a hard disk!";
                return View(desktopInventory);
            }

            // Generate new desktopInvCode and set other properties
            var paramCode = await _context.tbl_ictams_parameters.Where(p => p.parm_code == "dt_id").MaxAsync(p => p.parm_value);
            var newparamCode = paramCode + 1;
            var param = await _context.tbl_ictams_parameters.FirstOrDefaultAsync(p => p.parm_code == "dt_id");
            param.parm_value = newparamCode;

            var ucode = HttpContext.Session.GetString("UserName");
            desktopInventory.desktopInvCode = "DT" + newparamCode.ToString().PadLeft(8, '0');
            desktopInventory.Description = desktopInventory.Description.ToUpper();
            desktopInventory.DTCreated = ucode;
            desktopInventory.DateCreated = DateTime.Now;
            desktopInventory.DTStatus = "AV";
            desktopInventory.Quantity = 0;
            desktopInventory.AllocatedNo = 0;

            _context.Add(desktopInventory);
            await _context.SaveChangesAsync();

            TempData["SuccessNotification"] = "Successfully added a new Desktop to inventory";
            return RedirectToAction(nameof(Index));
        }


        // GET: DesktopInventories/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // Populate dropdown lists using a helper method
            PopulateDropDowns();

            var desktopInventory = await _context.tbl_ictams_desktopinv
                .Include(d => d.Brand)
                .Include(d => d.CPU)
                .Include(d => d.Createdby)
                .Include(d => d.GraphicsCard)
                .Include(d => d.HardDisk)
                .Include(d => d.Level)
                .Include(d => d.MainBoard)
                .Include(d => d.Memory)
                .Include(d => d.Model)
                .Include(d => d.OS)
                .Include(d => d.Status)
                .Include(d => d.Updatedby)
                .FirstOrDefaultAsync(m => m.desktopInvCode == id);

            if (desktopInventory == null)
            {
                return NotFound();
            }

            return View(desktopInventory);
        }


        // POST: DesktopInventories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("desktopInvCode,Description,DTLevel,DTBrand,DTModel,DTMBoard,DTcpu,DTgraphics,DTHardisk,DTMemory,DTOS,Quantity,AllocatedNo,DTStatus,DTCreated,DateCreated,DTUpdated,DateUpdated")] DesktopInventory desktopInventory)
        {
            if (id != desktopInventory.desktopInvCode)
            {
                return NotFound();
            }

            var desktopInventory1 = await _context.tbl_ictams_desktopinv.FirstOrDefaultAsync(v => v.desktopInvCode == desktopInventory.desktopInvCode);

            if (desktopInventory1 == null)
            {
                TempData["AlertMessage"] = "Desktop Inventory not found!";
                return RedirectToAction("Edit");
            }

            // Prevent editing if quantity is greater than or equal to 1
            if (desktopInventory1.AllocatedNo >= 1)
            {
                TempData["AlertMessage"] = "You can't edit this, It is already allocated!";
                return RedirectToAction("Edit", new { id = desktopInventory.desktopInvCode });
            }

            try
            {
                var ucode = HttpContext.Session.GetString("UserName");

                // Update values
                desktopInventory1.Description = desktopInventory.Description;
                desktopInventory1.DTLevel = desktopInventory.DTLevel;
                desktopInventory1.DTBrand = desktopInventory.DTBrand;
                desktopInventory1.DTModel = desktopInventory.DTModel;
                desktopInventory1.DTMBoard = desktopInventory.DTMBoard;
                desktopInventory1.DTcpu = desktopInventory.DTcpu;
                desktopInventory1.DTgraphics = desktopInventory.DTgraphics;
                desktopInventory1.DTHardisk = desktopInventory.DTHardisk;
                desktopInventory1.DTMemory = desktopInventory.DTMemory;
                desktopInventory1.DTOS = desktopInventory.DTOS;
                desktopInventory1.DTUpdated = ucode;
                desktopInventory1.DateUpdated = DateTime.Now;

                await _context.SaveChangesAsync();

                TempData["SuccessNotification"] = "Successfully edited!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DesktopInventoryExists(desktopInventory.desktopInvCode))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }


        public async Task<IActionResult> DeleteAsEdit(string[] selectedIds)
        {
            ViewData["UserStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_name");
            foreach (string id in selectedIds)
            {
                var desktopInventoryCode = await _context.tbl_ictams_desktopinv.FindAsync(id);
                var ltinvExist = await _context.tbl_ictams_desktopalloc.FirstOrDefaultAsync(x => x.DesktopCode == id);
                if (desktopInventoryCode != null)
                {
                    // Check if the selected profile status exists in the tbl_lsm_status table
                    var status = await _context.tbl_ictams_status.FindAsync(desktopInventoryCode.DTStatus);
                    if (status == null)
                    {
                        TempData["AlertMessage"] = $"Status for {desktopInventoryCode.desktopInvCode} does not exist or is invalid.";
                    }
                    if (ltinvExist != null)
                    {
                        TempData["AlertMessage"] = "Deletion Not Permitted: Device Already Allocated. The selected device cannot be deleted as it is currently allocated in the Laptop Allocation system. This device is already in use and its allocation status prevents its removal at this time.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        // Update the profile
                        var ucode = HttpContext.Session.GetString("UserName");
                        desktopInventoryCode.DTUpdated = ucode;
                        desktopInventoryCode.DTStatus = "IN";
                        desktopInventoryCode.DateUpdated = DateTime.Now;

                        _context.Update(desktopInventoryCode);
                        await _context.SaveChangesAsync();
                        // ...
                        TempData["SuccessNotification"] = "Successfully Temporary Deleted!";
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Retrieve(string[] selectedIds)
        {
            ViewData["UserStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_name");
            foreach (string id in selectedIds)
            {
                var desktopInventoryCode = await _context.tbl_ictams_desktopinv.FindAsync(id);
                if (desktopInventoryCode != null)
                {
                    var status = await _context.tbl_ictams_desktopinv.FindAsync(desktopInventoryCode.desktopInvCode);
                    if (status == null)
                    {
                        TempData["AlertMessage"] = $"Status for {desktopInventoryCode.desktopInvCode} does not exist or is invalid.";
                    }
                    else 
                    {
                        var ucode = HttpContext.Session.GetString("UserName");
                        desktopInventoryCode.DTUpdated = ucode;
                        desktopInventoryCode.DTStatus = "AV";
                        desktopInventoryCode.DateUpdated = DateTime.Now;

                        _context.Update(desktopInventoryCode);
                        await _context.SaveChangesAsync();
                        TempData["SuccessNotification"] = $"Successfully retrieved data";
                    }
                }
                else
                {
                    TempData["AlertMessage"] = $"Desktop Inventory Code {id} not found.!";
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: DesktopInventories/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.tbl_ictams_desktopinv == null)
            {
                return NotFound();
            }

            var desktopInventory = await _context.tbl_ictams_desktopinv
                .Include(d => d.Brand)
                .Include(d => d.CPU)
                .Include(d => d.Createdby)
                .Include(d => d.GraphicsCard)
                .Include(d => d.HardDisk)
                .Include(d => d.Level)
                .Include(d => d.MainBoard)
                .Include(d => d.Memory)
                .Include(d => d.Model)
                .Include(d => d.OS)
                .Include(d => d.Status)
                .Include(d => d.Updatedby)
                .FirstOrDefaultAsync(m => m.desktopInvCode == id);
            if (desktopInventory == null)
            {
                return NotFound();
            }

            return View(desktopInventory);
        }

        // POST: DesktopInventories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.tbl_ictams_desktopinv == null)
            {
                return Problem("Entity set 'AssetManagementContext.DesktopInventory'  is null.");
            }
            var desktopInventory = await _context.tbl_ictams_desktopinv.FindAsync(id);
            if (desktopInventory != null)
            {
                _context.tbl_ictams_desktopinv.Remove(desktopInventory);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // Helper method to populate dropdowns
        private void PopulateDropDowns()
        {
            ViewData["DTLevel"] = new SelectList(_context.tbl_ictams_level, "LevelId", "LevelDescription");
            ViewData["DTBrand"] = new SelectList(_context.tbl_ictams_brand, "BrandId", "BrandDescription");
            ViewData["DTModel"] = new SelectList(_context.tbl_ictams_model, "ModelId", "ModelDescription");
            ViewData["DTMBoard"] = new SelectList(_context.tbl_ictams_mainboard, "BoardID", "BoardDescription");
            ViewData["DTcpu"] = new SelectList(_context.tbl_ictams_cpu, "CPUId", "CPUDescription");
            ViewData["DTgraphics"] = new SelectList(_context.tbl_ictams_graphicscard, "GraphicsID", "GraphicsDescription");
            ViewData["DTHardisk"] = new SelectList(_context.tbl_ictams_hardisk, "HDId", "HDDescription");
            ViewData["DTMemory"] = new SelectList(_context.tbl_ictams_memory, "MemoryId", "MemoryDescription");
            ViewData["DTOS"] = new SelectList(_context.tbl_ictams_os, "OSId", "OSDescription");

            ViewData["DTCreated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode");
            ViewData["DTUpdated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode");
            ViewData["DTStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_code");
        }

        private bool DesktopInventoryExists(string id)
        {
          return (_context.tbl_ictams_desktopinv?.Any(e => e.desktopInvCode == id)).GetValueOrDefault();
        }
    }
}
