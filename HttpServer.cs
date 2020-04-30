using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using System.Web;
using System.Collections;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Data;

namespace DynamicFaceDemo
{
    public class HttpServer
    {
        HttpListener httpListener = new HttpListener();
        Label label;
        DataGridView recordDataGrid;
        DataGridView personDataGrid;
        bool stop = false;
        public void Setup(int port = 7005 , Label label = null, DataGridView recordDataGrid = null, DataGridView personDataGrid = null)
        {
            if (port == 0)
            {
                stop = true;
                httpListener.Close();
                return;
            }
            stop = false;
            httpListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            httpListener.Prefixes.Add(string.Format("http://*:{0}/" , port));
            httpListener.Start();
            Receive();
            this.label = label;
            this.recordDataGrid = recordDataGrid;
            this.personDataGrid = personDataGrid;
        }

        private void Receive()
        {
            if(!stop)
                httpListener.BeginGetContext(new AsyncCallback(EndReciver), null);
        }

        void EndReciver(IAsyncResult ar)
        {
            if(!stop)
            {
                var context = httpListener.EndGetContext(ar);
                Dispather(context);
                Receive();
            } 
        }

        RequestHelper RequestHelper;
        ResponseHelper ResponseHelper;
        void Dispather(HttpListenerContext context)
        {
            HttpListenerRequest request= context.Request;
            HttpListenerResponse response = context.Response;
            RequestHelper = new RequestHelper(request, label, recordDataGrid, personDataGrid);
            ResponseHelper = new ResponseHelper(response);
            RequestHelper.DispatchResources(state => { ResponseHelper.WriteToClient(state); });
           
        }

    }

    public class RequestHelper
    {  
        private HttpListenerRequest request;
        private Label label;
        private DataGridView recordDataGrid;
        DataGridView personDataGrid;
        AccessOle Ole = new AccessOle();
        public RequestHelper(HttpListenerRequest request, Label label, DataGridView recordDataGrid, DataGridView personDataGrid)
        {
            this.request = request;
            this.label = label;
            this.recordDataGrid = recordDataGrid;
            this.personDataGrid = personDataGrid;
        }
        
