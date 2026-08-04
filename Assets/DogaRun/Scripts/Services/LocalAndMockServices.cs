using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DogaRun.Data;
using UnityEngine;

namespace DogaRun.Services
{
    public sealed class PlayerPrefsLocalStorageService : ILocalStorageService
    {
        public bool HasKey(string key) => PlayerPrefs.HasKey(key);
        public string GetString(string key, string fallback = "") => PlayerPrefs.GetString(key, fallback);
        public void SetString(string key, string value) => PlayerPrefs.SetString(key, value ?? string.Empty);
        public void DeleteKey(string key) => PlayerPrefs.DeleteKey(key);
        public void Save() => PlayerPrefs.Save();
    }

    public sealed class LocalGuestAuthenticationService : IAuthenticationService
    {
        private const string GuestIdKey = "DogaRun.GuestPlayerId";
        private readonly ILocalStorageService storage;

        public LocalGuestAuthenticationService(ILocalStorageService storage)
        {
            this.storage = storage;
        }

        public bool IsSignedIn { get; private set; }
        public string PlayerId { get; private set; }

        public Task<AuthenticationResult> SignInGuestAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            PlayerId = storage.GetString(GuestIdKey);
            if (string.IsNullOrWhiteSpace(PlayerId))
            {
                PlayerId = $"guest-{Guid.NewGuid():N}";
                storage.SetString(GuestIdKey, PlayerId);
                storage.Save();
            }
            IsSignedIn = true;
            return Task.FromResult(new AuthenticationResult { Success = true, PlayerId = PlayerId, Username = "Misafir" });
        }

        public Task<AuthenticationResult> SignInAsync(string username, string password, CancellationToken cancellationToken) =>
            Task.FromResult(Unavailable());

        public Task<AuthenticationResult> RegisterAsync(string username, string password, CancellationToken cancellationToken) =>
            Task.FromResult(Unavailable());

        public Task SignOutAsync(CancellationToken cancellationToken)
        {
            IsSignedIn = false;
            PlayerId = null;
            return Task.CompletedTask;
        }

        public Task<bool> DeleteAccountAsync(CancellationToken cancellationToken)
        {
            storage.DeleteKey(GuestIdKey);
            storage.Save();
            IsSignedIn = false;
            PlayerId = null;
            return Task.FromResult(true);
        }

        private static AuthenticationResult Unavailable() => new AuthenticationResult
        {
            Success = false,
            ErrorMessage = "Çevrimiçi hesap sistemi henüz yapılandırılmadı. Misafir olarak devam edebilirsiniz."
        };
    }

    public sealed class MockAuthenticationService : IAuthenticationService
    {
        public bool IsSignedIn { get; private set; }
        public string PlayerId { get; private set; }

        public Task<AuthenticationResult> SignInGuestAsync(CancellationToken cancellationToken) => Complete("mock-guest", "Misafir");
        public Task<AuthenticationResult> SignInAsync(string username, string password, CancellationToken cancellationToken) => Complete("mock-player", username);
        public Task<AuthenticationResult> RegisterAsync(string username, string password, CancellationToken cancellationToken) => Complete("mock-player", username);
        public Task SignOutAsync(CancellationToken cancellationToken) { IsSignedIn = false; return Task.CompletedTask; }
        public Task<bool> DeleteAccountAsync(CancellationToken cancellationToken) { IsSignedIn = false; return Task.FromResult(true); }

        private Task<AuthenticationResult> Complete(string playerId, string username)
        {
            IsSignedIn = true;
            PlayerId = playerId;
            return Task.FromResult(new AuthenticationResult { Success = true, PlayerId = playerId, Username = username });
        }
    }

    public sealed class MockCloudSaveService : ICloudSaveService
    {
        private readonly Dictionary<string, string> values = new Dictionary<string, string>(StringComparer.Ordinal);
        public Task SaveAsync(string key, string json, CancellationToken cancellationToken) { values[key] = json; return Task.CompletedTask; }
        public Task<string> LoadAsync(string key, CancellationToken cancellationToken) => Task.FromResult(values.TryGetValue(key, out var value) ? value : null);
    }

    public sealed class UgsAuthenticationService : IAuthenticationService
    {
        private const string Message = "Unity Gaming Services PHASE 4'te yapılandırılacak.";
        public bool IsSignedIn => false;
        public string PlayerId => null;
        public Task<AuthenticationResult> SignInGuestAsync(CancellationToken cancellationToken) => Task.FromResult(Failed());
        public Task<AuthenticationResult> SignInAsync(string username, string password, CancellationToken cancellationToken) => Task.FromResult(Failed());
        public Task<AuthenticationResult> RegisterAsync(string username, string password, CancellationToken cancellationToken) => Task.FromResult(Failed());
        public Task SignOutAsync(CancellationToken cancellationToken) => Task.CompletedTask;
        public Task<bool> DeleteAccountAsync(CancellationToken cancellationToken) => Task.FromResult(false);
        private static AuthenticationResult Failed() => new AuthenticationResult { Success = false, ErrorMessage = Message };
    }

    public sealed class LocalRunHistoryRepository : IRunHistoryRepository
    {
        private const string StorageKey = "DogaRun.RunHistory.v1";
        private readonly ILocalStorageService storage;

        public LocalRunHistoryRepository(ILocalStorageService storage)
        {
            this.storage = storage;
        }

        public Task<IReadOnlyList<RunRecord>> GetTopFiveAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var data = Read();
            return Task.FromResult(RunHistoryMerger.MergeTopFive(data.Records, Array.Empty<RunRecord>()));
        }

        public Task SaveAsync(RunRecord record, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var data = Read();
            var merged = RunHistoryMerger.MergeTopFive(data.Records, new[] { record });
            storage.SetString(StorageKey, JsonUtility.ToJson(new RunRecordCollection { Records = new List<RunRecord>(merged) }));
            storage.Save();
            return Task.CompletedTask;
        }

        private RunRecordCollection Read()
        {
            if (!storage.HasKey(StorageKey)) return new RunRecordCollection();
            var parsed = JsonUtility.FromJson<RunRecordCollection>(storage.GetString(StorageKey));
            return parsed ?? new RunRecordCollection();
        }

        [Serializable]
        private sealed class RunRecordCollection
        {
            public List<RunRecord> Records = new List<RunRecord>();
        }
    }

    public sealed class UgsRunHistoryRepository : IRunHistoryRepository
    {
        public Task<IReadOnlyList<RunRecord>> GetTopFiveAsync(CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<RunRecord>>(Array.Empty<RunRecord>());
        public Task SaveAsync(RunRecord record, CancellationToken cancellationToken) => Task.CompletedTask;
    }

    public sealed class SaveGameService
    {
        private readonly IRunHistoryRepository repository;
        public SaveGameService(IRunHistoryRepository repository) { this.repository = repository; }
        public Task SaveRunAsync(RunRecord record, CancellationToken cancellationToken) => repository.SaveAsync(record, cancellationToken);
    }

    public sealed class AuthenticationCoordinator
    {
        private readonly IAuthenticationService authenticationService;
        public AuthenticationCoordinator(IAuthenticationService authenticationService) { this.authenticationService = authenticationService; }
        public Task<AuthenticationResult> ContinueAsGuestAsync(CancellationToken cancellationToken) => authenticationService.SignInGuestAsync(cancellationToken);
    }
}
