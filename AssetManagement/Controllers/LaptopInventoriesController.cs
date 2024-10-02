

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Data;
using AssetManagement.Models;
using AssetManagement.Utility;
using OfficeOpenXml;
using System.ComponentModel;

namespace AssetManagement.Controllers
{
    public class LaptopInventoriesController : BaseController
    {
        private readonly AssetManagementContext _context;

        public LaptopInventoriesController(AssetManagementContext context)
        : base(context) // Pass the 'context' dependency to the base constructor
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetSpecs(string id, string id2)
        {
            ViewBag.Brands = id;
            ViewBag.Category = id2;

            if (!string.IsNullOrEmpty(id2))
            {
                var assetManagementContext = _context.tbl_ictams_laptopinv
                    .Where(x => x.LTStatus != "IN" && x.Brand.BrandDescription == id)
                    .Include(l => l.Brand)
                    .Include(l => l.CPU)
                    .Include(l => l.Createdby)
                    .Include(l => l.HardDisk)
                    .Include(l => l.Level)
                    .Include(l => l.Memory)
                    .Include(l => l.Memory.Capacity)
                    .Include(l => l.HardDisk.Capacity)
                    .Include(l => l.Model)
                    .Include(l => l.OS)
                    .Include(l => l.Status)
                    .Include(l => l.Updatedby);

                return View(await assetManagementContext.ToListAsync());
            }
            return View();
        }



        public async Task<IActionResult> SeeAll(string id, string ids, string id2)
        {
            ViewBag.Id = id;
            ViewBag.Ids = ids;
            ViewBag.Id2 = id2;
            var assetManagementContext = _context.tbl_ictams_laptopinvdetails.Where(x=>x.laptoptinvCode == id).Include(i => i.Createdby).Include(i => i.LaptopInventory).Include(i => i.Status).Include(i => i.Updatedby).Include(i => i.Vendor);
            return View(await assetManagementContext.ToListAsync());
        }

        public async Task<IActionResult> SeeAllSerial(string id)
        {
            var assetManagementContext = _context.tbl_ictams_laptopinvdetails.Where(x => x.laptoptinvCode == id && x.LTStatus == "AV").Include(i => i.Createdby).Include(i => i.LaptopInventory).Include(i => i.Status).Include(i => i.Updatedby).Include(i => i.Vendor);
            return View(await assetManagementContext.ToListAsync());
        }


        public async Task<IActionResult> ExportToExcel()
        {
            var data = await _context.tbl_ictams_laptopinv.ToListAsync(); // Fetch your data from the database

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Add headers
                var properties = typeof(LaptopInventory).GetProperties();
                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i].GetCustomAttributes(typeof(DisplayNameAttribute), true)
                                                            .OfType<DisplayNameAttribute>()
                                                            .FirstOrDefault()?.DisplayName ?? properties[i].Name;
                }

                // Add data rows
                for (int row = 0; row < data.Count; row++)
                {
                    for (int col = 0; col < properties.Length; col++)
                    {
                        worksheet.Cells[row + 2, col + 1].Value = properties[col].GetValue(data[row]);
                    }
                }

                // Generate file
                byte[] fileContents = package.GetAsByteArray();

