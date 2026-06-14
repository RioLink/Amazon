/**
 * Amazon ITStep — Client-side helpers
 */

;(function () {
  'use strict';

  // ── CSRF token helper ────────────────────────────────────────────
  function getCsrfToken() {
    return document.querySelector('input[name="__RequestVerificationToken"]')?.value ?? '';
  }

  // ── TOAST ────────────────────────────────────────────────────────
  let toastContainer = null;

  function showToast(message, type = 'success', duration = 3000) {
    if (!toastContainer) {
      toastContainer = document.createElement('div');
      toastContainer.className = 'toast-container';
      document.body.appendChild(toastContainer);
    }
    const toast = document.createElement('div');
    toast.className = `toast toast--${type}`;
    toast.innerHTML = `<span class="toast__icon">${type === 'success' ? '✓' : type === 'wish' ? '❤' : 'ℹ'}</span><span class="toast__msg">${message}</span>`;
    toastContainer.appendChild(toast);
    requestAnimationFrame(() => toast.classList.add('toast--visible'));
    setTimeout(() => {
      toast.classList.remove('toast--visible');
      setTimeout(() => toast.remove(), 300);
    }, duration);
  }

  // ── CART COUNT BADGE ─────────────────────────────────────────────
  const cartBadgeEl = document.querySelector('.cart-count');

  async function refreshCartBadge() {
    if (!cartBadgeEl) return;
    try {
      const res  = await fetch('/Cart/GetCount');
      const data = await res.json();
      cartBadgeEl.textContent = String(data.count ?? 0);
    } catch { cartBadgeEl.textContent = '0'; }
  }

  refreshCartBadge();

  // ── ADD TO CART (AJAX, product cards) ────────────────────────────
  function getAntiForgery() {
    // Try hidden input first, then meta tag
    return document.querySelector('input[name="__RequestVerificationToken"]')?.value
        ?? document.querySelector('meta[name="__RequestVerificationToken"]')?.content
        ?? '';
  }

  async function addToCartAjax(productId, productName) {
    const token = getAntiForgery();
    try {
      const res = await fetch('/Cart/AddAjax', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'RequestVerificationToken': token
        },
        body: JSON.stringify({ productId, quantity: 1 })
      });
      const data = await res.json();
      if (data.success) {
        if (cartBadgeEl) cartBadgeEl.textContent = String(data.cartCount ?? 0);
        showToast(`«${productName}» додано до кошика 🛒`, 'success');
      } else {
        showToast(data.message || 'Помилка', 'error');
      }
    } catch {
      showToast('Помилка з’єднання', 'error');
    }
  }

  document.addEventListener('click', e => {
    const btn = e.target.closest('.js-add-cart');
    if (!btn) return;
    e.preventDefault();
    const productId   = parseInt(btn.dataset.productId);
    const productName = btn.dataset.productName || 'Товар';
    btn.classList.add('is-added');
    setTimeout(() => btn.classList.remove('is-added'), 700);
    addToCartAjax(productId, productName);
  });

  // ── WISHLIST (AJAX toggle) ────────────────────────────────────────
  let wishlistIds = new Set();

  async function loadWishlistIds() {
    try {
      const res  = await fetch('/Wishlist/GetIds');
      const ids  = await res.json();
      wishlistIds = new Set(ids);
      paintWishlistBtns();
    } catch { /* not logged in */ }
  }

  function paintWishlistBtns() {
    document.querySelectorAll('.js-wishlist-btn').forEach(btn => {
      const id = parseInt(btn.dataset.productId);
      const wished = wishlistIds.has(id);
      btn.classList.toggle('is-wished', wished);
      // product card: button contains only text
      // product details: button contains .product-details__wishlist-icon + .product-details__wishlist-label
      const iconEl = btn.querySelector('.product-details__wishlist-icon');
      const labelEl = btn.querySelector('.product-details__wishlist-label');
      if (iconEl) {
        iconEl.textContent = wished ? '♥' : '♡';
        if (labelEl) labelEl.textContent = wished ? 'В обраному' : 'До обраного';
      } else {
        btn.textContent = wished ? '♥' : '♡';
      }
    });
  }

  document.addEventListener('click', async e => {
    const btn = e.target.closest('.js-wishlist-btn');
    if (!btn) return;
    const productId = parseInt(btn.dataset.productId);
    const token = getAntiForgery();
    try {
      const fd = new FormData();
      fd.append('productId', productId);
      fd.append('__RequestVerificationToken', token);
      const res  = await fetch('/Wishlist/Toggle', {
        method: 'POST',
        headers: { 'X-Requested-With': 'XMLHttpRequest' },
        body: fd
      });
      const data = await res.json();
      if (data.added) {
        wishlistIds.add(productId);
        showToast('Додано до обраного ❤', 'wish');
      } else {
        wishlistIds.delete(productId);
        showToast('Видалено з обраного', 'success');
        // якщо ми на сторінці Wishlist — анімовано прибираємо картку
        if (window.location.pathname.toLowerCase().startsWith('/wishlist')) {
          const card = btn.closest('.product-card');
          if (card) {
            card.style.transition = 'opacity .3s ease, transform .3s ease';
            card.style.opacity    = '0';
            card.style.transform  = 'scale(.92)';
            setTimeout(() => {
              card.remove();
              // якщо сітка порожня — показати empty-state
              const grid = document.querySelector('.product-grid');
              if (grid && !grid.querySelector('.product-card')) {
                grid.innerHTML = `
                  <div class="empty-state" style="grid-column:1/-1">
                    <span class="empty-state__icon">💔</span>
                    <h2>Список бажань порожній</h2>
                    <p>Натисніть ❤️ на будь-якому товарі, щоб зберегти його тут.</p>
                    <a class="amazon-button amazon-button--primary" href="/Product">Перейти до каталогу</a>
                  </div>`;
              }
            }, 320);
          }
        }
      }
      paintWishlistBtns();
    } catch { showToast('Помилка', 'error'); }
  });

  // Load wishlist ids only if logged in (button exists)
  if (document.querySelector('.js-wishlist-btn')) {
    loadWishlistIds();
  }

  // ──────────────────────────────────────────────────────────────────
  // PROFILE PAGE helpers
  // ──────────────────────────────────────────────────────────────────

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
        const img    = document.getElementById('avatarImg');
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

  // Modal helper
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

  // ── Skeleton loader ──────────────────────────────────────────────
  (function () {
    const grid     = document.querySelector('.js-product-grid');
    const skeleton = document.querySelector('.js-skeleton-grid');
    if (!grid || !skeleton) return;

    document.querySelectorAll('.pagination__btn, .catalog-category-card, .catalog-back-link').forEach(el => {
      el.addEventListener('click', () => {
        grid.style.display     = 'none';
        skeleton.style.display = '';
      });
    });
  })();

  // ── Scroll-reveal (Intersection Observer) ────────────────────────
  (function () {
    const els = document.querySelectorAll(
      '.product-card, .category-card, .benefit, .home-section, .section-heading'
    );
    if (!els.length || !('IntersectionObserver' in window)) return;

    const observer = new IntersectionObserver((entries) => {
      entries.forEach((entry, i) => {
        if (entry.isIntersecting) {
          // stagger siblings in the same grid
          const siblings = entry.target.parentElement?.querySelectorAll('.product-card, .category-card') ?? [];
          const idx = Array.from(siblings).indexOf(entry.target);
          const delay = idx >= 0 ? Math.min(idx * 60, 400) : 0;
          setTimeout(() => {
            entry.target.classList.add('is-revealed');
          }, delay);
          observer.unobserve(entry.target);
        }
      });
    }, { threshold: 0.08 });

    els.forEach(el => {
      el.classList.add('will-reveal');
      observer.observe(el);
    });
  })();

  // ── Ripple effect on buttons ──────────────────────────────────────
  document.addEventListener('click', e => {
    const btn = e.target.closest('.amazon-button, .js-add-cart, .admin-status-save');
    if (!btn) return;
    const r = document.createElement('span');
    r.className = 'btn-ripple';
    const rect = btn.getBoundingClientRect();
    const size = Math.max(rect.width, rect.height);
    r.style.cssText = `width:${size}px;height:${size}px;left:${e.clientX-rect.left-size/2}px;top:${e.clientY-rect.top-size/2}px`;
    btn.style.position = btn.style.position || 'relative';
    btn.appendChild(r);
    r.addEventListener('animationend', () => r.remove());
  });

  // ── Animated counter for stat numbers ────────────────────────────
  (function () {
    const els = document.querySelectorAll('[data-count-to]');
    if (!els.length) return;
    const observer = new IntersectionObserver(entries => {
      entries.forEach(entry => {
        if (!entry.isIntersecting) return;
        const el    = entry.target;
        const end   = parseFloat(el.dataset.countTo);
        const dur   = 1400;
        const step  = 16;
        const steps = dur / step;
        let cur = 0;
        const inc = end / steps;
        const suffix = el.dataset.countSuffix ?? '';
        const timer = setInterval(() => {
          cur += inc;
          if (cur >= end) { cur = end; clearInterval(timer); }
          el.textContent = (Number.isInteger(end) ? Math.round(cur) : cur.toFixed(1)) + suffix;
        }, step);
        observer.unobserve(el);
      });
    }, { threshold: 0.5 });
    els.forEach(el => observer.observe(el));
  })();

  // ── Back-to-top button ───────────────────────────────────────────
  (function () {
    const btn = document.getElementById('js-back-top');
    if (!btn) return;
    window.addEventListener('scroll', () => {
      btn.classList.toggle('is-visible', window.scrollY > 400);
    }, { passive: true });
    btn.addEventListener('click', () => window.scrollTo({ top: 0, behavior: 'smooth' }));
  })();

  // ── Hero parallax (subtle) ───────────────────────────────────────
  (function () {
    const visual = document.querySelector('.hero__visual');
    if (!visual) return;
    window.addEventListener('scroll', () => {
      const y = window.scrollY;
      if (y > 600) return;
      visual.style.transform = `translateY(${y * 0.12}px)`;
    }, { passive: true });
  })();

})();
