# CarnetDepenses

Application .NET MAUI (TP 3 Backend : persistance, services et API) pour noter ses dépenses.

## Historique par phase

| Phase | Contenu |
|---|---|
| A | Persistance locale en JSON (`JsonDepenseRepository`), MVVM, injection de dépendances |
| B | API simulée hors ligne (`FakeApiHandler`, `ApiService`), conversion du total en EUR, cache `Preferences` en cas de panne |
| C | Persistance SQLite (`SqliteDepenseRepository`) : une seule ligne changée dans `MauiProgram.cs` |

## Fonctionnalités ajoutées

- Interface épurée avec icônes vectorielles (SVG)
- Catégories de dépenses avec une icône par catégorie
- Recherche dans l'historique (libellé ou catégorie)
- Confirmation avant suppression
- Mode sombre automatique (suit le thème du système)

## Lancer

```bash
dotnet build -f net9.0-windows10.0.19041.0
```

Captures des interfaces : [`docs/CarnetDepenses_captures.pdf`](docs/CarnetDepenses_captures.pdf).
