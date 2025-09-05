-- MESSAGE DELIVERIES (per-recipient read/delivery status)
CREATE TABLE IF NOT EXISTS MessageDeliveries (
    MessageId INT NOT NULL,
    UserId INT NOT NULL,         -- recipient (not sender)
    Status SMALLINT NOT NULL,     -- 0=Sent,1=Delivered,2=Read
    DeliveredAt TIMESTAMP NULL,
    ReadAt TIMESTAMP NULL,
    PRIMARY KEY (MessageId, UserId),
    FOREIGN KEY (MessageId) REFERENCES Messages(MessageId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

-- Index for fast delivery queries
CREATE INDEX IF NOT EXISTS IX_MessageDeliveries_User_Status ON MessageDeliveries(UserId, Status);