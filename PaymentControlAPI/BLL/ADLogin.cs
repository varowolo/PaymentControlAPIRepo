using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using PaymentControlAPI.Responses;
using PaymentControlAPI.Utilities;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using DirectoryEntry = System.DirectoryServices.DirectoryEntry;

namespace PaymentControlAPI.BLL
{
    public class ADLogin
    {
        public  IConfiguration _config;
        public   IHostEnvironment _env;
        public ADLogin(IConfiguration config, IHostEnvironment env)
        {
            _config = config;
            _env = env;
        }
            public  LoginResponse AuthenticateUser(string user, string pass)
            {

            user = user.Contains("@") ? user.Split("@")[0] : user;
                    string path;
                    path = _config.GetSection("LDAP").Value.ToString();

               DirectoryEntry de = new DirectoryEntry(path, user, pass, AuthenticationTypes.Secure);
                    try
                    {
                        DirectorySearcher ds = new DirectorySearcher(de);
                        ds.FindOne();
                return GetActiveDirUserDetails(user);
                    }
                    catch
                    {
                        // otherwise, it will crash out so return false
                        return new LoginResponse { authenticated=false,message="Invalid Username or Password", name=""};
                    }
                
            }
            public  LoginResponse GetActiveDirUserDetails(string userid)
            {
              
                    //-----uncomment this ---code --- to get uet forstname and lastname from -- AD
                    string path;
                path = _config.GetSection("LDAP").Value.ToString();
            var ldapusername= _config.GetSection("LDAPUsername").Value.ToString();
            var ldappassword = _config.GetSection("LDAPPassword").Value.ToString();

            System.DirectoryServices.DirectoryEntry dirEntry;
                    System.DirectoryServices.DirectorySearcher dirSearcher;
                    string domainName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
                    try
                    {

                        dirEntry = new System.DirectoryServices.DirectoryEntry(path, ldapusername, ldappassword);
                        dirSearcher = new System.DirectoryServices.DirectorySearcher(dirEntry);
                        dirSearcher.Filter = "(userPrincipalName=" + userid + ")";
                        // dirSearcher.Filter = "(email=" + userid + ")";

                        dirSearcher.PropertiesToLoad.Add("GivenName");
                        // Users e-mail address
                        dirSearcher.PropertiesToLoad.Add("sn");
                        // Users last name
                        SearchResult sr = dirSearcher.FindOne();
                        if (sr == null)
                        {
                            new LogWriter(_env,"Could not find user", "ErrorLog");
                            dirSearcher.Filter = "(SAMAccountName=" + userid.Split('@')[0].ToLower() + ")";
                            SearchResult sr1 = dirSearcher.FindOne();
                            if (sr1 == null)
                            {
                                new LogWriter(_env,"Could not find user", "ErrorLog");
                                return new LoginResponse { authenticated = false, message = "User not found on active Directory" };
                            }
                            // return "User not found on active Directory";
                            else
                            {
                                System.DirectoryServices.DirectoryEntry de1 = sr1.GetDirectoryEntry();
                                var userFirstLastName1 = new LoginResponse { authenticated = true, name = de1.Properties["GivenName"].Value.ToString() +" " + de1.Properties["sn"].Value.ToString(), message= "Account authenticated" };
                                return userFirstLastName1;
                            }
                        }
                        System.DirectoryServices.DirectoryEntry de = sr.GetDirectoryEntry();
                var userFirstLastName = new LoginResponse { authenticated = true, name = de.Properties["GivenName"].Value.ToString() + " " + de.Properties["sn"].Value.ToString(), message = "Account authenticated" };
                return userFirstLastName;
                    }
                    catch (Exception ex)
                    {
                        new LogWriter(_env, JsonConvert.SerializeObject(ex), "ErrorLog");

                return new LoginResponse { authenticated = false, message = "Invalid Username or Password", name = "" };
            }

        }
        
    }
}
