

using Pulperia.Models;

public class RolSystem
{
    public int Id {get;set;}
    public int Name {get;set;}
    public ICollection<RolUser> RolUsers {get; set; }

}