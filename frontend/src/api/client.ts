import type { AuthResponse, Category, DashboardSummary, Expense } from "./types";

const BASE_URL = "/api";

export class ApiError extends Error {
  status: number;
  constructor(status: number, message: string) {
    super(message);
    this.status = status;
  }
}

async function request<T>(path: string, options: RequestInit = {}): Promise<T> {
  const token = localStorage.getItem("token");
  const headers: Record<string, string> = {
    "Content-Type": "application/json",
    ...(options.headers as Record<string, string> | undefined),
  };
  if (token) {
    headers["Authorization"] = `Bearer ${token}`;
  }

  const res = await fetch(`${BASE_URL}${path}`, { ...options, headers });

  if (res.status === 204) {
    return undefined as T;
  }

  const isJson = res.headers.get("content-type")?.includes("application/json");
  const body = isJson ? await res.json() : undefined;

  if (!res.ok) {
    const message = body?.detail ?? body?.title ?? `Error ${res.status}`;
    throw new ApiError(res.status, message);
  }

  return body as T;
}

export const authApi = {
  register: (email: string, password: string, name: string) =>
    request<AuthResponse>("/auth/register", {
      method: "POST",
      body: JSON.stringify({ email, password, name }),
    }),
  login: (email: string, password: string) =>
    request<AuthResponse>("/auth/login", {
      method: "POST",
      body: JSON.stringify({ email, password }),
    }),
};

export const categoriesApi = {
  getAll: () => request<Category[]>("/categories"),
  create: (data: { name: string; monthlyBudget: number | null }) =>
    request<Category>("/categories", { method: "POST", body: JSON.stringify(data) }),
  remove: (id: string) => request<void>(`/categories/${id}`, { method: "DELETE" }),
};

export const expensesApi = {
  getAll: () => request<Expense[]>("/expenses"),
  create: (data: { categoryId: string; amount: number; description: string | null; date: string }) =>
    request<Expense>("/expenses", { method: "POST", body: JSON.stringify(data) }),
  remove: (id: string) => request<void>(`/expenses/${id}`, { method: "DELETE" }),
};

export const dashboardApi = {
  getSummary: (month: number, year: number) =>
    request<DashboardSummary>(`/dashboard/summary?month=${month}&year=${year}`),
};
