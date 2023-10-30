using Core.Contract;
using Core.Domain;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace UserComplaint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserComplaintController : ControllerBase
    {
        private readonly IWebHostEnvironment? _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _context;
        public UserComplaintController(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor,
            AppDbContext context)
        {
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<ComplaintDTO>> Get()
        {
            return Ok(_context.Complaints.ToList());
        }

        [HttpGet("id:int")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<ComplaintDTO>> Get(int id)
        {
            if(id == 0)
            {
                return BadRequest();
            }
            var complaint = _context.Complaints.FirstOrDefault(u => u.ComplaintId == id);
            if(complaint == null)
            {
                return NotFound();
            }
            return Ok(complaint);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<ComplaintDTO> CreateComplaint([FromForm] ComplaintDTO complaintDTO)
        {
            if(complaintDTO == null)
            {
                return BadRequest(complaintDTO);
            }
            if(complaintDTO.ComplaintId > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            Complaint complaints = new()
            {
                ComplaintId = complaintDTO.ComplaintId,
                ComplaintText = complaintDTO.ComplaintText,
                isApproved = complaintDTO.isApproved
            };
            _context.Complaints.Add(complaints);
            _context.SaveChanges();
            return Ok();
        }
    }
}
