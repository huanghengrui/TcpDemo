using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace DynamicFaceDemo
{
    public partial class frmBaseForm : Form
    {
        public AccessOle Ole = new AccessOle();
        public frmBaseForm()
        {
            InitializeComponent();
        }
        [DllImport("user32", EntryPoint = "GetWindowLong")]
        public static extern int GetWindowLong(IntPtr hwnd, int nIndex);
        [DllImport("user32", EntryPoint = "SetWindowLong")]
        public static extern int SetWindowLong(IntPtr hwnd, int nIndex, int dwNewLong);
        private const int GWL_STYLE = (-16);
        private const int ES_NUMBER = 0x2000;
        /// <summary>
        /// 设置只允许输入数字
        /// </summary>
        /// <param name="txtBox"></param>
        public void SetTextboxNumber(TextBox txtBox)
        {
            int CurrentStyle = GetWindowLong(txtBox.Handle, GWL_STYLE);
            CurrentStyle = CurrentStyle | ES_NUMBER;
            SetWindowLong(txtBox.Handle, GWL_STYLE, CurrentStyle);
            txtBox.ImeMode = ImeMode.Disable;
        }
        /// <summary>
        /// 设置只允许输入数字
        /// </summary>
        /// <param name="txtBox"></param>
        public void SetcomboboxNumber(ComboBox txtBox)
        {
            int CurrentStyle = GetWindowLong(txtBox.Handle, GWL_STYLE);
            CurrentStyle = CurrentStyle | ES_NUMBER;
            SetWindowLong(txtBox.Handle, GWL_STYLE, CurrentStyle);
            txtBox.ImeMode = ImeMode.Disable;
        }
        /// <summary>
        /// 添加DataGridView的列
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="colType"></param>
        /// <param name="Field"></param>
        /// <param name="HeText"></param>
        /// <param name="IsHide"></param>
        /// <param name="HasSort"></param>
        /// <param name="CenterFlag"></param>
        /// <param name="colWidth"></param>
        public void AddColumn(DataGridView grid, int colType, string Field, string HeText, bool IsHide, bool HasSort,
     byte CenterFlag, int colWidth)
        {
            DataGridViewTextBoxColumn colText;
            DataGridViewCheckBoxColumn colCheck;
            DataGridViewComboBoxColumn colCombo;
            switch (colType)
            {
                case 0:
                    colText = new DataGridViewTextBoxColumn();
                    colText.DataPropertyName = Field;
                    colText.HeaderText = HeText;
                    colText.Visible = !IsHide;
                    if (!HasSort) colText.SortMode = DataGridViewColumnSortMode.NotSortable;
                    colText.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    if (CenterFlag == 1)
                        colText.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    else if (CenterFlag == 2)
                        colText.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    if (colWidth > 0)
                        colText.Width = colWidth;
                    else
                        colText.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    grid.Columns.Add(colText);
                    break;
                case 1:
                    colCheck = new DataGridViewCheckBoxColumn();
                    colCheck.DataPropertyName = Field;
                    colCheck.HeaderText = HeText;
                    colCheck.Visible = !IsHide;
                    colCheck.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    if (CenterFlag == 1)
                        colCheck.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    else if (CenterFlag == 2)
                        colCheck.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    if (colWidth > 0)
                        colCheck.Width = colWidth;
                    else
                        colCheck.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    grid.Columns.Add(colCheck);
                    break;
                case 2:
                    colCombo = new DataGridViewComboBoxColumn();
                    colCombo.DataPropertyName = Field;
                    colCombo.HeaderText = HeText;
                    colCombo.Visible = !IsHide;
                    colCombo.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    if (CenterFlag == 1)
                        colCombo.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    else if (CenterFlag == 2)
                        colCombo.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    if (colWidth > 0)
                        colCombo.Width = colWidth;
                    else
                        colCombo.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    colCombo.DisplayStyleForCurrentCellOnly = true;
                    grid.Columns.Add(colCombo);
                    break;
                case 3:
                    colCheck = new DataGridViewCheckBoxColumn();
                    colCheck.DataPropertyName = Field;

                    colCheck.Visible = !IsHide;
                    colCheck.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

                    if (CenterFlag == 1)
                        colCheck.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    else if (CenterFlag == 2)
                        colCheck.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                    if (colWidth > 0)
                        colCheck.Width = colWidth;

                    else
                        colCheck.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                    datagridviewCheckboxHeaderCell ch = new datagridviewCheckboxHeaderCell();
                    ch.OnCheckBoxClicked += new datagridviewCheckboxHeaderCell.HeaderEventHander(ch_OnCheckBoxClicked);
                    colCheck.HeaderCell = ch;
                    colCheck.HeaderText = HeText;
                    grid.Columns.Add(colCheck);
                    break;
            }
        }
        /// <summary>
        /// 多选按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void ch_OnCheckBoxClicked(object sender, datagridviewCheckboxHeaderEventArgs e)
        {

        }
        /// <summary>
        /// 选择数据
        /// </summary>
        /// <param name="bindingSource"></param>
        /// <param name="State"></param>
        protected virtual void SelectData(BindingSource bindingSource, bool State)
        {
            if (bindingSource.DataSource != null)
            {
                DataTable dt = (DataTable)bindingSource.DataSource;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dt.Rows[i].BeginEdit();
                    dt.Rows[i]["select"] = State;
                    dt.Rows[i].EndEdit();
                }
            }
        }
        /// <summary>
        /// 选择数据
        /// </summary>
        /// <param name="bindingSource"></param>
        /// <param name="State"></param>
        protected virtual void SelectData(DataGridView dt, bool State)
        {
            if(dt!=null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dt[0,i].Value = State; 
                }
            }
        }
        /// <summary>
        /// 选择单个数据
        /// </summary>
        /// <param name="bindingSource"></param>
        /// <param name="index"></param>
        /// <param name="State"></param>
        protected virtual void SelectOneData(BindingSource bindingSource, int index, bool State)
        {
            if (bindingSource.DataSource != null)
            {
                DataTable dt = (DataTable)bindingSource.DataSource;
                dt.Rows[index].BeginEdit();
                dt.Rows[index]["select"] = State;
                dt.Rows[index].EndEdit();
            }
        }
        /// <summary>
        /// 选择单个数据
        /// </summary>
        /// <param name="bindingSource"></param>
        /// <param name="index"></param>
        /// <param name="State"></param>
        protected virtual void SelectOneData(DataGridView dt, int index, bool State)
        {
            if (dt != null)
            {
                dt[0, index].Value = State;
            }
        }
        /// <summary>
        /// 获取本机IP
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIP()
        {
            try
            {
                string HostName = Dns.GetHostName();
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        string ip = "";
                        ip = IpEntry.AddressList[i].ToString();
                        return IpEntry.AddressList[i].ToString();
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public struct GeneralUser
        {
            public const string id = "0";
            public const string name = "GeneralUser";
        }

        public struct Administrator
        {
            public const string id = "1";
            public const string name = "Administrator";
        }

        public string GetGeneralUserId(string name)
        {
            string id = "0";
            switch (name)
            {
                case Administrator.name:
                    id = Administrator.id;
                    break;
                case GeneralUser.name:
                    id = GeneralUser.id;
                    break;
            }
            return id;
        }
        /// <summary>
        /// 更新数据库人员资料里面的指纹、人脸、掌纹等数目
        /// </summary>
        protected void UpdateDataCount()
        {
            int fpsCount = 0;
            int faceCount = 0;
            int palmCount = 0;
            string userid = "";
            string sql = "";
            string fpsStr = "";
            DataTableReader dr = null;

            try
            {
                sql = "select * from personlist";
                dr = Ole.GetDataReader(sql);
                while(dr.Read())
                {
                    fpsCount = 0;
                    faceCount = 0;
                    palmCount = 0;
                    userid = dr["user_id"].ToString();

                    for(int i = 0;i < 10; i++)
                    {
                        fpsStr = "fps0" + i;
                        if (!string.IsNullOrEmpty(dr[fpsStr].ToString()))
                        {
                            fpsCount++;
                        }
                    }
                    if (!string.IsNullOrEmpty(dr["face00"].ToString()))
                    {
                        faceCount++;
                    }
                    if (!string.IsNullOrEmpty(dr["palm00"].ToString()))
                    {
                        palmCount++;
                    }

                    sql = "update personlist set [fps]='"+ fpsCount +"', [face]='"+ faceCount +"', [palm]='"+ palmCount +"' where [user_id]='"+ userid +"'";
                    Ole.ExecSQL(sql);
                }
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
        }
     
       
       
        /// <summary>
        /// 转换时间格式
        /// </summary>
        /// <param name="type"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool IsDateTime(string type, ref string time)
        {
            try
            {
                time = Convert.ToDateTime(time).ToString(type);

            }
            catch
            {
                time = null;
                return false;
            }
            return true;
        }
        /// <summary>
        /// byte[]转16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ByteToHexString(byte[] bytes)
        {
            string hexString = "";
            if(bytes!=null)
            {
                StringBuilder strB = new StringBuilder();
                for(int i=0;i<bytes.Length;i++)
                {
                    strB.Append(bytes[i].ToString("X2")+" ");
                }
                hexString = strB.ToString();
            }
            return hexString;
        }

        public static byte[] strToHexByte(string hexString)
        {
            hexString = hexString.Replace("-", "");
            if (hexString.Length % 2 != 0)
            {
                hexString += "20";
            }
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return returnBytes;
        }

        /// <summary>
        /// 计算发送的数据的大小
        /// </summary>
        /// <param name="setUsers"></param>
        /// <returns></returns>
        public int CalculatedLength(SetUsers setUsers)
        {
            int ret = 300;  //约包含一个int/两个byte/标点符号/名称
            if (setUsers.card != null)
            {
                ret += setUsers.card.Length;
            }
            if (setUsers.face != null)
            {
                ret += setUsers.face.Length;
            }
            if (setUsers.name != null)
            {
                ret += setUsers.name.Length;
            }
            if (setUsers.palm != null)
            {
                ret += setUsers.palm.Length;
            }
            if (setUsers.photo != null)
            {
                ret += setUsers.photo.Length;
            }
            if (setUsers.pwd != null)
            {
                ret += setUsers.pwd.Length;
            }
            if (setUsers.userId != null)
            {
                ret += setUsers.userId.Length;
            }
            if (setUsers.vaildEnd != null)
            {
                ret += setUsers.vaildEnd.Length;
            }
            if (setUsers.vaildStart != null)
            {
                ret += setUsers.vaildStart.Length;
            }
            if (setUsers.fps != null)
            {
                for (int i = 0; i < setUsers.fps.Count; i++)
                {
                    if(setUsers.fps[i] != null)
                        ret += setUsers.fps[i].Length;
                }
            }
            return ret;
        }
    }

    /// <summary>
    /// 错误代码
    /// </summary>
    public class ResultCode
    {
        public const int EQUIPMENT_CAPACITY_FULL = -5;
        public const int COMMUNICATION_ERROR = -4;
        public const int TIMEOUT_ERROR = -3;
        public const int PARAMETER_ERROR = -2;
        public const int ERROR = -1;
        public const int SUCCESS = 0;
        public const int EQUIPMENT_BUSY = 1;
        public const int DURING_EQUIPMENT_OPERATION= 2;
        public const int EQUIPMENT_WORK_COMPLETED = 3;
    }
    public class LanguageType
    {
        private string _id;
        private string _name;

        public LanguageType(string id, string name)
        {
            _id = id;
            _name = name;
        }

        public string id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string name
        {
            get { return _name; }
            set { _name = value; }
        }

        public override string ToString()
        {
            return _name;
        }
    }
    public class VerifyModeType
    {
        private string _id;
        private string _name;

        public VerifyModeType(string id, string name)
        {
            _id = id;
            _name = name;
        }

        public string id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string name
        {
            get { return _name; }
            set { _name = value; }
        }

        public override string ToString()
        {
            return _name;
        }
    }
}
