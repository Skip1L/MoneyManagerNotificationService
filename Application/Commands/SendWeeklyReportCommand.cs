using Application.Interfaces;
using DTOs.NotificationDTOs;

namespace Application.Commands
{
    public class SendWeeklyReportCommand : ICommand<bool>
    {
        public List<AnalyticEmailRequestDTO> AnalyticEmailRequestDTO { get; set; }

        public SendWeeklyReportCommand(List<AnalyticEmailRequestDTO> analyticEmailRequestDTO)
        {
            AnalyticEmailRequestDTO = analyticEmailRequestDTO;
        }
    }
}
