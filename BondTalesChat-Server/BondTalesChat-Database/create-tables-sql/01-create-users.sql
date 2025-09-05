-- USERS table
CREATE TABLE IF NOT EXISTS Users (
    UserId SERIAL PRIMARY KEY,
    username VARCHAR(200) NOT NULL,
    email VARCHAR(200) NOT NULL,
    userpassword VARCHAR(200) NOT NULL,
    ProfilePicture VARCHAR(500) NULL,
    CreatedAt TIMESTAMP NOT NULL DEFAULT NOW(),
    phoneNumber VARCHAR(10) NOT NULL
        CHECK (LENGTH(phoneNumber) = 10)
);
