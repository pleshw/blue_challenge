using BlueChallenge.Api.Model.User;
using BlueChallenge.Api.Repository;

namespace BlueChallenge.Api.Service;

public class UserService
{
    private IUserRepository UserRepository { get; }

    public UserService(IUserRepository userRepository)
    {
        UserRepository = userRepository;
    }

    public async Task<UserModel> CreateAndSaveUserAsync(string fullEmail, string password)
    {
        UserModel user = CreateUser(fullEmail, password);
        await UserRepository.CreateAsync(user);
        return user;
    }

    public async Task<UserModel> SaveUserAsync(UserModel user)
    {
        await UserRepository.CreateAsync(user);
        return user;
    }

    public UserModel CreateUser(string fullEmail, string password)
    {
        ArgumentException.ThrowIfNullOrEmpty(fullEmail);
        ArgumentException.ThrowIfNullOrEmpty(password);

        EmailModel emailModel = ParseEmail(fullEmail);

        UserCredentialsModel credentials = new()
        {
            Email = emailModel,
            Password = password
        };

        return new UserModel
        {
            Id = Guid.NewGuid(),
            Credentials = credentials
        };
    }

    public UserModel UpdateUser(UserModel existingUser, string fullEmail, string password)
    {
        ArgumentNullException.ThrowIfNull(existingUser);
        ArgumentException.ThrowIfNullOrEmpty(fullEmail);
        ArgumentException.ThrowIfNullOrEmpty(password);

        EmailModel emailModel = ParseEmail(fullEmail);

        UserCredentialsModel credentials = new()
        {
            Email = emailModel,
            Password = password
        };

        return existingUser with
        {
            Credentials = credentials
        };
    }

    private static EmailModel ParseEmail(string fullEmail)
    {
        var parts = fullEmail.Split('@');
        if (parts.Length != 2)
        {
            throw new ArgumentException("Invalid email format.");
        }

        return new EmailModel
        {
            Alias = parts[0],
            Provider = parts[1]
        };
    }
}