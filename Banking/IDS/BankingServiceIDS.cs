﻿using Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.Configuration;

namespace IDS
{
    public class BankingServiceIDS : IBankingService
    {
        public IDSResult Check(Request request, int pin)
        {
            if(request.Type==RequestType.Payment)
            {

                if( CheckIfRequestsOverload(request.Account))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Payment failed.");
                    Console.ForegroundColor = ConsoleColor.White;
                    return IDSResult.BlockForOverload;
                }

                if (request.Account.PIN != pin)
                {
                    if (request.Account.LoginAttempts == Int32.Parse(ConfigurationManager.AppSettings["wrongPinAttemptsLimit"]) - 1)
                    {
                       
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Payment failed.");
                        Console.ForegroundColor = ConsoleColor.White;
                        return IDSResult.BlockForWrongPIN;
                    }

                    request.Account.LoginAttempts++;
                    return IDSResult.FailedPayment;
                }
                else
                {
                    request.Account.LoginAttempts = 0;
                }

                if (!request.IsOutgoing)
                {
                    if ((request.Account.DailyAmount + request.Amount) > Int32.Parse(ConfigurationManager.AppSettings["maxDailyIncomingAmount"]))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Raise a loan failed.");
                        Console.ForegroundColor = ConsoleColor.White;
                        return IDSResult.BlockForDailyLimit;
                    }
                    
                }

                Console.WriteLine("Payment done!");
                return IDSResult.OK;
            }
            else if(request.Type==RequestType.RaiseALoan)
            {
                if (CheckIfRequestsOverload(request.Account))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Raise a loan failed.");
                    Console.ForegroundColor = ConsoleColor.White;
                    return IDSResult.BlockForOverload;
                }

                if (request.Account.PIN != pin)
                {
                    if (request.Account.LoginAttempts == Int32.Parse(ConfigurationManager.AppSettings["wrongPinAttemptsLimit"]) - 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Raise a loan failed.");
                        Console.ForegroundColor = ConsoleColor.White;
                        return IDSResult.BlockForWrongPIN;
                    }

                    request.Account.LoginAttempts++;
                    Console.WriteLine("Raise a loan failed!");
                    return IDSResult.FailedLoan;
                }
                else
                {
                    request.Account.LoginAttempts = 0;
                    return IDSResult.OK;
                }


            }
            
            Console.WriteLine("Ne treba ovde da dodje!");
            return IDSResult.OK;
        }

        public bool CheckIfRequestsOverload(Account account)
        {
            bool retVal = false;

            lock (account)
            {
                if (account.IntevalBeginning == null || account.IntevalBeginning > DateTime.Now.AddSeconds(Int32.Parse(ConfigurationManager.AppSettings["requestsOverloadCheckInterval"])))
                {
                    account.IntevalBeginning = DateTime.Now;
                    account.RequestsCount = 1;
                }
                else if (account.RequestsCount + 1 > Int32.Parse(ConfigurationManager.AppSettings["requestsOverloadLimit"]))
                {
                    retVal = true;
                }
                else
                    ++account.RequestsCount;
            }

            return retVal;
        }
    }
}
