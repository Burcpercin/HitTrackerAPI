// ============================================================
// ai.js — AI Coach (Gemini) — sadece suggestion gösterir
// ============================================================

const aiChoices  = { ai_exp: null, ai_eq: null }
let selectedDays = []

function toggleDay(btn) {
  const day = parseInt(btn.dataset.day)
  if (selectedDays.includes(day)) {
    selectedDays = selectedDays.filter(d => d !== day)
    btn.classList.remove('selected')
  } else {
    selectedDays.push(day)
    btn.classList.add('selected')
  }
}

async function getAISuggestion() {
  const goal            = document.getElementById('ai-goal').value
  const experienceLevel = aiChoices.ai_exp
  const equipment       = aiChoices.ai_eq
  const injuriesEl      = document.getElementById('ai-injuries')
  const injuries        = injuriesEl ? injuriesEl.value.trim() : ''

  if (selectedDays.length === 0) { showErr('ai-error', 'Please select at least 1 training day'); return }
  if (!experienceLevel) { showErr('ai-error', 'Select experience level'); return }
  if (!equipment)       { showErr('ai-error', 'Select equipment'); return }

  const daysPerWeek = selectedDays.length
  const btn = document.getElementById('ai-generate-btn')
  btn.innerHTML = '<div class="spinner" style="width:18px;height:18px;border-width:2px;margin:0 auto"></div>'
  btn.disabled  = true
  hideErr('ai-error')

  const resultEl = document.getElementById('ai-result')
  if (resultEl) resultEl.classList.add('hidden')

  try {
    const result = await api('/ai/workout-suggestion', 'POST', {
      daysPerWeek,
      availableDays: selectedDays,
      goal,
      experienceLevel,
      equipment,
      injuries: injuries || null
    })

   const textEl = document.getElementById('ai-response-text')
    if (textEl) {
      let rawText = result.suggestion || 'No suggestion received.';
      
      // Basit Markdown ve Satır atlama dönüştürücü
      let formattedText = rawText
        .replace(/\n/g, '<br>') // Satır atlamalarını HTML'e çevir
        .replace(/\*\*(.*?)\*\*/g, '<strong style="color:var(--text-1)">$1</strong>') // Kalın yazıları vurgula
        .replace(/---/g, '<hr style="border:0; border-top:1px solid var(--border); margin: 15px 0;">'); // Ayırıcı çizgiler
        
      textEl.innerHTML = formattedText;
    }
    
    if (resultEl) {
      resultEl.classList.remove('hidden')
      resultEl.scrollIntoView({ behavior: 'smooth' })
    }
  } catch (err) {
    showErr('ai-error', err.message.includes('Profile not found')
      ? 'Please complete your profile first.'
      : err.message)
  } finally {
    btn.innerHTML = '<i data-lucide="sparkles"></i> Generate My Program'
    btn.disabled  = false
    lucide.createIcons()
  }
}