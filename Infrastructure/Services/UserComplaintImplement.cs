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
    public class UserComplaintImplement : UnitOfWork<Complaint> ,IUserComplaints
    {
        private readonly AppDbContext _context;
        public UserComplaintImplement(AppDbContext context) : base(context)
        {
            _context = context;
        }
        

        public async Task<Complaint> Update(Complaint complaint)
        {
            complaint.DateModified = DateTime.Now;
            _context.Complaints.Update(complaint);
            await _context.SaveChangesAsync();
            return complaint;
        }
    }
}
