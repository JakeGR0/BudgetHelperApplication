using BudgetHelperApplication.ViewModels;
using CommunityToolkit.Maui;

namespace BudgetHelperApplication;

public partial class AdjustDepositPage : ContentPage
{
	AccountViewModel viewModel;
	public AdjustDepositPage(AccountViewModel vm)
	{
		InitializeComponent();
		viewModel = vm;
        BindingContext = viewModel;
	}

    private async void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
    {
        if (!viewModel.fromRoot)
        {

            await Shell.Current.Navigation.PopToRootAsync();
        }
        else
        {
            viewModel.fromRoot = false;
        }
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}