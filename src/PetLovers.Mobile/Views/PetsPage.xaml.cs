using PetLovers.Mobile.ViewModels;

namespace PetLovers.Mobile.Views;

public partial class PetsPage : ContentPage
{
    private readonly PetsViewModel _vm;

    public PetsPage(PetsViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.CarregarCommand.Execute(null);
    }
}
