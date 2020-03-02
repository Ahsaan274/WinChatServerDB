using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace WinChatServer
{
    class Query
    {
        internal static void Execute(string sql)
        {
            OracleCommand com = new OracleCommand(sql, clsConnection.con);
            com.ExecuteNonQuery(); 
        }
        internal static DataTable getData(string sql)
        {
            DataTable dt = new DataTable();           
            OracleDataAdapter adap = new OracleDataAdapter(sql, clsConnection.con);
            adap.Fill(dt);
            adap.Dispose();
            return dt;
        }

        internal static string  getData1(string sql)
        {
            DataTable dt = new DataTable();
            OracleDataAdapter adap = new OracleDataAdapter(sql, clsConnection.con);
            adap.Fill(dt);
            adap.Dispose();
            string JSONresult;
            JSONresult = JsonConvert.SerializeObject(dt) + "&";
           // DataTable  dt1 = (DataTable )JsonConvert.DeserializeObject(JSONresult, typeof(DataTable));  
            return JSONresult;
        }
        internal static DataTable getData(OracleCommand com)
        {
            DataTable dt = new DataTable();
            com.Connection = clsConnection.con;
            OracleDataAdapter adap = new OracleDataAdapter(com);
            adap.Fill(dt);
            adap.Dispose();
            return dt;
        }
        internal static DataSet  getDataSet(string sql)
        {
            DataSet ds = new DataSet();
            // SqlConnection con = conOpen();            
            OracleDataAdapter adap = new OracleDataAdapter(sql, clsConnection.con);
            OracleCommandBuilder comBuild = new OracleCommandBuilder();
            
            adap.Fill(ds);
            adap.Dispose();
            return ds;
        }
       
       

    }
}
