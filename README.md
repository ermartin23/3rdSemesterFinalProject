README file

# 🐦 Dead Pigeons — Jerne IF Lottery System
Final Project — Programming II · CDS Security · Systems Development

**Team:** CodeBusters

**Team Members**
Emre
Erica
Katja
Laura

---

## Project Overview

Dead Pigeons is a weekly lottery-style game played by supporters of Jerne IF. This project delivers a full digital platform for managing players, transactions, number boards, weekly games, authentication, and admin workflows.

The system is fully distributed: a React frontend communicating with a .NET Web API backend, deployed to the cloud with secure handling of all secrets and user data.

---

## Tech Stack

### Backend
The backend is implemented as a .NETWeb API, which is responsible for business logic, persistence and security.

- .NET 9 Web API
- Entity Framework Core (PostgreSQL)
- Authentication & Authorization (JWT)
- Server-side validation
- Swagger / OpenAPI (NSwag-generated client)
- GUID-based IDs
- GitHub Actions CI (build + test)
- XUnit + XUnit.DependencyInjection
- TestContainers (isolated test database)
- Docker (for development + deployment)

### Frontend
The frontend is a React client that’s responsible for user interaction and presentation logic.

- React + TypeScript
- React Router
- Vite
- Component styling with TailwindCSS/DaisyUI
- API communication via fetch/NSwag client

---

## Infrastructure & Security

- Cloud deployment via Fly.io
- No secrets in git
- Environment-based configuration
- Secure password hashing
- Authorization policies based on roles

---

## Repository Structure

/client → React + TypeScript frontend
/server → .NET Web API backend
/tests  → Automated backend tests
README.md

---

## Deployment (Fly.io)
### Backend

**Fly.io App Monitoring**
https://fly.io/apps/deadpigeonsdev-muddy-flower-4166/monitoring

**Public API URL**
https://deadpigeons-aged-voice-1352.fly.dev

### Frontend
The frontend is deployed separately and configured via environment variables to communicate with the Fly.io backend.

---

## Test Accounts & Password Hashing

### Admin
- **Email:** firstadmin@example.com
- **Password:** MyAdminPassword123!

### Player
- **Email:** secondplayer@example.com
- **Password:** secondplayer123

Passwords are never stored in plain text.
All credentials are hashed before persistence.

---

## Password Hashing Tool

A small utility is included to generate hashed passwords for seeding and testing.

dotnet run --project server/HashTool -- <PASSWORD_YOU_WANT>

Example:
dotnet run --project server/HashTool -- MyAdminPassword123!

---

## Authentication & Authorization

The system enforces role-based authorization using JWT tokens and ASP.NET Core authorization attributes.

Two roles exist in the system:

Admin
Player

Access is controlled using:

[Authorize]
[Authorize(Roles = "Admin")]
[Authorize(Roles = "Player")]

Ownership checks based on the authenticated user’s JWT sub claim (user id)

Authentication

Authentication is performed via POST /api/auth/login
On successful login, a JWT is returned

The JWT contains:
sub → user id (AdminId or PlayerId)

email
role → Admin or Player

JWT must be sent in the Authorization: Bearer <token> header

Role Overview
Role	Description
Admin	System administrator. Manages players, admins, games, boards, and transactions
Player	End user. Can manage own boards, view own transactions, and participate in games
Access Control by Feature

Auth Controller
Endpoint	Access
POST /api/auth/login	Public (anonymous)

Admins Controller (/api/admins)
Action	Access
View all admins	Admin
Create admin	Admin
Update admin	Admin
Delete admin	Admin

Players Controller (/api/players)
Action	Access
View all players	Admin
View player by id	Admin
Create player	Admin
Update player	Admin
Toggle active status	Admin
Soft delete player	Admin

Games Controller (/api/games)
Action	Access
View all games	Public
View game by id	Public
Create game	Admin
Set winning numbers	Admin
View game details	Admin
View latest winning numbers	Player

