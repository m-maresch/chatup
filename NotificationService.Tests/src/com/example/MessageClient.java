package com.example;

import org.java_websocket.client.WebSocketClient;
import org.java_websocket.drafts.Draft;
import org.java_websocket.handshake.ServerHandshake;
import org.json.JSONException;
import org.json.JSONObject;

import java.net.URI;
import java.nio.ByteBuffer;

public class MessageClient extends WebSocketClient {

    private int userID;

    public MessageClient(URI serverUri, Draft draft, int userID) {
        super(serverUri, draft);
        this.userID = userID;
    }

    public MessageClient(URI serverURI, int userID) {
        super(serverURI);
        this.userID = userID;
    }

    @Override
    public void onOpen(ServerHandshake handshakedata) {
        SignalRUtils.register(userID, this);
    }

    @Override
    public void onClose(int code, String reason, boolean remote) {
        SignalRUtils.unregister(userID, this);
    }

    @Override
    public void onMessage(String message) {
        try {
            NotificationManager.onMessage(new JSONObject(message).getString("target"));
        } catch (JSONException ex) {}
    }

    @Override
    public void onMessage(ByteBuffer message) {}

    @Override
    public void onError(Exception ex) {
        NotificationManager.onError(ex);
    }
}
