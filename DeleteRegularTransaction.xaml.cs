using BudgetHelperApplication.Models;
using BudgetHelperApplication.ViewModels;
using System.Collections.ObjectModel;

namespace BudgetHelperApplication;

public partial class DeleteRegularTransaction : ContentPage
{
    AccountViewModel viewModel;
	public DeleteRegularTransaction(AccountViewModel vm)
	{
		InitializeComponent();
        viewModel = vm;
        BindingContext= viewModel;
	}

    private void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not RegularTransaction t)
            return;

        var temp = new List<RegularTransaction>();
        foreach (var rT in e.CurrentSelection)
        {
            temp.Add((RegularTransaction)rT);
        }

        viewModel.DeleteSelection = temp;
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//IncomePage");
    }

    private async void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
    {
        if (!viewModel.fromRoot)
        {

            await Shell.Current.Navigation.PopToRootAsync();
        }
        else
        {
            if (viewModel.DeleteSelection is null)
            {
                viewModel.DeleteSelection = new List<RegularTransaction>();
            }
            else
            {
                viewModel.DeleteSelection.Clear();
                viewModel.DeleteSelection = viewModel.DeleteSelection;

            }
            viewModel.fromRoot = false;
        }
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var x = sender as Frame;
        RegularTransaction regTrans = ((TapGestureRecognizer)x.GestureRecognizers[0]).CommandParameter as RegularTransaction;


        if (viewModel.DeleteSelection.Contains(regTrans))
        {
            viewModel.DeleteSelection.Remove(regTrans);
            viewModel.DeleteSelection = viewModel.DeleteSelection;
            x.BackgroundColor = Color.Parse("gold");
        }
        else
        {
            viewModel.DeleteSelection.Add(regTrans);
            viewModel.DeleteSelection = viewModel.DeleteSelection;
            x.BackgroundColor = Color.Parse("orange");
        }
    }
}