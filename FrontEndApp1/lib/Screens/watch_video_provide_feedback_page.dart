import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'package:url_launcher/url_launcher.dart'; // For launching the browser to download the file

class WatchVideoPage extends StatelessWidget {
  final String assignmentId;

  WatchVideoPage({required this.assignmentId});

  @override
  Widget build(BuildContext context) {
    // Sample data for the assignment
    final Map<String, String> assignmentDetails = {
      'title': 'Assignment 1',
      'dueDate': '2024-10-15',
      'gradeScale': '100 points',
      'studentNumber': '123456789',
      'dateSubmitted': '2024-10-05',
      'file': 'assignment_file.pdf',
    };

    // Create controllers to capture feedback and mark allocation
    final TextEditingController feedbackController = TextEditingController();
    final TextEditingController markController = TextEditingController();

    return Scaffold(
      appBar: AppBar(
        title: Text("Watch Video & Provide Feedback"),
      ),
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: SingleChildScrollView(
          child: Column(
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
                        Navigator.pushNamed(context, '/viewSubmissions');
                      },
                      child: Text("View Submissions", style: TextStyle(color: Colors.white)),
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

              // Video Playback Area
              Container(
                height: 180,
                width: double.infinity,
                color: Colors.black12,
                child: Center(
                  child: Icon(
                    Icons.play_circle_filled,
                    color: Colors.blue,
                    size: 100,
                  ),
                ),
              ),
              SizedBox(height: 20),

              // Assignment Details and Download Button
              Row(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  // Left Column - Assignment Details and Feedback
                  Expanded(
                    flex: 2,
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        _buildDetailRow('Assignment Title:', assignmentDetails['title']!),
                        _buildDetailRow('Due Date:', assignmentDetails['dueDate']!),
                        _buildDetailRow('Grade Scale:', assignmentDetails['gradeScale']!),
                        _buildDetailRow('Student Number:', assignmentDetails['studentNumber']!),
                        _buildDetailRow('Date Submitted:', assignmentDetails['dateSubmitted']!),
                        _buildDetailRow('File:', assignmentDetails['file']!),

                        SizedBox(height: 20),

                        // Feedback Text Area
                        Text("Feedback:", style: TextStyle(fontSize: 16)),
                        SizedBox(height: 10),
                        TextField(
                          controller: feedbackController, // Capture the feedback
                          maxLines: 4,
                          decoration: InputDecoration(
                            border: OutlineInputBorder(),
                            hintText: 'Enter feedback here...',
                          ),
                        ),
                      ],
                    ),
                  ),

                  SizedBox(width: 30),

                  // Right Column - Download Button and Allocate Mark Section
                  Expanded(
                    flex: 1,
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        ElevatedButton.icon(
                          onPressed: () {
                            _downloadVideo(context, assignmentId);
                          },
                          icon: Icon(Icons.download),
                          label: Text("Download File"),
                        ),

                        SizedBox(height: 20),

                        // Allocate Mark Section
                        Text("Allocate Mark:", style: TextStyle(fontSize: 16)),
                        SizedBox(height: 10),
                        TextField(
                          controller: markController, // Capture the allocated mark
                          keyboardType: TextInputType.number,
                          decoration: InputDecoration(
                            border: OutlineInputBorder(),
                            hintText: 'Enter mark...',
                          ),
                        ),

                        SizedBox(height: 30),

                        // Submit Button
                        Center(
                          child: ElevatedButton(
                            onPressed: () {
                              // Navigate to confirm_feedback_page.dart
                              Navigator.pushNamed(context, '/confirmFeedback', arguments: {
                                'assignmentTitle': assignmentDetails['title']!,
                                'studentNumber': assignmentDetails['studentNumber']!,
                                'gradeScale': assignmentDetails['gradeScale']!,
                                'markAllocated': markController.text, // Pass the actual allocated mark
                                'feedback': feedbackController.text, // Pass the actual feedback text
                              });
                            },
                            child: Text("Submit"),
                          ),
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
    );
  }

  // Method to trigger video download by launching the URL
  void _downloadVideo(BuildContext context, String assignmentId) async {
    final String url = 'https://yourbackendurl.com/download/$assignmentId'; // Use your actual download URL

    if (await canLaunch(url)) {
      await launch(url);
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Could not open download link')),
      );
    }
  }

  // Helper function to build detail rows
  Widget _buildDetailRow(String label, String value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8.0),
      child: Row(
        children: [
          Text(
            label,
            style: TextStyle(fontWeight: FontWeight.bold),
          ),
          SizedBox(width: 10),
          Text(value),
        ],
      ),
    );
  }
}
