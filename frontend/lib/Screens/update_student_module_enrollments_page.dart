import 'package:flutter/material.dart';
import 'package:hms_web_app/globals.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';

class UpdateStudentModuleEnrollmentsPage extends StatefulWidget {
  @override
  _UpdateStudentModuleEnrollmentsPageState createState() =>
      _UpdateStudentModuleEnrollmentsPageState();
}

class _UpdateStudentModuleEnrollmentsPageState
    extends State<UpdateStudentModuleEnrollmentsPage> {
  // Lists to hold student and module data
  List<Map<String, dynamic>> students = [];
  List<Map<String, dynamic>> modules = [];

  // Selected student and module variables
  String selectedStudentId = '';
  String selectedStudentUsername = '';
  String selectedStudentName = '';

  String selectedModuleId = '';
  String selectedModuleCode = '';
  String selectedModuleName = '';

  @override
  void initState() {
    super.initState();
    fetchStudents();
    fetchModules();
  }

  Future<void> fetchStudents() async {
    // final url = Uri.parse('https://localhost:5004/students?role=3'); // Added query parameter to filter by role 3 (students)
    final url = getApiPath('students?role=3');    //Review!

    try {
      final response = await http.get(url);
      if (response.statusCode == 200) {
        final data = jsonDecode(response.body) as List;
        setState(() {
          students = data.map<Map<String, dynamic>>((student) => {
            'id': student['id'].toString(),
            'username': student['username'],
            'name': student['name'],
          }).toList();
        });
      } else {
        throw Exception('Failed to load students');
      }
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Error fetching students: $e')),
      );
    }
  }

  Future<void> fetchModules() async {
    // final url = Uri.parse('https://localhost:5004/modules'); // Replace with your API URL
    final url = getApiPath('modules');

    try {
      final response = await http.get(url);
      if (response.statusCode == 200) {
        final data = jsonDecode(response.body) as List;
        setState(() {
          modules = data.map<Map<String, dynamic>>((module) => {
            'id': module['id'].toString(),
            'moduleCode': module['moduleCode'],
            'moduleName': module['moduleName'],
          }).toList();
        });
      } else {
        throw Exception('Failed to load modules');
      }
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Error fetching modules: $e')),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text("Update Student/Module Enrollments"),
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
                    SizedBox(height: 20),
                    Row(
                      children: [
                        // Username aligned to the left center
                        Expanded(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text('Username'),
                              SizedBox(height: 5),
                              DropdownButton<String>(
                                value: selectedStudentUsername.isEmpty ? null : selectedStudentUsername,
                                hint: Text("Select Username"),
                                onChanged: (newValue) {
                                  setState(() {
                                    selectedStudentUsername = newValue!;
                                    final selectedStudent = students.firstWhere(
                                      (student) => student['username'] == newValue,
                                    );
                                    selectedStudentId = selectedStudent['id']!;
                                    selectedStudentName = selectedStudent['name']!;
                                  });
                                },
                                items: students.map<DropdownMenuItem<String>>((student) {
                                  return DropdownMenuItem<String>(
                                    value: student['username'],
                                    child: Text(student['username']),
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
                                controller: TextEditingController(text: selectedStudentId),
                                readOnly: true,
                              ),
                            ],
                          ),
                        ),
                        SizedBox(width: 20),
                        // Name aligned to the right center
                        Expanded(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text('Name'),
                              SizedBox(height: 5),
                              TextField(
                                controller: TextEditingController(text: selectedStudentName),
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
                    SizedBox(height: 20),
                    Row(
                      children: [
                        // Module Code aligned to the left center
                        Expanded(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text('Module Code'),
                              SizedBox(height: 5),
                              DropdownButton<String>(
                                value: selectedModuleCode.isEmpty ? null : selectedModuleCode,
                                hint: Text("Select Module Code"),
                                onChanged: (newValue) {
                                  setState(() {
                                    selectedModuleCode = newValue!;
                                    final selectedModule = modules.firstWhere(
                                      (module) => module['moduleCode'] == newValue,
                                    );
                                    selectedModuleId = selectedModule['id']!;
                                    selectedModuleName = selectedModule['moduleName']!;
                                  });
                                },
                                items: modules.map<DropdownMenuItem<String>>((module) {
                                  return DropdownMenuItem<String>(
                                    value: module['moduleCode'],
                                    child: Text(module['moduleCode']),
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
                                controller: TextEditingController(text: selectedModuleId),
                                readOnly: true,
                              ),
                            ],
                          ),
                        ),
                        SizedBox(width: 20),
                        // Module Name aligned to the right center
                        Expanded(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text('Name'),
                              SizedBox(height: 5),
                              TextField(
                                controller: TextEditingController(text: selectedModuleName),
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

            // Update Button
            Center(
              child: ElevatedButton(
                onPressed: () {
                  Navigator.pushNamed(context, '/updateEnrollments');
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
