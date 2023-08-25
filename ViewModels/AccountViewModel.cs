using BudgetHelperApplication.Data;
using BudgetHelperApplication.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetHelperApplication.ViewModels
{
    public partial class AccountViewModel: ObservableObject
    {
        public bool fromRoot { get; set; }

        Task<Account> task;

        NumberFormatInfo formatInfo;

        [ObservableProperty]
        long accountId;

        [ObservableProperty]
        bool tabIsVisible;

        [ObservableProperty]
        bool tabIsNotVisible;

        [ObservableProperty]
        Account currentAccount;

        AccountDAO dAO;

        [ObservableProperty]
        ObservableCollection<RegularTransaction> income;

        [ObservableProperty]
        ObservableCollection<RegularTransaction> outgoing;

        [ObservableProperty]
        string deposit;

        [ObservableProperty]
        string wallet;

        [ObservableProperty]
        string dailyAllowance;

        [ObservableProperty]
        string newDeposit;

        [ObservableProperty]
        bool newDepositIsValid;

        [ObservableProperty]
        string newRegTransName;        

        [ObservableProperty]
        DateTime newRegTransLPDate;        

        [ObservableProperty]
        string newRegTransCurrencyAmount;        

        [ObservableProperty]
        string newRegTransFrequency;        

        [ObservableProperty]
        RegularTransaction.FrequencyIncrement newRegTransFrequencyIncrement;        

        [ObservableProperty]
        bool newTransactionIsValid;

        readonly RegularTransaction.FrequencyIncrement[] _frequencyIncrements = { RegularTransaction.FrequencyIncrement.Day, RegularTransaction.FrequencyIncrement.Week, RegularTransaction.FrequencyIncrement.Month };

        public List<RegularTransaction.FrequencyIncrement> FrequencyIncrements
        {
            get { return _frequencyIncrements.ToList(); }
        }

        [ObservableProperty]
        RegularTransaction newRegularTransaction;

        [ObservableProperty]
        List<RegularTransaction> deleteSelection;

        [ObservableProperty]
        bool updateSelected;

        

        [ObservableProperty]
        bool updateNotSelected;

       


        public AccountViewModel()
        {
            dAO = new AccountDAO();
            AccountId = 0;
            TabIsVisible= false;
            TabIsNotVisible = true;
            NewDepositIsValid= false;
            UpdateSelected = false;
            UpdateNotSelected = true;
            Income = new ObservableCollection<RegularTransaction>();
            Outgoing = new ObservableCollection<RegularTransaction>();
            NewRegularTransaction = new RegularTransaction();
            DeleteSelection = new List<RegularTransaction>();
            formatInfo = new NumberFormatInfo();
            formatInfo.CurrencySymbol = "";
            formatInfo.CurrencyNegativePattern = 1;
            StartUpTimer();
            
            
        }
        
        async Task StartUpTimer()
        {
            await dAO.WaitForANewDay().ContinueWith(async (t) =>
            {
                if (t.IsCompletedSuccessfully)
                {
                    var temp = AccountId;
                    AccountId = 0;
                    AccountId = temp;
                    await StartUpTimer();
                }
                else
                {
                    var x = t.Exception.Message;
                    t.Dispose();
                    
                    await StartUpTimer();
                }
            });
        }
        
        partial void OnUpdateSelectedChanged(bool value)
        {
            UpdateNotSelected = !value;
        }

        partial void OnAccountIdChanged(long value)
        {
            if (AccountId != 0)
            {
                TabIsVisible = true;
                TabIsNotVisible= false;
            }
            else
            {
                return;
            }
            task = dAO.GetAccountAsync(AccountId);

            task.ContinueWith((t) => 
            {
                CurrentAccount = t.Result;                
            });

            
        }

        partial void OnCurrentAccountChanged(Account value) 
        {
            loadRegularTransactions();            
            Deposit = (CurrentAccount.CurrencyUnitAmount / 100.0).ToString("c",formatInfo);
        }

        private async void loadRegularTransactions()
        {
            await dAO.GetRegularTransactionsAsync(AccountId, true).ContinueWith((t) =>
            {
                var tempIn = new ObservableCollection<RegularTransaction>();
                foreach(var x in t.Result)
                {
                    tempIn.Add(x);
                }
                Income = tempIn;
            });
            await dAO.GetRegularTransactionsAsync(AccountId, false).ContinueWith((t) =>
            {
                var tempOut = new ObservableCollection<RegularTransaction>();
                foreach(var x in t.Result)
                {
                    tempOut.Add(x);
                }
                Outgoing = tempOut;
            });

            

            
                loadDailyAllowance();
            
        }

        private void loadDailyAllowance()
        {
            double temp = 0.0;

            foreach(var x in Income)
            {
                double days = (double)x.Frequency;
                switch(x.increment)
                {
                    case RegularTransaction.FrequencyIncrement.Week:
                        days *= 7;
                        break;
                    case RegularTransaction.FrequencyIncrement.Month:
                        days *= (365.24/12);
                        break;
                }
                temp += x.CurrencyAmount / days;
            }
            foreach (var x in Outgoing)
            {
                double days = (double)x.Frequency;
                switch (x.increment)
                {
                    case RegularTransaction.FrequencyIncrement.Week:
                        days *= 7;
                        break;
                    case RegularTransaction.FrequencyIncrement.Month:
                        days *= (365.24 / 12);
                        break;
                }
                temp -= x.CurrencyAmount / days;
            }
            

            DailyAllowance = ((long)temp/100.0).ToString("c",formatInfo);
            loadWallet();
        }

        partial void OnDailyAllowanceChanged(string value)
        {
           // loadWallet();
        }

        partial void OnNewDepositChanged(string value)
        {
            double x;
            if(double.TryParse(value,out x) && x * 100 <= (double)long.MaxValue)
            {
                NewDepositIsValid= true;
            }
            else
            {
                NewDepositIsValid = false;
            }
        }

        partial void OnNewRegTransCurrencyAmountChanged(string value)
        {
            ValidateNewRegularTransaction();
        }
        partial void OnNewRegTransNameChanged(string value) { ValidateNewRegularTransaction(); }

        partial void OnNewRegTransLPDateChanged(DateTime value)
        {
            ValidateNewRegularTransaction();
        }

        partial void OnNewRegTransFrequencyChanged(string value)
        {
            ValidateNewRegularTransaction();
        }

        partial void OnNewRegTransFrequencyIncrementChanged(RegularTransaction.FrequencyIncrement value)
        {
            ValidateNewRegularTransaction();
        }

        static DateTime NpDate(RegularTransaction transaction)
        {
            if (transaction == null) return DateTime.Now;
            var temp = transaction.LPDate;
            switch (transaction.increment)
            {
                case RegularTransaction.FrequencyIncrement.Day:
                    temp = temp.AddDays(transaction.Frequency);
                    break;
                case RegularTransaction.FrequencyIncrement.Week:
                    temp = temp.AddDays(transaction.Frequency * 7); 
                    break;
                case RegularTransaction.FrequencyIncrement.Month:
                    temp = temp.AddMonths(transaction.Frequency);
                    break;
            }

            return temp;
        }

        static double dailyValue(RegularTransaction transaction)
        {
            double temp = 0.0;
            double days = (double)transaction.Frequency;
            switch (transaction.increment)
            {
                case RegularTransaction.FrequencyIncrement.Week:
                    days *= 7;
                    break;
                case RegularTransaction.FrequencyIncrement.Month:
                    days *= (365.24 / 12);
                    break;
            }
            temp = (long)(transaction.CurrencyAmount / days)/100.0;
            return temp;
        }

        RegularTransaction highestIncome
        {
            get
            {
                RegularTransaction highIncome = null;
                double temp = double.MinValue;
                foreach (var x in Income)
                {
                    if (temp < dailyValue(x))
                    {
                        temp = dailyValue(x);
                        highIncome = x;
                    }
                }
                return highIncome;
            }
        }

        private double offset
        {
            get
            {
                
                
                var temp = 0.0;

                foreach(var x in Income)
                {
                    var tempTrans = new RegularTransaction();
                    tempTrans.Frequency = x.Frequency;
                    tempTrans.increment = x.increment;
                    tempTrans.LPDate= x.LPDate;
                    if (x != highestIncome && tempTrans.LPDate < highestIncome.LPDate && NpDate(tempTrans) >= NpDate(highestIncome))
                    {
                        int days = (NpDate(highestIncome) - tempTrans.LPDate).Days;
                        temp += dailyValue(x) * (days-1);
                    }
                    else if (x != highestIncome)
                    {
                        while (x != highestIncome && NpDate(tempTrans) < NpDate(highestIncome))
                        {
                            temp += x.CurrencyAmount / 100.0;
                            tempTrans.LPDate = NpDate(tempTrans);
                        }
                    }
                }

                foreach (var x in Outgoing)
                {
                    var tempTrans = new RegularTransaction();
                    tempTrans.Frequency = x.Frequency;
                    tempTrans.increment = x.increment;
                    tempTrans.LPDate = x.LPDate;
                    while (NpDate(tempTrans) < NpDate(highestIncome))
                    {
                        temp -= x.CurrencyAmount / 100.0;
                        tempTrans.LPDate = NpDate(tempTrans);
                    }
                    int days = (NpDate(highestIncome) - tempTrans.LPDate).Days;
                    temp -= dailyValue(x) * (days - 1);
                }

                return temp;

            }
        }
        
        private void loadWallet()
        {   
            int days = (NpDate(highestIncome) - DateTime.Now.Date).Days;
            
            Wallet = (double.Parse(Deposit) + offset).ToString("c",formatInfo);
            
            if(double.Parse(DailyAllowance) > 0)
            {
                double temp = double.Parse(Wallet);
                temp -= double.Parse(DailyAllowance) * (days-1);
                
                Wallet = temp.ToString("c",formatInfo);
            }

            //long x = (long)(double.Parse(Wallet) * 100);
            //var wal = x / 100.0;
           // Wallet = wal.ToString("c",c);


        }
        

        [RelayCommand]
        async void AdjustDeposit()
        {
            CurrentAccount.CurrencyUnitAmount = (long)(double.Parse(NewDeposit) * 100);

            await dAO.SaveAccountAsync(CurrentAccount);

            
            await dAO.GetAccountAsync(AccountId).ContinueWith((t) =>
            {
                CurrentAccount = t.Result;
            });

            

            NewDeposit = "";

        }

        [RelayCommand]
        async void SaveRegularTransaction()
        {
            NewRegularTransaction.Name = NewRegTransName;
            NewRegularTransaction.increment = NewRegTransFrequencyIncrement;
            NewRegularTransaction.Frequency = int.Parse(NewRegTransFrequency);
            NewRegularTransaction.LPDate = NewRegTransLPDate;
            NewRegularTransaction.CurrencyAmount = (long)(double.Parse(NewRegTransCurrencyAmount) * 100);

            await dAO.SaveRegularTransactionAsync(NewRegularTransaction).ContinueWith((t) =>
            {
                var temp = AccountId;
                AccountId = 0;
                AccountId = temp;
            });

            NewRegTransName = "";
            NewRegTransFrequencyIncrement= 0;
            NewRegTransFrequency = "";
            NewRegTransLPDate= DateTime.Now;
            NewRegTransCurrencyAmount = "";

            

        }

        void ValidateNewRegularTransaction()
        {
            int x;
            if(!int.TryParse(NewRegTransFrequency,out x))
            {
                NewTransactionIsValid= false;
                return;
            }
            double d;
            DateTime dateTime = DateTime.Now.Date;
            switch(NewRegTransFrequencyIncrement)
            {
                case RegularTransaction.FrequencyIncrement.Day:
                    dateTime= dateTime.AddDays(x * -1);
                    break;
                case RegularTransaction.FrequencyIncrement.Week:
                    dateTime = dateTime.AddDays(x * 7 * -1);
                    break;
                case RegularTransaction.FrequencyIncrement.Month:
                    dateTime = dateTime.AddMonths(x* -1);
                    break;
                default:
                    NewTransactionIsValid= false;
                    return;
            }
            
            NewTransactionIsValid = NewRegTransName.Length > 0 && NewRegTransName.Length<=18 && double.TryParse(NewRegTransCurrencyAmount,out d ) 
                && NewRegTransLPDate.Date <= DateTime.Now.Date && NewRegTransLPDate.Date > dateTime;
        }

        [RelayCommand]
        async void Delete()
        {
            var tasks = new List<Task<int>>();
            if (DeleteSelection != null)
            {
                foreach (var rT in DeleteSelection)
                {
                    tasks.Add(dAO.DeleteRegularTransactionAsync(rT));
                }
                DeleteSelection.Clear();
                await Task.WhenAll(tasks);
                loadRegularTransactions(); 
            }


        }







    }
}
