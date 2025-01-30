# Platformer

Gra platformowa w stylu klasycznych odsłon Super  Mario.

### Fajne rzeczy
- poruszanie wykorzystujące fizykę
- patrolujący okolicę przeciwnicy
- przepaść czyhająca na nieuważnych graczy
- licznik punktów i system żyć
- zapis wyniku do pliku

### Odpalenie projektu
Aby uniknąć błędu podczas kompilacji projektu w Visual Studio na niektórych komputerach po pobraniu go należy odblokować następujące pliki we właściwościach:
- src/Shared/Domain/Domain.csproj
- src/Platformer/Platformer.csproj
- src/Platformer/.config/dotnet-tools.json
