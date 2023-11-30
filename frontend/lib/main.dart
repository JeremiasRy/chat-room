import 'package:flutter/material.dart';
import 'package:frontend/src/models/current_user.dart';
import 'package:frontend/src/token_provider.dart';
import 'package:google_sign_in/google_sign_in.dart';
import 'package:provider/provider.dart';
import 'src/models/chat_message.dart';
import 'src/server.dart';

GoogleSignIn _googleSignIn = GoogleSignIn(
  scopes: ["https://www.googleapis.com/auth/cloud-platform"]
);

void main() {
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MultiProvider(
      providers: [
        ChangeNotifierProvider(create: (context) => TokenProvider()),
        ChangeNotifierProxyProvider<TokenProvider, ProtectedEndpoints>(
          create: (context) => ProtectedEndpoints(Provider.of<TokenProvider>(context, listen: false)),
          update: (context, tokenProvider, protectedEndpoints) {
            protectedEndpoints!.updateTokenProvider(tokenProvider);
            return protectedEndpoints;
          }
        )
      ],
      child: const MaterialApp(
        home: ChatPage()
      ),
    );
  }
}

class ChatPage extends StatelessWidget {
  const ChatPage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("Chat Room")
        ),
      body: const ChatPageContent()
    );
  }

}

class ChatPageContent extends StatefulWidget {
  const ChatPageContent({
    super.key,
  });

  @override
  State<StatefulWidget> createState() => _ChatPageContentState();
}

class _ChatPageContentState extends State<ChatPageContent> {
  List<ChatMessage> messages = [];
  CurrentUser? _currentUser;
  bool _cantAuthorize = false;

  @override void initState() {
    super.initState();
    final TokenProvider tokenProvider = Provider.of<TokenProvider>(context, listen: false);
    final ProtectedEndpoints protectedEndpoints = Provider.of<ProtectedEndpoints>(context, listen: false);

    _googleSignIn.onCurrentUserChanged
      .listen((GoogleSignInAccount? account) async {
        bool cantAuthorize = false;
        CurrentUser? currentUser;

        if (account == null) {
          cantAuthorize = true;
        } else {
          GoogleSignInAuthentication auth = await account.authentication;
          String? response = await authenticate(auth.idToken!);
          if (response == null) {
            cantAuthorize = true;
          } else {
            tokenProvider.token = response;
            currentUser = await protectedEndpoints.getCurrentUser();
          }
        }
        setState(() {
          _currentUser = currentUser;
          _cantAuthorize = cantAuthorize;
        });
      }
    );
  _googleSignIn.signInSilently();
  }

  Future<void> fetchAndSetMessages() async {
    try {
      //List<ChatMessage> fetchedMessages = await prote();
      setState(() {
        //messages = fetchedMessages;
      });
    } catch (error) {
      //Implement some logging
    }
  }

  @override
  Widget build(BuildContext context) {
    return ListView.builder(
      itemCount: messages.length,
      itemBuilder: (context, index) {
        return ListTile(
          title: Text(messages[index].name),
          subtitle: Text(messages[index].content),
          trailing: Text(messages[index].createdAt.toString()),
        );
      }
    );
  }
}
