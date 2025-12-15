// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Page Index
const listFieldToSort = [
    { champ: "Nom", libelle: "Tâche" },
    { champ: "Priorite", libelle: "Priorité" }
]; // Champs dans le Model 'Task' qu'il doit être possible de classer
const orderSorting = ["asc", "desc"];
const selectSort = document.querySelector("#sortList");

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
        .then(html => document.querySelector("#body-table-index").innerHTML = html);
})
function setSortingOptionList(field, order) {
    return `<option value="${field.champ}" data-order="${order}">${field.libelle} (${order})</option>`;
}
