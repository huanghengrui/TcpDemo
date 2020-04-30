using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DynamicFaceDemo
{
    public partial class frmDeviceAdd : frmBaseForm
    {
        private string oleid = "";

        public frmDeviceAdd()
        {   
            InitializeComponent();
            SetTextboxNumber(txtConnectPwd);
            SetTextboxNumber(txtPort);
        }

        public frmDeviceAdd(string id)
        {
            oleid = id;
            InitializeComponent();
        }

        private void frmDeviceAdd_Load(object sender, EventArgs e)
        {
            if (oleid != "")
            {
                DataTableReader dr = null;
                string QuerySQL = "select * from devicelist where id=" + oleid + " order by id";
                try
                {
                    dr = Ole.GetDataReader(QuerySQL);
                    if (dr.Read())
                    {
                        txtDeviceId.Text = dr["device_id"].ToString();
                        txtDeviceName.Text = dr["device_name"].ToString();
                        txtIPAddress.Text = dr["device_ip"].ToString();
                        txtPort.Text = dr["device_port"].ToString();
                        txtConnectPwd.Text = dr["device_pwd"].ToString();
                    }
                }
                catch (Exception E)
                {
                    MessageBox.Show(E.Message + "\r\n" + QuerySQL);
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string deviceId = txtDeviceId.Text.Trim();
            string deviceName = txtDeviceName.Text.Trim();
            string deviceIp = txtIPAddress.Text.Trim();
            string devicePort = txtPort.Text.Trim();
            string devicePwd = txtConnectPwd.Text.Trim();
            string oldDeviceId = "0";
            DataTableReader dr = null;
            string sql = "";

            if (deviceId == "")
            {
                MessageBox.Show("Please Enter DeviceId!");
                txtDeviceId.Focus();
                return;
            }

            if (deviceIp == "")
            {
                MessageBox.Show("Please Enter IPAddress!");
                txtIPAddress.Focus();
                return;
            }
            if (devicePort == "")
            {
                MessageBox.Show("Please Enter Port!");
                txtPort.Focus();
                return;
            }
            if (devicePwd == "")
            {
                MessageBox.Show("Please Enter ConnectPwd!");
                txtConnectPwd.Focus();
                return; 
            }
            try
            {
              
                if (oleid != "")
                {
                    sql = "select [device_id] from devicelist where [id]=" + oleid + "";
                    dr = Ole.GetDataReader(sql);
                    if (dr.Read())
                    {
                        oldDeviceId = dr[0].ToString();
                    }
                    dr.Close();

                    sql = string.Format("update devicelist set [device_id]='{0}',[device_name]='{1}',[device_ip]='{2}',[device_port]='{3}',[device_pwd]='{4}' where [id]={5}",
                        deviceId,
                        deviceName,
                        deviceIp,
                        devicePort,
                        devicePwd,
                        oleid);
                    Ole.ExecSQL(sql);
                    if (Ole.ExecSQL(sql) > 0)
                    {
                        sql = string.Format("update parameterlist set [device_id]='{0}',[device_name]='{1}' where [device_id]='{2}'",
                        deviceId,
                        deviceName,
                        oldDeviceId);
                        Ole.ExecSQL(sql);
                    }
                }
                else
                {
                    sql = "select [device_id] from devicelist where [device_id]='"+deviceId+"'";
                    dr = Ole.GetDataReader(sql);
                    if (dr.Read()) 
                    {
                        MessageBox.Show("The database already exists with the same ID!");
                        txtDeviceId.Focus();
                        return;
                    }

                    sql = string.Format("insert into devicelist([device_id],[device_name],[device_ip],[device_port],[device_pwd]) values('{0}','{1}','{2}','{3}','{4}')",
                        deviceId,
                        deviceName,
                        deviceIp,
                        devicePort,
                        devicePwd);
                    
                    if (Ole.ExecSQL(sql) > 0)
                    {
                        Ole.SaveParameter(deviceId, deviceName, "34", GetLocalIP(), "7005", GetLocalIP(), "7005", "1", "CHS", "5", "yes", "7", "yes", "7", "1", "0", "0","no", "Face or Finger or Card or Password");
                    }
                }

              
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message + "\r\n" + sql);
            }
            finally
            {
                if (dr != null)
                    dr.Close();
                dr = null;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnGetDevId_Click(object sender, EventArgs e)
        {
            try
            {
                frmMain frm = new frmMain();
                frm.socKetIp = txtIPAddress.Text;
                frm.socKetPort = Convert.ToInt32(txtPort.Text);
                frm.socKetPwd = Convert.ToInt32(txtConnectPwd.Text);
                frm.btnGetDeviceInfo_Click(null, null);
                txtDeviceId.Text = frm.deviceId;
                txtDeviceName.Text = frm.deviceName;
            }
            catch(Exception E)
            {
                MessageBox.Show(E.Message);
            }
           
        }
    }
}
