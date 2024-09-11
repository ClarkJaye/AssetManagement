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
    public class MainBoardsController : BaseController
    {
        private readonly AssetManagementContext _context;

        public MainBoardsController(AssetManagementContext context)
             : base(context)
        {
            _context = context;
        }

        // GET: MainBoards
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
                          pa.Module.ModuleTitle == "Brands" &&  // Adjust the module name as needed
                          pa.ProfileId == userProfile.Value);
                if (!hasOpenAccess)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var myData = HttpContext.Session.GetString("name");
                    var lSM_PNContext = _context.tbl_ictams_mainboard.Where(Brand => Brand.BoardStatus == "AC");
                    return View(await lSM_PNContext.ToListAsync());
                }
            }
            return View();
           
        }

        public async Task<IActionResult> MainBoardPartialView()
        {
            var myData = HttpContext.Session.GetString("name");
            var lSM_PNContext = _context.tbl_ictams_mainboard.Where(Brand => Brand.BoardStatus == "AC");
            return View(await lSM_PNContext.ToListAsync());
        }

        public async Task<IActionResult> Inactive()
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
                    var myData = HttpContext.Session.GetString("name");
                    var lSM_PNContext = _context.tbl_ictams_mainboard.Where(Brand => Brand.BoardStatus == "IN");
                    return View(await lSM_PNContext.ToListAsync());
                }
            }
            return View();

        }

        public async Task<IActionResult> Activate(int? id)
        {
            var userr = HttpContext.Session.GetString("name");
            var mainBoard = await _context.tbl_ictams_mainboard.FindAsync(id);
            if (mainBoard == null)
            {
                return NotFound();
            }
            mainBoard.BoardUpdatedBy = userr;
            mainBoard.BoardUpdatedDate = DateTime.Now;
            mainBoard.BoardStatus = "AC"; // Set the status to "Active"

            _context.Entry(mainBoard).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully retrieve a retrieve main board!";
            // ...
            return RedirectToAction(nameof(Index));
        }

        // GET: MainBoards/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.tbl_ictams_mainboard == null)
            {
                return NotFound();
            }

            var mainBoard = await _context.tbl_ictams_mainboard
                .FirstOrDefaultAsync(m => m.BoardID == id);
            if (mainBoard == null)
            {
                return NotFound();
            }

            return View(mainBoard);
        }

        // GET: MainBoards/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MainBoards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BoardID,BoardDescription,BoardStatus,BoardCreatedBy,BoardCreatedDate,BoardUpdatedBy,BoardUpdatedDate")] MainBoard mainBoard)
        {
            var userrr = HttpContext.Session.GetString("name");
            bool descriptionExists = await _context.tbl_ictams_mainboard.AnyAsync(x => x.BoardDescription == mainBoard.BoardDescription);
            if (descriptionExists)
            {
                TempData["ErrorMessage"] = "Description already exists. Please enter a different description!";
                return RedirectToAction(nameof(Index));
            }

            var paramCode = await _context.tbl_ictams_parameters.Where(p => p.parm_code == "mobo_id").MaxAsync(p => p.parm_value);
            var newparamCode = paramCode + 1;

            var param = await _context.tbl_ictams_parameters.FirstOrDefaultAsync(p => p.parm_code == "mobo_id");
            param.parm_value = newparamCode;



            mainBoard.BoardDescription = mainBoard.BoardDescription.ToUpper();
            mainBoard.BoardStatus = "AC";
            mainBoard.BoardID = newparamCode;
            mainBoard.BoardCreatedDate = DateTime.Now;
            mainBoard.BoardCreatedBy = userrr;

            _context.Add(mainBoard);
                await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully added a new main board!";
            // ...
            return RedirectToAction(nameof(Index));
            
        }

        // GET: MainBoards/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.tbl_ictams_mainboard == null)
            {
                return NotFound();
            }

            var mainBoard = await _context.tbl_ictams_mainboard.FindAsync(id);
            if (mainBoard == null)
            {
                return NotFound();
            }
            return View(mainBoard);
        }

        // POST: MainBoards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BoardID,BoardDescription,BoardStatus,BoardCreatedBy,BoardCreatedDate,BoardUpdatedBy,BoardUpdatedDate")] MainBoard mainBoard)
        {
            var userrr = HttpContext.Session.GetString("name");
            bool descriptionExists = await _context.tbl_ictams_mainboard.AnyAsync(x => x.BoardDescription == mainBoard.BoardDescription);
            if (descriptionExists)
            {
                TempData["ErrorMessage"] = "Description already exists. Please enter a different description!";
                return RedirectToAction(nameof(Index));
            }

                try
                {
                    mainBoard.BoardDescription = mainBoard.BoardDescription.ToUpper();
                    mainBoard.BoardUpdatedBy = userrr;
                    mainBoard.BoardUpdatedDate = DateTime.Now;
                    _context.Update(mainBoard);
                    await _context.SaveChangesAsync();
                    // ...
                    TempData["SuccessNotification"] = "Successfully edit a  main board!";
                    return RedirectToAction(nameof(Index));
                    // ...
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MainBoardExists(mainBoard.BoardID))
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

        // GET: MainBoards/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.tbl_ictams_mainboard == null)
            {
                return NotFound();
            }

            var mainBoard = await _context.tbl_ictams_mainboard
                .FirstOrDefaultAsync(m => m.BoardID == id);
            if (mainBoard == null)
            {
                return NotFound();
            }

            return View(mainBoard);
        }

        // POST: MainBoards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //var findUse = await _context.tbl_ictams_mainboard.Where(x => x.BoardID == id).FirstOrDefaultAsync();
            //if (findUse != null)
            //{
            //    TempData["AlertMessage"] = "Cannot be deleted. It is already in use!";
            //    return RedirectToAction(nameof(Index));
            //}

            var userrr = HttpContext.Session.GetString("name");
            if (_context.tbl_ictams_mainboard == null)
            {
                return Problem("Entity set 'AssetManagementContext.Brand'  is null.");
            }
            var mainBoard = await _context.tbl_ictams_mainboard.FindAsync(id);
            if (mainBoard != null)
            {
                mainBoard.BoardStatus = "IN";
                mainBoard.BoardUpdatedBy = userrr;
                mainBoard.BoardUpdatedDate = DateTime.Now;
                _context.tbl_ictams_mainboard.Update(mainBoard);
            }

            await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully delete a  main board!";
            return RedirectToAction(nameof(Index));
            // ...

        }

        private bool MainBoardExists(int id)
        {
          return (_context.tbl_ictams_mainboard?.Any(e => e.BoardID == id)).GetValueOrDefault();
        }
    }
}
