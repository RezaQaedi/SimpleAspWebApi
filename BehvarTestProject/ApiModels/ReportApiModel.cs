namespace BehvarTestProject.ApiModels
{
    public class ReportApiModel
    {
        /// <summary>
        /// Title of this query report 
        /// </summary>
        /// <example>Select all querys</example>
        public string Title { get; set; }

        /// <summary>
        /// Query do run in db 
        /// </summary>
        /// <example>Select * from dbo.reports</example>
        public string Query { get; set; }
    }
}
