import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';

class CreateModulePage extends StatefulWidget {
  @override
  _CreateModulePageState createState() => _CreateModulePageState();
}

class _CreateModulePageState extends State<CreateModulePage> {
  final TextEditingController _moduleCodeController = TextEditingController();
  final TextEditingController _lecturerIdController = TextEditingController();
  final TextEditingController _moduleNameController = TextEditingController();

  String? errorMessage;

  // Function to create a new module
  Future<void> createModule() async {
    final String moduleCode = _moduleCodeController.text;
    final String moduleName = _moduleNameController.text;
    final String lecturerId = _lecturerIdController.text;

    // Input validation
    if (moduleCode.isEmpty || moduleName.isEmpty || lecturerId.isEmpty) {
      setState(() {
        errorMessage = 'Please fill in all fields';
      });
      return;
    }

    final newModule = {
      'moduleCode': moduleCode,
      'moduleName': moduleName,
      'lecturerId': lecturerId,
    };

    try {
      final response = await http.post(
        Uri.parse('https://localhost:5004/modules'), // Replace with your API URL
        headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer YOUR_TOKEN_HERE', // Add your token if needed
        },
        body: jsonEncode(newModule),
      );

      if (response.statusCode == 201) {
        // Successfully created the module
        Navigator.pushNamed(context, '/manageModule'); // Navigate to Manage Module page
      } else {
        setState(() {
          errorMessage = 'Failed to create module: ${response.reasonPhrase}';
        });
      }
    } catch (e) {
      setState(() {
        errorMessage = 'Error creating module: $e';
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text("Create Module"),
      ),
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            if (errorMessage != null) ...[
              Text(
                errorMessage!,
                style: TextStyle(color: Colors.red),
              ),
              SizedBox(height: 8),
            ],
            TextField(
              controller: _moduleCodeController,
              decoration: InputDecoration(
                labelText: "Module Code",
                border: OutlineInputBorder(),
              ),
            ),
            SizedBox(height: 16),
            TextField(
              controller: _moduleNameController,
              decoration: InputDecoration(
                labelText: "Module Name",
                border: OutlineInputBorder(),
              ),
            ),
            SizedBox(height: 16),
            TextField(
              controller: _lecturerIdController,
              decoration: InputDecoration(
                labelText: "Lecturer ID",
                border: OutlineInputBorder(),
              ),
            ),
            SizedBox(height: 20),
            ElevatedButton(
              onPressed: createModule, // Call createModule on button press
              child: Text("Create Module"),
              style: ElevatedButton.styleFrom(
                padding: EdgeInsets.symmetric(horizontal: 24, vertical: 12), // Adjust padding for button size
              ),
            ),
          ],
        ),
      ),
    );
  }
}
