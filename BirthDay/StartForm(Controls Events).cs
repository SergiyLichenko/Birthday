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
        private void lb_Person_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox lb = (ListBox)sender;

            if (lb.SelectedIndex == -1)
                return;

            string[] paramethers = this.GetSplitName(lb);

            Man man = DataLayer.GetManDetails(paramethers[0], paramethers[1]);

            SplitContainer sc =
                (SplitContainer)(((SplitterPanel)(lb.Parent)).GetContainerControl());

            Control.ControlCollection controlCollection = sc.Panel2.Controls;

            
            List<string> manInfo = new List<string>();
            foreach(string val in man.ManInfo.Values)
                manInfo.Add(val);
            
            int i = 0;
            foreach (Control c in controlCollection)
            {
                if (c.GetType() == typeof(MaskedTextBox))
                    ((MaskedTextBox)c).Text = manInfo[i++];
            }

            RichTextBox rtb = GetChildeControl<RichTextBox>(tc_Person.SelectedTab);
            rtb.Text = man.Description;
        }

        private void btn_create_Click(object sender, EventArgs e)
        {
            Man newMan = new Man();

            RichTextBox rtb = GetChildeControl(tc_Person.SelectedTab, typeof(RichTextBox)) as RichTextBox;
            //List<Control> maskedTextBoxManInfo = new List<Control>();

            //GetAllTypedControls(tc_Person.SelectedTab, maskedTextBoxManInfo, typeof(MaskedTextBox));

            newMan.Name = this.mtb_Name_CreatePerson.Text.Trim();
            newMan.SurName = this.mtb_SurName_CreatePerson.Text.Trim();
            newMan.DateOfBirth = this.mtb_Birthday_CreatePerson.Text.Trim();
            newMan.Category = this.mtb_Relation_CreatePerson.Text.Trim();
            newMan.Phone = this.mtb_Phone_CreatePerson.Text.Trim();
            newMan.MobilePhone = this.mtb_MobPhone_CreatePerson.Text.Trim();
            newMan.Country = this.mtb_Country_CreatePerson.Text.Trim();
            newMan.Sity = this.mtb_Sity_CreatePerson.Text.Trim();
            newMan.Street = this.mtb_Street_CreatePerson.Text.Trim();
            newMan.House = this.mtb_House_CreatePerson.Text.Trim();
            newMan.Description = rtb.Text.Trim();

            string message = null;
            DataLayer.AddPerson(newMan, ref message);
            MessageBox.Show(message);

            BindingData();
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Подтвердите желание удалить данную персону",
                "Delete Person", MessageBoxButtons.YesNoCancel);
            if (dr == DialogResult.Yes)
            {
                /*TabPage tp = tc_Person.SelectedTab;
                Control.ControlCollection labelsAndMasked =
                    ((SplitContainer)((SplitContainer)tp.Controls["sc_DeletePerson"])
                    .Panel1.Controls["sc_DeletePerson_inner"]).Panel2.Controls;*/
                Man newMan = new Man();

                newMan.Name = this.mtb_Name_DeletePerson.Text.Trim();
                newMan.SurName = this.mtb_SurName_DeletePerson.Text.Trim();
                newMan.DateOfBirth = this.mtb_BirthDay_DeletePerson.Text.Trim();
                newMan.Category = this.mtb_Relation_DeletePerson.Text.Trim();
                newMan.Phone = this.mtb_Phone_DeletePerson.Text.Trim();
                newMan.MobilePhone = this.mtb_MobPhone_DeletePerson.Text.Trim();
                newMan.Country = this.mtb_Country_DeletePerson.Text.Trim();
                newMan.Sity = this.mtb_Sity_DeletePerson.Text.Trim();
                newMan.Street = this.mtb_Street_DeletePerson.Text.Trim();
                newMan.House = this.mtb_House_DeletePerson.Text.Trim();

                if (DataLayer.DeletePerson(newMan))
                    MessageBox.Show("Персона удалена из базы данных");
                
                BindingData();
            }
            return;
        }

        private void btn_update_Click(object sender, EventArgs e)
        {
            TabPage tp = tc_Person.SelectedTab;
            ListBox lb = (ListBox)GetChildeControl(tp, typeof(ListBox));

            RichTextBox rtb = GetChildeControl<RichTextBox>(tp);

            string[] name_surName = GetSplitName(lb);

            Man proposedMan = new Man();
            proposedMan.Name = this.mtb_Name_UpdatePerson.Text.Trim();
            proposedMan.SurName = this.mtb_SurName_UpdatePerson.Text.Trim();
            proposedMan.DateOfBirth = this.mtb_BirthDate_UpdatePerson.Text.Trim();
            proposedMan.Category = this.mtb_Relation_UpdatePerson.Text.Trim();
            proposedMan.Phone = this.mtb_Phone_UpdatePerson.Text.Trim();
            proposedMan.MobilePhone = this.mtb_MobPhone_UpdatePerson.Text.Trim();
            proposedMan.Country = this.mtb_Country_UpdatePerson.Text.Trim();
            proposedMan.Sity = this.mtb_Sity_UpdatePerson.Text.Trim();
            proposedMan.Street = this.mtb_Street_UpdatePerson.Text.Trim();
            proposedMan.House = this.mtb_House_UpdatePerson.Text.Trim();
            proposedMan.Description = rtb.Text.Trim();

            string message = 
                DataLayer.UpdatePerson(name_surName[0], name_surName[1], proposedMan);
                
            MessageBox.Show(message);
            BindingData();
        }

        private void tc_Person_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tc_Person.SelectedIndex)
            {
                case 0: ButtonManagement(TabMode.Nothin); break;
                case 1: ButtonManagement(TabMode.Create); break;
                case 2: ButtonManagement(TabMode.Update); break;
                case 3: ButtonManagement(TabMode.Delete); break;
            }
        }

        private void btn_reset_Click(object sender, EventArgs e)
        {
            TabPage tp = tc_Person.SelectedTab;

            ListBox lb = GetChildeControl<ListBox>(tp);
            if (lb != null)
                lb.SelectedIndex = -1;

            List<Control> maskedBox = new List<Control>();

            GetAllTypedControls(tp, maskedBox, typeof(MaskedTextBox));
            if (maskedBox.Count > 0)
                foreach (Control mt in maskedBox)
                    ((MaskedTextBox)mt).Text = string.Empty;
        }

        private void reverse_btn_Click(object sender, EventArgs e)
        {
            ListBox lb = GetChildeControl<ListBox>(tc_Person.SelectedTab);

            ListBox.ObjectCollection items = lb.Items;

            lb.BeginUpdate();
            for (int i = 0, j = items.Count - 1; i < j; i++, j--)
            {
                object tempI = items[i];
                object tempJ = items[j];

                items.RemoveAt(j);
                items.RemoveAt(i);
                
                items.Insert(i, tempJ);
                items.Insert(j, tempI);
            }
            lb.EndUpdate();
        }

        void MaskedTextBox_Leave(object sender, EventArgs e)
        {
            ((MaskedTextBox)sender).BackColor = SystemColors.Info;
        }

        void MaskedTextBox_Click(object sender, EventArgs e)
        {
            ((MaskedTextBox)sender).BackColor = Color.LightGreen;
        }

        void NameSurName_MaskedTextBox_Leave(object sender, EventArgs e)
        {
            btn_create.Enabled = true;
            string name = mtb_Name_CreatePerson.Text;
            string surName = mtb_SurName_CreatePerson.Text;

            for (int i = 0; i < lb_CreatePerson.Items.Count; i++)
            {
                string tempItem = lb_CreatePerson.Items[i].ToString();

                if (
                    (tempItem.Contains(name) && !string.IsNullOrEmpty(name))
                    &&
                    (tempItem.Contains(surName) && !string.IsNullOrEmpty(surName))
                   )
                {
                    lb_CreatePerson.SelectedIndex = i;
                    btn_create.Enabled = false;
                    break;
                }
            }
        }
    }
}
