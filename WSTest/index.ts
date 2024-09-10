import { WebSocket, WebSocketServer } from "ws";
import "./app";

const wss = new WebSocketServer({ port: 8000 });

let connections : WebSocket[] = [];

wss.on("connection", function connection(ws) {
    connections.push(ws);
    ws.on("message", data => onMessageReceived(JSON.parse(data.toString()) as Message));
    ws.on("close", () => connections = connections.filter(c => c != ws));
});

async function onMessageReceived(data : Message) {
    console.log("<---", data);

   // add responses here
}

export function send(msg : Message) {
    console.log("--->", msg);

    const msgStr = JSON.stringify(msg);
    for (const connection of connections) {
        connection.send(msgStr);
    }
}

export type Message = {
    Command: string,
    Data?: {[key: string]:any}
}
