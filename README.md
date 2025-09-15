# EasyGames

EasyGames is a student project built for an online store using ASP.NET Core MVC. The online store sells books, games, and toys. The store app uses authentication, role-based authorization, CRUD operations, and session-based cart management.

## Features
- User accounts: 
    Users can view the app and Catalog items without Login. They can Regsiter and Login along with changing Passwords.
    Once logged in, they can easily add items on cart based on Stock Availability.
    Once the items are on Cart, they can update the number of items or remove the items.
    If they wish to checkout, they can click Chceckout and the order will be placed successfully.
    
- Roles: 
    There are two roles in the app: Admin (who can manage users, stock, subscribers) and User (who can browse catalog, add to cart, checkout).

- Stock management: 
    Admin can create, edit, delete, and view stock items by category. Once User places an order, stock quantity is updated. User can not place any order above the stock availability number.

- Catalog: 
    Users can filter items by category or keyword, and add items to their cart. Users can view item even without logging in.

- Shopping cart: 
    Cart can be used for adding, updating, removing and checking out the items by User. Quantity is capped to stock availability.

- Subscribers: 
    Visitors can subscribe with their email to get latest offers and information from the Owner. Admin can view total subscribers, and use the emails for promotions.

- Dashboard: 
    Admin can see the total users, stock, categories, subscribers on Admin Dashboard. They can also see low-stock alerts, and order items based on early reminders.


## Installation
1. Clone or download this project.
2. Open solution in Visual Studio 2022 (or later).
3. Update the connection string in **appsettings.json**:
   ```json
   "ConnectionStrings": {
     "Default": "Server=YOUR_SERVER;Database=EasyGamesDB;Trusted_Connection=True;TrustServerCertificate=True;"
   }
4. The project uses Entity Framework Core with SQL Server. To create the database and tables, open the Package Manager Console and run:
   ```
   Update-Database
   ```
5. Run the application (F5 or Ctrl+F5).
6. The application will open in your default web browser. You can register a new user or log in with the following admin credentials:
   - Username: Admin@easygames.com
   - Password: Admin@123
7. Explore the features of the online store!