                // Return Excel file
                return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "laptop_inventory.xlsx");
            }
        }

        public async Task<IActionResult> DeletedHistory(string userCODE)
        {

            var assetManagementContext = _context.tbl_ictams_laptopalloc.Include(l => l.Status).Include(l => l.Createdby).Include(l => l.LaptopInventory).Where(x => x.AllocationStatus == "IN" && x.LaptopCode.Contains(userCODE)).Include(l => l.Owner).Include(l => l.Updatedby).Include(l=>l.InventoryDetails);
            return View(await assetManagementContext.ToListAsync());
        }

        public async Task<IActionResult> ReturnDetails(string userCODE)
        {
            if (!string.IsNullOrEmpty(userCODE))
            {
                var assetManagementContext = _context.tbl_ictams_ltareturn.Where(x => x.LaptopAllocation.LaptopCode.Contains(userCODE)).Include(r => r.LaptopAllocation).Include(r => r.ReturnType).Include(r => r.Status).Include(r => r.UserCreated).Include(r => r.UserUpdated);
                return View(await assetManagementContext.ToListAsync());
            }
            return View();
        }

        public async Task<IActionResult> Borrowed(string userCODE)
        {
            if (!string.IsNullOrEmpty(userCODE))
            {
                var assetManagementContext = _context.tbl_ictams_ltborrowed.Where(x => x.StatusID == "AC" && x.UnitID.Contains(userCODE) ).Include(l => l.LaptopInventory).Include(l => l.Owner).Include(l => l.Status).Include(l => l.UserCreated).Include(l => l.UserUpdated);
                return View(await assetManagementContext.ToListAsync());
            }
            return View();
        }


        // GET: LaptopAllocations
        public async Task<IActionResult> InventoryDetails(string id,string ids, string id2)
        {
            ViewBag.Id = id;
            ViewBag.Ids = ids;
            ViewBag.Id2 = id2;

            var assetManagementContext = _context.tbl_ictams_laptopalloc.Where(x => x.AllocationStatus == "AC" && x.LaptopCode == id).Include(l => l.Status).Include(l => l.Createdby).Include(l => l.LaptopInventory).Include(l => l.Owner).Include(l => l.Updatedby).Include(l => l.InventoryDetails);
            return View(await assetManagementContext.ToListAsync());
        }

        public async Task<IActionResult> seeDetails(string userCODE)
        {
            if (!string.IsNullOrEmpty(userCODE))
            {
                var laptopInventory = await _context.tbl_ictams_laptopinv
                .Where(x => x.LTStatus == "AV" || x.LTStatus == "CO")
                .Include(l => l.Brand)
                .Include(l => l.CPU)
                .Include(l => l.Createdby)
                .Include(l => l.HardDisk)
                .Include(l => l.Level)
                .Include(l => l.Memory)
                .Include(l => l.Model)
                .Include(l => l.OS)
                .Include(l => l.Status)
                .Include(l => l.Updatedby)
                .FirstOrDefaultAsync(x => x.laptoptinvCode == userCODE);
                return View(laptopInventory);
            }
            return View();
        }

        public async Task<IActionResult> InventorySecOwnerDetails(string userCODE )
        {
            if (!string.IsNullOrEmpty(userCODE))
            {
                var assetManagementContext = await _context.tbl_ictams_ltnewalloc.Where(x => x.SecAllocationStatus == "AC" && x.SecLaptopCode.Contains(userCODE)).Include(s => s.Createdby).Include(s => s.LaptopAllocation).Include(s => s.LaptopInventory).Include(s => s.Owner).Include(s => s.Status).Include(s => s.Updatedby).Include(s => s.InventoryDetails).ToListAsync();

                return View( assetManagementContext);
            }
            return View();
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

        // GET: LaptopInventories
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
                    await FindStatus();
                    var countAvailLT = await _context.tbl_ictams_laptopinv.Where(x => x.Quantity > x.AllocatedNo && x.LTStatus == "AV").SumAsync(x => x.Quantity - x.AllocatedNo);
                    var totalAllocatedLaptops = await _context.tbl_ictams_laptopinv.SumAsync(x => x.AllocatedNo);

                    var assetManagementContext = _context.tbl_ictams_laptopinv.Where(x => x.LTStatus == "AV" || x.LTStatus == "CO").Include(l => l.Brand).Include(l => l.CPU).Include(l => l.Createdby).Include(l => l.HardDisk).Include(l => l.Level).Include(l => l.Memory).ThenInclude(CA => CA.Capacity).Include(l => l.Model).Include(l => l.OS).Include(l => l.Status).Include(l => l.Updatedby);

                    ViewBag.TotalAvailableLaptops = countAvailLT;
                    ViewBag.TotalAllocatedLaptops = totalAllocatedLaptops;

                    return View(await assetManagementContext.ToListAsync());
                }
            }

            return RedirectToAction("Index", "Home");
            
        }


        public async Task<IActionResult> RetrieveRow(string laptopCode)
        {
            if (string.IsNullOrEmpty(laptopCode))
            {
                // Handle the case where the laptopCode parameter is empty or null
                return BadRequest();
            }

            var laptopInventory = await _context.tbl_ictams_laptopinv
                .Where(x => x.LTStatus == "AV" || x.LTStatus == "CO")
                .Include(l => l.Brand)
                .Include(l => l.CPU)
                .Include(l => l.Createdby)
                .Include(l => l.HardDisk)
                .Include(l => l.Level)
                .Include(l => l.Memory)
                .Include(l => l.Model)
                .Include(l => l.OS)
                .Include(l => l.Status)
                .Include(l => l.Updatedby)
                .FirstOrDefaultAsync(x => x.Description == laptopCode);

            if (laptopInventory == null)
            {
                // Handle the case where the laptop with the given laptopCode is not found
                return NotFound();
            }

            return View("~/Views/LaptopInventories/RetrieveRow.cshtml", laptopInventory);

        }


        public async Task<IActionResult> Inactive()
        {
            await FindStatus();
            var assetManagementContext = _context.tbl_ictams_laptopinv.Where(x => x.LTStatus == "IN").Include(l => l.Brand).Include(l => l.CPU).Include(l => l.Createdby).Include(l => l.HardDisk).Include(l => l.Level).Include(l => l.Memory).Include(l => l.Model).Include(l => l.OS).Include(l => l.Status).Include(l => l.Updatedby);
            return View(await assetManagementContext.ToListAsync());
        }



        // GET: LaptopInventories/Create
        public IActionResult Create()
        {

            ViewData["LTBrand"] = new SelectList(_context.tbl_ictams_brand, "BrandId", "BrandId");
            ViewData["LTcpu"] = new SelectList(_context.tbl_ictams_cpu, "CPUId", "CPUId");
            ViewData["LTCreated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode");
            ViewData["LTHardisk"] = new SelectList(_context.tbl_ictams_hardisk, "HDId", "HDId");
            ViewData["LTLevel"] = new SelectList(_context.tbl_ictams_level, "LevelId", "LevelId");
            ViewData["LTMemory"] = new SelectList(_context.tbl_ictams_memory, "MemoryId", "MemoryId");
            ViewData["LTModel"] = new SelectList(_context.tbl_ictams_model, "ModelId", "ModelId");
            ViewData["LTOS"] = new SelectList(_context.tbl_ictams_os, "OSId", "OSId");
            ViewData["LTStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_code");
            ViewData["LTUpdated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode");
            return View();
        }

        // POST: LaptopInventories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("laptoptinvCode,Description,LTLevel,LTBrand,LTModel,LTcpu,LTHardisk,LTMemory,LTOS,AllocatedNo,LTStatus,LTCreated,DateCreated")] LaptopInventory laptopInventory)
        {
            if (laptopInventory.LTLevel.Equals(0))
            {
                TempData["AlertMessage"] = "Please select a level!";
                return RedirectToAction("Create");
            }
            if (laptopInventory.LTBrand.Equals(0))
            {
                TempData["AlertMessage"] = "Please select a brand!";
                return RedirectToAction("Create");
            }
            if (laptopInventory.LTcpu.Equals(0))
            {
                TempData["AlertMessage"] = "Please select a CPU!";
                return RedirectToAction("Create");
            }
            if (laptopInventory.LTModel.Equals(0))
            {
                TempData["AlertMessage"] = "Please select a model!";
                return RedirectToAction("Create");
            }
            if (laptopInventory.LTMemory.Equals(0))
            {
                TempData["AlertMessage"] = "Please select a memory!";
                return RedirectToAction("Create");
            }
            if (laptopInventory.LTOS.Equals(0))
            {
                TempData["AlertMessage"] = "Please select an OS!";
                return RedirectToAction("Create");
            }
            if (laptopInventory.LTHardisk.Equals(0))
            {
                TempData["AlertMessage"] = "Please select a hard disk!";
                return RedirectToAction("Create");
            }

            else
            {
                var paramCode = await _context.tbl_ictams_parameters.Where(p => p.parm_code == "lt_id").MaxAsync(p => p.parm_value);
                var newparamCode = paramCode + 1;
                var param = await _context.tbl_ictams_parameters.FirstOrDefaultAsync(p => p.parm_code == "lt_id");
                param.parm_value = newparamCode;


                var ucode = HttpContext.Session.GetString("UserName");
                laptopInventory.laptoptinvCode = "LT" + newparamCode.ToString().PadLeft(8, '0'); 
                laptopInventory.Description = laptopInventory.Description.ToUpper();
                laptopInventory.LTCreated = ucode;
                laptopInventory.DateCreated = DateTime.Now;
                laptopInventory.LTStatus = "AV";
                _context.Add(laptopInventory);
                await _context.SaveChangesAsync();
                // ...
                TempData["SuccessNotification"] = "Successfully added a new device to inventory";
                // ...
                return RedirectToAction(nameof(Index));

            }
            return RedirectToAction(nameof(Index));
        }

        // GET: LaptopInventories/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var laptopInventory = await _context.tbl_ictams_laptopinv
                .Include(x => x.Level)
                .Include(x => x.Brand)
                .Include(x => x.Model)
                .Include(x => x.CPU)
                .Include(x => x.HardDisk)
                .Include(x => x.Memory)
                .Include(x => x.OS)
                .FirstOrDefaultAsync(x => x.laptoptinvCode == id);

            if (laptopInventory == null)
            {
                return NotFound();
            }

            return View(laptopInventory);
        }


        // POST: LaptopInventories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("laptoptinvCode,Description,LTLevel,LTBrand,LTModel,LTcpu,LTHardisk,LTMemory,LTOS,Quantity,AllocatedNo,LTStatus,LTCreated,DateCreated,LTUpdated,DateUpdated")] LaptopInventory laptopInventory)
        {
            if (id != laptopInventory.laptoptinvCode)
            {
                return NotFound();
            }

            var AllocatedChecker = await _context.tbl_ictams_laptopinv.Where(x => x.laptoptinvCode == laptopInventory.laptoptinvCode).FirstOrDefaultAsync();
            if (AllocatedChecker.Quantity > 0)
            {
                TempData["AlertMessage"] = "You can't Edit this!";
                return RedirectToAction("Edit");
            }
            var laptopInventory1 = await _context.tbl_ictams_laptopinv.Where(c => c.laptoptinvCode == laptopInventory.laptoptinvCode).FirstOrDefaultAsync();
            try
            {
                var ucode = HttpContext.Session.GetString("UserName");
                laptopInventory1.LTUpdated = ucode;
                laptopInventory1.DateUpdated = DateTime.Now;
                laptopInventory1.LTStatus = "AV";
                laptopInventory1.Description = laptopInventory.Description;
                laptopInventory1.LTLevel = laptopInventory.LTLevel;
                laptopInventory1.LTOS = laptopInventory.LTOS;
                laptopInventory1.LTBrand = laptopInventory.LTBrand;
                laptopInventory1.LTcpu = laptopInventory.LTcpu;
                laptopInventory1.LTHardisk = laptopInventory.LTHardisk;
                laptopInventory1.LTMemory = laptopInventory.LTMemory;
                laptopInventory1.LTModel = laptopInventory.LTModel;
                laptopInventory1.LTCreated = laptopInventory.LTCreated;
                laptopInventory1.DateCreated = laptopInventory.DateCreated;

                await _context.SaveChangesAsync();
                // ...
                TempData["SuccessNotification"] = "Successfully edited!";
            // ...
                return RedirectToAction(nameof(Index));
            }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LaptopInventoryExists(laptopInventory.laptoptinvCode))
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


        public async Task<IActionResult> DeleteAsEdit(string[] selectedIds)
        {
            ViewData["UserStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_name");
            foreach (string id in selectedIds)
            {
                var inventoryCode = await _context.tbl_ictams_laptopinv.FindAsync(id);
                var ltinvExist = await _context.tbl_ictams_laptopalloc.FirstOrDefaultAsync(x => x.LaptopCode == id);
                if (inventoryCode != null)
                {
                    // Check if the selected profile status exists in the tbl_lsm_status table
                    var status = await _context.tbl_ictams_status.FindAsync(inventoryCode.LTStatus);
                    if (status == null)
                    {
                        ModelState.AddModelError("", $"Profile status '{inventoryCode.laptoptinvCode}' does not exist.");
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
                        inventoryCode.LTUpdated = ucode;
                        inventoryCode.LTStatus = "IN";
                        inventoryCode.DateUpdated = DateTime.Now;

                        _context.Update(inventoryCode);
                        await _context.SaveChangesAsync();
                        // ...
                        TempData["SuccessNotification"] = "Successfully deleted!";
                        // ...
                        return RedirectToAction(nameof(Index));
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
                var inventoryCode = await _context.tbl_ictams_laptopinv.FindAsync(id);
                if (inventoryCode != null)
                {
                    // Check if the selected profile status exists in the tbl_lsm_status table
                    var status = await _context.tbl_ictams_status.FindAsync(inventoryCode.laptoptinvCode);
                    if (status == null)
                    {
                        ModelState.AddModelError("", $"Profile status '{inventoryCode.laptoptinvCode}' does not exist.");
                    }

                    // Update the profile
                    var ucode = HttpContext.Session.GetString("UserName");
                    inventoryCode.LTUpdated = ucode;
                    inventoryCode.LTStatus = "AV";
                    inventoryCode.DateUpdated = DateTime.Now;

                    _context.Update(inventoryCode);
                    await _context.SaveChangesAsync();

                    // ...
                    TempData["SuccessNotification"] = "Successfully retrieved!";
                    // ...
                    return RedirectToAction(nameof(Index));
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool LaptopInventoryExists(string id)
        {
          return (_context.tbl_ictams_laptopinv?.Any(e => e.laptoptinvCode == id)).GetValueOrDefault();
        }
    }
}
