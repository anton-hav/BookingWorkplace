# BookingWorkplace
The project implements a service to manage workplace reservations in the office.

## About
This project implements a service to manage workplace reservations in the office using ASP.NET Core MVC.

### Main application features
- authorization system,
- role based functional division,
- workplaces management,
- equipment management,
- reseration history,
- creating reservations based on filters,
- generating messages about important events and sending them to the owner.

### Functionality available to the user
- the user can view all workplaces,
- the user can view workplaces details,
- the user can view all equipment types,
- the user can view equipment type details,
- the user can view the history of their own reservations,
- the user can reserve workstations,
- the user can manage the history of his reservations.

### Functionality available to the admin
- the admin can manage workplaces,
- the admin can manage equipment types,
- the admin can add equipment to workplaces,
- the admin can view the all history of users reservations,

## Application preview
This part in progress

<details>
  <summary>Registration</summary>

  ![image](https://drive.google.com/uc?export=view&id=1eyHNVze2NxJBIexSsawyAlyh_LfS2omt)
</details>

<details>
  <summary>Login</summary>

  ![image](https://drive.google.com/uc?export=view&id=1LtagXhUNxgP5ZT9t7OO5jbFPVn-mS4uk)
</details>

<details>
  <summary>Logout</summary>

  ![image](https://drive.google.com/uc?export=view&id=1MPRx5aAvz1ylIq4vXCsTMes02edF2_Cu)
</details>

<details>
  <summary>Unauthorise access</summary>

  ![image](https://drive.google.com/uc?export=view&id=15ayhkD43iyzvOZLo3NfK_D_aCwQVzt0z)
</details>

<details>
  <summary>Workplace by user</summary>

  ![image](https://drive.google.com/uc?export=view&id=1Z_US57Wuj7wJXm_Ulg5talmnUsphOT5B)
</details>

<details>
  <summary>Equipmen by user</summary>

  ![image](https://drive.google.com/uc?export=view&id=1vLnAvaZlr2tuR6wyP56y3XKgRoVjvDxW)
</details>

<details>
  <summary>Reservation by user</summary>

  ![image](https://drive.google.com/uc?export=view&id=1O8d6hWXeJSd9phT6X4kpQ8kKuDf8KEX5)
</details>

<details>
  <summary>Safe calendar</summary>

  ![image](https://drive.google.com/uc?export=view&id=1oP7lqeS2bVj31CeMtL-DPiwdOdXYT32w)
</details>

<details>
  <summary>Workplace by admin</summary>

  ![image](https://drive.google.com/uc?export=view&id=1U-SdJrey051_e9cLn52dJSYkx-K4BFks)
</details>

<details>
  <summary>Equipmen by admin</summary>

  ![image](https://drive.google.com/uc?export=view&id=10uACAlXONtn6WdYDp8Wvjd7OxAdIkFa4)
</details>

<details>
  <summary>Send a message about new reservation</summary>

  ![image](https://drive.google.com/uc?export=view&id=1rEJaTypu7hAMUohUvzNam51SUhqtNDVe)
</details>

<details>
  <summary>Send a message about new equipment movement</summary>

  ![image](https://drive.google.com/uc?export=view&id=15I5cvfGhhypEr9Tmw7SpAYsNWeo6aFYk)
</details>



## How to configure
### 1. Change database connection string
Firstly, you should change connection string in `appsettings.json`

```json
"ConnectionStrings": {
    "Default": "Server=myServer;Database=myDataBase;User Id=myUsername;Password=myPassword;TrustServerCertificate=True"
  },
```

If you run application in dev environment, change connection string in `appsettings.Development.json`

```json
  "ConnectionStrings": {
    "Default": "Server=YOUR-SERVER-NAME;Database=BookingWorkplaceDataBase;Trusted_Connection=True;TrustServerCertificate=True"
  },
```

>**Note**
> The project is designed to use Microsoft SQL.Server version 15 or higher. For versions below 15 stable operations is not guaranteed. The project uses the GUID type as the primary and therefore using other database providers requires additional changes and adjustments.

#### 2. Initiate database
Open the Packege Manager Console in Visual Studio (VIew -> Other Windows -> Packege Manager Console). Choose BookingWorkplace.DataBase in Default project.

![image](https://drive.google.com/uc?export=view&id=1sYz8qHzqTZXRbouNwXHwMAI0U-mhsi4O)

Use the following command to create or update the database schema. 

```console
PM> update-database
```

## Description of the project architecture.
### Summary
The application is based on ASP.NET Core MVC and Microsoft SQL Server. Entity Framework Core is used to work with the database. The interaction between the application and the database is done using the Generic Repository and Unit of Work.

The application writes logs to a new file for each run. Logging is based on the Serilog library.

Key functions of the server part:
- generic repository
- unit of work
- paginator
- search box
- complex filter box
- claims based authorization
- patch model and patching
- custom session class and session extension;

### Database model
Database contains four entities: 

- User: keeps information about users (email, password hash, fullname, roleId)
- Role: keeps name of role
- Reservation: keeps reservation information (user id, workplace id, time from, time to)
- Workplace: keeps information about the location of the workplace (floor, room, desk number)
- EquipmentForWorkplace: keeps information about what equipment is in the workplace
- Equipment: keeps equipment type

>**Note**
> The first and last name fields are intentionally replaced by the FullName field in the Users table. This approach allows people of different cultures and ethnicities to use the application.

<details>
  <summary>Database structure</summary>

  ![image](https://drive.google.com/uc?export=view&id=1H8B4cQLLhPd8NLioWxYU_u9y-JjZ1Nyb)
</details>

### Composition of solution
The solution contains the main project and several libraries:

- **BookingWorkplace:** main project
- **BookingWorkplace.Business:** contains the basic business logic of the application not directly related to MVC (services implementations and etc.)
- **BookingWorkplace.Core:** contains entities that do not depend on the implementation of other parts of the application (interfaces of services, data transfer objects, patch model)
- **BookingWorkplace.Data.Abstractions:** contains interfaces for database logic utils
- **BookingWorkplace.Data.Repositories:** contains implementation of Data.Abstractions
- **BookingWorkplace.DataBase:** contains entities and DBContext class

### Controllers
The BookingWorkplace contains five controllers:
- **HomeController:** logic for the main page 
- **AccountController:** provides the account operation like register, login, logout, etc.  
- **EquipmentController:** provides logic for create, read, update equipment
- **ReservationController:** provides logic for create, read, update reservations
- **WorkplaceController:** provides logic for create, read, update workplaces

### Features of integration
The application uses the `ConsoleSender` class, which sends information about important events to the console. If you need to send messages to messenger or email, you should develop a `CustomSender` class that implements the `ISender` interface. And connect it to IOC in `Program.cs`

### Description of the workplace selection algorithm
The application uses the following rules when selecting workplaces:
- if a workplace is reserved now or in the future - you cannot move equipment from it;
- the same workplace cannot be reserved twice for the same time;
- a user cannot have several workplaces reserved for the same day;
- equipment doesn't just show up, it has to be in the workplace. Then you can try to move it to where it is needed

## Key features:
ASP.Net Core MVC, Entity Framework Core, Microsoft SQL Server, C#, JavaScript, Serilog, Automapper, Newtonsoft Json.Net, Dependepcy Injection, Generic Repository, Unit of Work, Bootstrap, Claims, Sessions, Pagination, Search filters

