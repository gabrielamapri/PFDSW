using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PoliclinicoWeb.Controllers
{
    public class GrupoSanguineoController : Controller
    {
        private readonly string _connectionString;

        public GrupoSanguineoController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("PoliclinicoDb") ?? throw new InvalidOperationException("Connection string 'PoliclinicoDb' is not configured.");
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var list = new List<object>();

            using (var cn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_ListarGrupoSanguineo", cn) { CommandType = CommandType.StoredProcedure })
            {
                await cn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        list.Add(new
                        {
                            idGrupo = reader.GetInt32(reader.GetOrdinal("IdGrupo")),
                            codigo = reader.GetString(reader.GetOrdinal("Codigo")),
                            nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? null : reader.GetString(reader.GetOrdinal("Nombre")),
                            activo = reader.GetBoolean(reader.GetOrdinal("Activo"))
                        });
                    }
                }
            }

            return Json(list);
        }
    }
}
