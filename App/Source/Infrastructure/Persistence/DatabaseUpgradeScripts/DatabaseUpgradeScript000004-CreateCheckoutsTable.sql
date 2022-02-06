CREATE TABLE checkouts(
    id UUID PRIMARY KEY NOT NULL DEFAULT uuid_generate_v1(),
    creation_timestamp_utc TIMESTAMP WITH TIME ZONE NOT NULL,
    expiration_timestamp_utc TIMESTAMP WITH TIME ZONE NOT NULL,
    data JSONB
);