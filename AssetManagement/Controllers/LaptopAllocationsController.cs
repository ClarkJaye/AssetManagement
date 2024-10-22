
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Data;
using AssetManagement.Models;
using AssetManagement.Utility;
using OfficeOpenXml;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Filters;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;

namespace AssetManagement.Controllers
{
    public class LaptopAllocationsController : BaseController
    {
        private readonly AssetManagementContext _context;

        public LaptopAllocationsController(AssetManagementContext context)
        : base(context) // Pass the 'context' dependency to the base constructor
        {
            _context = context;
        }

        public async Task<IActionResult> ChangeStatus(string id)
        {
            var findAllocation = await _context.tbl_ictams_laptopalloc.Where(x => x.AllocId == id).FirstOrDefaultAsync();
            var findSerial = await _context.tbl_ictams_laptopinvdetails.Where(x => x.SerialCode == findAllocation.SerialNumber).FirstOrDefaultAsync();

            if(findSerial != null)
            {
                findAllocation.AllocationStatus = "CO";
                findSerial.LTStatus = "CO";
            }

            await _context.SaveChangesAsync();

            // ...
            TempData["SuccessNotification"] = "Successfully updated the allocation status!";
            // ...
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ExportToExcel()
        {
            var data = await _context.tbl_ictams_laptopalloc.ToListAsync(); // Fetch your data from the database

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                // Add headers
                var properties = typeof(LaptopAllocation).GetProperties();
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
                return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "laptop_allocation.xlsx");
            }
        }
        public async Task<IActionResult> LaptopReports(string id)
        {
            ViewBag.Id = id;

            if (id == null)
            {
                return View();
            }
            else
            {
                var count = await _context.tbl_ictams_laptopalloc
                .Where(x => x.AllocationStatus == "CO" &&
                            _context.tbl_ictams_owners.Any(a => a.OwnerCode == x.OwnerCode && a.Department.Dept_name == id) &&
                            _context.tbl_ictams_laptopinvdetails.Any(a => a.SerialCode == x.SerialNumber && a.PurchaseDate < DateTime.Now.AddMonths(-48)))
                .CountAsync();

                ViewBag.TotalAvailableLaptop = "Total number of" + " " + id + ": " + count;

                var assetManagementContext = _context.tbl_ictams_laptopalloc
                    .Where(x => x.AllocationStatus == "CO" &&
                        _context.tbl_ictams_owners.Any(a => a.OwnerCode == x.OwnerCode && a.Department.Dept_name == id) &&
                        _context.tbl_ictams_laptopinvdetails.Any(a => a.SerialCode == x.SerialNumber  && a.PurchaseDate < DateTime.Now.AddMonths(-12)))
                    .Include(l => l.Status)
                    .Include(l => l.Createdby)
                    .Include(l => l.LaptopInventory)
                    .Include(l => l.Owner)
                    .ThenInclude(l => l.Department)
                    .Include(l => l.Updatedby)
                    .Include(l => l.InventoryDetails);

                var laptopAllocations = await assetManagementContext.ToListAsync();

                // Check if there are any laptop allocations that meet the criteria
                if (laptopAllocations.Any())
                {
                    return View(laptopAllocations);
                }
                else
                {
                    // Set id to null and return a view with id as null
                    id = null;
                    ViewBag.Id = id; // Update ViewBag.Id to reflect the change
                    return View(); // Return the view with id as null
                }
            }
        }
        public async Task<IActionResult> LaptopReportsDeploy(string id)
{
    ViewBag.Id = id;

    if (id == null)
    {
        return View();
    }
    else
    {
        DateTime fourYearsAgo = DateTime.Now.AddYears(-4);

        var assetManagementContext = _context.tbl_ictams_laptopalloc
            .Where(x => x.AllocationStatus == "CO" &&
                _context.tbl_ictams_owners.Any(a => a.OwnerCode == x.OwnerCode && a.Department.Dept_name == id) &&
                x.DateDeployed < DateTime.Now && x.DateDeployed < fourYearsAgo)
            .Include(l => l.Status)
            .Include(l => l.Createdby)
            .Include(l => l.LaptopInventory)
            .Include(l => l.Owner)
            .ThenInclude(l => l.Department)
            .Include(l => l.Updatedby)
            .Include(l => l.InventoryDetails);

        var laptopAllocations = await assetManagementContext.ToListAsync();

        // Check if there are any laptop allocations that meet the criteria
        if (laptopAllocations.Any())
        {
            return View(laptopAllocations);
        }
        else
        {
            // Set id to null and return a view with id as null
            id = null;
            ViewBag.Id = id; // Update ViewBag.Id to reflect the change
            return View(); // Return the view with id as null
        }
    }
}




