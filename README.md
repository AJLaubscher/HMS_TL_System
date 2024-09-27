#HMS_TL_System

##Setup 

#Clone the repository 

```bash
git clone https://github.com/AJLaubscher/HMS_TL_System.git 				
```

The project references external libraries in NuGet packages. In the project directory use the following command to look for dependencies and download them if neccessary

#Install Dependencies 

```bash
dotnet restore
```

Currently we do not have a the database on a server on our selected cloud computing service, so one has to update the connection string to point to a specific local database server.
Look in the appsettings.json file

```c#
"ConnectionStrings": {
    "HMS": "Server=MSI\\SQLSERVER;Database=HMS_DB;TrustServerCertificate=True;Trusted_Connection=True;"
  }
```

##Database Migrations 

To Add new columns to our database schema, we need to create migrations. We do this with the following command in the .NET CLI. Migrations are mapped to the Entities folder
e.g Module.cs 

```.NET CLI
dotnet ef migrations add userRole
```

```.NET CLI
dotnet ef database update
```

##Docker Container Builds

The dockerfile build uses the ASP.Net core 8.0 image from the dockerhub image registry as base.
It containerizes the entire project folder, and does not make use of a dockerignore or dockercompose file

```Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["HMS_API/HMS_API.csproj", "HMS_API/"]
RUN dotnet restore "HMS_API/HMS_API.csproj"
COPY . .
WORKDIR "/src/HMS_API"
RUN dotnet build "HMS_API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "HMS_API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HMS_API.dll"]
```


#To Build and run the Docker Image

To build a dockerfile when the current directory is the project folder and to Build a docker image with a tag for image versioning

```bash
dotnet build -t HMS_TL_System:latest. 
```


Publish the container port /map the container port to port 8080
```bash
docker run -p 8080:8080 HMS_TL_System .
```

#How Users can interact with the API

To interact with the API, we make use of the Swagger tool, to make use of our backend system. 
Alternatively one can also Send a request in the HttpRequests folder, e.g AssignmentRequests.http

```c#
//Send Request
GET http://localhost:5157/assignments
```


To make use of the swagger, in the project folder directory build and run the application using :

```.NET CLI
dotnet run 
```

After the application is done building it will show log information 
```.NET CLI
{
  "EventId": 14,
  "LogLevel": "Information",
  "Category": "Microsoft.Hosting.Lifetime",
  "Message": "Now listening on: http://localhost:5157/",
  "State": {
    "Message": "Now listening on: http://localhost:5157/",
    "address": "http://localhost:5157/",
    "{OriginalFormat}": "Now listening on: {address}"
  }
}
```

Click on the link to use the swagger tool and interact with the API.