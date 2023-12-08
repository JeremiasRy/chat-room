import 'dart:async';
import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:frontend/src/models/current_user.dart';
import 'package:frontend/src/shared_preferences.dart';
import 'package:frontend/src/providers.dart';
import 'package:http/http.dart' as http;
import 'package:signalr_netcore/signalr_client.dart';

import 'models/chat_message.dart';

class ChatHubConnection with ChangeNotifier {
  final String _serverUrl;
  late TokenProvider _tokenProvider;

  void updateTokenProvider(TokenProvider tokenProvider) {
    _tokenProvider = tokenProvider;
  }

  void onReceiveMessage(Function(ChatMessage) callback) {
    hubConnection.on('ReceiveMessage', (message) {
        List<dynamic> response = json.decode(message!.toString());
        ChatMessage responseMessage = ChatMessage.fromJson(response[0]);
        callback(responseMessage);
    });
  }

  void onReceiveConnectedUsers(Function(List<String>) callback) {
    hubConnection.on('ConnectedUsers', (message) {
        List<dynamic> response = json.decode(message!.toString());
        List<String> connectedUsers = List<String>.from(response[0]);
        callback(connectedUsers);
    });
  }

  void onUserConnected(Function(String) callback) {
    hubConnection.on("UserConnected", (message) {
      List<dynamic> response = json.decode(message!.toString());
      callback(response[0]);
    });
  }

  late HubConnection hubConnection;

  Future<void> startConnection() async {
    final token = _tokenProvider.token;
    hubConnection = HubConnectionBuilder()
      .withUrl(
        _serverUrl,
        options: HttpConnectionOptions(
          accessTokenFactory: () async => "$token"
        ))
      .build();
      
    await hubConnection.start();
  }

  void sendMessage(String message) async {
    hubConnection.invoke('SendMessage', args: [message]);
  }

  ChatHubConnection(this._serverUrl, this._tokenProvider);
}

class ProtectedEndpoints with ChangeNotifier {
  late TokenProvider _tokenProvider;

  void updateTokenProvider(TokenProvider tokenProvider) {
    _tokenProvider = tokenProvider;
  }

  ProtectedEndpoints(this._tokenProvider);

  Future<bool> checkAndRefreshToken() async {
    final token = await SharedPreferencesHelper.getToken();

    if (token == null) {
      return false;
    }

    final url = Uri.parse("https://localhost:7098/api/v1/Auth/refresh"); 

    final response = await http.get(
      url,
      headers: {
        "Authorization": "Bearer $token",
        "Content-Type": "application/json"
      }
    );

    if (response.statusCode != 200) {
      return false;
    }
    SharedPreferencesHelper.saveToken(response.body);
    _tokenProvider.token = response.body;
    return true;
  }
  
  Future<CurrentUser> getCurrentUser() async {
    final token = _tokenProvider.token;

    if (token == null) {
      throw Exception("You shouldn't be calling this endpoint without token");
    }

    final url = Uri.parse("https://localhost:7098/api/v1/User");

    final response = await http.get(
      url,
      headers: {
        "Authorization": "Bearer $token",
        "Content-Type": "application/json"
      }
    );

    if (response.statusCode == 200) {
      return CurrentUser.fromJson(json.decode(response.body));
    } else {
      throw Exception("Somehow we failed miserably here.");
    }
  }

  Future<List<ChatMessage>> fetchMessages({DateTime? lastCreatedAt}) async {
    final token = _tokenProvider.token;

    if (token == null) {
      throw Exception("You shouldn't be calling this endpoint without token");
    }

    final url = Uri.parse("https://localhost:7098/api/v1/Messages");
  
    if (lastCreatedAt != null) {
      Map<String, String> queryParams = {
        'lastCreatedAt': lastCreatedAt.toIso8601String(),
      };
      url.replace(queryParameters: queryParams);
    }

    final response = await http.get(
      url,
      headers: {
        "Authorization": "Bearer $token",
        "Content-Type": "application/json"
      }
    );
  
    if (response.statusCode == 200) {
      List<dynamic> responseBody = json.decode(response.body);
      List<ChatMessage> messages = responseBody.map((json) {
        return ChatMessage.fromJson(json);
      }).toList();
      return messages;
    } else {
      throw Exception("Things went south while fetching stuff from the internet");
    }
  }
}

Future<String?> authenticate(String token) async {
  final url = Uri.parse("https://localhost:7098/api/v1/Auth"); 
 
  final response = await http.post(
    url, 
    headers: {
      "content-type": "application/json"
    }, 
    body: json.encode({
      "credential": token
    })
  );

  if (response.statusCode != 200) {
    return null;
  }
  SharedPreferencesHelper.saveToken(response.body);
  return response.body;
}