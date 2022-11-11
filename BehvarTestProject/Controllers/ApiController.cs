using BehvarTestProject.ApiModels;
using BehvarTestProject.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BehvarTestProject.Controllers
{
    [Route("[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class ReportsController : Controller
    {
        readonly ApplicationDpContext _context;
        readonly UserManager<IdentityUser> _userManager;

        public ReportsController(ApplicationDpContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Adds new reports 
        /// </summary>
        /// <param name="reportApiModel">Should be defined as a model </param>
        /// <response code ="201">Indicats new reports has been addded</response>
        /// <remarks>
        /// **Sample request:**
        ///
        ///     POST /Report
        ///     {
        ///        "Title": "This is title",
        ///        "Query": "Select * from Reports",
        ///     }
        ///
        /// </remarks>
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

        /// <summary>
        /// Gets item by it id 
        /// </summary>
        /// <param name="id" example="123">Report id</param>
        /// <response code="404">No item found</response>
        /// <response code="200">If item exist</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
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


        [HttpPost("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _userManager.Users.ToListAsync());
        }

        [HttpPost("rest")]
        public async Task<IActionResult> GetToken()
        {
            var user = await _userManager.Users.FirstAsync();
            //var token = user.GenerateJwtToken();
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            return Ok(token);
        }

        [HttpPost("token/{token}")]
        public async Task<IActionResult> UseToken(string token)
        {
            var user = await _userManager.Users.FirstAsync();
            var r = await _userManager.ResetPasswordAsync(user, token, "123");

            return Ok(token);
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
