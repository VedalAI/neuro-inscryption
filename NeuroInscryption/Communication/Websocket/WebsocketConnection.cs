using System;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Utilities;
using Newtonsoft.Json.Linq;
using UnityEngine;
using WebSocketSharp;

namespace NeuroInscryption.Communication.Websocket;

[HarmonyPatch]
public sealed class WebsocketConnection : Singlyboi<WebsocketConnection>
{
    private static readonly Queue<string> _messageQueue = [];
    private static readonly Dictionary<string, List<Action<JToken?>>> _subscriptions = [];
    private static WebSocket? _socket;

    private new void Awake()
    {
        base.Awake();
        StartWs();
    }

    private IEnumerator Reconnect()
    {
        yield return new WaitForSeconds(3);
        StartWs();
    }

    private void StartWs()
    {
        Plugin.Logger.LogWarning("Initializing WebSocket connection");

        try
        {
            if (_socket?.ReadyState is WebSocketState.Open or WebSocketState.Connecting) _socket.Close();
        }
        catch
        {
            // ignored
        }

        _socket = new WebSocket(Plugin.Config.WebsocketUrl.Value);
        _socket.OnMessage += (_, msg) => ReceiveMessage(msg);
        _socket.OnError += (_, e) =>
        {
            Plugin.Logger.LogError("Websocket connection has encountered an error!");
            Plugin.Logger.LogError(e.Message);
            Plugin.Logger.LogError(e.Exception);
            StartCoroutine(Reconnect());
        };
        _socket.OnClose += (_, _) =>
        {
            Plugin.Logger.LogError("Websocket connection has been closed!");
            StartCoroutine(Reconnect());
        };
        _socket.ConnectAsync();
    }

    private void Update()
    {
        if (_socket?.ReadyState is WebSocketState.Open)
        {
            while (_messageQueue.Count > 0)
            {
                string message = _messageQueue.Dequeue();

                Plugin.Logger.LogInfo("Sending ws message " + message);

                _socket.SendAsync(message, success =>
                {
                    if (!success)
                    {
                        Plugin.Logger.LogError("Failed to send ws message " + message);
                        _messageQueue.Enqueue(message);
                    }
                });
            }
        }
    }

    public static void Send(WsMessage wsMessage)
    {
        string message = wsMessage.SerializeToJson();
        // insanity check
        AutomationManager.Assert(!message.Contains("Enumerator") && !message.Contains("Enumerable"), "Someone called ToString on an enumerable");
        _messageQueue.Enqueue(message);
    }

    public static void SubscribeOnce(string command, Action<JToken?> callback)
    {
        if (!_subscriptions.ContainsKey(command)) _subscriptions[command] = [];
        _subscriptions[command].Add(callback);
    }

    private static void ReceiveMessage(MessageEventArgs msg)
    {
        try
        {
            Plugin.Logger.LogMessage("Received message " + msg.Data);

            JObject message = JObject.Parse(msg.Data);
            string? command = message["command"]?.Value<string>();
            JToken? data = message["data"];

            if (command == null)
            {
                Plugin.Logger.LogError("Previous command could not be deserialized (what??)");
                return;
            }

            if (!_subscriptions.TryGetValue(command, out List<Action<JToken?>> subscriptions) || subscriptions.Count == 0)
            {
                Plugin.Logger.LogWarning("Command " + command + " has no subscriptions, ignoring message");
                return;
            }

            foreach (Action<JToken?> callback in subscriptions)
            {
                callback(data);
            }
            subscriptions.Clear();
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError("Received invalid message");
            Plugin.Logger.LogError(e);
        }
    }
}
