import 'dart:ui';

import 'package:flutter/material.dart';
import 'package:hms_web_app/globals.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';
import 'package:shared_preferences/shared_preferences.dart'; // Import shared_preferences

class ChangePasswordPage extends StatefulWidget {
  final String username; // Accept username
  final String role; // Accept role

  ChangePasswordPage({required this.username, required this.role});

  @override
  _ChangePasswordPageState createState() => _ChangePasswordPageState();
}

//Change password
class _ChangePasswordPageState extends State<ChangePasswordPage> {
  final _formKey = GlobalKey<FormState>();
  String _oldPassword = '';
  String _newPassword = '';
  String? _userID; // Variable to hold the user ID
  final String apiUrl = getApiPath('users').toString(); // Replace with your actual API URL

  @override
  void initState() {
    super.initState();
    _loadUserID(); // Load user ID when page initializes
  }

  Future<void> _loadUserID() async {
    SharedPreferences prefs = await SharedPreferences.getInstance();
    setState(() {
      _userID = prefs.getString('userID'); // Get the user ID from shared_preferences
    });
  }

  Future<void> _changePassword() async {
    if (_userID == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('User ID not found. Please log in again.')),
      );
      return;
    }

    final response = await http.put(
      Uri.parse('$apiUrl/$_userID'), // Use the stored user ID
      headers: {
        'Content-Type': 'application/json',
        // 'Authorization': 'Bearer YOUR_JWT_TOKEN', // Uncomment if needed
      },
      body: json.encode({
        'oldPassword': _oldPassword,
        'newPassword': _newPassword,
      }),
    );

    if (response.statusCode == 204) {
      // Success
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Password changed successfully!')),
      );
      Navigator.pushNamed(context, '/dashboard'); // Navigate to List Assignments page
    } else if (response.statusCode == 400) {
      // Bad request
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Invalid data provided. Please try again.')),
      );
    } else if (response.statusCode == 401) {
      // Unauthorized
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Unauthorized. Please log in again.')),
      );
    } else {
      // Handle other errors
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Failed to change password: ${response.body}')),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text("Change Password"),
      ),
      body: Padding(
        padding: EdgeInsets.all(16.0),
        child: Column(
          children: [
            // Updated Navigation Bar (without User Admin)
            Container(
              padding: EdgeInsets.symmetric(vertical: 10, horizontal: 20),
              color: Colors.black.withOpacity(0.7),
              child: Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Row(
                    children: [
                      TextButton(
                        onPressed: () {
                          Navigator.pushNamed(context, '/dashboard');
                        },
                        child: Text("View Assignments", style: TextStyle(color: Colors.white)),
                      ),
                      TextButton(
                        onPressed: () {
                          Navigator.pushNamed(context, '/createAssignment');
                        },
                        child: Text("Create Assignment", style: TextStyle(color: Colors.white)),
                      ),
                    ],
                  ),
                  TextButton(
                    onPressed: () {
                      Navigator.pushNamed(context, '/'); // Logout by redirecting to landing page
                    },
                    child: Text("Logout", style: TextStyle(color: Colors.white)),
                  ),
                ],
              ),
            ),
            SizedBox(height: 20), // Space between navbar and content
            // Form Fields
            Expanded(
              child: Form(
                key: _formKey,
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    TextFormField(
                      decoration: InputDecoration(labelText: "Old Password"),
                      obscureText: true,
                      onSaved: (value) => _oldPassword = value!,
                      validator: (value) {
                        if (value == null || value.isEmpty) {
                          return 'Please enter the old password';
                        }
                        return null;
                      },
                    ),
                    TextFormField(
                      decoration: InputDecoration(labelText: "New Password"),
                      obscureText: true,
                      onSaved: (value) => _newPassword = value!,
                      validator: (value) {
                        if (value == null || value.isEmpty) {
                          return 'Please enter the new password';
                        }
                        return null;
                      },
                    ),
                    SizedBox(height: 20),
                    Center(
                      child: ElevatedButton(
                        onPressed: () {
                          if (_formKey.currentState!.validate()) {
                            _formKey.currentState!.save();
                            _changePassword(); // Call the change password function
                          }
                        },
                        child: Text("Submit"),
                      ),
                    ),
                  ],
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
