# Overview

CloudSeed was built to provide a simple, solid project base for your application.

CloudSeed is split into three primary services:

* Web (Frontend) - A thin Web app - built to be heavily modified / replaced.
    * Built with Svelte / Sveltekit
* App (Backend) - The Core App logic, API, and where DB integration happens
    * Built with FSharp + .NET Core
* App.Tests - Tests for App
* DB - A local, dockerized DB you can spin up to simulate real-world deployment + integration use cases.
    * Postgres

The App is the core and the rest serve as useful but optional and replaceable additions.

## Getting Support

We will continually be adding and updating documentation on the site at https://cloudseed.xyz
 
If you're still stuck / have questions / see a problem: 

* Create an Issue on the GitHub
* Contact me at hamy+cloudseed@hamy.xyz

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

* If using VS Code, Run Tasks:
    * Debug: `Launch Docker Compose + Attach`
    * Run: Use Task: `launch-compose`

If all goes smoothly, you should be able to access each service at:

* Web - `localhost:5000`
* App - `localhost:5001`
    * Verify via: `localhost:5001/sentinels`
* DB - `localhost:5002`

Manual commands

* From root: `docker-compose down --remove-orphans && docker-compose build && docker-compose up`

Additional commands

* RM all old artifacts: `sudo docker-compose down`
* Full rebuild - `sudo docker-compose build`
* Run: `sudo docker-compose up`

## Testing

* If using VS Code, Run Tasks:
    * `launch-test-compose`

Manual commands:

* From root: `sudo docker-compose -f docker-compose.test.yml down --remove-orphans && sudo docker-compose -f docker-compose.test.yml up cloudseed_db_test` (this needs to stay running)

* From root, in new terminal: `sudo docker-compose -f docker-compose.test.yml build cloudseed_app_tests && sudo docker-compose -f docker-compose.test.yml up cloudseed_app_tests`

## Data Model

We believe in simple, accurate data model upgrades. This means having a single source of truth and always acting on it.

By default, all database model upgrades happen via DBUp. 

All database model upgrades happen via DBUp. DBUp will upgrade the database at app startup, running scripts in alphabetical order found in `./App/Source/Infrastructures/DatabaseUpgradeScripts`. 

This is the _single source of truth_ for upgrades. It can run any sql compatible with Postgres by default - though you can always change to a different sql engine if you prefer.

## Data Access

For the sake of simplicity, we've opted to make data access as transparent as possible. By default, CloudSeed utilizes Dapper for data persistence which allows you to write simple, performant object <> sql functions.

You can find examples of this in action in the `Sentinels` and `Counter` domains.

These are intentionally lightweight so it's easy to replace with another ORM if you wish.
