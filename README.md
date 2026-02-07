# PetMinder

[![Live Demo](https://img.shields.io/badge/Live_Demo-petminder.org-2ea44f?style=for-the-badge&logo=microsoftedge)](https://petminder.org)
[![Documentation](https://img.shields.io/badge/Read_Documentation-PDF-CC2927?style=for-the-badge&logo=adobeacrobatreader)](./PetMinderDocumentation.pdf)

> **A community-driven, circular economy platform for pet care services.**  
> *Eliminating financial barriers through mutual aid and virtual currency.*

---

## Getting Started

> [!IMPORTANT]  
> **Account Verification Required:** To access the application's features, you must verify your account after signing up. Please **register with a valid email address** that you can access to receive your verification link.

---

## Project Documentation

**Detailed technical documentation is available in this repository.**  
This includes architectural diagrams, database schemas, and engineering decisions.

**[Click here to read the full PetMinderDocumentation.pdf](./PetMinderDocumentation.pdf)**

---

## Key Features

*   **Hybrid Cloud Architecture:** Cost-efficient strategy using **Azure Static Web Apps** (Client) and **Render** (API), with Azure Blob Storage for stateless media handling.
*   **High-Performance Database:** Optimized **PostgreSQL** strategy with Covering & Directional B-Tree indexes.
*   **Virtual Economy:** ACID-compliant "Points Ledger" with an automated currency expiration engine powered by **Hangfire**.
*   **Real-Time Negotiation:** Built with **SignalR** to allow instant booking proposals and counter-offers.
*   **Geo-Spatial Discovery:** Proximity-based search engine using **Nominatim API** for local results.

## Tech Stack

**Backend**
*   .NET 8 (ASP.NET Core Web API)
*   Entity Framework Core 9 (Code-First)
*   SignalR (WebSockets)
*   Hangfire (Background Jobs)

**Frontend**
*   Blazor WebAssembly (WASM)
*   Tailwind CSS
*   MudBlazor (UI Components)

**Data & Infrastructure**
*   PostgreSQL (Neon Serverless)
*   Azure Static Web Apps & Blob Storage
*   Render (Containerized API Hosting)
*   Docker

---

### Contact
**Yaroslav Khanin**  
[LinkedIn](https://www.linkedin.com/in/yaroslav-khanin-712202305/) | [Email](mailto:khaninyaroslav28@gmail.com)
