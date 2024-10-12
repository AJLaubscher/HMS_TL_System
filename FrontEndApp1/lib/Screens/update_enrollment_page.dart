import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';

class UpdateEnrollmentPage extends StatefulWidget {
  @override
  _UpdateEnrollmentPageState createState() => _UpdateEnrollmentPageState();
}

class _UpdateEnrollmentPageState extends State<UpdateEnrollmentPage> {
  final _formKey = GlobalKey<FormState>();
  late String username;
  late String name;
  late String surname;
  late String moduleId;
  late String moduleCode;
  late String moduleName;
  late String studId; // New variable for student ID
  bool _isLoading = false; // Loading state

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();
    final args = ModalRoute.of(context)!.settings.arguments as Map<String, dynamic>;
    username = args['username'];
    name = args['name'];
    surname = args['surname'];
    moduleId = args['moduleId'];
    moduleCode = args['moduleCode'];
    moduleName = args['moduleName'];
    studId = args['studId']; // Extract student ID from arguments
  }

  Future<void> updateEnrollment() async {
    setState(() {
      _isLoading = true; // Set loading state to true
    });

    final response = await http.put(
      Uri.parse('https://localhost:5004/enrolments/$moduleId/$studId'), // Update the URL to include modId and studId
      headers: <String, String>{
        'Content-Type': 'application/json; charset=UTF-8',
      },
      body: jsonEncode(<String, dynamic>{
        'username': username,
        'name': name,
        'surname': surname,
        'moduleId': moduleId,
        'moduleCode': moduleCode,
        'moduleName': moduleName,
      }),
    );

    setState(() {
      _isLoading = false; // Set loading state to false
    });

    if (response.statusCode == 200) {
      // Successfully updated
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Enrollment updated successfully!')),
      );
      Navigator.pop(context); // Go back to the previous page
    } else {
      // Failed to update
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Failed to update enrollment: ${response.reasonPhrase}')),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text("Update Enrollment"),
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16.0),
        child: Form(
          key: _formKey,
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
                      Text("Username"),
                      TextFormField(
                        initialValue: username,
                        enabled: false,
                      ),
                      SizedBox(height: 10),
                      Text("ID"),
                      TextFormField(
                        initialValue: studId, // Use studId for the student ID
                        enabled: false,
                      ),
                      SizedBox(height: 10),
                      Text("Name"),
                      TextFormField(
                        initialValue: name,
                        onChanged: (value) => name = value,
                        validator: (value) => value!.isEmpty ? 'Please enter your name' : null, // Validation
                      ),
                      SizedBox(height: 10),
                      Text("Surname"),
                      TextFormField(
                        initialValue: surname,
                        onChanged: (value) => surname = value,
                        validator: (value) => value!.isEmpty ? 'Please enter your surname' : null, // Validation
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
                      Text("Module Code"),
                      TextFormField(
                        initialValue: moduleCode,
                        enabled: false,
                      ),
                      SizedBox(height: 10),
                      Text("Module ID"),
                      TextFormField(
                        initialValue: moduleId,
                        enabled: false,
                      ),
                      SizedBox(height: 10),
                      Text("Module Name"),
                      TextFormField(
                        initialValue: moduleName,
                        onChanged: (value) => moduleName = value,
                        validator: (value) => value!.isEmpty ? 'Please enter module name' : null, // Validation
                      ),
                    ],
                  ),
                ),
              ),

              SizedBox(height: 20),

              // Submit Button
              Center(
                child: _isLoading
                    ? CircularProgressIndicator() // Show loading indicator
                    : ElevatedButton(
                        onPressed: () {
                          if (_formKey.currentState!.validate()) {
                            updateEnrollment(); // Call the update function
                          }
                        },
                        child: Text("Submit"),
                      ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
