import 'package:flutter/material.dart';
import 'package:hms_web_app/globals.dart';
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
    final String modCode = _moduleCodeController.text;
    final String modName = _moduleNameController.text;
    final String lectID = _lecturerIdController.text;

    // Input validation
    if (modCode.isEmpty || modName.isEmpty || lectID.isEmpty) {
      setState(() {
        errorMessage = 'Please fill in all fields';
      });
      return;
    }

    final newModule = {
      'Code': modCode,
      'ModName': modName,
      'LectID': lectID,
    };

    try {
      final response = await http.post(
        getApiPath('modules'),
        headers: getHeaders(),
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
