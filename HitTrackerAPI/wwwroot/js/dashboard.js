// ============================================================
// dashboard.js — Ana Sayfa
// ============================================================

async function loadDashboard() {
  const u = localStorage.getItem('username')
  if (u) document.getElementById('dash-username').textContent = u

  const [quoteRes, sessionsRes, exercisesRes, programsRes, profileRes] =
    await Promise.allSettled([
      api('/quotes/random'),
      api('/sessions'),
      api('/exercises'),
      api('/programs'),
      api('/calories/profile')
    ])

  if (quoteRes.status === 'fulfilled') {
    const q = quoteRes.value
    document.getElementById('daily-quote').innerHTML =
      `<em>"${q.quote}"</em> <strong>— ${q.author}</strong>`
  }

  if (sessionsRes.status === 'fulfilled') {
    document.getElementById('stat-sessions').textContent = sessionsRes.value.length
    renderRecentSessions(sessionsRes.value.slice(0, 3))
  }

  if (exercisesRes.status === 'fulfilled') {
    document.getElementById('stat-exercises').textContent = exercisesRes.value.length
  }

  if (programsRes.status === 'fulfilled') {
    const programs = programsRes.value
    document.getElementById('stat-programs').textContent = programs.length
    const active = programs.find(p => p.isActive === true || p.is_active === true)
    renderActiveProgram(active)
  }

  if (profileRes.status === 'fulfilled') {
    const savedGoal = localStorage.getItem('selectedGoal') || 'strength'
    let data = profileRes.value
    // Kaydedilen goal ile yeniden çek
    if (savedGoal && savedGoal !== 'strength') {
      try { data = await api(`/calories/profile?goal=${savedGoal}`) } catch {}
    }
    const report  = data.report
    const profile = data.profile
    if (report) {
      report.goal = savedGoal
      const calories = report.targetCalories ?? report.target_calories ?? 0
      document.getElementById('stat-calories').textContent = calories
      renderCalorieWidget(report, profile)
    }
  }
}

function renderCalorieWidget(report, profile) {
  const widget = document.getElementById('calorie-widget')
  if (!widget) return
  widget.classList.remove('hidden')

  const calories  = report.targetCalories  ?? report.target_calories  ?? 0
  const proteinG  = report.macros?.proteinG ?? report.macros?.protein_g ?? 0
  const carbsG    = report.macros?.carbsG   ?? report.macros?.carbs_g   ?? 0
  const fatG      = report.macros?.fatG     ?? report.macros?.fat_g     ?? 0
  const bmr       = report.bmr ?? 0
  const weightVal = profile?.weightKg ?? profile?.weight_kg ?? ''
  const goal      = report.goal ?? 'strength'

  widget.innerHTML = `
    <div class="calorie-widget-header">
      <div>
        <h3>Daily Nutrition</h3>
        <p style="color:var(--text-3);font-size:0.78rem;margin-top:2px">Based on your profile</p>
      </div>
      <div style="display:flex;gap:10px;align-items:center">
        <span class="badge badge-gold">${goal.replace('_',' ').toUpperCase()}</span>
        <button class="btn-secondary" style="font-size:0.78rem;padding:6px 12px"
          onclick="toggleForm('nutrition-edit')">
          <i data-lucide="pencil"></i> Edit
        </button>
      </div>
    </div>
    <div id="nutrition-edit" class="hidden" style="margin-bottom:20px;padding:16px;
      background:var(--bg-3);border-radius:var(--radius-sm);border:1px solid var(--border)">
      <div style="display:grid;grid-template-columns:1fr 1fr 1fr;gap:12px;align-items:end">
        <div class="form-group" style="margin-bottom:0">
          <label>Weight (kg)</label>
          <input type="number" id="edit-weight" value="${weightVal}" min="30" max="300" step="0.1">
        </div>
        <div class="form-group" style="margin-bottom:0">
          <label>Goal</label>
          <select id="edit-goal">
            <option value="muscle_gain" ${goal==='muscle_gain'?'selected':''}>Build Muscle</option>
            <option value="fat_loss"    ${goal==='fat_loss'?'selected':''}>Burn Fat</option>
            <option value="strength"    ${goal==='strength'?'selected':''}>Strength</option>
            <option value="endurance"   ${goal==='endurance'?'selected':''}>Endurance</option>
          </select>
        </div>
        <button class="btn-primary" style="height:40px" onclick="updateNutrition()">
          <i data-lucide="refresh-cw"></i> Recalculate
        </button>
      </div>
    </div>
    <div class="calorie-widget-grid">
      <div class="cal-stat main">
        <label>Target Calories</label>
        <span class="cal-num">${calories}</span>
        <small>kcal / day</small>
      </div>
      <div class="cal-stat">
        <label>Protein</label>
        <span class="cal-num protein">${proteinG}g</span>
        <small>${Math.round(proteinG * 4)} kcal</small>
      </div>
      <div class="cal-stat">
        <label>Carbs</label>
        <span class="cal-num carbs">${carbsG}g</span>
        <small>${Math.round(carbsG * 4)} kcal</small>
      </div>
      <div class="cal-stat">
        <label>Fat</label>
        <span class="cal-num fat">${fatG}g</span>
        <small>${Math.round(fatG * 9)} kcal</small>
      </div>
      <div class="cal-stat">
        <label>BMR</label>
        <span class="cal-num">${bmr}</span>
        <small>base rate</small>
      </div>
    </div>
    ${report.summary ? `<p class="cal-summary">${report.summary}</p>` : ''}
  `
  lucide.createIcons()
}

