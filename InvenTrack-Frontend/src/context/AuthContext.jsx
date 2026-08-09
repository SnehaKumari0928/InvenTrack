import { createContext, useContext, useEffect, useMemo, useState } from "react";
import { authApi, configureAuth, getErrorMessage } from "../services/api";

const AuthContext = createContext(null);
const ACCESS = "inventrack_access_token";
const REFRESH = "inventrack_refresh_token";
const USER = "inventrack_user";

export function AuthProvider({ children }) {
  const [accessToken, setAccessToken] = useState(() => localStorage.getItem(ACCESS));
  const [refreshToken, setRefreshToken] = useState(() => localStorage.getItem(REFRESH));
  const [user, setUser] = useState(() => {
    try { return JSON.parse(localStorage.getItem(USER) || "null"); } catch { return null; }
  });

  const persist = (data) => {
    setAccessToken(data.accessToken);
    setRefreshToken(data.refreshToken);
    setUser(data.user);
    localStorage.setItem(ACCESS, data.accessToken);
    localStorage.setItem(REFRESH, data.refreshToken);
    localStorage.setItem(USER, JSON.stringify(data.user));
  };

  const clear = () => {
    setAccessToken(null); setRefreshToken(null); setUser(null);
    localStorage.removeItem(ACCESS);
    localStorage.removeItem(REFRESH);
    localStorage.removeItem(USER);
  };

  const login = async (payload) => {
    const { data } = await authApi.login(payload);
    persist(data);
    return data;
  };

  const register = async (payload) => {
    const { data } = await authApi.register(payload);
    persist(data);
    return data;
  };

  const refresh = async () => {
    const token = localStorage.getItem(REFRESH);
    if (!token) return null;
    try {
      const { data } = await authApi.refresh(token);
      persist(data);
      return data.accessToken;
    } catch {
      clear();
      return null;
    }
  };

  const logout = async () => {
    const token = refreshToken;
    clear();
    if (token) {
      try { await authApi.logout(token); } catch {}
    }
  };

  useEffect(() => {
    configureAuth({
      getAccessToken: () => localStorage.getItem(ACCESS),
      refresh,
      onLogout: clear
    });
  }, []);

  const value = useMemo(() => ({
    accessToken, refreshToken, user, login, register, refresh, logout,
    isAuthenticated: Boolean(accessToken && user),
    isAdmin: user?.role === 1 || user?.role === "Admin",
    isStaff: user?.role === 2 || user?.role === "Staff",
    errorText: getErrorMessage
  }), [accessToken, refreshToken, user]);

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export const useAuth = () => useContext(AuthContext);