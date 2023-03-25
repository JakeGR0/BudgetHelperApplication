using BudgetHelperApplication.Models;
using BudgetHelperApplication.ViewModels;

namespace BudgetHelperApplication;

public partial class AddRegularTransactionPage : ContentPage
{
	AccountViewModel viewModel;

	public AddRegularTransactionPage(AccountViewModel vm)
	{

		InitializeComponent();
		viewModel = vm;
		BindingContext= viewModel;
	}

    private async void ContentPage_Loaded(object sender, EventArgs e)
    {
		if (!viewModel.fromRoot)
        {

            await Shell.Current.Navigation.PopToRootAsync();
        }
        else
        {
            viewModel.fromRoot = false;
        }
		RegularTransaction newRegTrans= new RegularTransaction();
		newRegTrans.AccountId = viewModel.AccountId;
		newRegTrans.Id = 0;
		newRegTrans.Incoming = true;
		viewModel.NewRegTransName = "";
		viewModel.NewRegTransCurrencyAmount = "";
		viewModel.NewRegTransFrequency = "";
		viewModel.NewRegTransFrequencyIncrement = RegularTransaction.FrequencyIncrement.Day;

        viewModel.NewRegularTransaction = newRegTrans;
		viewModel.NewRegTransLPDate = DateTime.Now.Date;
		viewModel.UpdateSelected = false;
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
		
		await Shell.Current.GoToAsync("//IncomePage");
		
    }

    private async void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
    {
        
    }
}