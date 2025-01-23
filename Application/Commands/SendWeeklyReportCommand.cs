using Application.Interfaces;
using DTOs.NotificationDTOs;

namespace Application.Commands
{
    public class SendWeeklyReportCommand : ICommand<bool>
    {
        public AnalyticEmailRequestDTO AnalyticEmailRequestDTO { get; set; }

        public SendWeeklyReportCommand(AnalyticEmailRequestDTO analyticEmailRequestDTO)
        {
            AnalyticEmailRequestDTO = analyticEmailRequestDTO;
        }
    }
}
