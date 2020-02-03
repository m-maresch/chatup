package com.example;

public class NotificationManager {

    public static void onMessage(String message){
        System.out.println("Received: " + message);
    }

    public static void onError(Exception ex){
        System.out.println(ex.getMessage());
    }
}
