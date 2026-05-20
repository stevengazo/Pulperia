

using Pulperia.Models;

public class RolSystem
{
    public int Id {get;set;}
    public string Name {get;set;}
    public ICollection<RolUser> RolUsers {get; set; }

}