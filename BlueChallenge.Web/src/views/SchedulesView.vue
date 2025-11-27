<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useSchedulesStore } from '@/stores/schedules'
import { useUsersStore } from '@/stores/users'
import type { ISchedule, ICreateScheduleRequest, IUpdateScheduleRequest } from '@/types'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Button from 'primevue/button'
import Dialog from 'primevue/dialog'
import InputText from 'primevue/inputtext'
import Textarea from 'primevue/textarea'
import DatePicker from 'primevue/datepicker'
import Select from 'primevue/select'
import Checkbox from 'primevue/checkbox'
import Card from 'primevue/card'
import Tag from 'primevue/tag'
import Message from 'primevue/message'
import ConfirmDialog from 'primevue/confirmdialog'
import { useConfirm } from 'primevue/useconfirm'
import { useToast } from 'primevue/usetoast'

const schedulesStore = useSchedulesStore()
const usersStore = useUsersStore()
const confirm = useConfirm()
const toast = useToast()

const showDialog = ref(false)
const isEditing = ref(false)
const editingScheduleId = ref<string | null>(null)

interface FormData {
  startDate: Date | null
  endDate: Date | null
  startTime: Date | null
  endTime: Date | null
  isAllDay: boolean
  description: string
  userId: string
}

const form = ref<FormData>({
  startDate: null,
  endDate: null,
  startTime: null,
  endTime: null,
  isAllDay: false,
  description: '',
  userId: '',
})

const userOptions = computed(() =>
  usersStore.users.map((user) => ({
    label: `${user.credentials.email.alias} (${user.credentials.email.address})`,
    value: user.id,
  }))
)

// Validações de datas e horas
const minEndDate = computed(() => form.value.startDate || undefined)

const isDateRangeValid = computed(() => {
  if (!form.value.startDate || !form.value.endDate) return true
  return form.value.endDate >= form.value.startDate
})

const isHourRangeValid = computed(() => {
  if (form.value.isAllDay) return true
  if (!form.value.startTime || !form.value.endTime) return true
  
  // Se as datas são iguais, validar as horas
  if (form.value.startDate && form.value.endDate) {
    const sameDay = form.value.startDate.toDateString() === form.value.endDate.toDateString()
    if (sameDay) {
      return form.value.endTime >= form.value.startTime
    }
  }
  return true
})

const isFormValid = computed(() => {
  return (
    form.value.userId &&
    form.value.description &&
    form.value.startDate &&
    form.value.endDate &&
    isDateRangeValid.value &&
    isHourRangeValid.value &&
    (form.value.isAllDay || (form.value.startTime && form.value.endTime))
  )
})

const dateValidationError = computed(() => {
  if (!isDateRangeValid.value) {
    return 'A data fim não pode ser anterior à data início'
  }
  return null
})

const hourValidationError = computed(() => {
  if (!isHourRangeValid.value) {
    return 'A hora fim não pode ser anterior à hora início no mesmo dia'
  }
  if (!form.value.isAllDay && (!form.value.startTime || !form.value.endTime)) {
    return 'Informe a hora início e hora fim'
  }
  return null
})

onMounted(() => {
  schedulesStore.fetchSchedules()
  usersStore.fetchUsers()
})

function formatDate(dateString: string): string {
  return new Date(dateString).toLocaleDateString('pt-BR')
}

function formatTime(timeString: string): string {
  const parts = timeString.split(':')
  return `${parts[0]}:${parts[1]}`
}

function openCreateDialog() {
  isEditing.value = false
  editingScheduleId.value = null
  form.value = {
    startDate: null,
    endDate: null,
    startTime: null,
    endTime: null,
    isAllDay: false,
    description: '',
    userId: '',
  }
  showDialog.value = true
}

function openEditDialog(schedule: ISchedule) {
  isEditing.value = true
  editingScheduleId.value = schedule.id
  
  const startDate = new Date(schedule.dateRange.start)
  const endDate = new Date(schedule.dateRange.end)
  
  let startTime: Date | null = null
  let endTime: Date | null = null
  
  if (schedule.hourRange) {
    const [startH, startM] = schedule.hourRange.start.split(':').map(Number)
    const [endH, endM] = schedule.hourRange.end.split(':').map(Number)
    startTime = new Date()
    startTime.setHours(startH ?? 0, startM ?? 0, 0, 0)
    endTime = new Date()
    endTime.setHours(endH ?? 0, endM ?? 0, 0, 0)
  }
  
  form.value = {
    startDate,
    endDate,
    startTime,
    endTime,
    isAllDay: schedule.isAllDay,
    description: schedule.description,
    userId: schedule.user.id,
  }
  showDialog.value = true
}

