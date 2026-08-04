using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DogaRun.Data;

namespace DogaRun.Services
{
    public sealed class AuthenticationResult
    {
        public bool Success;
        public string PlayerId;
        public string Username;
        public string ErrorMessage;
    }

    public interface IAuthenticationService
    {
        bool IsSignedIn { get; }
        string PlayerId { get; }
        Task<AuthenticationResult> SignInGuestAsync(CancellationToken cancellationToken);
        Task<AuthenticationResult> SignInAsync(string username, string password, CancellationToken cancellationToken);
        Task<AuthenticationResult> RegisterAsync(string username, string password, CancellationToken cancellationToken);
        Task SignOutAsync(CancellationToken cancellationToken);
        Task<bool> DeleteAccountAsync(CancellationToken cancellationToken);
    }

    public interface IPlayerProfileService
    {
        Task<string> GetUsernameAsync(CancellationToken cancellationToken);
    }

    public interface IRunHistoryRepository
    {
        Task<IReadOnlyList<RunRecord>> GetTopFiveAsync(CancellationToken cancellationToken);
        Task SaveAsync(RunRecord record, CancellationToken cancellationToken);
    }

    public interface ICloudSaveService
    {
        Task SaveAsync(string key, string json, CancellationToken cancellationToken);
        Task<string> LoadAsync(string key, CancellationToken cancellationToken);
    }

    public interface ILeaderboardService
    {
        Task SubmitScoreAsync(int score, CancellationToken cancellationToken);
    }

    public interface ILocalStorageService
    {
        bool HasKey(string key);
        string GetString(string key, string fallback = "");
        void SetString(string key, string value);
        void DeleteKey(string key);
        void Save();
    }
}
