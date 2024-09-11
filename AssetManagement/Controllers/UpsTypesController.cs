using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Data;
using AssetManagement.Models;

namespace AssetManagement.Controllers
{
    public class UpsTypesController : Controller
    {
        private readonly AssetManagementContext _context;

        public UpsTypesController(AssetManagementContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> UpsTypesPartialView()
        {

            var myData = HttpContext.Session.GetString("name");
            var lSM_PNContext = _context.tbl_ictams_upstype.Where(UpsType => UpsType.type_status == "AC");
            return View(await lSM_PNContext.ToListAsync());
        }

        // GET: UpsTypes
        public async Task<IActionResult> Index()
        {
              return _context.UpsType != null ? 
                          View(await _context.UpsType.ToListAsync()) :
                          Problem("Entity set 'AssetManagementContext.UpsType'  is null.");
        }

        // GET: UpsTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.UpsType == null)
            {
                return NotFound();
            }

            var upsType = await _context.UpsType
                .FirstOrDefaultAsync(m => m.type_id == id);
            if (upsType == null)
            {
                return NotFound();
            }

            return View(upsType);
        }

        // GET: UpsTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UpsTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("type_id,type_description,type_status,type_createdby,type_createddt,type_updatedby,type_updateddt")] UpsType upsType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(upsType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(upsType);
        }

        // GET: UpsTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.UpsType == null)
            {
                return NotFound();
            }

            var upsType = await _context.UpsType.FindAsync(id);
            if (upsType == null)
            {
                return NotFound();
            }
            return View(upsType);
        }

        // POST: UpsTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("type_id,type_description,type_status,type_createdby,type_createddt,type_updatedby,type_updateddt")] UpsType upsType)
        {
            if (id != upsType.type_id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(upsType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UpsTypeExists(upsType.type_id))
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
            return View(upsType);
        }

        // GET: UpsTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.UpsType == null)
            {
                return NotFound();
            }

            var upsType = await _context.UpsType
                .FirstOrDefaultAsync(m => m.type_id == id);
            if (upsType == null)
            {
                return NotFound();
            }

            return View(upsType);
        }

        // POST: UpsTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.UpsType == null)
            {
                return Problem("Entity set 'AssetManagementContext.UpsType'  is null.");
            }
            var upsType = await _context.UpsType.FindAsync(id);
            if (upsType != null)
            {
                _context.UpsType.Remove(upsType);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UpsTypeExists(int id)
        {
          return (_context.UpsType?.Any(e => e.type_id == id)).GetValueOrDefault();
        }
    }
}