        public async Task<IActionResult> LaptopReport2(string id)
        {
            ViewBag.Id = id;

            if (id == null)
            {
                return View();
            }
            else
            {
                var count = await _context.tbl_ictams_ltnewalloc
                .Where(x => x.SecAllocationStatus == "CO" &&
                            _context.tbl_ictams_owners.Any(a => a.OwnerCode == x.SecOwnerCode && a.Department.Dept_name == id) &&
                            _context.tbl_ictams_laptopinvdetails.Any(a => a.SerialCode == x.SerialNumber && a.PurchaseDate < DateTime.Now.AddMonths(-48)))
                .CountAsync();

                ViewBag.TotalAvailableLaptop = "Total number of" + " " + id + ": " + count;

                var assetManagementContext = _context.tbl_ictams_ltnewalloc
                    .Where(x => x.SecAllocationStatus == "CO" &&
                        _context.tbl_ictams_owners.Any(a => a.OwnerCode == x.SecOwnerCode && a.Department.Dept_name == id) &&
                        _context.tbl_ictams_laptopinvdetails.Any(a => a.SerialCode == x.SerialNumber && a.PurchaseDate < DateTime.Now.AddMonths(-12)))
                    .Include(l => l.Status)
                    .Include(l => l.Createdby)
                    .Include(l => l.LaptopInventory)
                    .Include(l => l.LaptopAllocation)
                    .Include(l => l.LaptopAllocation.Owner)
                    .Include(l => l.Owner)
                    .ThenInclude(l => l.Department)
                    .Include(l => l.Updatedby)
                    .Include(l => l.InventoryDetails);

                var laptopAllocations = await assetManagementContext.ToListAsync();

                // Check if there are any laptop allocations that meet the criteria
                if (laptopAllocations.Any())
                {
                    return View(laptopAllocations);
                }
                else
                {
                    // Set id to null and return a view with id as null
                    id = null;
                    ViewBag.Id = id; // Update ViewBag.Id to reflect the change
                    return View(); // Return the view with id as null
                }
            }
        }
        public async Task<IActionResult> LaptopReportsDeploy2(string id)
        {
            ViewBag.Id = id;

            if (id == null)
            {
                return View();
            }
            else
            {
                DateTime fourYearsAgo = DateTime.Now.AddYears(-4);

                var assetManagementContext = _context.tbl_ictams_ltnewalloc
                    .Where(x => x.SecAllocationStatus == "CO" &&
                        _context.tbl_ictams_owners.Any(a => a.OwnerCode == x.SecOwnerCode && a.Department.Dept_name == id) &&  _context.tbl_ictams_laptopinvdetails.Any(x => x.SerialCode == x.SerialCode && x.DeployedDate < DateTime.Now && x.DeployedDate < fourYearsAgo))
                    .Include(l => l.Status)
                    .Include(l => l.Createdby)
                    .Include(l => l.LaptopInventory)
                    .Include(l => l.Owner)
                    .ThenInclude(l => l.Department)
                    .Include(l => l.Updatedby)
                    .Include(l => l.InventoryDetails);

                var laptopAllocations = await assetManagementContext.ToListAsync();

                // Check if there are any laptop allocations that meet the criteria
                if (laptopAllocations.Any())
                {
                    return View(laptopAllocations);
                }
                else
                {
                    // Set id to null and return a view with id as null
                    id = null;
                    ViewBag.Id = id; // Update ViewBag.Id to reflect the change
                    return View(); // Return the view with id as null
                }
            }
        }

        public async Task<IActionResult> OwnedLaptopReport()
        {
            await FindAllocationCompleted();
            await FindStatus();


            var totalActiveLaptops = await _context.tbl_ictams_laptopalloc.CountAsync(x => x.AllocationStatus == "CO");

            ViewBag.TotalActiveLaptops = totalActiveLaptops;

            var assetManagementContext = _context.tbl_ictams_laptopalloc.Where(x => x.AllocationStatus == "CO")
                .Include(l => l.Status)
                .Include(l => l.Createdby)
                .Include(l => l.LaptopInventory)
                .Include(l => l.LaptopInventory.Memory.Capacity).Include(l => l.LaptopInventory.HardDisk.Capacity).Include(l => l.LaptopInventory.Level).Include(l => l.LaptopInventory.Brand).Include(l => l.LaptopInventory.OS)
                .Include(l => l.Owner)
                .Include(l => l.InventoryDetails)
                .Include(l => l.Updatedby);

            return View(await assetManagementContext.ToListAsync());
        }



