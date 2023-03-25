using BudgetHelperApplication.ViewModels;

namespace BudgetHelperApplication;

public partial class AddAccountPage : ContentPage
{

	ShellViewModel viewModel;
	public AddAccountPage(ShellViewModel SVM)
	{
		InitializeComponent();

		viewModel = SVM;

		BindingContext = viewModel;


	}

    private async void ContentPage_NavigatedFrom(object sender, NavigatedFromEventArgs e)
    {
		
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
}