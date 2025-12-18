// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Page Index : Création select pour le tri des tâches
const tableBodyTaskIndex = document.querySelector("#body-table-index");
const modalEdit = document.getElementById('modalEdit');


const selectSort = document.querySelector("#sortList");
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
    selectSort.addEventListener('change', () => {
        const fieldToSort = selectSort.value;
        const sortingOrder = selectSort.options[selectSort.selectedIndex].getAttribute("data-order");

        fetch(`Tasks/GetSortedTasks?column=${fieldToSort}&order=${sortingOrder}`)
            .then(response => { return response.text() })
            .then(html => tableBodyTaskIndex.innerHTML = html);
    })
    function setSortingOptionList(field, order) {
        return `<option value="${field.champ}" data-order="${order}">${field.libelle} (${order})</option>`;
    }

}


// Page Index : Gestion de l'évènement sur checkbox pour Statut
document.querySelectorAll(".statut-checkbox").forEach(checkbox => {
    checkbox.addEventListener('change', (e) => {
        const id = e.target.getAttribute("data-id");
        const statut = e.target.checked;
        // Solution 1: La plus simple
        //fetch(`Tasks/ToggleStatut?id=${id}&statut=${statut}`, {
        //    method: 'POST'
        //});
        // Solution 1 bis
        //fetch('Tasks/ToggleStatut', {
        //    method: 'POST',
        //    headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        //    body: `id=${id}&statut=${statut}`
        //});
        // Solution 2 : La plus élégante (lais necessite un DTO)
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