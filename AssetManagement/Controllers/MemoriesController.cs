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
    public class MemoriesController : BaseController
    {
        private readonly AssetManagementContext _context;

        public MemoriesController(AssetManagementContext context)
        : base(context) // Pass the 'context' dependency to the base constructor
        {
            _context = context;
        }

        public async Task<IActionResult> MemoryPartialView()
        {   
            var myData = HttpContext.Session.GetString("name");
            var lSM_PNContext = _context.tbl_ictams_memory.Where(Memory => Memory.MemoryStatus == "AC").Include(CA => CA.Capacity);
            return View(await lSM_PNContext.ToListAsync());
        }

        // GET: Memories
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
                          pa.Module.ModuleTitle == "Memories" &&  // Adjust the module name as needed
                          pa.ProfileId == userProfile.Value);
                if (!hasOpenAccess)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {

                    var myData = HttpContext.Session.GetString("name");
                    var lSM_PNContext = _context.tbl_ictams_memory.Where(Memory => Memory.MemoryStatus == "AC").Include(CA => CA.Capacity);
                    return View(await lSM_PNContext.ToListAsync());
                }
            }

            return RedirectToAction("Index", "Home");

        }
        public IActionResult InactiveMemory()
        {
            var myData1 = HttpContext.Session.GetString("name");
            ViewBag.showprofile = myData1;
            var myData = HttpContext.Session.GetString("name");
            ViewBag.showprofile = myData;
            var inactiveMemory = _context.tbl_ictams_memory
                .Where(memory => memory.MemoryStatus == "IN")
                .ToList();
            return View(inactiveMemory);
        }
        public async Task<IActionResult> Activate(int? id)
        {
            var userr = HttpContext.Session.GetString("name");
            var memory = await _context.tbl_ictams_memory.FindAsync(id);
            if (memory == null)
            {
                return NotFound();
            }
            memory.MemoryUpdatedBy = userr;
            memory.MemoryUpdatedDate = DateTime.Now;
            memory.MemoryStatus = "AC"; // Set the status to "Active"

            _context.Entry(memory).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // ...
            TempData["SuccessNotification"] = "Successfully retrieve a deleted memory!";
            // ...
            return RedirectToAction(nameof(Index));
        }

        // GET: Memories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.tbl_ictams_memory == null)
            {
                return NotFound();
            }

            var memory = await _context.tbl_ictams_memory
                .FirstOrDefaultAsync(m => m.MemoryId == id);
            if (memory == null)
            {
                return NotFound();
            }

            return View(memory);
        }

        // GET: Memories/Create
        public IActionResult Create()
        {
            ViewData["MemoryCapacity"] = new SelectList(_context.tbl_ictams_capacity, "CapacityId", "CapacityDescription");
            return View();
        }

        // POST: Memories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MemoryId,MemoryDescription,MemoryCapacity,MemoryStatus,MemoryCreatedBy,MemoryCreatedDate,MemoryUpdatedBy,MemoryUpdatedDate")] Memory memory)
        {
            var userrr = HttpContext.Session.GetString("name");

            bool descriptionExists = await _context.tbl_ictams_memory.AnyAsync(x => x.MemoryDescription == memory.MemoryDescription);
            if (descriptionExists)
            {
                TempData["ErrorMessage"] = "Description already exists. Please enter a different description!";
                return RedirectToAction(nameof(Index));
            }

            var paramCode = await _context.tbl_ictams_parameters.Where(p => p.parm_code == "memo_id").MaxAsync(p => p.parm_value);
            var newparamCode = paramCode + 1;

            var param = await _context.tbl_ictams_parameters.FirstOrDefaultAsync(p => p.parm_code == "memo_id");
            param.parm_value = newparamCode;

            memory.MemoryDescription = memory.MemoryDescription.ToUpper();
            memory.MemoryStatus = "AC";
            memory.MemoryId = newparamCode;
            memory.MemoryCreatedDate = DateTime.Now;
            memory.MemoryCreatedBy = userrr;
            _context.Add(memory);
                await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully added a new memory!";
            // ...
            return RedirectToAction(nameof(Index));

        }

        // GET: Memories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["MemoryCapacity"] = new SelectList(_context.tbl_ictams_capacity, "CapacityId", "CapacityDescription");

            if (id == null || _context.tbl_ictams_memory == null)
            {
                return NotFound();
            }

            var memory = await _context.tbl_ictams_memory.FindAsync(id);
            if (memory == null)
            {
                return NotFound();
            }
            return View(memory);

          
        }

        // POST: Memories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MemoryId,MemoryDescription,MemoryCapacity,MemoryStatus,MemoryCreatedBy,MemoryCreatedDate,MemoryUpdatedBy,MemoryUpdatedDate")] Memory memory)
        {
            var userrr = HttpContext.Session.GetString("name");
            bool descriptionExists = await _context.tbl_ictams_memory.AnyAsync(x => x.MemoryDescription == memory.MemoryDescription);
            if (descriptionExists)
            {
                TempData["ErrorMessage"] = "Description already exists. Please enter a different description!";
                return RedirectToAction(nameof(Index));
            }
            if (ModelState.IsValid)
            {
                try
                {
                    memory.MemoryDescription = memory.MemoryDescription.ToUpper();
                    memory.MemoryUpdatedBy = userrr;
                    memory.MemoryUpdatedDate = DateTime.Now;
                    _context.Update(memory);
                    await _context.SaveChangesAsync();
                    // ...
                    TempData["SuccessNotification"] = "Successfully edit a memory!";
                    // ...
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MemoryExists(memory.MemoryId))
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
            return View(memory);
        }

        // GET: Memories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.tbl_ictams_memory == null)
            {
                return NotFound();
            }

            var memory = await _context.tbl_ictams_memory
                .FirstOrDefaultAsync(m => m.MemoryId == id);
            if (memory == null)
            {
                return NotFound();
            }

            return View(memory);
        }

        // POST: Memories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var findUse = await _context.tbl_ictams_laptopinv.Where(x => x.LTMemory == id).FirstOrDefaultAsync();
            if (findUse != null)
            {
                TempData["AlertMessage"] = "Cannot be deleted. It is already in use!";
                return RedirectToAction(nameof(Index));
            }



            var userrr = HttpContext.Session.GetString("name");
            if (_context.tbl_ictams_memory == null)
            {
                return Problem("Entity set 'AssetManagementContext.tbl_ictams_memory'  is null.");
            }
            var memory = await _context.tbl_ictams_memory.FindAsync(id);
            if (memory != null)
            {
                memory.MemoryStatus = "IN";
                memory.MemoryUpdatedBy = userrr;
                memory.MemoryUpdatedDate = DateTime.Now;
                _context.tbl_ictams_memory.Update(memory);
            }
            
            await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully delete a memory!";
            // ...
            return RedirectToAction(nameof(Index));
        }

        private bool MemoryExists(int id)
        {
          return (_context.tbl_ictams_memory?.Any(e => e.MemoryId == id)).GetValueOrDefault();
        }
    }
}
