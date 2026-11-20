let measurementUnits = [];
let pantryItems = [];
let ingredientSuggestions = [];
let ingredientSearchTimeout;

document.addEventListener("DOMContentLoaded", async () => {

    await loadMeasurementUnits();
    await loadPantry();

    document
        .getElementById("ingredientName")
        .addEventListener(
            "input",
            searchIngredients);

    document
        .getElementById("addPantryForm")
        .addEventListener("submit", addPantryItem);

    document
        .getElementById("saveEditButton")
        .addEventListener("click", savePantryItem);
});


function getAntiForgeryToken() {

    return document
        .querySelector(
            'input[name="__RequestVerificationToken"]')
        .value;
}


async function loadMeasurementUnits() {

    const response =
        await fetch("/api/pantry/units");


    if (!response.ok) {

        showError(
            "Could not load measurement units.");

        return;
    }


    measurementUnits =
        await response.json();


    populateUnitSelect(
        document.getElementById("unit"));

    populateUnitSelect(
        document.getElementById("editUnit"));
}


function populateUnitSelect(selectElement) {

    selectElement.replaceChildren();


    measurementUnits.forEach(unit => {

        const option =
            document.createElement("option");

        option.value =
            unit.id;

        option.textContent =
            unit.name;

        selectElement.appendChild(option);
    });
}


async function loadPantry() {

    const response =
        await fetch("/api/pantry");


    if (!response.ok) {

        showError(
            "Could not load your pantry.");

        return;
    }


    pantryItems =
        await response.json();


    renderPantry();
}


function renderPantry() {

    const tableBody =
        document.getElementById(
            "pantryTableBody");

    const tableContainer =
        document.getElementById(
            "pantryTableContainer");

    const emptyPantry =
        document.getElementById(
            "emptyPantry");


    tableBody.replaceChildren();


    if (pantryItems.length === 0) {

        tableContainer
            .classList
            .add("d-none");

        emptyPantry
            .classList
            .remove("d-none");

        return;
    }


    tableContainer
        .classList
        .remove("d-none");

    emptyPantry
        .classList
        .add("d-none");


    pantryItems.forEach(item => {

        const row =
            document.createElement("tr");


        const ingredientCell =
            document.createElement("td");

        ingredientCell.textContent =
            item.ingredientName;


        const quantityCell =
            document.createElement("td");

        quantityCell.textContent =
            item.quantity;


        const unitCell =
            document.createElement("td");

        unitCell.textContent =
            getUnitName(item.unit);


        const actionsCell =
            document.createElement("td");

        actionsCell.className =
            "text-end";


        const editButton =
            document.createElement("button");

        editButton.type =
            "button";

        editButton.className =
            "btn btn-sm btn-outline-primary me-2";

        editButton.textContent =
            "Edit";

        editButton.addEventListener(
            "click",
            () => openEditModal(item.id));


        const deleteButton =
            document.createElement("button");

        deleteButton.type =
            "button";

        deleteButton.className =
            "btn btn-sm btn-outline-danger";

        deleteButton.textContent =
            "Delete";

        deleteButton.addEventListener(
            "click",
            () => deletePantryItem(item.id));


        actionsCell.append(
            editButton,
            deleteButton);


        row.append(
            ingredientCell,
            quantityCell,
            unitCell,
            actionsCell);


        tableBody.appendChild(row);
    });
}


function getUnitName(unitId) {

    const unit =
        measurementUnits.find(
            u => u.id === unitId);


    return unit
        ? unit.name
        : "";
}


function searchIngredients(event) {

    clearTimeout(
        ingredientSearchTimeout);


    const search =
        event
            .target
            .value
            .trim();


    if (search.length === 0) {

        ingredientSuggestions = [];

        document
            .getElementById(
                "ingredientSuggestions")
            .replaceChildren();

        return;
    }


    ingredientSearchTimeout =
        setTimeout(
            () =>
                loadIngredientSuggestions(
                    search),
            250);
}


async function loadIngredientSuggestions(search) {

    const response =
        await fetch(
            `/api/pantry/ingredients?search=${encodeURIComponent(search)}`);


    if (!response.ok) {

        showError(
            "Could not load ingredient suggestions.");

        return;
    }


    ingredientSuggestions =
        await response.json();


    const datalist =
        document.getElementById(
            "ingredientSuggestions");


    datalist.replaceChildren();


    ingredientSuggestions.forEach(ingredient => {

        const option =
            document.createElement("option");

        option.value =
            ingredient.name;

        datalist.appendChild(option);
    });
}


