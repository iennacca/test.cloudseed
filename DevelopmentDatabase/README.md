# Connecting to database

Assuming that we already have the docker running. We can connect to it via:

* `sudo docker exec -it NAME_OF_DB_CONTAINER bash`
    * `sudo docker exec -it linetimes-server_linetimes_database_1 bash`
* In bash: `psql -U NAME_OF_DEV_USER -d NAME_OF_DATABASE -c 'SQL_COMMAND'`
    * `psql -U linetimes_server -d linetimes -c 'SELECT * FROM best_times_live_forecast_response_log'`

Remote:

* `psql -h HOST -p PORT -U NAME_OF_DEV_USER`
    * `psql -h localhost -p 5432 -U linetimes_server`

# Postgres commands

* List all databases: `\l`
* 