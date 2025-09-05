-- CONVERSATIONS (1:1 or group)
CREATE TABLE IF NOT EXISTS Conversations (
    ConversationId SERIAL PRIMARY KEY,
    IsGroup BOOLEAN NOT NULL DEFAULT FALSE,     -- FALSE=1:1, TRUE=Group
    Title VARCHAR(200) NULL,           -- Group name
    CreatedBy INT NULL,                 -- Creator for group
    CreatedAt TIMESTAMP NOT NULL DEFAULT NOW(),
    FOREIGN KEY (CreatedBy) REFERENCES Users(UserId)
);