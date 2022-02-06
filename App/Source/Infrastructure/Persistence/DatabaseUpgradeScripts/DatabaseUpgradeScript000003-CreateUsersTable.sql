CREATE TABLE users(
    id UUID PRIMARY KEY NOT NULL DEFAULT uuid_generate_v1(),
    email VARCHAR(128) NOT NULL UNIQUE,
    data JSONB 
);