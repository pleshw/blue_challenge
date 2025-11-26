using BlueChallenge.Api.Model.User;

namespace BlueChallenge.Api.Service.Auth;

public interface ITokenService
{
    AuthenticationResult GenerateToken(UserModel user);
}
