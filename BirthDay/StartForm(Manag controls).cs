using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;

namespace BIRTHDAY
{
    public partial class StartForm : Form
    {
        private string[] GetSplitName(ListBox lb)
        {
            if (lb.SelectedIndex == -1)
                return new string[2] { "", ""};

            char[] separator = new char[] { (char)09, (char)32 };
            string tempString = lb.SelectedItem.ToString().Trim();//001 Александр Гудок
            return tempString.Substring(4).Split(separator, StringSplitOptions.RemoveEmptyEntries);
        }

        private void GetAllTypedControls(Control parentControl, 
            List<Control> childeControls, Type type)
        { 
            // работаем только с элементами искомого типа
            if (parentControl.GetType() == type)
                childeControls.Add(parentControl);

            // проходим через все элементы рекурсивно,
            // что бы не пропустить элементы, которые находятся в контейнерах
            foreach (Control ctrChild in parentControl.Controls)
                GetAllTypedControls(ctrChild, childeControls, type);
        }

        private Control GetChildeControl(Control parentControl, Type type)
        {
            //работаем только с искомым контролом
            if (parentControl.GetType() == type)
                return parentControl;

            foreach (Control childeControl in parentControl.Controls)
            {
                Control potenResultControl = 
                    GetChildeControl(childeControl, type);

                if (potenResultControl != null)
                    return potenResultControl;
            }

            return null;
        }

        private T GetChildeControl<T>(Control parentControl) where T : Control
        {
            if (parentControl.GetType() == typeof(T))
                return (T)parentControl;
            foreach (Control childeControl in parentControl.Controls)
            {
                var neededControl = GetChildeControl<T>(childeControl);
                if (neededControl != null)
                    return neededControl;
            }
            return null;
        }

        private void SetControlsProperties()
        {
            allListBoxes = new ListBox[4] { this.lb_AllPerson,
                this.lb_CreatePerson, this.lb_DeletePerson,
                this.lb_UpdatePerson};

            allMaskedTextBox = new List<Control>();
            allLabel = new List<Control>();
            GetAllTypedControls(this, allMaskedTextBox, typeof(MaskedTextBox));
            GetAllTypedControls(this, allLabel, typeof(Label));

            foreach (MaskedTextBox mtb in allMaskedTextBox)
            {
                mtb.BackColor = SystemColors.Info;
                mtb.ForeColor = Color.Red;

                mtb.Click += new EventHandler(MaskedTextBox_Click);
                mtb.Leave += new EventHandler(MaskedTextBox_Leave);
            }
            foreach (Control lbl in allLabel)
                ((Label)lbl).ForeColor = SystemColors.GrayText;
        }
        void ButtonManagement(TabMode tm)
        {
            foreach (Control c in this.Controls)
                if (c.GetType() == typeof(Button))
                    c.Enabled = true;
            
            switch (tm)
            {
                case TabMode.Nothin: btn_create.Enabled = btn_delete.Enabled = btn_update.Enabled = false; break;
                case TabMode.Create: btn_create.Enabled = btn_delete.Enabled = false; break;
                case TabMode.Update: btn_update.Enabled = btn_delete.Enabled = false; break;
                case TabMode.Delete: btn_create.Enabled = btn_update.Enabled = false; break;
            }
        }
    }
}
