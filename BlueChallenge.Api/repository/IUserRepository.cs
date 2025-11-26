using BlueChallenge.Api.Model.User;

namespace BlueChallenge.Api.Repository;

public interface IUserRepository
{
    Task CreateAsync(UserModel user, CancellationToken cancellationToken = default);
    Task<UserModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserModel?> GetByEmailAsync(string alias, string provider, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<UserModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task UpdateAsync(UserModel user, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
