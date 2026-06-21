namespace RefugeAnimaux.classesMetier;

public class Entree
{
    public int IdEntree { get; set; }
    public DateTime DateEntree { get; set; }
    public string IdAnimal { get; set; } = string.Empty;
    public int IdMotifEntree { get; set; }
    public int? IdContact { get; set; }
}
