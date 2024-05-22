using KoshelekTestTask.Core.Entities;

namespace KoshelekTestTask.Core.Interfaces
{
    public interface IMessageDispatcher
    {
        Task SendMessageToAllUsers(Message message);
    }
}
