// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace NovaLab.Services.Twitch.Hubs;

using ApiClient.Model;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[UsedImplicitly]
public class TwitchHubClient(NavigationManager navigationManager) {
    private HubConnection? _hubCache;
    private HubConnection Hub => _hubCache ??= new HubConnectionBuilder()
        .WithUrl(navigationManager.ToAbsoluteUri("/hubs/twitch"), options => { options.UseDefaultCredentials = true; })
        .Build();

    private List<IDisposable> _disposables = [];
    
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public virtual async Task StartAsync(string userId, CancellationToken cancellationToken = default) {
        await Hub.StartAsync(cancellationToken);
        await Hub.InvokeAsync("AddToGroup", userId, cancellationToken: cancellationToken);
    }

    public async ValueTask DisposeAsync() {
        if (_hubCache is not null) {
            await _hubCache.StopAsync();
            await _hubCache.DisposeAsync();
            
            _hubCache = null;
        }

        foreach (IDisposable disposable in _disposables) {
            disposable.Dispose();
        }
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Callback Methods
    // -----------------------------------------------------------------------------------------------------------------
    public void OnNewManagedRewardRedemption(Action<TwitchManagedRewardRedemptionDto> action) {
        _disposables.Add(Hub.On(TwitchHubMethods.NewManagedRewardRedemption, action)); 
    }
    
    public void OnClearedManagedReward(Action<Guid> action) {
        _disposables.Add(Hub.On(TwitchHubMethods.ClearedManagedReward, action)); 
    }
    
    public void OnNewTwitchFollower(Action action) {
        _disposables.Add(Hub.On(TwitchHubMethods.NewTwitchFollower, action)); 
    }

    public void OnSelectedTwitchManagedSubject(Action<TwitchManagedStreamSubjectDto> action) {
        _disposables.Add(Hub.On(TwitchHubMethods.SelectedTwitchManagedSubject, action)); 
    }
}
