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
using Core.IService;

namespace UserComplaint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserComplaintController : ControllerBase
    {
        private readonly IUserComplaints _userComplaints;
        private readonly IMapper _mapper;
        public UserComplaintController(IUserComplaints userComplaints, IMapper mapper)
        {
            _userComplaints = userComplaints;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ComplaintDTO>>> Get()
        {
            IEnumerable<Complaint> complaints = await _userComplaints.GetAll();
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
            var complaint = await _userComplaints.Get(u => u.ComplaintId == id);
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
        public async Task<IActionResult> CreateComplaint([FromForm] IFormFile file, 
            [FromForm] string ComplainText,
            [FromForm] string Tilte, [FromForm] int isApproved, [FromForm] string DocName
            )
        {
            ValidateFileUpload(file);
            if (ModelState.IsValid)
            {
                var complaints = new Complaint
                {
                    ComplaintText = ComplainText,
                    DocName = DocName,
                    Title = Tilte,
                    FileExtension = Path.GetExtension(file.FileName).ToLower()

                };
                complaints = await _userComplaints.Create(file, ComplainText, isApproved, complaints);

                var response = new ComplaintDTO
                {
                    ComplaintText = complaints.ComplaintText,
                    Url = complaints.Url,
                    isApproved = complaints.isApproved,
                    DocName = complaints.DocName,
                    Title = complaints.Title,
                    FileExtension = complaints.FileExtension
                };
                return Ok(response);
            }

            return BadRequest(ModelState);

            //if (complaintDTO == null)
            //{
            //    return BadRequest(complaintDTO);
            //}

            //Complaint complaints = _mapper.Map<Complaint>(complaintDTO);
            //await _userComplaints.Create(file, complaintText, isApproved, complaints);

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
            var complaint = await _userComplaints.Get(u => u.ComplaintId == id);
            if(complaint == null)
            {
                return NotFound();
            }
            await _userComplaints.Remove(complaint!);
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
            
            
            await _userComplaints.Update(complaint!);
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
          var complaint = await _userComplaints.Get(u=> u.ComplaintId == id, tracked: false);

            ComplaintUpdateDTO complaintsDto = _mapper.Map<ComplaintUpdateDTO>(complaint);
            if (complaint == null)
            {
                return BadRequest();
            }
            patchDTO.ApplyTo(complaintsDto, ModelState);
            Complaint complain = _mapper.Map<Complaint>(complaintsDto);
            await _userComplaints.Update(complain);
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok();
        }

        private void ValidateFileUpload(IFormFile file)
        {
            var allowExtenision = new string[] { ".pdf" };

            if (!allowExtenision.Contains(Path.GetExtension(file.FileName).ToLower()))
            {
                ModelState.AddModelError("FileName", "Unsupported File Format");
            }

            if(file.Length > 104857600) 
            {
                ModelState.AddModelError("FileName", "File Size Cann't Be More Than 100MB");
            }

        }
    
    }
}
