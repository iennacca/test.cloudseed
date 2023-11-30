# Overview

CloudSeed was built to provide a simple, solid F# project base.

CloudSeed is split into three primary services:

* App - Your app logic. Everything from business logic to data access to UI creation (if you stick with Server-side Rendering)
    * Built with F# and Giraffe (ASP.NET)
    * Template views are created with Giraffe.ViewEngine but it's easy to switch to other rendering paradigms should you wish.
* App.Tests - Tests for App
* DB - A local, dockerized DB you can spin up to simulate real-world deployment + integration use cases.
    * Postgres

# Getting Started

1. Clone the repo
2. Change the upstream
    * You won't be able to push to CloudSeed, but you may still want the upstream handy in case you want to grab some updates
    * Rename cloudseed remote: `git remote rename origin cloudseed`
    * Add your repo as origin: `git remote add origin URLHERE`
    * Note: CloudSeed is not built to be backwards compatible, so it's not encouraged to blindly do an update and expect things to work exactly - though we'll do as little breaking changes as we can
3. If you're planning to build many projects with cloudseed, you will need to modify the `docker-compose` so that the container names don't collide on your machine
    * Repeat for `docker-compose.test`

# Development

## Building and running locally

From root: `docker-compose down --remove-orphans && docker-compose build && docker-compose up`

If all goes smoothly, you should be able to access each service at:

* App - `localhost:5001`
    * Verify at: `localhost:5001/sentinels`
* DB - `localhost:5002`

Additional commands

* RM all old artifacts: `sudo docker-compose down`
* Full rebuild - `sudo docker-compose build`
* Run: `sudo docker-compose up`

## Testing

Run both from root:

* Setup DB (this must stay running): `docker-compose -f docker-compose.test.yml down --remove-orphans && docker-compose -f docker-compose.test.yml up cloudseed_db_test`
* Run Tests (run this in a new terminal): `docker-compose -f docker-compose.test.yml build cloudseed_app_tests && docker-compose -f docker-compose.test.yml up cloudseed_app_tests`

# Data

## Data Migrations

By default, CloudSeed uses DBUp for migrations.

All database model upgrades happen via DBUp. DBUp will upgrade the database at app startup, running scripts in alphabetical order found in `./App/Source/Infrastructures/DatabaseUpgradeScripts`. 

This is the _single source of truth_ for upgrades. It can run any sql compatible with Postgres by default - though you can always change to a different sql engine if you prefer.

## Data ORM

CloudSeed is pre-configured with Entity Framework. 