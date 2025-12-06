// ============================================
// VALIDACIONES DEL LADO DEL CLIENTE
// ============================================

$(document).ready(function() {
    // Aplicar validaciones automáticas
    aplicarValidacionesFormularios();
});

// ============================================
// CONFIGURACIÓN GENERAL DE VALIDACIONES
// ============================================

function aplicarValidacionesFormularios() {
    // Validación de formularios con clase 'needs-validation'
    const forms = document.querySelectorAll('.needs-validation');
    
    Array.prototype.slice.call(forms).forEach(function(form) {
        form.addEventListener('submit', function(event) {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            }
            form.classList.add('was-validated');
        }, false);
    });
    
    // Validaciones personalizadas en tiempo real
    aplicarValidacionDNI();
    aplicarValidacionEmail();
    aplicarValidacionTelefono();
    aplicarValidacionSoloLetras();
    aplicarValidacionFechaNacimiento();
}

// ============================================
// VALIDACIÓN DE DNI
// ============================================

function aplicarValidacionDNI() {
    $('input[name="DNI"], input[data-tipo="dni"]').on('input', function() {
        const valor = $(this).val();
        const esValido = /^\d{0,8}$/.test(valor);
        
        if (!esValido) {
            $(this).val(valor.slice(0, -1));
            return;
        }
        
        if (valor.length === 8) {
            validarCampoDNI($(this));
        } else if (valor.length > 0) {
            marcarCampoInvalido($(this), 'El DNI debe tener 8 dígitos');
        } else {
            limpiarValidacion($(this));
        }
    });
}

function validarCampoDNI(campo) {
    const dni = campo.val();
    
    if (validarDNI(dni)) {
        marcarCampoValido(campo);
    } else {
        marcarCampoInvalido(campo, 'DNI inválido. Debe tener 8 dígitos');
    }
}

// ============================================
// VALIDACIÓN DE EMAIL
// ============================================

function aplicarValidacionEmail() {
    $('input[type="email"], input[data-tipo="email"]').on('blur', function() {
        const email = $(this).val();
        
        if (!email) {
            limpiarValidacion($(this));
            return;
        }
        
        if (validarEmail(email)) {
            marcarCampoValido($(this));
        } else {
            marcarCampoInvalido($(this), 'Email inválido. Ejemplo: usuario@correo.com');
        }
    });
}

// ============================================
// VALIDACIÓN DE TELÉFONO
// ============================================

function aplicarValidacionTelefono() {
    $('input[name="Telefono"], input[data-tipo="telefono"]').on('input', function() {
        const valor = $(this).val();
        const esValido = /^\d{0,9}$/.test(valor);
        
        if (!esValido) {
            $(this).val(valor.slice(0, -1));
            return;
        }
        
        if (valor.length === 9) {
            marcarCampoValido($(this));
        } else if (valor.length > 0) {
            marcarCampoInvalido($(this), 'El teléfono debe tener 9 dígitos');
        } else {
            limpiarValidacion($(this));
        }
    });
}

// ============================================
// VALIDACIÓN SOLO LETRAS (NOMBRES)
// ============================================

function aplicarValidacionSoloLetras() {
    $('input[data-tipo="solo-letras"]').on('input', function() {
        const valor = $(this).val();
        
        // Permitir solo letras, espacios y acentos
        if (!/^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]*$/.test(valor)) {
            $(this).val(valor.slice(0, -1));
            marcarCampoInvalido($(this), 'Solo se permiten letras');
        } else if (valor.length > 0) {
            marcarCampoValido($(this));
        } else {
            limpiarValidacion($(this));
        }
    });
}

// ============================================
// VALIDACIÓN FECHA DE NACIMIENTO
// ============================================

function aplicarValidacionFechaNacimiento() {
    $('input[name="FechaNacimiento"], input[data-tipo="fecha-nacimiento"]').on('change', function() {
        const fecha = $(this).val();
        
        if (!fecha) {
            limpiarValidacion($(this));
            return;
        }
        
        const fechaNacimiento = new Date(fecha);
        const hoy = new Date();
        const edad = calcularEdad(fechaNacimiento);
        
        if (fechaNacimiento > hoy) {
            marcarCampoInvalido($(this), 'La fecha de nacimiento no puede ser futura');
        } else if (edad > 120) {
            marcarCampoInvalido($(this), 'Fecha de nacimiento inválida');
        } else if (edad < 0) {
            marcarCampoInvalido($(this), 'Fecha de nacimiento inválida');
        } else {
            marcarCampoValido($(this));
            // Mostrar edad calculada si existe un elemento para ello
            const edadDisplay = $('[data-edad-display]');
            if (edadDisplay.length) {
                edadDisplay.text(`${edad} años`);
            }
        }
    });
}

// ============================================
// VALIDACIÓN DE CONTRASEÑAS
// ============================================

function validarContraseñas(inputPassword1, inputPassword2) {
    const password1 = $(inputPassword1).val();
    const password2 = $(inputPassword2).val();
    
    if (password1.length < 6) {
        marcarCampoInvalido($(inputPassword1), 'La contraseña debe tener al menos 6 caracteres');
        return false;
    }
    
    if (password1 !== password2) {
        marcarCampoInvalido($(inputPassword2), 'Las contraseñas no coinciden');
        return false;
    }
    
    marcarCampoValido($(inputPassword1));
    marcarCampoValido($(inputPassword2));
    return true;
}

