using KoshelekTestTask.Core.Entities;

namespace KoshelekTestTask.Core.Interfaces
{
    public interface IMessageHandler
    {
        bool CheckMessage(Message message, out string error);
        DateTime GetCurrentMoscowTime();
    }
}
