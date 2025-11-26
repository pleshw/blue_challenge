using BlueChallenge.Api.Data;
using BlueChallenge.Api.Model.User;
using Microsoft.EntityFrameworkCore;

namespace BlueChallenge.Api.Repository;

public class UserRepository : IUserRepository
{
    private readonly BlueChallengeDbContext _context;

    public UserRepository(BlueChallengeDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(UserModel user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<UserModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _context.Users
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return users;
    }

    public async Task<UserModel?> GetByEmailAsync(string alias, string provider, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(alias);
        ArgumentException.ThrowIfNullOrEmpty(provider);

        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                user => user.Credentials.Email.Alias == alias && user.Credentials.Email.Provider == provider,
                cancellationToken);
    }

    public async Task UpdateAsync(UserModel user, CancellationToken cancellationToken = default)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (user is null)
        {
            return false;
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
