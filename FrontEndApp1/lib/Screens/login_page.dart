// import 'package:flutter/material.dart';
// import 'package:http/http.dart' as http;
// import 'dart:convert';
// import 'package:shared_preferences/shared_preferences.dart';

// class LoginPage extends StatefulWidget {
//   @override
//   _LoginPageState createState() => _LoginPageState();
// }

// class _LoginPageState extends State<LoginPage> {
//   final TextEditingController _usernameController = TextEditingController();
//   final TextEditingController _passwordController = TextEditingController();
//   String? _errorMessage;
//   bool _isLoading = false;

//   Future<void> login() async {
//     setState(() {
//       _isLoading = true; // Start loading
//       _errorMessage = null; // Clear previous error
//     });

//     try {
//       final response = await http.post(
//         Uri.parse('http://localhost:5004/users/login'),
//         headers: {
//           'Content-Type': 'application/json',
//         },
//         body: json.encode({
//           'username': _usernameController.text,
//           'userPassword': _passwordController.text,
//         }),
//       );

//       print('Response status: ${response.statusCode}'); // Log response status
//       print('Response body: ${response.body}'); // Log response body

//       if (response.statusCode == 200) {
//         final responseData = json.decode(response.body);
        
//         // Log the parsed response
//         print('Parsed response: $responseData'); 

//         // Check if the token exists in the response
//         if (responseData.containsKey('token') && responseData['token'] != null) {
//           final token = responseData['token'];
          
//           // Store token in shared_preferences
//           SharedPreferences prefs = await SharedPreferences.getInstance();
//           await prefs.setString('token', token); // Store token for future requests

//           // Handle successful login
//           print('Login successful! Token: $token');
//           Navigator.pushNamed(context, '/dashboard');
//         } else {
//           setState(() {
//             _errorMessage = 'Login failed: Invalid response from server'; // Display error
//           });
//         }
//       } else {
//         setState(() {
//           _errorMessage = 'Login failed: ${response.body}'; // Display error
//         });
//       }
//     } catch (e) {
//       setState(() {
//         _errorMessage = 'An error occurred: $e'; // Display error
//       });
//     } finally {
//       setState(() {
//         _isLoading = false; // Stop loading
//       });
//     }
//   }

//   @override
//   Widget build(BuildContext context) {
//     return Scaffold(
//       appBar: AppBar(
//         title: Text('Login'),
//       ),
//       body: Padding(
//         padding: const EdgeInsets.all(16.0),
//         child: Column(
//           mainAxisAlignment: MainAxisAlignment.center,
//           children: [
//             TextField(
//               controller: _usernameController,
//               decoration: InputDecoration(
//                 labelText: 'Username',
//                 errorText: _errorMessage,
//               ),
//             ),
//             TextField(
//               controller: _passwordController,
//               decoration: InputDecoration(
//                 labelText: 'Password',
//                 errorText: _errorMessage,
//               ),
//               obscureText: true,
//             ),
//             SizedBox(height: 20),
//             _isLoading
//                 ? CircularProgressIndicator()
//                 : ElevatedButton(
//                     onPressed: login,
//                     child: Text('Login'),
//                   ),
//           ],
//         ),
//       ),
//     );
//   }
// }


import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';
import 'package:shared_preferences/shared_preferences.dart';

class LoginPage extends StatefulWidget {
  @override
  _LoginPageState createState() => _LoginPageState();
}

class _LoginPageState extends State<LoginPage> {
  final TextEditingController _usernameController = TextEditingController();
  final TextEditingController _passwordController = TextEditingController();
  String? _errorMessage;
  bool _isLoading = false;

  Future<void> login() async {
    setState(() {
      _isLoading = true; // Start loading
      _errorMessage = null; // Clear previous error
    });

    try {
      final response = await http.post(
        Uri.parse('http://localhost:5004/users/login'),
        headers: {
          'Content-Type': 'application/json',
        },
        body: json.encode({
          'username': _usernameController.text,
          'userPassword': _passwordController.text,
        }),
      );

      print('Response status: ${response.statusCode}'); // Log response status
      print('Response body: ${response.body}'); // Log response body

      if (response.statusCode == 200) {
        final responseData = json.decode(response.body);
        
        // Log the parsed response
        print('Parsed response: $responseData'); 

        // Check if the token exists in the response
        if (responseData.containsKey('token') && responseData['token'] != null) {
          final token = responseData['token'];
          
          // Store token in shared_preferences
          SharedPreferences prefs = await SharedPreferences.getInstance();
          await prefs.setString('token', token); // Store token for future requests

          // Handle successful login
          print('Login successful! Token: $token');
          Navigator.pushNamed(context, '/dashboard');
        } else {
          setState(() {
            _errorMessage = 'Login failed: Invalid response from server'; // Display error
          });
        }
      } else {
        setState(() {
          _errorMessage = 'Login failed: ${response.body}'; // Display error
        });
      }
    } catch (e) {
      setState(() {
        _errorMessage = 'An error occurred: $e'; // Display error
      });
    } finally {
      setState(() {
        _isLoading = false; // Stop loading
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text('Login'),
      ),
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            TextField(
              controller: _usernameController,
              decoration: InputDecoration(
                labelText: 'Username',
                errorText: _errorMessage,
              ),
            ),
            TextField(
              controller: _passwordController,
              decoration: InputDecoration(
                labelText: 'Password',
                errorText: _errorMessage,
              ),
              obscureText: true,
            ),
            SizedBox(height: 20),
            _isLoading
                ? CircularProgressIndicator()
                : ElevatedButton(
                    onPressed: login,
                    child: Text('Login'),
                  ),
          ],
        ),
      ),
    );
  }
}
