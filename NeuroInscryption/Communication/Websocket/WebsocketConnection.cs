using System;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using NeuroInscryption.Utilities;
using Newtonsoft.Json.Linq;
using UnityEngine;
using WebSocketSharp;

namespace NeuroInscryption.Communication.Websocket;

[HarmonyPatch]
public sealed class WebsocketConnection : Singlyboi<WebsocketConnection>
{
    private static readonly Queue<WsMessage> _messageQueue = [];
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
        _socket.Connect();
    }

    private void Update()
    {
        if (_socket?.ReadyState is WebSocketState.Open)
        {
            while (_messageQueue.Count > 0)
            {
                WsMessage message = _messageQueue.Dequeue();
                _socket.SendAsync(message.SerializeToJson(), success =>
                {
                    if (!success)
                    {
                        Plugin.Logger.LogError("Failed to send ws message " + message.SerializeToJson());
                        _messageQueue.Enqueue(message);
                    }
                });
            }
        }
    }

    public static void Send(WsMessage wsMessage)
    {
        _messageQueue.Enqueue(wsMessage);
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
            Plugin.Logger.LogInfo("Received message " + msg.Data);

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
