# Banking Transactional System - Recruitment Task
## Tech & Features
- C#
- ASP.NET Web API
- MS SQL Server
- Entity Framework Core (Code First)
- JWT Token Authentication
- Concurrent Operation Protection
- MediatR / CQRS
- SignalR
- Simple Logging and Exception Handling

## Setup
- No additional setup required, just build & start the app.
- Migrations run automatically on startup, and will create a DB if one is not present (localhost MS SQL required).
- UserId is not used for anything in the app, it'a just a name of the account.
- Some files are included for Docker, but in the end, it didn't work correctly for me. Would finish, but I ran out of time.
- The Accounts table will have the following records pre-seeded:

    | Id | Balance | UserId |
    | ------ | ------ | ------ |
    | 1 | 1000 | user1 |
    | 2 | 2000 | user2 |
    | 3 | 3000 | user3 | 
    | 4 | 4000 | user4 |
    | 5 | 5000 | user5 |

## API
- Default Ports
```sh
HTTP Listens on 5105
HTTPS Listens on 7141
```
- Obtain a JWT token
```sh
POST /api/auth/demo_token 
No Auth
Body (JSON):
{
  "username": "demo",
  "password": "demo"
}
```
- Deposit
```sh
POST /api/accounts/deposit
Auth: JWT Token (Bearer)
Body (JSON):
{
  "accountId": int,
  "amount": decimal
}
```
- Withdraw
```sh
POST /api/accounts/withdraw
Auth: JWT Token (Bearer)
Body (JSON):
{
  "accountId": int,
  "amount": decimal
}
```
- Get Balance
```sh
GET /api/accounts/{id}/balance
Auth: JWT Token (Bearer)
```
