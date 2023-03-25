using BudgetHelperApplication.Models;
using BudgetHelperApplication.ViewModels;
using System.Collections.ObjectModel;

namespace BudgetHelperApplication;

public partial class DeleteAccountPage : ContentPage
{
	ShellViewModel viewModel { get; set; }
	public DeleteAccountPage(ShellViewModel vm)
	{
		InitializeComponent();
		viewModel = vm;
        BindingContext= viewModel;
	}

    private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Account a)
            return;

        var temp = new ObservableCollection<Account>();
        foreach (var account in e.CurrentSelection)
        {
            temp.Add((Account)account);
        }

        viewModel.Selected = temp;
    }
    private async void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
    {
        if (!viewModel.fromRoot)
        {

            await Shell.Current.Navigation.PopToRootAsync();
        }
        else
        {
            viewModel.Selected = new ObservableCollection<Account>();
            viewModel.fromRoot = false;
        }
    }

   

    private void TapGestureRecognizer_Tapped_1(object sender, TappedEventArgs e)
    {
        var x = sender as Frame;
        Account account = ((TapGestureRecognizer)x.GestureRecognizers[0]).CommandParameter as Account;
        

        if(viewModel.Selected.Contains(account))
        {
             viewModel.Selected.Remove(account);
             viewModel.Selected = viewModel.Selected;
            x.BackgroundColor = Color.Parse("gold");
        }
        else
        {
            viewModel.Selected.Add(account);
            viewModel.Selected = viewModel.Selected;
            x.BackgroundColor = Color.Parse("orange");
        }
    }
}