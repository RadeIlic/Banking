﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Account
    {
        private string owner;
        private string accountName;
        private int amount;

        public string  Owner
        {
            get { return owner; }
            set { owner = value; }
        }

        public string AccountName
        {
            get { return accountName; }
            set { accountName = value; }
        }

        public int Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        public Account(string owner, string accountName)
        {
            this.Owner = owner;
            this.AccountName = accountName;
            this.Amount = 0;
        }
    }
}
