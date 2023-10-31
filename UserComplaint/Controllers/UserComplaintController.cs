using Azure;
using Core.Contract;
using Core.Domain;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace UserComplaint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserComplaintController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public UserComplaintController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ComplaintDTO>>> Get()
        {
            IEnumerable<Complaint> complaints = await _context.Complaints.ToListAsync();
            return Ok(_mapper.Map<List<ComplaintDTO>>(complaints));
        }

        [HttpGet("id:int")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ComplaintDTO>>> Get(int id)
        {
            if(id == 0)
            {
                return BadRequest();
            }
            var complaint = await _context.Complaints.FirstOrDefaultAsync(u => u.ComplaintId == id);
            if(complaint == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<ComplaintDTO>(complaint));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ComplaintDTO>> CreateComplaint([FromForm] ComplaintDTO complaintDTO)
        {
            if(complaintDTO == null)
            {
                return BadRequest(complaintDTO);
            }

            Complaint complaints = _mapper.Map<Complaint>(complaintDTO);
            await _context.Complaints.AddAsync(complaints);
            await _context.SaveChangesAsync();
            return Ok();
        }

       
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteComplaint(int id)
        {
          if(id == 0)
          {
             return BadRequest();
          }
            var complaint = await _context.Complaints.FirstOrDefaultAsync(u => u.ComplaintId == id);
            if(complaint == null)
            {
                return NotFound();
            }
            _context.Complaints.Remove(complaint!);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateComplaint([FromForm] ComplaintUpdateDTO updateDTO, int id)
        {
            if (updateDTO == null || id != updateDTO.ComplaintId)
            {
                return BadRequest();
            }
            Complaint complaint = _mapper.Map<Complaint>(updateDTO);
            
            
            _context.Complaints.Update(complaint!);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> UpdatePartialComplaint(int id, JsonPatchDocument<ComplaintUpdateDTO> patchDTO)
        {
          if(patchDTO == null || id == 0)
          {
                return BadRequest();
          }
          var complaint = await _context.Complaints.AsNoTracking().FirstOrDefaultAsync(u=> u.ComplaintId == id);

            ComplaintUpdateDTO complaintsDto = _mapper.Map<ComplaintUpdateDTO>(complaint);
            if (complaint == null)
            {
                return BadRequest();
            }
            patchDTO.ApplyTo(complaintsDto, ModelState);
            Complaint complain = _mapper.Map<Complaint>(complaintsDto);
            _context.Complaints.Update(complain);
            await _context.SaveChangesAsync();
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok();
        }
    }
}
