import 'package:flutter/material.dart';
import 'package:hms_web_app/globals.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';

class ViewSubmissionsPage extends StatefulWidget {
  @override
  _ViewSubmissionsPageState createState() => _ViewSubmissionsPageState();
}

class _ViewSubmissionsPageState extends State<ViewSubmissionsPage> {
  Assignment? assignment;
  List<FeedbackFile> feedbackRequired = [];
  List<FeedbackFile> feedbackProvided = [];
  String? errorMessage;

  @override
  void initState() {
    super.initState();
    fetchAssignmentData();
  }

  //Get submissions
  Future<void> fetchAssignmentData() async {
    try {
      final response = await http.get(
        getApiPath('submissions'),
        headers: getHeaders(),
      );

      if (response.statusCode == 200) {
        final data = jsonDecode(response.body);
        setState(() {
          var ass = data['assignment'];
          assignment = Assignment.fromJson(ass);
          feedbackRequired = (data['feedbackRequired'] as List)
              .map((file) => FeedbackFile.fromJson(file))
              .toList();
          feedbackProvided = (data['feedbackProvided'] as List)
              .map((file) => FeedbackFile.fromJson(file))
              .toList();
        });
      } else {
        setState(() {
          errorMessage = 'Failed to load assignment data: ${response.reasonPhrase}';
        });
      }
    } catch (e) {
      setState(() {
        errorMessage = 'Error fetching assignment data: $e';
      });
    }
  }

  void downloadMarks(BuildContext context) async {
    final url = "YOUR_API_ENDPOINT_FOR_DOWNLOADING_MARKS"; // Replace with your actual download URL
    try {
      final response = await http.get(Uri.parse(url));

      if (response.statusCode == 200) {
        // Handle the file download here
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Marks downloaded successfully!')),
        );
      } else {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Failed to download marks: ${response.reasonPhrase}')),
        );
      }
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('An error occurred: $e')),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text("View Submissions"),
      ),
      body: Column(
        children: [
          // Navigation Bar
          Container(
            padding: EdgeInsets.symmetric(vertical: 10, horizontal: 20),
            color: Colors.black.withOpacity(0.7),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                TextButton(
                  onPressed: () {
                    Navigator.pushNamed(context, '/dashboard'); // Navigate to view assignments page
                  },
                  child: Text("View Assignments", style: TextStyle(color: Colors.white)),
                ),
                TextButton(
                  onPressed: () {
                    Navigator.pushNamed(context, '/'); // Logout to landing page
                  },
                  child: Text("Logout", style: TextStyle(color: Colors.white)),
                ),
              ],
            ),
          ),
          SizedBox(height: 20), // Space between navbar and content
          // Assignment Details
          if (assignment != null) ...[
            Padding(
              padding: const EdgeInsets.all(16.0),
              child: Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text("Module: ${assignment!.module}", style: TextStyle(fontSize: 18)),
                        SizedBox(height: 10),
                        Text("Assignment Title: ${assignment!.title}", style: TextStyle(fontSize: 18)),
                        SizedBox(height: 10),
                        Text("Open Date: ${assignment!.openDate.toLocal().toShortDateString()}", style: TextStyle(fontSize: 18)),
                        SizedBox(height: 10),
                        Text("Due Date: ${assignment!.dueDate.toLocal().toShortDateString()}", style: TextStyle(fontSize: 18)),
                        SizedBox(height: 10),
                        Text("Grade Scale: ${assignment!.gradeScale}", style: TextStyle(fontSize: 18)),
                      ],
                    ),
                  ),
                  ElevatedButton(
                    onPressed: () {
                      downloadMarks(context);
                    },
                    child: Text("Download Marks"),
                  ),
                ],
              ),
            ),
          ] else if (errorMessage != null) ...[
            Padding(
              padding: const EdgeInsets.all(8.0),
              child: Text(
                errorMessage!,
                style: TextStyle(color: Colors.red),
              ),
            ),
          ],
          SizedBox(height: 30), // Space between details and grids
          // Feedback Required Grid
          buildFeedbackGrid(context, "Feedback Required:", feedbackRequired),
          SizedBox(height: 30), // Space between grids
          // Feedback Provided Grid
          buildFeedbackGrid(context, "Feedback Provided:", feedbackProvided),
        ],
      ),
    );
  }

  Widget buildFeedbackGrid(BuildContext context, String title, List<FeedbackFile> feedbackFiles) {
    return Padding(
      padding: const EdgeInsets.all(16.0),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(title, style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold)),
          SizedBox(height: 10),
          DataTable(
            columns: const [
              DataColumn(label: Text("File")),
              DataColumn(label: Text("Student Number")),
              DataColumn(label: Text("Date Submitted")),
            ],
            rows: feedbackFiles.map((file) {
              return DataRow(cells: [
                DataCell(
                  GestureDetector(
                    onTap: () {
                      Navigator.pushNamed(context, '/watchVideo', arguments: file.fileName);
                    },
                    child: Text(
                      file.fileName,
                      style: TextStyle(
                        color: Colors.blue,
                        decoration: TextDecoration.underline,
                      ),
                    ),
                  ),
                ),
                DataCell(Text(file.studentNumber)),
                DataCell(Text(file.dateSubmitted)),
              ]);
            }).toList(),
          ),
        ],
      ),
    );
  }
}

class Assignment {
  final String module;
  final String title;
  final DateTime openDate;
  final DateTime dueDate;
  final int gradeScale;

  Assignment({required this.module, required this.title, required this.openDate, required this.dueDate, required this.gradeScale});

  factory Assignment.fromJson(Map<String, dynamic> json) {
    return Assignment(
      module: json['module'],
      title: json['title'],
      openDate: DateTime.parse(json['openDate']),
      dueDate: DateTime.parse(json['dueDate']),
      gradeScale: json['gradeScale'],
    );
  }
}

class FeedbackFile {
  final String fileName;
  final String studentNumber;
  final String dateSubmitted;

  FeedbackFile({required this.fileName, required this.studentNumber, required this.dateSubmitted});

  factory FeedbackFile.fromJson(Map<String, dynamic> json) {
    return FeedbackFile(
      fileName: json['fileName'],
      studentNumber: json['studentNumber'],
      dateSubmitted: json['dateSubmitted'],
    );
  }
}

// Extension method for formatting date
extension DateFormatting on DateTime {
  String toShortDateString() {
    return "${this.year}-${this.month.toString().padLeft(2, '0')}-${this.day.toString().padLeft(2, '0')}";
  }
}
