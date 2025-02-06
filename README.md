# Task Assessment

## Task Overview

This project implements a microservice-based system with the following functionalities:

1. A **file reader microservice** that reads a file line by line and processes new data.
2. A **message queue (RabbitMQ)** to asynchronously handle requests efficiently.
3. A **consumer microservice** that retrieves messages from the queue and stores data in a **PostgreSQL database**.
4. A **REST API** to fetch stored data, supporting **search, filtering, and pagination**.
5. The consumer is rate-limited to process **30 requests per second**.

---

## System Architecture

The system consists of three main components:

1. **File Reader Service (Producer)**

   - Reads a file line by line.
   - Publishes each line as a message to a RabbitMQ queue.

2. **File Consumer Service**

   - Consumes messages from RabbitMQ.
   - Stores the extracted data in a PostgreSQL database.
   - Implements rate limiting to process at most **30 requests per second**.

3. **REST API Service**

   - Provides an endpoint to fetch data from the database.
   - Supports search, filtering, and pagination.

---

## Installation and Setup

### Prerequisites

Ensure you have the following installed:

- **.NET Core** (for API and microservices)
- **PostgreS****QL** (for data storage)
- **RabbitMQ** (for message queuing)

### Clone Repository

```sh
 git clone https://github.com/AlishaAafrin/FileReaderSolution/

```

### Configuration

Update `appsettings.json` in both Producer and Consumer services with:

- **RabbitMQ connection details**
- **PostgreSQL database connection string**

### Running Services

#### Start RabbitMQ (Docker)

#### Start PostgreSQL (Docker)


#### Start Producer Service

```sh
cd FileReaderSolution
 dotnet run
```

#### Start Consumer Service

```sh
cd FileConsumerSolution
 dotnet run
```
Note: Make sure this path matches \FileReaderSolution\FileReaderSolution> cd FileConsumerSolution

#### Start API Service

```sh
cd FileReaderWebApi
 dotnet run
```

---

## API Endpoints

### Fetch Data (With Search, Filter, Pagination)

```http
GET <HOST>:<PORT>/api/data?pageno=1&pagesize=10&name=xyz
local sample tested url: http://localhost:5108/api/FileData?pageno=1&pagesize=10&name=Test

```
