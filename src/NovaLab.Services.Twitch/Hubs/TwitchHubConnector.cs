// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace NovaLab.Services.Twitch.Hubs;

using Microsoft.AspNetCore.SignalR.Client;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public class HubConnector(Uri endpoint) : IAsyncDisposable {
    private HubConnection? _connection;
    
    public HubConnector CreateConnection() {
        // Setup hub connection
        _connection = new HubConnectionBuilder()
            .WithUrl(endpoint, options => { options.UseDefaultCredentials = true; })
            .Build();

        return this;
    }

    public async Task Connect() {
        if (_connection != null) 
            await _connection.StartAsync();
    }

    public HubConnector RegisterCallback<T>(string methodName, Action<T> callback) {
        _connection?.On(methodName, callback);

        return this;
    }
    
    public async ValueTask DisposeAsync() {
        if (_connection != null) 
            await _connection.DisposeAsync();
    }
}
