import axios from "axios";

const baseURL = import.meta.env.VITE_API_BASE_URL || "https://localhost:7019/api";

export const api = axios.create({
  baseURL,
  headers: { "Content-Type": "application/json" }
});

let authHandlers = {
  getAccessToken: () => localStorage.getItem("inventrack_access_token"),
  refresh: async () => null,
  onLogout: () => {}
};

export const configureAuth = (handlers) => {
  authHandlers = { ...authHandlers, ...handlers };
};

api.interceptors.request.use((config) => {
  const token = authHandlers.getAccessToken();
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

let refreshing = null;

api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const original = error.config;
    if (error.response?.status !== 401 || original?._retry || original?.url?.includes("/Auth/")) {
      return Promise.reject(error);
    }

    original._retry = true;
    try {
      if (!refreshing) refreshing = authHandlers.refresh();
      const token = await refreshing;
      refreshing = null;
      if (!token) throw error;
      original.headers.Authorization = `Bearer ${token}`;
      return api(original);
    } catch (refreshError) {
      refreshing = null;
      authHandlers.onLogout();
      return Promise.reject(refreshError);
    }
  }
);

export const getErrorMessage = (error) =>
  error?.response?.data?.message ||
  error?.response?.data?.title ||
  (typeof error?.response?.data === "string" ? error.response.data : null) ||
  error?.message ||
  "Something went wrong.";

export const authApi = {
  login: (payload) => api.post("/Auth/login", payload),
  register: (payload) => api.post("/Auth/register", payload),
  refresh: (refreshToken) => api.post("/Auth/refresh", { refreshToken }),
  logout: (refreshToken) => api.post("/Auth/logout", { refreshToken })
};

export const productApi = {
  list: (params) => api.get("/Product", { params }),
  get: (id) => api.get(`/Product/${id}`),
  create: (payload) => api.post("/Product", payload),
  update: (id, payload) => api.put(`/Product/${id}`, payload),
  remove: (id) => api.delete(`/Product/${id}`)
};

export const supplierApi = {
  list: () => api.get("/Supplier"),
  create: (payload) => api.post("/Supplier", payload),
  update: (id, payload) => api.put(`/Supplier/${id}`, payload)
};

export const orderApi = {
  create: (payload) => api.post("/Order", payload),
  get: (id) => api.get(`/Order/${id}`)
};

export const userApi = {
  list: () => api.get("/User"),
  get: (id) => api.get(`/User/${id}`)
};