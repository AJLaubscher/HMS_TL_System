import 'package:flutter/material.dart';

class UserAdminPage extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text("User Administration")),
      body: ListView.builder(
        itemCount: 10, // Replace with actual user count
        itemBuilder: (context, index) {
          return ListTile(
            title: Text("User $index"),
            trailing: IconButton(
              icon: Icon(Icons.delete),
              onPressed: () {
                // Delete user action
              },
            ),
          );
        },
      ),
    );
  }
}
