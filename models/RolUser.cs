

namespace Pulperia.Models;


public class RolUser
{
    public int Id { get; set; }
    public string UserId { get; set; }

    public int RolId { get; set; }
    public RolSystem Rol { get; set; }
}