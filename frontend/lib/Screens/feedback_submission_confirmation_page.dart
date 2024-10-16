import 'package:flutter/material.dart';

class ConfirmFeedbackPage extends StatelessWidget {
  final String assignmentTitle;
  final String studentNumber;
  final String gradeScale;
  final String markAllocated;
  final String feedback;

  ConfirmFeedbackPage({
    required this.assignmentTitle,
    required this.studentNumber,
    required this.gradeScale,
    required this.markAllocated,
    required this.feedback,
  });

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text("Confirm Feedback"),
        actions: [
          TextButton(
            onPressed: () {
              Navigator.pushNamed(context, '/'); // Redirect to landing page (logout)
            },
            child: Text("Logout", style: TextStyle(color: Colors.white)),
          ),
        ],
      ),
      body: Padding(
        padding: EdgeInsets.all(16.0),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.start,
          crossAxisAlignment: CrossAxisAlignment.start, // Align the content to the left
          children: [
            // Assignment Details
            _buildDetailRow("Assignment Title:", assignmentTitle),
            _buildDetailRow("Student Number:", studentNumber),
            _buildDetailRow("Grade Scale:", gradeScale),
            _buildDetailRow("Mark Allocated:", markAllocated),
            SizedBox(height: 20),

            // Feedback Section
            Text("Feedback:", style: TextStyle(fontSize: 18)),
            SizedBox(height: 10),
            Container(
              padding: EdgeInsets.all(10),
              decoration: BoxDecoration(
                border: Border.all(color: Colors.grey),
                borderRadius: BorderRadius.circular(5),
              ),
              child: Text(feedback, style: TextStyle(fontSize: 16)),
            ),
            SizedBox(height: 30),

            // Buttons for Confirm and Cancel
            Row(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                ElevatedButton(
                  onPressed: () {
                    // Implement your feedback submission logic here
                    Navigator.pushNamed(context, '/viewSubmissions'); // Redirect to view submissions
                  },
                  child: Text("Confirm"),
                ),
                SizedBox(width: 20),
                ElevatedButton(
                  onPressed: () {
                    // Go back to the Watch Video page
                    Navigator.pop(context, '/watchVideo'); // This will go back to the previous page in the stack
                  },
                  style: ElevatedButton.styleFrom(
                    backgroundColor: Colors.grey, // Grey for cancel button
                  ),
                  child: Text("Cancel"),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  // Helper function to build detail rows
  Widget _buildDetailRow(String label, String value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8.0),
      child: Row(
        children: [
          Expanded(
            flex: 2,
            child: Text(
              label,
              style: TextStyle(fontWeight: FontWeight.bold, fontSize: 18),
            ),
          ),
          Expanded(
            flex: 3,
            child: Text(
              value,
              style: TextStyle(fontSize: 18),
            ),
          ),
        ],
      ),
    );
  }
}
