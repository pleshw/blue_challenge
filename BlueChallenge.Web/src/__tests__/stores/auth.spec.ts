import { describe, it, expect, vi, beforeEach } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { useAuthStore } from '@/stores/auth'
import { authService } from '@/services/auth.service'

vi.mock('@/services/auth.service', () => ({
  authService: {
    login: vi.fn(),
    logout: vi.fn(),
    isAuthenticated: vi.fn(),
    getToken: vi.fn(),
  },
}))

describe('Auth Store', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  it('should initialize with authentication status from service', () => {
    vi.mocked(authService.isAuthenticated).mockReturnValue(true)
    const store = useAuthStore()
    expect(store.isAuthenticated).toBe(true)
  })

  it('should login successfully', async () => {
    vi.mocked(authService.isAuthenticated).mockReturnValue(false)
    vi.mocked(authService.login).mockResolvedValue({
      accessToken: 'test-token',
      expiresAtUtc: '2025-12-31T23:59:59Z',
    })

    const store = useAuthStore()
    const result = await store.login({ email: 'test@test.com', password: 'password' })

    expect(result).toBe(true)
    expect(store.isAuthenticated).toBe(true)
    expect(store.error).toBeNull()
  })

  it('should handle login failure', async () => {
    vi.mocked(authService.isAuthenticated).mockReturnValue(false)
    vi.mocked(authService.login).mockRejectedValue(new Error('Invalid credentials'))

    const store = useAuthStore()
    const result = await store.login({ email: 'test@test.com', password: 'wrong' })

    expect(result).toBe(false)
    expect(store.isAuthenticated).toBe(false)
    expect(store.error).toBe('Invalid credentials')
  })

  it('should logout', () => {
    vi.mocked(authService.isAuthenticated).mockReturnValue(true)
    const store = useAuthStore()
    store.isAuthenticated = true

    store.logout()

    expect(authService.logout).toHaveBeenCalled()
    expect(store.isAuthenticated).toBe(false)
  })

  it('should check auth status', () => {
    vi.mocked(authService.isAuthenticated).mockReturnValue(true)
    const store = useAuthStore()
    store.isAuthenticated = false

    vi.mocked(authService.isAuthenticated).mockReturnValue(true)
    store.checkAuth()

    expect(store.isAuthenticated).toBe(true)
  })
})