async function updateNutrition() {
  const weight = parseFloat(document.getElementById('edit-weight').value)
  const goal   = document.getElementById('edit-goal').value
  if (!weight || weight < 30 || weight > 300) {
    showToast('Weight must be 30-300 kg', 'error')
    return
  }
  try {
    const current = await api('/calories/profile')
    const p = current.profile
    await api('/calories/profile', 'POST', {
      birthDate:     p.birthDate     || p.birth_date,
      heightCm:      p.heightCm      || p.height_cm,
      gender:        p.gender,
      activityLevel: p.activityLevel || p.activity_level,
      weightKg:      weight
    })

    localStorage.setItem('selectedGoal', goal)

    const updated = await api(`/calories/profile?goal=${goal}`)
    updated.report.goal = goal
    renderCalorieWidget(updated.report, updated.profile)
    const calories = updated.report.targetCalories ?? updated.report.target_calories ?? 0
    document.getElementById('stat-calories').textContent = calories
    showToast('Nutrition updated!', 'success')
  } catch (err) {
    showToast(err.message, 'error')
  }
}

function renderRecentSessions(sessions) {
  const el = document.getElementById('recent-sessions')
  if (!el) return
  if (!sessions?.length) {
    el.innerHTML = '<div class="empty-state"><p>No sessions yet</p></div>'
    return
  }
  el.innerHTML = sessions.map(s => {
    const date     = s.sessionDate || s.session_date
    const progName = s.program?.name || s.programName || s.program_name || 'Manual'
    const exCount  = s.exercises?.length || s.exerciseCount || s.exercise_count || 0
    return `
    <div style="display:flex;justify-content:space-between;align-items:center;
      padding:10px 0;border-bottom:1px solid var(--border)">
      <div>
        <p style="font-weight:500;font-size:0.9rem">${formatDate(date)}</p>
        <p style="color:var(--text-3);font-size:0.78rem">${progName} · ${exCount} exercises</p>
      </div>
    </div>
  `}).join('')
}

function renderActiveProgram(program) {
  const el = document.getElementById('active-program')
  if (!el) return
  if (!program) {
    el.innerHTML = `
      <div class="empty-state">
        <p>No active program</p>
        <button class="btn-primary" style="margin-top:12px"
          onclick="window.location.href='/Training'">
          Create Program
        </button>
      </div>
    `
    return
  }
  const exCount = program.exercises?.length || program.exerciseCount || program.exercise_count || 0
  el.innerHTML = `
    <div style="padding:14px;background:var(--bg-3);border-radius:var(--radius-sm);
      border:1px solid var(--gold-border)">
      <div style="display:flex;justify-content:space-between;align-items:center;margin-bottom:8px">
        <h4 style="font-size:0.92rem">${program.name}</h4>
        <span class="badge badge-active">Active</span>
      </div>
      <p style="color:var(--text-3);font-size:0.8rem;margin-bottom:12px">${exCount} exercises</p>
      <button class="btn-primary" style="font-size:0.82rem;padding:8px 14px"
        onclick="window.location.href='/Training'">
        <i data-lucide="play"></i> Start Workout
      </button>
    </div>
  `
  lucide.createIcons()
}
