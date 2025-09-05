CREATE TABLE IF NOT EXISTS GroupMembers (
    GroupId INT NOT NULL,
    UserId INT NOT NULL,
    JoinedAt TIMESTAMP NOT NULL DEFAULT NOW(),
    PRIMARY KEY (GroupId, UserId),
    FOREIGN KEY (GroupId) REFERENCES Groups(GroupId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

-- Index for faster lookup
CREATE INDEX IF NOT EXISTS IX_GroupMembers_User ON GroupMembers(UserId);
