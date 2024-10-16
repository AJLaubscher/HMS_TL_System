import 'package:flutter/material.dart';
import 'package:hms_web_app/globals.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';

class CreateAssignmentsPage extends StatefulWidget {
  @override
  _CreateAssignmentsPageState createState() => _CreateAssignmentsPageState();
}

class _CreateAssignmentsPageState extends State<CreateAssignmentsPage> {
  final _formKey = GlobalKey<FormState>();
  final _moduleController = TextEditingController();
  final _titleController = TextEditingController();
  final _openDateController = TextEditingController();
  final _dueDateController = TextEditingController();
  final _gradeScaleController = TextEditingController();

  Future<void> _createAssignment() async {
    if (_formKey.currentState!.validate()) {
      final String module = _moduleController.text;
      final String title = _titleController.text;
      final String openDate = _openDateController.text;
      final String dueDate = _dueDateController.text;
      final int maxMarks = int.tryParse(_gradeScaleController.text) ?? 0;

      try {
        final response = await http.post(
          getApiPath('assignments'),
          headers: getHeaders(),
          body: jsonEncode(<String, dynamic>{
            'module': module,
            'title': title,
            'openDate': openDate,
            'dueDate': dueDate,
            'maxMarks': maxMarks,
          }),
        );

        if (response.statusCode == 201) {
          // If the server returns a 201 CREATED response, then navigate to another page
          Navigator.pushNamed(context, '/assignments'); // Change to the route you want
        } else {
          // If the server did not return a 201 CREATED response, show an error message
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(content: Text('Failed to create assignment: ${response.reasonPhrase}')),
          );
        }
      } catch (e) {
        // Handle any exceptions
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('An error occurred: $e')),
        );
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text("Create Assignment"),
      ),
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Form(
          key: _formKey,
          child: Column(
            children: [
              TextFormField(
                controller: _moduleController,
                decoration: InputDecoration(labelText: "Module"),
                validator: (value) {
                  if (value == null || value.isEmpty) {
                    return 'Please enter module name';
                  }
                  return null;
                },
              ),
              TextFormField(
                controller: _titleController,
                decoration: InputDecoration(labelText: "Assignment Title"),
                validator: (value) {
                  if (value == null || value.isEmpty) {
                    return 'Please enter assignment title';
                  }
                  return null;
                },
              ),
              TextFormField(
                controller: _openDateController,
                decoration: InputDecoration(labelText: "Open Date (YYYY-MM-DD)"),
                validator: (value) {
                  if (value == null || value.isEmpty) {
                    return 'Please enter open date';
                  }
                  return null;
                },
              ),
              TextFormField(
                controller: _dueDateController,
                decoration: InputDecoration(labelText: "Due Date (YYYY-MM-DD)"),
                validator: (value) {
                  if (value == null || value.isEmpty) {
                    return 'Please enter due date';
                  }
                  return null;
                },
              ),
              TextFormField(
                controller: _gradeScaleController,
                decoration: InputDecoration(labelText: "Grade Scale"),
                validator: (value) {
                  if (value == null || value.isEmpty) {
                    return 'Please enter grade scale';
                  }
                  return null;
                },
              ),
              SizedBox(height: 20),
              ElevatedButton(
                onPressed: _createAssignment,
                child: Text("Create Assignment"),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
