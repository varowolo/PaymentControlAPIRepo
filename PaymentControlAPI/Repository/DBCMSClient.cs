using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using Dapper;
using PaymentControlAPI.Responses;
using Newtonsoft.Json;
using PaymentControlAPI.Utilities;

namespace PaymentControlAPI.Repository
{
    public class DBCMSClient
    {
        public List<GetCustomerInfoResponse> GetCustomerCardInfo2(string accountNo, string connString)
        {
            //var reader = conn.QueryMultiple("ProductSearch", param: new { CategoryID = 1, SubCategoryID = "", PageNumber = 1 }, commandType: CommandType.StoredProcedure);
            //var CategoryOneList = reader.Read<CategoryOne>().ToList();
            //var CategoryTwoList = reader.Read<CategoryTwo>().ToList();

            try 
            {
                using (IDbConnection db = new SqlConnection(connString))
                {
                    SqlParameter[] param = {
                new SqlParameter("@AccountNo",accountNo)
                };
                    string readSp = "PaymentControl_FetchCardList";
                    return db.Query<GetCustomerInfoResponse>(readSp, param, commandType: CommandType.StoredProcedure).ToList();

                    //var rdr =  db.QueryMultiple(readSp, param, commandType: CommandType.StoredProcedure);

                    //var resultListInfo = rdr.Read<GetCustomerInfoResponse>().ToList();
                    //var resultListCard = rdr.Read<GetCustomerInfoResponse>().ToList();
                    //return resultListCard;
                    //return db.Query<IntraTranQuery>(readSp, param, commandType: CommandType.StoredProcedure).ToList();
                }
            }
            catch (Exception ex)
            {
                string err = ex.Message + " " + ex.StackTrace;
                //new LogWriter(_env, JsonConvert.SerializeObject(ex), "ErrorLog");
                //return new GetCustomerInfoResponse { };
                return new List<GetCustomerInfoResponse> { };

            }

        }


        public List<card> GetCustomerCardInfo(string accountNo, string connString)
        {
            try
            {

                var myCardDetails = new List<card>();
                var readSp = "PaymentControl_FetchCardList";                
                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@AccountNo", accountNo);

                using (IDbConnection db = new SqlConnection(connString))
                {
                    db.Open(); //<--open the connection
                    var retlist = db.Query<dynamic>(readSp, dynamicParameters, commandType: CommandType.StoredProcedure)
                         .Select(x => new card
                         {
                             pan = x.pan,
                             //tokenizedPan = Encryptor.SHA256Hash(x.tokenizedPan).Trim(),
                             tokenizedPan = Encryptor.SHA256Hash(x.tokenizedPan).Trim(),
                             expiry = x.expiry
                         }).ToList();

                    return retlist;
                }
            }
            catch (Exception ex)
            {
                string err = ex.Message + " " + ex.StackTrace;
                return new List<card> { };

            }

        }

        public List<customerInfo> GetCustomerInfo(string accountNo, string connString)
        {
            try
            {
                var myCustomerInfo = new List<customerInfo>();
                var readSp = "PaymentControl_CustInfo";
                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@AccountNo", accountNo);

                using (IDbConnection db = new SqlConnection(connString))
                {
                    db.Open(); //<--open the connection
                    return db.Query<customerInfo>(readSp, dynamicParameters, commandType: CommandType.StoredProcedure).ToList();
                }

            }
            catch (Exception ex)
            {
                string err = ex.Message + " " + ex.StackTrace;
                return new List<customerInfo> { };

            }

        }



    }
}
