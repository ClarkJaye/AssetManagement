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
    public class GraphicsCardsController : BaseController
    {
        private readonly AssetManagementContext _context;

        public GraphicsCardsController(AssetManagementContext context)
             : base(context)
        {
            _context = context;
        }

        public async Task<IActionResult> GraphicsPartialView()
        {

            var myData = HttpContext.Session.GetString("name");
            var lSM_PNContext = _context.tbl_ictams_graphicscard.Where(Brand => Brand.GraphicsStatus == "AC");
            return View(await lSM_PNContext.ToListAsync());
        }

        // GET: GraphicsCards
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
                    var lSM_PNContext = _context.tbl_ictams_graphicscard.Where(Brand => Brand.GraphicsStatus == "AC");
                    return View(await lSM_PNContext.ToListAsync());
                }
            }
            return View();
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
                    var lSM_PNContext = _context.tbl_ictams_graphicscard.Where(Brand => Brand.GraphicsStatus == "IN");
                    return View(await lSM_PNContext.ToListAsync());
                }
            }
            return View();
        }

        public async Task<IActionResult> Activate(int? id)
        {
            var userr = HttpContext.Session.GetString("name");
            var graphicsCard = await _context.tbl_ictams_graphicscard.FindAsync(id);
            if (graphicsCard == null)
            {
                return NotFound();
            }
            graphicsCard.GraphicsUpdatedBy = userr;
            graphicsCard.GraphicsUpdatedDate = DateTime.Now;
            graphicsCard.GraphicsStatus = "AC"; // Set the status to "Active"

            _context.Entry(graphicsCard).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully retrieve a retrieve graphics card!";
            // ...
            return RedirectToAction(nameof(Index));
        }


        // GET: GraphicsCards/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.tbl_ictams_graphicscard == null)
            {
                return NotFound();
            }

            var graphicsCard = await _context.tbl_ictams_graphicscard
                .FirstOrDefaultAsync(m => m.GraphicsID == id);
            if (graphicsCard == null)
            {
                return NotFound();
            }

            return View(graphicsCard);
        }

        // GET: GraphicsCards/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GraphicsCards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GraphicsID,GraphicsDescription,GraphicsStatus,GraphicsCreatedBy,GraphicsCreatedDate,GraphicsUpdatedBy,GraphicsUpdatedDate")] GraphicsCard graphicsCard)
        {
            var userrr = HttpContext.Session.GetString("name");
            bool descriptionExists = await _context.tbl_ictams_graphicscard.AnyAsync(x => x.GraphicsDescription == graphicsCard.GraphicsDescription);
            if (descriptionExists)
            {
                TempData["ErrorMessage"] = "Description already exists. Please enter a different description!";
                return RedirectToAction(nameof(Index));
            }

            var paramCode = await _context.tbl_ictams_parameters.Where(p => p.parm_code == "graphic_id").MaxAsync(p => p.parm_value);
            var newparamCode = paramCode + 1;

            var param = await _context.tbl_ictams_parameters.FirstOrDefaultAsync(p => p.parm_code == "graphic_id");
            param.parm_value = newparamCode;

            graphicsCard.GraphicsDescription = graphicsCard.GraphicsDescription.ToUpper();
            graphicsCard.GraphicsStatus = "AC";
            graphicsCard.GraphicsID = newparamCode;
            graphicsCard.GraphicsCreatedDate = DateTime.Now;
            graphicsCard.GraphicsCreatedBy = userrr;

            _context.Add(graphicsCard);
                await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully added a new main board!";
            // ...
            return RedirectToAction(nameof(Index));
            
        }

        // GET: GraphicsCards/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.tbl_ictams_graphicscard == null)
            {
                return NotFound();
            }

            var graphicsCard = await _context.tbl_ictams_graphicscard.FindAsync(id);
            if (graphicsCard == null)
            {
                return NotFound();
            }
            return View(graphicsCard);
        }

        // POST: GraphicsCards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GraphicsID,GraphicsDescription,GraphicsStatus,GraphicsCreatedBy,GraphicsCreatedDate,GraphicsUpdatedBy,GraphicsUpdatedDate")] GraphicsCard graphicsCard)
        {
            var userrr = HttpContext.Session.GetString("name");
            bool descriptionExists = await _context.tbl_ictams_graphicscard.AnyAsync(x => x.GraphicsDescription == graphicsCard.GraphicsDescription);
            if (descriptionExists)
            {
                TempData["ErrorMessage"] = "Description already exists. Please enter a different description!";
                return RedirectToAction(nameof(Index));
            }

 
                try
                {
                    graphicsCard.GraphicsDescription = graphicsCard.GraphicsDescription.ToUpper();
                    graphicsCard.GraphicsUpdatedBy = userrr;
                    graphicsCard.GraphicsUpdatedDate = DateTime.Now;
                    _context.Update(graphicsCard);
                    await _context.SaveChangesAsync();
                    // ...
                    TempData["SuccessNotification"] = "Successfully edit a  graphics card!";
                    return RedirectToAction(nameof(Index));
                        // ...
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GraphicsCardExists(graphicsCard.GraphicsID))
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

        // GET: GraphicsCards/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.tbl_ictams_graphicscard == null)
            {
                return NotFound();
            }

            var graphicsCard = await _context.tbl_ictams_graphicscard
                .FirstOrDefaultAsync(m => m.GraphicsID == id);
            if (graphicsCard == null)
            {
                return NotFound();
            }

            return View(graphicsCard);
        }

        // POST: GraphicsCards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //var findUse = await _context.tbl_ictams_graphicscard.Where(x => x.GraphicsID == id).FirstOrDefaultAsync();
            //if (findUse != null)
            //{
            //    TempData["AlertMessage"] = "Cannot be deleted. It is already in use!";
            //    return RedirectToAction(nameof(Index));
            //}




            var userrr = HttpContext.Session.GetString("name");
            if (_context.tbl_ictams_graphicscard == null)
            {
                return Problem("Entity set 'AssetManagementContext.Brand'  is null.");
            }
            var graphicsCard = await _context.tbl_ictams_graphicscard.FindAsync(id);
            if (graphicsCard != null)
            {
                graphicsCard.GraphicsStatus = "IN";
                graphicsCard.GraphicsUpdatedBy = userrr;
                graphicsCard.GraphicsUpdatedDate = DateTime.Now;
                _context.tbl_ictams_graphicscard.Update(graphicsCard);
            }

            await _context.SaveChangesAsync();
            // ...
            TempData["SuccessNotification"] = "Successfully delete a  graphics card!";
            return RedirectToAction(nameof(Index));
            // ...
        }

        private bool GraphicsCardExists(int id)
        {
          return (_context.tbl_ictams_graphicscard?.Any(e => e.GraphicsID == id)).GetValueOrDefault();
        }
    }
}
