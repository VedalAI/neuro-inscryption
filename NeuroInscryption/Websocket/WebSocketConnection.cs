using System;
using System.Collections;
using HarmonyLib;
using NeuroInscryption.Utilities;
using Newtonsoft.Json;
using UnityEngine;
using WebSocketSharp;

namespace NeuroInscryption.Websocket;

[HarmonyPatch]
internal sealed class WebSocketConnection : Singlyboi<WebSocketConnection>
{
    private static WebSocket _socket;

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
            if (_socket?.ReadyState is WebSocketState.Open or WebSocketState.Connecting) _socket?.Close();
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

    public static void Send(WsMessage wsMessage)
    {
        _socket.Send(wsMessage);
    }

    private static void ReceiveMessage(MessageEventArgs msg)
    {
        try
        {
            Plugin.Logger.LogWarning("Received message " + msg.Data);

            WsMessage wsMessage = JsonConvert.DeserializeObject<WsMessage>(msg.Data);
            switch (wsMessage.Command)
            {
            }
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError("Received invalid message");
            Plugin.Logger.LogError(e);
        }
    }
}
