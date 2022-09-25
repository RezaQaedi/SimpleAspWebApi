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
        public async Task<ApiResponse> PostReport([Bind("Title,Query")] ReportApiModel reportDataModel)
        {
            var report = new ReportDataModel(reportDataModel.Title, reportDataModel.Query)
            {
                CreatedAt = DateTime.Now
            };

            if (ModelState.IsValid)
            {
                _context.Add(report);
                await _context.SaveChangesAsync();
                return new ApiResponse()
                {
                    Data = report,
                };
            }
            return new ApiResponse()
            {
                ErrorMessage = "Couldn't add new item!"
            };
        }

        // GET : api/Reports/1
        [HttpGet("{id}")]
        public async Task<ApiResponse> GetReports(int id)
        {
            var errorResponse = new ApiResponse()
            {
                ErrorMessage = "Not found!",
            };

            if (_context.Reports == null)
            {
                return errorResponse;
            }

            var reportDataModel = await _context.Reports
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reportDataModel == null)
            {
                return errorResponse;
            }

            return new ApiResponse()
            {
                Data = reportDataModel,
            };
        }

        // GET : api/Reports
        [HttpGet]
        public async Task<ApiResponse> GetReports()
        {
            if (_context.Reports == null)
            {
                return new ApiResponse()
                {
                    ErrorMessage = "Not found!",
                };
            }
            var dataList = await _context.Reports.ToListAsync();


            return new ApiResponse()
            {
                Data = dataList,
            };
        }

        // PUT : api/Reports/2
        [HttpPut("{id}")]
        public async Task<ApiResponse> ExecuteReport(int id)
        {
            var report = await _context.Reports.FirstOrDefaultAsync(m => m.Id == id);

            if (report == null || report.Query == null)
                return new ApiResponse { ErrorMessage = "Not Found" };

            if (report.Query.Substring(0, 6).ToUpper() == "SELECT")
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
                        // Make an Anonymous type for each row 
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
                    return new ApiResponse { ErrorMessage = "Query not valid!" };
                }
                finally
                {
                    _context.Database.CloseConnection();
                }

                return new ApiResponse() { Data = results };
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
                    return new ApiResponse { ErrorMessage = "Query not valid!" };
                }

                return new ApiResponse() { Data = "Done!" };
            }
        }
    }
}
