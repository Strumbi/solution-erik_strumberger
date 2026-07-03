# BackendAkademija/erik_strumberger

API middleware koji dohvaća proizvode iz vanjskih izvora (trenutno DummyJSON REST API)
s naprednim mogućnostima filtriranja, pretrage i autentifikacije.

---

## Tehnološki stack

- **.NET 8** – ASP.NET Web API
- **MediatR** – CQRS pattern (Commands/Queries)
- **FluentValidation** – validacija inputa
- **Entity Framework Core** – perzistencija refresh tokena
- **SQL Server** – baza podataka (via Docker)
- **Serilog** – strukturirano logiranje (Console + File)
- **JWT Bearer** – autentifikacija i autorizacija

---

## Arhitektura

Projekt koristi **Clean Architecture** s CQRS pattern-om:

```
src/
├── BackendAkademija.Domain/             # Entiteti (Product, Category)
├── BackendAkademija.Application/        # CQRS Queries, DTOs, interfejsi, validacija
├── BackendAkademija.Infrastructure/     # DummyJSON klijent, JWT, EF, caching
├── BackendAkademija.api/                # Controllers, middleware, DI kompozicija
├── BackendAkademija.UnitTests/          # Unit testovi (handleri, validatori, helperi)
└── BackendAkademija.IntegrationTests/   # Integracijski testovi (WebApplicationFactory + Testcontainers)
```

Ovisnosti idu samo prema unutra — Infrastructure i WebApi ovise o Application,
Application ovisi o Domain. Ni jedan unutarnji sloj ne zna za vanjski.

---

## Preduvjeti

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

---

## Pokretanje

Postoje dva načina pokretanja: kompletno kroz Docker (preporučeno, najmanje koraka)
ili lokalno kroz `dotnet` uz SQL Server u kontejneru.

### 1. Kloniraj repozitorij

```bash
git clone https://github.com/Strumbi/solution-erik_strumberger.git
cd BackendAkademija
```

### Opcija A — cijela aplikacija kroz Docker

`docker-compose.yml` diže i SQL Server i API u kontejnerima, sve odjednom:

```bash
docker-compose up -d --build
```

Provjeri da su oba containera gore i da je `sqlserver` `healthy`:

```bash
docker-compose ps
```

Migracije se pri startu aplikacije primjenjuju automatski (`db.Database.Migrate()`
u `Program.cs`), pa nije potreban zaseban korak.

Aplikacija je dostupna na `localhost:8080`, Swagger UI na
`localhost:8080/swagger`.

> **Napomena**: `ASPNETCORE_ENVIRONMENT` je u `docker-compose.yml` postavljen na
> `Development` upravo zato da Swagger middleware (koji se u ovom projektu
> registrira samo za Development environment) bude dostupan i unutar kontejnera.
> Za pravu produkcijsku postavu, Swagger bi trebalo ili eksplicitno omogućiti
> neovisno o environmentu ili postaviti iza autentifikacije.

Za rebuild nakon promjena u kodu (Docker zna zadržati stari cache-irani layer):

```bash
docker-compose up -d --build
```

### Opcija B — lokalno pokretanje (samo SQL Server u Dockeru)

```bash
docker-compose up -d sqlserver
```

Pričekaj da container bude `healthy`:

```bash
docker-compose ps
```

Pokreni migracije:

```bash
dotnet ef database update \
    --project BackendAkademija.Infrastructure \
    --startup-project BackendAkademija.api
```

Pokreni aplikaciju:

```bash
dotnet watch run --project BackendAkademija.api
```

Aplikacija je dostupna na `localhost:5162`, Swagger UI na
`localhost:5162/swagger`.

---

## Autentifikacija

