using System.Text.Json.Serialization;
using SQLite;

namespace CarnetDepenses.Models;

public class Depense
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Libelle { get; set; } = "";
    public decimal Montant { get; set; }
    public DateTime Date { get; set; } = DateTime.Today;
    public string Categorie { get; set; } = "Autre";

    // Calculée : ni stockée en base, ni dans le JSON.
    [Ignore, JsonIgnore]
    public string Icone => Categorie switch
    {
        "Transport" => "cat_transport.png",
        "Repas" => "cat_repas.png",
        "Courses" => "cat_courses.png",
        "Loisirs" => "cat_loisirs.png",
        "Santé" => "cat_sante.png",
        _ => "cat_autre.png"
    };
}
