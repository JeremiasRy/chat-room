import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:google_sign_in/google_sign_in.dart';
import 'src/chat_message.dart';
import 'src/server.dart';

const scope = [
  'name',
];

GoogleSignIn _googleSignIn = GoogleSignIn(
  scopes: scope,
);

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
  GoogleSignInAccount? _currentUser;
  bool _isAuthorized = false;

  @override void initState() {
    super.initState();

    _googleSignIn.onCurrentUserChanged
      .listen((GoogleSignInAccount? account) async {
        bool isAuthorized = account != null;
        if (kIsWeb && account != null) {
          isAuthorized = await _googleSignIn.canAccessScopes(scope);
        }

        setState(() {
          _currentUser = account;
          _isAuthorized = isAuthorized;
        });

        if (isAuthorized) {
          //Get the credential and send it to backend to be checked
        }
      }
    );
  
  _googleSignIn.signInSilently();
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

  Future<void> authenticate() async {

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
