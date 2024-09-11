using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Data;
using AssetManagement.Models;
using System.Drawing.Drawing2D;
using AssetManagement.Utility;

namespace AssetManagement.Controllers
{
    public class ModelssController : BaseController
    {
        private readonly AssetManagementContext _context;

        public ModelssController(AssetManagementContext context)
        : base(context) // Pass the 'context' dependency to the base constructor
        {
            _context = context;
        }


        public async Task<IActionResult> ModelPartialView()
        {   
            var myData = HttpContext.Session.GetString("name");
            var lSM_PNContext = _context.tbl_ictams_model.Where(Model => Model.ModelStatus == "AC");
            return View(await lSM_PNContext.ToListAsync());
        }


        // GET: Models
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
                          pa.Module.ModuleTitle == "Modelss" &&  // Adjust the module name as needed
                          pa.ProfileId == userProfile.Value);
                if (!hasOpenAccess)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var myData = HttpContext.Session.GetString("name");
                    var lSM_PNContext = _context.tbl_ictams_model.Where(Model => Model.ModelStatus == "AC");
                    return View(await lSM_PNContext.ToListAsync());
                }
            }

            return RedirectToAction("Index", "Home");

           
        }

        // GET: Models/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.tbl_ictams_model == null)
            {
                return NotFound();
            }

            var model = await _context.tbl_ictams_model
                .FirstOrDefaultAsync(m => m.ModelId == id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }
        public IActionResult InactiveModel()
        {
            var myData1 = HttpContext.Session.GetString("name");
            ViewBag.showprofile = myData1;
            var myData = HttpContext.Session.GetString("name");
            ViewBag.showprofile = myData;
            var inactiveModel = _context.tbl_ictams_model
                .Where(model => model.ModelStatus == "IN")
                .ToList();
            return View(inactiveModel);
        }
        public async Task<IActionResult> Activate(int? id)
        {
            var userr = HttpContext.Session.GetString("name");
            var model = await _context.tbl_ictams_model.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            model.ModelUpdatedBy = userr;
            model.ModelUpdatedDate = DateTime.Now;
            model.ModelStatus = "AC"; // Set the status to "Active"

            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // ...
            TempData["SuccessNotification"] = "Successfully retrieve a deleted laptop model!";
            // ...
            return RedirectToAction(nameof(Index));
        }

        // GET: Models/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Models/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ModelId,ModelDescription,ModelStatus,ModelCreatedBy,ModelCreatedDate,ModelUpdatedBy,ModelUpdatedDate")] Model model)
        {
            var userrr = HttpContext.Session.GetString("name");
            bool descriptionExists = await _context.tbl_ictams_model.AnyAsync(x => x.ModelDescription == model.ModelDescription);
            if (descriptionExists)
            {
                TempData["ErrorMessage"] = "Description already exists. Please enter a different description!";
                return RedirectToAction(nameof(Index));
            }

            var paramCode = await _context.tbl_ictams_parameters.Where(p => p.parm_code == "model_id").MaxAsync(p => p.parm_value);
            var newparamCode = paramCode + 1;
            var param = await _context.tbl_ictams_parameters.FirstOrDefaultAsync(p => p.parm_code == "model_id");
            param.parm_value = newparamCode;

            model.ModelDescription = model.ModelDescription.ToUpper();
            model.ModelStatus = "AC";
            model.ModelId = newparamCode;
            model.ModelCreatedDate = DateTime.Now;
            model.ModelCreatedBy = userrr;
            _context.Add(model);
            await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully added a new laptop model!";
            // ...
            return RedirectToAction(nameof(Index));

        }

        // GET: Models/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.tbl_ictams_model == null)
            {
                return NotFound();
            }

            var model = await _context.tbl_ictams_model.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        // POST: Models/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ModelId,ModelDescription,ModelStatus,ModelCreatedBy,ModelCreatedDate,ModelUpdatedBy,ModelUpdatedDate")] Model model)
        {
            var userrr = HttpContext.Session.GetString("name");
            bool descriptionExists = await _context.tbl_ictams_model.AnyAsync(x => x.ModelDescription == model.ModelDescription);
            if (descriptionExists)
            {
                TempData["ErrorMessage"] = "Description already exists. Please enter a different description!";
                return RedirectToAction(nameof(Index));
            }


            if (ModelState.IsValid)
            {
                try
                {
                    model.ModelDescription = model.ModelDescription.ToUpper();
                    model.ModelUpdatedBy = userrr;
                    model.ModelUpdatedDate = DateTime.Now;
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                    // ...
                    TempData["SuccessNotification"] = "Successfully edit a laptop model!";
                    // ...
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModelExists(model.ModelId))
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
            return View(model);
        }


        // GET: Models/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.tbl_ictams_model == null)
            {
                return NotFound();
            }

            var model = await _context.tbl_ictams_model
                .FirstOrDefaultAsync(m => m.ModelId == id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        // POST: Models/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var findUse = await _context.tbl_ictams_laptopinv.Where(x => x.LTModel == id).FirstOrDefaultAsync();
            if (findUse != null)
            {
                TempData["AlertMessage"] = "Cannot be deleted. It is already in use!";
                return RedirectToAction(nameof(Index));
            }

            var userrr = HttpContext.Session.GetString("name");
            if (_context.tbl_ictams_model == null)
            {
                return Problem("Entity set 'AssetManagementContext.Model'  is null.");
            }
            var brand = await _context.tbl_ictams_model.FindAsync(id);
            if (brand != null)
            {
                brand.ModelStatus = "IN";
                brand.ModelUpdatedBy = userrr;
                brand.ModelUpdatedDate = DateTime.Now;
                _context.tbl_ictams_model.Update(brand);
            }

            await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully delete a laptop model!";
            // ...
            return RedirectToAction(nameof(Index));
        }

        private bool ModelExists(int id)
        {
            return (_context.tbl_ictams_model?.Any(e => e.ModelId == id)).GetValueOrDefault();
        }
    }
}