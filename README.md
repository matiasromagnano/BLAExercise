# Welcome to the BLA Exercise!

## User Story: Managing my Sneaker Collection with the SneakerCollection API

As a sneaker collector, I want a Web API that allows me to **manage my personal collection of sneakers securely**. This includes:

- **Registering for an account** so I can have my own space to store my collection.
- **Logging in** to access my collection securely.
- **Adding new sneakers** to keep my collection up to date.
- **Viewing my collection** with options to **sort** and **paginate**, making it easy to browse through my sneakers.
- **Updating sneaker details** when I need to correct or change information.
- **Deleting sneakers** I no longer want in my collection.
- **Viewing specific sneaker details** to check information about a particular pair.

## Introduction to the Analysis
The project is a web application built to manage Sneakers and Users, adhering to Clean Architecture principles and Test-Driven Development (TDD) methodologies, using ASP.NET Core and SQL Server.

# Project Structure and Architecture
The repository showcases a well-organized structure, with separate projects for Application, Domain, Infrastructure, Presentation, and Tests. This reflects a Clean Architecture implementation, structured as follows:

<img src="images/ProjectDepenencyDiagram.png" alt="Project Dependency Diagram" style="display: block; margin: 0 auto;">

## Presentation Layer
Located in Presentation/Controllers, it houses controllers like AuthenticationController and SneakerController, handling HTTP requests and delegating to application services. This keeps the presentation layer as an adapter, free of business logic.
This layered approach ensures modularity, maintainability, and scalability, aligning with Clean Architecture principles. The decision to use this structure allows for layer independence, enabling changes in implementation (e.g., switching from SQL Server to another database) without impacting the business core.

## Application Layer
Found in Application/Services, it includes services such as SneakerService and UserService, which encapsulate business logic and coordinate operations between the domain and infrastructure layers. For instance, SneakerService handles adding or updating sneakers by delegating to repositories.
Infrastructure Layer: In Infrastructure/Data, it manages data persistence with repositories like SneakerRepository and UserRepository, leveraging SQL Server for database interactions. This is configured in Program.cs with dependency injection.

## Domain Layer

Located in Domain/Models, it contains entities like Sneaker and User, representing the pure business model without external dependencies. This isolates the core business logic, as seen in files like Sneaker.cs.

## Infrastructure Layer

The project uses SQL Server, the DatabaseCreator class in Infrastructure/Data creates the SneakerCollectionDB database and the Users and Sneakers tables, seeding initial data. This setup facilitates testing, with sample users like "maria.gonzalez@example.com" and associated sneakers, as shown in the SQL script:

Choosing SQL Server was driven by its maturity and robust support within the .NET ecosystem, simplifying the management of relationships like Sneakers to Users. However, per the exercise requirements, I avoided Entity Framework and Dapper, opting for generic repositories with ADO.NET to meet the constraints.

- *Note: If at any moment you need to delete de database to start over, and you don't have access to SSMS to do it manually, don't worry, I've created a powershell script to delete it. You can find it in the root folder of the solution, open a terminal on the root folder and run the below command:*

```console
PS C:\Users\matt\source\repos\BLAExercise> .\DeleteDatabase.ps1
Opening connection to localhost,1433...
Executing delete script for SneakerCollectionDB...
No message returned from SQL Server, but operation completed.
Connection closed.
PS C:\Users\matt\source\repos\BLAExercise>
```

If you open the script you'll se we can send 4 parameters (if you don't send any we use the default ones):

```console
param (
    [string]$Server = "localhost,1433",
    [string]$Database = "SneakerCollectionDB",
    [string]$UserId = "sa",
    [string]$Password = "YourStrongPassw0rd!"
)
```

So if we call the script as below, this will delete the SneakerCollectionDB by default, but for example, you want to delete the test database (this one is created the first time you ran the integration tests), you can send some parameters to this script too:


```console
.\DeleteDatabase.ps1 -Database "SneakerCollectionTestDB"
```

This will delete the Test Database.

#### DB Diagram

<img src="images/DB_Diagram.png" alt="DB Diagram" style="display: block; margin: 0 auto;">


## Tests Layer

