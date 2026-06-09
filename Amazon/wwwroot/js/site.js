/**
 * Amazon Clone — Client-side engine
 * ───────────────────────────────────────────────────────────────
 * Cart, Checkout та Orders повністю зберігаються в localStorage.
 * Коли backend (BLL/DAL) буде готовий — замінити виклики
 * localStorage на fetch() до API-ендпоінтів.
 * ───────────────────────────────────────────────────────────────
 */

;(function () {
  'use strict';

  // ──────────────────────────────────────────────────────────────
  // CONSTANTS & STORAGE HELPERS
  // ──────────────────────────────────────────────────────────────

  const CART_KEY   = 'amazon-clone-cart';
  const ORDERS_KEY = 'amazon-clone-orders';

  /** @returns {Array} */
  function getCart() {
    try { return JSON.parse(localStorage.getItem(CART_KEY) || '[]'); }
    catch { return []; }
  }

  /** @param {Array} items */
  function saveCart(items) {
    localStorage.setItem(CART_KEY, JSON.stringify(items));
  }

  /** @returns {Array} */
  function getOrders() {
    try { return JSON.parse(localStorage.getItem(ORDERS_KEY) || '[]'); }
    catch { return []; }
  }

  /** @param {Array} orders */
  function saveOrders(orders) {
    localStorage.setItem(ORDERS_KEY, JSON.stringify(orders));
  }

  // ──────────────────────────────────────────────────────────────
  // UTILITY
  // ──────────────────────────────────────────────────────────────

  /** Format number as Ukrainian hryvnia string, e.g. 3 499 ₴ */
  function formatPrice(n) {
    return Number(n).toLocaleString('uk-UA', { maximumFractionDigits: 0 }) + ' ₴';
  }

  /** Safely escape string for HTML output (prevents XSS) */
  function esc(str) {
    const div = document.createElement('div');
    div.textContent = String(str);
    return div.innerHTML;
  }

  /** Calculate subtotal for a cart array */
  function calcSubtotal(cart) {
    return cart.reduce((sum, item) => sum + item.price * item.quantity, 0);
  }

  /** Calculate shipping: free over 500 ₴, else 99 ₴ */
  function calcShipping(subtotal) {
    return subtotal >= 500 ? 0 : 99;
  }

  /** Generate a unique order ID */
  function makeOrderId() {
    const d = new Date();
    const pad = n => String(n).padStart(2, '0');
    const date = `${d.getFullYear()}${pad(d.getMonth() + 1)}${pad(d.getDate())}`;
    const rand = Math.floor(Math.random() * 90000) + 10000;
    return `ORD-${date}-${rand}`;
  }

  // ──────────────────────────────────────────────────────────────
  // CART COUNT BADGE  (visible on every page in the header)
  // ──────────────────────────────────────────────────────────────

  const cartBadgeEl = document.querySelector('.cart-count');

  function renderCartBadge() {
    if (!cartBadgeEl) return;
    const qty = getCart().reduce((s, i) => s + i.quantity, 0);
    cartBadgeEl.textContent = qty.toString();
  }

  renderCartBadge(); // run immediately on page load

  // ──────────────────────────────────────────────────────────────
  // ADD-TO-CART BUTTONS  (product cards + details page)
  // Each button must have data-add-to-cart and data-product (JSON)
  // ──────────────────────────────────────────────────────────────

  document.querySelectorAll('[data-add-to-cart]').forEach(btn => {
    btn.addEventListener('click', () => {
      const raw = btn.dataset.product;
      if (!raw) return;

      let product;
      try { product = JSON.parse(raw); }
      catch { console.warn('Invalid data-product JSON', raw); return; }

      const cart = getCart();
      const existing = cart.find(i => i.id === product.id);

      if (existing) {
        existing.quantity += 1;
      } else {
        cart.push({ ...product, quantity: 1 });
      }

      saveCart(cart);
      renderCartBadge();

      // Visual feedback
      btn.classList.add('is-added');
      setTimeout(() => btn.classList.remove('is-added'), 700);
    });
  });

  // ══════════════════════════════════════════════════════════════
  // CART PAGE  (/Cart)
  // ══════════════════════════════════════════════════════════════

  const cartItemsListEl  = document.getElementById('cart-items-list');
  const cartEmptyStateEl = document.getElementById('cart-empty-state');
  const cartFilledEl     = document.getElementById('cart-filled-state');

  if (cartItemsListEl) {
    renderCartPage();
  }

  function renderCartPage() {
    const cart = getCart();

    if (cart.length === 0) {
      cartEmptyStateEl && (cartEmptyStateEl.hidden = false);
      cartFilledEl     && (cartFilledEl.hidden = true);
      syncCartSummary(0, 0);
      return;
    }

    cartEmptyStateEl && (cartEmptyStateEl.hidden = true);
    cartFilledEl     && (cartFilledEl.hidden = false);

    cartItemsListEl.innerHTML = cart.map(item => `
      <li class="cart-item" data-id="${item.id}">
        <img class="cart-item__image"
             src="${esc(item.imageUrl)}"
             alt="${esc(item.name)}" />

        <div class="cart-item__details">
          <span class="cart-item__category">${esc(item.category)}</span>
          <p class="cart-item__name">${esc(item.name)}</p>
          <span class="cart-item__unit-price">Ціна за одиницю: ${formatPrice(item.price)}</span>
        </div>

        <div class="cart-item__actions">
          <span class="cart-item__price">${formatPrice(item.price * item.quantity)}</span>

          <div class="qty-stepper" role="group" aria-label="Кількість ${esc(item.name)}">
            <button class="qty-stepper__btn"
                    data-qty-dec="${item.id}"
                    aria-label="Зменшити кількість">−</button>
            <span class="qty-stepper__value" aria-live="polite">${item.quantity}</span>
            <button class="qty-stepper__btn"
                    data-qty-inc="${item.id}"
                    aria-label="Збільшити кількість">+</button>
          </div>

          <button class="cart-item__remove"
                  data-remove="${item.id}"
                  aria-label="Видалити ${esc(item.name)} з кошика">
            Видалити
          </button>
        </div>
      </li>
    `).join('');

    attachCartItemHandlers();

    const subtotal = calcSubtotal(cart);
    const shipping = calcShipping(subtotal);
    syncCartSummary(subtotal, shipping);
  }

  function attachCartItemHandlers() {
    // Decrease quantity
    document.querySelectorAll('[data-qty-dec]').forEach(btn => {
      btn.addEventListener('click', () => {
        const id   = Number(btn.dataset.qtyDec);
        const cart = getCart();
        const item = cart.find(i => i.id === id);
        if (!item) return;

        if (item.quantity > 1) {
          item.quantity -= 1;
        } else {
          cart.splice(cart.indexOf(item), 1);
        }

        saveCart(cart);
        renderCartBadge();
        renderCartPage();
      });
    });

    // Increase quantity
    document.querySelectorAll('[data-qty-inc]').forEach(btn => {
      btn.addEventListener('click', () => {
        const id   = Number(btn.dataset.qtyInc);
        const cart = getCart();
        const item = cart.find(i => i.id === id);
        if (!item) return;

        item.quantity += 1;
        saveCart(cart);
        renderCartBadge();
        renderCartPage();
      });
    });

    // Remove item
    document.querySelectorAll('[data-remove]').forEach(btn => {
      btn.addEventListener('click', () => {
        const id      = Number(btn.dataset.remove);
        const newCart = getCart().filter(i => i.id !== id);
        saveCart(newCart);
        renderCartBadge();
        renderCartPage();
      });
    });
  }

  function syncCartSummary(subtotal, shipping) {
    const subtotalEl    = document.getElementById('cart-subtotal');
    const shippingEl    = document.getElementById('cart-shipping');
    const shippingRowEl = document.getElementById('cart-shipping-row');
    const totalEl       = document.getElementById('cart-total');

    if (subtotalEl)    subtotalEl.textContent = formatPrice(subtotal);
    if (shippingEl)    shippingEl.textContent = shipping === 0 ? 'Безкоштовно' : formatPrice(shipping);
    if (shippingRowEl) shippingRowEl.classList.toggle('cart-summary__row--shipping-free', shipping === 0);
    if (totalEl)       totalEl.textContent = formatPrice(subtotal + shipping);
  }

  // "Clear cart" button
  const clearCartBtn = document.getElementById('clear-cart-btn');
  if (clearCartBtn) {
    clearCartBtn.addEventListener('click', () => {
      if (window.confirm('Очистити кошик? Усі товари будуть видалені.')) {
        saveCart([]);
        renderCartBadge();
        renderCartPage();
      }
    });
  }

  // ══════════════════════════════════════════════════════════════
  // CHECKOUT PAGE  (/Checkout)
  // ══════════════════════════════════════════════════════════════

  const checkoutSummaryItemsEl = document.getElementById('checkout-summary-items');

  if (checkoutSummaryItemsEl) {
    initCheckoutPage();
  }

  function initCheckoutPage() {
    const cart = getCart();

    // If cart is empty redirect back
    if (cart.length === 0) {
      window.location.href = '/Cart';
      return;
    }

    renderCheckoutSummary(cart);
    attachCheckoutFormHandler();
  }

  function renderCheckoutSummary(cart) {
    checkoutSummaryItemsEl.innerHTML = cart.map(item => `
      <li class="checkout-summary__item">
        <img src="${esc(item.imageUrl)}" alt="${esc(item.name)}" />
        <div class="checkout-summary__item-info">
          <div class="checkout-summary__item-name">${esc(item.name)}</div>
          <div class="checkout-summary__item-qty">× ${item.quantity}</div>
        </div>
        <span class="checkout-summary__item-price">${formatPrice(item.price * item.quantity)}</span>
      </li>
    `).join('');

    const subtotal = calcSubtotal(cart);
    const shipping = calcShipping(subtotal);

    const subEl     = document.getElementById('checkout-subtotal');
    const shipEl    = document.getElementById('checkout-shipping');
    const shipRowEl = document.getElementById('checkout-shipping-row');
    const totalEl   = document.getElementById('checkout-total');

    if (subEl)     subEl.textContent = formatPrice(subtotal);
    if (shipEl)    shipEl.textContent = shipping === 0 ? 'Безкоштовно' : formatPrice(shipping);
    if (shipRowEl) shipRowEl.classList.toggle('checkout-summary__row--free', shipping === 0);
    if (totalEl)   totalEl.textContent = formatPrice(subtotal + shipping);
  }

  function attachCheckoutFormHandler() {
    const form = document.getElementById('checkout-form');
    if (!form) return;

    // Real-time validation: clear errors on input
    form.querySelectorAll('input').forEach(input => {
      input.addEventListener('input', () => {
        input.classList.remove('is-invalid');
        const errEl = document.getElementById('err-' + input.name);
        if (errEl) errEl.classList.remove('is-visible');
      });
    });

    form.addEventListener('submit', e => {
      e.preventDefault();
      if (!validateCheckoutForm(form)) return;

      const cart     = getCart();
      const subtotal = calcSubtotal(cart);
      const shipping = calcShipping(subtotal);

      const selectedPayment = form.querySelector('[name="payment"]:checked');

      /** @type {Order} */
      const order = {
        id:       makeOrderId(),
        date:     new Date().toISOString(),
        status:   'Прийнято',
        items:    cart.map(i => ({ ...i })),
        subtotal,
        shipping,
        total:    subtotal + shipping,
        payment:  selectedPayment ? selectedPayment.value : 'card',
        address: {
          name:    form.fullName.value.trim(),
          email:   form.email.value.trim(),
          phone:   form.phone.value.trim(),
          city:    form.city.value.trim(),
          address: form.address.value.trim(),
          postal:  form.postal.value.trim(),
        },
      };

      // Save order → persist in orders list
      const orders = getOrders();
      orders.unshift(order);
      saveOrders(orders);

      // Store id in sessionStorage for the confirm page
      sessionStorage.setItem('amazon-last-order', order.id);

      // Clear the cart
      saveCart([]);
      renderCartBadge();

      window.location.href = '/Checkout/Confirm';
    });
  }

  /** Returns true if form is valid, false otherwise. Shows inline errors. */
  function validateCheckoutForm(form) {
    const rules = [
      { name: 'fullName', msg: "Введіть ваше повне ім'я" },
      { name: 'email',    msg: 'Введіть коректну email-адресу', pattern: /^[^\s@]+@[^\s@]+\.[^\s@]+$/ },
      { name: 'phone',    msg: 'Введіть номер телефону' },
      { name: 'city',     msg: 'Введіть ваше місто' },
      { name: 'address',  msg: 'Введіть вулицю і номер будинку' },
      { name: 'postal',   msg: 'Введіть поштовий індекс' },
    ];

    let isValid = true;

    rules.forEach(({ name, msg, pattern }) => {
      const input  = form[name];
      const errEl  = document.getElementById('err-' + name);
      if (!input) return;

      const val        = input.value.trim();
      const empty      = val.length === 0;
      const badPattern = pattern && !empty && !pattern.test(val);
      const invalid    = empty || badPattern;

      input.classList.toggle('is-invalid', invalid);
      if (errEl) {
        errEl.textContent = msg;
        errEl.classList.toggle('is-visible', invalid);
      }
      if (invalid) isValid = false;
    });

    return isValid;
  }

  // ══════════════════════════════════════════════════════════════
  // CHECKOUT CONFIRM PAGE  (/Checkout/Confirm)
  // ══════════════════════════════════════════════════════════════

  const confirmContentEl = document.getElementById('confirm-content');

  if (confirmContentEl) {
    renderConfirmPage();
  }

  function renderConfirmPage() {
    const orderId = sessionStorage.getItem('amazon-last-order');

    if (!orderId) {
      // No pending order — redirect to home
      window.location.href = '/';
      return;
    }

    const order = getOrders().find(o => o.id === orderId);
    if (!order) {
      window.location.href = '/';
      return;
    }

    // Order ID
    const idEl = document.getElementById('confirm-order-id');
    if (idEl) idEl.textContent = order.id;

    // Date
    const dateEl = document.getElementById('confirm-date');
    if (dateEl) {
      const d = new Date(order.date);
      dateEl.textContent = d.toLocaleDateString('uk-UA', {
        day: '2-digit', month: 'long', year: 'numeric',
        hour: '2-digit', minute: '2-digit',
      });
    }

    // Payment
    const paymentMap = {
      card:    '💳 Оплата карткою',
      cash:    '💵 Готівкою при отриманні',
      digital: '📱 Apple / Google Pay',
    };
    const payEl = document.getElementById('confirm-payment');
    if (payEl) payEl.textContent = paymentMap[order.payment] || order.payment;

    // Address
    const addrEl = document.getElementById('confirm-address');
    if (addrEl && order.address) {
      const a = order.address;
      addrEl.innerHTML = `
        <strong>${esc(a.name)}</strong><br>
        ${esc(a.email)}<br>
        ${esc(a.phone)}<br>
        ${esc(a.city)}, ${esc(a.address)}<br>
        Індекс: ${esc(a.postal)}
      `;
    }

    // Items list
    const itemsEl = document.getElementById('confirm-items-list');
    if (itemsEl) {
      itemsEl.innerHTML = order.items.map(item => `
        <li class="confirm-item">
          <img src="${esc(item.imageUrl)}" alt="${esc(item.name)}" />
          <span class="confirm-item__name">${esc(item.name)}</span>
          <span class="confirm-item__qty">× ${item.quantity}</span>
          <span class="confirm-item__price">${formatPrice(item.price * item.quantity)}</span>
        </li>
      `).join('');
    }

    // Totals
    setTextById('confirm-subtotal', formatPrice(order.subtotal));
    setTextById('confirm-shipping', order.shipping === 0 ? 'Безкоштовно' : formatPrice(order.shipping));
    setTextById('confirm-total',    formatPrice(order.total));
  }

  function setTextById(id, text) {
    const el = document.getElementById(id);
    if (el) el.textContent = text;
  }

  // ══════════════════════════════════════════════════════════════
  // PROFILE PAGE  (/Profile)
  // ══════════════════════════════════════════════════════════════

  const ordersContainerEl = document.getElementById('orders-container');

  if (ordersContainerEl) {
    renderProfileOrders();
  }

  function renderProfileOrders() {
    const orders  = getOrders();
    const countEl = document.getElementById('orders-count');
    if (countEl) countEl.textContent = orders.length;

    if (orders.length === 0) {
      ordersContainerEl.innerHTML = `
        <div class="orders-empty">
          <div class="orders-empty__icon">📦</div>
          <p>Ви ще не зробили жодного замовлення.</p>
          <a class="amazon-button amazon-button--primary" href="/Product">
            Перейти до каталогу
          </a>
        </div>`;
      return;
    }

    const STATUS_CLASS = {
      'Прийнято':   'order-status--received',
      'В обробці':  'order-status--processing',
      'Відправлено':'order-status--shipped',
      'Доставлено': 'order-status--delivered',
    };

    ordersContainerEl.innerHTML = `
      <div class="orders-list">
        ${orders.map(order => {
          const d        = new Date(order.date);
          const dateStr  = d.toLocaleDateString('uk-UA', { day: '2-digit', month: 'long', year: 'numeric' });
          const cls      = STATUS_CLASS[order.status] || 'order-status--received';
          const thumbs   = order.items.map(i =>
            `<img class="order-card__item-thumb"
                  src="${esc(i.imageUrl)}"
                  alt="${esc(i.name)}"
                  title="${esc(i.name)} × ${i.quantity}" />`
          ).join('');
          const totalQty = order.items.reduce((s, i) => s + i.quantity, 0);

          return `
            <article class="order-card" data-order-id="${esc(order.id)}">
              <div class="order-card__header">
                <div class="order-card__meta">
                  <span class="order-card__meta-label">Замовлення</span>
                  <span class="order-card__meta-value">${esc(order.id)}</span>
                </div>
                <div class="order-card__meta">
                  <span class="order-card__meta-label">Дата</span>
                  <span class="order-card__meta-value">${dateStr}</span>
                </div>
                <div class="order-card__meta">
                  <span class="order-card__meta-label">Кількість</span>
                  <span class="order-card__meta-value">${totalQty} шт.</span>
                </div>
                <div class="order-card__status">
                  <span class="order-status ${cls}">${esc(order.status)}</span>
                </div>
              </div>

              <div class="order-card__body">
                <div class="order-card__items-preview">${thumbs}</div>
              </div>

              <div class="order-card__footer">
                <span class="order-card__total">
                  Сума: <span>${formatPrice(order.total)}</span>
                </span>
                <div class="order-card__actions">
                  <button class="btn-ghost" data-toggle-details="${esc(order.id)}">
                    Деталі
                  </button>
                </div>
              </div>
            </article>
          `;
        }).join('')}
      </div>`;

    // Attach toggle-details handlers
    document.querySelectorAll('[data-toggle-details]').forEach(btn => {
      btn.addEventListener('click', () => toggleOrderDetails(btn));
    });
  }

  /** Expand / collapse the details panel inside an order card */
  function toggleOrderDetails(btn) {
    const card    = btn.closest('.order-card');
    const orderId = btn.dataset.toggleDetails;
    const existing = card.querySelector('.order-card__details');

    if (existing) {
      existing.remove();
      btn.textContent = 'Деталі';
      return;
    }

    btn.textContent = 'Сховати';

    const order = getOrders().find(o => o.id === orderId);
    if (!order) return;

    const paymentMap = {
      card:    '💳 Карткою',
      cash:    '💵 Готівкою',
      digital: '📱 Apple/Google Pay',
    };

    const detailsEl = document.createElement('div');
    detailsEl.className = 'order-card__details';
    detailsEl.innerHTML = `
      <div class="order-details-grid">
        <div>
          <div class="order-details-label">Адреса доставки</div>
          <div class="order-details-text">
            <strong>${esc(order.address.name)}</strong><br>
            ${esc(order.address.email)}<br>
            ${esc(order.address.phone)}<br>
            ${esc(order.address.city)}, ${esc(order.address.address)}<br>
            Індекс: ${esc(order.address.postal)}
          </div>
        </div>
        <div>
          <div class="order-details-label">Спосіб оплати</div>
          <div class="order-details-text">${esc(paymentMap[order.payment] || order.payment)}</div>
        </div>
      </div>
      <ul class="order-details-items">
        ${order.items.map(item => `
          <li class="order-details-item">
            <img src="${esc(item.imageUrl)}" alt="${esc(item.name)}" />
            <span class="order-details-item__name">${esc(item.name)}</span>
            <span class="order-details-item__qty">× ${item.quantity}</span>
            <span class="order-details-item__price">${formatPrice(item.price * item.quantity)}</span>
          </li>
        `).join('')}
      </ul>
      <div class="order-details-total">
        <span>Разом</span>
        <span>${formatPrice(order.total)}</span>
      </div>
    `;

    card.querySelector('.order-card__footer').insertAdjacentElement('beforebegin', detailsEl);
  }

})();
