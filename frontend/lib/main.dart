import 'package:flutter/material.dart';
import 'package:frontend/src/models/current_user.dart';
import 'package:frontend/src/providers.dart';
import 'package:google_sign_in/google_sign_in.dart';
import 'package:provider/provider.dart';
import 'package:signalr_netcore/utils.dart';
import 'src/models/chat_message.dart';
import 'src/server.dart';

const String hubUrl = "https://localhost:7098/chat";

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
        ),
        ChangeNotifierProxyProvider<TokenProvider, ChatHubConnection>(
          create: (context) => ChatHubConnection(hubUrl, Provider.of<TokenProvider>(context, listen: false)),
          update: (context, tokenProvider, chatHubConnection) {
            chatHubConnection!.updateTokenProvider(tokenProvider);
            return chatHubConnection;
          })
      ],
      child: const MaterialApp(
        home: ChatPage(),
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

  const ChatPageContent({super.key});

  @override
  State<StatefulWidget> createState() => _ChatPageContentState();
}

class _ChatPageContentState extends State<ChatPageContent> {
  late ChatHubConnection _chat;
  List<ChatMessage> _messages = [];
  List<String> _connectedUsers = [];
  CurrentUser? _currentUser;
  bool _cantAuthorize = false;
  bool _loggingIn = true;
  final TextEditingController _textController = TextEditingController();
  final ScrollController _scrollController = ScrollController();

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
        await setupChatEnvironment();
      }
    );

    if (_currentUser == null) {
      protectedEndpoints
        .checkAndRefreshToken()
        .then((success) {
          if (!success) {
            _googleSignIn.signInSilently();
          } else {
            setupChatEnvironment();
            return;
          }
        }
      );
    }
  }

  Future<void> setupChatEnvironment() async {
    if (_cantAuthorize) {
      return;
    }
    final protectedEndpoints = Provider.of<ProtectedEndpoints>(context, listen: false);
    final ChatHubConnection chat = Provider.of<ChatHubConnection>(context, listen: false);
    final user = await protectedEndpoints.getCurrentUser();
    final messages = await protectedEndpoints.fetchMessages();
    await chat.startConnection();

    chat.onUserConnected((user) {
      final SnackBar snackBar = SnackBar(
        behavior: SnackBarBehavior.floating,
        width: MediaQuery.of(context).size.width * 0.25,
        content: Text("$user connected"),
      );

      ScaffoldMessenger.of(context).showSnackBar(snackBar);
    });

    chat.onReceiveConnectedUsers((connectedUsers) {
      setState(() {
        _connectedUsers = connectedUsers;
      });
    });

    chat.onReceiveMessage((message) {
      _messages.add(message);

      WidgetsBinding.instance.addPostFrameCallback((_) {
        scrollToLatestItem();
      });

      setState(() {});
    });

    setState(() {
      _messages = messages;
      _currentUser = user;
      _chat = chat;
      _loggingIn = false;
    });
    WidgetsBinding.instance.addPostFrameCallback((_) {
      scrollToLatestItem();
    });
  }

  void _sendMessage() {
    if (_loggingIn || _textController.text.isEmpty) {
      return;
    }
    _chat.sendMessage(_textController.text);
    _textController.clear();
  }

  void scrollToLatestItem() {
    _scrollController.animateTo(
      _scrollController.position.maxScrollExtent,
      duration: const Duration(milliseconds: 100),
      curve: Curves.easeInOut,
    );
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Expanded(
          child: _loggingIn 
          ? _cantAuthorize 
            ? const Text("Everything went to sh*t")
            : const Center(
              child: CircularProgressIndicator()
              )
          : ListView.builder(
              controller: _scrollController,
              itemCount: _messages.length,
              itemBuilder: (context, index) {
                return ListTile(
                  title: Text(_messages[index].name),
                  subtitle: Text(_messages[index].content),
                  trailing: Text(_messages[index].createdAt.toString()),
                );
              },
            ),
        ),
        TextField(
          controller: _textController,
          enabled: !_loggingIn,
          decoration: const InputDecoration(
            hintText: "Write your message here...",
          )
        ),
        ElevatedButton(
          onPressed: _sendMessage,
          child: const Text('Send'),
        ),
      ],
    );
  }
}
