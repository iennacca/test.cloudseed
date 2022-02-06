# Getting Started

1. Clone the repo
2. Change the upstream
    * You won't be able to push to CloudSeed, but you may still want the upstream handy in case you want to grab some updates
    * Rename cloudseed remote: `git remote rename origin cloudseed`
    * Add your repo as origin: `git remote add origin URLHERE`
    * Note: CloudSeed is not built to be backwards compatible, so it's not encouraged to blindly do an update and expect things to work exactly - though we'll do as little breaking changes as we can
3. If you're planning to build many projects with cloudseed, you will need to modify the `docker-compose` so that the 'docker' names don't collide
    * In `docker-compose`, search for `cloudseed`.
    * Replace all with the unique name for your project 
    * This will allow you to work on multiple projects at once!
    * Repeat for `docker-compose.test`

# Development

## Building and running locally

From root.

* `sudo docker-compose down --remove-orphans && sudo docker-compose build && sudo docker-compose up`

If all goes smoothly, you should be able to access each service at:

* Web - `localhost:5000`
* App - `localhost:5001`
    * Verify via: `localhost:5001/sentinels`
* DB - `localhost:5002`

Additional commands

* RM all old artifacts: `sudo docker-compose down`
* Full rebuild - `sudo docker-compose build`
* Run: `sudo docker-compose up`

## Testing

From root.

* `sudo docker-compose -f docker-compose.test.yml down --remove-orphans && sudo docker-compose -f docker-compose.test.yml up cloudseed_db_test` (this needs to stay running)

In new terminal:

* `sudo docker-compose -f docker-compose.test.yml build cloudseed_app_tests && sudo docker-compose -f docker-compose.test.yml up cloudseed_app_tests`

## Updating the data model

We believe in simple, accurate data model upgrades. This means having a single source of truth and always acting on it.

All database model upgrades happen via DBUp. DBUp will upgrade the database at app startup, running scripts in alphabetical order found in `./App/Source/Data/DatabaseUpgradeScripts`. This is the _single source of truth_ for upgrades. It can run any sql compatible with Postgres.

Thus to update the data model, add a new script in `./App/Source/Data/DatabaseUpgradeScripts` with naming convention: `DatabaseUpgradeScriptXXXXXX-DESCRIPTION.sql`, like `DatabaseUpgradeScript000001-MyUpdate.sql`. This will then get applied to the database on deploy.

### EF Migrations

EF Migrations is a useful tool for generating code first updates to a data model (i.e. update the model in code then generate the sql to make that work for the db). However this has a lot of drawbacks:

* Lack of transparency in what updates are applied
* Lack of control over what changes are applied in what order
* Complexity due to creation of c# migrations which are then turned into sql migrations
* Lack of robust sql migration upgrade code

However we do acknowledge the usefulness of this tool for making code first changes and scaffolding sql to reflect it. So we've made it possible to do that here.

Here is how you can levereage EF Migrations to scaffold sql updates to the database which you can then review and commit to `DatabaseUpgradeScripts` (See Updating the data model) to deploy these changes to the database.

First make sure you have `dotnet` and `dotnet-ef` installed locally. Docs: https://docs.microsoft.com/en-us/ef/core/cli/dotnet

* Install: `dotnet tool install --global dotnet-ef`
* Update: `dotnet tool update --global dotnet-ef`

Next we'll create the migration scripts for our data model.

* Build and Run App and DB locally (see Building and running locally). Keep this running through the next processes.
    * `dotnet ef` requires a db to create snapshots and running the whole project further enables us to make sure we're on a working commit (we don't want to build a broken data model!)
    * Tip: Make sure that `./App/appsettings.Development.json` has the correct environment variables for connecting to the database (should match that in `./docker-compose.yml`). This will allow `dotnet ef` to talk to the database to create its snapshots
* In a new terminal, navigate to `./App`
* Take a snapshot of the original model (i.e. before data changes were made). This will likely mean stopping the app, checking out the commit before changes were made, and restarting the app.
    * Run `dotnet ef migrations add InitialTemp --output-dir ./MigrationsTemp`
    * This will take a snapshot of the existing db model and place that in `./App/MigrationsTemp`
* Save the new model (i.e. after data changes were made). This will likely mean stopping the app, checking out the commit where data model changes were made, and restarting the app.
* Take a snapshot of the new model
    * Run `dotnet ef migrations add NewTemp --output-dir ./MigrationsTemp`
* Examine the output migration and see if it makes sense. Make changes where necessary.
    * In particular, if check to see if any tables or columns are being deleted or updated, this could lead to data loss (typically dotnet ef will output warnings if this is happening)
* Create a sql script that captures these changes
    * Run `dotnet ef migrations script INITIALMIGRATION NEWMIGRATION --output ./MigrationsTemp/TOMIGRATE.sql` - where INITIALMIGRATION and NEWMIGRATION are the names of the migrations located in `./App/MigrationsTemp`
        * For example they might look like `dotnet ef migrations script 20220101_InitialTemp 20210101_NewTemp --output ./MigrationsTemp/TOMIGRATE.sql`
