// ============================================
// FUNCIONES GLOBALES DEL SISTEMA
// ============================================

$(document).ready(function() {
    // Inicializar tooltips de Bootstrap
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Auto-cerrar alertas después de 5 segundos
    setTimeout(function() {
        $('.alert').fadeOut('slow');
    }, 5000);
});

// ============================================
// VALIDACIONES COMUNES
// ============================================

// Validar DNI (8 dígitos)
function validarDNI(dni) {
    const regex = /^\d{8}$/;
    return regex.test(dni);
}

// Validar Email
function validarEmail(email) {
    const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return regex.test(email);
}

// Validar Teléfono (9 dígitos)
function validarTelefono(telefono) {
    const regex = /^\d{9}$/;
    return regex.test(telefono);
}

// Validar solo letras
function validarSoloLetras(texto) {
    const regex = /^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$/;
    return regex.test(texto);
}

// Validar fecha no sea futura (para fecha de nacimiento)
function validarFechaNoFutura(fecha) {
    const fechaSeleccionada = new Date(fecha);
    const hoy = new Date();
    return fechaSeleccionada <= hoy;
}

// ============================================
// CONFIRMACIONES CON SWEETALERT2
// ============================================

// Confirmación de eliminación
function confirmarEliminacion(mensaje, callback) {
    Swal.fire({
        title: '¿Estás seguro?',
        text: mensaje || "No podrás revertir esta acción",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed && callback) {
            callback();
        }
    });
}

// Mensaje de éxito
function mostrarMensajeExito(titulo, mensaje) {
    Swal.fire({
        icon: 'success',
        title: titulo || '¡Éxito!',
        text: mensaje,
        timer: 3000,
        showConfirmButton: false
    });
}

// Mensaje de error
function mostrarMensajeError(titulo, mensaje) {
    Swal.fire({
        icon: 'error',
        title: titulo || 'Error',
        text: mensaje
    });
}

// Mensaje de información
function mostrarMensajeInfo(titulo, mensaje) {
    Swal.fire({
        icon: 'info',
        title: titulo,
        text: mensaje
    });
}

// ============================================
// LOADING OVERLAY
// ============================================

function mostrarCargando() {
    Swal.fire({
        title: 'Cargando...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
}

function ocultarCargando() {
    Swal.close();
}

// ============================================
// FORMATEO DE DATOS
// ============================================

// Formatear fecha a dd/MM/yyyy
function formatearFecha(fecha) {
    const date = new Date(fecha);
    const dia = String(date.getDate()).padStart(2, '0');
    const mes = String(date.getMonth() + 1).padStart(2, '0');
    const año = date.getFullYear();
    return `${dia}/${mes}/${año}`;
}

// Formatear hora a HH:mm
function formatearHora(hora) {
    if (!hora) return '';
    const partes = hora.split(':');
    return `${partes[0]}:${partes[1]}`;
}

// ============================================
// MANEJO DE TABLAS DATATABLES
// ============================================

// Configuración por defecto de DataTables en español
$.extend(true, $.fn.dataTable.defaults, {
    language: {
        url: '//cdn.datatables.net/plug-ins/1.13.7/i18n/es-ES.json'
    },
    responsive: true,
    pageLength: 10,
    lengthMenu: [[5, 10, 25, 50, -1], [5, 10, 25, 50, "Todos"]],
    dom: '<"row"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6"f>>' +
         '<"row"<"col-sm-12"tr>>' +
         '<"row"<"col-sm-12 col-md-5"i><"col-sm-12 col-md-7"p>>'
});

// Inicializar DataTable con opciones personalizadas
function inicializarDataTable(selector, opciones) {
    const opcionesDefecto = {
        language: {
            url: '//cdn.datatables.net/plug-ins/1.13.7/i18n/es-ES.json'
        },
        responsive: true,
        pageLength: 10
    };
    
    const opcionesFinales = $.extend({}, opcionesDefecto, opciones);
    return $(selector).DataTable(opcionesFinales);
}

// ============================================
// AJAX HELPERS
// ============================================

// Realizar petición GET con manejo de errores
function ajaxGet(url, successCallback, errorCallback) {
    $.ajax({
        url: url,
        type: 'GET',
        dataType: 'json',
        beforeSend: function() {
            mostrarCargando();
        },
        success: function(response) {
            ocultarCargando();
            if (successCallback) successCallback(response);
        },
        error: function(xhr, status, error) {
            ocultarCargando();
            if (errorCallback) {
                errorCallback(xhr, status, error);
            } else {
                mostrarMensajeError('Error', 'Error en la petición: ' + error);
            }
        }
    });
}

// Realizar petición POST con manejo de errores
function ajaxPost(url, data, successCallback, errorCallback) {
    $.ajax({
        url: url,
        type: 'POST',
        data: JSON.stringify(data),
        contentType: 'application/json',
        dataType: 'json',
        beforeSend: function() {
            mostrarCargando();
        },
        success: function(response) {
            ocultarCargando();
            if (successCallback) successCallback(response);
        },
        error: function(xhr, status, error) {
            ocultarCargando();
            if (errorCallback) {
                errorCallback(xhr, status, error);
            } else {
                mostrarMensajeError('Error', 'Error en la petición: ' + error);
            }
        }
    });
}

// Realizar petición DELETE
function ajaxDelete(url, successCallback, errorCallback) {
    $.ajax({
        url: url,
        type: 'POST',
        beforeSend: function() {
            mostrarCargando();
        },
        success: function(response) {
            ocultarCargando();
            if (response.success) {
                mostrarMensajeExito('¡Eliminado!', response.message);
                if (successCallback) successCallback(response);
            } else {
                mostrarMensajeError('Error', response.message);
            }
        },
        error: function(xhr, status, error) {
            ocultarCargando();
            if (errorCallback) {
                errorCallback(xhr, status, error);
            } else {
                mostrarMensajeError('Error', 'Error al eliminar: ' + error);
            }
        }
    });
}

// ============================================
// UTILIDADES
// ============================================

// Limpiar formulario
function limpiarFormulario(formId) {
    $(formId)[0].reset();
    $(formId).find('.is-invalid').removeClass('is-invalid');
    $(formId).find('.invalid-feedback').remove();
}

// Mostrar errores de validación
function mostrarErroresValidacion(form, errores) {
    // Limpiar errores previos
    $(form).find('.is-invalid').removeClass('is-invalid');
    $(form).find('.invalid-feedback').remove();
    
    // Mostrar nuevos errores
    $.each(errores, function(campo, mensajes) {
        const input = $(form).find(`[name="${campo}"]`);
        input.addClass('is-invalid');
        input.after(`<div class="invalid-feedback">${mensajes[0]}</div>`);
    });
}

// Capitalizar primera letra
function capitalizarPrimeraLetra(texto) {
    return texto.charAt(0).toUpperCase() + texto.slice(1).toLowerCase();
}

// Obtener parámetro de URL
function obtenerParametroURL(nombre) {
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get(nombre);
}