        public async Task<IActionResult> LaptopBrandReport(string id, string id2, string id3, string id4)
        {
            ViewData["Brand"] = new SelectList(_context.tbl_ictams_brand, "BrandId", "BrandDescription");

            var assetManagementContext = _context.tbl_ictams_laptopinvdetails.Include(i => i.Createdby).Include(i => i.LaptopInventory).ThenInclude(i => i.Level).Include(i => i.LaptopInventory.OS).Include(i => i.LaptopInventory.Model).Include(i => i.LaptopInventory.CPU).Include(i => i.LaptopInventory.Memory.Capacity).Include(i => i.LaptopInventory).Include(i => i.Status).Include(i => i.LaptopInventory.HardDisk.Capacity)
                .Include(i => i.Updatedby)
                .Include(i => i.Vendor);

            if (id4 != null && id == null && id2 == null && id3 == null)
            {
                var assetManagementContext1 = _context.tbl_ictams_laptopinvdetails.Where(b=>b.Status.status_name == id4).Include(i => i.Createdby).Include(i => i.LaptopInventory).ThenInclude(i => i.Level).Include(i => i.LaptopInventory.OS).Include(i => i.LaptopInventory.Model).Include(i => i.LaptopInventory.CPU).Include(i => i.LaptopInventory.Memory.Capacity).Include(i => i.LaptopInventory).Include(i => i.Status).Include(i => i.LaptopInventory.HardDisk.Capacity)
                .Include(i => i.Updatedby)
                .Include(i => i.Vendor);
                return View(await assetManagementContext1.ToListAsync());
            }
            // Filter by brand if a brand ID is selected
            if (!string.IsNullOrEmpty(id))
            {
                ViewBag.Id = id;
                var totalAvailableLaptop = await _context.tbl_ictams_laptopinvdetails.CountAsync(x => x.LaptopInventory.Brand.BrandDescription == id);
                var totalAvailableLaptop2 = await _context.tbl_ictams_laptopinvdetails.CountAsync(x => x.LaptopInventory.Brand.BrandDescription == id && x.LTStatus == "CO");
                var totalAvailableLaptop3 = await _context.tbl_ictams_laptopinvdetails.CountAsync(x => x.LaptopInventory.Brand.BrandDescription == id && x.LTStatus == "AV");

                ViewBag.TotalAvailableLaptop = totalAvailableLaptop;
                ViewBag.TotalCompletedLaptop = totalAvailableLaptop2;
                ViewBag.TotalAvailableLaptop3 = totalAvailableLaptop3;

               
                if (id4 != null && id2 == null && id3 == null)
                {
                    var FassetManagementContext = assetManagementContext
                    .Where(b => b.LaptopInventory.Brand.BrandDescription == id && b.Status.status_name == id4);
                    return View(await FassetManagementContext.ToListAsync());
                }

                if (id2 == "OS" && id4 == null)
                {
                    var FassetManagementContext = assetManagementContext
                    .Where(b => b.LaptopInventory.Brand.BrandDescription == id && b.LaptopInventory.OS.OSDescription == id3);
                    return View(await FassetManagementContext.ToListAsync());
                }
                if (id2 == "OS" && id4 != null)
                {
                    var FassetManagementContext = assetManagementContext
                    .Where(b => b.LaptopInventory.Brand.BrandDescription == id && b.LaptopInventory.OS.OSDescription == id3
                    && b.Status.status_name == id4);
                    return View(await FassetManagementContext.ToListAsync());
                }
                if (id2 == "CPU" && id4 == null)
                {
                    var FassetManagementContext = assetManagementContext
                    .Where(b => b.LaptopInventory.Brand.BrandDescription == id && b.LaptopInventory.CPU.CPUDescription == id3);
                    return View(await FassetManagementContext.ToListAsync());
                }
                if (id2 == "CPU" && id4 != null)
                {
                    var FassetManagementContext = assetManagementContext
                    .Where(b => b.LaptopInventory.Brand.BrandDescription == id && b.LaptopInventory.CPU.CPUDescription == id3
                    && b.Status.status_name == id4);
                    return View(await FassetManagementContext.ToListAsync());
                }
                if (id2 == "MODEL" && id4 == null)
                {
                    var FassetManagementContext = assetManagementContext
                    .Where(b => b.LaptopInventory.Brand.BrandDescription == id && b.LaptopInventory.Model.ModelDescription == id3);
                    return View(await FassetManagementContext.ToListAsync());
                }
                if (id2 == "MODEL" && id4 != null)
                {
                    var FassetManagementContext = assetManagementContext
                    .Where(b => b.LaptopInventory.Brand.BrandDescription == id && b.LaptopInventory.Model.ModelDescription == id3 && b.Status.status_name == id4);
                    return View(await FassetManagementContext.ToListAsync());
                }
                if (id2 == "LEVEL" && id4 == null)
                {
                    var FassetManagementContext = assetManagementContext
                    .Where(b => b.LaptopInventory.Brand.BrandDescription == id && b.LaptopInventory.Level.LevelDescription == id3 );
                    return View(await FassetManagementContext.ToListAsync());
                }
                if (id2 == "LEVEL" && id4 != null)
                {
                    var FassetManagementContext = assetManagementContext
                    .Where(b => b.LaptopInventory.Brand.BrandDescription == id && b.LaptopInventory.Level.LevelDescription == id3 && b.Status.status_name == id4);
                    return View(await FassetManagementContext.ToListAsync());
                }
                if (id2 == "MEMORY" && id4 == null)
                {
                    var FassetManagementContext = assetManagementContext
                    .Where(b => b.LaptopInventory.Brand.BrandDescription == id && b.LaptopInventory.Memory.Capacity.CapacityDescription == id3);
                    return View(await FassetManagementContext.ToListAsync());
                }
                if (id2 == "MEMORY" && id4 != null)
                {
                    var FassetManagementContext = assetManagementContext
                    .Where(b => b.LaptopInventory.Brand.BrandDescription == id && b.LaptopInventory.Memory.Capacity.CapacityDescription == id3 && b.Status.status_name == id4);
                    return View(await FassetManagementContext.ToListAsync());
                }
                if (id2 == "HARDDISK" && id4 == null)
                {
                    var FassetManagementContext = assetManagementContext
                    .Where(b => b.LaptopInventory.Brand.BrandDescription == id && b.LaptopInventory.HardDisk.Capacity.CapacityDescription == id3);
                    return View(await FassetManagementContext.ToListAsync());
                }
                if (id2 == "HARDDISK" && id4 != null)
                {
                    var FassetManagementContext = assetManagementContext
                    .Where(b => b.LaptopInventory.Brand.BrandDescription == id && b.LaptopInventory.HardDisk.Capacity.CapacityDescription == id3 && b.Status.status_name == id4);
                    return View(await FassetManagementContext.ToListAsync());
                }
                if(id2 == null && id3 == null && id4 == null)
                {
                    var FassetManagementContext = assetManagementContext
                    .Where(b => b.LaptopInventory.Brand.BrandDescription == id);
                    return View(await FassetManagementContext.ToListAsync());
                }
            }

            return View(await assetManagementContext.ToListAsync());
        }



