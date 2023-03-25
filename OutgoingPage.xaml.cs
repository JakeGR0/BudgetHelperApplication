using BudgetHelperApplication.Models;
using BudgetHelperApplication.ViewModels;
using System.Globalization;
using System.Transactions;

namespace BudgetHelperApplication;

public partial class OutgoingPage : ContentPage
{
    AccountViewModel viewModel;
	public OutgoingPage(AccountViewModel vm)
	{
		InitializeComponent();
        viewModel= vm;
        BindingContext= viewModel;
	}

    private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Models.RegularTransaction transaction)
            return;

        viewModel.fromRoot = true;

        viewModel.UpdateSelected = true;
        viewModel.NewRegularTransaction = transaction;
        viewModel.NewRegTransName = transaction.Name;
        var x = new NumberFormatInfo();
        x.CurrencySymbol = "";
        x.CurrencyNegativePattern = 1;
        viewModel.NewRegTransCurrencyAmount = (transaction.CurrencyAmount / 100.0).ToString("c", x);
        viewModel.NewRegTransFrequency = transaction.Frequency.ToString();
        viewModel.NewRegTransFrequencyIncrement = transaction.increment;
        viewModel.NewRegTransLPDate = transaction.LPDate;





        await Shell.Current.GoToAsync("UpdateOutgoingPage");

        CollectionView collectionV = sender as CollectionView;

        collectionV.SelectedItem = null;
    }

    private async void GoToAddOutgoingPage(object sender, EventArgs e)
    {
        viewModel.fromRoot = true;
        await Shell.Current.GoToAsync("AddOutgoingPage");
    }

    private async void GoToDeleteOutgoingPage(object sender, EventArgs e)
    {
        viewModel.fromRoot = true;
        await Shell.Current.GoToAsync("DeleteOutgoingPage");
    }

    private async  void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var frame = sender as Frame;
        RegularTransaction selected = (RegularTransaction)((TapGestureRecognizer)frame.GestureRecognizers[0]).CommandParameter;
        viewModel.fromRoot = true;

        viewModel.UpdateSelected = true;
        viewModel.NewRegularTransaction = selected;
        viewModel.NewRegTransName = selected.Name;
        var x = new NumberFormatInfo();
        x.CurrencySymbol = "";
        viewModel.NewRegTransCurrencyAmount = (selected.CurrencyAmount / 100.0).ToString("c", x);
        viewModel.NewRegTransFrequency = selected.Frequency.ToString();
        viewModel.NewRegTransFrequencyIncrement = selected.increment;
        viewModel.NewRegTransLPDate = selected.LPDate;





        await Shell.Current.GoToAsync("UpdateOutgoingPage");
    }
}