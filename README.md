# FC Manager — Football Management System

A web-based football management simulation built with ASP.NET Core MVC. Users can manage a club, set tactics, navigate the transfer market, simulate live matches, and track league standings across a full season.

Made for CSCB766 at NBU

---

## Features

- **Authentication** — Session-based login with role support
- **Team Management** — Browse and manage clubs with logos and ratings
- **Player Management** — Full player pool with stats, positions, and overall ratings
- **Squad & Tactics** — Drag-and-drop tactical board with formation selection (4-3-3, 4-4-2, 3-5-2, 4-2-3-1, 5-3-2); formation is persisted per team
- **Transfer Market** — Buy and sell players between clubs
- **Season & Fixtures** — Auto-generated fixture list ordered by matchweek
- **Live Match Simulation** — Tick-based match engine that simulates minute-by-minute events (goals, yellow/red cards, commentary) using player stats and formation matchups
- **League Table** — Live standings with points, goal difference, wins, draws, and losses
- **Dashboard** — KPI overview, league table, news feed, and a Manager Hub quick-access carousel

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core MVC (.NET 8) |
| ORM | Entity Framework Core |
| Database | Microsoft SQL Server |
| Frontend | Razor Views, Vanilla JS, Bootstrap 5 |
| Session | ASP.NET Core Session (cookie-based) |

---

## Project Structure

```
football_management_system_cscb/
├── Controllers/         # MVC controllers (Match, Squad, Season, Market, etc.)
├── Models/              # Domain models (Player, Team, User, Fixture, Squad, etc.)
│   ├── Formation/       # Formation definitions and library
│   ├── Match/           # Match state, events, result models
│   └── Season/          # Fixture and season models
├── Services/            # Business logic (MatchEngine, SquadService, etc.)
├── ViewModels/          # View-specific models
├── Views/               # Razor views per controller
├── Data/                # DbContext and EF configuration
└── wwwroot/             # Static assets (CSS, JS, images)
```

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server (local or remote)
- Visual Studio 2022 or VS Code

### Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/your-username/football-management-system.git
   cd football-management-system
   ```

2. **Configure the connection string** in `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER;Database=FCManager;Trusted_Connection=True;"
   }
   ```

3. **Apply migrations and seed the database**
   ```bash
   dotnet ef database update
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

5. Open your browser at `https://localhost:7176`

---

## Match Engine

The match simulation runs on a tick-based engine (`MatchEngine.cs`) that advances the match one minute at a time. Each tick:

- Calculates team strength from player stats (shooting, pace, defense, passing, stamina)
- Applies formation advantages via `FormationMatchupService`
- Generates expected goals (xG) and probabilistically scores goals
- Fires random events — yellow cards, red cards, substitutions, commentary
- Persists the result to the fixture record on full time

---

## Key Design Decisions

- **Session-based match state** — the live match is stored in server-side session via `SessionMatchStore`, avoiding database writes on every tick
- **Formation persistence** — `DefaultFormation` is stored on the `Team` entity and read by the match engine and squad builder at runtime
- **Role-based routing** — users are assigned a team on first login; all squad, match, and fixture actions are scoped to their `TeamId` from session

---

## Author

Developed as a university project for the Computer Science with Business degree programme.