        public async Task<IActionResult> AvailableLaptopReport()
        {
            var totalAvailableLaptop = await _context.tbl_ictams_laptopinvdetails.CountAsync(x => x.LTStatus == "AV");
            ViewBag.TotalAvailableLaptop = totalAvailableLaptop;

            var assetManagementContext = _context.tbl_ictams_laptopinvdetails.Where(x=>x.LTStatus == "AV").Include(i => i.Createdby).Include(i => i.LaptopInventory).Include(l => l.LaptopInventory.Memory.Capacity).Include(l => l.LaptopInventory.HardDisk.Capacity).Include(l => l.LaptopInventory.Level).Include(l => l.LaptopInventory.Brand).Include(l => l.LaptopInventory.OS).Include(i => i.Status).Include(i => i.Updatedby).Include(i => i.Vendor);
            return View(await assetManagementContext.ToListAsync());
        }

        public async Task<IActionResult> SpareLaptopReport()
        {
            var totalDisposeLaptop = await _context.tbl_ictams_ltareturn.CountAsync(x => x.ReturnType.Return_Inv == "Y");
            ViewBag.TotalDisposeLaptop = totalDisposeLaptop;

            var assetManagementContext = _context.tbl_ictams_ltareturn.Where(x => x.ReturnType.Return_Inv == "Y").Include(r => r.LaptopAllocation).ThenInclude(l=>l.InventoryDetails).ThenInclude(s=>s.LaptopInventory).Include(l => l.LaptopAllocation.LaptopInventory.OS).Include(l => l.LaptopAllocation.LaptopInventory.Brand).Include(l => l.LaptopAllocation.LaptopInventory.Memory.Capacity).Include(l => l.LaptopAllocation.LaptopInventory.HardDisk.Capacity).Include(l => l.LaptopAllocation.LaptopInventory.Level).Include(r => r.ReturnType).Include(r => r.Status).Include(r => r.UserCreated).Include(r => r.UserUpdated);
            return View(await assetManagementContext.ToListAsync());
        }

        public async Task<IActionResult> LaptopBorrowedReport()
        {
            var totalDisposeLaptop = await _context.tbl_ictams_ltborrowed.CountAsync(x => x.StatusID == "AC");
            ViewBag.TotalDisposeLaptop = totalDisposeLaptop;

            var assetManagementContext = _context.tbl_ictams_ltborrowed.Where(x => x.StatusID == "AC").Include(l => l.LaptopInventory).Include(l=>l.LaptopInventory.Memory.Capacity).Include(l=>l.LaptopInventory.HardDisk.Capacity).Include(l=>l.LaptopInventory.Level).Include(l=>l.LaptopInventory.Brand).Include(l => l.LaptopInventory.OS).Include(l => l.Owner).Include(l => l.Status).Include(l => l.UserCreated).Include(l => l.UserUpdated);
            return View(await assetManagementContext.ToListAsync());
        }

