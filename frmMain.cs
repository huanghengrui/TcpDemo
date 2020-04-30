using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DynamicFaceDemo
{
    public partial class frmMain : frmBaseForm
    {
        public string deviceId = "";
        public string deviceName = "";
        public string cmd = "";
        public string jsonStr = "";
        public string socKetIp = "";
        public int socKetPort = 0;
        public SocKetClient socKetClient = new SocKetClient();
        public HttpServer httpServer;
        public int socKetPwd = 0;
        public const int PACKAGE_ID = 0;
        public int maxBufferLen = 0;
        public DataTable dtRecordData = new DataTable();  //从数据库获取的记录
        public DataTable dtInsertRecordData = new DataTable();  //保存记录
        public DataTable dtInsertPersonData = new DataTable();  //保存要插入数据库的人员 
        public DataTable dtUpdatePersonData = new DataTable();  //保存要更新到数据库的人员
        public DataTable dtOlePersonData = new DataTable();     //从数据库获取的人员

        public List<string> usersIDList = new List<string>();   //人员列表

        public List<_ResultInfo<PersonInfo<GetUsers>>> PersonInfoList = new List<_ResultInfo<PersonInfo<GetUsers>>>();//获取人员信息
        public List<_ResultInfo<RecordInfo<Logs>>> RecordInfoList = new List<_ResultInfo<RecordInfo<Logs>>>();//获取记录
        public _ResultInfo<ParameterInfo> parameterInfo = new _ResultInfo<ParameterInfo>(); //获取参数

        public frmMain()
        {
            InitializeComponent();
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
            Version ApplicationVersion = new Version(Application.ProductVersion);
            string ss = ApplicationVersion.ToString();//获取主版本号  
            lbTitle.Text = lbTitle.Text + " (Version: " + ss + ")";
            Application.DoEvents();
            InitForm();
        }
        public void InitForm()
        {
            try
            {
                InitParameterTabPage();
                InitDeviceListTabPage();
                InitPersonnelTabPage();
                InitRecordDataGridTabPage();
            }
            catch (Exception E)
            {
                showMsg(E.Message);
            }

        }

        /// <summary>
        /// 设置按钮是否可用
        /// </summary>
        /// <param name="state"></param>
        private void RefreshButton(bool state)
        {
            btnAdd.Enabled = state;
            btnAddUser.Enabled = state;
            btnClearLogData.Enabled = state;
            btnClearPhoto.Enabled = state;
            btnCleraManager.Enabled = state;
            btnDelete.Enabled = state;
            btnDeleteOleUser.Enabled = state;
            btnDeleteUser.Enabled = state;
            btnEdit.Enabled = state;
            btnEditUser.Enabled = state;
            btnEnterEnroll.Enabled = state;
            btnGetDeviceInfo.Enabled = state;
            btnGetDeviceSetting.Enabled = state;
            btnGetLogData.Enabled = state;
            btnGetUserInfo.Enabled = state;
            btnGetUserList.Enabled = state;
            btnRestartDev.Enabled = state;
            btnSelectPhoto.Enabled = state;
            btnSetDeviceSetting.Enabled = state;
            btnSetTime.Enabled = state;
            btnSetUserInfo.Enabled = state;
            groupBoxMonitoring.Enabled = state;
        }

        /// <summary>
        /// 初始化设备列表
        /// </summary>
        public void InitDeviceListTabPage()
        {
            deviceDataGrid.Columns.Clear();
            //添加列
            AddColumn(deviceDataGrid, 0, "id", "Id", true, false, 0, 80);
            AddColumn(deviceDataGrid, 0, "device_id", "Device_Id", false, false, 0, 110);
            AddColumn(deviceDataGrid, 0, "device_name", "Device_Name", false, false, 0, 80);
            AddColumn(deviceDataGrid, 1, "online", "Online", true, false, 0, 80);
            AddColumn(deviceDataGrid, 0, "device_ip", "Device_Ip", false, false, 0, 100);
            AddColumn(deviceDataGrid, 0, "device_port", "Device_Port", false, false, 0, 80);
            AddColumn(deviceDataGrid, 0, "device_pwd", "Device_Pwd", false, false, 0, 100);

            RefreshDeviceListData();
        }
        /// <summary>
        /// 从数据库获取设备列表显示到界面
        /// </summary>
        public void RefreshDeviceListData()
        {
            string QuerySQL = "select * from devicelist order by device_id";
            bindingSourceDevice.DataSource = null;
            bindingSourceDevice.DataSource = Ole.GetDataTable(QuerySQL);
            if (bindingSourceDevice.Count > 0)
            {
                deviceDataGrid_CellClick(null, null);
            }

            Application.DoEvents();
        }
        /// <summary>
        /// 初始化人员模块的界面
        /// </summary>
        public void InitPersonnelTabPage()
        {
            SetTextboxNumber(txtOleUserId);
            personDataGrid.Columns.Clear();
            AddColumn(personDataGrid, 3, "select", "Select", false, true, 1, 70);
            AddColumn(personDataGrid, 0, "id", "ID", true, false, 0, 60);
            AddColumn(personDataGrid, 0, "user_id", "User_Id", false, false, 0, 100);
            AddColumn(personDataGrid, 0, "user_name", "User_Name", false, false, 0, 100);
            AddColumn(personDataGrid, 0, "privilege", "Privilege", false, false, 0, 100);
            AddColumn(personDataGrid, 0, "card", "Card", false, false, 0, 100);
            AddColumn(personDataGrid, 0, "pwd", "Pwd", false, false, 0, 100);
            AddColumn(personDataGrid, 0, "fps", "Fps", false, false, 0, 60);
            AddColumn(personDataGrid, 0, "face", "Face", false, false, 0, 60);
            AddColumn(personDataGrid, 0, "palm", "Palm", false, false, 0, 60);
            AddColumn(personDataGrid, 0, "vaild_start", "Vaild_Start", false, false, 0, 120);
            AddColumn(personDataGrid, 0, "vaild_end", "Vaild_End", false, false, 0, 120);

            cbbDataInfo.Items.AddRange(new object[]{
                                       "",
                                       DataInfo.Fps00,
                                       DataInfo.Fps01,
                                       DataInfo.Fps02,
                                       DataInfo.Fps03,
                                       DataInfo.Fps04,
                                       DataInfo.Fps05,
                                       DataInfo.Fps06,
                                       DataInfo.Fps07,
                                       DataInfo.Fps08,
                                       DataInfo.Fps09,
                                       DataInfo.Face00,
                                       DataInfo.Palm00
                                       });
            cbbOlePrivilege.Items.AddRange(new object[] {
                                           GeneralUser.name,
                                           Administrator.name
                                           });
            cbbOlePrivilege.SelectedIndex = 0;
            cbbEnterEnrollFeature.SelectedIndex = 0;

            RefreshPersonnelData();
        }

        /// <summary>
        /// 从数据库获取人员显示到界面
        /// </summary>
        public void RefreshPersonnelData()
        {
            UpdateDataCount();
            string QuerySQL = "select * from personlist where 1=2";
            dtInsertPersonData = Ole.GetDataTable(QuerySQL);
            dtUpdatePersonData = Ole.GetDataTable(QuerySQL);
            QuerySQL = "select [id],[user_id],[user_name],[privilege],[card],[pwd],[fps],[face],[palm],[vaild_start],[vaild_end] from personlist order by user_id+0";
            dtOlePersonData = Ole.GetDataTable(QuerySQL);
            personDataGrid.DataSource = dtOlePersonData;
            if (dtOlePersonData.Rows.Count > 0)
            {
                btnEditUser.Enabled = true;
                btnDeleteOleUser.Enabled = true;
                personDataGrid.Rows[personDataGrid.Rows.Count - 1].Selected = true;
                personDataGrid.CurrentCell = personDataGrid.Rows[personDataGrid.Rows.Count - 1].Cells[0];
                personDataGrid_CellClick(null, null);
            }
            else
            {
                btnEditUser.Enabled = false;
                btnDeleteOleUser.Enabled = false;
                lbTotalPerson.Text = "0/0";
            }

            Application.DoEvents();
        }

        private void btnRefreshPerson_Click(object sender, EventArgs e)
        {
            RefreshPersonnelData();
        }

        /// <summary>
        /// 表格标题的多选框点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void ch_OnCheckBoxClicked(object sender, datagridviewCheckboxHeaderEventArgs e)
        {
            SelectData(personDataGrid, e.CheckedState);
        }
        /// <summary>
        /// 初始化记录模块
        /// </summary>
        public void InitRecordDataGridTabPage()
        {
            SetTextboxNumber(txtMonitoring);
            btnCloseMonitor.Enabled = false;
            recordDataGrid.Columns.Clear();
            AddColumn(recordDataGrid, 0, "no", "No", false, false, 0, 60);
            AddColumn(recordDataGrid, 0, "id", "ID", true, false, 0, 60);
            AddColumn(recordDataGrid, 0, "device_id", "Device_Id", false, false, 0, 120);
            AddColumn(recordDataGrid, 0, "user_id", "User_Id", false, false, 0, 100);
            AddColumn(recordDataGrid, 0, "time", "Time", false, false, 0, 120);
            AddColumn(recordDataGrid, 0, "verify_mode", "Verify_Mode", false, false, 0, 100);
            AddColumn(recordDataGrid, 0, "io_mode", "IO_Mode", false, false, 0, 100);
            AddColumn(recordDataGrid, 0, "in_out", "In_Out", false, false, 0, 100);
            RefreshRecordData();

        }
        /// <summary>
        /// 从数据库获取记录
        /// </summary>
        public void RefreshRecordData()
        {
            string sql = "select * from recordlist where 1=2";
            try
            {
                dtInsertRecordData = Ole.GetDataTable(sql);
                sql = "select [id],[device_id],[user_id],[time],[verify_mode],[io_mode],[in_out] from recordlist order by time";
                dtRecordData = Ole.GetDataTable(sql);
                recordDataGrid.DataSource = dtRecordData;
                if (dtRecordData.Rows.Count > 0)
                {
                    lbTotalRecord.Text = recordDataGrid.Rows.Count + "/" + 1;
                    recordDataGrid.Rows[recordDataGrid.Rows.Count - 1].Selected = true;
                    recordDataGrid.CurrentCell = recordDataGrid.Rows[recordDataGrid.Rows.Count - 1].Cells[0];
                }
                else
                {
                    lbTotalRecord.Text = "0/0";
                }
                Application.DoEvents();
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message + "\r\n" + sql);
            }

        }
        /// <summary>
        /// 初始化参数模块
        /// </summary>
        public void InitParameterTabPage()
        {
            SetTextboxNumber(txtAlarmDelay);
            SetTextboxNumber(txtInterval);
            SetTextboxNumber(txtOpenDoorDelay);
            SetTextboxNumber(txtScreensaversTime);
            SetTextboxNumber(txtServerPort);
            SetTextboxNumber(txtSleepTime);
            cbbVerifyMode.Items.AddRange(new object[] {
                new VerifyModeType("1", "PersonalPunchInMode"),
                new VerifyModeType("2", "Face or Finger or Card or Password"),
                new VerifyModeType("3", "Password + (Face or Finger)"),
                new VerifyModeType("4", "Card + (Face or Finger)"),
                new VerifyModeType("6", "Finger + Face")
            }) ;
            cbbLanguage.Items.AddRange(new object[] {
                 new LanguageType("ar","Arabic"),
                 new LanguageType("CHS","SimplifiedChinese"),
                 new LanguageType("CHT","TraditionalChinese"),
                 new LanguageType("en","English"),
                 new LanguageType("KR","Korean"),
                 new LanguageType("th","ThaiLanguage"),
                 new LanguageType("tr","Turkish"),
                 new LanguageType("es-AR","Spanish-Argentine"),
                 new LanguageType("es","Spanish"),
                 new LanguageType("pt","Portuguese"),
                 new LanguageType("pt-BR","Portuguese-Brazilian"),
                 new LanguageType("FRA","French"),
                 new LanguageType("id","Indonesian"),
                 new LanguageType("de","German"),
                 new LanguageType("fa","Persian"),
                 new LanguageType("ja","Japanese"),
                 new LanguageType("vi","Vietnamese"),
                 });
            RefreshParameter();
        }
        /// <summary>
        /// 从数据库获取参数
        /// </summary>
        public void RefreshParameter()
        {
            if (deviceDataGrid.Rows.Count > 0)
            {
                DataTableReader dr = null;

                string sql = "select * from parameterlist where [device_id]='" + deviceId + "'";

                try
                {
                    dr = Ole.GetDataReader(sql);
                    if (dr.Read())
                    {
                        txtDevName.Text = dr["device_name"].ToString();
                        cbbWiegandType.Text = dr["wiegand_type"].ToString();
                        txtServerHost.Text = dr["server_host"].ToString();
                        txtServerPort.Text = dr["server_port"].ToString();
                        txtPushServerHost.Text = dr["push_server_host"].ToString();
                        txtPushServerPort.Text = dr["push_server_port"].ToString();
                        txtInterval.Text = dr["interval"].ToString();
                        foreach(LanguageType type in cbbLanguage.Items)
                        {
                            if(type.id == dr["language"].ToString())
                            {
                                cbbLanguage.SelectedItem = type;
                                break;
                            }
                        }

                        cbbVolume.Text = dr["volume"].ToString();
                        cbbAntiPass.Text = dr["anti_pass"].ToString();
                        txtOpenDoorDelay.Text = dr["open_door_delay"].ToString();
                        cbbTamperAlarm.Text = dr["tamper_alarm"].ToString();
                        txtAlarmDelay.Text = dr["alarm_delay"].ToString();

                        foreach(VerifyModeType verify in cbbVerifyMode.Items)
                        {
                            if(verify.id == dr["verifyMode"].ToString())
                            {
                                cbbVerifyMode.SelectedItem = verify;
                                break;
                            }
                        }
                        txtReverifyTime.Text = dr["reverify_time"].ToString();
                        txtScreensaversTime.Text = dr["screensavers_time"].ToString();
                        txtSleepTime.Text = dr["sleep_time"].ToString();
                        cbbPushEnable.Text = dr["push_enable"].ToString();
                    }
                    else
                    {
                        DefaultParameter();
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
            }
            else
            {
                DefaultParameter();
            }

            Application.DoEvents();
        }
        /// <summary>
        /// 默认参数
        /// </summary>
        private void DefaultParameter()
        {
            txtDevName.Text = "Dynamic Face Recognition";
            cbbWiegandType.SelectedIndex = 1;
            txtServerHost.Text = GetLocalIP();
            txtServerPort.Text = "7005";
            txtPushServerHost.Text = GetLocalIP();
            txtPushServerPort.Text = "8001";
            txtInterval.Text = "1";

            cbbLanguage.SelectedIndex = 1;
            cbbVolume.SelectedIndex = 5;
            cbbAntiPass.SelectedIndex = 0;
            txtOpenDoorDelay.Text = "7";
            cbbTamperAlarm.SelectedIndex = 0;
            txtAlarmDelay.Text = "5";
            txtReverifyTime.Text = "1";
            txtScreensaversTime.Text = "0";
            txtSleepTime.Text = "0";
            cbbPushEnable.Text = "no";
        }
        /// <summary>
        /// 添加设备信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            frmDeviceAdd frm = new frmDeviceAdd();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                RefreshDeviceListData();
            }
        }
        /// <summary>
        /// 修改设备信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (deviceDataGrid.Rows.Count == 0) return;
            string id = deviceDataGrid[0, deviceDataGrid.CurrentRow.Index].Value.ToString();
            frmDeviceAdd frm = new frmDeviceAdd(id);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                RefreshDeviceListData();
            }
        }
        /// <summary>
        /// 删除设备信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (deviceDataGrid.Rows.Count == 0) return;

            string sql = "delete from devicelist where [device_id]='" + deviceId + "'";
            string paramSql = "delete from parameterlist where [device_id]='" + deviceId + "'";
            try
            {
                if (Ole.ExecSQL(sql) > 0)
                {
                    Ole.ExecSQL(paramSql);
                    RefreshDeviceListData();
                }
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message + "\r\n" + sql);
            }
        }
        /// <summary>
        /// 选择图片文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectPhoto_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFi = new OpenFileDialog();

            if (openFi.ShowDialog() == DialogResult.OK)
            {
                pbPerson.Image = null;
                pbPerson.Image = ImageHelper.CustomSizeImage(Image.FromFile(openFi.FileName));
            }
        }
        /// <summary>
        /// 清除界面的图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearPhoto_Click(object sender, EventArgs e)
        {
            pbPerson.Image = null;
        }
        /// <summary>
        /// 从界面获取人员信息
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="username"></param>
        /// <param name="privilege"></param>
        /// <param name="card"></param>
        /// <param name="pwd"></param>
        /// <param name="vaildstart"></param>
        /// <param name="vaildend"></param>
        /// <param name="photoBuff"></param>
        /// <returns></returns>
        public bool GetUserInfo(ref string userid, ref string username, ref string privilege, ref string card, ref string pwd, ref string vaildstart, ref string vaildend, ref byte[] photoBuff)
        {
            bool ret = false;
            userid = txtOleUserId.Text.Trim();
            username = txtOleName.Text.Trim();
            privilege = GetGeneralUserId(cbbOlePrivilege.Text);
            card = txtOleCard.Text.Trim();
            pwd = txtOlePwd.Text.Trim();
            vaildstart = dtOleVaildStart.Text.Trim().ToString();
            vaildend = dtOleVaildEnd.Text.Trim().ToString();

            if (pbPerson.Image != null)
            {
                MemoryStream ms = new MemoryStream();
                Bitmap t = new Bitmap(pbPerson.Image);
                t.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                photoBuff = ms.ToArray();
            }
            else
            {
                photoBuff = null;
            }

            if (userid == "")
            {
                MessageBox.Show("Please Enter UserId!");
                txtOleUserId.Focus();
                return ret;
            }

            ret = true;
            return ret;
        }
        /// <summary>
        /// 添加用户到数据库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddUser_Click(object sender, EventArgs e)
        {
            string userid = "";
            string username = "";
            string privilege = "";
            string card = "";
            string pwd = "";
            string vaildstart = "";
            string vaildend = "";
            string sql = "";
            string photoSql = "";
            bool isUpdate = false;
            DataTableReader dr = null;

            byte[] photoBuff = new byte[0];

            if (!GetUserInfo(ref userid, ref username, ref privilege, ref card, ref pwd, ref vaildstart, ref vaildend, ref photoBuff))
            {
                return;
            }
            try
            {
                if (Ole.SaveUser(userid, username, privilege, card, pwd, vaildstart, vaildend, ref isUpdate) > 0)//保存用户基本信息
                {
                    if (photoBuff != null)
                    {
                        photoSql = "update personlist set [photo]=@photo where [user_id]='" + userid + "'";//保存图片
                        Ole.UpdateByteData(photoSql, "photo", photoBuff);
                    }

                    RefreshPersonnelData();
                    showMsg("ADD Success!\r\n");
                }
                else
                {
                    showMsg("ADD Failed!\r\n");
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
        }
        /// <summary>
        /// 查看用户信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void personDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e != null)
            {
                if (e.RowIndex < 0)
                {
                    return;
                }

            }
            int index = personDataGrid.CurrentCell.RowIndex;
            if (index >= 0)
            {
                string sql = "";
                DataTableReader dr = null;
                lbTotalPerson.Text = personDataGrid.Rows.Count + "/" + (index + 1);
                try
                {
                    pbPerson.Image = null;
                    if (e != null)
                    {
                        if (e.ColumnIndex == 0)
                            SelectOneData(personDataGrid, index, !Convert.ToBoolean(personDataGrid[0, index].Value));//选择数据
                    }

                    txtOleUserId.Text = personDataGrid[2, index].Value.ToString();
                    txtOleName.Text = personDataGrid[3, index].Value.ToString();
                    cbbOlePrivilege.SelectedIndex = Convert.ToInt32(personDataGrid[4, index].Value);
                    txtOleCard.Text = personDataGrid[5, index].Value.ToString();
                    txtOlePwd.Text = personDataGrid[6, index].Value.ToString();

                    dtOleVaildStart.Text = personDataGrid[10, index].Value.ToString();
                    dtOleVaildEnd.Text = personDataGrid[11, index].Value.ToString();

                    sql = "select [photo] from personlist where [user_id]='" + txtOleUserId.Text + "'";
                    dr = Ole.GetDataReader(sql);

                    if (dr.Read())
                    {
                        //显示照片
                        if (!string.IsNullOrEmpty(dr["photo"].ToString()))
                        {
                            byte[] photoBuff = (Byte[])dr["photo"];
                            if (photoBuff.Length > 0)
                                pbPerson.Image = ImageHelper.BytesToImage(photoBuff);
                        }
                    }
                    cbbDataInfo_SelectedIndexChanged(null, null);
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
            }

        }
        /// <summary>
        /// 删除数据库的人员资料
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteOleUser_Click(object sender, EventArgs e)
        {
            if (personDataGrid.Rows.Count == 0) return;
            string id = "";
            List<string> sql = new List<string>();
            try
            {
                for (int i = 0; i < personDataGrid.Rows.Count; i++)
                {
                    if (Convert.ToBoolean(personDataGrid[0, i].Value))
                    {
                        id = personDataGrid[1, i].Value.ToString();
                        sql.Add("delete from personlist where [id]=" + id + "");
                    }
                }
                if (Ole.ExecSQL(sql) > 0)
                    RefreshPersonnelData();
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message + "\r\n" + sql);
            }
        }
        /// <summary>
        /// 修改数据库的人员资料
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEditUser_Click(object sender, EventArgs e)
        {
            string userid = "";
            string username = "";
            string privilege = "";
            string card = "";
            string pwd = "";
            string vaildstart = "";
            string vaildend = "";
            string sql = "";
            string photoSql = "";
            bool isUpdate = false;
            DataTableReader dr = null;
            byte[] photoBuff = new byte[0];

            if (!GetUserInfo(ref userid, ref username, ref privilege, ref card, ref pwd, ref vaildstart, ref vaildend, ref photoBuff))
            {
                return;
            }
            try
            {
                if (Ole.SaveUser(userid, username, privilege, card, pwd, vaildstart, vaildend, ref isUpdate) > 0)
                {
                    if (photoBuff != null)
                    {
                        photoSql = "update personlist set [photo]=@photo where [user_id]='" + userid + "'";
                        Ole.UpdateByteData(photoSql, "photo", photoBuff);
                    }
                    else
                    {
                        photoSql = "update personlist set [photo]=NULL where [user_id]='" + userid + "'";
                        Ole.ExecSQL(photoSql);
                    }

                    RefreshPersonnelData();
                    showMsg("Update Success!\r\n");
                }
                else
                {
                    showMsg("Update Failure!\r\n");
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
        }
        /// <summary>
        /// 查看记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void recordDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e != null)
            {
                if (e.RowIndex < 0)
                    return;
            }
            int index = recordDataGrid.CurrentCell.RowIndex;
            if (index >= 0)
            {
                string sql = "";
                DataTableReader dr = null;
                try
                {
                    lbTotalRecord.Text = recordDataGrid.Rows.Count + "/" + (index + 1);

                    sql = "select [log_photo] from recordlist where [id]=" + recordDataGrid[1, index].Value.ToString() + "";
                    dr = Ole.GetDataReader(sql);

                    if (dr.Read())
                    {
                        if (!string.IsNullOrEmpty(dr["log_photo"].ToString()))
                        {
                            byte[] photoBuff = (Byte[])dr["log_photo"];
                            if (photoBuff.Length > 0)
                                pbRecord.Image = ImageHelper.BytesToImage(photoBuff);
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
            }

        }
        /// <summary>
        /// 获取设备信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deviceDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = deviceDataGrid.CurrentCell.RowIndex;
            if (index >= 0)
            {
                deviceId = deviceDataGrid[1, index].Value.ToString();
                socKetIp = deviceDataGrid[4, index].Value.ToString();
                socKetPort = Convert.ToInt32(deviceDataGrid[5, index].Value);
                socKetPwd = Convert.ToInt32(deviceDataGrid[6, index].Value);
                RefreshParameter();
            }
        }
        /// <summary>
        /// 发送的数据
        /// </summary>
        private void SendData(ref StringBuilder jsonStringBuilder)
        {
            int index = socKetClient.Start(socKetIp, socKetPort, socKetPwd, txtReciveData);
            if (index > 0)
            {
                if (File.Exists(socKetClient.rfile))
                {
                    File.Delete(socKetClient.rfile);
                }

                if (File.Exists(socKetClient.sfile))
                {
                    File.Delete(socKetClient.sfile);
                }

                byte[] buffer = Encoding.UTF8.GetBytes(jsonStringBuilder.ToString());
                jsonStringBuilder = new StringBuilder("");
                if (socKetClient.SendData(socKetClient.GetSendBuff(buffer), ref jsonStringBuilder) > 0)
                {
                    showMsg("Send " + cmd + " Success!");
                }
            }
            else
            {
                showMsg("Send " + cmd + " Failed!");
                jsonStringBuilder = new StringBuilder("");
            }
        }
        /// <summary>
        /// 获取设备信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnGetDeviceInfo_Click(object sender, EventArgs e)
        {
            RefreshButton(false);
            cmd = "GetDeviceInfo";
            DeviceCmd getDeviceCmd = new DeviceCmd(cmd);
            StringBuilder jsonStringBuilder = new StringBuilder(JsonConvert.SerializeObject(getDeviceCmd));
            SendData(ref jsonStringBuilder);
            if (Recive(jsonStringBuilder) == 0)
            {
                _ResultInfo<DeviceInfo> deviceInfo = JsonConvert.DeserializeObject<_ResultInfo<DeviceInfo>>(jsonStringBuilder.ToString());
                maxBufferLen = deviceInfo.result_data.maxBufferLen;
                deviceId = deviceInfo.result_data.deviceId;
                deviceName = deviceInfo.result_data.name;
                lbDevInfo.Text = "DeviceInfo: " + JsonConvert.SerializeObject(deviceInfo);
            }
            if (e != null)
                RefreshButton(true);
        }
        /// <summary>
        /// 设置设备时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetTime_Click(object sender, EventArgs e)
        {
            RefreshButton(false);
            cmd = "SetTime";
            string syncTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            SetTimeCmd setTimeCmd = new SetTimeCmd(syncTime);
            _DeviceCmd<SetTimeCmd> devSetTimeCmd = new _DeviceCmd<SetTimeCmd>(cmd, setTimeCmd);
            StringBuilder jsonStringBuilder = new StringBuilder(JsonConvert.SerializeObject(devSetTimeCmd));
            SendData(ref jsonStringBuilder);
            Recive(jsonStringBuilder);
            RefreshButton(true);
        }
        /// <summary>
        /// 重启设备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRestartDev_Click(object sender, EventArgs e)
        {
            RefreshButton(false);
            cmd = "Restart";
            DeviceCmd devRestartCmd = new DeviceCmd(cmd);
            StringBuilder jsonStringBuilder = new StringBuilder(JsonConvert.SerializeObject(devRestartCmd));
            SendData(ref jsonStringBuilder);
            Recive(jsonStringBuilder);
            RefreshButton(true);
        }
        /// <summary>
        /// 清除管理员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCleraManager_Click(object sender, EventArgs e)
        {
            RefreshButton(false);
            cmd = "ClearManager";
            DeviceCmd devClearManagerCmd = new DeviceCmd(cmd);
            StringBuilder jsonStringBuilder = new StringBuilder(JsonConvert.SerializeObject(devClearManagerCmd));
            SendData(ref jsonStringBuilder);
            Recive(jsonStringBuilder);
            RefreshButton(true);
        }
        /// <summary>
        /// 获取用户id列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGetUserList_Click(object sender, EventArgs e)
        {
            RefreshButton(false);
            try
            {
                string idInfoStr = "";
                cmd = "GetUserIdList";
                GetUserIdListCmd getUserIdListCmd = new GetUserIdListCmd(0);
                _DeviceCmd<GetUserIdListCmd> devGetUserIdListCmd = new _DeviceCmd<GetUserIdListCmd>(cmd, getUserIdListCmd);
                RE:  
                StringBuilder jsonStringBuilder = new StringBuilder(JsonConvert.SerializeObject(devGetUserIdListCmd));
                SendData(ref jsonStringBuilder);

                if (Recive(jsonStringBuilder) == 0)
                {
                    _ResultInfo<UserListInfo<UserIdName>> personIDList = JsonConvert.DeserializeObject<_ResultInfo<UserListInfo<UserIdName>>>(jsonStringBuilder.ToString());
                    dtInsertPersonData.Clear();

                    for (int i = 0; i < personIDList.result_data.usersCount; i++)
                    {
                        idInfoStr += "[" + personIDList.result_data.users[i].userId + "[" + personIDList.result_data.users[i].name + "] ";
                        DataRow[] selectRow = dtOlePersonData.Select("user_id='" + personIDList.result_data.users[i].userId + "'");
                        if (selectRow.Length == 0)
                        {
                            dtInsertPersonData.Rows.Add(new object[] {0,  personIDList.result_data.users[i].userId , personIDList.result_data.users[i].name,0, "","",
                                                           null, null,null,null,null,null,null,null,null,null,
                                                           "0",null,"0",null,"0",null,"","",
                                                           });

                        }
                        if (dtInsertPersonData.Rows.Count > 1000)
                        {
                            Ole.batchSeveData(dtInsertPersonData, "personlist");
                            dtInsertPersonData.Clear();
                        }
                    }
                    if (dtInsertPersonData.Rows.Count > 0)
                    {
                        Ole.batchSeveData(dtInsertPersonData, "personlist");
                        dtInsertPersonData.Clear();
                    }

                    RefreshPersonnelData();
                    showMsg(idInfoStr);

                    if (personIDList.result_data.packageId != 0)
                    {
                        devGetUserIdListCmd.data.packageId++;
                        goto RE;
                    }
                }
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
            }
            finally
            {
                RefreshButton(true);
            }
        }

        public List<string> GetPersonGridUserId()
        {
            List<string> usersId = new List<string>();

            if (personDataGrid.Rows.Count == 0)
            {
                MessageBox.Show("Please add people to the list first!");
                return usersId;
            }
            try
            {
                for (int i = 0; i < personDataGrid.Rows.Count; i++)
                {
                    if (Convert.ToBoolean(personDataGrid[0, i].Value))
                    {
                        usersId.Add(personDataGrid[2, i].Value.ToString());
                    }
                }
                if (usersId.Count == 0)
                {
                    MessageBox.Show("Please Select personnel!");
                }
            }
            catch (Exception E)
            {
                throw (E);
            }
            return usersId;
        }
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGetUserInfo_Click(object sender, EventArgs e)
        {
            RefreshButton(false);
            int UserCount = 0;
            GetUserInfoCmd getUserInfoCmd = null;
            try
            {
                if (cbGetUserInfoAll.Checked)
                {
                    #region 首先获取机器上的所有用户id
                    cmd = "GetDeviceInfo";
                    DeviceCmd getDeviceCmd = new DeviceCmd(cmd);
                    StringBuilder jsonStringBuilder = new StringBuilder(JsonConvert.SerializeObject(getDeviceCmd));
                    SendData(ref jsonStringBuilder);
                    if (Recive(jsonStringBuilder) == 0)
                    {
                        _ResultInfo<DeviceInfo> deviceInfo = JsonConvert.DeserializeObject<_ResultInfo<DeviceInfo>>(jsonStringBuilder.ToString());
                        UserCount = deviceInfo.result_data.userCount;
                    }
                    else
                    {
                        return;
                    }
                    getUserInfoCmd = new GetUserInfoCmd(PACKAGE_ID, null);
                    #endregion
                   
                }
                else
                {
                    usersIDList = GetPersonGridUserId();
                    if (usersIDList == null) return;
                    UserCount = usersIDList.Count;
                    getUserInfoCmd = new GetUserInfoCmd(PACKAGE_ID, usersIDList);
                }
               
                if (UserCount > 0)
                {
                    cmd = "GetUserInfo";
                    PersonInfoList.Clear();
                   
                    _DeviceCmd<GetUserInfoCmd> devGetUserInfoCmd = new _DeviceCmd<GetUserInfoCmd>(cmd, getUserInfoCmd);
                    while (true)
                    {
                        StringBuilder jsonStringBuilder = new StringBuilder(JsonConvert.SerializeObject(devGetUserInfoCmd));
                        SendData(ref jsonStringBuilder);

                        if (Recive(jsonStringBuilder) == 0)
                        {
                            _ResultInfo<PersonInfo<GetUsers>> getUserInfo = JsonConvert.DeserializeObject<_ResultInfo<PersonInfo<GetUsers>>>(jsonStringBuilder.ToString());

                            if (getUserInfo.result_data.users == null)
                            {
                                showMsg("No personnel information found!");
                                return;
                            }

                            PersonInfoList.Add(getUserInfo);

                            if (getUserInfo.result_data.packageId != 0)//表示没有获取完数据，让packageId+1，重新发送获取获取下一包数据
                            {
                                devGetUserInfoCmd.data.packageId++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                  
                    if (PersonInfoList.Count > 0)
                        SavePersonInfo();
                }

            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
            }
            finally
            {
                RefreshButton(true);
            }
        }
        /// <summary>
        /// 设置用户信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetUserInfo_Click(object sender, EventArgs e)
        {
            RefreshButton(false);
            string ID = "";
            string name = "";
            int privilege = 0;
            string card = "";
            string pwd = "";
            List<string> fps = new List<string>();
            string face = null;
            string palm = null;
            string photo = null;
            string vaildStart = "";
            string vaildEnd = "";
            byte update = Convert.ToByte(cbUpdate.Checked);
            byte photoEnroll = Convert.ToByte(cbPhotoEnroll.Checked);
            byte[] buf = null;
            try
            {
                List<string> userIdList = GetPersonGridUserId(); //获取选择的用户id列表
                if (userIdList.Count == 0) return;

                List<SetUsers> usersList = new List<SetUsers>();
                DataTableReader dr = null;
               
               
                maxBufferLen = 0;

                int BufferLen = 0;
                SetUsers setUsers = null;
                SetUserInfoCmd<SetUsers> setUserInfoCmd = null;
                _DeviceCmd<SetUserInfoCmd<SetUsers>> devSetUserInfoCmd = null;

                #region 获取可发送数据的最大值
                cmd = "GetDeviceInfo";
                DeviceCmd getDeviceCmd = new DeviceCmd(cmd);
                StringBuilder jsonStringBuilder = new StringBuilder(JsonConvert.SerializeObject(getDeviceCmd));
                SendData(ref jsonStringBuilder);
                if (Recive(jsonStringBuilder) == 0)
                {
                    _ResultInfo<DeviceInfo> deviceInfo = JsonConvert.DeserializeObject<_ResultInfo<DeviceInfo>>(jsonStringBuilder.ToString());
                    maxBufferLen = deviceInfo.result_data.maxBufferLen;
                }
                #endregion

                cmd = "SetUserInfo";
                usersList.Clear();
                for (int i = 0; i < userIdList.Count; i++)
                {
                    lbRestate.Text = (i + 1) + "/" + userIdList.Count;
                    Application.DoEvents();
                    fps = new List<string>();
                    face = null;
                    palm = null;
                    photo = null;
                    vaildStart = "";
                    vaildEnd = "";
                    dr = Ole.GetDataReader("select * from personlist where user_id='" + userIdList[i] + "'");
                    if (dr.Read())
                    {
                        name = dr["user_name"].ToString();
                        Int32.TryParse(dr["privilege"].ToString(), out privilege);
                        card = dr["card"].ToString();
                        pwd = dr["pwd"].ToString();
                        if (card == "") card = null;
                        if (pwd == "") pwd = null;
                        for (int j = 0; j < 10; j++)
                        {
                            if (!string.IsNullOrEmpty(dr["fps0" + j].ToString()))
                            {
                                buf = (byte[])(dr["fps0" + j]);
                                fps.Add(Convert.ToBase64String(buf));
                            }

                        }
                        if (!string.IsNullOrEmpty(dr["face00"].ToString()))
                        {
                            buf = (byte[])(dr["face00"]);
                            face = Convert.ToBase64String(buf);
                        }

                        if (!string.IsNullOrEmpty(dr["palm00"].ToString()))
                        {
                            buf = (byte[])(dr["palm00"]);
                            palm = Convert.ToBase64String(buf);
                        }

                        if (!string.IsNullOrEmpty(dr["photo"].ToString()))
                        {
                            buf = (byte[])(dr["photo"]);
                            photo = Convert.ToBase64String(buf);
                        }
                        vaildStart = dr["vaild_start"].ToString();
                        IsDateTime("yyyyMMdd", ref vaildStart);

                        vaildEnd = dr["vaild_end"].ToString();
                        IsDateTime("yyyyMMdd", ref vaildEnd);
                    }

                    setUsers = new SetUsers(userIdList[i], name, privilege, card, pwd, fps, face, palm, photo, vaildStart, vaildEnd, update, photoEnroll);
                    BufferLen += CalculatedLength(setUsers);
                    if (BufferLen > maxBufferLen)
                    {
                        setUserInfoCmd = new SetUserInfoCmd<SetUsers>(usersList);
                        devSetUserInfoCmd = new _DeviceCmd<SetUserInfoCmd<SetUsers>>(cmd, setUserInfoCmd);
                        jsonStringBuilder = new StringBuilder(JsonConvert.SerializeObject(devSetUserInfoCmd));
                        SendData(ref jsonStringBuilder);
                        if (Recive(jsonStringBuilder) == 0)
                        {
                            _ResultInfo<SetUsersErorr> resultInfo = JsonConvert.DeserializeObject<_ResultInfo<SetUsersErorr>>(jsonStringBuilder.ToString());
                            if (resultInfo.result_data != null)
                            {
                                foreach (string id in resultInfo.result_data.usersId)
                                {
                                    ID += " [" + id + "] ";
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                        BufferLen = CalculatedLength(setUsers);
                        usersList.Clear();
                    }

                    usersList.Add(setUsers);

                }
                if (usersList.Count > 0)
                {
                    setUserInfoCmd = new SetUserInfoCmd<SetUsers>(usersList);
                    devSetUserInfoCmd = new _DeviceCmd<SetUserInfoCmd<SetUsers>>(cmd, setUserInfoCmd);
                    jsonStringBuilder = new StringBuilder(JsonConvert.SerializeObject(devSetUserInfoCmd));
                    string ii = jsonStringBuilder.ToString();
                    SendData(ref jsonStringBuilder);
                    if (Recive(jsonStringBuilder) == 0)
                    {
                        _ResultInfo<SetUsersErorr> resultInfo = JsonConvert.DeserializeObject<_ResultInfo<SetUsersErorr>>(jsonStringBuilder.ToString());
                        if (resultInfo.result_data != null)
                        {
                            foreach (string id in resultInfo.result_data.usersId)
                            {
                                ID += " [" + id + "] ";
                            }
                        }
                    }
                }
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
            }
            finally
            {
                lbRestate.Text = "未成功人员编号：" + ID;
                RefreshButton(true);
            }
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteUser_Click(object sender, EventArgs e)
        {
            RefreshButton(false);
            try
            {
                DeleteUserInfoCmd deleteUserInfoCmd = null;
                if (MessageBox.Show("Are you sure you want to delete the personnel of the equipment?", null, MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }
                if (e != null)
                {
                    if (cbDeleteUserInfoAll.Checked)
                    {
                        deleteUserInfoCmd = new DeleteUserInfoCmd(0, null);
                    }
                    else
                    {
                        usersIDList = GetPersonGridUserId();
                        if (usersIDList == null) return;
                        deleteUserInfoCmd = new DeleteUserInfoCmd(usersIDList.Count, usersIDList);
                    }
                }

                cmd = "DeleteUserInfo";
                _DeviceCmd<DeleteUserInfoCmd> devDeleteUserInfoCmd = new _DeviceCmd<DeleteUserInfoCmd>(cmd, deleteUserInfoCmd);
                StringBuilder jsonStringBuilder = new StringBuilder(JsonConvert.SerializeObject(devDeleteUserInfoCmd));
                SendData(ref jsonStringBuilder);
                Recive(jsonStringBuilder);
            }
            catch (Exception E)
            {
                showMsg(E.Message);
            }
            finally
            {
                RefreshButton(true);
            }
        }
        /// <summary>
        /// 进入注册界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEnterEnroll_Click(object sender, EventArgs e)
        {
            RefreshButton(false);
            try
            {
                string cmd = "EnterEnroll";
                string userId = txtOleUserId.Text.Trim();
                if (userId == "")
                {
                    MessageBox.Show("Please Enter UserId!");
                    txtOleUserId.Focus();
                    return;
                }
                string feature = cbbEnterEnrollFeature.Text;
                EnterEnrollCmd enterEnrollCmd = new EnterEnrollCmd(userId, feature);
                _DeviceCmd<EnterEnrollCmd> devEnterEnrollCmd = new _DeviceCmd<EnterEnrollCmd>(cmd, enterEnrollCmd);
                StringBuilder jsonStringBuilder = new StringBuilder(JsonConvert.SerializeObject(devEnterEnrollCmd));
                SendData(ref jsonStringBuilder);
                Recive(jsonStringBuilder);
            }
            catch (Exception E)
            {
                showMsg(E.Message);
            }
            finally
            {
                RefreshButton(true);
            }
        }
        /// <summary>
        /// 获取设备的记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGetLogData_Click(object sender, EventArgs e)
        {
            GetLog(0);
        }

        private void btnGetNewLog_Click(object sender, EventArgs e)
        {
            GetLog(1);
        }

        public void GetLog(byte NewLog)
        {

            RefreshButton(false);
            try
            {
                Ole.ExecSQL("delete from recordlist");
            }
            catch
            {

            }
            try
            {
                RecordInfoList.Clear();
                cmd = "GetLogData";
                string beginTime = txtStartTime.Text.Trim();
                IsDateTime("yyyyMMdd", ref beginTime);
                string endTime = txtEndTime.Text.Trim();
                IsDateTime("yyyyMMdd", ref endTime);

                GetLogDataCmd getLogDataCmd = new GetLogDataCmd(PACKAGE_ID, NewLog, beginTime, endTime, Convert.ToByte(chkClearMark.Checked));
                _DeviceCmd<GetLogDataCmd> devGetLogDataCmd = new _DeviceCmd<GetLogDataCmd>(cmd, getLogDataCmd);


                while (true)
                {
                    StringBuilder jsonStringBuilder = new StringBuilder(JsonConvert.SerializeObject(devGetLogDataCmd));
                    SendData(ref jsonStringBuilder);
                    if (Recive(jsonStringBuilder) == 0)
                    {
                        _ResultInfo<RecordInfo<Logs>> getLogInfo = JsonConvert.DeserializeObject<_ResultInfo<RecordInfo<Logs>>>(jsonStringBuilder.ToString());

                        if (getLogInfo.result_data.logs == null)
                        {
                            showMsg("No Logs information found!");
                            return;
                        }

                        RecordInfoList.Add(getLogInfo);

                        if (getLogInfo.result_data.packageId != 0)//表示没有获取完数据，让packageId+1，重新发送获取下一包数据
                        {
                            devGetLogDataCmd.data.packageId++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }

                }
                if (RecordInfoList.Count > 0)
                    SaveRecordInfo();
            }
            catch (Exception E)
            {
                showMsg(E.Message);
            }
            finally
            {
                RefreshButton(true);
            }
        }

        /// <summary>
        /// 清空设备记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearLogData_Click(object sender, EventArgs e)
        {
            RefreshButton(false);
            try
            {
                cmd = "ClearLogData";
                DeviceCmd devClearLogDataCmd = new DeviceCmd(cmd);
                StringBuilder jsonStringBuilder = new StringBuilder(JsonConvert.SerializeObject(devClearLogDataCmd));
                SendData(ref jsonStringBuilder);
                Recive(jsonStringBuilder);
            }
            catch (Exception E)
            {
                showMsg(E.Message);
            }
            finally
            {
                RefreshButton(true);
            }
        }
        /// <summary>
        /// 获取设备参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGetDeviceSetting_Click(object sender, EventArgs e)
        {
            RefreshButton(false);
            try
            {
                cmd = "GetDeviceSetting";
                DeviceCmd devGetDeviceSettingCmd = new DeviceCmd(cmd);
                StringBuilder jsonStringBuilder = new StringBuilder(JsonConvert.SerializeObject(devGetDeviceSettingCmd));
                SendData(ref jsonStringBuilder);

                if (Recive(jsonStringBuilder) == 0)
                {
                    parameterInfo = JsonConvert.DeserializeObject<_ResultInfo<ParameterInfo>>(jsonStringBuilder.ToString());

                 
                    if (Ole.SaveParameter(deviceId, parameterInfo.result_data.devName,
                                   parameterInfo.result_data.wiegandType,
                                   parameterInfo.result_data.serverHost,
                                   parameterInfo.result_data.serverPort,
                                   parameterInfo.result_data.pushServerHost,
                                   parameterInfo.result_data.pushServerPort,
                                   parameterInfo.result_data.interval,
                                   parameterInfo.result_data.language,
                                   parameterInfo.result_data.volume,
                                   parameterInfo.result_data.antiPass,
                                   parameterInfo.result_data.openDoorDelay,
                                   parameterInfo.result_data.tamperAlarm,
                                   parameterInfo.result_data.alarmDelay,
                                   parameterInfo.result_data.reverifyTime,
                                   parameterInfo.result_data.screensaversTime,
                                   parameterInfo.result_data.sleepTime,
                                   parameterInfo.result_data.pushEnable,
                                   parameterInfo.result_data.verifyMode) > 0)
                    {
                        RefreshParameter();
                    }
                }
            }
            catch (Exception E)
            {
                showMsg(E.Message);
            }
            finally
            {
                RefreshButton(true);
            }
        }
        /// <summary>
        /// 设备设备参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetDeviceSetting_Click(object sender, EventArgs e)
        {
            RefreshButton(false);
            try
            {
                string deviceName = txtDevName.Text.Trim();
                string wiegandType = cbbWiegandType.Text;
                string serverHost = txtServerHost.Text.Trim();
                string serverPort = txtServerPort.Text.Trim();
                string pushServerHost = txtPushServerHost.Text.Trim();
                string pushServerPort = txtPushServerPort.Text.Trim();
                string interval = txtInterval.Text.Trim();
                string language = ((LanguageType)cbbLanguage.SelectedItem).id;
                string volume = cbbVolume.Text;
                string antiPass = cbbAntiPass.Text;
                string openDoorDelay = txtOpenDoorDelay.Text.Trim();
                string tamperAlarm = cbbTamperAlarm.Text;
                string alarmDelay = txtAlarmDelay.Text.Trim();
                string reverifyTime = txtReverifyTime.Text.Trim();
                string screensaversTime = txtScreensaversTime.Text.Trim();
                string sleepTime = txtSleepTime.Text.Trim();
                string pushEnable = cbbPushEnable.Text.Trim();
                string verifyMode = ((VerifyModeType)cbbVerifyMode.SelectedItem).id;

                Ole.SaveParameter(deviceId, deviceName, wiegandType, serverHost, serverPort, pushServerHost, pushServerPort, interval, language, volume, antiPass,
                              openDoorDelay, tamperAlarm, alarmDelay, reverifyTime, screensaversTime, sleepTime, pushEnable, verifyMode);


                cmd = "SetDeviceSetting";

                ParameterInfo parameterInfoCmd = new ParameterInfo(deviceName, wiegandType, serverHost, serverPort, pushServerHost, pushServerPort, interval, language, volume, antiPass,
                              openDoorDelay, tamperAlarm, alarmDelay, reverifyTime, screensaversTime, sleepTime, pushEnable, verifyMode);
                _DeviceCmd<ParameterInfo> devParameterInfoCmd = new _DeviceCmd<ParameterInfo>(cmd, parameterInfoCmd);
                StringBuilder jsonStringBuilder = new StringBuilder(JsonConvert.SerializeObject(devParameterInfoCmd));
                SendData(ref jsonStringBuilder);
                Recive(jsonStringBuilder);
            }
            catch (Exception E)
            {
                showMsg(E.Message);
            }
            finally
            {
                RefreshButton(true);
            }
        }

        /// <summary>
        /// 显示信息
        /// </summary>
        /// <param name="text"></param>
        public void showMsg(string text)
        {
            txtReciveData.AppendText("\r\n" + text + "\r\n");
        }

        /// <summary>
        /// 显示错误信息
        /// </summary>
        /// <param name="e"></param>
        public void ShowErorr(int e)
        {
            switch (e)
            {
                case ResultCode.EQUIPMENT_CAPACITY_FULL:
                    showMsg("Equipment capacity is full!");
                    break;
                case ResultCode.COMMUNICATION_ERROR:
                    showMsg("Communication error!");
                    break;
                case ResultCode.TIMEOUT_ERROR:
                    showMsg("Timeout error!");
                    break;
                case ResultCode.PARAMETER_ERROR:
                    showMsg("Parameter error!");
                    break;
                case ResultCode.ERROR:
                    showMsg("Error!");
                    break;
                case ResultCode.SUCCESS:
                    showMsg("Success!");
                    break;
                case ResultCode.EQUIPMENT_BUSY:
                    showMsg("Equipment busy!");
                    break;
                case ResultCode.DURING_EQUIPMENT_OPERATION:
                    showMsg("During equipment operation!");
                    break;
                case ResultCode.EQUIPMENT_WORK_COMPLETED:
                    showMsg("Equipment work completed!");
                    break;
                default:
                    showMsg("Unknown error!");
                    break;
            }
        }

        /// <summary>
        /// 解析从设备获取到的数据
        /// </summary>
        public int Recive(StringBuilder jsonStringBuilder)
        {
            string reciveDataStr = jsonStringBuilder.ToString();
            if (!reciveDataStr.Contains("cmd"))
            {
                showMsg(reciveDataStr);
                return -6;
            }

            _AnswerInfo resultInfo = JsonConvert.DeserializeObject<_AnswerInfo>(reciveDataStr);
            if (resultInfo.result_code == 0)
            {
                showMsg(JsonConvert.SerializeObject(resultInfo));
            }
            else
            {
                ShowErorr(resultInfo.result_code);
            }
            return resultInfo.result_code;
        }
        /// <summary>
        /// 保存人员
        /// </summary>
        public void SavePersonInfo()
        {
            if (PersonInfoList.Count > 0)
            {
                byte[] fps00 = new byte[0];
                byte[] fps01 = new byte[0];
                byte[] fps02 = new byte[0];
                byte[] fps03 = new byte[0];
                byte[] fps04 = new byte[0];
                byte[] fps05 = new byte[0];
                byte[] fps06 = new byte[0];
                byte[] fps07 = new byte[0];
                byte[] fps08 = new byte[0];
                byte[] fps09 = new byte[0];
                byte[] face00 = new byte[0];
                byte[] palm00 = new byte[0];
                byte[] photo = new byte[0];
                DataRow[] selectRow = null;
                long id = 0;
                dtInsertPersonData.Clear();
                dtUpdatePersonData.Clear();

                int count = 0;
                for (int l = 0; l < PersonInfoList.Count; l++)
                {
                    _ResultInfo<PersonInfo<GetUsers>> getUserInfo = PersonInfoList[l];
                    for (int i = 0; i < getUserInfo.result_data.users.Length; i++)
                    {
                        count++;
                        #region 批量保存
                        fps00 = null;
                        fps01 = null;
                        fps02 = null;
                        fps03 = null;
                        fps04 = null;
                        fps05 = null;
                        fps06 = null;
                        fps07 = null;
                        fps08 = null;
                        fps09 = null;
                        face00 = null;
                        palm00 = null;
                        photo = null;
                        if (getUserInfo.result_data.users[i].photo != null)
                        {
                            string photoStr = getUserInfo.result_data.users[i].photo;
                            photo = Convert.FromBase64String(photoStr);
                        }
                        if (getUserInfo.result_data.users[i].face != null)
                        {
                            string faceStr = getUserInfo.result_data.users[i].face;
                            face00 = Convert.FromBase64String(faceStr);
                        }
                        if (getUserInfo.result_data.users[i].palm != null)
                        {
                            string palmStr = getUserInfo.result_data.users[i].palm;
                            palm00 = Convert.FromBase64String(palmStr);
                        }
                        if (getUserInfo.result_data.users[i].fps != null)
                        {
                            for (int j = 0; j < getUserInfo.result_data.users[i].fps.Count; j++)
                            {
                                if (getUserInfo.result_data.users[i].fps[j] != null)
                                {
                                    string fpsStr = getUserInfo.result_data.users[i].fps[j];
                                    switch (j)
                                    {
                                        case 0:
                                            fps00 = Convert.FromBase64String(fpsStr);
                                            break;
                                        case 1:
                                            fps01 = Convert.FromBase64String(fpsStr);
                                            break;
                                        case 2:
                                            fps02 = Convert.FromBase64String(fpsStr);
                                            break;
                                        case 3:
                                            fps03 = Convert.FromBase64String(fpsStr);
                                            break;
                                        case 4:
                                            fps04 = Convert.FromBase64String(fpsStr);
                                            break;
                                        case 5:
                                            fps05 = Convert.FromBase64String(fpsStr);
                                            break;
                                        case 6:
                                            fps06 = Convert.FromBase64String(fpsStr);
                                            break;
                                        case 7:
                                            fps07 = Convert.FromBase64String(fpsStr);
                                            break;
                                        case 8:
                                            fps08 = Convert.FromBase64String(fpsStr);
                                            break;
                                        case 9:
                                            fps09 = Convert.FromBase64String(fpsStr);
                                            break;
                                    }

                                }
                            }
                        }

                        selectRow = dtOlePersonData.Select("user_id='" + getUserInfo.result_data.users[i].userId + "'");

                        if (selectRow.Length > 0)
                        {
                            Int64.TryParse(selectRow[0]["id"].ToString(), out id);
                            dtUpdatePersonData.Rows.Add(new object[] {id, getUserInfo.result_data.users[i].userId,
                                                           getUserInfo.result_data.users[i].name,
                                                           getUserInfo.result_data.users[i].privilege.ToString(),
                                                           getUserInfo.result_data.users[i].card,
                                                           getUserInfo.result_data.users[i].pwd,
                                                           fps00,
                                                           fps01,
                                                           fps02,
                                                           fps03,
                                                           fps04,
                                                           fps05,
                                                           fps06,
                                                           fps07,
                                                           fps08,
                                                           fps09,
                                                           "0",
                                                           face00,
                                                           "0",
                                                           palm00,
                                                           "0",
                                                           photo,
                                                           getUserInfo.result_data.users[i].vaildStart,
                                                           getUserInfo.result_data.users[i].vaildEnd
                                                           });
                        }
                        else
                        {
                            dtInsertPersonData.Rows.Add(new object[] {0, getUserInfo.result_data.users[i].userId,
                                                           getUserInfo.result_data.users[i].name,
                                                           getUserInfo.result_data.users[i].privilege.ToString(),
                                                           getUserInfo.result_data.users[i].card,
                                                           getUserInfo.result_data.users[i].pwd,
                                                           fps00,
                                                           fps01,
                                                           fps02,
                                                           fps03,
                                                           fps04,
                                                           fps05,
                                                           fps06,
                                                           fps07,
                                                           fps08,
                                                           fps09,
                                                           "0",
                                                           face00,
                                                           "0",
                                                           palm00,
                                                           "0",
                                                           photo,
                                                           getUserInfo.result_data.users[i].vaildStart,
                                                           getUserInfo.result_data.users[i].vaildEnd
                                                           });

                            dtOlePersonData.Rows.Add(new object[] {0, getUserInfo.result_data.users[i].userId,
                                                           getUserInfo.result_data.users[i].name,
                                                           getUserInfo.result_data.users[i].privilege.ToString(),
                                                           getUserInfo.result_data.users[i].card,
                                                           getUserInfo.result_data.users[i].pwd,
                                                           "0",
                                                           "0",
                                                           "0",
                                                           getUserInfo.result_data.users[i].vaildStart,
                                                           getUserInfo.result_data.users[i].vaildEnd
                                                           });

                        }

                        if (dtInsertPersonData.Rows.Count > 1000)
                        {
                            Ole.batchSeveData(dtInsertPersonData, "personlist");
                            dtInsertPersonData.Clear();
                        }
                        if (dtUpdatePersonData.Rows.Count > 1000)
                        {
                            Ole.batchUpdateData(dtUpdatePersonData, "personlist");
                            dtUpdatePersonData.Clear();
                        }
                        #endregion
                        #region 一条一条的保存数据
                        /*if (SaveUser(getUserInfo.result_data.users[i].userId, getUserInfo.result_data.users[i].name,
                            getUserInfo.result_data.users[i].privilege.ToString(), getUserInfo.result_data.users[i].card, getUserInfo.result_data.users[i].pwd,
                            getUserInfo.result_data.users[i].vaildStart, getUserInfo.result_data.users[i].vaildEnd) > 0)
                        {
                            if (getUserInfo.result_data.users[i].photo != null)
                            {
                                sql = "update personlist set [photo]=@photo where [user_id]='" + getUserInfo.result_data.users[i].userId + "'";
                                string photoStr = getUserInfo.result_data.users[i].photo;

                                Ole.UpdateByteData(sql, "photo", Convert.FromBase64String(photoStr));
                            }
                            if (getUserInfo.result_data.users[i].face != null)
                            {
                                sql = "update personlist set [face00]=@face00 where [user_id]='" + getUserInfo.result_data.users[i].userId + "'";
                                string faceStr = getUserInfo.result_data.users[i].face;
                                Ole.UpdateByteData(sql, "face00", Convert.FromBase64String(faceStr));
                            }
                            if (getUserInfo.result_data.users[i].palm != null)
                            {
                                sql = "update personlist set [palm00]=@palm00 where [user_id]='" + getUserInfo.result_data.users[i].userId + "'";
                                string palmStr = getUserInfo.result_data.users[i].palm;
                                Ole.UpdateByteData(sql, "palm00", Convert.FromBase64String(palmStr));
                            }
                            if (getUserInfo.result_data.users[i].fps != null)
                            {
                                for (int j = 0; j < getUserInfo.result_data.users[i].fps.Count; j++)
                                {
                                    if (getUserInfo.result_data.users[i].fps[j] != null)
                                    {
                                        sql = "update personlist set [fps0" + j + "]=@fps0" + j + " where [user_id]='" + getUserInfo.result_data.users[i].userId + "'";
                                        string fpsStr = getUserInfo.result_data.users[i].fps[j];
                                        Ole.UpdateByteData(sql, "fps0" + j, Convert.FromBase64String(fpsStr));
                                    }
                                }
                            }
                        }*/
                        #endregion

                        lbTotalPerson.Text = usersIDList.Count + "/" + count;
                        Application.DoEvents();
                    }
                }
                if (dtInsertPersonData.Rows.Count > 0)
                {
                    Ole.batchSeveData(dtInsertPersonData, "personlist");
                    dtInsertPersonData.Clear();
                }
                if (dtUpdatePersonData.Rows.Count > 0)
                {
                    Ole.batchUpdateData(dtUpdatePersonData, "personlist");
                    dtUpdatePersonData.Clear();
                }
            }

            RefreshPersonnelData();
        }
        /// <summary>
        /// 保存记录
        /// </summary>
        public void SaveRecordInfo()
        {
            int count = 0;
            dtRecordData.Clear();
            dtInsertRecordData.Clear();
            _ResultInfo<RecordInfo<Logs>> logInfo = null;
            string dateTime = "";

            for (int l = 0; l < RecordInfoList.Count; l++)
            {
                logInfo = RecordInfoList[l];
                for (int i = 0; i < logInfo.result_data.logsCount; i++)
                {
                    count++;
                    dateTime = Ole.stringToTimeStr(logInfo.result_data.logs[i].time);
                    #region 一条条保存
                    /*SaveLogsData(deviceId, logInfo.result_data.logs[i].userId, dateTime,
                         logInfo.result_data.logs[i].verifyMode,
                         logInfo.result_data.logs[i].ioMode.ToString(),
                         logInfo.result_data.logs[i].inOut);*/
                    #endregion

                    #region 批量保存

                    object[] objRecord = new object[] {0, deviceId, logInfo.result_data.logs[i].userId,
                                                         dateTime, logInfo.result_data.logs[i].verifyMode, logInfo.result_data.logs[i].ioMode.ToString(),
                                                         logInfo.result_data.logs[i].inOut,null };
                    if (cbFilterDuplicateData.Checked)
                    {
                        //过滤重复数据，精确秒
                        if (dtRecordData.Select("user_id='" + logInfo.result_data.logs[i].userId + "' AND time='" + dateTime + "'").Length == 0)
                        {
                            dtInsertRecordData.Rows.Add(objRecord);
                        }

                    }
                    else
                    {
                        dtInsertRecordData.Rows.Add(objRecord);
                    }

                    if (dtRecordData.Rows.Count > 1000)
                    {
                        Ole.batchSeveData(dtRecordData, "recordlist");
                        dtInsertRecordData.Clear();
                    }
                    #endregion
                    lbTotalRecord.Text = logInfo.result_data.allLogCount + "/" + count;
                    Application.DoEvents();
                }
            }
            if (dtInsertRecordData.Rows.Count > 0)
            {
                Ole.batchSeveData(dtInsertRecordData, "recordlist");
            }
            RefreshRecordData();
        }

        private void timer_Tick(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 查看指纹、人脸、掌纹等数据的HEX信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbbDataInfo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string data = cbbDataInfo.Text.Trim();
            if (data != "")
            {
                txtReciveData.Text = "";
                string sql = "select [" + data + "] from personlist where [user_id] = '" + txtOleUserId.Text + "'";
                DataTableReader dr = Ole.GetDataReader(sql);

                if (dr.Read())
                {
                    if (!string.IsNullOrEmpty(dr[0].ToString()))
                    {
                        byte[] photoBuff = (Byte[])dr[0];
                        if (photoBuff.Length > 0)
                        {
                            txtReciveData.Text = ByteToHexString(photoBuff);
                        }

                    }

                }
            }

        }
        /// <summary>
        /// 在表中添加序号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var dgv = sender as DataGridView;
            if (dgv != null)
            {
                Rectangle rect = new Rectangle(e.RowBounds.Location.X, e.RowBounds.Location.Y, dgv.RowHeadersWidth - 4, e.RowBounds.Height);
                TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
                    dgv.RowHeadersDefaultCellStyle.Font, rect, dgv.RowHeadersDefaultCellStyle.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
                dgv[0, 0].Value = "";
            }
        }

        /// <summary>
        /// 打开实时监控
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenMonitor_Click(object sender, EventArgs e)
        {
            RefreshButton(false);
            httpServer = new HttpServer();
            try
            {
                httpServer.Setup(Convert.ToInt32(txtMonitoring.Text.Trim()), lbtime, recordDataGrid, personDataGrid);
                showMsg("Real-time monitoring is Open!");
            }
            catch (Exception E)
            {
                showMsg(E.Message);
            }
            finally
            {
                RefreshButton(true);
                btnOpenMonitor.Enabled = false;
                btnCloseMonitor.Enabled = true;
            }
        }
        /// <summary>
        /// 关闭实时监控
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCloseMonitor_Click(object sender, EventArgs e)
        {
            RefreshButton(false);
            try
            {
                httpServer.Setup(0, lbtime, recordDataGrid, personDataGrid);
                showMsg("Real-time monitoring is Close!");
            }
            catch (Exception E)
            {
                showMsg(E.Message);
            }
            finally
            {
                RefreshButton(true);
                btnCloseMonitor.Enabled = false;
                btnOpenMonitor.Enabled = true;
            }
        }

        private void btnOpenClose_Click(object sender, EventArgs e)
        {
            OpenCloseDoor("open_close");
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenCloseDoor("open");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            OpenCloseDoor("close");
        }

        public void OpenCloseDoor(string doorCmd)
        {
            RefreshButton(false);
            cmd = "SetDoorStatus";

            DoorStatusCmd doorStatusCmd = new DoorStatusCmd(doorCmd);
            _DeviceCmd<DoorStatusCmd> devDoorStatusCmd = new _DeviceCmd<DoorStatusCmd>(cmd, doorStatusCmd);
            StringBuilder jsonStringBuilder = new StringBuilder(JsonConvert.SerializeObject(devDoorStatusCmd));
            SendData(ref jsonStringBuilder);
            Recive(jsonStringBuilder);
            RefreshButton(true);
        }

        private void btnResetDevice_Click(object sender, EventArgs e)
        {
            RefreshButton(false);
            cmd = "ResetDevice";
            DeviceCmd devCmd = new DeviceCmd(cmd);
            StringBuilder jsonStringBuilder = new StringBuilder(JsonConvert.SerializeObject(devCmd));
            SendData(ref jsonStringBuilder);
            Recive(jsonStringBuilder);
            RefreshButton(true);
        }
    }
}
