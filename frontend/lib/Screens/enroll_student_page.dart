import 'package:flutter/material.dart';
import 'package:hms_web_app/globals.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';

class EnrollStudentPage extends StatefulWidget {
  @override
  _EnrollStudentPageState createState() => _EnrollStudentPageState();
}

class _EnrollStudentPageState extends State<EnrollStudentPage> {
  final _formKey = GlobalKey<FormState>();
  late String username;
  late String name;
  late String surname;
  late String moduleId;
  late String moduleCode;
  late String moduleName;

  Future<void> enrollStudent() async {
    // Define the endpoint for enrollment
    //final url = Uri.parse('https://localhost:5004/enrolments'); // Update with your API URL
    final url = getApiPath('enrolments');     //Review please!!!


    // Prepare the request body
    final body = jsonEncode({
      'username': username,
      'name': name,
      'surname': surname,
      'moduleId': moduleId,
      'moduleCode': moduleCode,
      'moduleName': moduleName,
    });

    try {
      final response = await http.post(
        url,
        headers: getHeaders(),
        body: body,
      );

      if (response.statusCode == 201) {
        // If the server returns a 201 response, show a success message
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Student enrolled successfully!')),
        );
        Navigator.pop(context); // Go back to the previous page
      } else {
        // Handle different responses based on status code
        final responseBody = jsonDecode(response.body);
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Failed to enroll student: ${responseBody['message']}')),
        );
      }
    } catch (e) {
      // Show an error message
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Error enrolling student: $e')),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text("Enroll Student"),
      ),
      body: SingleChildScrollView(
        child: Padding(
          padding: const EdgeInsets.all(16.0),
          child: Form(
            key: _formKey,
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                // Navigation Bar
                Container(
                  padding: EdgeInsets.symmetric(vertical: 10, horizontal: 20),
                  color: Colors.black.withOpacity(0.7), // Semi-transparent background
                  child: Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Row(
                        children: [
                          TextButton(
                            onPressed: () {
                              Navigator.pushNamed(context, '/administration'); // Navigate to Administration page
                            },
                            child: Text("Admin", style: TextStyle(color: Colors.white)),
                          ),
                          TextButton(
                            onPressed: () {
                              Navigator.pushNamed(context, '/manageEnrollments'); // Navigate to Manage Enrollments page
                            },
                            child: Text("Manage Enrollments", style: TextStyle(color: Colors.white)),
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

                // Add spacing before the Find Student/Module button
                SizedBox(height: 20),

                // Find Student/Module Button
                Align(
                  alignment: Alignment.topRight, // Change alignment to top right
                  child: ElevatedButton(
                    onPressed: () {
                      Navigator.pushNamed(context, '/createStudentModuleEnrollments'); // Navigate to create_student_module_enrollments_page.dart
                    },
                    child: Text("Find Student/Module"),
                  ),
                ),

                // Add spacing after the Find Student/Module button
                SizedBox(height: 20),

                // Student Panel
                Card(
                  elevation: 2,
                  child: Padding(
                    padding: const EdgeInsets.all(16.0),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          "Student",
                          style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
                        ),
                        SizedBox(height: 10),
                        Row(
                          children: [
                            // Username and ID on the left
                            Expanded(
                              child: Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Text("Username"),
                                  TextFormField(
                                    onChanged: (value) => username = value,
                                    validator: (value) {
                                      if (value == null || value.isEmpty) {
                                        return 'Please enter a username';
                                      }
                                      return null;
                                    },
                                  ),
                                  SizedBox(height: 10),
                                  Text("ID"),
                                  TextFormField(
                                    onChanged: (value) => moduleId = value,
                                    validator: (value) {
                                      if (value == null || value.isEmpty) {
                                        return 'Please enter an ID';
                                      }
                                      return null;
                                    },
                                  ),
                                ],
                              ),
                            ),
                            SizedBox(width: 20),
                            // Name and Surname on the right
                            Expanded(
                              child: Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Text("Name"),
                                  TextFormField(
                                    onChanged: (value) => name = value,
                                    validator: (value) {
                                      if (value == null || value.isEmpty) {
                                        return 'Please enter a name';
                                      }
                                      return null;
                                    },
                                  ),
                                  SizedBox(height: 10),
                                  Text("Surname"),
                                  TextFormField(
                                    onChanged: (value) => surname = value,
                                    validator: (value) {
                                      if (value == null || value.isEmpty) {
                                        return 'Please enter a surname';
                                      }
                                      return null;
                                    },
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

                // Module Panel
                Card(
                  elevation: 2,
                  child: Padding(
                    padding: const EdgeInsets.all(16.0),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          "Module",
                          style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
                        ),
                        SizedBox(height: 10),
                        Row(
                          children: [
                            // Module Code and ID on the left
                            Expanded(
                              child: Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Text("Module Code"),
                                  TextFormField(
                                    onChanged: (value) => moduleCode = value,
                                    validator: (value) {
                                      if (value == null || value.isEmpty) {
                                        return 'Please enter a module code';
                                      }
                                      return null;
                                    },
                                  ),
                                  SizedBox(height: 10),
                                  Text("Module ID"),
                                  TextFormField(
                                    onChanged: (value) => moduleId = value,
                                    validator: (value) {
                                      if (value == null || value.isEmpty) {
                                        return 'Please enter a module ID';
                                      }
                                      return null;
                                    },
                                  ),
                                ],
                              ),
                            ),
                            SizedBox(width: 20),
                            // Module Name on the right
                            Expanded(
                              child: Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Text("Module Name"),
                                  TextFormField(
                                    onChanged: (value) => moduleName = value,
                                    validator: (value) {
                                      if (value == null || value.isEmpty) {
                                        return 'Please enter a module name';
                                      }
                                      return null;
                                    },
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

                // Submit Button
                Center(
                  child: ElevatedButton(
                    onPressed: () {
                      if (_formKey.currentState!.validate()) {
                        enrollStudent(); // Call the enroll function
                      }
                    },
                    child: Text("Submit"),
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
