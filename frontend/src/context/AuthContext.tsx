import { createContext, useCallback, useContext, useState, type ReactNode } from "react";
import { authApi } from "../api/client";
import type { AuthResponse } from "../api/types";

interface AuthState {
  token: string | null;
  email: string | null;
  name: string | null;
}

interface AuthContextValue extends AuthState {
  login: (email: string, password: string) => Promise<void>;
  register: (email: string, password: string, name: string) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

function readStoredAuth(): AuthState {
  return {
    token: localStorage.getItem("token"),
    email: localStorage.getItem("email"),
    name: localStorage.getItem("name"),
  };
}

function persistAuth(response: AuthResponse) {
  localStorage.setItem("token", response.token);
  localStorage.setItem("email", response.email);
  localStorage.setItem("name", response.name);
}

function clearAuth() {
  localStorage.removeItem("token");
  localStorage.removeItem("email");
  localStorage.removeItem("name");
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [state, setState] = useState<AuthState>(readStoredAuth);

  const login = useCallback(async (email: string, password: string) => {
    const response = await authApi.login(email, password);
    persistAuth(response);
    setState({ token: response.token, email: response.email, name: response.name });
  }, []);

  const register = useCallback(async (email: string, password: string, name: string) => {
    const response = await authApi.register(email, password, name);
    persistAuth(response);
    setState({ token: response.token, email: response.email, name: response.name });
  }, []);

  const logout = useCallback(() => {
    clearAuth();
    setState({ token: null, email: null, name: null });
  }, []);

  return (
    <AuthContext.Provider value={{ ...state, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth debe usarse dentro de AuthProvider");
  return ctx;
}
