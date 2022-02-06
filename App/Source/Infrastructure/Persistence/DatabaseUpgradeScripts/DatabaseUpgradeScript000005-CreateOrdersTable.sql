CREATE TABLE orders(
    id UUID PRIMARY KEY NOT NULL DEFAULT uuid_generate_v1(),
    userid UUID NOT NULL references users(id),
    order_timestamp_utc TIMESTAMP WITH TIME ZONE NOT NULL,
    data JSONB
    -- CONSTRAINT fk_orders_users
    --     FOREIGN KEY (userid)
    --         REFERENCES users(id)
);