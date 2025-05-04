using ems_back.Repo.Models;

namespace ems_back.Repo.Interfaces.Service {
    public interface ITokenService {
        Task<string> GenerateTokenAsync(User user);
    }
}