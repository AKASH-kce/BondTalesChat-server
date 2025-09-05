-- CONVERSATION MEMBERS
CREATE TABLE IF NOT EXISTS ConversationMembers (
    ConversationId INT NOT NULL,
    UserId INT NOT NULL,
    JoinedAt TIMESTAMP NOT NULL DEFAULT NOW(),
    PRIMARY KEY (ConversationId, UserId),
    FOREIGN KEY (ConversationId) REFERENCES Conversations(ConversationId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
-- Index for faster lookup of user's conversations
CREATE INDEX IF NOT EXISTS IX_ConversationMembers_User ON ConversationMembers(UserId);