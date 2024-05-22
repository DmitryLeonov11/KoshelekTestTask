using KoshelekTestTask.Core.Entities;
using KoshelekTestTask.Core.Models;

namespace KoshelekTestTask.Core.Interfaces
{
    internal interface IMessgaeService
    {
        Task SendMessageAsync(Message message);
        Task<List<Message>> FindMessageOverPeriodOfTimeAsync(Interval interval);
    }
}