function buildRequest(): ICreateScheduleRequest | IUpdateScheduleRequest {
  const startDate = form.value.startDate!.toISOString()
  const endDate = form.value.endDate!.toISOString()
  
  let hourRange = undefined
  if (!form.value.isAllDay && form.value.startTime && form.value.endTime) {
    const formatTimeValue = (date: Date) => {
      const h = date.getHours().toString().padStart(2, '0')
      const m = date.getMinutes().toString().padStart(2, '0')
      return `${h}:${m}:00`
    }
    hourRange = {
      start: formatTimeValue(form.value.startTime),
      end: formatTimeValue(form.value.endTime),
    }
  }
  
  return {
    dateRange: { start: startDate, end: endDate },
    isAllDay: form.value.isAllDay,
    hourRange,
    description: form.value.description,
    userId: form.value.userId,
  }
}

async function handleSubmit() {
  try {
    if (isEditing.value && editingScheduleId.value) {
      await schedulesStore.updateSchedule(editingScheduleId.value, buildRequest())
      toast.add({
        severity: 'success',
        summary: 'Sucesso',
        detail: 'Agendamento atualizado com sucesso',
        life: 3000,
      })
    } else {
      await schedulesStore.createSchedule(buildRequest())
      toast.add({
        severity: 'success',
        summary: 'Sucesso',
        detail: 'Agendamento criado com sucesso',
        life: 3000,
      })
    }
    showDialog.value = false
  } catch {
    toast.add({
      severity: 'error',
      summary: 'Erro',
      detail: schedulesStore.error || 'Ocorreu um erro',
      life: 5000,
    })
  }
}

function confirmDelete(schedule: ISchedule) {
  confirm.require({
    message: `Deseja realmente excluir o agendamento "${schedule.description}"?`,
    header: 'Confirmar Exclusão',
    icon: 'pi pi-exclamation-triangle',
    rejectClass: 'p-button-secondary p-button-outlined',
    rejectLabel: 'Cancelar',
    acceptLabel: 'Excluir',
    acceptClass: 'p-button-danger',
    accept: async () => {
      try {
        await schedulesStore.deleteSchedule(schedule.id)
        toast.add({
          severity: 'success',
          summary: 'Sucesso',
          detail: 'Agendamento excluído com sucesso',
          life: 3000,
        })
      } catch {
        toast.add({
          severity: 'error',
          summary: 'Erro',
          detail: schedulesStore.error || 'Erro ao excluir agendamento',
          life: 5000,
        })
      }
    },
  })
}
</script>

