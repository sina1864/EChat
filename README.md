# E-Chat
A real-time chat application using ASP.NET Core 7, SignalR and KnockoutJS.

![](https://raw.githubusercontent.com/AKouki/SignalR-Chat/main/src/Chat.Web/wwwroot/images/screenshots/1.png)
![](https://raw.githubusercontent.com/AKouki/SignalR-Chat/main/src/Chat.Web/wwwroot/images/screenshots/2.png)

## Features
* Group Chat
* Private Chat `/private(Name) Hello, how are you?`
* Photo Message
* Basic Emojis
* Chat Rooms
* User Profile
* Two-Factor Authentication (2FA)
* Docker Enabled

## Packages
* AutoMapper `v12.0.1`
* AutoMapper.Extensions.Microsoft.DependencyInjection `v12.0.1`
* Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore `v7.0.5`
* Microsoft.AspNetCore.Identity.EntityFrameworkCore `v7.0.5`
* Microsoft.AspNetCore.Identity.UI `v7.0.5`
* Microsoft.EntityFrameworkCore `v7.0.5`
* Microsoft.EntityFrameworkCore.SqlServer `v7.0.5`
* Microsoft.EntityFrameworkCore.Tools `v7.0.5`
* Microsoft.VisualStudio.Azure.Containers.Tools.Targets `v7.0.5`
* Microsoft.VisualStudio.Web.CodeGeneration.Design `v7.0.6`

## Getting Started
To run the application:

1. Clone the Project
2. Open Visual Studio and load the Solution
3. Resolve any missing/required nuget package
4. Build Database. Open `Package Manager Console` and run the following commands: `update-database`
5. That's all... Run the Project!