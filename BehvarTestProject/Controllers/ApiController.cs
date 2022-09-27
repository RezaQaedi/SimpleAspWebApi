using BehvarTestProject.ApiModels;
using BehvarTestProject.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Dynamic;

namespace BehvarTestProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : Controller
    {
        readonly ApplicationDpContext _context;

        public ReportsController(ApplicationDpContext context)
        {
            _context = context;
        }

        // POST: api/Reports
        [HttpPost]
        public async Task<IActionResult> PostReport([Bind("Title,Query")] ReportApiModel reportApiModel)
        {
            var report = new ReportDataModel(reportApiModel.Title, reportApiModel.Query)
            {
                CreatedAt = DateTime.Now
            };

            _context.Add(report);
            await _context.SaveChangesAsync();
            var baseUri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = baseUri + $"/api/Reports/{report.Id}";
            return Created(locationUri, report);
        }

        // GET : api/Reports/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReports(int id)
        {
            var reportDataModel = await _context.Reports
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reportDataModel == null)
                return NotFound();

            return Ok(reportDataModel);
        }

        // GET : api/Reports
        [HttpGet]
        public async Task<IActionResult> GetReports()
        {
            var dataList = await _context.Reports.ToListAsync();

            return Ok(dataList);
        }

        // PUT : api/Reports/2
        [HttpPut("{id}")]
        public async Task<IActionResult> ExecuteReport(int id)
        {
            var report = await _context.Reports.FirstOrDefaultAsync(m => m.Id == id);

            if (report == null || report.Query == null)
                return BadRequest();

            if (report.Query[..6].ToUpper() == "SELECT")
            {
                var results = new List<object>();
                try
                {
                    using var command = _context.Database.GetDbConnection().CreateCommand();
                    command.CommandText = report.Query;
                    command.CommandType = CommandType.Text;

                    _context.Database.OpenConnection();
                    using var reader = command.ExecuteReader();

                    DataTable schemaTable = reader.GetSchemaTable()!;
                    var cNames = Helpers.GetTableColumn(schemaTable!);

                    while (reader.Read())
                    {
                        var data = new Dictionary<string, object>();
                        dynamic expando = new ExpandoObject();
                        foreach (var cName in cNames)
                        {
                            var value = reader.GetValue(cName);
                            data.Add(cName, value);

                        }
                        // Add keyValues to expando obj
                        foreach (var keyValue in data)
                        {
                            Helpers.AddProperty(expando, keyValue.Key, keyValue.Value);
                        }

                        // Add dynamic obj to our result 
                        results.Add(expando);
                    }

                    results.Add(cNames);
                }
                catch (Exception)
                {
                    return BadRequest();
                }
                finally
                {
                    _context.Database.CloseConnection();
                }

                return Ok(results);
            }
            else
            {
                // If we dont have a table in return just excute 
                try
                {
                    _context.Database.ExecuteSqlRaw(report.Query);
                }
                catch (Exception)
                {
                    return BadRequest();
                }

                return Ok();
            }
        }
    }
}
