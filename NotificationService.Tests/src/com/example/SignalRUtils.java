package com.example;

import com.google.gson.Gson;
import okhttp3.MediaType;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.RequestBody;
import org.java_websocket.client.WebSocketClient;
import org.json.JSONObject;

import java.util.HashMap;
import java.util.Map;
import java.util.UUID;

public class SignalRUtils {

    private static final String record_separator = "\u001E";
    private static final Gson gson = new Gson();

    public static void connect(MessageClient wsClient){
        wsClient.connect();

        Map<String, Object> map = new HashMap<>();
        map.put("protocol", "json");
        map.put("version", 1);
        wsClient.send(gson.toJson(map) + record_separator);
    }

    public static void disconnect(MessageClient wsClient){
        wsClient.close();
    }

    public static void register(int userID, MessageClient wsClient){
        Map<String, Object> map = new HashMap<>();
        map.put("type", 1);
        map.put("target", "OnConnected");
        map.put("arguments", new Object[] {userID});
        wsClient.send(gson.toJson(map) + record_separator);
    }

    public static void unregister(int userID, MessageClient wsClient){
        Map<String, Object> map = new HashMap<>();
        map.put("type", 1);
        map.put("target", "OnDisconnected");
        map.put("arguments", new Object[] {userID});
        wsClient.send(gson.toJson(map) + record_separator);
    }
}