The project includes both unit and integration tests, adhering to TDD principles. Unit tests in BLAExercise.Tests/Application cover services like SneakerService and UserService, utilizing CustomFaker for realistic test data and AutoMapper to validate mappings. Integration tests in BLAExercise.Tests use WebApplicationFactory to test endpoints, covering success cases, not-found scenarios, and errors, with simulated JWT authentication for protected endpoints.

Choosing TDD ensures robustness, with comprehensive coverage for scenarios like validating user existence before adding sneakers and handling exceptions with NotFoundException and CommonException. This proactive approach to code quality is evident in test files like SneakerServiceTests.

To show how good the test coverage is, I'm using a third party application on Visual Studio called "Fine Code Coverage" it gives us details about each line ad it's coverage, here is a screenshot of the report, the app has above 90% of coverage!

### Summary
<img src="images/TestCodeCoverage_Summary.png" alt="DB Diagram" style="display: block; margin: 0 auto;">

---

### Details
<img src="images/TestCodeCoverage.png" alt="DB Diagram" style="display: block; margin: 0 auto;">

# Authentication
Authentication is implemented using JWT with the JwtBearer scheme, configured in Program.cs:

```c#
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection(nameof(ApplicationOptions)).GetSection(nameof(ApplicationOptions.JWTSecretKey)).Value!)),
            ValidateIssuer = false,
            ValidateAudience = false,
        };
    });

```
The AuthenticationController allows token generation without authentication (```[AllowAnonymous]```), while other endpoints, such as those in SneakerController, require valid tokens (```[Authorize]```). This ensures that only authenticated users can access sensitive operations like creating or updating sneakers. I chose JWT for its scalability and industry-standard status, though the JWTSecretKey in appsettings.json ("your-secret-key-here") is a placeholder and should be replaced with a strong key in a production environment, I decided to do it like this for simplicity purposes and time constraints.


# Design Decisions and Challenges

## Separation of concerns

I structured the application with a clear separation of concerns, following the flow ```Controller => Service => Repository```, to align with Clean Architecture principles and enhance maintainability. Here’s why I made these choices:

- **Controllers as Entry Points**: Controllers in the Presentation layer act solely as HTTP request handlers, receiving inputs (e.g., DTOs from API calls) and delegating to the Application layer’s services. This keeps them lightweight and focused on presentation concerns, avoiding business logic leakage. For example, SneakerController simply calls ISneakerService methods, ensuring it remains an adapter rather than a decision-maker.

- **Services for Business Logic**: I placed business logic in the Application layer’s services (e.g., SneakerService, UserService) because they serve as the central coordinators of the application’s functionality. This isolates rules—like validating user existence before adding a sneaker or enforcing data consistency—from both the presentation and infrastructure layers. By doing so, I ensured that the logic is reusable, testable, and independent of how data is stored or presented. For instance, SneakerService.AddAsync checks user existence via IUserService before proceeding, encapsulating this rule cleanly.

- **Repositories for Infrastructure**: I implemented repositories (e.g., SneakerRepository, UserRepository) in the Infrastructure layer to handle all data access operations, abstracting the persistence details (e.g., SQL Server queries via ADO.NET) from the business logic. This separation allows the Application layer to focus on what needs to be done (e.g., "add a sneaker") without knowing how it’s stored (e.g., SQL INSERT statements). It also makes swapping databases (e.g., from SQL Server to PostgreSQL or any other platform) straightforward, as only the repository implementation changes.

- **DTOs in the Application Layer**: I decided to place Data Transfer Objects (DTOs) like SneakerCreateDto and UserLoginDto in the Application layer because they represent the data contracts between the Presentation and Application layers. By defining them here, Services can directly use these DTOs as inputs and outputs, ensuring they’re unaware of HTTP-specific details (like request/response formats) while still being accessible to Controllers. This avoids tight coupling with the Presentation layer, adhering to dependency inversion. For example, SneakerService.AddAsync accepts a SneakerCreateDto, processes it, and returns a SneakerDto, keeping the flow consistent and encapsulated.

## Exception Handling: 

I introduced a NormalizeApiResponse filter to standardize API responses, enhancing consistency and security by preventing sensitive data leaks. This is visible across controllers, ensuring predictable outputs.


