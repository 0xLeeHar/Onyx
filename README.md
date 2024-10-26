# Onyx coding challenge

## Products service

The products service can be found in `~/Backend/Onyx/`. The general structure is a modular monolith, built in a manor so that each service is contained and could be deployed independently in future.

Its formed of several .NET 8 applications:
1. `Onyx.Api` is a WebAPI application and hosts the other services.
2. `Onyx.Services.Products` is the main products service.
3. `Onyx.Services.Products.Tests` contains tests for the products service.
4. `Onyx.Common` contains abstractions and shared code. 

### General structure

Currently `Onyx.Api` hosts the other self-contained "Services", this could change in future and the services could be hosted independently.

### Message bus
[Rebus](https://github.com/rebus-org/Rebus) was used as an abstraction over an in-memory message bus. Commands and Events can be past between services to extend functionality.

### Data persistence
[EF Core](https://github.com/dotnet/efcore) with SQL Server and In-memory database have been implemented at the data persistence layer. 
Currently each service "owns" its own data segregated by schema.

### Repository Pattern
This has been implemented as a simple way to control data mutations and reads and allow for easer testing. Other approaches like CQRS could be a viable alternative.

### Async APIs
As the API controller rases a command to create a new product, this has induced the use of async APIs. A resource ID is retured and then the calling code will need to wait for its creation.

There are pros and conns here. It allows for retries, better error handling, ect, but also adds overhead to the calling code.

### Authentication

Two authentication schemes have been implemented. JWT Bearer tokens and API Keys.  
1. JWT will rely on an external OAuth provider, settings are loaded from the config.  
2. The API Key is hard coded and set to `fakeapikey` in the app settings

> In production the API key (or keys) should be stored and loaded from a secrets store.

### TODO

> For production the in-memory message bus and database will need to be hosted, along with the some implementation of an OAuth provider.

### Tests

There are examples of both functional and units tests. They are a _light-touch_ approach.

The functional tests check the workflow from command being rased and ensure the expected DB write is preformed. A `TestFixture` is used to maintain the tests lifetime context.

The unit tests check a single unit of code is working as expected. They use Moq and Fakes to stub out external code.

### Postman 

There is a Postman collection in `~/Backend/` that includes the endpoint definitions.

# Architecture diagram

The architecture diagram can be found at `~/diagram`.

It shows how this `Products` service can be incorporated with other services and the inversion between them.