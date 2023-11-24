import 'dart:async';
import 'dart:convert';

import 'package:http/http.dart' as http;

class ChatMessage {
  final String name;
  final String content;
  final DateTime createdAt;

  const ChatMessage({
    required this.name,
    required this.content,
    required this.createdAt,
  });

  factory ChatMessage.fromJson(Map<String, dynamic> json) {
    return switch (json) {
      {
        'name': String name,
        'content': String content,
        'createdAt': String createdAt
      } => ChatMessage(
        name:name,
        content:content,
        createdAt: DateTime.parse(createdAt)
      ),
      _ => throw const FormatException("Failed to load message")
    };
  }
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
