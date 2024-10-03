# Order Management Web Application

This is a web application built with **ASP.NET Core (MVC)** and **.NET 7**, designed for managing orders with real-time status updates. The application handles order processing, priority-based execution, and real-time notifications using **SignalR**. Background tasks are managed using **Hangfire**, and order messages are integrated with **RabbitMQ** for messaging queues.

## Features

- **CRUD Operations**: Manage orders (Create, Read, Update, Delete) via a RESTful API.
- **Real-Time Status Updates**: Order status updates are shown in real time using **SignalR**.
- **Background Processing**: Orders with **Pending** status are processed every 5 minutes using **Hangfire**.
- **Priority-Based Processing**: Orders are processed based on their calculated priority.
- **Currency Conversion**: Uses an external API to convert order amounts into a base currency (e.g., USD).
- **Messaging with RabbitMQ**: Processed orders are sent to **RabbitMQ** for further processing or integration.
- **Validation**: Ensures that order data is valid using **FluentValidation** (e.g., `TotalAmount` > 0, valid `Currency`, etc.).

## Technologies Used

- **ASP.NET Core (MVC)**: The web framework used for building the web API and MVC views.
- **Entity Framework Core**: For database access and handling data persistence.
- **Hangfire**: Manages background tasks (processes orders in the background every 5 minutes).
- **SignalR**: Provides real-time updates for the order status on the front end.
- **RabbitMQ**: Used to send order data to message queues for further processing.
- **FluentValidation**: For ensuring that incoming order data is validated.

## Requirements

- **.NET 7 SDK**
- **SQL Server** or **PostgreSQL** (depending on your configuration)
- **RabbitMQ** (for message queuing)
- **Hangfire** (for background job scheduling)
- **SignalR** (for real-time updates)

## Setup Instructions

### 1. Clone the Repository
```bash
git clone https://github.com/deliver21/OrderManagementApp.git
cd OrderManagementApp

### 2.Configure your database connection in the appsettings.json 

From OrderManagement.OrderAPI

"ConnectionStrings": {
  "DefaultConnection": "Server=your_server; Database=OrderManagementDb; Trusted_Connection=True; TrustServerCertificate=True",
}
Run the database Migration
dotnet ef database update

From OrderManagement.Web
 "ServiceUrls": {
    "OrderAPI": "https://localhost:7001"
  }

### 3.Rabbit Configuration
docker run -d --name rabbitmq -p 15672:15672 -p 5672:5672 rabbitmq:management
http://localhost:15672
Default credentials:

Username: guest
Password: guest

**OrderManagement.Web should run on https://localhost:7002
***OrderManagement.OrderAPI should run on https://localhost:7001
