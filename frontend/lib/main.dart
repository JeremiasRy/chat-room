import 'package:flutter/material.dart';
import 'package:frontend/src/models/current_user.dart';
import 'package:frontend/src/shared_preferences.dart';
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
  List<ChatMessage> _messages = [];
  CurrentUser? _currentUser;
  bool _cantAuthorize = false;

  @override void initState() {
    super.initState();
    final TokenProvider tokenProvider = Provider.of<TokenProvider>(context, listen: false);
    final ProtectedEndpoints protectedEndpoints = Provider.of<ProtectedEndpoints>(context, listen: false);

    _googleSignIn.onCurrentUserChanged
      .listen((GoogleSignInAccount? account) async {

        if (account == null) {
          _cantAuthorize = true;
          return;
        }

        GoogleSignInAuthentication auth = await account.authentication;
        String? response = await authenticate(auth.idToken!);

        if (response == null) {
          _cantAuthorize = true;
          return;
        } 

        tokenProvider.token = response;
        setupChatEnvironment();
      }
    );

    protectedEndpoints.checkAndRefreshToken()
      .then((success) {
        if (!success) {
          _googleSignIn.signInSilently();
        } else {
          setupChatEnvironment();
        }
      }
    );
  }

  Future<void> setupChatEnvironment() async {
    if (_cantAuthorize) {
      return;
    }

    final protectedEndpoints = Provider.of<ProtectedEndpoints>(context, listen: false);
    final user = await protectedEndpoints.getCurrentUser();
    final messages = await protectedEndpoints.fetchMessages();

    setState(() {
      _currentUser = user;
      _messages = messages;
    });
  }

  @override
  Widget build(BuildContext context) {
    return ListView.builder(
      itemCount: _messages.length,
      itemBuilder: (context, index) {
        return ListTile(
          title: Text(_messages[index].name),
          subtitle: Text(_messages[index].content),
          trailing: Text(_messages[index].createdAt.toString()),
        );
      }
    );
  }
}
