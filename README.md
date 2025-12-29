# Homes_API

Homes_API is the backend service for a real estate tracking application, built using .NET 8.0 and connected to a MySQL database. It serves as the API layer for an Angular frontend, providing essential functionality for users to manage and track real estate properties, user actions, and communication history with sellers and buyers.

The application is designed to reflect production-oriented backend practices, with a focus on security, scalability, and clean separation of concerns.

## Features

- RESTful API built with .NET 8.0
- MySQL database integration
- JWT-based authentication with refresh tokens
- Role-based authorization
- Rate limiting on sensitive endpoints
- Google reCAPTCHA verification
- Designed to run behind an Nginx reverse proxy
- Supports security monitoring tools such as Fail2Ban
  
## Prerequisites

- .NET SDK 8.0 or higher
- MySQL 8.0  or higher Server
- Angular 18.0 or higher frontend (for full functionality)

