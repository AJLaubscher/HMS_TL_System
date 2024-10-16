import 'package:flutter/material.dart';

class AdministrationPage extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text("Administration"),
      ),
      body: Column(
        children: [
          // Navigation Bar
          Container(
            padding: EdgeInsets.symmetric(vertical: 10, horizontal: 20),
            color: Colors.black.withOpacity(0.7), // Semi-transparent background
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween, // Align navigation items
              children: [
                Row(
                  children: [
                    // Home Button
                    TextButton(
                      onPressed: () {
                        Navigator.pushNamed(context, '/dashboard'); // Navigate to List Assignments page
                      },
                      child: Text("Home", style: TextStyle(color: Colors.white)),
                    ),
                  ],
                ),
                TextButton(
                  onPressed: () {
                    Navigator.pushNamed(context, '/'); // Navigate to landing page (logout)
                  },
                  child: Text("Logout", style: TextStyle(color: Colors.white)),
                ),
              ],
            ),
          ),
          Expanded(
            child: Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  // Manage User Button
                  SizedBox(
                    width: 250, // Increase button width
                    child: ElevatedButton(
                      style: ElevatedButton.styleFrom(
                        padding: EdgeInsets.symmetric(vertical: 20), // Increase button height
                      ),
                      onPressed: () {
                        Navigator.pushNamed(context, '/manageUser'); // Navigate to Manage User page
                      },
                      child: Text(
                        "Manage Users",
                        style: TextStyle(fontSize: 18), // Increase font size for better visibility
                      ),
                    ),
                  ),
                  SizedBox(height: 40), // Increase spacing between buttons

                  // Manage Modules Button
                  SizedBox(
                    width: 250,
                    child: ElevatedButton(
                      style: ElevatedButton.styleFrom(
                        padding: EdgeInsets.symmetric(vertical: 20),
                      ),
                      onPressed: () {
                        Navigator.pushNamed(context, '/manageModule'); // Navigate to Manage Modules page
                      },
                      child: Text(
                        "Manage Modules",
                        style: TextStyle(fontSize: 18),
                      ),
                    ),
                  ),
                  SizedBox(height: 40), // Increase spacing between buttons

                  // Manage Student Enrollments Button
                  SizedBox(
                    width: 250,
                    child: ElevatedButton(
                      style: ElevatedButton.styleFrom(
                        padding: EdgeInsets.symmetric(vertical: 20),
                      ),
                      onPressed: () {
                        Navigator.pushNamed(context, '/manageEnrollments'); // Navigate to Manage Enrollments page
                      },
                      child: Text(
                        "Manage Student Enrollments",
                        style: TextStyle(fontSize: 18),
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }
}
