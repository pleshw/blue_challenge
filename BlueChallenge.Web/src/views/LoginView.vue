<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import InputText from 'primevue/inputtext'
import Password from 'primevue/password'
import Button from 'primevue/button'
import Card from 'primevue/card'
import Message from 'primevue/message'

const router = useRouter()
const authStore = useAuthStore()

const email = ref('')
const password = ref('')

async function handleLogin() {
  const success = await authStore.login({
    email: email.value,
    password: password.value,
  })

  if (success) {
    router.push('/dashboard')
  }
}

function goToRegister() {
  router.push('/register')
}
</script>

<template>
  <div class="login-container">
    <Card class="login-card">
      <template #title>
        <div class="login-header">
          <i class="pi pi-calendar text-4xl text-primary"></i>
          <h1>Blue Challenge</h1>
        </div>
      </template>
      <template #content>
        <form @submit.prevent="handleLogin" class="login-form">
          <Message v-if="authStore.error" severity="error" :closable="false">
            {{ authStore.error }}
          </Message>

          <div class="field">
            <label for="email">Email</label>
            <InputText
              id="email"
              v-model="email"
              type="email"
              placeholder="Digite seu email"
              class="w-full"
              :disabled="authStore.loading"
            />
          </div>

          <div class="field">
            <label for="password">Senha</label>
            <Password
              id="password"
              v-model="password"
              placeholder="Digite sua senha"
              class="w-full"
              :feedback="false"
              :disabled="authStore.loading"
              toggleMask
            />
          </div>

          <Button
            type="submit"
            label="Entrar"
            icon="pi pi-sign-in"
            class="w-full"
            :loading="authStore.loading"
          />

          <div class="register-link">
            <span>Não tem uma conta?</span>
            <Button
              type="button"
              label="Criar Conta"
              link
              @click="goToRegister"
              :disabled="authStore.loading"
            />
          </div>
        </form>
      </template>
    </Card>
  </div>
</template>

<style scoped>
.login-container {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #001433 0%, #003d99 100%);
}

.login-card {
  width: 100%;
  max-width: 400px;
  margin: 1rem;
}

.login-header {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
  text-align: center;
}

.login-header h1 {
  margin: 0;
  font-size: 1.5rem;
  color: var(--p-primary-color);
}

.login-form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.field {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.field label {
  font-weight: 500;
}

.register-link {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.25rem;
  margin-top: 0.5rem;
}

.register-link span {
  color: var(--p-text-muted-color);
  font-size: 0.875rem;
}
</style>
