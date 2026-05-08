import { create } from 'zustand'
import type { AuthResponse } from '../types/Types'
 
interface AuthState {
  token: string | null
  username: string | null
  displayName: string | null
  role: 'Inspetor' | 'Coordenador' | null
  isAuthenticated: boolean
  login: (data: AuthResponse) => void
  logout: () => void
}
 
export const useAuthStore = create<AuthState>((set) => ({
  token: localStorage.getItem('token'),
  username: localStorage.getItem('username'),
  displayName: localStorage.getItem('displayName'),
  role: localStorage.getItem('role') as 'Inspetor' | 'Coordenador' | null,
  isAuthenticated: !!localStorage.getItem('token'),
 
  login: (data) => {
    localStorage.setItem('token',       data.token)
    localStorage.setItem('username',    data.username)
    localStorage.setItem('displayName', data.displayName)
    localStorage.setItem('role',        data.role)
    set({
      token: data.token,
      username: data.username,
      displayName: data.displayName,
      role: data.role,
      isAuthenticated: true,
    })
  },
 
  logout: () => {
    localStorage.clear()
    set({ token: null, username: null, displayName: null, role: null, isAuthenticated: false })
  },
}))
