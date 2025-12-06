using Microsoft.EntityFrameworkCore;
using PoliclinicoWeb.Models;

namespace PoliclinicoWeb.Data
{
    public class PacienteEfData : IPacienteData
    {
        private readonly ApplicationDbContext _context;

        public PacienteEfData(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Paciente>> ListarTodos()
        {
            return await _context.Pacientes
                .AsNoTracking()
                .OrderBy(p => p.Apellidos)
                .ThenBy(p => p.Nombres)
                .ToListAsync();
        }

        public async Task<PaginacionViewModel<Paciente>> ListarConPaginacion(int pagina, int registrosPorPagina, string? busqueda = null)
        {
            var resultado = new PaginacionViewModel<Paciente>
            {
                PaginaActual = pagina,
                RegistrosPorPagina = registrosPorPagina
            };

            var query = _context.Pacientes.AsNoTracking().Where(p => p.Activo);

            if (!string.IsNullOrEmpty(busqueda))
            {
                var b = busqueda.Trim();
                query = query.Where(p => p.Nombres.Contains(b) || p.Apellidos.Contains(b) || p.DNI.Contains(b));
            }

            resultado.TotalRegistros = await query.CountAsync();
            resultado.TotalPaginas = (int)Math.Ceiling((double)resultado.TotalRegistros / registrosPorPagina);

            var items = await query
                .OrderBy(p => p.Apellidos)
                .ThenBy(p => p.Nombres)
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToListAsync();

            resultado.Items = items;
            return resultado;
        }

        public async Task<Paciente?> ObtenerPorId(int idPaciente)
        {
            return await _context.Pacientes.AsNoTracking().FirstOrDefaultAsync(p => p.IdPaciente == idPaciente);
        }

        public async Task<Paciente?> ObtenerPorDNI(string dni)
        {
            return await _context.Pacientes.AsNoTracking().FirstOrDefaultAsync(p => p.DNI == dni);
        }

        public async Task<bool> Crear(Paciente paciente)
        {
            try
            {
                // Ensure FechaRegistro has a valid value for SQL Server datetime range
                if (paciente.FechaRegistro == default)
                {
                    paciente.FechaRegistro = DateTime.Now;
                }
                _context.Pacientes.Add(paciente);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Actualizar(Paciente paciente)
        {
            try
            {
                var existente = await _context.Pacientes.FirstOrDefaultAsync(p => p.IdPaciente == paciente.IdPaciente);
                if (existente == null) return false;

                existente.Nombres = paciente.Nombres;
                existente.Apellidos = paciente.Apellidos;
                existente.DNI = paciente.DNI;
                existente.FechaNacimiento = paciente.FechaNacimiento;
                existente.Sexo = paciente.Sexo;
                existente.Direccion = paciente.Direccion;
                existente.Telefono = paciente.Telefono;
                existente.Email = paciente.Email;
                existente.GrupoSanguineo = paciente.GrupoSanguineo;
                existente.Activo = paciente.Activo;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Eliminar(int idPaciente)
        {
            try
            {
                var existente = await _context.Pacientes.FirstOrDefaultAsync(p => p.IdPaciente == idPaciente);
                if (existente == null) return false;

                existente.Activo = false;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
