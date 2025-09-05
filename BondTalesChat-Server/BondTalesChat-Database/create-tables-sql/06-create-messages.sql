-- MESSAGES
CREATE TABLE IF NOT EXISTS Messages (
    MessageId SERIAL PRIMARY KEY,
    ConversationId INT NOT NULL,
    SenderId INT NOT NULL,
    MessageText TEXT NULL,
    MediaUrl VARCHAR(500) NULL,
    MessageType SMALLINT NOT NULL DEFAULT 0,   -- 0=Text,1=Image,2=Video,3=Doc,4=Audio
    SentAt TIMESTAMP NOT NULL,
    Edited BOOLEAN NOT NULL DEFAULT FALSE,
    Deleted BOOLEAN NOT NULL DEFAULT FALSE,
    FOREIGN KEY (ConversationId) REFERENCES Conversations(ConversationId),
    FOREIGN KEY (SenderId) REFERENCES Users(UserId)
);

-- Indexes for fast retrieval
CREATE INDEX IF NOT EXISTS IX_Messages_Conv_SentAt ON Messages(ConversationId, SentAt DESC, MessageId DESC);
CREATE INDEX IF NOT EXISTS IX_Messages_Sender ON Messages(SenderId);