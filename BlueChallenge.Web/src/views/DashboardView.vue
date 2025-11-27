<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useSchedulesStore } from '@/stores/schedules'
import { useUsersStore } from '@/stores/users'
import type { ICreateScheduleRequest } from '@/types'
import Card from 'primevue/card'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Tag from 'primevue/tag'
import Dialog from 'primevue/dialog'
import Button from 'primevue/button'
import Textarea from 'primevue/textarea'
import DatePicker from 'primevue/datepicker'
import Select from 'primevue/select'
import Checkbox from 'primevue/checkbox'
import { useToast } from 'primevue/usetoast'

const schedulesStore = useSchedulesStore()
const usersStore = useUsersStore()
const toast = useToast()

// Modal de criação
const showDialog = ref(false)

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

// Validações
const minEndDate = computed(() => form.value.startDate || undefined)

const isDateRangeValid = computed(() => {
  if (!form.value.startDate || !form.value.endDate) return true
  return form.value.endDate >= form.value.startDate
})

const isHourRangeValid = computed(() => {
  if (form.value.isAllDay) return true
  if (!form.value.startTime || !form.value.endTime) return true
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
  if (!isDateRangeValid.value) return 'A data fim não pode ser anterior à data início'
  return null
})

const hourValidationError = computed(() => {
  if (!isHourRangeValid.value) return 'A hora fim não pode ser anterior à hora início no mesmo dia'
  if (!form.value.isAllDay && (!form.value.startTime || !form.value.endTime)) return 'Informe a hora início e hora fim'
  return null
})

onMounted(() => {
  schedulesStore.fetchSchedules()
  usersStore.fetchUsers()
})

const totalSchedules = computed(() => schedulesStore.schedules.length)
const totalUsers = computed(() => usersStore.users.length)

const today = computed(() => {
  const now = new Date()
  return new Date(now.getFullYear(), now.getMonth(), now.getDate()).getTime()
})

// Função para obter o timestamp completo (data + hora)
function getFullDateTime(schedule: { dateRange: { start: string; end: string }; hourRange?: { start: string; end: string } | null; isAllDay: boolean }, useStart: boolean): number {
  const dateStr = useStart ? schedule.dateRange.start : schedule.dateRange.end
  const date = new Date(dateStr)
  
  if (!schedule.isAllDay && schedule.hourRange) {
    const timeStr = useStart ? schedule.hourRange.start : schedule.hourRange.end
    const [hours, minutes] = timeStr.split(':').map(Number)
    date.setHours(hours ?? 0, minutes ?? 0, 0, 0)
  } else {
    // Dia todo: início às 00:00, fim às 23:59
    if (useStart) {
      date.setHours(0, 0, 0, 0)
    } else {
      date.setHours(23, 59, 59, 999)
    }
  }
  
  return date.getTime()
}

// Agendamentos futuros e de hoje (ordenados por data e hora crescente)
const upcomingSchedules = computed(() => {
  return schedulesStore.schedules
    .filter((s) => {
      const endDate = new Date(s.dateRange.end)
      const endDateTime = new Date(endDate.getFullYear(), endDate.getMonth(), endDate.getDate()).getTime()
      return endDateTime >= today.value
    })
    .sort((a, b) => getFullDateTime(a, true) - getFullDateTime(b, true))
})

// Agendamentos passados (ordenados por data e hora decrescente - mais recentes primeiro)
const pastSchedules = computed(() => {
  return schedulesStore.schedules
    .filter((s) => {
      const endDate = new Date(s.dateRange.end)
      const endDateTime = new Date(endDate.getFullYear(), endDate.getMonth(), endDate.getDate()).getTime()
      return endDateTime < today.value
    })
    .sort((a, b) => getFullDateTime(b, false) - getFullDateTime(a, false))
})

function isToday(schedule: { dateRange: { start: string; end: string } }): boolean {
  const startDate = new Date(schedule.dateRange.start)
  const endDate = new Date(schedule.dateRange.end)
  const startDateTime = new Date(startDate.getFullYear(), startDate.getMonth(), startDate.getDate()).getTime()
  const endDateTime = new Date(endDate.getFullYear(), endDate.getMonth(), endDate.getDate()).getTime()
  return startDateTime <= today.value && endDateTime >= today.value
}

