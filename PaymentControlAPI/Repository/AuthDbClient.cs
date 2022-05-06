using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using Dapper;
using PaymentControlAPI.Responses;

namespace PaymentControlAPI.Repository
{
    public class AuthDbClient
    {
        //public List<IntraTranQuery> GetCustomerCardInfo(string accountNo, string connString)
        public List<GetCustomerInfoResponse> GetCustomerCardInfo(string accountNo, string connString)
        {
            using (IDbConnection db = new SqlConnection(connString))
            {
                SqlParameter[] param = {
                new SqlParameter("@AccountNo",accountNo)
            };
                string readSp = "PaymentControl_FetchCardList";
                return db.Query<GetCustomerInfoResponse>(readSp, param, commandType: CommandType.StoredProcedure).ToList();
                //return db.Query<IntraTranQuery>(readSp, param, commandType: CommandType.StoredProcedure).ToList();
            }
        }


    }
}
