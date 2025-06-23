# 📚 Book Collection Project

An experimental ASP.NET Core MVC project designed to demonstrate automatic entity change tracking and dynamic filtering & sorting in a clean, modular way.

⸻

## ✨ Key Features
- Automatic Change Tracking
Each time a tracked property of a entity is updated, a corresponding EntityChanged record of the appropriate type is automatically generated, providing a full history of modifications.
- Dynamic Filtering & Sorting
The EntityQueryComposer enables clients to define complex filter and sort queries directly via the API — supporting powerful, customizable data retrieval without manual query building.
- Minimalistic Front-End
The front-end is intentionally kept simple, using pure JavaScript and CSS served by ASP.NET MVC views. This design keeps focus on backend logic but can easily be decoupled for use with modern SPA frameworks like React, Angular, or Vue.
- API-First Design
All functionality — including data access, filtering, sorting, and change tracking — is exposed via a clean, RESTful API. This separation enables easy migration to alternative front-end technologies or external integrations.
- Reusable Core Components
    - EntityChangeTracker: Generic mechanism for detecting and recording entity changes.
    - EntityQueryComposer: Utility for generating LINQ expressions for filtering and sorting based on incoming API queries.
    Both components are fully decoupled and suitable for extraction into standalone libraries or NuGet packages.
- In-Memory Database: Uses Entity Framework Core In-Memory provider for easy setup, development, and testing without requiring an external database.

⸻

## 🏗️ Project Structure

```text
/Controllers           # API and MVC controllers
/Data                  # 
/Models                # Domain models: Book, BookChange, etc.
/Tools                 # EntityChangeTracker and EntityQueryComposer implementations, alongside other helpers
/Views                 # Basic Razor Views for demo purposes
/wwwroot               # Pure JS and CSS front-end assets
```
⸻

## 🚀 Potential Future Improvements
- Extract EntityChangeTracker and EntityQueryComposer to a reusable library/NuGet package.
- Replace MVC views with a SPA front-end (React, Vue, etc.) consuming the same API.
- Add unit and integration tests for change tracking and filtering logic.

⸻

## ⚠️ Known Issues
- The front-end lacks proper input validation.
- API response error handling is minimal and should be improved to handle unexpected or invalid server responses gracefully.

⸻

## 📝 Notes
This project is experimental and intended as a proof of concept for:
- Entity auditing (change logs)
- Backend-driven dynamic querying
- API-driven architecture
- In-Memory Entity Framework Core is used for persistence to allow easy setup, running, and testing without requiring an external database.