<template>
  <div class="schedules-view">
    <Card>
      <template #title>
        <div class="card-header">
          <span>Agendamentos</span>
          <Button
            label="Novo Agendamento"
            icon="pi pi-plus"
            @click="openCreateDialog"
          />
        </div>
      </template>
      <template #content>
        <Message v-if="schedulesStore.error" severity="error" :closable="true" @close="schedulesStore.clearError">
          {{ schedulesStore.error }}
        </Message>

        <DataTable
          :value="schedulesStore.schedules"
          :loading="schedulesStore.loading"
          paginator
          :rows="10"
          :rowsPerPageOptions="[5, 10, 20, 50]"
          stripedRows
          showGridlines
          tableStyle="min-width: 50rem"
        >
          <template #empty>
            <div class="empty-message">
              <i class="pi pi-calendar"></i>
              <p>Nenhum agendamento encontrado</p>
            </div>
          </template>

          <Column field="description" header="Descrição" sortable style="width: 25%">
            <template #body="{ data }">
              <span class="description-cell">{{ data.description }}</span>
            </template>
          </Column>

          <Column header="Período" style="width: 20%">
            <template #body="{ data }">
              <div class="date-range">
                <span>{{ formatDate(data.dateRange.start) }}</span>
                <span>até</span>
                <span>{{ formatDate(data.dateRange.end) }}</span>
              </div>
            </template>
          </Column>

          <Column header="Horário" style="width: 15%">
            <template #body="{ data }">
              <Tag v-if="data.isAllDay" severity="info" value="Dia todo" />
              <span v-else-if="data.hourRange">
                {{ formatTime(data.hourRange.start) }} - {{ formatTime(data.hourRange.end) }}
              </span>
              <span v-else>-</span>
            </template>
          </Column>

          <Column field="user.credentials.email.address" header="Usuário" sortable style="width: 20%">
            <template #body="{ data }">
              {{ data.user.credentials.email.address }}
            </template>
          </Column>

          <Column header="Ações" style="width: 20%">
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
      :header="isEditing ? 'Editar Agendamento' : 'Novo Agendamento'"
      :style="{ width: '550px' }"
      modal
      :closable="!schedulesStore.loading"
    >
      <form @submit.prevent="handleSubmit" class="dialog-form">
        <div class="field">
          <label for="userId">Usuário</label>
          <Select
            id="userId"
            v-model="form.userId"
            :options="userOptions"
            optionLabel="label"
            optionValue="value"
            placeholder="Selecione um usuário"
            class="w-full"
            :disabled="schedulesStore.loading"
            required
          />
        </div>

        <div class="field">
          <label for="description">Descrição</label>
          <Textarea
            id="description"
            v-model="form.description"
            placeholder="Descrição do agendamento"
            class="w-full"
            rows="3"
            :disabled="schedulesStore.loading"
            required
          />
        </div>

        <div class="field-row">
          <div class="field">
            <label for="startDate">Data Início</label>
            <DatePicker
              id="startDate"
              v-model="form.startDate"
              dateFormat="dd/mm/yy"
              placeholder="Selecione"
              class="w-full"
              :disabled="schedulesStore.loading"
              showIcon
              required
            />
          </div>

          <div class="field">
            <label for="endDate">Data Fim</label>
            <DatePicker
              id="endDate"
              v-model="form.endDate"
              dateFormat="dd/mm/yy"
              placeholder="Selecione"
              class="w-full"
              :disabled="schedulesStore.loading"
              :minDate="minEndDate"
              :class="{ 'p-invalid': !isDateRangeValid }"
              showIcon
              required
            />
          </div>
        </div>

        <small v-if="dateValidationError" class="p-error">{{ dateValidationError }}</small>

        <div class="field-checkbox">
          <Checkbox
            id="isAllDay"
            v-model="form.isAllDay"
            :binary="true"
            :disabled="schedulesStore.loading"
          />
          <label for="isAllDay">Dia todo</label>
        </div>

        <div v-if="!form.isAllDay" class="field-row">
          <div class="field">
            <label for="startTime">Hora Início</label>
            <DatePicker
              id="startTime"
              v-model="form.startTime"
              timeOnly
              hourFormat="24"
              placeholder="HH:MM"
              class="w-full"
              :disabled="schedulesStore.loading"
              required
            />
          </div>

          <div class="field">
            <label for="endTime">Hora Fim</label>
            <DatePicker
              id="endTime"
              v-model="form.endTime"
              timeOnly
              hourFormat="24"
              placeholder="HH:MM"
              class="w-full"
              :disabled="schedulesStore.loading"
              :class="{ 'p-invalid': !isHourRangeValid }"
              required
            />
          </div>
        </div>

        <small v-if="hourValidationError" class="p-error">{{ hourValidationError }}</small>

        <div class="dialog-actions">
          <Button
            type="button"
            label="Cancelar"
            severity="secondary"
            @click="showDialog = false"
            :disabled="schedulesStore.loading"
          />
          <Button
            type="submit"
            :label="isEditing ? 'Salvar' : 'Criar'"
            :loading="schedulesStore.loading"
            :disabled="!isFormValid || schedulesStore.loading"
          />
        </div>
      </form>
    </Dialog>

    <ConfirmDialog />
  </div>
</template>

<style scoped>
.schedules-view {
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

.description-cell {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.date-range {
  display: flex;
  flex-direction: column;
  gap: 0.125rem;
  font-size: 0.875rem;
}

.action-buttons {
  display: flex;
  gap: 0.25rem;
}

.dialog-form {
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
  padding-top: 1rem;
}

.field {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  flex: 1;
}

.field label {
  font-weight: 500;
}

.field-row {
  display: flex;
  gap: 1rem;
}

.field-checkbox {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.dialog-actions {
  display: flex;
  justify-content: flex-end;
  gap: 0.5rem;
  padding-top: 1rem;
}
</style>
