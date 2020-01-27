using BPal.Business.Portal.Core.Models;
using BPal.Business.Portal.Website.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPal.Business.Portal.Website.Helpers
{
    public static class ViewModelBuilder
    {
        // account
        public static AccountModel ToViewModel(this Account Entity)
        {
            if (Entity == null || Entity.AccountId == string.Empty)
                return null;

            var accountModel = new AccountModel
            {
                AccountId = Entity.AccountId,
                DateOfBirth = Entity.DateOfBirth,
                EmailAddress = Entity.EmailAddress,
                FirstName = Entity.FirstName,
                LastName = Entity.LastName,
                LastLogIn = Entity.LastLogIn,
                LastLoginLocation = Entity.LastLoginLocation,
                PhoneNumber = Entity.PhoneNumber
            };

            return accountModel;
        }

        public static List<AccountModel> ToViewModel(this List<Account> Entities)
        {
            if (Entities == null)
                return null;

            var accountModels = new List<AccountModel>();

            Entities.ForEach(Entity => accountModels.Add(ToViewModel(Entity)));

            return accountModels;
        }
    }
}
