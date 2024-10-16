
import 'package:flutter/material.dart';
import 'package:hms_web_app/globals.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';

class ManageModulePage extends StatefulWidget {
  @override
  _ManageModulePageState createState() => _ManageModulePageState();
}

class _ManageModulePageState extends State<ManageModulePage> {
  List<Map<String, String>> modules = [];
  bool isLoading = true; // Loading state
  String? errorMessage; // Error message

  @override
  void initState() {
    super.initState();
    fetchModules();
  }

  Future<void> fetchModules() async {
    setState(() {
      isLoading = true;
      errorMessage = null; // Reset error message
    });

    try {
      final response = await http.get(
        //Uri.parse('https://localhost:5004/modules'), // Replace with your actual API URL
        getApiPath('modules'),
        headers: getHeaders(),
      );

      if (response.statusCode >=  200 && response.statusCode < 300) {
        final List<dynamic> moduleData = jsonDecode(response.body);
        setState(() {
          modules = moduleData.map<Map<String, String>>((module) {
            return {
              // 'id': module['id'].toString(),
              // 'moduleCode': module['moduleCode'],
              // 'moduleName': module['moduleName'],
              // 'lecturerName': module['lecturerName'],
              // 'lecturerId': module['lecturerId'],
              'id': module['id'].toString(),
              'code': module['code'],
              'moduleName': module['modName'],
              'lecturerId': module['lectID'].toString(),
              'lecturerName': module['lectName'],
              'lecturerName': module['lectName'],
              'lecturer': module['lectName'],
            };
          }).toList();
        });
      } else {
        setState(() {
          errorMessage = 'Failed to load modules';
        });
      }
    } catch (e) {
      setState(() {
        errorMessage = 'Error fetching modules: $e';
      });
    } finally {
      setState(() {
        isLoading = false; // End loading state
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text("Manage Modules"),
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
                        Navigator.pushNamed(context, '/createModule'); // Navigate to Create Module page
                      },
                      child: Text("Create Module", style: TextStyle(color: Colors.white)),
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
          // Module DataTable
          Expanded(
            child: isLoading
                ? Center(child: CircularProgressIndicator()) // Loading indicator
                : errorMessage != null
                    ? Center(child: Text(errorMessage!)) // Show error message
                    : SingleChildScrollView(
                        child: DataTable(
                          columns: const [
                            DataColumn(label: Text("ID")),
                            DataColumn(label: Text("Module Code")),
                            DataColumn(label: Text("Module Name")),
                            DataColumn(label: Text("Lecturer Name")),
                            DataColumn(label: Text("Lecturer ID")),
                          ],
                          rows: modules.map((module) {
                            return DataRow(
                              cells: [
                                DataCell(Text(module['id']!)), // Display ID
                                DataCell(
                                  GestureDetector(
                                    onTap: () {
                                      // Navigate to update module page when the Module Code is clicked
                                      Navigator.pushNamed(context, '/updateModule', arguments: module['id']);
                                    },
                                    child: Text(
                                      module['code']!,
                                      style: TextStyle(
                                        color: Colors.blue, // Change color to indicate hyperlink
                                        decoration: TextDecoration.underline, // Underline for hyperlink effect
                                      ),
                                    ),
                                  ),
                                ),
                                DataCell(Text(module['moduleName']!)), // Display Module Name
                                DataCell(Text(module['lecturerName']!)), // Display Lecturer Name
                                DataCell(Text(module['lecturerId']!)), // Display Lecturer ID
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
