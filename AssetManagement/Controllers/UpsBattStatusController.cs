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
    public class UpsBattStatusController : Controller
    {
        private readonly AssetManagementContext _context;

        public UpsBattStatusController(AssetManagementContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> UpsBattStatusPartialView()
        {

            var myData = HttpContext.Session.GetString("name");
            var lSM_PNContext = _context.tbl_ictams_upsbattstatus.Where(UpsCondition => UpsCondition.status_status == "AC");
            return View(await lSM_PNContext.ToListAsync());
        }

        // GET: UpsBattStatus
        public async Task<IActionResult> Index()
        {
              return _context.UpsBattStatus != null ? 
                          View(await _context.UpsBattStatus.ToListAsync()) :
                          Problem("Entity set 'AssetManagementContext.UpsBattStatus'  is null.");
        }

        // GET: UpsBattStatus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.UpsBattStatus == null)
            {
                return NotFound();
            }

            var upsBattStatus = await _context.UpsBattStatus
                .FirstOrDefaultAsync(m => m.status_id == id);
            if (upsBattStatus == null)
            {
                return NotFound();
            }

            return View(upsBattStatus);
        }

        // GET: UpsBattStatus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UpsBattStatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("status_id,status_description,status_status,status_createdby,status_createddt,status_updatedby,status_updateddt")] UpsBattStatus upsBattStatus)
        {
            if (ModelState.IsValid)
            {
                _context.Add(upsBattStatus);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(upsBattStatus);
        }

        // GET: UpsBattStatus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.UpsBattStatus == null)
            {
                return NotFound();
            }

            var upsBattStatus = await _context.UpsBattStatus.FindAsync(id);
            if (upsBattStatus == null)
            {
                return NotFound();
            }
            return View(upsBattStatus);
        }

        // POST: UpsBattStatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("status_id,status_description,status_status,status_createdby,status_createddt,status_updatedby,status_updateddt")] UpsBattStatus upsBattStatus)
        {
            if (id != upsBattStatus.status_id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(upsBattStatus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UpsBattStatusExists(upsBattStatus.status_id))
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
            return View(upsBattStatus);
        }

        // GET: UpsBattStatus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.UpsBattStatus == null)
            {
                return NotFound();
            }

            var upsBattStatus = await _context.UpsBattStatus
                .FirstOrDefaultAsync(m => m.status_id == id);
            if (upsBattStatus == null)
            {
                return NotFound();
            }

            return View(upsBattStatus);
        }

        // POST: UpsBattStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.UpsBattStatus == null)
            {
                return Problem("Entity set 'AssetManagementContext.UpsBattStatus'  is null.");
            }
            var upsBattStatus = await _context.UpsBattStatus.FindAsync(id);
            if (upsBattStatus != null)
            {
                _context.UpsBattStatus.Remove(upsBattStatus);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UpsBattStatusExists(int id)
        {
          return (_context.UpsBattStatus?.Any(e => e.status_id == id)).GetValueOrDefault();
        }
    }
}
