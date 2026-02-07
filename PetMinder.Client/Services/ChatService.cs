using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using PetMinder.Shared.DTO;
using System.Collections.Generic;

namespace PetMinder.Client.Services
{
    public class ChatService : IAsyncDisposable
    {
        private readonly HubConnection _hubConnection;
        private readonly HttpClient _httpClient;
        public event Action<MessageDTO>? OnMessageReceived;

        public ChatService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("AuthApi");
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_httpClient.BaseAddress + "chathub")
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<MessageDTO>("ReceiveMessage", (msg) =>
            {
                OnMessageReceived?.Invoke(msg);
            });
        }

        public async Task StartAsync()
        {
            if (_hubConnection.State == HubConnectionState.Disconnected)
                await _hubConnection.StartAsync();
        }

        public async Task JoinConversation(string conversationId)
        {
            await _hubConnection.InvokeAsync("JoinConversation", conversationId);
        }

        public async Task LeaveConversation(string conversationId)
        {
            await _hubConnection.InvokeAsync("LeaveConversation", conversationId);
        }

        public async Task SendMessageToConversation(MessageDTO message)
        {
            await _httpClient.PostAsJsonAsync("api/messages", message);
        }

        public async Task<List<MessageDTO>> GetMessagesForConversation(long conversationId)
        {
            return await _httpClient.GetFromJsonAsync<List<MessageDTO>>($"api/messages?conversationId={conversationId}") 
                   ?? new List<MessageDTO>();
        }

        public async ValueTask DisposeAsync()
        {
            await _hubConnection.DisposeAsync();
        }
    }
}
