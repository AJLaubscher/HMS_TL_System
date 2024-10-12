import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';

class UpdateModulePage extends StatefulWidget {
  final int moduleId; // Receive the module ID dynamically
  final String moduleName; // Added
  final String moduleCode; // Added
  final String lecturerId; // Added

  UpdateModulePage({
    required this.moduleId,
    required this.moduleName,
    required this.moduleCode,
    required this.lecturerId,
  });

  @override
  _UpdateModulePageState createState() => _UpdateModulePageState();
}

class _UpdateModulePageState extends State<UpdateModulePage> {
  final TextEditingController _moduleCodeController = TextEditingController();
  final TextEditingController _lecturerIdController = TextEditingController();
  final TextEditingController _moduleNameController = TextEditingController();

  String? errorMessage;

  @override
  void initState() {
    super.initState();
    // Pre-fill the fields with the passed arguments
    _moduleCodeController.text = widget.moduleCode;
    _moduleNameController.text = widget.moduleName;
    _lecturerIdController.text = widget.lecturerId;
  }

  // Function to update the module using the PUT endpoint
  Future<void> updateModule() async {
    final updatedModule = {
      'code': _moduleCodeController.text,
      'modName': _moduleNameController.text,
      'lectID': int.tryParse(_lecturerIdController.text) ?? 0,
      'deleted': false,
    };

    try {
      final response = await http.put(
        Uri.parse('https://localhost:5004/modules/${widget.moduleId}'), // Replace with your API URL
        headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer YOUR_TOKEN_HERE', // Add your token if needed
        },
        body: jsonEncode(updatedModule),
      );

      if (response.statusCode == 204) {
        // Successfully updated the module
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Module updated successfully!')),
        );
        Navigator.pushNamed(context, '/manageModule'); // Navigate back to Manage Module page
      } else {
        setState(() {
          errorMessage = 'Failed to update module: ${response.reasonPhrase}';
        });
      }
    } catch (e) {
      setState(() {
        errorMessage = 'Error updating module: $e';
      });
    }
  }

  void updateModuleIfValid() {
    if (_moduleCodeController.text.isEmpty || 
        _moduleNameController.text.isEmpty || 
        _lecturerIdController.text.isEmpty) {
      setState(() {
        errorMessage = 'Please fill in all fields';
      });
    } else {
      updateModule(); // Call updateModule on button press
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text("Update Module"),
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
              keyboardType: TextInputType.number, // Optional: Set to number keyboard for lecturer ID
            ),
            SizedBox(height: 20),
            ElevatedButton(
              onPressed: updateModuleIfValid, // Use the validation method
              child: Text("Update Module"),
              style: ElevatedButton.styleFrom(
                padding: EdgeInsets.symmetric(horizontal: 24, vertical: 12),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
