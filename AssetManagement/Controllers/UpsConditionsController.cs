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
    public class UpsConditionsController : Controller
    {
        private readonly AssetManagementContext _context;

        public UpsConditionsController(AssetManagementContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> UpsConditionsPartialView()
        {

            var myData = HttpContext.Session.GetString("name");
            var lSM_PNContext = _context.tbl_ictams_upscondition.Where(UpsCondition => UpsCondition.condition_status == "AC");
            return View(await lSM_PNContext.ToListAsync());
        }

        // GET: UpsConditions
        public async Task<IActionResult> Index()
        {
              return _context.UpsCondition != null ? 
                          View(await _context.UpsCondition.ToListAsync()) :
                          Problem("Entity set 'AssetManagementContext.UpsCondition'  is null.");
        }

        // GET: UpsConditions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.UpsCondition == null)
            {
                return NotFound();
            }

            var upsCondition = await _context.UpsCondition
                .FirstOrDefaultAsync(m => m.condition_id == id);
            if (upsCondition == null)
            {
                return NotFound();
            }

            return View(upsCondition);
        }

        // GET: UpsConditions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UpsConditions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("condition_id,condition_description,condition_status,condition_createdby,condition_createddt,condition_updatedby,condition_updateddt")] UpsCondition upsCondition)
        {
            if (ModelState.IsValid)
            {
                _context.Add(upsCondition);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(upsCondition);
        }

        // GET: UpsConditions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.UpsCondition == null)
            {
                return NotFound();
            }

            var upsCondition = await _context.UpsCondition.FindAsync(id);
            if (upsCondition == null)
            {
                return NotFound();
            }
            return View(upsCondition);
        }

        // POST: UpsConditions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("condition_id,condition_description,condition_status,condition_createdby,condition_createddt,condition_updatedby,condition_updateddt")] UpsCondition upsCondition)
        {
            if (id != upsCondition.condition_id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(upsCondition);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UpsConditionExists(upsCondition.condition_id))
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
            return View(upsCondition);
        }

        // GET: UpsConditions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.UpsCondition == null)
            {
                return NotFound();
            }

            var upsCondition = await _context.UpsCondition
                .FirstOrDefaultAsync(m => m.condition_id == id);
            if (upsCondition == null)
            {
                return NotFound();
            }

            return View(upsCondition);
        }

        // POST: UpsConditions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.UpsCondition == null)
            {
                return Problem("Entity set 'AssetManagementContext.UpsCondition'  is null.");
            }
            var upsCondition = await _context.UpsCondition.FindAsync(id);
            if (upsCondition != null)
            {
                _context.UpsCondition.Remove(upsCondition);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UpsConditionExists(int id)
        {
          return (_context.UpsCondition?.Any(e => e.condition_id == id)).GetValueOrDefault();
        }
    }
}
