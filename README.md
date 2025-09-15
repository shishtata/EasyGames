# EasyGames

EasyGames is a student project built for an online store using ASP.NET Core MVC. The online store sells books, games, and toys. The store app uses authentication, role-based authorization, CRUD operations, and session-based cart management.

# Features
- User accounts: Register, login, change password.
- Roles: Admin (manage users, stock, subscribers) and User (browse catalog, add to cart, checkout).
- Stock management: Admin can create, edit, delete, and view stock items by category.
- Catalog: Users can filter items by category or keyword, and add items to their cart.
- Shopping cart**: Add, update, remove, checkout items. Quantity is capped to stock availability.
- Subscribers**: Visitors can subscribe with their email. Admin can view total subscribers.
- Dashboard**: Admin sees total users, stock, categories, subscribers, plus low-stock alerts.

---

## Installation
1. Clone or download this project.
2. Open solution in Visual Studio 2022 (or later).
3. Update the connection string in **appsettings.json**:
   ```json
   "ConnectionStrings": {
     "Default": "Server=YOUR_SERVER;Database=EasyGamesDB;Trusted_Connection=True;TrustServerCertificate=True;"
   }
