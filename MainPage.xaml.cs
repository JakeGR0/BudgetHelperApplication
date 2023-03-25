using BudgetHelperApplication.Models;
using BudgetHelperApplication.ViewModels;

namespace BudgetHelperApplication;

public partial class MainPage : ContentPage
{
	ShellViewModel viewModel;
    AccountViewModel avm;

	public MainPage(ShellViewModel vm, AccountViewModel accountView )
	{
		InitializeComponent();
		viewModel= vm;
        avm = accountView;
        BindingContext = avm;
		BindingContext= viewModel;
	}

    private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Account account)
            return;
        avm.AccountId = account.Id;

        await Shell.Current.GoToAsync("//OverviewPage");

        CollectionView collectionV = sender as CollectionView;

        collectionV.SelectedItem = null;



    }
    private async void GoToAddAccountPage(object sender, EventArgs e)
    {
        viewModel.fromRoot= true;
        await Shell.Current.GoToAsync("AddAccountPage");
        
        
        
    }

    private async void GoToDeleteAccountPage(object sender, EventArgs e)
    {
        viewModel.fromRoot = true;
        await Shell.Current.GoToAsync("DeleteAccountPage");
        
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var frame = sender as Frame;
        Account account= ((TapGestureRecognizer)frame.GestureRecognizers[0]).CommandParameter as Account;
        avm.AccountId = account.Id;

        await Shell.Current.GoToAsync("//OverviewPage");
    }
}

