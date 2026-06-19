# AmazonScraper
## Tech Stack & Architecture
* **Framework:** .NET 8.0 (ASP.NET Core Web API)
* **Database:** Postgresql
* **Design Patterns:** Strategy Pattern
* 
# using
1- create database for caching in postresql and adjust the connection string in appsetting with yours

---

## Setup instructions 

### Prerequisites
Ensure you have the following installed locally:
* [.NET 8.0 SDK](https://microsoft.com)
* Postgresql

### Configuration
1. create database for caching in postresql
2. Open the `appsettings.json` file in the Web API project root.
3. Update the default connection string to point to your Postgresql instance and the cache database name:
```json
 "ConnectionStrings": {
   "PostgresCacheConnection": "Host=localhost;Database=product_cache_db;Username=postgres;Password=postgres;"
 }
```

### Running the Project
Launch the server using the .NET CLI command:
```bash
dotnet run
```
The application will default to `https://localhost:7001` or `http://localhost:5001`.

```
#### known limitations
- paging not working in search page.  
- couldn't scrape offers data and so there fixed object in frontend for testing.
- in product details page selected image not appear after loading 

---