Boards Controller (/api/boards)
Action	Access
View all boards	Admin
View board by id	Admin (any board)
View board by id	Player (only own boards)
Create board	Player
Update board	Admin
Delete board	Player (only own boards)
View own board history (/me)	Player

Ownership is enforced by matching the board’s PlayerId with the authenticated user’s sub claim.

Repeating Boards (/api/repeatingboard)
Action	Access
Toggle repeating board	Player (only own boards)

Transactions Controller (/api/transactions)
Action	Access
View all transactions	Admin
View transaction by id	Admin
View player balance	Admin
View own balance	Player
View own transactions	Player
Create transaction	Player
Approve transaction	Admin
Reject transaction	Admin

Ownership Enforcement

For Player endpoints, access is restricted to their own data only:
Player ID is extracted from the JWT sub claim
Services validate ownership before returning or mutating data
Requests attempting to access another player’s data result in 403 Forbidden


## Testing Authorization via Swagger / OpenAPI

The API exposes a Swagger (OpenAPI) interface that allows manual testing of authentication and authorization rules.

How to Test

Open Swagger UI (local or deployed)
Call POST /api/auth/login
Copy the returned JWT token
Click Authorize in Swagger
Enter the token using the format:
Bearer <YOUR_JWT_TOKEN>
Execute protected endpoints

What Can Be Verified

Using Swagger, it is possible to verify that:
Anonymous users can only access public endpoints
Admin-only endpoints reject Player tokens (403 Forbidden)
Player-only endpoints reject Admin tokens where applicable
Players cannot access or modify resources they do not own
Missing or invalid tokens result in 401 Unauthorized

Expected Results
Scenario	Result
No token	401 Unauthorized
Wrong role	403 Forbidden
Valid role	200 OK / 201 Created
Purpose

Swagger testing demonstrates that:

Authorization policies are enforced server-side
Role claims inside JWTs are actively validated
Security rules are independent of the frontend

---

## Environment, Configuration & Linting

Configuration Sources (Backend)

The backend uses the Options Pattern (AppOptions) and reads configuration in this order:

1 - server/api/.env (loaded via DotNetEnv.Env.Load(...))
2 - Environment variables (via builder.Configuration.AddEnvironmentVariables())
3 - appsettings.json (defaults / placeholders)

AppOptions is validated on startup (required fields + JWT secret min length). If configuration is missing, the API fails fast with a clear error message (see AddAppOptions).

Required backend variables:

AppOptions__DbConnectionString
AppOptions__JwtSecret
AppOptions__JwtIssuer
AppOptions__JwtAudience

Example local .env:

AppOptions__DbConnectionString=Host=...;Database=...;Username=...;Password=...;SSL Mode=Require
AppOptions__JwtSecret=... (min 32 chars)
AppOptions__JwtIssuer=DeadPigeonsAPI
AppOptions__JwtAudience=DeadPigeonsClient

! appsettings.json contains empty placeholders 

Deployment Secrets (Fly.io)

In production, secrets are injected using Fly.io secrets (no secrets stored in git). Example:

fly -a deadpigeonsdev-muddy-flower-4166 secrets set \
AppOptions__DbConnectionString="..." \
AppOptions__JwtSecret="..." \
AppOptions__JwtIssuer="DeadPigeonsAPI" \
AppOptions__JwtAudience="DeadPigeonsClient"

Configuration Sources (Frontend)

The React client reads the API URL from Vite environment variables:

client/.env.development (local dev)
VITE_API_URL=http://127.0.0.1:5239


The value is used in src/core/config.ts:
export const baseUrl = import.meta.env.VITE_API_URL;


This allows switching between local and deployed backends without code changes.

Linting / Code Quality

Frontend code quality is enforced using the standard Vite + TypeScript toolchain (linting via ESLint if enabled in the project setup).
Backend code quality is enforced through build + tests in GitHub Actions (compilation + xUnit + integration tests with TestContainers).

---

## Known Issues / Current Limitations

Cookie-based authentication is not implemented yet
JWT is handled via HTTP headers
No refresh-token mechanism
Intended for academic and demonstration purposes