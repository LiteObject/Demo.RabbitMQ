namespace Demo.Publisher.API.Services
{
    public interface IMessageService
    {
        bool Enqueue(string message);
    }
}
