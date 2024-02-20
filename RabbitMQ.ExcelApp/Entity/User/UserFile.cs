using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace RabbitMQ.ExcelApp.Entity.User
{

    public enum FileStatus
    {
        [Description("Creating")] Creating,
        [Description("Completed")] Completed
    }

    public class UserFile
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime? CreatedDate { get; set; }
        public FileStatus FileStatus { get; set; }

        [NotMapped]
        public string GetCreatedDate => 
            CreatedDate.HasValue ? CreatedDate.Value.ToShortDateString() : "-";
    }
}
