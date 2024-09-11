using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Data;
using AssetManagement.Models;
using System.Security.Policy;
using AssetManagement.Utility;

namespace AssetManagement.Controllers
{
    public class BrandsController : BaseController
    {
        private readonly AssetManagementContext _context;

        public BrandsController(AssetManagementContext context)
        : base(context) // Pass the 'context' dependency to the base constructor
        {
            _context = context;
        }

        public async Task<IActionResult> BrandPartialView()
        {
           
            var myData = HttpContext.Session.GetString("name");
            var lSM_PNContext = _context.tbl_ictams_brand.Where(Brand => Brand.BrandStatus == "AC");
            return View(await lSM_PNContext.ToListAsync());
        }


        // GET: Brands
        public async Task<IActionResult> Index()
        {
            int? userProfile = HttpContext.Session.GetInt32("UserProfile");
            if (userProfile.HasValue)
            {

                var hasOpenAccess = await _context.tbl_ictams_profileaccess
          .AnyAsync(pa => pa.OpenAccess == "Y" &&
                          pa.Module.ModuleTitle == "Brands" &&  // Adjust the module name as needed
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

                    var myData = HttpContext.Session.GetString("name");
                    var lSM_PNContext = _context.tbl_ictams_brand.Where(Brand => Brand.BrandStatus == "AC");
                    return View(await lSM_PNContext.ToListAsync());
                }
            }

            return RedirectToAction("Index", "Home");
            
        }

        // GET: Brands/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.tbl_ictams_brand == null)
            {
                return NotFound();
            }

            var brand = await _context.tbl_ictams_brand
                .FirstOrDefaultAsync(m => m.BrandId == id);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }
        public IActionResult InactiveBrands()
        {
            var myData = HttpContext.Session.GetString("name");
            ViewBag.showprofile = myData;
            var myData1= HttpContext.Session.GetString("name");
            ViewBag.showprofile = myData1;
            var inactiveBrands = _context.tbl_ictams_brand
                .Where(brand =>brand.BrandStatus == "IN")
                .ToList();
            return View(inactiveBrands);
        }
        public async Task<IActionResult> Activate(int? id)
        {
            var userr = HttpContext.Session.GetString("name");
            var brand = await _context.tbl_ictams_brand.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }
            brand.BrandUpdatedBy = userr;
            brand.BrandUpdatedDate = DateTime.Now;
            brand.BrandStatus = "AC"; // Set the status to "Active"

            _context.Entry(brand).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // ...
            TempData["SuccessNotification"] = "Successfully retrieve a retrieve graphics card!";
            // ...
            return RedirectToAction(nameof(Index));
        }

        // GET: Brands/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Brands/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BrandId,BrandDescription,BrandStatus,BrandCreatedBy,BrandCreatedDate,BrandUpdatedBy,BrandUpdatedDate")] Brand brand)
        {
            var userrr = HttpContext.Session.GetString("name");
            bool descriptionExists = await _context.tbl_ictams_brand.AnyAsync(x => x.BrandDescription == brand.BrandDescription);
            if (descriptionExists)
            {
                TempData["ErrorMessage"] = "Description already exists. Please enter a different description!";
                return RedirectToAction(nameof(Index));
            }
            var paramCode = await _context.tbl_ictams_parameters.Where(p => p.parm_code == "brand_id").MaxAsync(p => p.parm_value);
                var newparamCode = paramCode + 1;

                var param = await _context.tbl_ictams_parameters.FirstOrDefaultAsync(p => p.parm_code == "brand_id");
                param.parm_value = newparamCode;



                brand.BrandDescription = brand.BrandDescription.ToUpper();
                brand.BrandStatus = "AC";
                brand.BrandId = newparamCode;
                brand.BrandCreatedDate = DateTime.Now;
                brand.BrandCreatedBy = userrr;
                _context.Add(brand);
                await _context.SaveChangesAsync();
                // ...
                TempData["SuccessNotification"] = "Successfully added a new brand!";
                // ...
                return RedirectToAction(nameof(Index));
       
        }

        // GET: Brands/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.tbl_ictams_brand == null)
            {
                return NotFound();
            }

            var brand = await _context.tbl_ictams_brand.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }

        // POST: Brands/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BrandId,BrandDescription,BrandStatus,BrandCreatedBy,BrandCreatedDate,BrandUpdatedBy,BrandUpdatedDate")] Brand brand)
        {
            var userrr = HttpContext.Session.GetString("name");
            bool descriptionExists = await _context.tbl_ictams_brand.AnyAsync(x => x.BrandDescription == brand.BrandDescription);
            if (descriptionExists)
            {
                TempData["ErrorMessage"] = "Description already exists. Please enter a different description!";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    brand.BrandDescription = brand.BrandDescription.ToUpper();
                    brand.BrandUpdatedBy = userrr;
                    brand.BrandUpdatedDate = DateTime.Now;
                    _context.Update(brand);
                    await _context.SaveChangesAsync();
                    // ...
                    TempData["SuccessNotification"] = "Successfully edit a  brand!";
                    return RedirectToAction(nameof(Index));
                    // ...
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BrandExists(brand.BrandId))
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
            return View(brand);
        }

        // GET: Brands/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.tbl_ictams_brand == null)
            {
                return NotFound();
            }

            var brand = await _context.tbl_ictams_brand
                .FirstOrDefaultAsync(m => m.BrandId == id);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        // POST: Brands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var findUse = await _context.tbl_ictams_laptopinv.Where(x => x.LTBrand == id).FirstOrDefaultAsync();
            if (findUse != null)
            {
                TempData["AlertMessage"] = "Cannot be deleted. It is already in use!";
                return RedirectToAction(nameof(Index));
            }


            var userrr = HttpContext.Session.GetString("name");
            if (_context.tbl_ictams_brand == null)
            {
                return Problem("Entity set 'AssetManagementContext.Brand'  is null.");
            }
            var brand = await _context.tbl_ictams_brand.FindAsync(id);
            if (brand != null)
            {
                brand.BrandStatus = "IN";
                brand.BrandUpdatedBy =userrr;
                brand.BrandUpdatedDate = DateTime.Now;
                _context.tbl_ictams_brand.Update(brand);
            }
            
            await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully delete a  brand!";
            return RedirectToAction(nameof(Index));
            // ...
            return RedirectToAction(nameof(Index));
        }

        private bool BrandExists(int id)
        {
          return (_context.tbl_ictams_brand?.Any(e => e.BrandId == id)).GetValueOrDefault();
        }
    }
}
