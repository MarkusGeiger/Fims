# Flexible Inventory Management System (FIMS)

This will be an application to track and manage inventory items using the data points that are sent from different sources into different databases.

## The Plan
The main UI application is a ASP .NET Core Web API with a vite based react frontend.

### Main Features:
* User Management
  * Authentication with indivdual user accounts
  * Authentication using OAuth
  * User management
* Inventory Management
  * Agents gather data and send it to the API
  * Inventory items are hierachically organized and displayed
  * The structure can be customized anytime
* Observability
  * Full Opentelemetry integration
