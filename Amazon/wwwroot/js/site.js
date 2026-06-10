/**
 * Amazon ITStep — Client-side helpers
 * Cart, Checkout та Orders керуються сервером (БД).
 * Цей файл оновлює лише UI: бейдж кошика та дрібні інтерактивні елементи.
 */

;(function () {
  'use strict';

  // ──────────────────────────────────────────────────────────────
  // CART COUNT BADGE  (оновлюється через API на кожній сторінці)
  // ──────────────────────────────────────────────────────────────

  const cartBadgeEl = document.querySelector('.cart-count');

  async function refreshCartBadge() {
    if (!cartBadgeEl) return;
    try {
      const res  = await fetch('/Cart/GetCount');
      const data = await res.json();
      cartBadgeEl.textContent = String(data.count ?? 0);
    } catch {
      cartBadgeEl.textContent = '0';
    }
  }

  refreshCartBadge();

  // ──────────────────────────────────────────────────────────────
  // ADD-TO-CART FORMS (product cards)
  // Після submit форми — оновлюємо бейдж без перезавантаження
  // ──────────────────────────────────────────────────────────────

  document.querySelectorAll('.product-card__cart-form').forEach(form => {
    form.addEventListener('submit', async e => {
      e.preventDefault();
      const btn = form.querySelector('.product-card__cart-btn');
      try {
        await fetch(form.action, { method: 'POST', body: new FormData(form) });
        btn && btn.classList.add('is-added');
        setTimeout(() => btn && btn.classList.remove('is-added'), 700);
      } catch { /* ігноруємо, сторінка все одно відкрита */ }
      await refreshCartBadge();
    });
  });

  // ──────────────────────────────────────────────────────────────
  // PROFILE PAGE — аватар / email / пароль / адреси (без змін)
  // ──────────────────────────────────────────────────────────────

  // Avatar upload
  const avatarWrap  = document.getElementById('avatarWrap');
  const avatarInput = document.getElementById('avatarInput');
  const avatarForm  = document.getElementById('avatarForm');
  const avatarHint  = document.getElementById('avatarHint');

  if (avatarWrap && avatarInput) {
    avatarWrap.addEventListener('click', () => avatarInput.click());

    avatarInput.addEventListener('change', async () => {
      if (!avatarInput.files?.length) return;
      const fd = new FormData(avatarForm);
      const res = await fetch(avatarForm.action, { method: 'POST', body: fd });
      const data = await res.json();
      if (data.success) {
        const img = document.getElementById('avatarImg');
        const letter = document.getElementById('avatarLetter');
        if (img) {
          img.src = data.avatarPath + '?t=' + Date.now();
        } else if (letter) {
          const newImg = document.createElement('img');
          newImg.id = 'avatarImg';
          newImg.className = 'profile-avatar profile-avatar--img';
          newImg.src = data.avatarPath;
          newImg.alt = letter.textContent;
          letter.replaceWith(newImg);
        }
        if (avatarHint) { avatarHint.textContent = 'Фото оновлено!'; setTimeout(() => avatarHint.textContent = '', 2500); }
      } else {
        if (avatarHint) avatarHint.textContent = data.message || 'Помилка.';
      }
    });
  }

  // Change email modal
  function setupModal(triggerId, modalId, cancelId, formId, hintId, submitUrl) {
    const trigger = document.getElementById(triggerId);
    const modal   = document.getElementById(modalId);
    const cancel  = document.getElementById(cancelId);
    const form    = document.getElementById(formId);
    const hint    = document.getElementById(hintId);
    if (!trigger || !modal) return;

    trigger.addEventListener('click', () => modal.hidden = false);
    cancel && cancel.addEventListener('click',  () => { modal.hidden = true; hint && (hint.textContent = ''); });
    modal.addEventListener('click', e => { if (e.target === modal) { modal.hidden = true; hint && (hint.textContent = ''); } });

    form && form.addEventListener('submit', async e => {
      e.preventDefault();
      const fd  = new FormData(form);
      const res = await fetch(submitUrl, { method: 'POST', body: fd });
      const d   = await res.json();
      if (hint) { hint.textContent = d.message || ''; hint.style.color = d.success ? 'green' : 'red'; }
      if (d.success) setTimeout(() => { modal.hidden = true; location.reload(); }, 1200);
    });
  }

  const profileChangeEmailUrl    = document.getElementById('changeEmailForm')    ? document.getElementById('changeEmailForm').action    : '';
  const profileChangePasswordUrl = document.getElementById('changePasswordForm') ? document.getElementById('changePasswordForm').action : '';

  setupModal('changeEmailBtn',    'changeEmailModal',    'cancelEmailBtn',    'changeEmailForm',    'emailHint',    profileChangeEmailUrl);
  setupModal('changePasswordBtn', 'changePasswordModal', 'cancelPasswordBtn', 'changePasswordForm', 'passwordHint', profileChangePasswordUrl);

  // Add address modal
  const addAddrBtn    = document.getElementById('addAddressBtn');
  const addAddrModal  = document.getElementById('addAddressModal');
  const cancelAddrBtn = document.getElementById('cancelAddressBtn');
  const addAddrForm   = document.getElementById('addAddressForm');
  const addAddrHint   = document.getElementById('addressHint');

  if (addAddrBtn) addAddrBtn.addEventListener('click', () => addAddrModal.hidden = false);
  if (cancelAddrBtn) cancelAddrBtn.addEventListener('click', () => addAddrModal.hidden = true);
  if (addAddrModal) addAddrModal.addEventListener('click', e => { if (e.target === addAddrModal) addAddrModal.hidden = true; });

  if (addAddrForm) {
    addAddrForm.addEventListener('submit', async e => {
      e.preventDefault();
      const fd  = new FormData(addAddrForm);
      const res = await fetch(addAddrForm.action, { method: 'POST', body: fd });
      const d   = await res.json();
      if (addAddrHint) { addAddrHint.textContent = d.message || ''; addAddrHint.style.color = d.success ? 'green' : 'red'; }
      if (d.success) setTimeout(() => location.reload(), 800);
    });
  }

  // Delete / set default address
  document.querySelectorAll('[data-delete-address]').forEach(btn => {
    btn.addEventListener('click', async () => {
      if (!confirm('Видалити адресу?')) return;
      const fd = new FormData(); fd.append('id', btn.dataset.deleteAddress);
      const token = document.querySelector('input[name="__RequestVerificationToken"]');
      if (token) fd.append('__RequestVerificationToken', token.value);
      await fetch(btn.dataset.url, { method: 'POST', body: fd });
      location.reload();
    });
  });

  document.querySelectorAll('[data-default-address]').forEach(btn => {
    btn.addEventListener('click', async () => {
      const fd = new FormData(); fd.append('id', btn.dataset.defaultAddress);
      const token = document.querySelector('input[name="__RequestVerificationToken"]');
      if (token) fd.append('__RequestVerificationToken', token.value);
      await fetch(btn.dataset.url, { method: 'POST', body: fd });
      location.reload();
    });
  });

})();
