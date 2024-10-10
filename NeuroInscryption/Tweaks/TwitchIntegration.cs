using System;
using System.Collections.Concurrent;
using BepInEx.Logging;
using NeuroInscryption.Utilities;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace NeuroInscryption.Tweaks;

partial class TwitchIntegration : Singlyboi<TwitchIntegration>
{
    private const string TwitchUsername = "alexejherodev";
    private const string TokenEnvVarName = "SCHIZO_TWITCH_TOKEN";
    private const string CommandPrefix = "pls";
    private const string TargetChannel = "vedal987";
    private static readonly ManualLogSource Log = Plugin.Logger;

    public static event Action<string>? HandleTwitchMessage;

    private TwitchClient _client = null!;
    private readonly ConcurrentQueue<string> _msgQueue = new();

    protected new void Awake()
    {
        base.Awake();
        ClientOptions clientOptions = new()
        {
            MessagesAllowedInPeriod = 750,
            ThrottlingPeriod = TimeSpan.FromSeconds(30)
        };
        WebSocketClient customClient = new(clientOptions);
        _client = new TwitchClient(customClient);

        _client.OnError += (_, evt) => Log.LogError(evt.Exception);
        _client.OnIncorrectLogin += (_, evt) => Log.LogError($"Could not connect to Twitch: {evt.Exception.Message}");
        _client.OnConnected += (_, evt) => Log.LogInfo($"Connected to Twitch as {evt.BotUsername}");
        _client.OnConnectionError += (_, evt) => Log.LogError($"Could not connect to Twitch: {evt.Error.Message}");
        _client.OnJoinedChannel += (_, evt) => Log.LogInfo($"Joined Twitch channel {evt.Channel}");
        _client.OnFailureToReceiveJoinConfirmation += (_, evt) => Log.LogError($"Could not join Twitch channel: {evt.Exception.Details}");
        _client.OnMessageReceived += Client_OnMessageReceived;

        string token = Environment.GetEnvironmentVariable(TokenEnvVarName);
        if (string.IsNullOrWhiteSpace(token))
        {
            Log.LogWarning("Twitch OAuth token is not set, Twitch Integration will be disabled.");
            Log.LogMessage($"Token should be set the {nameof(TokenEnvVarName)} environment variable.");
            return;
        }
        ConnectionCredentials credentials = new(TwitchUsername, token);

        _client.Initialize(credentials, TargetChannel);

        _client.Connect();
    }

    private void Client_OnMessageReceived(object _, OnMessageReceivedArgs evt)
    {
        ChatMessage message = evt.ChatMessage;
        if (!IsUserWhitelisted(message.Username)) return; // ensure I don't get isekaid
        if (!CheckPrefix(message.Message)) return;

        // OnMessageReceived runs in a worker thread, where we can't use Unity APIs
        _msgQueue.Enqueue(message.Message[CommandPrefix.Length..]);
    }

    private bool IsUserWhitelisted(string username)
    {
        return username == TwitchUsername;
    }

    private bool CheckPrefix(string message)
    {
        return message.StartsWith(CommandPrefix, StringComparison.OrdinalIgnoreCase);
    }

    private void FixedUpdate()
    {
        if (_msgQueue.TryDequeue(out string message)) HandleMessage(message);
    }

    private static void HandleMessage(string message)
    {
        HandleTwitchMessage?.Invoke(message);
    }
}
