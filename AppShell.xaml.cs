using BudgetHelperApplication.Data;
using System.Collections.ObjectModel;
using BudgetHelperApplication.Models;
using BudgetHelperApplication.ViewModels;
using Microsoft.Maui.Controls;

namespace BudgetHelperApplication;

public partial class AppShell : Shell
{
	
	ShellViewModel viewModel;

	public AppShell()
	{
		InitializeComponent();

		viewModel = new ShellViewModel();
		BindingContext = viewModel;
		
		RegisterRoutes();
		

		

		

	}

	void RegisterRoutes()
	{
		Routing.RegisterRoute("MainPage/AddAccountPage", typeof(AddAccountPage));
		Routing.RegisterRoute("MainPage", typeof(MainPage));
		Routing.RegisterRoute("MainPage/DeleteAccountPage", typeof (DeleteAccountPage));
		
        Routing.RegisterRoute("OverviewPage", typeof(OverviewPage));
		Routing.RegisterRoute("OverviewPage/AdjustDepositPage", typeof(AdjustDepositPage));

        Routing.RegisterRoute("IncomePage", typeof(IncomePage));
		Routing.RegisterRoute("IncomePage/AddRegularTransactionPage", typeof(AddRegularTransactionPage));
        Routing.RegisterRoute("IncomePage/UpdateRegularTransactionPage", typeof(UpdateRegularTransactionPage));
        Routing.RegisterRoute("IncomePage/DeleteRegularTransaction", typeof(DeleteRegularTransaction));

        Routing.RegisterRoute("OutgoingPage", typeof(OutgoingPage));
		Routing.RegisterRoute("OutgoingPage/AddOutgoingPage", typeof(AddOutgoingPage));
        Routing.RegisterRoute("OutgoingPage/UpdateOutgoingPage", typeof(UpdateOutgoingPage));
        Routing.RegisterRoute("OutgoingPage/DeleteOutgoingPage", typeof(DeleteOutgoingPage));
		
       
    }

	
}
