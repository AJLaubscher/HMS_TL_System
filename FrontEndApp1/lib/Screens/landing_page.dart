import 'package:flutter/material.dart';

class LandingPage extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Stack(
        children: [
          // Background Image
          Container(
            decoration: BoxDecoration(
              image: DecorationImage(
                image: AssetImage("assets/NWU_Logo.png"), // Add your background image path here
                fit: BoxFit.cover,
              ),
            ),
          ),
          // Navigation Bar and Page Content
          Column(
            children: [
              // Navigation Bar at the top
              Container(
                padding: EdgeInsets.symmetric(vertical: 20, horizontal: 40),
                color: Colors.black.withOpacity(0.7), // Semi-transparent background
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.end, // Align navigation items to the right
                  children: [
                    // Remaining Navigation Links
                    TextButton(
                      onPressed: () {
                        Navigator.pushNamed(context, '/login');
                      },
                      child: Text("Login", style: TextStyle(color: Colors.white)),
                    ),
                  ],
                ),
              ),
              // Welcome Text moved to below the navigation bar
              Container(
                padding: EdgeInsets.symmetric(vertical: 20),
                child: Center(
                  child: Text(
                    "Welcome to the HMS T&L System",
                    style: TextStyle(
                      fontSize: 40,
                      color: Colors.white,
                      fontWeight: FontWeight.bold,
                      shadows: [
                        Shadow(
                          blurRadius: 10.0,
                          color: Colors.black,
                          offset: Offset(5.0, 5.0),
                        ),
                      ],
                    ),
                  ),
                ),
              ),
              // Body content (empty for now, adjust as needed)
              Expanded(
                child: Center(
                  child: Container(), // Empty space for future content
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }
}