         public delegate void ExecutingDispatch(int state);
         public void DispatchResources(ExecutingDispatch action)
         {
             var rawUrl = request.RawUrl;
             try
             {
                 string deviceId = request.Headers.Get("dev_id");
                 StreamReader sr = new StreamReader(request.InputStream);
                 string requestBody = sr.ReadToEnd();
                 string sql = "";
                int faceCount = 0;
                int fpsCount = 0;
                int palmCount = 0;
                bool isUpdate = false;
                if (true)
                {
                    if(request.Headers.Get("request_code").Contains("realtime_glog"))
                    {
                        MonitoringLogs logs = JsonConvert.DeserializeObject<MonitoringLogs>(requestBody);    
                        string dataTime = Ole.stringToTimeStr(logs.time);
                        Ole.SaveLogsData(deviceId, logs.userId, dataTime, logs.verifyMode, logs.ioMode.ToString(), logs.inOut);
                        if (recordDataGrid.InvokeRequired)
                        {
                            recordDataGrid.Invoke(new Action<MonitoringLogs>(s =>
                            {
                                DataTable dt = (DataTable)recordDataGrid.DataSource;
                                dt.Rows.Add(new object[] { 0, deviceId, s.userId, dataTime, s.verifyMode, s.ioMode.ToString(), s.inOut});
                                recordDataGrid.DataSource = dt;
                                recordDataGrid.Rows[recordDataGrid.Rows.Count - 1].Selected = true;
                                recordDataGrid.CurrentCell = recordDataGrid.Rows[recordDataGrid.Rows.Count - 1].Cells[0];
                                Application.DoEvents();
                            }), logs);
                        }
                        else
                        {
                            DataTable dt = (DataTable)recordDataGrid.DataSource;
                            dt.Rows.Add(new object[] { 0, deviceId, logs.userId, dataTime, logs.verifyMode, logs.ioMode.ToString(), logs.inOut });
                            recordDataGrid.DataSource = dt;
                            recordDataGrid.Rows[recordDataGrid.Rows.Count - 1].Selected = true;
                            recordDataGrid.CurrentCell = recordDataGrid.Rows[recordDataGrid.Rows.Count - 1].Cells[0];
                            Application.DoEvents();
                        }
                    }
                    else if(request.Headers.Get("request_code").Contains("realtime_enroll_data"))
                    {
                        GetUsers UserInfo = JsonConvert.DeserializeObject<GetUsers>(requestBody);
                        if (Ole.SaveUser(UserInfo.userId, UserInfo.name,
                            UserInfo.privilege.ToString(), UserInfo.card, UserInfo.pwd,
                            UserInfo.vaildStart, UserInfo.vaildEnd,ref isUpdate) > 0)
                        {
                            if (UserInfo.photo != null)
                            {
                                sql = "update personlist set [photo]=@photo where [user_id]='" + UserInfo.userId + "'";
                                string photoStr = UserInfo.photo;
                               
                                Ole.UpdateByteData(sql, "photo", Convert.FromBase64String(photoStr));
                            }
                            if (UserInfo.face != null)
                            {
                                faceCount++;
                                sql = "update personlist set [face00]=@face00 where [user_id]='" + UserInfo.userId + "'";
                                string faceStr = UserInfo.face;
                                Ole.UpdateByteData(sql, "face00", Convert.FromBase64String(faceStr));
                            }
                            if (UserInfo.palm != null)
                            {
                                palmCount++;
                                sql = "update personlist set [palm00]=@palm00 where [user_id]='" + UserInfo.userId + "'";
                                string palmStr = UserInfo.palm;
                                Ole.UpdateByteData(sql, "palm00", Convert.FromBase64String(palmStr));
                            }
                            if (UserInfo.fps != null)
                            {
                                for (int j = 0; j < UserInfo.fps.Count; j++)
                                {
                                    if (UserInfo.fps[j] != null)
                                    {
                                        fpsCount++;
                                        sql = "update personlist set [fps0" + j + "]=@fps0" + j + " where [user_id]='" + UserInfo.userId + "'";
                                        string fpsStr = UserInfo.fps[j].Replace(" ", "+");
                                        Ole.UpdateByteData(sql, "fps0" + j, Convert.FromBase64String(fpsStr));
                                    }
                                }
                            }
                        }
                        if(!isUpdate)
                        {
                            if (personDataGrid.InvokeRequired)
                            {
                                personDataGrid.Invoke(new Action<GetUsers>(s =>
                                {
                                    DataTable dt = (DataTable)personDataGrid.DataSource;
                                    dt.Rows.Add(new object[] {0,s.userId, s.name,
                                s.privilege.ToString(), s.card, s.pwd,fpsCount, faceCount,palmCount,
                                s.vaildStart, s.vaildEnd});
                                    personDataGrid.DataSource = dt;
                                    personDataGrid.Rows[personDataGrid.Rows.Count - 1].Selected = true;
                                    personDataGrid.CurrentCell = personDataGrid.Rows[personDataGrid.Rows.Count - 1].Cells[0];
                                    Application.DoEvents();
                                }), UserInfo);
                            }
                            else
                            {
                                DataTable dt = (DataTable)personDataGrid.DataSource;
                                dt.Rows.Add(new object[] {0,UserInfo.userId, UserInfo.name,
                            UserInfo.privilege.ToString(), UserInfo.card, UserInfo.pwd,fpsCount, faceCount,palmCount,
                            UserInfo.vaildStart, UserInfo.vaildEnd});
                                personDataGrid.DataSource = dt;
                                personDataGrid.Rows[personDataGrid.Rows.Count - 1].Selected = true;
                                personDataGrid.CurrentCell = personDataGrid.Rows[personDataGrid.Rows.Count - 1].Cells[0];
                                Application.DoEvents();
                            }
                        }
                    }
                    else
                    {
                        if(request.Headers.Get("request_code").Contains("receive_cmd"))
                        {
                            httpTime time = JsonConvert.DeserializeObject<httpTime>(requestBody);
                            string dataTime = Ole.stringToTimeStr(time.time);
                            if (label.InvokeRequired)
                            {
                                label.Invoke(new Action<String>(s =>
                                {
                                    label.Text = s;
                                }), dataTime);
                            }
                            else
                            {
                                label.Text = dataTime;
                            }
                        } 
                    }
                }
             }
              catch
             {
                action.Invoke(405);
                return;
             }

            action.Invoke(404);
        }
    }

    /// <summary>
    /// 返回确认信息
    /// </summary>
    public class ResponseHelper
    {
        private HttpListenerResponse response;
        private Stream OutputStream;
        public Stream _OutputStream
        {
            get { return OutputStream; }
            set { OutputStream = value; }
        }

        public ResponseHelper(HttpListenerResponse response)
        {
            this.response = response;
            OutputStream = response.OutputStream;
        }
      
        public void WriteToClient(int state)
        {
            response.StatusCode = 200;

            try
            {
                using (StreamWriter writer = new StreamWriter(OutputStream))
                {
                    response.Headers["response_code"] = "OK";
                    response.Headers["trans_id"] = "100";
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
         
        }
    }
}