// ============================================
// VALIDACIÓN NUMÉRICA
// ============================================

function aplicarValidacionNumerica(selector, min, max) {
    $(selector).on('input', function() {
        let valor = $(this).val();
        
        // Permitir solo números
        valor = valor.replace(/[^0-9]/g, '');
        $(this).val(valor);
        
        const numero = parseInt(valor);
        
        if (valor && (numero < min || numero > max)) {
            marcarCampoInvalido($(this), `El valor debe estar entre ${min} y ${max}`);
        } else if (valor) {
            marcarCampoValido($(this));
        } else {
            limpiarValidacion($(this));
        }
    });
}

// ============================================
// FUNCIONES AUXILIARES DE VALIDACIÓN
// ============================================

function marcarCampoValido(campo) {
    campo.removeClass('is-invalid').addClass('is-valid');
    campo.siblings('.invalid-feedback').hide();
    campo.siblings('.valid-feedback').show();
}

function marcarCampoInvalido(campo, mensaje) {
    campo.removeClass('is-valid').addClass('is-invalid');
    campo.siblings('.valid-feedback').hide();
    
    let feedbackElement = campo.siblings('.invalid-feedback');
    if (feedbackElement.length === 0) {
        campo.after(`<div class="invalid-feedback">${mensaje}</div>`);
    } else {
        feedbackElement.text(mensaje).show();
    }
}

function limpiarValidacion(campo) {
    campo.removeClass('is-valid is-invalid');
    campo.siblings('.invalid-feedback').hide();
    campo.siblings('.valid-feedback').hide();
}

function calcularEdad(fechaNacimiento) {
    const hoy = new Date();
    let edad = hoy.getFullYear() - fechaNacimiento.getFullYear();
    const mes = hoy.getMonth() - fechaNacimiento.getMonth();
    
    if (mes < 0 || (mes === 0 && hoy.getDate() < fechaNacimiento.getDate())) {
        edad--;
    }
    
    return edad;
}

// ============================================
// VALIDACIÓN DE FORMULARIOS COMPLETOS
// ============================================

function validarFormularioPaciente(formId) {
    const form = $(formId);
    let esValido = true;
    
    // Validar nombres
    const nombres = form.find('[name="Nombres"]').val();
    if (!nombres || !validarSoloLetras(nombres)) {
        marcarCampoInvalido(form.find('[name="Nombres"]'), 'Nombres inválidos');
        esValido = false;
    }
    
    // Validar apellidos
    const apellidos = form.find('[name="Apellidos"]').val();
    if (!apellidos || !validarSoloLetras(apellidos)) {
        marcarCampoInvalido(form.find('[name="Apellidos"]'), 'Apellidos inválidos');
        esValido = false;
    }
    
    // Validar DNI
    const dni = form.find('[name="DNI"]').val();
    if (!validarDNI(dni)) {
        marcarCampoInvalido(form.find('[name="DNI"]'), 'DNI inválido');
        esValido = false;
    }
    
    // Validar email (opcional)
    const email = form.find('[name="Email"]').val();
    if (email && !validarEmail(email)) {
        marcarCampoInvalido(form.find('[name="Email"]'), 'Email inválido');
        esValido = false;
    }
    
    // Validar teléfono (opcional)
    const telefono = form.find('[name="Telefono"]').val();
    if (telefono && !validarTelefono(telefono)) {
        marcarCampoInvalido(form.find('[name="Telefono"]'), 'Teléfono inválido');
        esValido = false;
    }
    
    return esValido;
}

function validarFormularioDoctor(formId) {
    const form = $(formId);
    let esValido = true;
    
    // Validar especialidad seleccionada
    const idEspecialidad = form.find('[name="IdEspecialidad"]').val();
    if (!idEspecialidad || idEspecialidad === '') {
        marcarCampoInvalido(form.find('[name="IdEspecialidad"]'), 'Debe seleccionar una especialidad');
        esValido = false;
    }
    
    // Validar CMP (obligatorio para doctores)
    const cmp = form.find('[name="CMP"]').val();
    if (!cmp || cmp.length < 3) {
        marcarCampoInvalido(form.find('[name="CMP"]'), 'CMP inválido');
        esValido = false;
    }
    
    return esValido && validarFormularioPaciente(formId);
}

// ============================================
// PREVENIR ENVÍO DUPLICADO DE FORMULARIOS
// ============================================

function prevenirEnvioDuplicado() {
    $('form').on('submit', function() {
        const submitBtn = $(this).find('button[type="submit"]');
        submitBtn.prop('disabled', true);
        submitBtn.html('<i class="fas fa-spinner fa-spin"></i> Procesando...');
        
        // Reactivar después de 3 segundos por si hay error
        setTimeout(function() {
            submitBtn.prop('disabled', false);
            submitBtn.html('Guardar');
        }, 3000);
    });
}

// Inicializar prevención de envío duplicado
$(document).ready(function() {
    prevenirEnvioDuplicado();
});
