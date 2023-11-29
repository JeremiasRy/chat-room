import 'dart:async';
import 'dart:convert';

import 'package:google_sign_in/google_sign_in.dart';
import 'package:http/http.dart' as http;

import 'chat_message.dart';

Future authenticate(String token) async {
  final url = Uri.parse("https://localhost:7098/Auth"); 
  final response = await http.post(url, body: token);
  print(response);
}

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