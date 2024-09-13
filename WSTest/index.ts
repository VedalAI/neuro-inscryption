import { WebSocket, WebSocketServer } from "ws";
import "./app";

const wss = new WebSocketServer({ port: 8000 });

let connections : WebSocket[] = [];

wss.on("connection", function connection(ws) {
    console.log("Connection opened");
    connections.push(ws);
    ws.on("message", data => onMessageReceived(JSON.parse(data.toString()) as Message));
    ws.on("close", () => {
        console.log("Connection closed");
        connections = connections.filter(c => c != ws);
    });
});

async function onMessageReceived(data : Message) {
    console.log("<---", data);

   if (data.command == "decision") {
       const oneOf: {properties:{action:{const: string}, data:{enum: string[]}}}[] = data.data["responseSchema"]["oneOf"];
       const action = oneOf.find(o => o.properties.action.const == "choose_card_to_play");
       if (action) {
           const possibleCards = action.properties.data.enum;
           const randomCard = possibleCards[Math.floor(Math.random() * possibleCards.length)];
           send({command: "action", data:{action: "choose_card_to_play", data: randomCard}});
       }
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
