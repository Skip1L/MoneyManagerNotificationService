using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DTOs.NotificationDTOs;
using Services.Protos;

namespace Services.Mapping
{
    public class GrpcMappingProfile : Profile
    {
        public GrpcMappingProfile()
        {
            CreateMap<CategoryReport, CategoryReportDTO>();
            CreateMap<BudgetReport, BudgetReportDTO>();
            CreateMap<TransactionsSummary, TransactionsSummaryDTO>();
        }
    }
}
