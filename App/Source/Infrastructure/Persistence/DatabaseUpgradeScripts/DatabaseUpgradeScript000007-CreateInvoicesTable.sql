CREATE TABLE invoices(
    id UUID PRIMARY KEY NOT NULL DEFAULT uuid_generate_v1(),
    user_id UUID,
    creation_timestamp_utc TIMESTAMP WITH TIME ZONE NOT NULL,
    data JSONB,
    CONSTRAINT fk_invoices_users
        FOREIGN KEY (user_id)
            REFERENCES users(id)
);