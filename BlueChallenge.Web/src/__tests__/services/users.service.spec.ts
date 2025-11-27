import { describe, it, expect, vi, beforeEach } from 'vitest'
import { usersService } from '@/services/users.service'
import { api } from '@/services/api'
import type { IUser } from '@/types'

vi.mock('@/services/api', () => ({
  api: {
    get: vi.fn(),
    post: vi.fn(),
    put: vi.fn(),
    delete: vi.fn(),
  },
}))

const mockUser: IUser = {
  id: '123',
  credentials: { email: { alias: 'test', provider: 'test.com', address: 'test@test.com' }, password: 'hash' },
}

describe('Users Service', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('should get all users', async () => {
    vi.mocked(api.get).mockResolvedValue([mockUser])

    const result = await usersService.getAll()

    expect(api.get).toHaveBeenCalledWith('/users')
    expect(result).toEqual([mockUser])
  })

  it('should get user by id', async () => {
    vi.mocked(api.get).mockResolvedValue(mockUser)

    const result = await usersService.getById('123')

    expect(api.get).toHaveBeenCalledWith('/users/123')
    expect(result).toEqual(mockUser)
  })

  it('should create user', async () => {
    vi.mocked(api.post).mockResolvedValue(mockUser)

    const result = await usersService.create({ email: 'test@test.com', password: 'password' })

    expect(api.post).toHaveBeenCalledWith('/users', { email: 'test@test.com', password: 'password' })
    expect(result).toEqual(mockUser)
  })

  it('should update user', async () => {
    vi.mocked(api.put).mockResolvedValue(mockUser)

    const result = await usersService.update('123', { email: 'test@test.com', password: 'password' })

    expect(api.put).toHaveBeenCalledWith('/users/123', { email: 'test@test.com', password: 'password' })
    expect(result).toEqual(mockUser)
  })

  it('should delete user', async () => {
    vi.mocked(api.delete).mockResolvedValue(undefined)

    await usersService.delete('123')

    expect(api.delete).toHaveBeenCalledWith('/users/123')
  })
})
