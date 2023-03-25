
using BudgetHelperApplication.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace BudgetHelperApplication.Data
{

    public class AccountDAO
    {


        bool doing;
        bool first;
            DateTime lastUsed;
            
        public async Task UpdateDate()
        {
            doing = true;
            await Init();
            
            Database.Table<DatePersist>().ToListAsync().ContinueWith(async (t) =>
            {
                if(t.Result.Count == 0)
                {
                    lastUsed = DateTime.Now.Date.AddDays(-1);
                   
                }
                else
                {
                    lastUsed = t.Result[0].LastUpdated;
                   // if (first) { lastUsed = DateTime.Now.Date.AddDays(-1); first = false; }
                    await Database.DeleteAllAsync(new TableMapping(typeof(DatePersist)));
                    
                }

                var temp = new DatePersist();
                temp.LastUpdated = DateTime.Now.Date;
                temp.Id = 0;
                await Database.InsertAsync(temp);
                List<Task<int>> updateTasks = new();
                if (DateTime.Now.Date - lastUsed.Date > new TimeSpan(0, 0, 0))
                {
                    doing = true;
                    lastUsed = DateTime.Now.Date;
                    var accounts = await GetAccountsAsync();
                    List<Task<List<RegularTransaction>>> tasks = new();
                    foreach (var account in accounts)
                    {
                        tasks.Add(GetRegularTransactionsAsync(account.Id));
                    }
                    do
                    {
                        var regularTransaction = await Task.WhenAny(tasks);
                        tasks.Remove(regularTransaction);
                        long accountId = long.MinValue;
                        bool first = true;
                        long total = 0;
                        foreach (var transaction in regularTransaction.Result)
                        {
                            if (accountId != transaction.AccountId && first)
                            {
                                accountId = transaction.AccountId;
                                first = false;
                            }
                            else if (accountId != transaction.AccountId)
                            {
                                throw new Exception("database issues.");
                            }
                            var lP = transaction.LPDate.Date;
                            var frequency = transaction.Frequency;
                            var fUnit = transaction.increment;
                            var amount = transaction.CurrencyAmount;
                            int days = 0;
                            DateTime nP = new DateTime();
                            do
                            {
                                switch (fUnit)
                                {
                                    case RegularTransaction.FrequencyIncrement.Day:
                                        days = frequency;
                                        nP = lP.AddDays(days);
                                        break;
                                    case RegularTransaction.FrequencyIncrement.Week:
                                        days = frequency * 7;
                                        nP = lP.AddDays(days);
                                        break;
                                    case RegularTransaction.FrequencyIncrement.Month:
                                        nP = lP.AddMonths(frequency);
                                        break;
                                }
                                if (nP.Date <= DateTime.Now.Date)
                                {
                                    if (transaction.Incoming)
                                    {
                                        total += amount;
                                    }
                                    else
                                    {
                                        total -= amount;
                                    }
                                    lP = nP;
                                }
                            }
                            while (nP.Date < DateTime.Now.Date);
                            transaction.LPDate = lP;
                            updateTasks.Add(SaveRegularTransactionAsync(transaction));
                        }
                        var tempAccount = new Account();
                        foreach (var account in accounts)
                        {
                            if (accountId == account.Id)
                            {
                                tempAccount = account;
                                break;
                            }
                        }
                        if (tempAccount.Id == accountId)
                        {
                            total += tempAccount.CurrencyUnitAmount;
                            tempAccount.CurrencyUnitAmount = total;
                            updateTasks.Add(SaveAccountAsync(tempAccount));
                        }
                    }
                    while (tasks.Count > 0);
                    
                }
                await Task.WhenAll(updateTasks);
                doing = false;
            }).Wait();
            
        }





        
    
    //get accounts
    //get Regular transactions by account id
    //get outgoing or incoming Regular Transactions by account id
    //get account by account id.
    //insert or update account.
    // delete account
    //insert or update regular transaction
    //delete RegularTransaction
    SQLiteAsyncConnection Database;
        
        
        public AccountDAO()
        {
            doing = false;
            first = true;
        }
        async Task Init()
        {
            if (Database is not null)
            {
                return;
            }

            Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            await Database.CreateTableAsync(typeof(Account));
            await Database.CreateTableAsync(typeof(RegularTransaction));
            await Database.CreateTableAsync(typeof(DatePersist));

            //await Database.DropTableAsync<DatePersist>();
            //await Database.obliterateTableintothevoidandneverbothermeagain<DatePersist>();

            
            
            
            

        }

        public async Task<List<BudgetHelperApplication.Models.Account>> GetAccountsAsync()
        {
            await Init();

            
            return await Database.Table<BudgetHelperApplication.Models.Account>().ToListAsync();
        }

        public async Task<List<RegularTransaction>> GetRegularTransactionsAsync(long accountId)
        {
            await Init();
            
            return await Database.Table<RegularTransaction>().Where(t => t.AccountId == accountId).ToListAsync();

            // SQL queries are also possible
            //return await Database.QueryAsync<TodoItem>("SELECT * FROM [TodoItem] WHERE [Done] = 0");
        }

        public async Task<List<RegularTransaction>> GetRegularTransactionsAsync(long accountId, bool incoming)
        {
            await Init();
            
            return await Database.Table<RegularTransaction>().Where(t => t.AccountId == accountId && t.Incoming == incoming).ToListAsync();
        }

        public async Task<BudgetHelperApplication.Models.Account> GetAccountAsync(long id)
        {
            await Init();
            await UpdateDate();
            return await Database.Table<BudgetHelperApplication.Models.Account>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> SaveAccountAsync(BudgetHelperApplication.Models.Account account)
        {
            await Init();
            
            if (account.Id != 0)
            {
                return await Database.UpdateAsync(account);
            }
            else
            {
                return await Database.InsertAsync(account);
            }
        }

        public async Task<int> SaveRegularTransactionAsync(RegularTransaction rT)
        {
            await Init();
            if (rT.Id != 0)
            {
                return await Database.UpdateAsync(rT);
            }
            else
            {
                return await Database.InsertAsync(rT);
            }
        }

        public async Task<int> DeleteAccountAsync(BudgetHelperApplication.Models.Account account)
        {
            await Init();
            var rTs = await GetRegularTransactionsAsync(account.Id);
            foreach (var rT in rTs)
            {
                await Database.DeleteAsync(rT);
            }
            return await Database.DeleteAsync(account);
        }

        public async Task<int> DeleteRegularTransactionAsync(RegularTransaction rT)
        {
            return await Database.DeleteAsync(rT);
        }

        public async Task WaitForANewDay()
        {
            await Init();
            await Database.Table<DatePersist>().ToListAsync().ContinueWith(async (t) =>
            {
                if (t.Result.Count == 0)
                {
                    lastUsed = DateTime.Now.Date.AddDays(-1);
                    var temp = new DatePersist();
                    temp.Id = 0;
                    temp.LastUpdated = DateTime.Now.Date.AddDays(-1);
                    await Database.InsertAsync(temp);

                }
                else
                {
                    lastUsed = t.Result[0].LastUpdated.Date;
                   // if (first) lastUsed = DateTime.Now.Date.AddDays(-1);
                    

                    
                    
                }
                DateTime nextUpdated = DateTime.Now;
                if (lastUsed == DateTime.Now.Date || doing)
                {
                   nextUpdated = DateTime.Now.AddDays(1).Date;
                }

                Task.Delay(nextUpdated - DateTime.Now).ContinueWith(async (t) => 
                {
                    await UpdateDate();

                }).Wait();

                
                
            });

            
        }
    }
}
