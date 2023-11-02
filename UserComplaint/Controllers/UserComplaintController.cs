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
using Microsoft.AspNetCore.Hosting;
using Core.Exception;
using System.Net;

namespace UserComplaint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserComplaintController : ControllerBase
    {
        private readonly IUserComplaints _userComplaints;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMapper _mapper;
        protected APIResponse _response;
        public UserComplaintController(IUserComplaints userComplaints, IMapper mapper, 
            IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _userComplaints = userComplaints;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _contextAccessor = httpContextAccessor;
            this._response = new ();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> Get()
        {
            try
            {
                IEnumerable<Complaint> complaints = await _userComplaints.GetAll();
                _response.Result = _mapper.Map<List<ComplaintDTO>>(complaints);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex) 
            {
                _response.IsSuccess = false;
                _response.Errors = new List<string> { ex.Message };
              
            }
            return Ok(_response);
        }

        [HttpGet("id:int")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> Get(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var complaint = await _userComplaints.Get(u => u.ComplaintId == id);
                if (complaint == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
                IEnumerable<Complaint> complaints = await _userComplaints.GetAll();
                _response.Result = _mapper.Map<ComplaintDTO>(complaint);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Errors = new List<string> { ex.Message };
              
            }
            return _response;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateComplaint([FromForm]ComplaintDTO complaintDTO,
            [FromForm] IFormFile file
            )
            
        {

         
            try
            {
                ValidateFileUpload(file);
                var localPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Images",
                $"{file.FileName}{Path.GetExtension(file.FileName).ToLower()}");

                using var stream = new FileStream(localPath, FileMode.Create);
                await file.CopyToAsync(stream);

                var request = _contextAccessor.HttpContext!.Request;
                var urlPath = $"{request.Scheme}://{request.Host}{request.PathBase}/Images/{file.FileName}{Path.GetExtension(file.FileName).ToLower()}";
                complaintDTO.Url = urlPath;

                if (complaintDTO == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                Complaint complaints = _mapper.Map<Complaint>(complaintDTO);
                await _userComplaints.Create(complaints);
                _response.Result = _mapper.Map<ComplaintDTO>(complaints);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);

            }
            catch(Exception ex)
            {
                _response.Result = ex.Message;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
        }
            

        

       
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<APIResponse>> DeleteComplaint(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                var complaint = await _userComplaints.Get(u => u.ComplaintId == id);
                if (complaint == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
            
                await _userComplaints.Remove(complaint!);
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                return Ok();
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Errors = new List<string> { ex.Message };
      
            }
            return _response;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<APIResponse>> UpdateComplaint([FromForm] ComplaintUpdateDTO updateDTO, int id)
        {
            try
            {
                if (updateDTO == null || id != updateDTO.ComplaintId)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                Complaint complaint = _mapper.Map<Complaint>(updateDTO);

                await _userComplaints.Update(complaint!);

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Errors = new List<string> { ex.Message };
               
            }
            return _response;
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
