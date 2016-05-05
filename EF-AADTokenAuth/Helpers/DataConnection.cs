using System;
using System.Configuration;
using System.Data.SqlClient;

namespace EF_AADTokenAuth.Helpers
{
    public class DataConnection
    {
        //Retrives an access token using TokenFactory.GetAccessToken(). Uses it to open a SQL connection (ADO.NET code)
        //ONce the connection to SQL is opeened, returns the connection
        public static SqlConnection GetDatabaseConnection()
        {
            //Building a connection string
            SqlConnectionStringBuilder connBuilder = new SqlConnectionStringBuilder();
            connBuilder["Data Source"] = ConfigurationManager.AppSettings["db:server"];
            connBuilder["Initial Catalog"] = ConfigurationManager.AppSettings["db:database"];
            connBuilder["Connect Timeout"] = 30;

            //getting the access token for SQL Azure
            string accessToken = TokenFactory.GetAccessToken();
            if (accessToken == null)
            {
                throw new Exception("Failed to get access token to connect to Database");
            }

            //Establing a connection to SQL Azure by passing the built connection string 
            SqlConnection sqlConn = new SqlConnection(connBuilder.ConnectionString);
            try
            {
                //Injecting the access token to SQL connection so that it can make use of this access token when it opens a connection to database 
                //Note: This property is exposed only when the project targets .net framework 4.6 and above 
                sqlConn.AccessToken = accessToken;
                sqlConn.Open();
            }
            catch (Exception ex)
            {
                throw new Exception("Could not connect to the database", ex.InnerException);
            }

            //return opened SQL connection 
            return sqlConn;
        }
    }
}