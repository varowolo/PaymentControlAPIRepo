using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using PaymentControlAPI.Responses;
using PaymentControlAPI.Utilities;
using PaymentControlAPI.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace PaymentControlAPI.BLL
{
    public class GetCustomerDetailsByAccountNumber
    {
        public string _accountNumber;
        public IConfiguration _config;
        public IHostEnvironment _env;
        public GetCustomerDetailsByAccountNumber(IConfiguration config, IHostEnvironment env,string accountNumber)
        {
            _accountNumber = accountNumber;
            _config = config;
            _env = env;

        }

        public  GetCustomerInfoResponse GetCustomerDetails()
        {


            try
            {
                //// Fetch customer card details from FEP or postillion here.
                //var cards = new List<card>();
                //cards.Add(new card { pan = "539941*******5846", tokenizedPan = "2adbasdlkda1398138nadshasjda21", expiry = "09/22" });
                //return new GetCustomerInfoResponse() {accountName="0012345678",currencyCode="NGN",cards =cards };

                //Fetch customer card details from CMS here.  GetCustomerCardInfo
                var cards = new List<card>();
                List<customerInfo> mycustinfo = new List<customerInfo>();
                List<card> mycardinfo = new List<card>();
                List<card> mycustcardinfo = new List<card>();

                //List<GetCustomerInfoResponse> mycustcardinfo = new List<GetCustomerInfoResponse>();
                DBCMSClient mydbclient = new DBCMSClient();
                mycustcardinfo = mydbclient.GetCustomerCardInfo(_accountNumber, _config.GetConnectionString("CMSConn"));

                //mycustcardinfo[0].
                if (mycustcardinfo.Count == 0)
                {
                    var mycard = new card();
                    mycard.expiry = "";
                    mycard.pan = "";
                    mycard.tokenizedPan = "";
                    mycustcardinfo.Add(mycard);
                    cards = mycustcardinfo;
                }

                mycustinfo = mydbclient.GetCustomerInfo(_accountNumber, _config.GetConnectionString("CMSConn"));
                //mycardinfo = mydbclient.GetCustomerCardInfo(_accountNumber, _config.GetConnectionString("CMSConn"));
                cards = mycustcardinfo; // mycardinfo;

                if (mycustinfo.Count == 0)
                {
                    return new GetCustomerInfoResponse()
                    {
                        accountName = "",
                        currencyCode = "",
                        coreBankingId = "",
                        cards = cards
                    };
                }
                else 
                {
                    return new GetCustomerInfoResponse()
                    {
                        accountName = mycustinfo[0].accountName,
                        currencyCode = mycustinfo[0].currencyCode,
                        coreBankingId = mycustinfo[0].coreBankingId,
                        cards = cards
                    };
                }

                //return new GetCustomerInfoResponse() { accountName = mycustinfo[0].accountName, currencyCode = mycustinfo[0].currencyCode, 
                //                                        coreBankingId = mycustinfo[0].coreBankingId, cards = cards };
                ////cards.Add(new card { pan = "539941*******5846", tokenizedPan = "2adbasdlkda1398138nadshasjda21", expiry = "09/22" });
                ////return new GetCustomerInfoResponse() { accountName = "0012345678", currencyCode = "NGN", cards = cards };
            }
            catch(Exception ex)
            {
                new LogWriter(_env, JsonConvert.SerializeObject(ex), "ErrorLog");

                return new GetCustomerInfoResponse() { };
            }
        }

        public AccountInfo GetAccountInfo()
        {

            try
            {
                //Fetch customer card details from CMS here.  GetCustomerCardInfo                
                List<customerInfo> mycustinfo = new List<customerInfo>();

                //List<GetCustomerInfoResponse> mycustcardinfo = new List<GetCustomerInfoResponse>();
                DBCMSClient mydbclient = new DBCMSClient();                

                mycustinfo = mydbclient.GetCustomerInfo(_accountNumber, _config.GetConnectionString("CMSConn"));

                if (mycustinfo.Count == 0)
                {
                    var myInfo = new customerInfo();
                    myInfo.accountName = "";
                    myInfo.coreBankingId = "";
                    myInfo.currencyCode = "";
                    mycustinfo.Add(myInfo);
                }

                return new AccountInfo()
                {                    
                    accountName = mycustinfo[0].accountName,
                    currencyCode = mycustinfo[0].currencyCode,
                    coreBankingId = mycustinfo[0].coreBankingId,
                };
            }
            catch (Exception ex)
            {
                new LogWriter(_env, JsonConvert.SerializeObject(ex), "ErrorLog");

                return new AccountInfo() { };
            }
        }

    }
}
