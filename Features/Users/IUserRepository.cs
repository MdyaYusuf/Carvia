using Carvia.Core.Repositories;

namespace Carvia.Features.Users;

public interface IUserRepository : IRepository<User, Guid>
{

}