async function addPantryItem(event) {

    event.preventDefault();


    const ingredientName =
        document
            .getElementById(
                "ingredientName")
            .value
            .trim();


    const selectedIngredient =
        ingredientSuggestions
            .find(i =>
                i.name.toLowerCase() ===
                ingredientName.toLowerCase());


    if (!selectedIngredient) {

        showError(
            "Please select an ingredient from the suggestions.");

        return;
    }


    const request = {

        ingredientId:
            selectedIngredient.id,

        quantity:
            Number(
                document
                    .getElementById(
                        "quantity")
                    .value),

        unit:
            Number(
                document
                    .getElementById(
                        "unit")
                    .value)
    };


    const response =
        await fetch(
            "/api/pantry",
            {
                method: "POST",

                headers: {
                    "Content-Type":
                        "application/json",

                    "RequestVerificationToken":
                        getAntiForgeryToken()
                },

                body:
                    JSON.stringify(request)
            });


    if (!response.ok) {

        await showApiErrors(response);

        return;
    }


    document
        .getElementById(
            "addPantryForm")
        .reset();


    ingredientSuggestions = [];

    document
        .getElementById(
            "ingredientSuggestions")
        .replaceChildren();


    showSuccess(
        "Ingredient added to your pantry.");


    await loadPantry();
}


function openEditModal(id) {

    const item =
        pantryItems.find(
            p => p.id === id);


    if (!item) {
        return;
    }


    document
        .getElementById(
            "editPantryItemId")
        .value =
        item.id;


    document
        .getElementById(
            "editIngredientName")
        .value =
        item.ingredientName;


    document
        .getElementById(
            "editQuantity")
        .value =
        item.quantity;


    document
        .getElementById(
            "editUnit")
        .value =
        item.unit;


    const modal =
        new bootstrap.Modal(
            document.getElementById(
                "editPantryModal"));


    modal.show();
}


async function savePantryItem() {

    const id =
        Number(
            document
                .getElementById(
                    "editPantryItemId")
                .value);


    const request = {

        quantity:
            Number(
                document
                    .getElementById(
                        "editQuantity")
                    .value),

        unit:
            Number(
                document
                    .getElementById(
                        "editUnit")
                    .value)
    };


    const response =
        await fetch(
            `/api/pantry/${id}`,
            {
                method: "PUT",

                headers: {
                    "Content-Type":
                        "application/json",

                    "RequestVerificationToken":
                        getAntiForgeryToken()
                },

                body:
                    JSON.stringify(request)
            });


    if (!response.ok) {

        await showApiErrors(response);

        return;
    }


    const modalElement =
        document.getElementById(
            "editPantryModal");


    bootstrap
        .Modal
        .getInstance(modalElement)
        .hide();


    showSuccess(
        "Pantry item updated.");


    await loadPantry();
}


async function deletePantryItem(id) {

    const confirmed =
        confirm(
            "Are you sure you want to remove this item from your pantry?");


    if (!confirmed) {
        return;
    }


    const response =
        await fetch(
            `/api/pantry/${id}`,
            {
                method: "DELETE",

                headers: {
                    "RequestVerificationToken":
                        getAntiForgeryToken()
                }
            });


    if (!response.ok) {

        await showApiErrors(response);

        return;
    }


    showSuccess(
        "Pantry item removed.");


    await loadPantry();
}


async function showApiErrors(response) {

    let data;


    try {

        data =
            await response.json();
    }
    catch {

        showError(
            "An unexpected error occurred.");

        return;
    }


    if (Array.isArray(data.errors)) {

        showError(
            data.errors.join(" "));

        return;
    }


    if (data.errors &&
        typeof data.errors === "object") {

        const errors =
            Object
                .values(data.errors)
                .flat();


        showError(
            errors.join(" "));

        return;
    }


    showError(
        "An unexpected error occurred.");
}


function showSuccess(message) {

    showAlert(
        message,
        "success");
}


function showError(message) {

    showAlert(
        message,
        "danger");
}


function showAlert(
    message,
    type) {

    const container =
        document.getElementById(
            "pantryAlert");


    const alert =
        document.createElement("div");


    alert.className =
        `alert alert-${type} alert-dismissible fade show`;


    const text =
        document.createElement("span");

    text.textContent =
        message;


    const closeButton =
        document.createElement("button");

    closeButton.type =
        "button";

    closeButton.className =
        "btn-close";

    closeButton.setAttribute(
        "data-bs-dismiss",
        "alert");


    alert.append(
        text,
        closeButton);


    container.replaceChildren(alert);
}