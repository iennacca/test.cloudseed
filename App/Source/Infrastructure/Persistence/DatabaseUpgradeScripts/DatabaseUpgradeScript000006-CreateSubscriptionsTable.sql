CREATE TABLE subscriptions(
    user_id UUID NOT NULL references users(id),
    product_id VARCHAR(128) NOT NULL,
    start_timestamp_utc TIMESTAMP WITH TIME ZONE NOT NULL,
    expiration_timestamp_utc TIMESTAMP WITH TIME ZONE NOT NULL,
    data JSONB,
    PRIMARY KEY (user_id, product_id)
);