using InvenTrack.Entities;

namespace InvenTrack.Security
{
    public interface IJwtService
    {

        string GenerateAccessToken(User user);

        string GenerateRefreshToken();
    }
}
