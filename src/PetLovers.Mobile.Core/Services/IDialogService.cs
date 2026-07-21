namespace PetLovers.Mobile.Services;

// Abstrai as caixas de diálogo. No app usa Shell.Current.DisplayAlert;
// num teste, um fake devolve respostas fixas — sem UI.
public interface IDialogService
{
    Task AlertAsync(string titulo, string mensagem, string cancelar);
    Task<bool> ConfirmAsync(string titulo, string mensagem, string aceitar, string cancelar);
}
