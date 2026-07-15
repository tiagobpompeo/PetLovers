using PetLovers.Mobile.Views;

namespace PetLovers.Mobile;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute("petform", typeof(PetFormPage));
	}
}
