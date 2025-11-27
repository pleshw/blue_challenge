<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useUsersStore } from '@/stores/users'
import type { IUser, ICreateUserRequest, IUpdateUserRequest } from '@/types'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Button from 'primevue/button'
import Dialog from 'primevue/dialog'
import InputText from 'primevue/inputtext'
import Password from 'primevue/password'
import Card from 'primevue/card'
import Message from 'primevue/message'
import ConfirmDialog from 'primevue/confirmdialog'
import { useConfirm } from 'primevue/useconfirm'
import { useToast } from 'primevue/usetoast'

const usersStore = useUsersStore()
const confirm = useConfirm()
const toast = useToast()

const showDialog = ref(false)
const isEditing = ref(false)
const editingUserId = ref<string | null>(null)

const form = ref<ICreateUserRequest>({
  email: '',
  password: '',
})

onMounted(() => {
  usersStore.fetchUsers()
})

function openCreateDialog() {
  isEditing.value = false
  editingUserId.value = null
  form.value = { email: '', password: '' }
  showDialog.value = true
}

function openEditDialog(user: IUser) {
  isEditing.value = true
  editingUserId.value = user.id
  form.value = {
    email: user.credentials.email.address,
    password: '',
  }
  showDialog.value = true
}

async function handleSubmit() {
  try {
    if (isEditing.value && editingUserId.value) {
      const updateRequest: IUpdateUserRequest = {
        email: form.value.email,
        password: form.value.password,
      }
      await usersStore.updateUser(editingUserId.value, updateRequest)
      toast.add({
        severity: 'success',
        summary: 'Sucesso',
        detail: 'Usuário atualizado com sucesso',
        life: 3000,
      })
    } else {
      await usersStore.createUser(form.value)
      toast.add({
        severity: 'success',
        summary: 'Sucesso',
        detail: 'Usuário criado com sucesso',
        life: 3000,
      })
    }
    showDialog.value = false
  } catch {
    toast.add({
      severity: 'error',
      summary: 'Erro',
      detail: usersStore.error || 'Ocorreu um erro',
      life: 5000,
    })
  }
}

function confirmDelete(user: IUser) {
  confirm.require({
    message: `Deseja realmente excluir o usuário ${user.credentials.email.address}?`,
    header: 'Confirmar Exclusão',
    icon: 'pi pi-exclamation-triangle',
    rejectClass: 'p-button-secondary p-button-outlined',
    rejectLabel: 'Cancelar',
    acceptLabel: 'Excluir',
    acceptClass: 'p-button-danger',
    accept: async () => {
      try {
        await usersStore.deleteUser(user.id)
        toast.add({
          severity: 'success',
          summary: 'Sucesso',
          detail: 'Usuário excluído com sucesso',
          life: 3000,
        })
      } catch {
        toast.add({
          severity: 'error',
          summary: 'Erro',
          detail: usersStore.error || 'Erro ao excluir usuário',
          life: 5000,
        })
      }
    },
  })
}
</script>

<template>
  <div class="users-view">
    <Card>
      <template #title>
        <div class="card-header">
          <span>Usuários</span>
          <Button
            label="Novo Usuário"
            icon="pi pi-plus"
            @click="openCreateDialog"
          />
        </div>
      </template>
      <template #content>
        <Message v-if="usersStore.error" severity="error" :closable="true" @close="usersStore.clearError">
          {{ usersStore.error }}
        </Message>

        <DataTable
          :value="usersStore.users"
          :loading="usersStore.loading"
          paginator
          :rows="10"
          :rowsPerPageOptions="[5, 10, 20, 50]"
          stripedRows
          showGridlines
          tableStyle="min-width: 50rem"
        >
          <template #empty>
            <div class="empty-message">
              <i class="pi pi-users"></i>
              <p>Nenhum usuário encontrado</p>
            </div>
          </template>

          <Column field="id" header="ID" sortable style="width: 25%">
            <template #body="{ data }">
              <span class="id-cell">{{ data.id.substring(0, 8) }}...</span>
            </template>
          </Column>

          <Column field="credentials.email.address" header="Email" sortable style="width: 45%">
            <template #body="{ data }">
              {{ data.credentials.email.address }}
            </template>
          </Column>

          <Column header="Ações" style="width: 30%">
            <template #body="{ data }">
              <div class="action-buttons">
                <Button
                  icon="pi pi-pencil"
                  severity="info"
                  text
                  rounded
                  @click="openEditDialog(data)"
                  v-tooltip.top="'Editar'"
                />
                <Button
                  icon="pi pi-trash"
                  severity="danger"
                  text
                  rounded
                  @click="confirmDelete(data)"
                  v-tooltip.top="'Excluir'"
                />
              </div>
            </template>
          </Column>
        </DataTable>
      </template>
    </Card>

    <Dialog
      v-model:visible="showDialog"
      :header="isEditing ? 'Editar Usuário' : 'Novo Usuário'"
      :style="{ width: '450px' }"
      modal
      :closable="!usersStore.loading"
    >
      <form @submit.prevent="handleSubmit" class="dialog-form">
        <div class="field">
          <label for="email">Email</label>
          <InputText
            id="email"
            v-model="form.email"
            type="email"
            placeholder="Digite o email"
            class="w-full"
            :disabled="usersStore.loading"
            required
          />
        </div>

        <div class="field">
          <label for="password">Senha</label>
          <Password
            id="password"
            v-model="form.password"
            :placeholder="isEditing ? 'Nova senha (deixe em branco para manter)' : 'Digite a senha'"
            class="w-full"
            :feedback="!isEditing"
            :disabled="usersStore.loading"
            toggleMask
            :required="!isEditing"
          />
        </div>

        <div class="dialog-actions">
          <Button
            type="button"
            label="Cancelar"
            severity="secondary"
            @click="showDialog = false"
            :disabled="usersStore.loading"
          />
          <Button
            type="submit"
            :label="isEditing ? 'Salvar' : 'Criar'"
            :loading="usersStore.loading"
          />
        </div>
      </form>
    </Dialog>

    <ConfirmDialog />
  </div>
</template>

<style scoped>
.users-view {
  padding: 1rem;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.empty-message {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
  padding: 2rem;
  color: var(--p-text-muted-color);
}

.empty-message i {
  font-size: 2rem;
}

.id-cell {
  font-family: monospace;
  font-size: 0.875rem;
}

.action-buttons {
  display: flex;
  gap: 0.25rem;
}

.dialog-form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
  padding-top: 1rem;
}

.field {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.field label {
  font-weight: 500;
}

.dialog-actions {
  display: flex;
  justify-content: flex-end;
  gap: 0.5rem;
  padding-top: 1rem;
}
</style>
