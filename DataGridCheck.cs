using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DynamicFaceDemo
{
    public class datagridviewCheckboxHeaderCell : DataGridViewColumnHeaderCell
    {
        public delegate void HeaderEventHander(object sender, datagridviewCheckboxHeaderEventArgs e);
        public event HeaderEventHander OnCheckBoxClicked;
        Point checkBoxLocation;
        Size checkBoxSize;
        bool _checked = false;
        Point _cellLocation = new Point();
        System.Windows.Forms.VisualStyles.CheckBoxState _cbState =
            System.Windows.Forms.VisualStyles.CheckBoxState.UncheckedNormal;

    
        protected override void Paint(System.Drawing.Graphics graphics,
           System.Drawing.Rectangle clipBounds,
           System.Drawing.Rectangle cellBounds,
           int rowIndex,
           DataGridViewElementStates dataGridViewElementState,
           object value,
           object formattedValue,
           string errorText,
           DataGridViewCellStyle cellStyle,
           DataGridViewAdvancedBorderStyle advancedBorderStyle,
           DataGridViewPaintParts paintParts)
        {
            base.Paint(graphics, clipBounds, cellBounds, rowIndex,
                dataGridViewElementState, value,
                formattedValue, errorText, cellStyle,
                advancedBorderStyle, paintParts);
            Point p = new Point();
            Size s = CheckBoxRenderer.GetGlyphSize(graphics,
            System.Windows.Forms.VisualStyles.CheckBoxState.UncheckedNormal);
            p.X = cellBounds.Location.X +
                10 - (s.Width / 2) - 1;
            p.Y = cellBounds.Location.Y +
                (cellBounds.Height / 2) - (s.Height / 2);
            _cellLocation = cellBounds.Location;
            checkBoxLocation = p;
            checkBoxSize = s;
            if (_checked)
                _cbState = System.Windows.Forms.VisualStyles.
                    CheckBoxState.CheckedNormal;
            else
                _cbState = System.Windows.Forms.VisualStyles.
                    CheckBoxState.UncheckedNormal;
            CheckBoxRenderer.DrawCheckBox
            (graphics, checkBoxLocation, _cbState);
        }

   
        protected override void OnMouseClick(DataGridViewCellMouseEventArgs e)
        {
            Point p = new Point(e.X + _cellLocation.X, e.Y + _cellLocation.Y);
            if (p.X >= checkBoxLocation.X && p.X <=
                checkBoxLocation.X + checkBoxSize.Width
            && p.Y >= checkBoxLocation.Y && p.Y <=
                checkBoxLocation.Y + checkBoxSize.Height)
            {
                _checked = !_checked;
               
                datagridviewCheckboxHeaderEventArgs ex = new datagridviewCheckboxHeaderEventArgs();
                ex.CheckedState = _checked;

                object sender = new object();

                if (OnCheckBoxClicked != null)
                {
                    OnCheckBoxClicked(sender, ex);
                    this.DataGridView.InvalidateCell(this);
                }
            }
            base.OnMouseClick(e);
        }
    }

    public class datagridviewCheckboxHeaderEventArgs : EventArgs
    {
        private bool checkedState = false;
        public bool CheckedState
        {
            get { return checkedState; }
            set { checkedState = value; }
        }
    }
}
