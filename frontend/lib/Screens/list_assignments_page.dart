// import 'package:flutter/material.dart';
// import 'package:http/http.dart' as http;
// import 'dart:convert';

// class ListAssignmentsPage extends StatefulWidget {
//   final int userId; // Receive the userId dynamically

//   ListAssignmentsPage({required this.userId}); // Constructor to accept userId

//   @override
//   _ListAssignmentsPageState createState() => _ListAssignmentsPageState();
// }

// class _ListAssignmentsPageState extends State<ListAssignmentsPage> {
//   List<dynamic> _assignments = [];
//   bool _isLoading = true;

//   @override
//   void initState() {
//     super.initState();
//     _fetchAssignments(); // Fetch assignments on page load
//   }

//   Future<void> _fetchAssignments() async {
//     setState(() {
//       _isLoading = true; // Show loading spinner
//     });

//     try {
//       final response = await http.get(
//         Uri.parse('http://localhost:5004/assignments'), // Update with your API URL
//       );

//       if (response.statusCode == 200) {
//         setState(() {
//           _assignments = json.decode(response.body);
//           _isLoading = false; // Turn off loading spinner
//         });
//       } else {
//         // Handle response errors
//         setState(() {
//           _isLoading = false;
//         });
//         _showErrorDialog("Failed to load assignments: ${response.statusCode}");
//       }
//     } catch (error) {
//       // Handle network errors
//       print("Error: $error");
//       _showErrorDialog("Error fetching assignments: $error");
//       setState(() {
//         _isLoading = false;
//       });
//     }
//   }

//   void _showErrorDialog(String message) {
//     showDialog(
//       context: context,
//       builder: (context) => AlertDialog(
//         title: Text("Error"),
//         content: Text(message),
//         actions: [
//           TextButton(
//             onPressed: () => Navigator.of(context).pop(),
//             child: Text("OK"),
//           ),
//         ],
//       ),
//     );
//   }

//   @override
//   Widget build(BuildContext context) {
//     return Scaffold(
//       appBar: AppBar(
//         title: Text("List Assignments"),
//       ),
//       body: Column(
//         children: [
//           // Navigation bar setup
//           Container(
//             padding: EdgeInsets.symmetric(vertical: 10, horizontal: 20),
//             color: Colors.black.withOpacity(0.7),
//             child: Row(
//               mainAxisAlignment: MainAxisAlignment.spaceBetween,
//               children: [
//                 Row(
//                   children: [
//                     TextButton(
//                       onPressed: () {
//                         Navigator.pushNamed(context, '/createAssignment');
//                       },
//                       child: Text("Create Assignment", style: TextStyle(color: Colors.white)),
//                     ),
//                     TextButton(
//                       onPressed: () {
//                         Navigator.pushNamed(context, '/viewSubmissions');
//                       },
//                       child: Text("View Submissions", style: TextStyle(color: Colors.white)),
//                     ),
//                     TextButton(
//                       onPressed: () {
//                         Navigator.pushNamed(context, '/changePassword');
//                       },
//                       child: Text("Change Password", style: TextStyle(color: Colors.white)),
//                     ),
//                     TextButton(
//                       onPressed: () {
//                         Navigator.pushNamed(context, '/administration');
//                       },
//                       child: Text("Admin", style: TextStyle(color: Colors.white)),
//                     ),
//                   ],
//                 ),
//                 TextButton(
//                   onPressed: () {
//                     Navigator.pushNamed(context, '/');
//                   },
//                   child: Text("Logout", style: TextStyle(color: Colors.white)),
//                 ),
//               ],
//             ),
//           ),
//           // Check if loading or show DataTable
//           Expanded(
//             child: _isLoading
//                 ? Center(child: CircularProgressIndicator()) // Show spinner while loading
//                 : SingleChildScrollView(
//                     child: DataTable(
//                       columns: const [
//                         DataColumn(label: Text("Assignment Title")),
//                         DataColumn(label: Text("Module")),
//                         DataColumn(label: Text("Open Date")),
//                         DataColumn(label: Text("Due Date")),
//                         DataColumn(label: Text("Actions")), // Add an actions column
//                       ],
//                       rows: _assignments.map(
//                         (assignment) {
//                           return DataRow(
//                             cells: [
//                               DataCell(Text(assignment['title'] ?? 'N/A')), // Use fallback if null
//                               DataCell(Text(assignment['module'] ?? 'N/A')),
//                               DataCell(Text(assignment['openDate'] ?? 'N/A')),
//                               DataCell(Text(assignment['dueDate'] ?? 'N/A')),
//                               DataCell(
//                                 Row(
//                                   children: [
//                                     IconButton(
//                                       icon: Icon(Icons.videocam),
//                                       onPressed: () {
//                                         // Navigate to ViewAssignmentVideosPage
//                                         Navigator.pushNamed(
//                                           context,
//                                           '/listAssignmentVideos',
//                                           arguments: {
//                                             'assignmentId': assignment['id'], // Pass assignmentId
//                                             'userId': widget.userId, // Pass userId if needed
//                                           },
//                                         );
//                                       },
//                                     ),
//                                     IconButton(
//                                       icon: Icon(Icons.edit),
//                                       onPressed: () {
//                                         // Navigate to UpdateUserPage
//                                         Navigator.pushNamed(
//                                           context,
//                                           '/updateUser',
//                                           arguments: {
//                                             'userId': widget.userId.toString(), // Pass userId
//                                           },
//                                         );
//                                       },
//                                     ),
//                                   ],
//                                 ),
//                               ),
//                             ],
//                           );
//                         },
//                       ).toList(),
//                     ),
//                   ),
//           ),
//         ],
//       ),
//     );
//   }
// }


