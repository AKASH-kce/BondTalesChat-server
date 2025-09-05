-- USER CONNECTIONS (for SignalR connection tracking)
CREATE TABLE IF NOT EXISTS UserConnections (
    ConnectionId VARCHAR(255) PRIMARY KEY,
    UserId INT NOT NULL,
    ConnectedAt TIMESTAMP NOT NULL DEFAULT NOW(),
    LastSeen TIMESTAMP NOT NULL DEFAULT NOW(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

-- Index for fast user lookup
CREATE INDEX IF NOT EXISTS IX_UserConnections_User ON UserConnections(UserId);
