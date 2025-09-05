
# BondTales Chat 📱💬
A real-time chat application that keeps conversations flowing — where every message leaves a little ‘tail’ of connection.

---

## 🚀 Features

### 👤 User Features
- Secure registration & login
- One-on-one and group chats
- Send text, images, voice, and files
- Message read receipts & typing indicators
- Searchable chat history
- Self-destructing media
- Mood-based themes
- Real-time message translation
- Shared live whiteboard
- Story-mode chat playback

### 🛠 Admin Features
- Manage users & groups
- Monitor reported messages
- Content moderation
- View analytics & usage stats
- Broadcast announcements

---

## 📂 Project Structure

```

BondTalesChat/
│
├── client/                  # Angular frontend (User)
│   ├── src/
│   │   ├── app/
│   │   │   ├── components/  # Reusable UI parts
│   │   │   ├── pages/       # Page views
│   │   │   ├── services/    # API & socket services
│   │   │   ├── store/       # State management
│   │   │   └── assets/      # Images, styles
│   │   └── index.html
│
├── admin/                   # Angular admin panel
│   ├── src/
│   │   ├── app/
│   │   │   ├── components/
│   │   │   ├── pages/
│   │   │   ├── services/
│   │   │   └── assets/
│   │   └── index.html
│
├── server/                  # C# .NET backend API
│   ├── Controllers/         # API endpoints
│   ├── Models/              # Entity models
│   ├── Services/            # Business logic
│   ├── Repositories/        # DB access
│   ├── DTOs/                # Data transfer objects
│   ├── Migrations/          # SQL migrations
│   └── Program.cs           # App entry point
│
├── database/                # SQL scripts
│   ├── schema.sql
│   └── seed-data.sql
│
├── firebase/                # Firebase configs
│   ├── firebase-config.json
│
├── docs/                    # Documentation & diagrams
│
├── .env.example             # Environment variables template
├── README.md
└── LICENSE

````

---

## 🛠 Tech Stack

**Frontend:** Angular, TailwindCSS, RxJS, WebSocket  
**Backend:** C# .NET Core, REST API, SignalR  
**Database:** PostgreSQL  
**Realtime Features:** Firebase Realtime Database / Firestore (for presence & notifications)  
**Auth:** JWT Authentication + Firebase Auth  
**Deployment:** Docker, Azure / AWS

---

## 📦 Installation

1. **Clone the repository**
```bash
git clone https://github.com/yourusername/BondTalesChat.git
cd BondTalesChat
````

2. **Setup environment variables**
   Copy `.env.example` to `.env` and update values.

3. **Run backend**

```bash
cd server
dotnet restore
dotnet run
```

4. **Run frontend (User app)**

```bash
cd client
npm install
ng serve
```

5. **Run admin panel**

```bash
cd admin
npm install
ng serve
```

---

## 🔐 Access

* **User App:** `/client`
* **Admin Panel:** `/admin`
* **Backend API:** `/server`

---

## 🗂 Admin Panel Setup

* The Admin Panel is developed as a separate Angular project (`admin`) for better modularity and maintainability.
* Both the main application and the admin panel are hosted separately.
* After successful login, the backend verifies the user role:

  * **Admin role:** Redirects to the Admin Panel URL.
  * **User role:** Redirects to the Main Application URL.
* Redirection is handled via Angular routing or browser navigation.
* JWT tokens are passed between apps for authentication, validated on the backend before granting access.

Example URLs:

* **Admin Panel:** `https://example.com/admin`
* **Main App:** `https://example.com`

---

## 🚢 Deployment

* Use Docker for containerized builds
* Host backend on Azure App Service or AWS EC2
* Deploy Angular apps via Netlify/Vercel/Azure Static Web Apps
* Use PostgreSQL on Azure Database for PostgreSQL or AWS RDS

---
## 🗂 Repository Structure Strategy

For better scalability and team collaboration, **BondTalesChat** is split into multiple repositories:

| Repository Name                  | Description                               |
|-----------------------------------|-------------------------------------------|
| **BondTalesChat-Client**          | Angular user-facing chat application      |
| **BondTalesChat-Admin**           | Angular-based admin panel for management  |
| **BondTalesChat-Server**          | C# .NET backend API with SignalR support  |
| **BondTalesChat-Database**        | SQL scripts, migrations, and seed data    |
| **BondTalesChat-Firebase**        | Firebase configuration for presence & push notifications |
| **BondTalesChat-Docs**            | Documentation, diagrams, and flowcharts   |

### 🔄 How These Repos Work Together
1. **User Login (Client)** → Calls **Server API** → Authenticates & issues JWT → Redirects based on role.
2. **Admin Login (Admin)** → Calls **Server API** → Authenticates & loads admin dashboard.
3. **Server** handles business logic, data access, and real-time events via SignalR.
4. **Database** repo holds all SQL schema and migration scripts, used by the Server repo.
5. **Firebase** repo holds configs for notifications and online/offline tracking.
6. **Docs** repo stores architecture diagrams, API references, and setup guides.


#  Chat System (Server)

This project is a **WhatsApp-like chat backend** built with **C# (.NET)** and **PostgreSQL**, using **SignalR** for real-time messaging.  
It supports **1:1 and group chats**, message delivery/read tracking, and user presence.

---

## **Table of Contents**