function formatDate(dateString: string): string {
  return new Date(dateString).toLocaleDateString('pt-BR')
}

// Funções de ordenação customizadas para incluir horário
function sortByStartDateTime(event: { data: typeof schedulesStore.schedules; order: number }) {
  return event.data.sort((a, b) => {
    const diff = getFullDateTime(a, true) - getFullDateTime(b, true)
    return event.order === 1 ? diff : -diff
  })
}

function sortByEndDateTime(event: { data: typeof schedulesStore.schedules; order: number }) {
  return event.data.sort((a, b) => {
    const diff = getFullDateTime(a, false) - getFullDateTime(b, false)
    return event.order === 1 ? diff : -diff
  })
}

function getRowClass(data: { dateRange: { start: string; end: string } }): string {
  return isToday(data) ? 'today-row' : ''
}

function openCreateDialog() {
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

function buildRequest(): ICreateScheduleRequest {
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
    await schedulesStore.createSchedule(buildRequest())
    toast.add({
      severity: 'success',
      summary: 'Sucesso',
      detail: 'Agendamento criado com sucesso',
      life: 3000,
    })
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
</script>

<template>
  <div class="dashboard">
    <div class="dashboard-header">
      <h1>Dashboard</h1>
      <div class="stats-mini">
        <span class="stat-mini"><i class="pi pi-users"></i> {{ totalUsers }} usuários</span>
        <span class="stat-mini"><i class="pi pi-calendar"></i> {{ totalSchedules }} agendamentos</span>
      </div>
    </div>

    <div class="action-wrapper">
      <Card class="action-card" @click="openCreateDialog">
        <template #content>
          <div class="action-content">
            <span class="action-text">Novo Agendamento</span>
          </div>
        </template>
      </Card>
    </div>

    <Card class="schedules-card">
      <template #title>
        <div class="section-header">
          <i class="pi pi-calendar-plus"></i>
          <span>Próximos Agendamentos</span>
          <Tag v-if="upcomingSchedules.length > 0" :value="upcomingSchedules.length.toString()" severity="info" />
        </div>
      </template>
      <template #content>
        <DataTable
          :value="upcomingSchedules"
          :rowClass="getRowClass"
          stripedRows
          showGridlines
          :rows="5"
          paginator
          :rowsPerPageOptions="[5, 10, 20]"
          tableStyle="min-width: 50rem"
        >
          <template #empty>
            <div class="empty-message">
              <i class="pi pi-calendar"></i>
              <p>Nenhum agendamento próximo</p>
            </div>
          </template>

          <Column header="Status" style="width: 100px">
            <template #body="{ data }">
              <Tag v-if="isToday(data)" value="Hoje" severity="warn" />
              <Tag v-else value="Futuro" severity="info" />
            </template>
          </Column>

          <Column field="description" header="Descrição" style="width: 25%"></Column>

          <Column field="dateRange.start" header="Data Inicial" sortable :sortFunction="sortByStartDateTime" style="width: 15%">
            <template #body="{ data }">
              <span>{{ formatDate(data.dateRange.start) }}</span>
            </template>
          </Column>

          <Column field="dateRange.end" header="Data Final" sortable :sortFunction="sortByEndDateTime" style="width: 15%">
            <template #body="{ data }">
              <span>{{ formatDate(data.dateRange.end) }}</span>
            </template>
          </Column>

          <Column header="Horário" style="width: 15%">
            <template #body="{ data }">
              <Tag v-if="data.isAllDay" value="Dia todo" severity="secondary" />
              <span v-else-if="data.hourRange">
                {{ data.hourRange.start.substring(0, 5) }} - {{ data.hourRange.end.substring(0, 5) }}
              </span>
            </template>
          </Column>

          <Column header="Usuário" style="width: 20%">
            <template #body="{ data }">
              <span>{{ data.user.credentials.email.address }}</span>
            </template>
          </Column>
        </DataTable>
      </template>
    </Card>

    <Card class="schedules-card past-schedules">
      <template #title>
        <div class="section-header">
          <i class="pi pi-history"></i>
          <span>Agendamentos Passados</span>
          <Tag v-if="pastSchedules.length > 0" :value="pastSchedules.length.toString()" severity="secondary" />
        </div>
      </template>
      <template #content>
        <DataTable
          :value="pastSchedules"
          stripedRows
          showGridlines
          :rows="5"
          paginator
          :rowsPerPageOptions="[5, 10, 20]"
          tableStyle="min-width: 50rem"
        >
          <template #empty>
            <div class="empty-message">
              <i class="pi pi-check-circle"></i>
              <p>Nenhum agendamento passado</p>
            </div>
          </template>

          <Column field="description" header="Descrição" style="width: 25%"></Column>

          <Column field="dateRange.start" header="Data Inicial" sortable :sortFunction="sortByStartDateTime" style="width: 15%">
            <template #body="{ data }">
              <span>{{ formatDate(data.dateRange.start) }}</span>
            </template>
          </Column>

          <Column field="dateRange.end" header="Data Final" sortable :sortFunction="sortByEndDateTime" style="width: 15%">
            <template #body="{ data }">
              <span>{{ formatDate(data.dateRange.end) }}</span>
            </template>
          </Column>

          <Column header="Horário" style="width: 15%">
            <template #body="{ data }">
              <Tag v-if="data.isAllDay" value="Dia todo" severity="secondary" />
              <span v-else-if="data.hourRange">
                {{ data.hourRange.start.substring(0, 5) }} - {{ data.hourRange.end.substring(0, 5) }}
              </span>
            </template>
          </Column>

          <Column header="Usuário" style="width: 25%">
            <template #body="{ data }">
              <span>{{ data.user.credentials.email.address }}</span>
            </template>
          </Column>
        </DataTable>
      </template>
    </Card>

    <!-- Modal de Criação -->
    <Dialog
      v-model:visible="showDialog"
      header="Novo Agendamento"
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
            label="Criar"
            :loading="schedulesStore.loading"
            :disabled="!isFormValid || schedulesStore.loading"
          />
        </div>
      </form>
    </Dialog>
  </div>
</template>

<style scoped>
.dashboard {
  padding: 1rem;
}

.dashboard-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.dashboard h1 {
  margin: 0;
  color: var(--p-text-color);
}

.stats-mini {
  display: flex;
  gap: 1.5rem;
}

.stat-mini {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: var(--p-text-muted-color);
  font-size: 0.875rem;
}

.stat-mini i {
  color: var(--p-primary-500);
}

.schedules-card {
  margin-bottom: 1.5rem;
}

.section-header {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.section-header i {
  color: var(--p-primary-500);
}

.past-schedules {
  opacity: 0.85;
}

.empty-message {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 2rem;
  color: var(--p-text-muted-color);
}

.empty-message i {
  font-size: 2rem;
  margin-bottom: 0.5rem;
}

:deep(.today-row) {
  background-color: var(--p-highlight-background) !important;
  font-weight: 600;
}

:deep(.today-row td) {
  background-color: var(--p-highlight-background) !important;
}

.action-wrapper {
  display: flex;
  justify-content: center;
  margin-bottom: 2rem;
}

.action-card {
  cursor: pointer;
  transition: transform 0.2s, box-shadow 0.2s;
  background: linear-gradient(135deg, var(--p-primary-400) 0%, var(--p-primary-500) 50%, var(--p-primary-600) 100%) !important;
  border: 3px solid var(--p-surface-0) !important;
  box-shadow: 0 4px 15px color-mix(in srgb, var(--p-primary-500) 40%, transparent);
  min-width: 300px;
}

.action-card:hover {
  transform: translateY(-4px) scale(1.02);
  box-shadow: 0 8px 30px color-mix(in srgb, var(--p-primary-500) 50%, transparent);
}

.action-content {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 60px;
}

.action-text {
  font-size: 1.5rem;
  font-weight: 700;
  color: white;
  text-shadow: 1px 1px 2px rgba(0, 0, 0, 0.2);
}

/* Estilos do Dialog */
.dialog-form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.field {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.field label {
  font-weight: 500;
  color: var(--p-text-color);
}

.field-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
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
  margin-top: 1rem;
  padding-top: 1rem;
  border-top: 1px solid var(--p-surface-border);
}

.p-error {
  color: var(--p-red-500);
}
</style>