API koristi JWT Bearer autentifikaciju. Za testiranje koristi
[DummyJSON test korisnike](https://dummyjson.com/users).

### Login

```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "emilys",
  "password": "emilyspass"
}
```

Response:

```json
{
  "accessToken": "eyJ...",
  "refreshToken": "abc123..."
}
```

### Korištenje tokena

Dodaj header na sve zaštićene endpointe:

```
Authorization: Bearer <accessToken>
```

U Swagger UI-u klikni **Authorize** gumb i unesi token.

### Refresh token

Access token istječe nakon 60 minuta. Koristi refresh token za novi par tokena
bez ponovnog logina. Sesija u potpunosti istječe nakon 7 dana od originalnog logina
(absolute expiry — refresh ne produžuje sesiju).

```http
POST /api/auth/refresh
Content-Type: application/json

{
  "refreshToken": "abc123..."
}
```

Response:

```json
{
  "accessToken": "eyJ...",
  "refreshToken": "xyz789..."
}
```

> **Napomena**: Svaki refresh token može se iskoristiti samo jednom (token rotation).
> Stari refresh token postaje nevažeći čim se izda novi par.

---

## Endpointi

### Auth

| Metoda | URL                  | Opis                                  | Auth |
|--------|----------------------|---------------------------------------|------|
| POST   | `/api/auth/login`   | Login, vraća access + refresh token   | ❌   |
| POST   | `/api/auth/refresh` | Izdaje novi par tokena                | ❌   |

### Products

| Metoda | URL                       | Opis                                              | Auth |
|--------|---------------------------|---------------------------------------------------|------|
| GET    | `/api/products`           | Lista proizvoda (naziv, slika, cijena, kratak opis) | ✅  |
| GET    | `/api/products/{id}`      | Detalji jednog proizvoda                          | ✅   |
| GET    | `/api/products/filter`    | Filtriranje po kategoriji i/ili cijeni            | ✅   |
| GET    | `/api/products/search`    | Pretraga po nazivu                                | ✅   |

### Query parametri

**Filter** – `/api/products/filter`:

```
?category=smartphones&minPrice=100&maxPrice=500
```

Svi parametri su opcionalni. Validacija: `minPrice` i `maxPrice` moraju biti >= 0,
a `minPrice` mora biti <= `maxPrice`.

**Search** – `/api/products/search`:

```
?searchTerm=phone
```

---

## Caching

Ponavljajući pozivi s istim parametrima vraćaju cached rezultat (in-memory):

| Endpoint                    | TTL        |
|-----------------------------|------------|
| `GET /api/products/{id}`    | 10 minuta  |
| `GET /api/products/search`  | 5 minuta   |
| `GET /api/products/filter`  | 5 minuta   |

---

## Testiranje

Projekt sadrži dva testna projekta:

- **BackendAkademija.UnitTests** – testira handlere, validatore i helper klase izolirano
- **BackendAkademija.IntegrationTests** – testira API end-to-end kroz `WebApplicationFactory`,
  uz pravi SQL Server podignut putem **Testcontainers** (potreban je pokrenut Docker)

Pokretanje svih testova:

```bash
dotnet test
```

Pokretanje samo jednog projekta:

```bash
dotnet test BackendAkademija.UnitTests
dotnet test BackendAkademija.IntegrationTests
```

> **Napomena**: Integracijski testovi ovise o dostupnosti Docker daemona (za Testcontainers)
> i o vanjskom DummyJSON API-ju (login/refresh testovi), pa mogu povremeno failati ako
> Docker nije pokrenut ili je DummyJSON nedostupan.

---

## Logiranje

Logovi se zapisuju u konzolu i u `Logs/` folder s dnevnom rotacijom:

```
Logs/log-20260701.txt
```

Razine logiranja:
- `Information` – normalni flow (dohvat proizvoda, cache hit/miss)
- `Warning` – validacijske greške, neuspješni pokušaji autentifikacije
- `Error` – neočekivane greške

---

## Napomene o arhitekturalnim odlukama

**Zašto nema DbContext za proizvode?**
Trenutni izvor podataka je vanjski REST API (DummyJSON). `IProductsSource` interfejs
je dizajniran tako da se implementacija može zamijeniti (EF, file system, RSS) bez
ikakvih izmjena u Application sloju — samo se doda nova Infrastructure implementacija.

**Zašto vlastiti JWT umjesto DummyJSON tokena?**
DummyJSON token je potpisan njihovim ključem koji mi ne znamo i ne možemo validirati(može se validirati kroz auth/me na način da prosljedimo token i vidimo je li se vratio ispravan korisnik,
ali ne kroz JwtBearer).
Middleware izdaje vlastiti JWT jer je autentifikacija naša odgovornost, neovisno o
izvoru podataka. DummyJSON služi samo kao "identity verifier" — provjerava jesu li
kredencijali ispravni, a mi izdajemo vlastiti token.

**Refresh token perzistencija**
Refresh tokeni se čuvaju u SQL Serveru (EF Core) kako bi preživjeli restart aplikacije
i podržali horizontal scaling.

**Caching**
Implementiran je in-memory cache za read-heavy endpointe putem MediatR Pipeline
Behavior — caching logika je centralizirana na jednom mjestu, a ne razbacana po
handlerima. U produkcijskom okruženju zamijenio bi se s Redis-om za podršku
horizontal scalinga.

---

## Korištenje AI-ja

AI alati (Claude) korišteni su tijekom razvoja za:

- automatizaciju unit i integracijskih testova
- debugiranje
- dodavanje `AddSwaggerGen` i `JwtBearer` unutar dependency injection skripte u api sloju clean arhitekture
- pisanje ovog README file-a (uz priloženi cijeli projekt i opis što je obvezno spomenuti u njemu)
- generiranje Serilog postavki unutar `appsettings.json`
- generiranje docker-compose file-a na temelju zadanih uputa (korištenje određenog image-a, prilagodba za rad na macu zbog ARM arhitekture, volume mounting radi perzistencije podataka nakon brisanja containera, implementacija healthcheck-a)