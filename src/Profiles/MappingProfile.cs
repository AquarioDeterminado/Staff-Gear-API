using AutoMapper;
using API.src.models.dtos;
using API.src.models;

namespace API.src.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AuthToken, AuthTokenDTO>().ReverseMap();
            CreateMap<Notification, NotificationDTO>().ReverseMap();
            CreateMap<JobCandidate, JobCandidateDTO>().ReverseMap();
            CreateMap<Employee, EmployeeDTO>().ReverseMap();
            CreateMap<EmployeePayHistory, EmployeePayHistoryDTO>().ReverseMap();
            CreateMap<Department, DepartmentDTO>().ReverseMap();
            CreateMap<BusinessEntity, BusinessEntityDTO>().ReverseMap();
            CreateMap<EmailAddress, EmailAddressDTO>().ReverseMap();
            CreateMap<EmployeeDepartmentHistory, EmployeeDepartmentHistoryDTO>().ReverseMap();
        }
    }
}