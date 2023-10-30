using Core.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace UserComplaint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserComplaintController : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> PostComplaint([FromForm] IFormFile AttachmentPath, [FromForm] string ComplaintText,
           [FromForm] bool isApproved)
        {
            VaildateUploadFile(AttachmentPath);

            if(ModelState.IsValid)
            {
                var Complaints = new Complaint
                {

                };

            }
            return Ok();
        }

        private void VaildateUploadFile(IFormFile AttachmentPath)
        {
            var allowedExtensions = new string[] { ".pdf" };

            if (!allowedExtensions.Contains(Path.GetExtension(AttachmentPath.FileName).ToLower()))
            {
                ModelState.AddModelError("AttachmentPath", "Unsupported File Format!!");
            }

            if(AttachmentPath.Length > 104857600)
            {
                ModelState.AddModelError("AttachmentPath", "File Size Cannot Be More Than 100MB!!");
            }
        }
    }
}
