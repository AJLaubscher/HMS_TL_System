import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';

class CreateUserPage extends StatefulWidget {
  @override
  _CreateUserPageState createState() => _CreateUserPageState();
}

class _CreateUserPageState extends State<CreateUserPage> {
  final TextEditingController _usernameController = TextEditingController();
  final TextEditingController _passwordController = TextEditingController();
  final TextEditingController _nameController = TextEditingController();
  final TextEditingController _surnameController = TextEditingController();
  String _selectedRole = '1'; // Default value for the role dropdown
  String? errorMessage;

  Future<void> createUser() async {
    final String username = _usernameController.text;
    final String password = _passwordController.text;
    final String name = _nameController.text;
    final String surname = _surnameController.text;
    final String role = _selectedRole;

    // Input validation
    if (username.isEmpty || password.isEmpty || name.isEmpty || surname.isEmpty) {
      setState(() {
        errorMessage = 'Please fill in all fields';
      });
      return;
    }

    final newUser = {
      'username': username,
      'password': password,
      'name': name,
      'surname': surname,
      'role': role,
    };

    try {
      final response = await http.post(
        Uri.parse('https://localhost:5004/users'), // Replace with your API URL
        headers: {
          'Content-Type': 'application/json',
          // 'Authorization': 'Bearer YOUR_TOKEN_HERE', // Uncomment if authentication is required
        },
        body: jsonEncode(newUser),
      );

      if (response.statusCode == 201) {
        // Successfully created the user
        Navigator.pushNamed(context, '/manageUser'); // Navigate to Manage Users page
      } else {
        setState(() {
          errorMessage = 'Failed to create user: ${response.reasonPhrase}';
        });
      }
    } catch (e) {
      setState(() {
        errorMessage = 'Error creating user: $e';
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text("Create User"),
      ),
      body: Column(
        children: [
          // Navigation Bar
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
                        Navigator.pushNamed(context, '/administration');
                      },
                      child: Text("Admin", style: TextStyle(color: Colors.white)),
                    ),
                    TextButton(
                      onPressed: () {
                        Navigator.pushNamed(context, '/manageUser');
                      },
                      child: Text("Manage Users", style: TextStyle(color: Colors.white)),
                    ),
                  ],
                ),
                TextButton(
                  onPressed: () {
                    Navigator.pushNamed(context, '/');
                  },
                  child: Text("Logout", style: TextStyle(color: Colors.white)),
                ),
              ],
            ),
          ),
          // User Creation Form
          Expanded(
            child: Padding(
              padding: const EdgeInsets.all(12.0), // Adjusted padding for smaller components
              child: Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  // Left Column
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        if (errorMessage != null) ...[
                          Text(
                            errorMessage!,
                            style: TextStyle(color: Colors.red),
                          ),
                          SizedBox(height: 8),
                        ],
                        Padding(
                          padding: const EdgeInsets.only(bottom: 8.0),
                          child: Text("Username"),
                        ),
                        TextField(
                          controller: _usernameController,
                          decoration: InputDecoration(
                            isDense: true, // Make the input field smaller
                            border: OutlineInputBorder(),
                          ),
                        ),
                        SizedBox(height: 12), // Added spacing between components

                        Padding(
                          padding: const EdgeInsets.only(bottom: 8.0, top: 16.0),
                          child: Text("Role"),
                        ),
                        DropdownButtonFormField<String>(
                          value: _selectedRole,
                          items: [
                            DropdownMenuItem(value: '1', child: Text("Admin")),
                            DropdownMenuItem(value: '2', child: Text("Lecturer")),
                            DropdownMenuItem(value: '3', child: Text("Student")),
                          ],
                          onChanged: (value) {
                            if (value != null) {
                              setState(() {
                                _selectedRole = value;
                              });
                            }
                          },
                          decoration: InputDecoration(
                            isDense: true, // Make the dropdown smaller
                            border: OutlineInputBorder(),
                          ),
                        ),
                        SizedBox(height: 12), // Added spacing

                        Padding(
                          padding: const EdgeInsets.only(bottom: 8.0, top: 16.0),
                          child: Text("Password"),
                        ),
                        TextField(
                          controller: _passwordController,
                          obscureText: true,
                          decoration: InputDecoration(
                            isDense: true, // Smaller input field
                            border: OutlineInputBorder(),
                          ),
                        ),
                      ],
                    ),
                  ),
                  SizedBox(width: 20), // Added spacing between columns
                  // Right Column
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Padding(
                          padding: const EdgeInsets.only(bottom: 8.0),
                          child: Text("Name"),
                        ),
                        TextField(
                          controller: _nameController,
                          decoration: InputDecoration(
                            isDense: true,
                            border: OutlineInputBorder(),
                          ),
                        ),
                        SizedBox(height: 12), // Added spacing between components

                        Padding(
                          padding: const EdgeInsets.only(bottom: 8.0, top: 16.0),
                          child: Text("Surname"),
                        ),
                        TextField(
                          controller: _surnameController,
                          decoration: InputDecoration(
                            isDense: true,
                            border: OutlineInputBorder(),
                          ),
                        ),
                      ],
                    ),
                  ),
                ],
              ),
            ),
          ),
          // Submit Button
          Padding(
            padding: const EdgeInsets.symmetric(vertical: 8.0, horizontal: 16.0), // Adjusted padding
            child: SizedBox(
              width: 140, // Adjusted button width
              child: ElevatedButton(
                onPressed: createUser, // Call createUser on button press
                child: Text("Submit"),
              ),
            ),
          ),
        ],
      ),
    );
  }

  @override
  void dispose() {
    _usernameController.dispose();
    _passwordController.dispose();
    _nameController.dispose();
    _surnameController.dispose();
    super.dispose();
  }
}
