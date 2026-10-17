using System.Windows.Input;

namespace CarnetDepenses.ViewModels;

// Transforme une méthode en objet liable à une commande du XAML.
public class RelayCommand : ICommand
{
    private readonly Action _executer;            // quoi faire
    private readonly Func<bool>? _peutExecuter;   // si possible

    public RelayCommand(Action executer, Func<bool>? peutExecuter = null)
    {
        _executer = executer;
        _peutExecuter = peutExecuter;
    }

    // La vue appelle ceci et grise le contrôle si false.
    public bool CanExecute(object? p) => _peutExecuter?.Invoke() ?? true;

    // Exécute l'action au clic.
    public void Execute(object? p) => _executer();

    public event EventHandler? CanExecuteChanged;

    // À appeler pour forcer la vue à reposer la question.
    public void Rafraichir() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
