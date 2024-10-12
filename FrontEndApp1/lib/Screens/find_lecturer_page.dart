import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';

class FindLecturerPage extends StatefulWidget {
  @override
  _FindLecturerPageState createState() => _FindLecturerPageState();
}

class _FindLecturerPageState extends State<FindLecturerPage> {
  List<dynamic> lecturers = [];  // List to store lecturers fetched from the API
  bool isLoading = true;         // Show loading spinner until data is loaded
  String? errorMessage;

  // Selected lecturer variables
  String selectedLecturerId = '';
  String selectedLecturerUsername = '';
  String selectedLecturerName = '';

  @override
  void initState() {
    super.initState();
    fetchLecturers(); // Fetch the lecturers when the page loads
  }

  // Function to fetch lecturers from the API
  Future<void> fetchLecturers() async {
    try {
      // Added query parameter to filter for role = 2 (lecturers)
      final response = await http.get(
        Uri.parse('https://localhost:5004/users?role=2'), // Adjust the endpoint as needed
        headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer YOUR_TOKEN_HERE', // Add token if authentication is required
        },
      );

      if (response.statusCode == 200) {
        setState(() {
          lecturers = jsonDecode(response.body); // Parse response to list of lecturers
          isLoading = false; // Data loaded, stop the spinner
        });
      } else {
        setState(() {
          errorMessage = 'Failed to load lecturers';
          isLoading = false;
        });
      }
    } catch (e) {
      setState(() {
        errorMessage = 'Error fetching lecturers: $e';
        isLoading = false;
      });
    }
  }

  // Function to handle lecturer selection
  void selectLecturer(dynamic lecturer) {
    setState(() {
      selectedLecturerId = lecturer['lecturerId'];
      selectedLecturerUsername = lecturer['username'];
      selectedLecturerName = lecturer['fullName']; // Modify as per your API structure
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text("Find Lecturer"),
      ),
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
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
                          Navigator.pushNamed(context, '/manageEnrollments');
                        },
                        child: Text("Manage Enrollments", style: TextStyle(color: Colors.white)),
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

            SizedBox(height: 20),

            // Lecturer Panel
            Card(
              elevation: 2,
              child: Padding(
                padding: const EdgeInsets.all(16.0),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      "Lecturer",
                      style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
                    ),
                    SizedBox(height: 20),
                    isLoading
                        ? Center(child: CircularProgressIndicator()) // Show loading spinner
                        : errorMessage != null
                            ? Center(child: Text(errorMessage!, style: TextStyle(color: Colors.red)))
                            : Row(
                                children: [
                                  // Username dropdown and Lecturer Info
                                  Expanded(
                                    child: Column(
                                      crossAxisAlignment: CrossAxisAlignment.start,
                                      children: [
                                        Text('Username'),
                                        SizedBox(height: 5),
                                        DropdownButton<String>(
                                          value: selectedLecturerUsername.isEmpty
                                              ? null
                                              : selectedLecturerUsername,
                                          hint: Text("Select Username"),
                                          onChanged: (newValue) {
                                            setState(() {
                                              selectedLecturerUsername = newValue!;
                                              final selectedLecturer = lecturers.firstWhere(
                                                (lecturer) => lecturer['username'] == newValue,
                                              );
                                              selectLecturer(selectedLecturer); // Set lecturer details
                                            });
                                          },
                                          items: lecturers
                                              .map<DropdownMenuItem<String>>((lecturer) {
                                            return DropdownMenuItem<String>(
                                              value: lecturer['username'],
                                              child: Text(lecturer['username']),
                                            );
                                          }).toList(),
                                        ),
                                      ],
                                    ),
                                  ),
                                  SizedBox(width: 20),
                                  Expanded(
                                    child: Column(
                                      crossAxisAlignment: CrossAxisAlignment.start,
                                      children: [
                                        Text('ID'),
                                        SizedBox(height: 5),
                                        TextField(
                                          controller:
                                              TextEditingController(text: selectedLecturerId),
                                          readOnly: true,
                                        ),
                                      ],
                                    ),
                                  ),
                                  SizedBox(width: 20),
                                  Expanded(
                                    child: Column(
                                      crossAxisAlignment: CrossAxisAlignment.start,
                                      children: [
                                        Text('Name'),
                                        SizedBox(height: 5),
                                        TextField(
                                          controller:
                                              TextEditingController(text: selectedLecturerName),
                                          readOnly: true,
                                        ),
                                      ],
                                    ),
                                  ),
                                ],
                              ),
                  ],
                ),
              ),
            ),

            SizedBox(height: 20),

            // Select Button
            Center(
              child: ElevatedButton(
                onPressed: () {
                  // Navigate or perform the select action
                  Navigator.pushNamed(context, '/manageEnrollments');
                },
                child: Text("Select"),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
