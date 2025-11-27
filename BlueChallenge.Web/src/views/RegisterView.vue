<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { usersService } from '@/services/users.service'
import InputText from 'primevue/inputtext'
import Password from 'primevue/password'
import Button from 'primevue/button'
import Card from 'primevue/card'
import Message from 'primevue/message'

const router = useRouter()

const email = ref('')
const password = ref('')
const confirmPassword = ref('')
const loading = ref(false)
const error = ref<string | null>(null)
const success = ref(false)

async function handleRegister() {
  error.value = null

  if (password.value !== confirmPassword.value) {
    error.value = 'As senhas não coincidem'
    return
  }

  if (password.value.length < 6) {
    error.value = 'A senha deve ter pelo menos 6 caracteres'
    return
  }

  loading.value = true

  try {
    await usersService.create({
      email: email.value,
      password: password.value,
    })
    success.value = true
    setTimeout(() => {
      router.push('/login')
    }, 2000)
  } catch (e) {
    error.value = e instanceof Error ? e.message : 'Erro ao criar conta'
  } finally {
    loading.value = false
  }
}

function goToLogin() {
  router.push('/login')
}
</script>

<template>
  <div class="register-container">
    <Card class="register-card">
      <template #title>
        <div class="register-header">
          <i class="pi pi-user-plus text-4xl text-primary"></i>
          <h1>Criar Conta</h1>
        </div>
      </template>
      <template #content>
        <Message v-if="success" severity="success" :closable="false">
          Conta criada com sucesso! Redirecionando para o login...
        </Message>

        <form v-if="!success" @submit.prevent="handleRegister" class="register-form">
          <Message v-if="error" severity="error" :closable="false">
            {{ error }}
          </Message>

          <div class="field">
            <label for="email">Email</label>
            <InputText
              id="email"
              v-model="email"
              type="email"
              placeholder="Digite seu email"
              class="w-full"
              :disabled="loading"
              required
            />
          </div>

          <div class="field">
            <label for="password">Senha</label>
            <Password
              id="password"
              v-model="password"
              placeholder="Digite sua senha"
              class="w-full"
              :disabled="loading"
              toggleMask
              required
            />
          </div>

          <div class="field">
            <label for="confirmPassword">Confirmar Senha</label>
            <Password
              id="confirmPassword"
              v-model="confirmPassword"
              placeholder="Confirme sua senha"
              class="w-full"
              :feedback="false"
              :disabled="loading"
              toggleMask
              required
            />
          </div>

          <Button
            type="submit"
            label="Criar Conta"
            icon="pi pi-user-plus"
            class="w-full"
            :loading="loading"
          />

          <div class="login-link">
            <span>Já tem uma conta?</span>
            <Button
              type="button"
              label="Fazer Login"
              link
              @click="goToLogin"
              :disabled="loading"
            />
          </div>
        </form>
      </template>
    </Card>
  </div>
</template>

<style scoped>
.register-container {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #001433 0%, #003d99 100%);
}

.register-card {
  width: 100%;
  max-width: 400px;
  margin: 1rem;
}

.register-header {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
  text-align: center;
}

.register-header h1 {
  margin: 0;
  font-size: 1.5rem;
  color: var(--p-primary-color);
}

.register-form {
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

.login-link {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.25rem;
  margin-top: 0.5rem;
}

.login-link span {
  color: var(--p-text-muted-color);
  font-size: 0.875rem;
}
</style>
