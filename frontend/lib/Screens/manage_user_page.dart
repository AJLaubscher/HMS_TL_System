import 'package:flutter/material.dart';
import 'package:hms_web_app/globals.dart';
import 'package:http/http.dart' as http;
import 'dart:convert'; // To decode JSON data

class ManageUserPage extends StatefulWidget {
  @override
  _ManageUserPageState createState() => _ManageUserPageState();
}

class _ManageUserPageState extends State<ManageUserPage> {
  List<dynamic> users = []; // Dynamic list to store users
  bool isLoading = true; // Show loading spinner until data is fetched
  String? errorMessage; // Handle errors during API call

  @override
  void initState() {
    super.initState();
    fetchUsers(); // Fetch users when the page loads
  }

  // Fetch users from API (Modify the endpoint URL as needed)
  Future<void> fetchUsers() async {
    try {
      final response = await http.get(
        getApiPath('users'), // Replace with your actual GET endpoint
        headers: getHeaders(),
      );

      if (response.statusCode == 200) {
        setState(() {
          users = json.decode(response.body); // Parse response body into a dynamic list
          isLoading = false; // Stop showing the loading spinner
        });
      } else {
        setState(() {
          errorMessage = 'Failed to load users';
          isLoading = false;
        });
      }
    } catch (e) {
      setState(() {
        errorMessage = 'Error fetching users: $e';
        isLoading = false;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text("Manage Users"),
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
                        Navigator.pushNamed(context, '/createUser');
                      },
                      child: Text("Create User", style: TextStyle(color: Colors.white)),
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
          // User Data Table
          Expanded(
            child: isLoading
                ? Center(child: CircularProgressIndicator()) // Show spinner while loading
                : errorMessage != null
                    ? Center(child: Text(errorMessage!, style: TextStyle(color: Colors.red)))
                    : SingleChildScrollView(
                        child: DataTable(
                          columns: const [
                            DataColumn(label: Text("ID")),
                            DataColumn(label: Text("Username")),
                            DataColumn(label: Text("Name")),
                            DataColumn(label: Text("Surname")),
                            DataColumn(label: Text("Role")),
                            DataColumn(label: Text("Password")),
                          ],
                          rows: users.map((user) {
                            return DataRow(
                              cells: [
                                DataCell(Text(user['id'])),
                                DataCell(
                                  TextButton(
                                    onPressed: () {
                                      Navigator.pushNamed(context, '/updateUser', arguments: user);
                                    },
                                    child: Text(
                                      user['username'],
                                      style: TextStyle(
                                        color: Colors.blue,
                                        decoration: TextDecoration.underline,
                                      ),
                                    ),
                                  ),
                                ),
                                // DataCell(Text(user['fName'])), // Assuming 'firstName' is in API response
                                // DataCell(Text(user['lName'])),   // Assuming 'surname' is in API response
                                // DataCell(Text(user['userRole'])),
                                // DataCell(Text(user['userPassword'])),

                                DataCell(Text(user['fName']!)), // Assuming 'firstName' is in API response
                                DataCell(Text(user['lName']!)),   // Assuming 'surname' is in API response
                                DataCell(Text(user['userRole']!)),
                                DataCell(Text(user['userPassword']!)),

                              ],
                            );
                          }).toList(),
                        ),
                      ),
          ),
        ],
      ),
    );
  }
}
