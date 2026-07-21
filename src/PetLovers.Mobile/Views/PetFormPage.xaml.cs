using PetLovers.Mobile.ViewModels;

namespace PetLovers.Mobile.Views;

// A página cuida da navegação do Shell (QueryProperty) e repassa o id para a
// ViewModel, que assim fica livre de acoplamento com o MAUI (e testável).
[QueryProperty(nameof(PetId), "petId")]
public partial class PetFormPage : ContentPage
{
    private readonly PetFormViewModel _vm;

    public int PetId { set => _vm.PetId = value; }

    public PetFormPage(PetFormViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.InicializarCommand.Execute(null);
    }
}
