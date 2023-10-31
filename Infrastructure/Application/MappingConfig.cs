using AutoMapper;
using Core.Contract;
using Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Application
{
    public class MappingConfig : Profile
    {
        public MappingConfig() 
        {
            CreateMap<Complaint, ComplaintDTO>().ReverseMap();
            CreateMap<Complaint, ComplaintUpdateDTO>().ReverseMap();
        }
    }
}
