# InvenTrack Frontend

React + Vite + Tailwind CSS frontend for the supplied InvenTrack ASP.NET Core API.

## Stack

- React 19
- Vite
- React Router
- Axios
- Tailwind CSS
- Lucide icons

## Run it with the supplied backend

The backend in the provided project exposes the API at:

- `http://localhost:5165/api`
- HTTPS: `https://localhost:7019/api`

The backend CORS policy currently allows `http://localhost:3000`, so this frontend intentionally runs on **port 3000**.

### 1. Start PostgreSQL + API

Open the supplied .NET solution and run the API. Make sure PostgreSQL is running and the API can reach it.

### 2. Install frontend dependencies

```bash
cd InvenTrack-Frontend
npm install
```

### 3. Configure API URL

Copy `.env.example` to `.env` if you want to override the API URL:

```env
VITE_API_BASE_URL=http://localhost:5165/api
VITE_LOW_STOCK_THRESHOLD=10
```

The default already points to the supplied backend.

### 4. Start React

```bash
npm run dev
```

Open:

`http://localhost:3000`

## Authentication

- Register creates a **Staff** account according to the supplied backend.
- Admin-only screens are shown when the API returns role `1` / `Admin`.
- Access and refresh tokens are persisted in local storage.
- On an expired access token, Axios attempts `/Auth/refresh` once and retries the original request.
- Signing out revokes the refresh token through the API.

## Implemented screens

- Login
- Staff registration
- Operations dashboard
- Products list
  - Search by name/SKU
  - Supplier filter
  - Low-stock filter
  - Product detail
  - Admin create/edit/delete
- Suppliers
  - Admin create/edit
- Create order
  - Product picker
  - Quantity controls
  - Customer details
  - Running total
  - API submission
- Order confirmation/detail
- Admin users list
- Responsive sidebar/mobile navigation

## Backend contract used

This frontend matches the controllers and DTOs in the uploaded backend:

- `/api/Auth/login`
- `/api/Auth/register`
- `/api/Auth/refresh`
- `/api/Auth/logout`
- `/api/Product`
- `/api/Supplier`
- `/api/Order`
- `/api/User`

## Important backend note

The supplied `SupplierController` does not expose a DELETE endpoint, so the UI intentionally provides create/edit but not supplier delete.

The supplied `OrderController` exposes create and get-by-id, not an orders list endpoint, so the frontend does not invent an orders history API.

## Build

```bash
npm run build
```

The production output is generated in `dist/`.

## Suggested next backend improvements

For a more complete production workflow, consider adding:

1. `GET /api/Order` for paginated order history.
2. `DELETE /api/Supplier/{id}` if supplier deletion is required.
3. A current-user endpoint such as `GET /api/Auth/me`.
4. Consistent 401/403 response bodies.
5. Server-side pagination for large product/user lists.
