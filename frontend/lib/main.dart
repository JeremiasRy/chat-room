import 'package:flutter/material.dart';
import 'package:frontend/chat_message.dart';

void main() {
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return const MaterialApp(
      title: "Just Chatting",
      home: ChatPage()
    );
  }
}

class ChatPage extends StatefulWidget {
  const ChatPage({super.key});

  @override
  State<StatefulWidget> createState() => _ChatPageState();
}

class _ChatPageState extends State<ChatPage> {
  List<ChatMessage> messages = [];

  @override void initState() {
    fetchAndSetMessages();
    super.initState();
  }

  Future<void> fetchAndSetMessages() async {
    try {
      List<ChatMessage> fetchedMessages = await fetchMessages();
      setState(() {
        messages = fetchedMessages;
      });
    } catch (error) {
      //Implement some logging
    }
  }
  
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("Chat Room")
        ),
      body: ListView.builder(
        itemCount: messages.length,
        itemBuilder: (context, index) {
          return ListTile(
            title: Text(messages[index].name),
            subtitle: Text(messages[index].content),
            trailing: Text(messages[index].createdAt.toString()),
          );
        }
      ),
    );
  }

}
