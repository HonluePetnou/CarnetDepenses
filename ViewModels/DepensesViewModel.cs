using System.Collections.ObjectModel;
using CarnetDepenses.Models;
using CarnetDepenses.Services;
using Microsoft.Maui.Storage;

namespace CarnetDepenses.ViewModels;

public class DepensesViewModel : BaseViewModel
{
    private readonly IDepenseRepository _repo;
    private readonly IApiService _api;
    private string _libelle = "", _montantTexte = "";
    private bool _isBusy;
    private string _totalConverti = "", _message = "";
    private string _categorieChoisie = "Autre";
    private string _filtre = "";

    public ObservableCollection<Depense> Depenses { get; } = new();
    public decimal Total => Depenses.Sum(d => d.Montant);

    // Ce que la liste affiche : Depenses filtrées par la recherche.
    public ObservableCollection<Depense> Affichees { get; } = new();

    public string Filtre
    {
        get => _filtre;
        set { if (SetField(ref _filtre, value)) AppliquerFiltre(); }
    }

    public string Libelle
    {
        get => _libelle;
        set { if (SetField(ref _libelle, value)) AjouterCommand.Rafraichir(); }
    }

    public string MontantTexte
    {
        get => _montantTexte;
        set { if (SetField(ref _montantTexte, value)) AjouterCommand.Rafraichir(); }
    }

    public List<string> Categories { get; } =
        new() { "Transport", "Repas", "Courses", "Loisirs", "Santé", "Autre" };

    public string CategorieChoisie
    {
        get => _categorieChoisie;
        set => SetField(ref _categorieChoisie, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        set { if (SetField(ref _isBusy, value)) ConvertirCommand.Rafraichir(); }
    }

    public string TotalConverti
    {
        get => _totalConverti;
        set => SetField(ref _totalConverti, value);
    }

    public string Message
    {
        get => _message;
        set => SetField(ref _message, value);
    }

    public RelayCommand AjouterCommand { get; }
    public RelayCommandP SupprimerCommand { get; }
    public RelayCommand ConvertirCommand { get; }

    // Le repository et le service API sont INJECTÉS par le conteneur (voir MauiProgram).
    public DepensesViewModel(IDepenseRepository repo, IApiService api)
    {
        _repo = repo;
        _api = api;
        AjouterCommand = new RelayCommand(
            async () => await AjouterAsync(),
            () => !string.IsNullOrWhiteSpace(Libelle)
               && !string.IsNullOrWhiteSpace(MontantTexte));
        SupprimerCommand = new RelayCommandP(
            async p => await SupprimerAsync((Depense)p!));
        ConvertirCommand = new RelayCommand(
            async () => await ConvertirAsync(), () => !IsBusy);
    }

    // Relit le stockage et remplit la liste affichée.
    public async Task ChargerAsync()
    {
        Depenses.Clear();
        foreach (var d in await _repo.ListerAsync())
            Depenses.Add(d);
        AppliquerFiltre();
        OnPropertyChanged(nameof(Total));
    }

    private void AppliquerFiltre()
    {
        Affichees.Clear();
        foreach (var d in Depenses)
            if (string.IsNullOrWhiteSpace(Filtre)
                || d.Libelle.Contains(Filtre, StringComparison.OrdinalIgnoreCase)
                || d.Categorie.Contains(Filtre, StringComparison.OrdinalIgnoreCase))
                Affichees.Add(d);
    }

    private async Task AjouterAsync()
    {
        if (!decimal.TryParse(MontantTexte, out decimal m))
            return;
        var d = new Depense { Libelle = Libelle, Montant = m, Categorie = CategorieChoisie };
        await _repo.AjouterAsync(d);   // 1. on sauvegarde
        Depenses.Add(d);               // 2. on affiche
        AppliquerFiltre();
        OnPropertyChanged(nameof(Total));
        Libelle = ""; MontantTexte = "";
    }

    private async Task SupprimerAsync(Depense d)
    {
        await _repo.SupprimerAsync(d);
        Depenses.Remove(d);
        Affichees.Remove(d);
        OnPropertyChanged(nameof(Total));
    }

    private async Task ConvertirAsync()
    {
        IsBusy = true; Message = "";
        try
        {
            var t = await _api.GetTauxAsync();             // appel réseau
            Preferences.Set("taux_eur", (double)t.Taux["EUR"]);   // cache
            TotalConverti = $"{Total * t.Taux["EUR"]:F2} EUR";
        }
        catch (HttpRequestException)                       // réseau coupé
        {
            double c = Preferences.Get("taux_eur", 0.0);
            if (c == 0) Message = "Hors ligne, pas de taux.";
            else
            {
                TotalConverti = $"{Total * (decimal)c:F2} EUR (cache)";
                Message = "Hors ligne : dernier taux connu.";
            }
        }
        finally { IsBusy = false; }                        // exécuté TOUJOURS
    }
}
