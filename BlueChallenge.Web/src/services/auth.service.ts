import { api } from './api'
import type { ILoginRequest, ILoginResponse } from '@/types'

export const authService = {
  async login(credentials: ILoginRequest): Promise<ILoginResponse> {
    const response = await api.post<ILoginResponse>('/auth/login', credentials)
    localStorage.setItem('accessToken', response.accessToken)
    localStorage.setItem('tokenExpiry', response.expiresAtUtc)
    return response
  },

  logout(): void {
    localStorage.removeItem('accessToken')
    localStorage.removeItem('tokenExpiry')
  },

  isAuthenticated(): boolean {
    const token = localStorage.getItem('accessToken')
    const expiry = localStorage.getItem('tokenExpiry')

    if (!token || !expiry) {
      return false
    }

    return new Date(expiry) > new Date()
  },

  getToken(): string | null {
    return localStorage.getItem('accessToken')
  },
}
