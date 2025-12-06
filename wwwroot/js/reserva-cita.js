// ============================================
// PROCESO DE RESERVA DE CITAS (CARRITO)
// ============================================

let reservaActual = {
    idEspecialidad: null,
    nombreEspecialidad: null,
    idDoctor: null,
    nombreDoctor: null,
    fechaCita: null,
    horaCita: null,
    motivo: ''
};

$(document).ready(function() {
    inicializarReservaCita();
});

// ============================================
// INICIALIZACIÓN
// ============================================

function inicializarReservaCita() {
    // Paso 1: Cargar especialidades
    cargarEspecialidades();
    
    // Event listeners
    $('#especialidadSelect').on('change', seleccionarEspecialidad);
    $('#doctorSelect').on('change', seleccionarDoctor);
    $('#fechaCitaInput').on('change', cargarHorasDisponibles);
    $('#btnConfirmarReserva').on('click', confirmarReserva);
    
    // Deshabilitar pasos siguientes inicialmente
    deshabilitarPaso(2);
    deshabilitarPaso(3);
    deshabilitarPaso(4);
}

// ============================================
// PASO 1: SELECCIONAR ESPECIALIDAD
// ============================================

function cargarEspecialidades() {
    ajaxGet('/Especialidad/ObtenerEspecialidadesActivas', function(especialidades) {
        const select = $('#especialidadSelect');
        select.empty();
        select.append('<option value="">-- Seleccione una especialidad --</option>');
        
        especialidades.forEach(function(esp) {
            select.append(`<option value="${esp.idEspecialidad}">${esp.nombre}</option>`);
        });
    });
}

function seleccionarEspecialidad() {
    const idEspecialidad = $(this).val();
    const nombreEspecialidad = $(this).find('option:selected').text();
    
    if (idEspecialidad) {
        reservaActual.idEspecialidad = idEspecialidad;
        reservaActual.nombreEspecialidad = nombreEspecialidad;
        
        // Habilitar paso 2
        habilitarPaso(2);
        marcarPasoCompletado(1);
        
        // Cargar doctores de la especialidad
        cargarDoctoresPorEspecialidad(idEspecialidad);
    } else {
        deshabilitarPaso(2);
        deshabilitarPaso(3);
        deshabilitarPaso(4);
    }
}

// ============================================
// PASO 2: SELECCIONAR DOCTOR
// ============================================

function cargarDoctoresPorEspecialidad(idEspecialidad) {
    ajaxGet(`/Doctor/ObtenerDoctoresPorEspecialidad?idEspecialidad=${idEspecialidad}`, 
        function(doctores) {
            const select = $('#doctorSelect');
            select.empty();
            select.append('<option value="">-- Seleccione un doctor --</option>');
            
            if (doctores.length === 0) {
                mostrarMensajeInfo('Sin doctores', 'No hay doctores disponibles para esta especialidad');
                return;
            }
            
            doctores.forEach(function(doc) {
                select.append(`<option value="${doc.idDoctor}">
                    Dr(a). ${doc.nombres} ${doc.apellidos}
                </option>`);
            });
        });
}

function seleccionarDoctor() {
    const idDoctor = $(this).val();
    const nombreDoctor = $(this).find('option:selected').text();
    
    if (idDoctor) {
        reservaActual.idDoctor = idDoctor;
        reservaActual.nombreDoctor = nombreDoctor;
        
        // Habilitar paso 3
        habilitarPaso(3);
        marcarPasoCompletado(2);
        
        // Configurar fecha mínima (solo días futuros)
        const hoy = new Date();
        const mañana = new Date(hoy);
        mañana.setDate(mañana.getDate() + 1);
        const fechaMin = mañana.toISOString().split('T')[0];
        $('#fechaCitaInput').attr('min', fechaMin);
        
    } else {
        deshabilitarPaso(3);
        deshabilitarPaso(4);
    }
}

// ============================================
// PASO 3: SELECCIONAR FECHA Y HORA
// ============================================

function cargarHorasDisponibles() {
    const fecha = $(this).val();
    
    if (!fecha) return;
    
    reservaActual.fechaCita = fecha;
    
    // Consultar horas disponibles vía AJAX
    ajaxGet(`/Cita/ObtenerHorasDisponibles?idDoctor=${reservaActual.idDoctor}&fecha=${fecha}`,
        function(horas) {
            mostrarHorasDisponibles(horas);
        });
}

function mostrarHorasDisponibles(horas) {
    const contenedor = $('#horasDisponibles');
    contenedor.empty();
    
    if (horas.length === 0) {
        contenedor.html(`
            <div class="alert alert-warning">
                <i class="fas fa-info-circle"></i> 
                No hay horarios disponibles para esta fecha. Por favor, seleccione otra fecha.
            </div>
        `);
        deshabilitarPaso(4);
        return;
    }
    
    contenedor.html('<div class="row g-2"></div>');
    const row = contenedor.find('.row');
    
    horas.forEach(function(hora) {
        const horaFormateada = formatearHora(hora);
        row.append(`
            <div class="col-6 col-md-3">
                <button type="button" class="btn btn-outline-primary w-100 btn-hora" 
                        data-hora="${hora}">
                    <i class="fas fa-clock"></i> ${horaFormateada}
                </button>
            </div>
        `);
    });
    
    // Event listener para seleccionar hora
    $('.btn-hora').on('click', seleccionarHora);
}

