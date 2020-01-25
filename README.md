# Single Sign-on Identity Server

This service provides [IdentityServer4](http://docs.identityserver.io/en/latest/) based services. This includes OpenID Connect and OAuth 2.0. 

This service can be managed using [SingleSignOnApiUI](https://github.com/laredoza/SingleSignOnUI) 

## Features and Requirements

### Features

- Manage Clients;
- Manage Api Resources;
- Manage Identity Resources;
- Manage Users;
- Manage Roles;


![Preview](https://raw.githubusercontent.com/laredoza/SingleSignOnUI/master/SingleSignOn.gif)

### Requirements 
- [SingleSignOnApiUI ( Angular admin front end )](https://github.com/laredoza/SingleSignOnUI)
- [SingleSignOnApi ( Api used to manage the Identity4 server )](https://github.com/laredoza/SingleSignOnApi) 

## Database Configuration

### Postgres

Postgres is the default database selected.

- Update DatabaseType to "Postgres" in appsettings.
- Update the defaultConnection to "Host=localhost;Database=SingleSignOn;Username=postgres;Password=password1;" in  appsettings.json
- Run Postgres Migration & Seeding in [SingleSignOnApi](https://github.com/laredoza/SingleSignOnApi)

### Microsoft Sql Server

- Update DatabaseType to "MsSql" in appsettings.
- Update the defaultConnection to "Data Source=.;Initial Catalog=SingleSignOn;User ID=sa;Password=yourStrong(!)Password;" in appsettings.json
- Run Microsoft Sql Server Migration & Seeding in [SingleSignOnApi](https://github.com/laredoza/SingleSignOnApi)