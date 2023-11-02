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
    public interface IUserComplaints : IUnitOfWork<Complaint>
    {
        Task<Complaint> Update(Complaint complaint);
    }
}
