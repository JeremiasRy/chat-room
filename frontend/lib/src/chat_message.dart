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


