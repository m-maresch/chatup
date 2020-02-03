package com.example;

import okhttp3.*;
import org.json.JSONObject;

import java.net.URI;
import java.text.SimpleDateFormat;
import java.util.*;

public class Main {

    public static void main(String[] args) {
        try {
            final MessageClient wsClient = new MessageClient(new URI("ws://localhost:5010/notifications"), 11);
            SignalRUtils.connect(wsClient);

            OkHttpClient httpClient = new OkHttpClient();
            while (true) {
                Scanner in = new Scanner(System.in);
                String content = in.nextLine();

                if (content.equals("exit")) {
                    break;
                }

                JSONObject jsonObject = new JSONObject();
                jsonObject.put("senderID","11");
                jsonObject.put("receiverID","11");
                jsonObject.put("messageContent", content);
                jsonObject.put("time", new SimpleDateFormat("dd.MM.yyyy HH:mm:ss").format(new Date()));
                String json = jsonObject.toString();

                RequestBody body = RequestBody.create(MediaType.parse("application/json; charset=utf-8"), json);
                Request request = new Request.Builder()
                        .url("http://localhost:5010/messages/send")
                        .post(body)
                        .build();
                httpClient.newCall(request).execute();
            }

            SignalRUtils.disconnect(wsClient);
        }
        catch (Exception e) {
            System.out.println(e.getMessage());
        }
    }
}
