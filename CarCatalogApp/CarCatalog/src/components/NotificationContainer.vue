<script setup>
import { ref } from 'vue'

const notifications = ref([])

const addNotification = (message, type = 'info') => {
  const id = Date.now()
  const notification = {
    id,
    message,
    type,
    timestamp: new Date()
  }
  
  notifications.value.unshift(notification)
  
  if (notifications.value.length > 5) {
    notifications.value = notifications.value.slice(0, 5)
  }
  
  setTimeout(() => {
    removeNotification(id)
  }, 5000)
}

const removeNotification = (id) => {
  const index = notifications.value.findIndex(n => n.id === id)
  if (index !== -1) {
    notifications.value.splice(index, 1)
  }
}

const notificationTypes = {
  success: {
    icon: '✅',
    color: 'var(--tts-success)',
    bg: 'rgba(16, 185, 129, 0.1)'
  },
  error: {
    icon: '❌',
    color: 'var(--tts-error)',
    bg: 'rgba(239, 68, 68, 0.1)'
  },
  warning: {
    icon: '⚠️',
    color: 'var(--tts-warning)',
    bg: 'rgba(245, 158, 11, 0.1)'
  },
  info: {
    icon: 'ℹ️',
    color: 'var(--tts-primary)',
    bg: 'rgba(26, 86, 219, 0.1)'
  }
}

defineExpose({ addNotification })
</script>

<template>
  <div class="notifications-container">
    <transition-group name="notification-slide">
      <div 
        v-for="notification in notifications" 
        :key="notification.id"
        :class="['notification', notification.type]"
        @click="removeNotification(notification.id)"
        :style="{
          '--notification-color': notificationTypes[notification.type]?.color || notificationTypes.info.color,
          '--notification-bg': notificationTypes[notification.type]?.bg || notificationTypes.info.bg
        }"
      >
        <div class="notification-icon">
          {{ notificationTypes[notification.type]?.icon || notificationTypes.info.icon }}
        </div>
        <div class="notification-content">
          <p class="notification-message">{{ notification.message }}</p>
          <span class="notification-time">
            {{ notification.timestamp.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }) }}
          </span>
        </div>
        <button class="notification-close" @click.stop="removeNotification(notification.id)">
          ×
        </button>
      </div>
    </transition-group>
  </div>
</template>

<style scoped>
.notifications-container {
  position: fixed;
  top: calc(var(--tts-navbar-height) + 1rem);
  right: 1rem;
  z-index: 9999;
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  max-width: 400px;
  width: 100%;
}

.notification {
  background: var(--tts-white);
  border-left: 4px solid var(--notification-color);
  border-radius: var(--tts-radius);
  box-shadow: var(--tts-shadow-lg);
  padding: 1rem;
  display: flex;
  align-items: flex-start;
  gap: 0.75rem;
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  position: relative;
  overflow: hidden;
}

.notification::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 4px;
  background: var(--notification-color);
  transform: scaleX(0);
  transform-origin: left;
  animation: progress 5s linear forwards;
}

.notification:hover {
  transform: translateX(-4px);
  box-shadow: var(--tts-shadow-xl);
}

.notification-icon {
  font-size: 1.25rem;
  flex-shrink: 0;
  margin-top: 0.125rem;
}

.notification-content {
  flex: 1;
  min-width: 0;
}

.notification-message {
  margin: 0;
  font-size: 0.875rem;
  color: var(--tts-gray-800);
  line-height: 1.5;
  word-break: break-word;
}

.notification-time {
  display: block;
  margin-top: 0.25rem;
  font-size: 0.75rem;
  color: var(--tts-gray-500);
}

.notification-close {
  background: none;
  border: none;
  color: var(--tts-gray-400);
  font-size: 1.5rem;
  line-height: 1;
  cursor: pointer;
  padding: 0;
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: var(--tts-radius-sm);
  transition: all 0.2s;
  flex-shrink: 0;
}

.notification-close:hover {
  background: var(--tts-gray-100);
  color: var(--tts-gray-700);
}

.notification.success {
  border-color: var(--tts-success);
}

.notification.error {
  border-color: var(--tts-error);
}

.notification.warning {
  border-color: var(--tts-warning);
}

.notification.info {
  border-color: var(--tts-primary);
}

.notification-slide-enter-active,
.notification-slide-leave-active {
  transition: all 0.4s cubic-bezier(0.68, -0.55, 0.265, 1.55);
}

.notification-slide-enter-from {
  opacity: 0;
  transform: translateX(100%);
}

.notification-slide-leave-to {
  opacity: 0;
  transform: translateX(100%) scale(0.8);
}

.notification-slide-move {
  transition: transform 0.4s ease;
}

@keyframes progress {
  from {
    transform: scaleX(1);
  }
  to {
    transform: scaleX(0);
  }
}

@media (max-width: 640px) {
  .notifications-container {
    right: 0.5rem;
    left: 0.5rem;
    max-width: none;
  }
}
</style>