1. [Database Schema](#database-schema)  
2. [Table Descriptions](#table-descriptions)  
3. [Message Flow](#message-flow)  
4. [SignalR Integration](#signalr-integration)  
5. [Usage](#usage)  
6. [Diagram](#diagram)  
7. [Notes](#notes)

---

## **Database Schema & Table Creation**

### 1. Users Table
Stores user information.

```sql
CREATE TABLE IF NOT EXISTS Users(
  UserId SERIAL PRIMARY KEY,
  DisplayName VARCHAR(200) NOT NULL,
  ProfilePicture VARCHAR(500) NULL,
  CreatedAt TIMESTAMP NOT NULL DEFAULT NOW()
);
````

### 2. Conversations Table

Stores both 1:1 and group chats.

```sql
CREATE TABLE IF NOT EXISTS Conversations(
  ConversationId SERIAL PRIMARY KEY,
  IsGroup BOOLEAN NOT NULL DEFAULT FALSE, -- FALSE=1:1, TRUE=Group
  Title VARCHAR(200) NULL,       -- Group title
  CreatedBy INT NULL,             -- Creator for group
  CreatedAt TIMESTAMP NOT NULL DEFAULT NOW(),
  FOREIGN KEY (CreatedBy) REFERENCES Users(UserId)
);
```

### 3. ConversationMembers Table

Maps users to conversations.

```sql
CREATE TABLE IF NOT EXISTS ConversationMembers(
  ConversationId INT NOT NULL,
  UserId INT NOT NULL,
  JoinedAt TIMESTAMP NOT NULL DEFAULT NOW(),
  PRIMARY KEY (ConversationId, UserId),
  FOREIGN KEY (ConversationId) REFERENCES Conversations(ConversationId),
  FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

CREATE INDEX IF NOT EXISTS IX_ConversationMembers_User ON ConversationMembers(UserId);
```

### 4. Messages Table

Single source for all messages.

```sql
CREATE TABLE IF NOT EXISTS Messages(
  MessageId SERIAL PRIMARY KEY,
  ConversationId INT NOT NULL,
  SenderId INT NOT NULL,
  MessageText TEXT NULL,
  MediaUrl VARCHAR(500) NULL,
  MessageType SMALLINT NOT NULL DEFAULT 0, -- 0=Text,1=Image,2=Video,3=Doc,4=Audio
  SentAt TIMESTAMP NOT NULL DEFAULT NOW(),
  Edited BOOLEAN NOT NULL DEFAULT FALSE,
  Deleted BOOLEAN NOT NULL DEFAULT FALSE,
  FOREIGN KEY (ConversationId) REFERENCES Conversations(ConversationId),
  FOREIGN KEY (SenderId) REFERENCES Users(UserId)
);

CREATE INDEX IF NOT EXISTS IX_Messages_Conv_SentAt ON Messages(ConversationId, SentAt DESC, MessageId DESC);
CREATE INDEX IF NOT EXISTS IX_Messages_Sender ON Messages(SenderId);
```

### 5. MessageDeliveries Table

Per-recipient delivery/read status.

```sql
CREATE TABLE IF NOT EXISTS MessageDeliveries(
  MessageId INT NOT NULL,
  UserId INT NOT NULL,
  Status SMALLINT NOT NULL, -- 0=Sent,1=Delivered,2=Read
  DeliveredAt TIMESTAMP NULL,
  ReadAt TIMESTAMP NULL,
  PRIMARY KEY (MessageId, UserId),
  FOREIGN KEY (MessageId) REFERENCES Messages(MessageId),
  FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

CREATE INDEX IF NOT EXISTS IX_MessageDeliveries_User_Status ON MessageDeliveries(UserId, Status);
```

### 6. UserConnections Table

Tracks online/offline presence.

```sql
CREATE TABLE IF NOT EXISTS UserConnections(
  ConnectionId VARCHAR(200) PRIMARY KEY,
  UserId INT NOT NULL,
  ConnectedAt TIMESTAMP NOT NULL DEFAULT NOW(),
  FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
```

---

## **Message Flow**

1. **1:1 Chat**

   * Check if a 1:1 conversation exists for the two users.
   * If not, create conversation and add both users to `ConversationMembers`.
   * Insert message in `Messages` with `ConversationId`.
   * Insert delivery record in `MessageDeliveries` for recipient.

2. **Group Chat**

   * Messages linked to group conversation.
   * Delivery/read tracking stored per member in `MessageDeliveries`.

3. **UI Display**

   * `SenderId` → message belongs to this user (right side in UI).
   * `ConversationMembers - SenderId` → recipient(s) (left side in UI).

---

## **SignalR Integration**

* `UserConnections` tracks multiple connections per user.
* When a message is sent:

  * Server broadcasts to all connections of members in `ConversationMembers` except sender (optional).
* Presence updates (`online/offline`) are updated in `UserConnections`.

---

## **Usage**

1. Setup PostgreSQL database with above tables.
2. Configure **AppDbContext** in .NET project.
3. Implement **ChatHub** (SignalR) for real-time messaging.
4. Implement **MessageService** for:

   * Conversation management
   * Message insertion
   * Delivery tracking
5. Client fetches messages by `ConversationId` and renders sender/receiver dynamically.

---

## **Diagram**

```
+----------------+
|     Users      |
+----------------+
| UserId PK      |
| DisplayName    |
| ProfilePicture |
+----------------+
        |
        | 1..*  Membership
        v
+----------------+
| ConversationMembers |
+----------------+
| ConversationId PK,FK |
| UserId PK,FK         |
| JoinedAt             |
+----------------+
        ^
        | belongs to
        |
+----------------+
| Conversations  |
+----------------+
| ConversationId PK |
| IsGroup          |
| Title            |
| CreatedBy FK     |
| CreatedAt        |
+----------------+
        |
        | 1..* Messages
        v
+----------------+
|   Messages     |
+----------------+
| MessageId PK   |
| ConversationId FK |
| SenderId FK    |
| MessageText    |
| MediaUrl       |
| MessageType    |
| SentAt         |
| Edited         |
| Deleted        |
+----------------+
        |
        | 1..* Delivery/Read per recipient
        v
+----------------+
| MessageDeliveries |
+----------------+
| MessageId PK,FK   |
| UserId PK,FK      |
| Status            |
| DeliveredAt       |
| ReadAt            |
+----------------+
        ^
        |
        | Tracks online connections
        v
+----------------+
| UserConnections |
+----------------+
| ConnectionId PK |
| UserId FK       |
| ConnectedAt     |
+----------------+
```

---

## **Notes**

* 1:1 conversations → `IsGroup = 0`, exactly 2 members.
* Group conversations → `IsGroup = 1`, multiple members.
* Delivery status → 0=Sent, 1=Delivered, 2=Read.
* Messages are **single source** for all conversations.
* Receiver(s) are **derived from `ConversationMembers`** excluding sender.
* Scalable for real-time apps like WhatsApp.

---
## 📜 License

MIT License — Free to use & modify.

