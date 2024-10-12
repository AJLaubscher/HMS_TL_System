import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';

class UpdateUserPage extends StatefulWidget {
  final int userId; // Receive the user ID dynamically

  UpdateUserPage({required this.userId}); // Constructor to accept userId

  @override
  _UpdateUserPageState createState() => _UpdateUserPageState();
}

class _UpdateUserPageState extends State<UpdateUserPage> {
  final TextEditingController _usernameController = TextEditingController();
  final TextEditingController _passwordController = TextEditingController();
  final TextEditingController _nameController = TextEditingController();
  final TextEditingController _surnameController = TextEditingController();
  String? _selectedRole;
  String? errorMessage;

  @override
  void initState() {
    super.initState();
    _fetchUserData(); // Fetch user data on initialization
  }

  // Function to fetch user data
  Future<void> _fetchUserData() async {
    try {
      final response = await http.get(
        Uri.parse('https://your-api-url/users/${widget.userId}'), // Replace with your API URL and userId
        headers: {
          'Authorization': 'Bearer YOUR_TOKEN_HERE', // Add your token if needed
        },
      );

      if (response.statusCode == 200) {
        var userData = json.decode(response.body);
        setState(() {
          _usernameController.text = userData['username'] ?? '';
          _nameController.text = userData['name'] ?? '';
          _surnameController.text = userData['surname'] ?? '';
          _selectedRole = userData['role'];
        });
      } else {
        setState(() {
          errorMessage = 'Failed to load user data: ${response.reasonPhrase}';
        });
      }
    } catch (e) {
      setState(() {
        errorMessage = 'Error fetching user data: $e';
      });
    }
  }

  // Function to update the user using the PUT endpoint
  Future<void> updateUser() async {
    final updatedUser = {
      'username': _usernameController.text,
      'name': _nameController.text,
      'surname': _surnameController.text,
      'role': _selectedRole,
      'password': _passwordController.text, // Include password if provided
    };

    try {
      final response = await http.put(
        Uri.parse('https://your-api-url/users/${widget.userId}'), // Replace with your API URL and userId
        headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer YOUR_TOKEN_HERE', // Add your token if needed
        },
        body: jsonEncode(updatedUser),
      );

      if (response.statusCode == 204) {
        // Successfully updated the user
        Navigator.pushNamed(context, '/manageUser'); // Navigate back to Manage User page
      } else {
        setState(() {
          errorMessage = 'Failed to update user: ${response.reasonPhrase}';
        });
      }
    } catch (e) {
      setState(() {
        errorMessage = 'Error updating user: $e';
      });
    }
  }

  void updateUserIfValid() {
    if (_usernameController.text.isEmpty ||
        _nameController.text.isEmpty ||
        _surnameController.text.isEmpty ||
        _selectedRole == null) {
      setState(() {
        errorMessage = 'Please fill in all fields';
      });
    } else {
      updateUser(); // Call updateUser on button press
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text("Update User"),
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
          // User Update Form
          Expanded(
            child: Padding(
              padding: const EdgeInsets.all(12.0), // Adjusted padding
              child: Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        // Username
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
                        SizedBox(height: 12), // Added spacing between fields

                        // Role
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
                        
                        // Password
                        Padding(
                          padding: const EdgeInsets.only(bottom: 8.0, top: 16.0),
                          child: Text("Password (leave blank to keep unchanged)"),
                        ),
                        TextField(
                          controller: _passwordController,
                          obscureText: true,
                          decoration: InputDecoration(
                            isDense: true,
                            border: OutlineInputBorder(),
                          ),
                        ),
                      ],
                    ),
                  ),
                  SizedBox(width: 20), // Added spacing between columns
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        // Name
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
                        SizedBox(height: 12), // Added spacing between fields

                        // Surname
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
            padding: const EdgeInsets.symmetric(vertical: 8.0, horizontal: 16.0),
            child: SizedBox(
              width: 140, // Adjusted button size
              child: ElevatedButton(
                onPressed: updateUserIfValid, // Use the validation method
                child: Text("Submit"),
              ),
            ),
          ),
          // Display error message if any
          if (errorMessage != null) ...[
            Padding(
              padding: const EdgeInsets.all(8.0),
              child: Text(
                errorMessage!,
                style: TextStyle(color: Colors.red),
              ),
            ),
          ],
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

