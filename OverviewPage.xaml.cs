
using BudgetHelperApplication.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BudgetHelperApplication;


public partial class OverviewPage : ContentPage
{
    
    
   

    private AccountViewModel viewModel;
   
    public OverviewPage(AccountViewModel vm)
	{
		InitializeComponent();
        viewModel = vm;
        BindingContext = viewModel;
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        viewModel.fromRoot = true;
        await Shell.Current.GoToAsync("AdjustDepositPage");
    }
}