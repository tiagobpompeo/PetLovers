using NSubstitute;
using PetLovers.Mobile.Models;
using PetLovers.Mobile.Services;
using PetLovers.Mobile.ViewModels;

namespace PetLovers.UnitTests;

// A MESMA ideia dos testes com fakes escritos à mão — mas o NSubstitute
// gera os dublês para nós. Nenhuma classe Fake precisa existir.
public class PetsViewModelComMockTests
{
    [Fact]
    public async Task Carregar_UsaOsPetsQueOMockDevolve()
    {
        var api = Substitute.For<IApiService>();                 // dublê gerado
        api.GetPetsAsync(Arg.Any<string?>())                     // "quando chamarem GetPets…"
           .Returns([                                            // "…devolva estes 2"
               new Pet(1, "Rex", "Cachorro", "", "Macho", null, 3, "", 28.5m, null, null, 1, "Maria", []),
               new Pet(2, "Mimi", "Gato", "", "Femea", null, 2, "", 4.2m, null, null, 1, "Maria", [])
           ]);

        var vm = new PetsViewModel(api, Substitute.For<INavigationService>(), Substitute.For<IDialogService>());
        await vm.CarregarCommand.ExecuteAsync(null);

        Assert.Equal(2, vm.Pets.Count);
    }

    [Fact]
    public async Task Excluir_VerificaQueDeleteFoiChamadoComOIdCerto()
    {
        var api = Substitute.For<IApiService>();
        var dialog = Substitute.For<IDialogService>();
        // o diálogo devolve default (false) se não configurado → o delete nem rodaria.
        dialog.ConfirmAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
              .Returns(true);

        var vm = new PetsViewModel(api, Substitute.For<INavigationService>(), dialog);
        var pet = new Pet(3, "Thor", "Cachorro", "", "Macho", null, 2, "", 11m, null, null, 1, "Maria", []);

        await vm.ExcluirPetCommand.ExecuteAsync(pet);

        await api.Received(1).DeletePetAsync(3);   // verifica a INTERAÇÃO: chamado 1x com id 3
    }
}
