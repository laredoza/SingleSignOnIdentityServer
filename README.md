# Single Sign-on Server

An Angular front end for IdentityServer4. [IdentityServer4](http://docs.identityserver.io/en/latest/) is an OpenID Connect and OAuth 2.0 framework for ASP.NET Core. 

### Features

- Manage Clients;
- Manage Api Resources;
- Manage Identity Resources;
- Manage Users;
- Manage Roles;

### There are two Api projects.
- IdentityServerAspNetIdentity ( This offers the main IdentityServer4 functionality )
- AdminApi ( Dotnet core api used to manage the Idenity4 server ) 

### Database Migration & Seeding
- Postgres ( Run the SingleSignOn.Migrations.Postgres project )
- SqlServer ( Run the SingleSignOn.Migrations.SqlServer project. Not tested yet )

### There is also an angular admin application
- [Admin](https://dev.azure.com/laredoza/SingleSignOn) ( Angular application used to manage IdentityServer4 )

### Sample Applications
- Angular
- Api
- Client
- Javascript Client
- MvcClient
- ResourceOwnerClient

### Project Status

- [x] Client Management
    - [x] Add
        - [x] Add No Interactive User
        - [x] Add Resource Owner
        - [x] Add MVC - Open Id Hybrid
        - [x] Add Javascript
        - [x] Add Angular
    - [x] Manage Client
        - [x] Add / Edit
        - [x] Scopes
        - [x] RedirectUris
        - [x] PostLogoutRedirectUris
        - [x] AllowedCorsOrigins
        - [x] AllowedScopes
        - [x] Keys 
- [ ] Resource Management
    - [ ] Api Resources
        - [x] Add / Edit
        - [x] Claims
        - [x] Properties
    - [ ] Identity Resources
        - [X] Add / Edit
        - [X] Claims
- [ ] Identity Management
    - [ ] Users
        - [X] Add / Edit
        - [X] Roles
        - [X] Claims
        - [X] Change Password
    - [ ] Roles
        - [X] Add / Edit
        - [ ] View Users in Group
