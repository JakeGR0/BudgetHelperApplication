
using BudgetHelperApplication.Data;
using BudgetHelperApplication.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Account = BudgetHelperApplication.Models.Account;
using Regex = System.Text.RegularExpressions.Regex;


namespace BudgetHelperApplication.ViewModels
{
    public partial class ShellViewModel : ObservableObject
    {
        private AccountDAO accountDAO;

        [ObservableProperty]
        ObservableCollection<Account> accounts;

        [ObservableProperty]
        ObservableCollection<Account> selected;

        [ObservableProperty]
        string name;

        [ObservableProperty]
        string amount;

        [ObservableProperty]
        bool accountIsValid;

        public bool fromRoot { get; set; }
        
        bool NewAccountIsValid()
        {
            double x;
            if(Name.Length >= 1 && Name.Length <=18 && double.TryParse(Amount, out x) && x <= (double)long.MaxValue/100)
            {
                AccountIsValid= true;
                return true;
            }
            else
            {
                AccountIsValid= false;
                return false;
            }

        }

        partial void OnNameChanged(string value)
        {
            NewAccountIsValid();
            

            
        }
        partial void OnAmountChanged(string value)
        {
            NewAccountIsValid();
        }

        private void LoadAccounts()
        {
            Accounts.Clear();
            var accountTask = this.accountDAO.GetAccountsAsync();
            

            accountTask.ContinueWith((x) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (x.IsCompletedSuccessfully)
                    {
                        foreach (var account in x.Result)
                        {
                            accounts.Add(account);
                        }
                    }
                });
            });
        }

        public ShellViewModel()
        {
            this.accountDAO = new AccountDAO();
            Accounts = new ObservableCollection<Account>();

            LoadAccounts();
        }

        [RelayCommand]
        async void Add()
        {

            double a = 0;
            a = double.Parse(Amount);

            var temp = new Account();

            temp.Name = Name;
            temp.CurrencyUnitAmount = (long)(a * 100.0);

            temp.Id = 0;



            await accountDAO.SaveAccountAsync(temp);
            await Shell.Current.GoToAsync("//MainPage");
            LoadAccounts();
            Name = "";
            Amount = "";

        }

      

        [RelayCommand]
        async void Delete()
        {
            var tasks = new List<Task<int>>();
            if (Selected != null)
            {
                foreach (var account in Selected)
                {
                    tasks.Add(accountDAO.DeleteAccountAsync(account));
                }
                Selected.Clear();
                await Shell.Current.GoToAsync("//MainPage");
                await Task.WhenAll(tasks);
                LoadAccounts();
            }

            
        }

        

    }
}
