import { WebSocket, WebSocketServer } from "ws";
import { JSONSchemaFaker } from "json-schema-faker";

const wss = new WebSocketServer({ port: 8000 });

let connections : WebSocket[] = [];

wss.on("connection", function connection(ws) {
    console.log("+ Connection opened");
    connections.push(ws);
    ws.on("message", data => onMessageReceived(JSON.parse(data.toString()) as Message));
    ws.on("close", () => {
        console.log("- Connection closed");
        connections = connections.filter(c => c != ws);
    });
});

async function onMessageReceived(data : Message) {
    console.log("<---", data);

   if (data.command == "decision") {
       const value = JSONSchemaFaker.generate(data.data["responseSchema"]);
       send({command: "action", data: value});
   }
}

export function send(msg : Message) {
    console.log("--->", msg);

    const msgStr = JSON.stringify(msg);
    for (const connection of connections) {
        connection.send(msgStr);
    }
}

export type Message = {
    command: string,
    data?: {[key: string]:any}
}
