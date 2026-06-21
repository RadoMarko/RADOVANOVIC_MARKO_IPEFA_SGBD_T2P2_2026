namespace RefugeAnimaux.classesMetier;

public class Sortie
{
    public int IdSortie { get; set; }
    public DateTime DateSortie { get; set; }
    public string IdAnimal { get; set; } = string.Empty;
    public int IdMotifSortie { get; set; }
    public int? IdContact { get; set; }
}