        public async Task<IActionResult> DisposeLaptopReport()
        {
            var totalDisposeLaptop = await _context.tbl_ictams_ltareturn.CountAsync(x => x.ReturnType.Description == "DISPOSE");
            ViewBag.TotalDisposeLaptop = totalDisposeLaptop;

            var assetManagementContext = _context.tbl_ictams_ltareturn.Where(x=>x.ReturnType.Description == "DISPOSE").Include(r => r.LaptopAllocation).ThenInclude(l => l.InventoryDetails).ThenInclude(s => s.LaptopInventory).Include(l => l.LaptopAllocation.LaptopInventory.OS).Include(l => l.LaptopAllocation.LaptopInventory.Brand).Include(l => l.LaptopAllocation.LaptopInventory.Memory.Capacity).Include(l => l.LaptopAllocation.LaptopInventory.HardDisk.Capacity).Include(l => l.LaptopAllocation.LaptopInventory.Level).Include(r => r.ReturnType).Include(r => r.Status).Include(r => r.UserCreated).Include(r => r.UserUpdated);
            return View(await assetManagementContext.ToListAsync());
        }

        public async Task<IActionResult> LTNewActiveReport()
        {
            var assetManagementContext = await _context.tbl_ictams_ltnewalloc.Where(x => x.SecAllocationStatus == "AC").Include(s => s.Createdby).Include(s => s.LaptopAllocation).Include(s => s.InventoryDetails).Include(s => s.LaptopInventory).Include(l => l.LaptopInventory.OS).Include(l => l.LaptopInventory.Brand).Include(l => l.LaptopInventory.Memory.Capacity).Include(l => l.LaptopInventory.HardDisk.Capacity).Include(l => l.LaptopInventory.Level).Include(s => s.Owner).Include(s => s.Status).Include(s => s.Updatedby).ToListAsync();
            return View(assetManagementContext);
        }

        public async Task<IActionResult> ActiveLaptopsReport()
        {
            await FindStatus();
            var totalActiveLaptops = await _context.tbl_ictams_laptopalloc.CountAsync(x => x.AllocationStatus == "AC");
            var totalACtiveSec = await _context.tbl_ictams_ltnewalloc.CountAsync(x => x.SecAllocationStatus == "AC");
            ViewBag.TotalActiveLaptops = totalActiveLaptops + totalACtiveSec;

            var assetManagementContext = _context.tbl_ictams_laptopalloc.Include(l => l.Status).Include(l => l.Createdby).Include(l => l.LaptopInventory).Include(l=>l.LaptopInventory.OS).Include(l => l.LaptopInventory.Brand).Include(l => l.LaptopInventory.Memory.Capacity).Include(l => l.LaptopInventory.HardDisk.Capacity).Include(l => l.LaptopInventory.Level).Where(x => x.AllocationStatus == "AC").Include(l => l.Owner).Include(l => l.Updatedby).Include(l => l.InventoryDetails);
            return View(await assetManagementContext.ToListAsync());
        }

        public async Task<IActionResult> Inactive()
        {
            await FindStatus();


            var assetManagementContext = _context.tbl_ictams_laptopalloc.Include(l => l.Status).Include(l => l.Createdby).Include(l => l.LaptopInventory).Where(x => x.AllocationStatus == "IN").Include(l => l.Owner).Include(l => l.Updatedby).Include(l => l.InventoryDetails);
            return View(await assetManagementContext.ToListAsync());
        }

        public async Task<IActionResult> AllocationViews()
        {
            await FindStatus();


            var assetManagementContext = _context.tbl_ictams_laptopalloc.Include(l => l.Status).Include(l => l.Createdby).Include(l => l.LaptopInventory).Where(x => x.AllocationStatus == "IN").Include(l => l.Owner).Include(l => l.Updatedby);
            return View(await assetManagementContext.ToListAsync());
        }

