namespace PetLovers.Mobile.Services;

// Implementação real de diálogos — usa o Shell do MAUI.
public class MauiDialogService : IDialogService
{
    public Task AlertAsync(string titulo, string mensagem, string cancelar) =>
        Shell.Current.DisplayAlert(titulo, mensagem, cancelar);

    public Task<bool> ConfirmAsync(string titulo, string mensagem, string aceitar, string cancelar) =>
        Shell.Current.DisplayAlert(titulo, mensagem, aceitar, cancelar);
}
