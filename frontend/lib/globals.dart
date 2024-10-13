import 'package:flutter/rendering.dart';
import 'package:shared_preferences/shared_preferences.dart';

const String basePath = 'http://localhost:5157';
String bearer = '';

Uri getApiPath(String path) {
  return Uri.parse('${basePath}/${path}');
}


Map<String, String> getHeaders() {
  return {
    'Content-Type': 'application/json; charset=UTF-8',
    'Authorization': 'Bearer ${bearer}',
  };
}

Future<void> initBearer() async {
  var prefs = await SharedPreferences.getInstance();
  bearer = await prefs.getString('token') ?? '';
}

