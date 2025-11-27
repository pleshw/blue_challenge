import { defineStore } from 'pinia'
import { ref } from 'vue'
import { usersService } from '@/services/users.service'
import type { IUser, ICreateUserRequest, IUpdateUserRequest } from '@/types'

export const useUsersStore = defineStore('users', () => {
  const users = ref<IUser[]>([])
  const selectedUser = ref<IUser | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function fetchUsers() {
    loading.value = true
    error.value = null

    try {
      users.value = await usersService.getAll()
    } catch (e) {
      error.value = e instanceof Error ? e.message : 'Failed to fetch users'
    } finally {
      loading.value = false
    }
  }

  async function fetchUserById(id: string) {
    loading.value = true
    error.value = null

    try {
      selectedUser.value = await usersService.getById(id)
    } catch (e) {
      error.value = e instanceof Error ? e.message : 'Failed to fetch user'
    } finally {
      loading.value = false
    }
  }

  async function createUser(request: ICreateUserRequest) {
    loading.value = true
    error.value = null

    try {
      const newUser = await usersService.create(request)
      users.value.push(newUser)
      return newUser
    } catch (e) {
      error.value = e instanceof Error ? e.message : 'Failed to create user'
      throw e
    } finally {
      loading.value = false
    }
  }

  async function updateUser(id: string, request: IUpdateUserRequest) {
    loading.value = true
    error.value = null

    try {
      const updatedUser = await usersService.update(id, request)
      const index = users.value.findIndex((u) => u.id === id)
      if (index !== -1) {
        users.value[index] = updatedUser
      }
      return updatedUser
    } catch (e) {
      error.value = e instanceof Error ? e.message : 'Failed to update user'
      throw e
    } finally {
      loading.value = false
    }
  }

  async function deleteUser(id: string) {
    loading.value = true
    error.value = null

    try {
      await usersService.delete(id)
      users.value = users.value.filter((u) => u.id !== id)
    } catch (e) {
      error.value = e instanceof Error ? e.message : 'Failed to delete user'
      throw e
    } finally {
      loading.value = false
    }
  }

  function clearError() {
    error.value = null
  }

  return {
    users,
    selectedUser,
    loading,
    error,
    fetchUsers,
    fetchUserById,
    createUser,
    updateUser,
    deleteUser,
    clearError,
  }
})
