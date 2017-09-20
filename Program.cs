using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Text;

namespace nav_object_timestamps_transmitter
{
    partial class Program
    {
        static void Main()
        {
            DateTime start = DateTime.Now;
            DataTable timestampData = CaptureTimestamps();
            DateTime end = DateTime.Now;
            string message = FormatMessage(ConfigurationManager.AppSettings["Tag"], timestampData, start, end);
            UploadTimestamps(message);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        private static DataTable CaptureTimestamps()
        {
            using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(timestampDataQuery, CreateSqlConnectionString()))
            {
                DataTable timestampData = new DataTable();
                sqlDataAdapter.Fill(timestampData);
                return timestampData;
            }
        }

        private static string CreateSqlConnectionString()
        {
            return string.Format(
                "Data Source={0}; Integrated Security=True; Initial Catalog={1};",
                ConfigurationManager.AppSettings["DatabaseServer"],
                ConfigurationManager.AppSettings["DatabaseName"]);
        }

        private static string FormatMessage(string tag, DataTable timestampData, DateTime start, DateTime end)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("{");
            stringBuilder.AppendFormat("\"tag\":\"{0}\",", tag);
            stringBuilder.AppendFormat("\"start\":\"{0}\",", start.ToUniversalTime().ToString("yyyy-MM-dd hh:mm:ss") + " UTC");
            stringBuilder.AppendFormat("\"end\":\"{0}\",", start.ToUniversalTime().ToString("yyyy-MM-dd hh:mm:ss") + " UTC");
            stringBuilder.Append("data:[");
            foreach (DataRow objectTimestamp in timestampData.Rows)
            {
                stringBuilder.AppendFormat("{{\"type\":{0},\"id\":{1},\"timestamp\":{2}}},", objectTimestamp[0], objectTimestamp[1], objectTimestamp[2]);
            }
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            stringBuilder.Append("]}");
            return stringBuilder.ToString();
        }

        private static void UploadTimestamps(string message)
        {
            using (WebClient webClient = new WebClient())
            {
                webClient.UploadString(ConfigurationManager.AppSettings["TargetAddress"], "PUT", message);
            }
        }
    }
}
