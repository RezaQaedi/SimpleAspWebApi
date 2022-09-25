using BehvarTestProject.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BehvarTestProject.Controllers
{
    public class ReportDataModelsController : Controller
    {
        private readonly ApplicationDpContext _context;

        public ReportDataModelsController(ApplicationDpContext context)
        {
            _context = context;
        }

        // GET: ReportDataModels
        public async Task<IActionResult> Index()
        {
              return View(await _context.Reports.ToListAsync());
        }

        // GET: ReportDataModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Reports == null)
            {
                return NotFound();
            }

            var reportDataModel = await _context.Reports
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reportDataModel == null)
            {
                return NotFound();
            }

            return View(reportDataModel);
        }

        // GET: ReportDataModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ReportDataModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Query,CreatedAt")] ReportDataModel reportDataModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reportDataModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(reportDataModel);
        }

        // GET: ReportDataModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Reports == null)
            {
                return NotFound();
            }

            var reportDataModel = await _context.Reports.FindAsync(id);
            if (reportDataModel == null)
            {
                return NotFound();
            }
            return View(reportDataModel);
        }

        // POST: ReportDataModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Query,CreatedAt")] ReportDataModel reportDataModel)
        {
            if (id != reportDataModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reportDataModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReportDataModelExists(reportDataModel.Id))
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
            return View(reportDataModel);
        }

        // GET: ReportDataModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Reports == null)
            {
                return NotFound();
            }

            var reportDataModel = await _context.Reports
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reportDataModel == null)
            {
                return NotFound();
            }

            return View(reportDataModel);
        }

        // POST: ReportDataModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Reports == null)
            {
                return Problem("Entity set 'ApplicationDpContext.Reports'  is null.");
            }
            var reportDataModel = await _context.Reports.FindAsync(id);
            if (reportDataModel != null)
            {
                _context.Reports.Remove(reportDataModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReportDataModelExists(int id)
        {
          return _context.Reports.Any(e => e.Id == id);
        }
    }
}
