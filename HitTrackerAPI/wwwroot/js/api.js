// ============================================================
// api.js — API helper, Toast, Confirm Modal
// ============================================================

const API = 'http://localhost:5210/api'

async function api(endpoint, method = 'GET', body = null) {
  const token = localStorage.getItem('token')
  const opts  = {
    method,
    headers: {
      'Content-Type': 'application/json',
      ...(token && { Authorization: `Bearer ${token}` })
    }
  }
  if (body) opts.body = JSON.stringify(body)
  const res = await fetch(`${API}${endpoint}`, opts)

  // 204 No Content veya boş body
  let data = null
  const text = await res.text()
  if (text) {
    try { data = JSON.parse(text) } catch { data = { error: text } }
  }

  if (!res.ok) {
    let msg = 'Error'
    if (data?.errors) {
      msg = Array.isArray(data.errors)
        ? data.errors.join(', ')
        : Object.values(data.errors).flat().join(', ')
    } else if (data?.error) {
      msg = data.error
    } else if (data?.title) {
      msg = data.title
    } else if (data?.message) {
      msg = data.message
    }
    throw new Error(msg)
  }
  return data
}

function showErr(id, msg) {
  const el = document.getElementById(id)
  if (!el) return
  el.textContent = msg
  el.classList.remove('hidden')
  setTimeout(() => el.classList.add('hidden'), 5000)
}

function hideErr(id) {
  document.getElementById(id)?.classList.add('hidden')
}

function showToast(message, type = 'info', duration = 3500) {
  const icons = { success: 'check-circle', error: 'x-circle', info: 'info' }
  const toast = document.createElement('div')
  toast.className = `toast ${type}`
  toast.innerHTML = `<i data-lucide="${icons[type]}"></i><p>${message}</p>`
  const container = document.getElementById('toast-container')
  if (!container) return
  container.appendChild(toast)
  lucide.createIcons()
  setTimeout(() => {
    toast.style.opacity    = '0'
    toast.style.transform  = 'translateX(110%)'
    toast.style.transition = 'all 0.3s ease'
    setTimeout(() => toast.remove(), 300)
  }, duration)
}

let confirmCallback = null

function showConfirm(title, message, onOk, okText = 'Delete', okType = 'danger') {
  document.getElementById('confirm-title').textContent   = title
  document.getElementById('confirm-message').textContent = message
  const btn       = document.getElementById('confirm-ok-btn')
  btn.textContent = okText
  btn.className   = okType === 'danger' ? 'btn-ghost-danger' : 'btn-primary'
  document.getElementById('confirm-modal').classList.remove('hidden')
  confirmCallback = onOk
  lucide.createIcons()
}

function confirmOk() {
  document.getElementById('confirm-modal').classList.add('hidden')
  if (confirmCallback) confirmCallback()
  confirmCallback = null
}

function confirmCancel() {
  document.getElementById('confirm-modal').classList.add('hidden')
  confirmCallback = null
}

function formatDate(str) {
  if (!str) return '—'
  const parts = str.split('T')[0].split('-')
  return new Date(parseInt(parts[0]), parseInt(parts[1]) - 1, parseInt(parts[2]))
    .toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' })
}
