export interface AuthResponse {
  token: string;
  email: string;
  name: string;
}

export interface Category {
  id: string;
  name: string;
  monthlyBudget: number | null;
  hasExpenses: boolean;
}

export interface Expense {
  id: string;
  categoryId: string;
  categoryName: string;
  amount: number;
  description: string | null;
  date: string;
}

export interface CategorySummary {
  categoryId: string;
  categoryName: string;
  monthlyBudget: number | null;
  spent: number;
  remaining: number | null;
}

export interface DashboardSummary {
  month: number;
  year: number;
  totalSpent: number;
  categories: CategorySummary[];
}