import 'package:flutter/material.dart';
import 'package:hms_web_app/globals.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';

class ListAssignmentsPage extends StatefulWidget {
  final int userId; // Receive the userId dynamically

  ListAssignmentsPage({required this.userId}); // Constructor to accept userId

  @override
  _ListAssignmentsPageState createState() => _ListAssignmentsPageState();
}

class _ListAssignmentsPageState extends State<ListAssignmentsPage> {
  List<dynamic> _assignments = []; // Initialize assignments
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _fetchAssignments(); // Fetch assignments on page load
  }

  Future<void> _fetchAssignments() async {
    setState(() {
      _isLoading = true; // Show loading spinner
    });

    try {
      final response = await http.get(
        getApiPath('assignments'),
        headers: getHeaders(),
      );

      print('Response status: ${response.statusCode}'); // Log response status
      print('Response body: ${response.body}'); // Log response body

      if (response.statusCode == 200) {
        // Try to decode the response
        try {
          _assignments = json.decode(response.body) as List<dynamic>;
        } catch (e) {
          _showErrorDialog("Error parsing assignments data: $e");
          return; // Early return on error
        }

        setState(() {
          _isLoading = false; // Turn off loading spinner
        });
      } else {
        // Handle response errors
        setState(() {
          _isLoading = false;
        });
        _showErrorDialog("Failed to load assignments: ${response.statusCode}");
      }
    } catch (error) {
      // Handle network errors
      print("Error: $error");
      _showErrorDialog("Error fetching assignments: $error");
      setState(() {
        _isLoading = false;
      });
    }
  }

  void _showErrorDialog(String message) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: Text("Error"),
        content: Text(message),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(),
            child: Text("OK"),
          ),
        ],
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text("List Assignments"),
      ),
      body: Column(
        children: [
          // Navigation bar setup
          Container(
            padding: EdgeInsets.symmetric(vertical: 10, horizontal: 20),
            color: Colors.black.withOpacity(0.7),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Row(
                  children: [
                    TextButton(
                      onPressed: () {
                        Navigator.pushNamed(context, '/createAssignment');
                      },
                      child: Text("Create Assignment", style: TextStyle(color: Colors.white)),
                    ),
                    TextButton(
                      onPressed: () {
                        Navigator.pushNamed(context, '/viewSubmissions');
                      },
                      child: Text("View Submissions", style: TextStyle(color: Colors.white)),
                    ),
                    TextButton(
                      onPressed: () {
                        Navigator.pushNamed(context, '/changePassword');
                      },
                      child: Text("Change Password", style: TextStyle(color: Colors.white)),
                    ),
                    TextButton(
                      onPressed: () {
                        Navigator.pushNamed(context, '/administration');
                      },
                      child: Text("Admin", style: TextStyle(color: Colors.white)),
                    ),
                  ],
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
          // Check if loading or show DataTable
          Expanded(
            child: _isLoading
                ? Center(child: CircularProgressIndicator()) // Show spinner while loading
                : SingleChildScrollView(
                    child: DataTable(
                      columns: const [
                        DataColumn(label: Text("Assignment Title")),
                        DataColumn(label: Text("Module")),
                        DataColumn(label: Text("Open Date")),
                        DataColumn(label: Text("Due Date")),
                        DataColumn(label: Text("Actions")), // Add an actions column
                      ],
                      rows: _assignments.map(
                        (assignment) {
                          return DataRow(
                            cells: [
                              DataCell(Text(assignment['title']?.toString() ?? 'N/A')), // Use fallback if null
                              DataCell(Text(assignment['module']?.toString() ?? 'N/A')),
                              DataCell(Text(assignment['openDate']?.toString() ?? 'N/A')),
                              DataCell(Text(assignment['dueDate']?.toString() ?? 'N/A')),
                              DataCell(
                                Row(
                                  children: [
                                    IconButton(
                                      icon: Icon(Icons.videocam),
                                      onPressed: () {
                                        // Navigate to ViewAssignmentVideosPage
                                        Navigator.pushNamed(
                                          context,
                                          '/listAssignmentVideos',
                                          arguments: {
                                            'assignmentId': assignment['id'], // Pass assignmentId
                                            'userId': widget.userId, // Pass userId if needed
                                          },
                                        );
                                      },
                                    ),
                                    IconButton(
                                      icon: Icon(Icons.edit),
                                      onPressed: () {
                                        // Navigate to UpdateUserPage
                                        Navigator.pushNamed(
                                          context,
                                          '/updateUser',
                                          arguments: {
                                            'userId': widget.userId.toString(), // Pass userId
                                          },
                                        );
                                      },
                                    ),
                                  ],
                                ),
                              ),
                            ],
                          );
                        },
                      ).toList(),
                    ),
                  ),
          ),
        ],
      ),
    );
  }
}
