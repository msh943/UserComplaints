using Core.Domain;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.IService
{
    public interface IUserComplaints
    {
        Task<List<Complaint>> GetAll(Expression<Func<Complaint, bool>> filter = null);
        Task<Complaint> Get(Expression<Func<Complaint, bool>> filter = null, bool tracked=true);
        Task Create(IFormFile file,string complaint, int IsApproved, Complaint complaints);
        Task Update(Complaint complaint);
        Task Remove(Complaint complaint);
        Task Save();
    }
}