        // GET: LaptopAllocations
        public async Task<IActionResult> Index()
        {


            await FindAllocationCompleted();
            await FindStatus();

            int? userProfile = HttpContext.Session.GetInt32("UserProfile");
            if (userProfile.HasValue)
            {

                var hasOpenAccess = await _context.tbl_ictams_profileaccess
          .AnyAsync(pa => pa.OpenAccess == "Y" &&
                          pa.Module.ModuleTitle == "Laptop Allocations" &&  // Adjust the module name as needed
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

                    // Count total laptops
                    var totalLaps = await _context.tbl_ictams_laptopinv.SumAsync(x => x.Quantity);
                    //var totalAllocLaps = await _context.tbl_ictams_laptopalloc.CountAsync(x => x.AllocationStatus == "AC");
                    //var totalNotAllocLaps = await _context.tbl_ictams_laptopinv.SumAsync(x => x.Quantity - x.AllocatedNo);
                    var totalAllocLaps = await _context.tbl_ictams_laptopinvdetails.CountAsync(x => x.LTStatus == "AC");
                    var totalNotAllocLaps = await _context.tbl_ictams_laptopinv.SumAsync(x => x.Quantity - x.AllocatedNo);

                    ViewBag.TotalLaptops = totalLaps;
                    ViewBag.TotalAllocLaptops = totalAllocLaps;
                    ViewBag.TotalNotAllocLaptops = totalNotAllocLaps;

                    var assetManagementContext = _context.tbl_ictams_laptopalloc.Where(x => x.AllocationStatus != "IN")
                        .Include(l => l.Status)
                        .Include(l => l.Createdby)
                        .Include(l => l.LaptopInventory)
                        .Include(l => l.Owner)
                        .Include(l => l.InventoryDetails)
                        .Include(l => l.Updatedby);

                    return View(await assetManagementContext.ToListAsync());
                }
            }

            return RedirectToAction("Logout", "Users");
        }

        public async Task<IActionResult> ReturnPartialView()
        {
            await FindStatus();


            var totalActiveLaptops = await _context.tbl_ictams_laptopalloc.CountAsync(x => x.AllocationStatus == "AC");
            var totalInactiveLaptops = await _context.tbl_ictams_laptopalloc.CountAsync(x => x.AllocationStatus == "IN");

            ViewBag.TotalActiveLaptops = totalActiveLaptops;
            ViewBag.TotalInactiveLaptops = totalInactiveLaptops;

            var assetManagementContext = _context.tbl_ictams_laptopalloc.Where(x => x.AllocationStatus == "AC")
                .Include(l => l.Status)
                .Include(l => l.Createdby)
                .Include(l => l.LaptopInventory)
                .Include(l => l.Owner)
                .Include(l => l.InventoryDetails)
                .Include(l => l.Updatedby);

            return View(await assetManagementContext.ToListAsync());


        }

        public async Task<IActionResult> LapAllocPartialView()
        {
            await FindStatus();


            var totalActiveLaptops = await _context.tbl_ictams_laptopalloc.CountAsync(x => x.AllocationStatus == "AC");
            var totalInactiveLaptops = await _context.tbl_ictams_laptopalloc.CountAsync(x => x.AllocationStatus == "IN");

            ViewBag.TotalActiveLaptops = totalActiveLaptops;
            ViewBag.TotalInactiveLaptops = totalInactiveLaptops;

            var assetManagementContext = _context.tbl_ictams_laptopalloc.Where(x => x.AllocationStatus == "AC")
                .Include(l => l.Status)
                .Include(l => l.Createdby)
                .Include(l => l.LaptopInventory)
                .Include(l => l.Owner)
                .Include(l => l.InventoryDetails)
                .Include(l => l.Updatedby);

            return View(await assetManagementContext.ToListAsync());


        }

        // GET: LaptopAllocations/Create
        public IActionResult Create()
        {
            var activeLaptops = _context.tbl_ictams_laptopinv
                                        .Where(l => l.LTStatus != "IN") 
                                        .ToList();

            ViewData["LaptopCode"] = new SelectList(activeLaptops, "laptoptinvCode", "laptoptinvCode");

            //ViewData["LaptopCode"] = new SelectList(_context.tbl_ictams_laptopinv, "laptoptinvCode", "laptoptinvCode"); 

            ViewData["SerialNumber"] = new SelectList(_context.tbl_ictams_laptopinvdetails, "SerialCode", "SerialCode"); 
            ViewData["AllocationStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_code");
            ViewData["AllocCreated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode");
            ViewData["OwnerCode"] = new SelectList(_context.tbl_ictams_owners, "OwnerCode", "OwnerCode");
            ViewData["AllocUpdated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode");
            return View();
        }

