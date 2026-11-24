document.addEventListener("DOMContentLoaded", function () {
    const searchContainer = document.querySelector(".navbar-search");

    if (!searchContainer) {
        return;
    }

    const searchInput = searchContainer.querySelector("input[type='search']");
    const searchButton = searchContainer.querySelector(".navbar-search-button");

    if (!searchInput) {
        return;
    }

    const controller = (searchContainer.dataset.controller || "").toLowerCase();
    const area = (searchContainer.dataset.area || "").toLowerCase();
    const pageKey = area ? `${area}:${controller}` : controller;

    const selectorsByController = {
        home: ".popular-card, .week-card",
        "admin:home": ".admin-card",
        dishes: ".dish-grid .dish-card",
        dailymenus: ".day-panel .meal-row",
        weeklymenus: ".week-panel .week-meal-card",
        shoppinglist: ".shopping-row:not(.shopping-row-head)",
        pantry: "#pantryTableBody tr",
        usermanagement: ".users-table tbody tr",
        shareddishes: ".shared-card tbody tr"
    };

    function getSearchItems() {
        const selector = selectorsByController[pageKey];

        if (!selector) {
            return [];
        }

        return Array.from(document.querySelectorAll(selector));
    }

    function applySearch() {
        if (controller === "meals") {
            return;
        }

        const query = searchInput.value.trim().toLowerCase();
        const items = getSearchItems();

        items.forEach(function (item) {
            const text = item.textContent.toLowerCase();
            item.style.display = !query || text.includes(query) ? "" : "none";
        });

        if (controller === "dishes") {
            const pagination = document.querySelector(".pagination-custom");

            if (pagination) {
                pagination.style.display = query ? "none" : "";
            }
        }
    }

    searchInput.addEventListener("input", applySearch);

    searchInput.addEventListener("keydown", function (event) {
        if (event.key === "Escape") {
            searchInput.value = "";
            applySearch();
        }
    });

    if (searchButton) {
        searchButton.addEventListener("click", function () {
            searchInput.focus();
        });
    }

    if (controller === "pantry") {
        const pantryTableBody = document.getElementById("pantryTableBody");

        if (pantryTableBody) {
            const observer = new MutationObserver(function () {
                if (searchInput.value.trim()) {
                    applySearch();
                }
            });

            observer.observe(pantryTableBody, {
                childList: true
            });
        }
    }
});
