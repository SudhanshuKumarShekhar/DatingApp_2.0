using DatingApp.IRepository;

namespace DatingApp.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IMessageRepository MessageRepository { get; }
        ILikesRepository likesRepository { get; }
        public Task<bool> Complete();
        bool HasChanges();
    }
}