        // POST: LaptopAllocations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AllocId,LaptopCode,OwnerCode,SerialNumber,FixedassetTag,ComputerName,Amount,AllocationStatus,AllocCreated,DateCreated,AllocUpdated,DateUpdated")] LaptopAllocation laptopAllocation)
        {
            bool descriptionExists = await _context.tbl_ictams_laptopalloc.AnyAsync(x => x.SerialNumber == laptopAllocation.SerialNumber
            && x.AllocationStatus == "AC");
            if (descriptionExists)
            {
                TempData["ErrorMessage"] = "This serial number already exists!";
                return RedirectToAction(nameof(Index));
            }

            var availableQuantity = await _context.tbl_ictams_laptopinv
                .Where(x => x.laptoptinvCode == laptopAllocation.LaptopCode && x.LTStatus == "AV")
                .Select(x => x.Quantity)
                .FirstOrDefaultAsync();

            var allocatedQuantity = await _context.tbl_ictams_laptopalloc.Where(z=>z.AllocationStatus == "AC")
                .CountAsync(x => x.LaptopCode == laptopAllocation.LaptopCode);

            var ownerCodeExist = await _context.tbl_ictams_laptopalloc.Where(x=>x.OwnerCode == laptopAllocation.OwnerCode)
                .FirstOrDefaultAsync();


            var ownerExists = await _context.tbl_ictams_laptopalloc.AnyAsync(x => x.OwnerCode == laptopAllocation.OwnerCode && x.DateCreated >= DateTime.Now.AddYears(4) );

            var findOwnerInBorrowed = await _context.tbl_ictams_ltborrowed.Where(x => x.OwnerID == laptopAllocation.OwnerCode && x.StatusID == "AC").FirstOrDefaultAsync();

            if (findOwnerInBorrowed != null)
            {
                TempData["AlertMessage"] = "The owner borrowed a laptop. Kindly return it to reallocate!";
                return RedirectToAction("Create");
            }

            if (laptopAllocation.LaptopCode != null)
            {
                var findLaptop = await _context.tbl_ictams_laptopinvdetails
                .Where(x => x.SerialCode == laptopAllocation.SerialNumber && x.LTStatus == "AV")
                .FirstOrDefaultAsync();


                var finSerialBorrowed = await _context.tbl_ictams_ltborrowed
                   .Where(z => z.StatusID == "AC" && z.SerialNumber == findLaptop.SerialCode)
                   .FirstOrDefaultAsync();

                if (finSerialBorrowed !=  null)
                {
                    TempData["AlertMessage"] = "The available laptop has been borrowed!";
                    return RedirectToAction("Create");
                }
            }
            else
            {
                if (laptopAllocation.LaptopCode == null)
                {
                    TempData["AlertMessage"] = "The laptop cannot be null!";
                    return RedirectToAction("Create");
                }
            }

            if (LaptopAllocationExistsOwner(laptopAllocation.OwnerCode))
            {
                TempData["AlertMessage"] = "The Selected Owner is already allocated with a device or Inventory Laptop! Select another Owner to Allocate with! Need to wait after 4 years!";
                return RedirectToAction("Create");
            }

            if (laptopAllocation.OwnerCode == null)
            {
                TempData["AlertMessage"] = "Owner code cannot be null or Empty!";
                return RedirectToAction("Create");
            }


            if (allocatedQuantity < availableQuantity)
            {
                var paramCode = await _context.tbl_ictams_parameters
                    .Where(p => p.parm_code == "lta_id")
                    .MaxAsync(p => p.parm_value);
                var newparamCode = paramCode + 1;

                var param = await _context.tbl_ictams_parameters
                    .FirstOrDefaultAsync(p => p.parm_code == "lta_id");
                param.parm_value = newparamCode;
 

                var allocQuantity = await _context.tbl_ictams_laptopinv.Where(a=>a.laptoptinvCode == laptopAllocation.LaptopCode).MaxAsync(a=>a.AllocatedNo);
                var newallocQuantity = allocQuantity + 1;
                var inv_alloc = await _context.tbl_ictams_laptopinv.FirstOrDefaultAsync(a => a.laptoptinvCode == laptopAllocation.LaptopCode);
                inv_alloc.AllocatedNo = newallocQuantity;
                var inv_details = await _context.tbl_ictams_laptopinvdetails.FirstOrDefaultAsync(a => a.laptoptinvCode == laptopAllocation.LaptopCode && a.SerialCode == laptopAllocation.SerialNumber);
                inv_details.LTStatus = "AC";
                inv_details.DeployedDate = DateTime.Now;


                var ucode = HttpContext.Session.GetString("UserName");
                laptopAllocation.AllocId = "LTA" + newparamCode.ToString().PadLeft(12, '0');
                laptopAllocation.FixedassetTag = laptopAllocation.FixedassetTag.ToUpper();
                laptopAllocation.SerialNumber = laptopAllocation.SerialNumber.ToUpper();
                laptopAllocation.ComputerName = laptopAllocation.ComputerName.ToUpper();
                laptopAllocation.AllocCreated = ucode;
                laptopAllocation.DateCreated = DateTime.Now;
                laptopAllocation.DateDeployed = DateTime.Now;

                _context.Add(laptopAllocation);
                await _context.SaveChangesAsync();

                // ...
                TempData["SuccessNotification"] = "Successfully added a new allocation!";
                // ...
                return RedirectToAction(nameof(Index));
            }           
             else if(allocatedQuantity >= availableQuantity)
            {
                TempData["AlertMessage"] = "The laptop inventory you selected has been fully allocated!";
                return RedirectToAction("Create");
            }
            if (ownerCodeExist != null)
            {
                TempData["AlertMessage"] = "The Selected Owner already allocated a device!";
                return RedirectToAction("Create");
            }
            



            return RedirectToAction(nameof(Index));



        }

