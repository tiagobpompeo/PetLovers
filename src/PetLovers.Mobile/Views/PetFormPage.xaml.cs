using PetLovers.Mobile.ViewModels;

namespace PetLovers.Mobile.Views;

public partial class PetFormPage : ContentPage
{
    private readonly PetFormViewModel _vm;

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
