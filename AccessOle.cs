using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DynamicFaceDemo
{
    public class AccessOle
    {
        string connString = "";
        OleDbConnection conn = null;
        const int CommandTimeout = 36000;
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private void SetOleStr()
        {
            string path = System.AppDomain.CurrentDomain.BaseDirectory;
            connString = path + "Database.mdb";
            conn = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + connString);
        }
        /// <summary>
        /// 关闭数据库
        /// </summary>
        private void ColseOle()
        {
            if (conn != null)
                conn.Close();
            conn = null;
        }
        /// <summary>
        /// 打开数据库
        /// </summary>
        private void OpenOle()
        {
            ColseOle();
            SetOleStr();
            conn.Open();
        }
        /// <summary>
        /// 获取DataTable类型的数据
        /// </summary>
        /// <param name="SQLQuery"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string SQLQuery)
        {
            DataTable dt = new DataTable();
            if (SQLQuery == "")
            {
                dt = null;
            }
            else
            {
                try
                {
                    OpenOle();
                    OleDbDataAdapter oleDA = new OleDbDataAdapter(SQLQuery, conn);
                    if (oleDA.SelectCommand != null) oleDA.SelectCommand.CommandTimeout = CommandTimeout;
                    if (oleDA.DeleteCommand != null) oleDA.DeleteCommand.CommandTimeout = CommandTimeout;
                    if (oleDA.UpdateCommand != null) oleDA.UpdateCommand.CommandTimeout = CommandTimeout;
                    oleDA.Fill(dt);
                    oleDA.Dispose();
                }
                catch (Exception E)
                {
                    MessageBox.Show(E.Message + "\r\n" + SQLQuery);
                }
                finally
                {
                    ColseOle();
                }

            }
            return dt;
        }
        /// <summary>
        /// 获取DataTableReader类型的数据
        /// </summary>
        /// <param name="SQLQuery"></param>
        /// <returns></returns>
        public DataTableReader GetDataReader(string SQLQuery)
        {
            DataSet ds = GetDataSet(SQLQuery);
            return ds.CreateDataReader();
        }
        /// <summary>
        /// 存储仓库
        /// </summary>
        /// <param name="SQLQuery"></param>
        /// <returns></returns>
        public DataSet GetDataSet(string SQLQuery)
        {
            DataSet ds = new DataSet();
            if (SQLQuery == "")
            {
                ds = null;
            }
            else
            {
                try
                {
                    OpenOle();
                    OleDbDataAdapter oleDA = new OleDbDataAdapter(SQLQuery, conn);
                    if (oleDA.SelectCommand != null) oleDA.SelectCommand.CommandTimeout = CommandTimeout;
                    if (oleDA.DeleteCommand != null) oleDA.DeleteCommand.CommandTimeout = CommandTimeout;
                    if (oleDA.UpdateCommand != null) oleDA.UpdateCommand.CommandTimeout = CommandTimeout;
                    oleDA.Fill(ds, "DataSource");
                    oleDA.Dispose();
                    oleDA = null;
                }
                catch (Exception E)
                {
                    MessageBox.Show(E.Message + "\r\n" + SQLQuery);
                }
                finally
                {
                    ColseOle();
                }
            }
            return ds;
        }
        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="SQLQuery"></param>
        /// <returns></returns>
        public int ExecSQL(string SQLQuery)
        {
            int ret = 0;
            if (SQLQuery == "") return ret;
            OpenOle();
            try
            {
                OleDbCommand oleCmd = new OleDbCommand(SQLQuery, conn);
                oleCmd.CommandTimeout = CommandTimeout;
                ret = oleCmd.ExecuteNonQuery();
                oleCmd.Dispose();
                oleCmd = null;
                ret = 1;
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message + "\r\n" + SQLQuery);
            }
            finally
            {
                ColseOle();
            }
            return ret;
        }

        /// <summary>
        /// 执行List<>Sql语句
        /// </summary>
        /// <param name="SQLQuery"></param>
        /// <returns></returns>
        public int ExecSQL(List<string> SQLQuery)
        {
            int ret = 0;
            string sql = "";
            if (SQLQuery.Count == 0) return ret;
            OpenOle();
            OleDbCommand oleCmd;
            OleDbTransaction oleTran = conn.BeginTransaction();
            try
            {
                for (int i = 0; i < SQLQuery.Count; i++)
                {
                    sql = SQLQuery[i].Trim();
                    if (sql == "") continue;
                    oleCmd = new OleDbCommand(sql, conn);
                    oleCmd.CommandTimeout = CommandTimeout;
                    oleCmd.Transaction = oleTran;
                    oleCmd.ExecuteNonQuery();
                    ret = 1;
                }
                oleTran.Commit();
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message + "\r\n" + SQLQuery);
            }
            finally
            {
                ColseOle();
            }

            return ret;
        }
        /// <summary>
        /// 更新byte[]数据
        /// </summary>
        /// <param name="SQLQuery"></param>
        /// <param name="picField"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public bool UpdateByteData(string SQLQuery, string picField, byte[] Data)
        {
            bool ret = false;
            if (SQLQuery == "") return ret;
            OpenOle();
            try
            {
                OleDbCommand oleCmd = new OleDbCommand(SQLQuery, conn);
                oleCmd.CommandTimeout = CommandTimeout;
                OleDbParameter oleParam = new OleDbParameter("@" + picField, OleDbType.Binary);
                oleParam.Value = Data;
                oleCmd.Parameters.Add(oleParam);
                ret = oleCmd.ExecuteNonQuery() > 0;
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message + "\r\n" + SQLQuery);
            }
            finally
            {
                ColseOle();
            }
            return ret;
        }
        /// <summary>
        /// 批量更新数据到数据库
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="DestinationTableName"></param>
        /// <returns></returns>
        public bool batchUpdateData(DataTable dt, string DestinationTableName)
        {
            string SQLQuery = "";
          
            bool ret = false;
            OleDbCommand oleCmd = null;
            OleDbParameter oleParam = null;
            OpenOle();
            try
            {
                SQLQuery = "UPDATE ["+ DestinationTableName + "] SET ";
                foreach (DataColumn one in dt.Columns)
                {
                    if (one.ColumnName == "id")
                    {
                        continue;
                    }
                       
                    SQLQuery +="["+one.ColumnName+"]=@"+ one.ColumnName+",";
                }
                SQLQuery = SQLQuery.Substring(0,SQLQuery.Length-1);

                for(int i=0;i<dt.Rows.Count;i++)
                {
                    foreach (DataColumn one in dt.Columns)
                    {
                        if (one.ColumnName == "id")
                        {
                            oleCmd = new OleDbCommand(SQLQuery+" where [id]="+ dt.Rows[i][one.ColumnName] + "", conn);
                            oleCmd.CommandTimeout = CommandTimeout;
                            continue;
                        }
                        oleParam = new OleDbParameter("@" + one.ColumnName, one.DataType);
                        oleParam.Value = dt.Rows[i][one.ColumnName];
                        oleCmd.Parameters.Add(oleParam);
                    }
                    ret = oleCmd.ExecuteNonQuery() > 0;
                }
              
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message + "\r\n" + SQLQuery);
            }
            finally
            {
                ColseOle();
            }
            return ret;
        }

        /// <summary>
        /// 批量插入数据到数据库
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="DestinationTableName"></param>
        /// <returns></returns>
        public bool batchSeveData(DataTable dt, string DestinationTableName)
        {
            try
            {
                if (dt == null) return false;
                OpenOle();
                List<string> columnList = new List<string>();
                foreach (DataColumn one in dt.Columns)
                {
                    columnList.Add(one.ColumnName);
                }
                OleDbDataAdapter adapter = new OleDbDataAdapter();
                adapter.SelectCommand = new OleDbCommand("SELECT * FROM " + DestinationTableName, conn);
                using (OleDbCommandBuilder builder = new OleDbCommandBuilder(adapter))
                {
                    builder.QuotePrefix = "[";
                    builder.QuoteSuffix = "]";
                    adapter.InsertCommand = builder.GetInsertCommand();
                    foreach (string one in columnList)
                    {
                        adapter.InsertCommand.Parameters.Add(new OleDbParameter(one, dt.Columns[one].DataType));
                    }
                    adapter.Update(dt);
                    adapter.Dispose();
                    adapter = null;
                }
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
            }
            finally
            {
                ColseOle();
            }
            return true;
        }

        /// <summary>
        /// 保存参数到数据库
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="deviceName"></param>
        /// <param name="wiegandType"></param>
        /// <param name="serverHost"></param>
        /// <param name="serverPort"></param>
        /// <param name="interval"></param>
        /// <param name="language"></param>
        /// <param name="volume"></param>
        /// <param name="antiPass"></param>
        /// <param name="openDoorDelay"></param>
        /// <param name="tamperAlarm"></param>
        /// <param name="alarmDelay"></param>
        /// <param name="reverifyTime"></param>
        /// <param name="screensaversTime"></param>
        /// <param name="sleepTime"></param>
        public int SaveParameter(string deviceId, string deviceName, string wiegandType, string serverHost, string serverPort, string pushServerHost, string pushServerPort, string interval, string language, string volume, string antiPass,
            string openDoorDelay, string tamperAlarm, string alarmDelay, string reverifyTime, string screensaversTime, string sleepTime,string pushEnable, string verifyMode)
        {
            DataTableReader dr = null;
            int ret = 0;
            string sql = "select * from parameterlist where [device_id]='" + deviceId + "'";

            try
            {
                dr = GetDataReader(sql);
                if (dr.Read())
                {
                    sql = string.Format("update parameterlist set [device_name]='{0}',[wiegand_type]='{1}',[server_host]='{2}'," +
                        "[server_port]='{3}',[push_server_host]='{4}',[push_server_port]='{5}',[interval]='{6}',[language]='{7}',[volume]='{8}',[anti_pass]='{9}'," +
                        "[open_door_delay]='{10}',[tamper_alarm]='{11}',[alarm_delay]='{12}',[reverify_time]='{13}',[screensavers_time]='{14}',[sleep_time]='{15}',[push_enable]='{16}',[verifyMode]='{17}' where [device_id]='{18}'",
                        deviceName,
                        wiegandType,
                        serverHost,
                        serverPort,
                        pushServerHost,
                        pushServerPort,
                        interval,
                        language,
                        volume,
                        antiPass,
                        openDoorDelay,
                        tamperAlarm,
                        alarmDelay,
                        reverifyTime,
                        screensaversTime,
                        sleepTime,
                        pushEnable,
                        verifyMode,
                        deviceId);
                }
                else
                {
                    sql = string.Format("insert into parameterlist([device_id],[device_name],[wiegand_type],[server_host],[server_port],[push_server_host],[push_server_port],[interval],[language],[volume],[anti_pass]," +
                        "[open_door_delay],[tamper_alarm],[alarm_delay],[reverify_time],[screensavers_time],[sleep_time],[push_enable],[verifyMode]) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}'," +
                        "'{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}')",
                        deviceId,
                        deviceName,
                        wiegandType,
                        serverHost,
                        serverPort,
                        pushServerHost,
                        pushServerPort,
                        interval,
                        language,
                        volume,
                        antiPass,
                        openDoorDelay,
                        tamperAlarm,
                        alarmDelay,
                        reverifyTime,
                        screensaversTime,
                        sleepTime,
                        pushEnable,
                        verifyMode);
                }
                ret = ExecSQL(sql);
            }
            catch (Exception E)
            {
                MessageBox.Show(E + "\r\n" + sql);
            }
            finally
            {
                if (dr != null)
                    dr.Close();
                dr = null;
            }
            return ret;
        }

        /// <summary>
        /// 保存用户信息到数据库
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="username"></param>
        /// <param name="privilege"></param>
        /// <param name="card"></param>
        /// <param name="pwd"></param>
        /// <param name="vaildstart"></param>
        /// <param name="vaildend"></param>
        /// <returns></returns>
        public int SaveUser(string userid, string username, string privilege, string card, string pwd, string vaildstart, string vaildend,ref bool IsUpdate)
        {
            int ret = 0;
            string sql = "";
            DataTableReader dr = null;
            sql = "select [user_id] from personlist where [user_id]='" + userid + "'";
            dr = GetDataReader(sql);
            if (dr.Read())
            {
                sql = string.Format("update personlist set [user_id]='{0}',[user_name]='{1}',[privilege]='{2}',[card]='{3}',[pwd]='{4}',[vaild_start]='{5}',[vaild_end]='{6}' where [user_id]='{7}'",
                   userid,
                   username,
                   privilege,
                   card,
                   pwd,
                   vaildstart,
                   vaildend,
                   userid);
                IsUpdate = true;
            }
            else
            {
                sql = string.Format("insert into personlist([user_id],[user_name],[privilege],[card],[pwd],[vaild_start],[vaild_end]) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}')",
                   userid,
                   username,
                   privilege,
                   card,
                   pwd,
                   vaildstart,
                   vaildend);
                IsUpdate = false;
            }

            ret = ExecSQL(sql);
            return ret;
        }
        /// <summary>
        /// 保存设备记录到数据库
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="userId"></param>
        /// <param name="dateTime"></param>
        /// <param name="verifyMode"></param>
        /// <param name="ioMode"></param>
        /// <param name="inOut"></param>
        /// <returns></returns>
        public int SaveLogsData(string deviceId, string userId, string dateTime, string verifyMode, string ioMode, string inOut)
        {
            int ret = 0;
            string sql = string.Format("insert into recordlist([device_id],[user_id],[time],[verify_mode],[io_mode],[in_out]) values('{0}','{1}','{2}','{3}','{4}','{5}')",
                  deviceId,
                  userId,
                  dateTime,
                  verifyMode,
                  ioMode,
                  inOut);
            ret = ExecSQL(sql);
            return ret;
        }
        /// <summary>
        /// 字符串转数据格式
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public string stringToTimeStr(string time)
        {
            if (time.Length == 14)
            {
                time = time.Insert(4, "-");
                time = time.Insert(7, "-");
                time = time.Insert(10, " ");
                time = time.Insert(13, ":");
                time = time.Insert(16, ":");
            }
            return time;
        }
    }
}
