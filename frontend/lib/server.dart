import 'package:frontend/chat_message.dart';

import 'dart:async';
import 'dart:convert';

import 'package:http/http.dart' as http;

Future<List<ChatMessage>> fetchMessages({DateTime? lastCreatedAt}) async {
  final url = Uri.parse("https://localhost:7098/api/v1/Messages");
  
  if (lastCreatedAt != null) {
    Map<String, String> queryParams = {
      'lastCreatedAt': lastCreatedAt.toIso8601String(),
    };
    url.replace(queryParameters: queryParams);
  }

  final response = await http.get(url);
  
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

  Future<void> authenticate() async {
    try {
      await http.get(Uri.parse('https://localhost:7098/api/v1/Auths/login'));
    } catch(error) {
      print("Whats going on here $error");
    }
  }