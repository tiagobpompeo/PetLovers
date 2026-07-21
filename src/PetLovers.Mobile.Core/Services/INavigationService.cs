namespace PetLovers.Mobile.Services;

// Abstrai a navegação. No app, a implementação usa Shell.Current;
// num teste, um fake registra para onde navegou — sem UI.
public interface INavigationService
{
    Task GoToAsync(string route);
}
