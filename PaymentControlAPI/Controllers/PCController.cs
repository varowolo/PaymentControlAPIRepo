using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PaymentControlAPI.BLL;
//using PaymentControlAPI.Model;
using PaymentControlAPI.Models;
using PaymentControlAPI.Payload;
using PaymentControlAPI.Responses;
using PaymentControlAPI.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Routing;

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Security.Cryptography;

namespace PaymentControlAPI.Controllers
{
    [ApiController]
    public class PCController : ControllerBase
    {
        public readonly IConfiguration _config;
        //public PaymentDBContext _db;
        public PaymentControlDBContext _db;
        public IHostEnvironment _env;
        //public  PCController(IConfiguration config, PaymentDBContext db,IHostEnvironment env)
        //{
        //    _db = db;
        //    _env = env;
        //    _config = config;
        //}
        public PCController(IConfiguration config, PaymentControlDBContext db, IHostEnvironment env)
        {
            _db = db;
            _env = env;
            _config = config;
        }
        [HttpPost]

        [Route("authenticate")] 
        public async Task<IActionResult> authenticate( LoginPayload userinfo) //public async Task<LoginResponse> authenticate( LoginPayload userinfo)
        {
            try
            {
                int respCode;
                string responseString = "";
                LoginResponse resp;
                DateTime resquestTime;
                DateTime responseTime;
                HttpResponseMessage httpResponseMsg;
                TblRequestAndReponse requestForDb = new TblRequestAndReponse();
                string json = "";

                LoginResponse myOwnResp;
                LoginUserPayload _ADauth = new LoginUserPayload();


                string _ADServiceUrl = _config.GetValue<string>("ADServiceUrl");

                if (string.IsNullOrEmpty(userinfo.email) || string.IsNullOrEmpty(userinfo.password) || string.IsNullOrEmpty(userinfo.requestId) || string.IsNullOrEmpty(userinfo.signature)) //Validate data false value == "" || value.Length > 35
                {
                    resquestTime = DateTime.Now;
                    requestForDb = new TblRequestAndReponse() { RequestPayload = JsonConvert.SerializeObject(userinfo), RequestTimestamp = resquestTime, RequestType = "Login" };

                    json = JsonConvert.SerializeObject(userinfo);
                    var dat = new StringContent(json, Encoding.UTF8, "application/json");

                    httpResponseMsg = new HttpResponseMessage();
                    respCode = (int)httpResponseMsg.StatusCode;


                    httpResponseMsg = new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = "Invalid input parameters!!!" };
                    responseString = httpResponseMsg.ReasonPhrase.ToString();

                    responseTime = DateTime.Now;
                    requestForDb.ResponseTimestamp = responseTime;

                    myOwnResp = new LoginResponse();
                    myOwnResp.authenticated = false;
                    myOwnResp.message = responseString;
                    myOwnResp.name = "";
                    resp = myOwnResp;

                    requestForDb.Response = responseString;
                    _db.TblRequestAndReponse.Add(requestForDb);
                    await _db.SaveChangesAsync();
                    //return resp;
                    return BadRequest(resp);
                }
                else
                {
                    string signature = Encryptor.myhmac512Hash(userinfo.email + userinfo.password + userinfo.requestId);

                    if ((userinfo.signature == signature)) //Validate data false value == "" || value.Length > 35
                    {
                        HttpClient client = new HttpClient();
                        //client.BaseAddress = new Uri("http://localhost:64680/LoginAdmin"); //new Uri("http://localhost:64195/");
                        client.BaseAddress = new Uri(_ADServiceUrl); //new Uri("http://localhost:64195/");

                        resquestTime = DateTime.Now;
                        requestForDb = new TblRequestAndReponse() { RequestPayload = JsonConvert.SerializeObject(userinfo), RequestTimestamp = resquestTime, RequestType = "Login" };

                        _ADauth.username = userinfo.email;
                        _ADauth.password = userinfo.password;

                        //json = JsonConvert.SerializeObject(userinfo);
                        json = JsonConvert.SerializeObject(_ADauth);
                        var data = new StringContent(json, Encoding.UTF8, "application/json");
                        //client.Timeout = TimeSpan.FromMilliseconds(10000); //10 seconds  == 10,000 milliseconds

                        try
                        {
                            httpResponseMsg = client.PostAsync(client.BaseAddress, data).Result;
                            //client.PostAsync(client.BaseAddress, data).Result
                        }
                        catch (Exception ex)
                        {
                            new LogWriter(_env, JsonConvert.SerializeObject(ex), "ErrorLog");
                            //return new LoginResponse { authenticated = false, message = "Something went wrong" };
                            return BadRequest(new LoginResponse { authenticated = false, message = "Something went wrong" });
                        }

                        if (httpResponseMsg.IsSuccessStatusCode)
                        {
                            respCode = (int)httpResponseMsg.StatusCode;
                            responseString = httpResponseMsg.Content.ReadAsStringAsync().Result;
                        }

                        responseTime = DateTime.Now;
                        requestForDb.ResponseTimestamp = responseTime;
                        resp = JsonConvert.DeserializeObject<LoginResponse>(responseString);
                        requestForDb.Response = responseString;
                        _db.TblRequestAndReponse.Add(requestForDb);
                        await _db.SaveChangesAsync();
                        //return resp;                       
                        return Ok(resp);
                    }
                    else
                    {
                        resquestTime = DateTime.Now;
                        requestForDb = new TblRequestAndReponse() { RequestPayload = JsonConvert.SerializeObject(userinfo), RequestTimestamp = resquestTime, RequestType = "Login" };

                        json = JsonConvert.SerializeObject(userinfo);
                        var dat = new StringContent(json, Encoding.UTF8, "application/json");

                        httpResponseMsg = new HttpResponseMessage();
                        respCode = (int)httpResponseMsg.StatusCode;
 
                        httpResponseMsg = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "401 - Unauthorized!!!" };
                        responseString = httpResponseMsg.ReasonPhrase.ToString();

                        responseTime = DateTime.Now;
                        requestForDb.ResponseTimestamp = responseTime;

                        myOwnResp = new LoginResponse();
                        myOwnResp.authenticated = false;
                        myOwnResp.message = responseString;
                        myOwnResp.name = "";
                        resp = myOwnResp;

                        requestForDb.Response = responseString;
                        _db.TblRequestAndReponse.Add(requestForDb);
                        await _db.SaveChangesAsync();
                        //return resp;
                        return Unauthorized(resp);
                    }

                }                           

            }
            catch(Exception ex)
            {
                new LogWriter(_env, JsonConvert.SerializeObject(ex), "ErrorLog");
                //return new LoginResponse { authenticated = false, message = "Something went wrong" };                
                return BadRequest(new LoginResponse { authenticated = false, message = "Something went wrong" });
            }
        }

        [HttpPost]
        [Route("getCustomerInfo")]
        public async Task<IActionResult> GetCustomerInfo(AccountNumberPayload detail)  //public async Task<GetCustomerInfoResponse> GetCustomerInfo(AccountNumberPayload detail)
        {
            try
            {             
                int respCode;
                string responseString = "";
                GetCustomerInfoResponse resp;
                GetCustomerInfoResponse myOwnResp;
                List<card> myCardResp = new List<card>();  
                card myCard = new card();

                DateTime resquestTime;
                DateTime responseTime;
                HttpResponseMessage httpResponseMsg;
                TblRequestAndReponse requestForDb = new TblRequestAndReponse();
                string json = "";

                if (string.IsNullOrEmpty(detail.accountNumber) || string.IsNullOrEmpty(detail.requestId) || string.IsNullOrEmpty(detail.signature)) //Validate data false value == "" || value.Length > 35
                {
                    resquestTime = DateTime.Now;
                    requestForDb = new TblRequestAndReponse() { RequestPayload = JsonConvert.SerializeObject(detail), RequestTimestamp = resquestTime, RequestType = "GetCustomerInfo" };

                    json = JsonConvert.SerializeObject(detail);
                    var dat = new StringContent(json, Encoding.UTF8, "application/json");

                    httpResponseMsg = new HttpResponseMessage();
                    respCode = (int)httpResponseMsg.StatusCode;

                    httpResponseMsg = new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = "Invalid input parameters!!!" };
                    responseString = httpResponseMsg.ReasonPhrase.ToString();

                    responseTime = DateTime.Now;
                    requestForDb.ResponseTimestamp = responseTime;
                    myOwnResp = new GetCustomerInfoResponse();

                    myOwnResp.accountName = responseString;
                    myOwnResp.coreBankingId = "";
                    myOwnResp.currencyCode = "";

                    myCard.expiry = "";
                    myCard.tokenizedPan = "";

                    myCardResp.Add(myCard);

                    myOwnResp.cards = myCardResp;

                    resp = myOwnResp;
                    responseString = JsonConvert.SerializeObject(resp);

                    requestForDb.Response = responseString;
                    _db.TblRequestAndReponse.Add(requestForDb);
                    await _db.SaveChangesAsync();
                    ////return resp;
                    //return CreateResponse(HttpStatusCode.OK, resp);
                    return BadRequest(resp);
                }

                else
                {
                    string signature = Encryptor.myhmac512Hash(detail.accountNumber + detail.requestId);

                    if ((detail.signature == signature))  //Validate data false value == "" || value.Length > 35
                    {                      
                        resquestTime = DateTime.Now;
                        requestForDb = new TblRequestAndReponse() { RequestPayload = JsonConvert.SerializeObject(detail), RequestTimestamp = resquestTime, RequestType = "getCustomerInfo" };
                        resp = new GetCustomerDetailsByAccountNumber(_config, _env, detail.accountNumber).GetCustomerDetails();
                        responseTime = DateTime.Now;
                        requestForDb.ResponseTimestamp = responseTime;
                        var stringResp = JsonConvert.SerializeObject(resp);
                        requestForDb.Response = stringResp;
                        _db.TblRequestAndReponse.Add(requestForDb);
                        await _db.SaveChangesAsync();
                        ////return resp;
                        //return Ok(resp);

                        if (resp.accountName != "")
                        {
                            return Ok(resp);
                        }
                        else
                        {
                            return NotFound(resp);
                        }
                        
                    }
                    else
                    {
                        resquestTime = DateTime.Now;
                        requestForDb = new TblRequestAndReponse() { RequestPayload = JsonConvert.SerializeObject(detail), RequestTimestamp = resquestTime, RequestType = "GetCustomerInfo" };

                        json = JsonConvert.SerializeObject(detail);
                        var dat = new StringContent(json, Encoding.UTF8, "application/json");

                        httpResponseMsg = new HttpResponseMessage();
                        respCode = (int)httpResponseMsg.StatusCode;

                        httpResponseMsg = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "401 - Unauthorized!!!" };
                        responseString = httpResponseMsg.ReasonPhrase.ToString();

                        responseTime = DateTime.Now;
                        requestForDb.ResponseTimestamp = responseTime;
                        myOwnResp = new GetCustomerInfoResponse();
                        myOwnResp.accountName = responseString;
                        myOwnResp.coreBankingId = "";
                        myOwnResp.currencyCode = "";

                        myCard.expiry = "";
                        myCard.tokenizedPan = "";

                        myCardResp.Add(myCard);

                        myOwnResp.cards = myCardResp;

                        resp = myOwnResp;
                        responseString = JsonConvert.SerializeObject(resp);
                        requestForDb.Response = responseString;
                        _db.TblRequestAndReponse.Add(requestForDb);
                        await _db.SaveChangesAsync();
                        //return resp;
                        return Unauthorized(resp);
                    }

                }
                             
            }
            catch(Exception ex)
            {
                new LogWriter(_env, JsonConvert.SerializeObject(ex), "ErrorLog");
                //return new GetCustomerInfoResponse {  };
                return BadRequest(new GetCustomerInfoResponse { });

            }
        }

        [HttpPost]
        [Route("/api/pc/receiveNotification")] 
        public  async Task<IActionResult> ReceiveNotification(NotificationPayload detail) //public  async Task<NotificationResponse> ReceiveNotification(NotificationPayload detail)
        {
            try
            {
                string _receiveNotofocationSubject = _config.GetValue<string>("receiveNotificationSubject");

                int respCode;
                string responseString = "";
                //TblCustNotificationLog resp;
                AccountInfo resp;
                DateTime resquestTime;
                DateTime responseTime;
                HttpResponseMessage httpResponseMsg;
                TblCustNotificationLog requestForDb = new TblCustNotificationLog();
                string json = "";

                if (string.IsNullOrEmpty(detail.accountNumber) || string.IsNullOrEmpty(detail.controlId) || string.IsNullOrEmpty(detail.createDate) || string.IsNullOrEmpty(detail.notificationMessage) || string.IsNullOrEmpty(detail.notificationType) || string.IsNullOrEmpty(detail.requestId) || string.IsNullOrEmpty(detail.signature))//Validate data false value == "" || value.Length > 35
                {
                    resquestTime = DateTime.Now;
                    requestForDb = new TblCustNotificationLog { ControlId = detail.controlId, AccountNumber = detail.accountNumber, NotificationType = detail.notificationType, NotificationMessage = detail.notificationMessage, CreateDate = detail.createDate, RequestId = detail.requestId, Signature = detail.signature, Subject = _receiveNotofocationSubject, acknowledgedResponse = "false", acknowledgedReason = "Invalid parameter(s)" };

                    json = JsonConvert.SerializeObject(detail);
                    var dat = new StringContent(json, Encoding.UTF8, "application/json");

                    httpResponseMsg = new HttpResponseMessage();
                    respCode = (int)httpResponseMsg.StatusCode;

                    httpResponseMsg = new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = "Invalid input parameters!!!" };
                    responseString = httpResponseMsg.ReasonPhrase.ToString();

                    responseTime = DateTime.Now;

                    _db.TblCustNotificationLog.Add(requestForDb);
                    await _db.SaveChangesAsync();
                    //return new NotificationResponse { acknowledged = false };
                    return BadRequest(new NotificationResponse { acknowledged = false });
                }
                else
                {
                    string signature = Encryptor.myhmac512Hash(detail.controlId + detail.accountNumber + detail.notificationType + detail.requestId);

                    if ((detail.signature == signature)) //Validate data false value == "" || value.Length > 35
                    {
                        //requestForDb = new TblCustNotificationLog { ControlId = detail.controlId, AccountNumber = detail.accountNumber, NotificationType = detail.notificationType, NotificationMessage = detail.notificationMessage, CreateDate = detail.createDate, RequestId = detail.requestId, Signature = detail.signature, Subject = _receiveNotofocationSubject, acknowledgedResponse = "true", acknowledgedReason = "Valid signature" };
                        //_db.TblCustNotificationLog.Add(requestForDb);
                        //await _db.SaveChangesAsync();
                        ////return new NotificationResponse { acknowledged = true };
                        //return Ok(new NotificationResponse { acknowledged = true });

                        resquestTime = DateTime.Now;
                        requestForDb = new TblCustNotificationLog { ControlId = detail.controlId, AccountNumber = detail.accountNumber, NotificationType = detail.notificationType, NotificationMessage = detail.notificationMessage, CreateDate = detail.createDate, RequestId = detail.requestId, Signature = detail.signature, Subject = _receiveNotofocationSubject, acknowledgedResponse = "true", acknowledgedReason = "Valid signature" };
                        resp = new GetCustomerDetailsByAccountNumber(_config, _env, detail.accountNumber).GetAccountInfo();
                        responseTime = DateTime.Now;
                        //requestForDb.ResponseTimestamp = responseTime;
                        var stringResp = JsonConvert.SerializeObject(resp);
                        //requestForDb.Response = stringResp;
                        _db.TblCustNotificationLog.Add(requestForDb);
                        await _db.SaveChangesAsync();

                        if (resp.accountName != "")
                        {
                            return Ok(new NotificationResponse { acknowledged = true });
                        }
                        else
                        {
                            return NotFound(new NotificationResponse { acknowledged = false });
                        }
                    }
                    else
                    {
                        resquestTime = DateTime.Now;
                        requestForDb = new TblCustNotificationLog { ControlId = detail.controlId, AccountNumber = detail.accountNumber, NotificationType = detail.notificationType, NotificationMessage = detail.notificationMessage, CreateDate = detail.createDate, RequestId = detail.requestId, Signature = detail.signature, Subject = _receiveNotofocationSubject, acknowledgedResponse = "false", acknowledgedReason = "Invalid signature" };

                        json = JsonConvert.SerializeObject(detail);
                        var dat = new StringContent(json, Encoding.UTF8, "application/json");

                        httpResponseMsg = new HttpResponseMessage();
                        respCode = (int)httpResponseMsg.StatusCode;

                        httpResponseMsg = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Unauthorized!!!" };
                        responseString = httpResponseMsg.ReasonPhrase.ToString();

                        responseTime = DateTime.Now;

                        _db.TblCustNotificationLog.Add(requestForDb);
                        await _db.SaveChangesAsync();
                        //return new NotificationResponse { acknowledged = false };
                        return Unauthorized(new NotificationResponse { acknowledged = false });
                    }

                }                             

            }
            catch(Exception ex)
            {
                if (ex.InnerException.Message.ToLower().Contains("violation of unique key constraint"))
                {
                    new LogWriter(_env, JsonConvert.SerializeObject(ex), "ErrorLog");
                    //return new NotificationResponse {acknowledged=false };
                    return BadRequest(new NotificationResponse { acknowledged = false });
                }
                else
                {
                    new LogWriter(_env, JsonConvert.SerializeObject(ex), "ErrorLog");
                    //return new NotificationResponse {acknowledged=false };
                    return BadRequest(new NotificationResponse { acknowledged = false });
                }                
            }
        }

        [HttpPost]
        [Route("/api/pc/sendEmail")] 
        public async Task<IActionResult> sendEmail(SendEmailPayload detail) //public async Task<SendEmailResponse> sendEmail(SendEmailPayload detail)
        {
            try
            {

                int respCode;
                string responseString = "";
                TblEmailLog resp;
                DateTime resquestTime;
                DateTime responseTime;
                HttpResponseMessage httpResponseMsg;
                TblEmailLog requestForDb = new TblEmailLog();
                string json = "";

                if (string.IsNullOrEmpty(detail.Body) || string.IsNullOrEmpty(detail.RequestId) || string.IsNullOrEmpty(detail.Signature) || string.IsNullOrEmpty(detail.Subject) || string.IsNullOrEmpty(detail.To)) //Validate data false value == "" || value.Length > 35
                {
                    resquestTime = DateTime.Now;
                    requestForDb = new TblEmailLog { Body = detail.Body, DropTimestamp = DateTime.Now, RequestId = detail.RequestId, Signature = detail.Signature, Subject = detail.Subject, ToEmail = detail.To, acknowledgedResponse = "false", acknowledgedReason = "Invalid parameter(s)" };

                    json = JsonConvert.SerializeObject(detail);
                    var dat = new StringContent(json, Encoding.UTF8, "application/json");

                    httpResponseMsg = new HttpResponseMessage();
                    respCode = (int)httpResponseMsg.StatusCode;

                    httpResponseMsg = new HttpResponseMessage(HttpStatusCode.BadRequest) { ReasonPhrase = "Invalid input parameters!!!" };
                    responseString = httpResponseMsg.ReasonPhrase.ToString();

                    responseTime = DateTime.Now;               

                    _db.TblEmailLog.Add(requestForDb);
                    await _db.SaveChangesAsync();
                    //return new SendEmailResponse { acknowledged = false };
                    return BadRequest(new SendEmailResponse { acknowledged = false });
                }
                else
                {
                    string signature = Encryptor.myhmac512Hash(detail.To + detail.RequestId);

                    if ((detail.Signature == signature)) //Validate data false value == "" || value.Length > 35
                    {                       
                        requestForDb = new TblEmailLog { Body = detail.Body, DropTimestamp = DateTime.Now, RequestId = detail.RequestId, Signature = detail.Signature, Subject = detail.Subject, ToEmail = detail.To, acknowledgedResponse = "true", acknowledgedReason = "Valid signature" };
                        _db.TblEmailLog.Add(requestForDb);
                        await _db.SaveChangesAsync();
                        //return new SendEmailResponse { acknowledged = true };
                        return Ok(new SendEmailResponse { acknowledged = true });
                    }
                    else
                    {
                        resquestTime = DateTime.Now;
                        requestForDb = new TblEmailLog { Body = detail.Body, DropTimestamp = DateTime.Now, RequestId = detail.RequestId, Signature = detail.Signature, Subject = detail.Subject, ToEmail = detail.To, acknowledgedResponse = "false", acknowledgedReason = "Invalid signature" };

                        json = JsonConvert.SerializeObject(detail);
                        var dat = new StringContent(json, Encoding.UTF8, "application/json");

                        httpResponseMsg = new HttpResponseMessage();
                        respCode = (int)httpResponseMsg.StatusCode;

                        httpResponseMsg = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Unauthorized!!!" };
                        responseString = httpResponseMsg.ReasonPhrase.ToString();

                        responseTime = DateTime.Now;

                        _db.TblEmailLog.Add(requestForDb);
                        await _db.SaveChangesAsync();
                        //return new SendEmailResponse { acknowledged = false };
                        return Unauthorized(new SendEmailResponse { acknowledged = false });
                    }
                }                
           
            }
            catch (Exception ex)
            {

                if (ex.InnerException.Message.ToLower().Contains("violation of unique key constraint"))
                {
                    new LogWriter(_env, JsonConvert.SerializeObject(ex), "ErrorLog");
                    //return new NotificationResponse {acknowledged=false };
                    return BadRequest(new NotificationResponse { acknowledged = false });
                }
                else
                {
                    new LogWriter(_env, JsonConvert.SerializeObject(ex), "ErrorLog");
                    //return new NotificationResponse {acknowledged=false };
                    return BadRequest(new NotificationResponse { acknowledged = false });
                }
            }
        }

    }
}