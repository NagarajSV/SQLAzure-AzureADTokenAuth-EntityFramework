using System;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Configuration;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;

namespace EF_AADTokenAuth.Helpers
{
    public class TokenFactory
    {
        //Fetching Azure AD related client and resource (Azure SQL) details from web.config file
        private static string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        private static string tenant = ConfigurationManager.AppSettings["ida:Tenant"];
        private static string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private static string certName = ConfigurationManager.AppSettings["ida:CertName"];
        private static string sqlDBResourceId = ConfigurationManager.AppSettings["sqldb:ResourceId"];

        static string identityProvider = String.Format(CultureInfo.InvariantCulture, aadInstance, tenant);

        //Public Method that consumers call to get access token
        public static string GetAccessToken()
        {
            //Preparing Client Credentials
            //Client Credentials = ClientID + ClientCertificate
            X509Certificate2 clientCert = GetClientCertificate();
            ClientAssertionCertificate clientCreds = new ClientAssertionCertificate(clientId, clientCert);

            //using the clientCreds to get access token from Azure AD
            AuthenticationContext authContext = new AuthenticationContext(identityProvider);

            AuthenticationResult authResult = null; 
            try
            {
                authResult = authContext.AcquireToken(sqlDBResourceId, clientCreds); 
            }
            catch (AdalException exception)
            {
                throw new Exception(exception.Message); 
            }

            return authResult.AccessToken; 
        }

        //Get's client certificate from current user store by the given subjet name
        private static X509Certificate2 GetClientCertificate()
        {
            X509Certificate2 clientCert = null;
            X509Store certStore = new X509Store(StoreLocation.CurrentUser);

            try
            {
                certStore.Open(OpenFlags.ReadOnly);

                X509Certificate2Collection allUserCerts = certStore.Certificates;
                X509Certificate2Collection nonExpiredCerts = allUserCerts.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
                X509Certificate2Collection allClientCerts = nonExpiredCerts.Find(X509FindType.FindBySubjectDistinguishedName, certName, false);

                if (allClientCerts.Count == 0)
                    return null;

                clientCert = allClientCerts[0];
            }
            finally
            {
                certStore.Close();
            }

            return clientCert;
        }
    }
}
