using DTOs.NotificationDTOs;
using MediatR;

namespace Persistence.Commands.EmailCommands
{
    public class SendWeeklyReportCommand : AnalyticEmailRequestDTO, IRequest<bool>
    {

    }
}
