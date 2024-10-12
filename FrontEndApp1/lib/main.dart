import 'package:flutter/material.dart';

// Import all necessary pages, including the new ones
import 'Screens/create_assignments_page.dart'; // Corrected import for CreateAssignmentsPage
import 'Screens/feedback_submission_confirmation_page.dart';
import 'Screens/landing_page.dart';
import 'Screens/list_assignments_page.dart';
import 'Screens/login_page.dart';
import 'Screens/user_admin_page.dart';
import 'Screens/view_assignments_page.dart';
import 'Screens/view_submissions_page.dart';
import 'Screens/watch_video_provide_feedback_page.dart';
import 'Screens/change_password_page.dart';
import 'Screens/administration_page.dart'; // Import Administration Page
import 'Screens/manage_user_page.dart'; // Import Manage User Page
import 'Screens/manage_module_page.dart'; // Import Manage Module Page
import 'Screens/manage_enrollments_page.dart'; // Import Manage Enrollments Page
import 'Screens/update_enrollment_page.dart';
import 'Screens/enroll_student_page.dart';
import 'Screens/create_student_module_enrollments_page.dart';
import 'Screens/update_student_module_enrollments_page.dart';
import 'Screens/create_module_page.dart';
import 'Screens/update_module_page.dart';
import 'Screens/create_user_page.dart';
import 'Screens/update_user_page.dart';
import 'Screens/find_lecturer_page.dart';

void main() {
  runApp(MyApp());
}

class MyApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Web App',
      initialRoute: '/',
      routes: {
        '/': (context) => LandingPage(),
        '/login': (context) => LoginPage(),
        '/dashboard': (context) {
          final args = ModalRoute.of(context)?.settings.arguments as Map<String, dynamic>;
          return ListAssignmentsPage(
            userId: args['userId'], // Pass userId to ListAssignmentsPage
          );
        },
        '/userAdmin': (context) => UserAdminPage(),
        '/createAssignment': (context) => CreateAssignmentsPage(),
        '/viewSubmissions': (context) => ViewSubmissionsPage(),
        '/watchVideo': (context) {
          final args = ModalRoute.of(context)?.settings.arguments as Map<String, dynamic>;
          return WatchVideoPage(
            assignmentId: args['assignmentId'],
          );
        },
        '/confirmFeedback': (context) {
          final args = ModalRoute.of(context)?.settings.arguments as Map<String, String>;
          return ConfirmFeedbackPage(
            assignmentTitle: args['assignmentTitle']!,
            studentNumber: args['studentNumber']!,
            gradeScale: args['gradeScale']!,
            markAllocated: args['markAllocated']!,
            feedback: args['feedback']!,
          );
        },
        '/changePassword': (context) {
          final args = ModalRoute.of(context)?.settings.arguments as Map<String, String>;
          return ChangePasswordPage(
            username: args['username']!,
            role: args['role']!,
          );
        },
        '/administration': (context) => AdministrationPage(),
        '/manageUser': (context) => ManageUserPage(),
        '/manageModule': (context) => ManageModulePage(),
        '/manageEnrollments': (context) => ManageEnrollmentsPage(),
        '/updateEnrollments': (context) => UpdateEnrollmentPage(),
        '/enrollStudent': (context) => EnrollStudentPage(),
        '/createStudentModuleEnrollments': (context) => CreateStudentModuleEnrollmentsPage(),
        '/updateStudentModuleEnrollments': (context) => UpdateStudentModuleEnrollmentsPage(),
        '/createModule': (context) => CreateModulePage(),
        '/updateModule': (context) {
          final args = ModalRoute.of(context)?.settings.arguments as Map<String, dynamic>;
          return UpdateModulePage(
            moduleId: args['moduleId'],
            moduleName: args['moduleName'],
            moduleCode: args['moduleCode'],
            lecturerId: args['lecturerId'],
          );
        },
        '/createUser': (context) => CreateUserPage(),
        '/updateUser': (context) {
          final args = ModalRoute.of(context)?.settings.arguments as Map<String, String>;
          return UpdateUserPage(
            userId: int.parse(args['userId']!), // Convert String to int
          );
        },
        '/findLecturer': (context) => FindLecturerPage(),
      },
    );
  }
}
