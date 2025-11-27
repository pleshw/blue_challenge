import { describe, it, expect, vi, beforeEach } from 'vitest'
import { authService } from '@/services/auth.service'
import { api } from '@/services/api'

vi.mock('@/services/api', () => ({
  api: {
    post: vi.fn(),
  },
}))

describe('Auth Service', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    localStorage.clear()
  })

  it('should login and store token', async () => {
    const mockResponse = {
      accessToken: 'test-token',
      expiresAtUtc: '2025-12-31T23:59:59Z',
    }
    vi.mocked(api.post).mockResolvedValue(mockResponse)

    const result = await authService.login({ email: 'test@test.com', password: 'password' })

    expect(api.post).toHaveBeenCalledWith('/auth/login', { email: 'test@test.com', password: 'password' })
    expect(result).toEqual(mockResponse)
    expect(localStorage.getItem('accessToken')).toBe('test-token')
    expect(localStorage.getItem('tokenExpiry')).toBe('2025-12-31T23:59:59Z')
  })

  it('should logout and clear token', () => {
    localStorage.setItem('accessToken', 'test-token')
    localStorage.setItem('tokenExpiry', '2025-12-31T23:59:59Z')

    authService.logout()

    expect(localStorage.getItem('accessToken')).toBeNull()
    expect(localStorage.getItem('tokenExpiry')).toBeNull()
  })

  it('should return true if authenticated with valid token', () => {
    localStorage.setItem('accessToken', 'test-token')
    localStorage.setItem('tokenExpiry', '2099-12-31T23:59:59Z')

    expect(authService.isAuthenticated()).toBe(true)
  })

  it('should return false if no token', () => {
    expect(authService.isAuthenticated()).toBe(false)
  })

  it('should return false if token expired', () => {
    localStorage.setItem('accessToken', 'test-token')
    localStorage.setItem('tokenExpiry', '2020-01-01T00:00:00Z')

    expect(authService.isAuthenticated()).toBe(false)
  })

  it('should get token', () => {
    localStorage.setItem('accessToken', 'test-token')

    expect(authService.getToken()).toBe('test-token')
  })

  it('should return null if no token', () => {
    expect(authService.getToken()).toBeNull()
  })
})
