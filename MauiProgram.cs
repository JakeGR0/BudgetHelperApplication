using BudgetHelperApplication.Data;
using BudgetHelperApplication.ViewModels;
using CommunityToolkit.Maui;

namespace BudgetHelperApplication; 

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		
		builder
			.UseMauiApp<App>().UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
			

		builder.Services.AddSingleton<AccountDAO>();
		builder.Services.AddSingleton<ShellViewModel>();
        builder.Services.AddSingleton<AccountViewModel>();
        builder.Services.AddTransient<AddAccountPage>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<DeleteAccountPage>();
		builder.Services.AddTransient<AdjustDepositPage>();
		builder.Services.AddTransient<AddRegularTransactionPage>();
        builder.Services.AddTransient<UpdateRegularTransactionPage>();
        builder.Services.AddTransient<DeleteRegularTransaction>();
        builder.Services.AddTransient<AddOutgoingPage>();
        builder.Services.AddTransient<UpdateOutgoingPage>();
        builder.Services.AddTransient<DeleteOutgoingPage>();

        builder.Services.AddTransient<OverviewPage>();
        builder.Services.AddTransient<IncomePage>();
        builder.Services.AddTransient<OutgoingPage>();
        return builder.Build();
	}
}
