namespace PetLovers.Mobile.Services;

// Implementação real de navegação — usa o Shell do MAUI.
public class MauiNavigationService : INavigationService
{
    public Task GoToAsync(string route) => Shell.Current.GoToAsync(route);
}