## JWT Authentication:

 I chose JWT for its scalability and widespread adoption within the industry, as it provides a robust mechanism for securing access to protected endpoints. I acknowledge that the JWTSecretKey should be strengthened for a production environment to enhance security. However, given the time constraints and the scope of this exercise, I believe the current implementation is sufficient for demonstration purposes.

## Testing Strategy:

I used WebApplicationFactory for integration tests with JWT simulation and CustomFaker for realistic data, ensuring full coverage of success and failure cases. This reflects a commitment to quality and reliability.

<br>

# How to run the Application:
- Clone this repository.
- Download and install [Docker Desktop](https://www.docker.com/products/docker-desktop/). (Follows the step and once done make sure you have it running before proceeding to next step).
- Run Docker and verify is running.
- Open a command line as Administrator, navigate to the repository path in which the docker-compose.yml file is located and run the following command: ```docker-compose up ```


- You should see how the sql image is pulled from the docker hub and the container starts running.
    - If you want to confirm it you may open another cmd and run ```docker ps``` to list the container and confirm is up.

- *Note*: The DB will be automatically created once you run the app, so no extra steps here!.

### **We are all set!**

We want to able to have Web API endpoints to start interacting with it, that's why we have SwaggerUI configured so it's easier to communicate with the System (of course that you can use Postman or any other client of your preference too). 

### Steps to run the API:

- Using Visual Studio
    - Set **"BLAExercise.Presentation"** project as startup project.
    - Set the app to Release mode (so you avoid getting handled exceptions to take the focus).
    - Press "F5" or click or the "Play" button.
    - If asked, trust the certificates and proceed to the website.
    - You should now see this:

- Using the terminal
    - Open your terminal on YouRepoFolder\BLAExercise\BLAExercise.Presentation folder
    - Run this command: ``` dotnet run --configuration Release --urls "https://localhost:7123" --launch-profile "https"```

    - Navigate to https://localhost:7123/swagger/index.html


You should now be able to see the Swagger UI on your browser now, or you can call the API using any client like Postman to make some request if you prefer.


<img src="images/SwaggerUI_1.png" alt="Swagger UI" style="display: block; margin: 0 auto;">
<br>

In order to be able to Authorize and use the API you will need to use the below credentials and call api/Authentication endpoint to get the token.

Here, we can use and extisting user from the initial Database Seed or you can call the api/User Post method to create a new user (User Registration) and authenticate with it.

```
{
  "email": "john.doe@example.com",
  "password": "MyPassword456@"
}
```

<img src="images/SwaggerUI_2.png" alt="Swagger UI" style="display: block; margin: 0 auto;">
<br>

You will get an API response like below with the token so you can quickly pick it up:

<img src="images/SwaggerUI_3.png" alt="Swagger UI" style="display: block; margin: 0 auto;">

```
{
  "statusCode": 200,
  "message": "Success",
  "data": "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJqb2huLmRvZUBleGFtcGxlLmNvbSIsIm5iZiI6MTc0MDQwNzc5OSwiZXhwIjoxNzQwNDk0MTk5LCJpYXQiOjE3NDA0MDc3OTl9.U8ZITb7BmeYos1ihJltoX2BeuRDS-Y_wLUQAL8nphlajiPOwdC6q4B62Ikg3gYWcT02vMsahbe5qHjlzg6Jdzw",
  "details": null
}
```

Once you got it, click the Authorize button (at the beginning on the Swagger page on the right), paste the token on the textbox and click Authorize. See image below.

After click on Authorize just close the popup. You are now authorized to call any other endpoint and interact with our API.


You can now click on the desired endpoint, then on "Try it out" and set the parameters as you like, note that we have pagination and sorting implemented.
Once done, click "Execute" and boom, you are interacting live with our API!

## Some examples responses

```[GET] api/Sneakers```

<img src="images/SwaggerUI_4.png" alt="Swagger UI" style="display: block; margin: 0 auto;">
<br>

``` [GET] api/User/{id} ```

<img src="images/SwaggerUI_5.png" alt="Swagger UI" style="display: block; margin: 0 auto;">
<br>

---

Thank you for your time, and if you have any questions or concerns, please reach me out to me via email: **matiasromagnano@gmail.com**.