using System.ComponentModel.DataAnnotations;

namespace BehvarTestProject.DataModels
{
    public class ReportDataModel
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Query { get; set; }
        public DateTime CreatedAt { get; set; }

        public ReportDataModel(string title, string query)
        {
            Title = title;
            Query = query;
        }
    }
}
