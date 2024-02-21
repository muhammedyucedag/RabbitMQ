using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.ExcelApp.Entity;
using RabbitMQ.ExcelApp.Entity.User;

namespace RabbitMQ.ExcelApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FilesController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpPost]
        public async Task<IActionResult> Upload (IFormFile file, Guid fileId)
        {
            if (file is not { Length: > 0 }) return BadRequest();

            var userFile = await _context.UserFiles.FirstAsync(x => x.Id == fileId);
            var filePath = userFile.FileName + Path.GetExtension(file.FileName);
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files", filePath);

            using FileStream stream = new(path, FileMode.Create);

            await file.CopyToAsync(stream);

            userFile.CreatedDate = DateTime.Now;
            userFile.FilePath = filePath;
            userFile.FileStatus = FileStatus.Completed;

            await _context.SaveChangesAsync();

            //SignalR Notification oluşturalacak 

            return Ok();
        }
    }
}