        // GET: LaptopAllocations/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
 
            ViewData["AllocationStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_code");
            ViewData["AllocCreated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode");
            ViewData["LaptopCode"] = new SelectList(_context.tbl_ictams_laptopinv, "laptoptinvCode", "laptoptinvCode");
            ViewData["OwnerCode"] = new SelectList(_context.tbl_ictams_owners, "OwnerCode", "OwnerCode");
            ViewData["AllocUpdated"] = new SelectList(_context.tbl_ictams_users, "UserCode", "UserCode");
            if (id == null || _context.tbl_ictams_laptopalloc == null)
            {
                return NotFound();
            }

            var laptopAllocation = await _context.tbl_ictams_laptopalloc.FindAsync(id);
            if (laptopAllocation == null)
            {
                return NotFound();
            }
            return View(laptopAllocation);
        }

        // POST: LaptopAllocations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("AllocId,LaptopCode,OwnerCode,SerialNumber,FixedassetTag,ComputerName,DatePurchase,AgeYears,PoNumber,Amount,AllocationStatus,AllocCreated,DateCreated,AllocUpdated,DateUpdated")] LaptopAllocation laptopAllocation)
        {
            if (id != laptopAllocation.AllocId)
            {
                return NotFound();
            }

                try
                {
                    var ucode = HttpContext.Session.GetString("UserName");
                    laptopAllocation.AllocUpdated = ucode;
                    laptopAllocation.DateUpdated = DateTime.Now;
                    _context.Update(laptopAllocation);
                    await _context.SaveChangesAsync();
                // ...
                TempData["SuccessNotification"] = "Successfully edited!";
                // ...
            }
            catch (DbUpdateConcurrencyException)
                {
                    if (!LaptopAllocationExists(laptopAllocation.AllocId))
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

        //DELETE
        public async Task<IActionResult> DeleteAsEdit(string[] selectedIds)
        {
            ViewData["UserStatus"] = new SelectList(_context.tbl_ictams_status, "status_code", "status_name");
            foreach (string id in selectedIds)
            {
            var ucode = HttpContext.Session.GetString("UserName");

                var allocationID = await _context.tbl_ictams_laptopalloc.FindAsync(id);
                if (allocationID != null)
                {
                    // Check if the selected profile status exists in the tbl_lsm_status table
                    var status = await _context.tbl_ictams_status.FindAsync(allocationID.AllocationStatus);
                    if (status == null)
                    {
                        ModelState.AddModelError("", $"Profile status '{allocationID.AllocId}' does not exist.");
                    }

                    // Find the corresponding LaptopCode in tbl_ictams_laptopinv and deduct 1 from AllocatedNo
                    var laptopCode = allocationID.LaptopCode;
                    var laptopInv = await _context.tbl_ictams_laptopinv.FirstOrDefaultAsync(a => a.laptoptinvCode == laptopCode);
                    if (laptopInv != null)
                    {
                        laptopInv.AllocatedNo -= 1;
                    }
                    var InvDetails = await _context.tbl_ictams_laptopinvdetails.FirstOrDefaultAsync(a => a.SerialCode == allocationID.SerialNumber);
                    InvDetails.UpdatedDate = DateTime.Now;
                    InvDetails.Updated = ucode;
                    InvDetails.LTStatus = "AV";

                    // Update the profile
                    allocationID.AllocUpdated = ucode;
                    allocationID.AllocationStatus = "IN";
                    allocationID.DateUpdated = DateTime.Now;

                }

                _context.Update(allocationID);
                await _context.SaveChangesAsync();
                // ...
                TempData["SuccessNotification"] = "Successfully deleted!";
                // ...
            }
            return RedirectToAction(nameof(Index));
        }

        private bool LaptopAllocationExists(string id)
        {
          return (_context.tbl_ictams_laptopalloc?.Any(e => e.AllocId == id)).GetValueOrDefault();
        }

        private bool LaptopAllocationExistsOwner(string id)
        {
            return (_context.tbl_ictams_laptopalloc?.Any(e => e.OwnerCode == id && e.AllocationStatus == "AC")).GetValueOrDefault();
        }

        private bool LaptopAllocationExistsComputerName(string id)
        {
            return (_context.tbl_ictams_laptopalloc?.Any(e => e.ComputerName == id && e.AllocationStatus == "AC")).GetValueOrDefault();
        }
    }
}
