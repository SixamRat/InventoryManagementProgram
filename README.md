# InventoryManagementProgram
An inventory management system for bigger restaurant and hotels.
## Systemarkitektur
```mermaid
graph TD
    subgraph Klient
        A[Blazor PWA - Desktop]
        B[Blazor PWA - Mobil]
    end

    subgraph Backend
        C[.NET 8 Web API]
        D[Swagger]
    end

    subgraph Extern
        E[Microsoft Entra ID]
        F[Azure SQL Database]
    end

    subgraph Simulator
        G[Vågsimulator]
    end

    B -->|QR-skanning| C
    A -->|HTTP/REST| C
    G -->|POST viktdata| C
    C -->|Auth| E
    C -->|Entity Framework| F
    C --- D
```

## Techstack

- **Backend:** C# / .NET 8 Web API
- **Frontend:** Blazor WebAssembly (PWA)
- **Database:** Azure SQL Database
- **ORM:** Entity Framework Core
- **Auth:** Microsoft Entra ID
- **API-documentation:** Swagger
- **Testing:** xUnit, Postman
- **Versionshantering:** GitHub
- **Simulator:** Konsolapp för att simulera nätverksvågar
