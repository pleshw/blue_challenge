import { describe, it, expect, vi, beforeEach } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { useUsersStore } from '@/stores/users'
import { usersService } from '@/services/users.service'
import type { IUser } from '@/types'

vi.mock('@/services/users.service', () => ({
  usersService: {
    getAll: vi.fn(),
    getById: vi.fn(),
    create: vi.fn(),
    update: vi.fn(),
    delete: vi.fn(),
  },
}))

const mockUser: IUser = {
  id: '123',
  credentials: { email: { alias: 'test', provider: 'test.com', address: 'test@test.com' }, password: 'hash' },
}

const mockUser2: IUser = {
  id: '456',
  credentials: { email: { alias: 'test2', provider: 'test.com', address: 'test2@test.com' }, password: 'hash2' },
}

describe('Users Store', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  it('should fetch users successfully', async () => {
    vi.mocked(usersService.getAll).mockResolvedValue([mockUser, mockUser2])

    const store = useUsersStore()
    await store.fetchUsers()

    expect(store.users).toHaveLength(2)
    expect(store.users[0].id).toBe('123')
    expect(store.loading).toBe(false)
    expect(store.error).toBeNull()
  })

  it('should handle fetch users error', async () => {
    vi.mocked(usersService.getAll).mockRejectedValue(new Error('Network error'))

    const store = useUsersStore()
    await store.fetchUsers()

    expect(store.users).toHaveLength(0)
    expect(store.error).toBe('Network error')
  })

  it('should fetch user by id', async () => {
    vi.mocked(usersService.getById).mockResolvedValue(mockUser)

    const store = useUsersStore()
    await store.fetchUserById('123')

    expect(store.selectedUser).toEqual(mockUser)
  })

  it('should create user', async () => {
    vi.mocked(usersService.create).mockResolvedValue(mockUser)

    const store = useUsersStore()
    const result = await store.createUser({ email: 'test@test.com', password: 'pass' })

    expect(result).toEqual(mockUser)
    expect(store.users).toContainEqual(mockUser)
  })

  it('should update user', async () => {
    const updatedUser = { ...mockUser, credentials: { email: { alias: 'updated', provider: 'test.com', address: 'updated@test.com' }, password: 'hash' } }
    vi.mocked(usersService.update).mockResolvedValue(updatedUser)

    const store = useUsersStore()
    store.users = [mockUser]

    const result = await store.updateUser('123', { email: 'updated@test.com', password: 'pass' })

    expect(result).toEqual(updatedUser)
    expect(store.users[0].credentials.email.address).toBe('updated@test.com')
  })

  it('should delete user', async () => {
    vi.mocked(usersService.delete).mockResolvedValue()

    const store = useUsersStore()
    store.users = [mockUser, mockUser2]

    await store.deleteUser('123')

    expect(store.users).toHaveLength(1)
    expect(store.users[0].id).toBe('456')
  })

  it('should clear error', () => {
    const store = useUsersStore()
    store.error = 'Some error'

    store.clearError()

    expect(store.error).toBeNull()
  })
})
