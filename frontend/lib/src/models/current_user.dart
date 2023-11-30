import 'package:frontend/src/models/chat_message.dart';

class CurrentUser {
  final String name;
  final List<ChatMessage>? messages;

  const CurrentUser({
    required this.name,
    required this.messages
  });

  factory CurrentUser.fromJson(Map<String, dynamic> json) {
    return switch (json) {
      {
      "name": String name,
      "messages": List<ChatMessage>? messages,
      } => CurrentUser(
        name:name,
        messages:messages
      ),
      {
        "name": String name
      } => CurrentUser(
        name: name, 
        messages: null
      ),
      _ => throw const FormatException("Can't build current user")
    };
  }
}