function seleccionarHora() {
    const hora = $(this).data('hora');
    
    // Remover selección previa
    $('.btn-hora').removeClass('btn-primary').addClass('btn-outline-primary');
    
    // Marcar como seleccionado
    $(this).removeClass('btn-outline-primary').addClass('btn-primary');
    
    reservaActual.horaCita = hora;
    
    // Habilitar paso 4
    habilitarPaso(4);
    marcarPasoCompletado(3);
    
    // Mostrar resumen
    mostrarResumenReserva();
}

// ============================================
// PASO 4: CONFIRMAR RESERVA
// ============================================

function mostrarResumenReserva() {
    const resumen = `
        <div class="card border-primary">
            <div class="card-header bg-primary text-white">
                <h5 class="mb-0"><i class="fas fa-calendar-check"></i> Resumen de la Reserva</h5>
            </div>
            <div class="card-body">
                <dl class="row mb-0">
                    <dt class="col-sm-4">Especialidad:</dt>
                    <dd class="col-sm-8">${reservaActual.nombreEspecialidad}</dd>
                    
                    <dt class="col-sm-4">Doctor:</dt>
                    <dd class="col-sm-8">${reservaActual.nombreDoctor}</dd>
                    
                    <dt class="col-sm-4">Fecha:</dt>
                    <dd class="col-sm-8">${formatearFecha(reservaActual.fechaCita)}</dd>
                    
                    <dt class="col-sm-4">Hora:</dt>
                    <dd class="col-sm-8">${formatearHora(reservaActual.horaCita)}</dd>
                </dl>
            </div>
        </div>
    `;
    
    $('#resumenReserva').html(resumen);
}

function confirmarReserva() {
    const motivo = $('#motivoInput').val();
    
    if (!motivo || motivo.trim() === '') {
        mostrarMensajeError('Error', 'Por favor, indique el motivo de la consulta');
        return;
    }
    
    reservaActual.motivo = motivo;
    
    // Confirmar con el usuario
    Swal.fire({
        title: '¿Confirmar reserva?',
        html: `
            <p>Se creará una cita con los siguientes datos:</p>
            <ul class="text-start">
                <li><strong>Especialidad:</strong> ${reservaActual.nombreEspecialidad}</li>
                <li><strong>Doctor:</strong> ${reservaActual.nombreDoctor}</li>
                <li><strong>Fecha:</strong> ${formatearFecha(reservaActual.fechaCita)}</li>
                <li><strong>Hora:</strong> ${formatearHora(reservaActual.horaCita)}</li>
            </ul>
        `,
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#28a745',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Sí, confirmar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            guardarReserva();
        }
    });
}

function guardarReserva() {
    const datos = {
        idDoctor: reservaActual.idDoctor,
        fechaCita: reservaActual.fechaCita,
        horaCita: reservaActual.horaCita,
        motivo: reservaActual.motivo
    };
    
    ajaxPost('/Cita/CrearCita', datos, 
        function(response) {
            if (response.success) {
                Swal.fire({
                    icon: 'success',
                    title: '¡Reserva Confirmada!',
                    html: `
                        <p>Su cita ha sido registrada exitosamente.</p>
                        <p><strong>Código de cita:</strong> ${response.idCita}</p>
                    `,
                    confirmButtonText: 'Ver mis citas'
                }).then(() => {
                    window.location.href = '/Cita/MisCitas';
                });
            } else {
                mostrarMensajeError('Error', response.message);
            }
        },
        function(xhr, status, error) {
            mostrarMensajeError('Error', 'No se pudo crear la reserva. Por favor, intente nuevamente.');
        }
    );
}

// ============================================
// FUNCIONES AUXILIARES
// ============================================

function habilitarPaso(paso) {
    $(`#paso${paso}`).removeClass('disabled');
    $(`#paso${paso} .card`).removeClass('border-secondary').addClass('border-primary');
}

function deshabilitarPaso(paso) {
    $(`#paso${paso}`).addClass('disabled');
    $(`#paso${paso} .card`).removeClass('border-primary').addClass('border-secondary');
}

function marcarPasoCompletado(paso) {
    $(`#paso${paso} .badge`).removeClass('bg-secondary').addClass('bg-success');
    $(`#paso${paso} .badge i`).removeClass('fa-circle').addClass('fa-check-circle');
}

// ============================================
// VERIFICACIÓN DE DISPONIBILIDAD EN TIEMPO REAL
// ============================================

function verificarDisponibilidad(idDoctor, fecha, hora) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: '/Cita/VerificarDisponibilidad',
            type: 'POST',
            data: JSON.stringify({ idDoctor, fecha, hora }),
            contentType: 'application/json',
            success: function(response) {
                resolve(response.disponible);
            },
            error: function() {
                reject(false);
            }
        });
    });
}
