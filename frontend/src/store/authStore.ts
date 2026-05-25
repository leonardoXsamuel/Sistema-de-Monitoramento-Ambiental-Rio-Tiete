import { create } from 'zustand';
import type { AuthUser } from '../types/types';

interface AuthState {
  user: AuthUser | null;
  token: string | null;
  setAuth: (user: AuthUser, token: string) => void;
  logout: () => void;
}

const storedUser = localStorage.getItem('user');

export const useAuthStore = create<AuthState>((set) => ({
  user: storedUser ? (JSON.parse(storedUser) as AuthUser) : null,
  token: localStorage.getItem('token'),

  setAuth: (user, token) => {
    localStorage.setItem('token', token);
    localStorage.setItem('user', JSON.stringify(user));
    set({ user, token });
  },

  logout: () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    set({ user: null, token: null });
  },
}));
