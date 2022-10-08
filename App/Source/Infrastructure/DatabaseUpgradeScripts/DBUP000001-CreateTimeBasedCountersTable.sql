CREATE TABLE time_based_counters(
    counter_id VARCHAR,
    timestamp_utc_epoch_ms bigint,
    hits bigint,
    PRIMARY KEY (counter_id, timestamp_utc_epoch_ms)
);