document.addEventListener("DOMContentLoaded", function () {
    // Función para cargar provincias según departamento
    function cargarProvincias(idDepartamento, selectProvincia, selectDistrito, idProvinciaSeleccionada = null, callback = null) {
        fetch(`/Trabajador/ProvinciasPorDepartamento/${idDepartamento}`)
            .then(response => response.json())
            .then(data => {
                selectProvincia.innerHTML = '<option value="">Seleccione</option>';
                selectDistrito.innerHTML = '<option value="">Seleccione</option>';

                data.forEach(p => {
                    const selected = (p.id == idProvinciaSeleccionada) ? 'selected' : '';
                    selectProvincia.innerHTML += `<option value="${p.id}" ${selected}>${p.nombreProvincia}</option>`;
                });

                if (callback) callback(); // Llama a los distritos luego
            });
    }

    // Función para cargar distritos según provincia
    function cargarDistritos(idProvincia, selectDistrito, idDistritoSeleccionado = null) {
        fetch(`/Trabajador/DistritosPorProvincia/${idProvincia}`)
            .then(response => response.json())
            .then(data => {
                selectDistrito.innerHTML = '<option value="">Seleccione</option>';
                data.forEach(d => {
                    const selected = (d.id == idDistritoSeleccionado) ? 'selected' : '';
                    selectDistrito.innerHTML += `<option value="${d.id}" ${selected}>${d.nombreDistrito}</option>`;
                });
            });
    }

    // REGISTRAR - NUEVO trabajador
    const depSelectNuevo = document.getElementById("DepartamentoSelect");
    const provSelectNuevo = document.getElementById("ProvinciaSelect");
    const distSelectNuevo = document.getElementById("DistritoSelect");

    if (depSelectNuevo && provSelectNuevo && distSelectNuevo) {
        depSelectNuevo.addEventListener("change", function () {
            cargarProvincias(this.value, provSelectNuevo, distSelectNuevo);
        });

        provSelectNuevo.addEventListener("change", function () {
            cargarDistritos(this.value, distSelectNuevo);
        });
    }

    // EDITAR - POR CADA trabajador
    document.querySelectorAll('[id^="editarModal-"]').forEach(function (modal) {
        modal.addEventListener('shown.bs.modal', function () {
            const id = modal.id.split('-')[1];

            const depSelect = modal.querySelector(`.departamento-editar[data-id="${id}"]`);
            const provSelect = modal.querySelector(`.provincia-editar[data-id="${id}"]`);
            const distSelect = modal.querySelector(`.distrito-editar[data-id="${id}"]`);

            const idDepartamento = depSelect.value;
            const idProvincia = depSelect.getAttribute('data-provincia');
            const idDistrito = depSelect.getAttribute('data-distrito');

            cargarProvincias(idDepartamento, provSelect, distSelect, idProvincia, () => {
                cargarDistritos(idProvincia, distSelect, idDistrito);
            });

            depSelect.addEventListener("change", function () {
                cargarProvincias(this.value, provSelect, distSelect);
            });

            provSelect.addEventListener("change", function () {
                cargarDistritos(this.value, distSelect);
            });
        });
    });
});
//Cuadro de confirmación para elimar un Trabajador
async function eliminarTrabajador(trabajadorId) {
    if (!confirm("¿Estás seguro de que quieres eliminar este trabajador?")) {
        return;
    }
//Control de errores con TryCatch
    try {
        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

        const response = await fetch(`/Trabajador/Delete/${trabajadorId}`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "RequestVerificationToken": token
            }
        });

        const result = await response.json();

        if (result.success) {
            const fila = document.getElementById(`row-${trabajadorId}`);
            fila.remove();
            alert(result.message);
        } else {
            alert(result.message);
        }
    } catch (error) {
        console.error("Error en la solicitud:", error);
        alert("Error en la solicitud. Revisa la consola para más detalles.");
    }
}
//Filtrado de Sexo, según elija el usuario se pintará de un color o no
document.getElementById('filtroSexo').addEventListener('change', function () {
    const sexo = this.value;

    fetch(`/Trabajador/FiltrarPorSexo?sexo=${sexo}`)
        .then(response => {
            if (!response.ok) throw new Error('Error en la red');
            return response.json();
        })
        .then(data => {
            const tbody = document.querySelector('table tbody');
            tbody.innerHTML = '';

            data.forEach(t => {
                const row = document.createElement('tr');
                row.id = `row-${t.id}`;
                row.dataset.sexo = t.sexo;

                // Clase de color basada en el sexo
                row.classList.add(
                    t.sexo === "M" ? 'table-row-m' : 'table-row-f',
                );
                row.innerHTML = `
                        <td>${t.tipoDocumento}</td>
                        <td>${t.numeroDocumento}</td>
                        <td>${t.nombres}</td>
                        <td>${t.sexo}</td>
                        <td>${t.nombreDepartamento}</td>
                        <td>${t.nombreProvincia}</td>
                        <td>${t.nombreDistrito}</td>
                        <td>
                            <button class="btn btn-sm btn-primary" data-bs-toggle="modal" 
                                data-bs-target="#editarModal-${t.id}">Editar</button>
                            <button class="btn btn-sm btn-danger" onclick="eliminarTrabajador(${t.id})">
                                Eliminar
                            </button>
                        </td>
                    `;

                tbody.appendChild(row);
            });
        })
        .catch(error => {
            console.error('Error:', error);
            alert('Error al filtrar trabajadores');
        });
});