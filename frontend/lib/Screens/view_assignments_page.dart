import 'package:flutter/material.dart';
import 'package:hms_web_app/globals.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';

class ViewAssignmentVideosPage extends StatefulWidget {
  @override
  _ViewAssignmentVideosPageState createState() => _ViewAssignmentVideosPageState();
}

class _ViewAssignmentVideosPageState extends State<ViewAssignmentVideosPage> {
  List<dynamic> _assignmentList = []; // Changed variable name
  String? errorMessage;

  @override
  void initState() {
    super.initState();
    fetchAssignments(); // Fetch assignments instead of videos
  }

  // Fetch assignments from the endpoint
  Future<void> fetchAssignments() async {
    try {
      final response = await http.get(
        Uri.parse('https://your-api-url/assignments'), // Replace with your actual API URL
        headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer YOUR_TOKEN_HERE', // Add your token if needed
        },
      );

      if (response.statusCode == 200) {
        setState(() {
          _assignmentList = jsonDecode(response.body); // Assume the API returns a list of assignments
        });
      } else {
        setState(() {
          errorMessage = 'Failed to load assignments: ${response.reasonPhrase}';
        });
      }
    } catch (e) {
      setState(() {
        errorMessage = 'Error fetching assignments: $e';
      });
    }
  }

  // The submitFeedback method may remain unchanged unless specific to videos
  Future<void> submitFeedback(String assignmentId, String feedback) async {
    final feedbackData = {      //Review!!! Feedback data should be LecID, SubID, Comment and MarkAchieved ????
      'assignmentId': assignmentId,
      'feedback': feedback,
    };

    try {
      final response = await http.post(
        getApiPath('assignments/$assignmentId/feedback'),
        headers: getHeaders(),
        body: jsonEncode(feedbackData),
      );

      if (response.statusCode == 200) {
        // Successfully submitted feedback
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Feedback submitted successfully!')),
        );
      } else {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Failed to submit feedback: ${response.reasonPhrase}')),
        );
      }
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Error submitting feedback: $e')),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text('Assignments'),
      ),
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Text(
              'Assignments List',
              style: TextStyle(fontSize: 18),
            ),
            // Display error message if any
            if (errorMessage != null) ...[
              Padding(
                padding: const EdgeInsets.all(8.0),
                child: Text(
                  errorMessage!,
                  style: TextStyle(color: Colors.red),
                ),
              ),
            ],
            // Placeholder for assignment list
            Padding(
              padding: const EdgeInsets.all(20.0),
              child: _assignmentList.isEmpty
                  ? CircularProgressIndicator() // Show a loading indicator while fetching
                  : Column(
                      children: _assignmentList.map<Widget>((assignment) {
                        return Card(
                          child: ListTile(
                            title: Text(assignment['title']), // Adjust according to your assignment data structure
                            subtitle: Text(assignment['description']), // Adjust according to your assignment data structure
                            onTap: () {
                              // Example feedback input; adjust as necessary
                              _showFeedbackDialog(assignment['id']); // Assuming each assignment has an ID
                            },
                          ),
                        );
                      }).toList(),
                    ),
            ),
          ],
        ),
      ),
    );
  }

  void _showFeedbackDialog(String assignmentId) {
    TextEditingController feedbackController = TextEditingController();
    showDialog(
      context: context,
      builder: (BuildContext context) {
        return AlertDialog(
          title: Text("Submit Feedback"),
          content: TextField(
            controller: feedbackController,
            decoration: InputDecoration(hintText: "Enter your feedback"),
          ),
          actions: [
            TextButton(
              onPressed: () {
                submitFeedback(assignmentId, feedbackController.text);
                Navigator.of(context).pop();
              },
              child: Text("Submit"),
            ),
            TextButton(
              onPressed: () {
                Navigator.of(context).pop();
              },
              child: Text("Cancel"),
            ),
          ],
        );
      },
    );
  }
}
