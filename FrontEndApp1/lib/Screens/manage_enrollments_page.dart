import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';

class Enrollment {
  final int id;
  final String username;
  final String name;
  final int moduleId;
  final String moduleCode;
  final String moduleName;

  Enrollment({
    required this.id,
    required this.username,
    required this.name,
    required this.moduleId,
    required this.moduleCode,
    required this.moduleName,
  });

  factory Enrollment.fromJson(Map<String, dynamic> json) {
    return Enrollment(
      id: json['id'],
      username: json['username'],
      name: json['name'],
      moduleId: json['moduleId'],
      moduleCode: json['moduleCode'],
      moduleName: json['moduleName'],
    );
  }
}

class ManageEnrollmentsPage extends StatelessWidget {
  Future<List<Enrollment>> fetchEnrollments() async {
    final response = await http.get(Uri.parse('https://localhost:5004/enrolments')); // Replace with your API URL

    if (response.statusCode == 200) {
      List<dynamic> data = json.decode(response.body);
      return data.map((json) => Enrollment.fromJson(json)).toList();
    } else {
      throw Exception('Failed to load enrollments');
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text("Manage Enrollments"),
      ),
      body: Column(
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
                        Navigator.pushNamed(context, '/enrollStudent'); // Navigate to Enroll Student page
                      },
                      child: Text("Enroll Student", style: TextStyle(color: Colors.white)),
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
          // Enrollments Table
          Expanded(
            child: FutureBuilder<List<Enrollment>>(
              future: fetchEnrollments(), // Fetch enrollments from API
              builder: (context, snapshot) {
                if (snapshot.connectionState == ConnectionState.waiting) {
                  return Center(child: CircularProgressIndicator());
                } else if (snapshot.hasError) {
                  return Center(child: Text('Error: ${snapshot.error}'));
                } else {
                  final enrollments = snapshot.data!;
                  return SingleChildScrollView(
                    child: DataTable(
                      columns: const [
                        DataColumn(label: Text("ID")),
                        DataColumn(label: Text("Username")),
                        DataColumn(label: Text("Name")),
                        DataColumn(label: Text("Module ID")),
                        DataColumn(label: Text("Module Code")),
                        DataColumn(label: Text("Module Name")),
                      ],
                      rows: enrollments.map((enrollment) {
                        return DataRow(cells: [
                          DataCell(Text(enrollment.id.toString())),
                          DataCell(
                            GestureDetector(
                              onTap: () {
                                Navigator.pushNamed(
                                  context,
                                  '/updateEnrollments',
                                  arguments: {
                                    'username': enrollment.username,
                                    'name': enrollment.name,
                                    'moduleId': enrollment.moduleId,
                                    'moduleCode': enrollment.moduleCode,
                                    'moduleName': enrollment.moduleName,
                                  },
                                );
                              },
                              child: Text(
                                enrollment.username,
                                style: TextStyle(
                                  color: Colors.blue,
                                  decoration: TextDecoration.underline,
                                ),
                              ),
                            ),
                          ),
                          DataCell(Text(enrollment.name)),
                          DataCell(Text(enrollment.moduleId.toString())),
                          DataCell(Text(enrollment.moduleCode)),
                          DataCell(Text(enrollment.moduleName)),
                        ]);
                      }).toList(),
                    ),
                  );
                }
              },
            ),
          ),
        ],
      ),
    );
  }
}
