# Miniblog

Use Miniblog to allow users with a configurable access level to publish articles or news and commenting in real-time.

_[Work in progress.]_

## Stack:
- C# 9 and .NET 5 (migrated from .NET Core 3.1)
- ASP.NET Core 5 (MVC)
- Entity Framework Core 5
- NUnit, Moq
- UIKit
- SignalR, jQuery (client-side validation only)

## Features
- Real-time commenting with SignalR
- Change the configuration while using website
- Changeable roles' features
- Verify questionable entities (articles, users, comments, etc)

## Access
- By default: username: User/Editor/Administrator, password: 12345678 (for simplicity, usernames correspond to roles)
- You can change defaults before building using the configuration file (users.json), but the values must match the constraints of the User model