* Examine the output `TOMIGRATE.sql` to make sure it makes sense
    * Note: 
        * Remove the transaction code from the sql statement - we run DBUp in transactions anyway
        * Remove the `__EFMigrationsHistory` lines as we are not using EF upgrade mechanisms to handle data migrations.
* Once reviewed, move it to the `DatabaseUpgradeScripts` folder (and update the name) to deploy it to the database (see Updating the data model for further instructions)
    * Test it works
        * Run the app
        * Run the tests
* Delete the `./App/MigrationsTemp` folder so it doesn't clog up the workspace
* Repeat for any other data changes you want to use EF Migrations for

## Running dotnet commands in container

* sudo docker run --rm -it -v $(pwd):/app/ -w /app mcr.microsoft.com/dotnet/sdk:5.0.203-alpine3.13 dotnet YOUR_COMMANDS_HERE

* Create a new webApp - new webApp -o myWebApp --no-https
* create a webApi - new webapi --no-https
* install a package like Newtonsoft - add package Newtonsoft.Json
* Add reference to project - dotnet add PATH_TO_CSPROJ reference PATH_TO_REFERENCE_CSPROJ

# Deployment

## Deploying to Google Cloud Run

1. Create a GCloud project
2. Create SQL instance
    * CloudSQL -> Choose PostgreSQL
        * Enable whatever you need to enable perm-wise (should be a prompt)
3. Setup repo for Cloud Build
    * Enable Cloud Build
        * `Cloud Build -> Settings`
        * If a new project, will see a popup saying you need to enable the api
            * Click `View API`
            * Click `Enable`
        * It may take a few seconds for Permissions to load everywhere.
    * Set permissions - `Cloud Build -> Settings`
        * Cloud Run Admin: Enabled
        * Service Accounts: Enabled
    * Create a Cloud Build service account
        * `IAM & Admin > Service Accounts`
            * Create Service Account
            * Name: `GitLab CI Cloud Build`
            * Role: `Cloud Build Service Agent`, `Cloud Build Editor`
        * Go to the list of Service Accounts
            * Click on the new service account you created
            * Go to `Keys` tab
            * Add Key > Create Key > JSON
            * Hold onto key! Will call `GitLab CI Cloud Build Key`
    * Configure GitLab CI Repo
        * Go to your project's GitLab Repo
        * Set key via `Settings > CI/CD > Variables`
            * GCP_PROJECT_ID: ID of project
            * GCP_CLOUD_BUILD_SERVICE_KEY: `GitLab CI Cloud Build Key` (JSON file from earlier)
    * Configure environment variables
        * You must configure environment variables, otherwise the build will fail
        * Cloud SQL Connections
            * `Cloud Run > Server > Edit & Deploy New Revision > Connections`
                * Add Connection
                * Enable APIs
                * Select DB
                * Deploy
        * App
            * Fill in all environment variables used in docker-compose in CloudRun
        * Web
            * Fill in all environment variables used in docker-compose in CloudRun
4. Create Cloud Run instance
    * Push to your GitLab CI Repo
        * This will run the builders in GitLab and then push to Cloud Build which will create your Cloud Run services
        * Monitor your GitLab build by going to GitLab repo -> CI / CD -> Pipelines
            * If there are failures, look at the logs to see why
        * Monitor Cloud Run success by going to Cloud Run and looking for your services.
            * If everything was successful you should have:
                * web
                * app

# Troubleshooting

* ERROR: for X: Cannot create container for service x: conflict. The container name x is already in use by container "Y"
    * Use docker rm Y

## Intellisense

C#

* If intellisense is failing, go to App / App.Tests folder and run `dotnet build`. This will download all necessary dlls and dependencies directly folder so intellisense can pick it up.

## Windows

* Docker error: FileNotFoundError when running docker-compose
    * Click the running apps, find Docker icon
        * right click -> Quit Docker Desktop
        * Wait til stops
        * Run Docker Desktop
        * Wait til Started
        * If in WSL, using VS Code, and in integrated terminal, sometimes just need to restart VS Code editor
        * If still happening, may need to restart computer

## Stripe Development

### Stripe webhooks locally

* `docker run --rm --entrypoint /bin/sh -it stripe/stripe-cli:latest`
* `stripe listen --forward-to host.docker.internal:5001/webhooks/stripe`
* Follow login instructions from CLI, then rerun ^

## Query Database

* Create connection to db `docker run -it --rm postgres psql -h host.docker.internal -p 5002 -U cloudseed_business_database_user -W`
* Input the password at prompt
* Now query

Useful queries:

* List all databases `\l`
* Connect to database `\c DATABASENAME`
* List all tables in database `\dt`
* Run a query: Write query followed by a semicolon `select * from users;`