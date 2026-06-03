// ============================================================
// app.js — Razor Pages için yönlendirme
// ============================================================

function toggleForm(id) {
  const el = document.getElementById(id)
  if (el) {
    el.classList.toggle('hidden')
    lucide.createIcons()
  }
}

const currentPath = window.location.pathname.toLowerCase()

async function init() {
  const token = localStorage.getItem('token')

  // Auth sayfasında token varsa dashboarda yönlendir
  if (currentPath.includes('/auth/')) {
    if (token) window.location.href = '/'
    return
  }

  // Onboarding — sadece token kontrolü
  if (currentPath.includes('/onboarding')) {
    if (!token) window.location.href = '/Auth/Login'
    return
  }

  // Diğer sayfalar — token yoksa login
  if (!token) {
    window.location.href = '/Auth/Login'
    return
  }

  try {
    await api('/calories/profile')
    document.getElementById('navbar')?.classList.remove('hidden')
    const u = localStorage.getItem('username')
    if (u) {
      const el = document.getElementById('dash-username')
      if (el) el.textContent = u
    }

    if (currentPath === '/' || currentPath === '/index') {
      loadDashboard()
    } else if (currentPath.includes('/exercises')) {
      loadExercises()
    } else if (currentPath.includes('/training') || currentPath.includes('/programs')) {
      loadTraining()
    }
    // AI sayfası kendi içinde init oluyor

    lucide.createIcons()
  } catch (err) {
    if (err.message.includes('Profile not found')) {
      window.location.href = '/Onboarding'
    } else {
      localStorage.clear()
      window.location.href = '/Auth/Login'
    }
  }
}

document.addEventListener('DOMContentLoaded', () => {
  init()
  lucide.createIcons()
})

document.addEventListener('keydown', e => {
  if (e.key === 'Escape') {
    document.getElementById('exercise-modal')?.classList.add('hidden')
    if (typeof confirmCancel === 'function') confirmCancel()
  }
})

document.addEventListener('click', e => {
  if (!e.target.closest('[id^="psearch-"]') && !e.target.closest('[id^="pdrop-"]')) {
    document.querySelectorAll('[id^="pdrop-"]').forEach(d => d.classList.add('hidden'))
  }
  const isAISearch = e.target.closest('#ai-ex-search') || e.target.closest('#ai-ex-drop')
  if (!isAISearch) {
    document.getElementById('ai-ex-drop')?.classList.add('hidden')
  }
})
