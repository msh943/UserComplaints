using Core.Domain;
using Core.IService;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class UserComplaintImplement : IUserComplaints
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly AppDbContext _context;
        public UserComplaintImplement(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor,
            AppDbContext context)
        {
            _webHostEnvironment = webHostEnvironment;
            _contextAccessor = httpContextAccessor;
            _context = context;
        }
        public async Task Create(IFormFile file, string complaint, int IsApproved, Complaint complaints)
        {
            var localPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Images",
                $"{complaints.FileName}{complaints.FileExtension}");

            using var stream = new FileStream(localPath, FileMode.Create);
            await file.CopyToAsync(stream);

            var request = _contextAccessor.HttpContext!.Request;
            var urlPath = $"{request.Scheme}://{request.Host}{request.PathBase}/Images/{complaints.FileName}{complaints.FileExtension}";
            complaints.Url = urlPath;
            complaints.isApproved = IsApproved;
            complaints.ComplaintText = complaint;
            await _context.Complaints.AddAsync(complaints);
            await Save();
        }

        public async Task<Complaint> Get(Expression<Func<Complaint,bool>> filter = null, bool tracked = true)
        {
            IQueryable<Complaint> query = _context.Complaints;

            if (!tracked)
            {
                query = query.AsNoTracking();
            }
            if (filter != null)
            {
                query = query.Where(filter);

            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<Complaint>> GetAll(Expression<Func<Complaint, bool>> filter = null)
        {
            IQueryable<Complaint> query = _context.Complaints;

            if(filter != null)
            {
                query = query.Where(filter);

            }
            return await query.ToListAsync();
        }

        public async Task Remove(Complaint complaint)
        {
            _context.Remove(complaint);
            await Save();
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public async Task Update(Complaint complaint)
        {
            _context.Complaints.Update(complaint);
            await Save();
        }
    }
}
