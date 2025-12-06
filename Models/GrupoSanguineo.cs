namespace PoliclinicoWeb.Models
{
    public class GrupoSanguineo
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int IdGrupo { get; set; }
        public string Codigo { get; set; } = string.Empty; // e.g. O+
        public string Nombre { get; set; } = string.Empty; // e.g. O+
        public bool Activo { get; set; } = true;
    }
}
