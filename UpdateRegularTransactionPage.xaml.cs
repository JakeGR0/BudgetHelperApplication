using BudgetHelperApplication.ViewModels;

namespace BudgetHelperApplication;

public partial class UpdateRegularTransactionPage : ContentPage
{
	AccountViewModel viewModel;
	public UpdateRegularTransactionPage(AccountViewModel vm)
	{
		InitializeComponent();
		viewModel= vm;
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
		if(viewModel.UpdateSelected && !viewModel.NewRegularTransaction.Incoming)
		{
			viewModel.UpdateSelected= false;
		}
		

    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
		await Shell.Current.GoToAsync("//IncomePage");
    }

    
}