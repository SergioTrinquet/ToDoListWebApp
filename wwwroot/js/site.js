// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Page /Tasks (Tasks/Index.cshtml)
const tableBodyTaskIndex = document.querySelector("#body-table-index");
const modalEdit = document.getElementById('modalEdit');
const filterInput = document.querySelector("input[name='filterInput']");
const displayWitnessTasks = document.querySelector("#toggle-author-tasks input[type='checkbox']");
const selectSort = document.querySelector("#sortList");
const minCharToFilter = 3;

if (displayWitnessTasks) {
    displayWitnessTasks.addEventListener('change', () => fetchTasks());
}

// Select pour le tri des tâches
if (selectSort) { 
    const listFieldToSort = [
        { champ: "Nom", libelle: "Tâche" },
        { champ: "Priorite", libelle: "Priorité" }
    ]; // Champs dans le Model 'Task' qu'il doit être possible de classer
    const orderSorting = ["asc", "desc"];

    listFieldToSort.forEach(f => {
        orderSorting.forEach(o => {
            const option = setSortingOptionList(f, o);
            selectSort.insertAdjacentHTML('beforeend', option);
        });
    })

    // Sélection filtre
    selectSort.addEventListener('change', () => fetchTasks());
    function setSortingOptionList(field, order) {
        return `<option value="${field.champ}" data-order="${order}">${field.libelle} (${order})</option>`;
    }

}

// Filtre
if (filterInput) { 
    let resetLastValue = null;
    let forbiddenKey = null;
    const timeDebounce = 500;
    const debouncedFetchTasks = debounce(fetchTasks, timeDebounce);

    filterInput.addEventListener('keydown', e => {
        forbiddenKey = false;
        if (e.code === 'Space' || e.key === ' ') forbiddenKey = true;
    });
    filterInput.addEventListener('input', e => {
        if (!forbiddenKey) {
            const lengthValue = e.target.value.trim().length;
            const resetFilter = lengthValue < minCharToFilter;
            e.target.classList[resetFilter && lengthValue !== 0  ? "add" : "remove"]("is-invalid");
            if (resetFilter && resetFilter === resetLastValue) return;
            resetLastValue = resetFilter;
            debouncedFetchTasks();
        }
    })
    document.querySelector("#bt-delete-filter").addEventListener("click", () => {
        filterInput.value = "";
        filterInput.dispatchEvent(new Event("input"));
    })
    function debounce(func, delay) {
        console.log(func, delay)
        let timerId;
        return function (...args) {
            clearTimeout(timerId);
            timerId = setTimeout(() => func.apply(this, args), delay);
        };
    }

}

// Pour rendre éléments de filtres, tri et bouton sticky quand scroll down, seulement pour pg 'Tasks'
if (displayWitnessTasks || selectSort) {
    window.addEventListener("scroll", () => {
        const scrollY = window.scrollY || window.pageYOffset;
        document.querySelector("main").classList.toggle("sticky-header-controls", (scrollY > 80));
    })
}


function getDataSortAndFilter() {
    return {
        "fieldToSort": selectSort?.value || "",
        "sortingOrder": selectSort?.options[selectSort.selectedIndex].getAttribute("data-order"),
        "filterInputValue": filterInput?.value.trim() || "",
        "displayWitnessTasks": displayWitnessTasks ? displayWitnessTasks.checked : false
    }
}
function fetchTasks() {
    const data = getDataSortAndFilter();

    data.filterInputValue = (data.filterInputValue.length < minCharToFilter ? "" : data.filterInputValue); // Cas ou saisie filter inf. à 3

    fetch(`Tasks/GetSortedAndFilteredTasks?column=${data.fieldToSort}&order=${data.sortingOrder}&filter=${data.filterInputValue}&displayDemoTasks=${data.displayWitnessTasks}`)
        .then(response => { return response.text() })
        .then(html => {
            tableBodyTaskIndex.innerHTML = html;
            setNbTasksLabel();
        });
}

function setNbTasksLabel() {
    const nbTasks = tableBodyTaskIndex.querySelector("[data-nb-tasks]")?.dataset.nbTasks || 0;
    document.querySelector("#nb-tasks").innerHTML = (nbTasks > 0 ? `<span>${nbTasks}</span> tâche${nbTasks > 1 ? "s" : ""}` : "");
}

document.addEventListener("DOMContentLoaded", e => setNbTasksLabel());


// Page Index : Gestion de l'évènement sur checkbox pour Statut
document.querySelectorAll(".statut-checkbox").forEach(checkbox => {
    checkbox.addEventListener('change', (e) => {
        const id = e.target.getAttribute("data-id");
        const statut = e.target.checked;
        fetch('Tasks/ToggleStatut', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ id, statut })
        });
        
    })
});


// Appel PartialView pour 'Edit'
if (tableBodyTaskIndex) {
    tableBodyTaskIndex.addEventListener("click", e => {
        if (!e.target.classList.contains("edit-task")) return;

        e.preventDefault();
        const id = e.target.getAttribute("data-id");

        fetch(`/Tasks/Edit/${id}`)
            .then(response => {
                return response.text()
            })
            .then(html => {
                modalEdit.innerHTML = html;
                const modal = new bootstrap.Modal(document.getElementById("editTaskModal"));
                modal.show();
            });
    })
}


document.addEventListener("submit", async e => {
    const form = e.target;
    if (form.id !== "editTaskForm") return;

    e.preventDefault(); // Empeche validation classique avec reload de la page

    const formData = new FormData(form);

    const response = await fetch(form.action, {
        method: "POST",
        headers: {
            "X-Requested-With": "XMLHttpRequest"
        },
        body: formData
    });

    const contentType = response.headers.get("content-type");
    // Selon action POST 'Edit' dans le Controller : 
    // si retour est du HTML => Erreur...
    if (contentType && contentType.includes("text/html")) {
        const html = await response.text();
        document.querySelector(".modal-backdrop").remove(); // Sinon accumule les backdrops
        modalEdit.innerHTML = html;
        const modalEl = document.getElementById("editTaskModal");
        new bootstrap.Modal(modalEl).show();

    // ...Si retour est du JSON => Succès
    } else {
        const json = await response.json();
        if (json.success) {
            const modalEl = document.getElementById("editTaskModal");
            bootstrap.Modal.getInstance(modalEl).hide();

            location.reload();
        }
    }

});