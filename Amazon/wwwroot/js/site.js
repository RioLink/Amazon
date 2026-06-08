const cartStorageKey = "amazon-clone-cart-count";
const cartCount = document.querySelector(".cart-count");
const addToCartButtons = document.querySelectorAll("[data-add-to-cart]");

function getCartCount() {
    return Number.parseInt(localStorage.getItem(cartStorageKey) ?? "0", 10);
}

function renderCartCount(value) {
    if (cartCount) {
        cartCount.textContent = value.toString();
    }
}

renderCartCount(getCartCount());

addToCartButtons.forEach((button) => {
    button.addEventListener("click", () => {
        const nextCount = getCartCount() + 1;

        localStorage.setItem(cartStorageKey, nextCount.toString());
        renderCartCount(nextCount);

        button.classList.add("is-added");
        window.setTimeout(() => button.classList.remove("is-added"), 700);
    });
});
