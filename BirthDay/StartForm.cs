using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Diagnostics;


namespace BIRTHDAY
{
    delegate void Invoker();
    public partial class StartForm : Form
    {
        ListBox[] allListBoxes;
        List<Control> allMaskedTextBox;
        List<Control> allLabel;

        public StartForm()
        {
            InitializeComponent();

            SetControlsProperties();

            BindingData();

            string message = DataLayer.GetTodayBirthDay();
            if(!String.IsNullOrEmpty(message))
                MessageBox.Show(message, "Сегодня День рождения у...");

            message = String.Empty;
            message = DataLayer.GetEarlyBirthDay();
            if (!String.IsNullOrEmpty(message))
                MessageBox.Show(message, "Скоро День рождения у...");
        }


        private void BindingData()
        {
            string[] peoplesName = DataLayer.GetPeopleName();
            Array.Sort(peoplesName);

            for (int i = 0; i < peoplesName.Length; i++)
                peoplesName[i] = String.Format("{0:D3}     {1}", (i + 1), peoplesName[i]);

            foreach (ListBox lb in this.allListBoxes)
            {
                lb.BeginUpdate();
                    lb.Items.Clear();
                    lb.Items.AddRange(peoplesName);
                lb.EndUpdate();
            }
        }

        private void StartForm_Load(object sender, EventArgs e)
        {
            FileSystemWatcher cop = new FileSystemWatcher(@"..\..\DESCRIPTION");
            cop.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.LastAccess |
                NotifyFilters.FileName | NotifyFilters.DirectoryName;

            cop.Changed += new FileSystemEventHandler(PersonFileChanged);
            cop.Created += new FileSystemEventHandler(PersonFileChanged);
            cop.Deleted += new FileSystemEventHandler(PersonFileChanged);

            cop.EnableRaisingEvents = true;
        }

        void PersonFileChanged(object sender, FileSystemEventArgs e)
        {
            MessageBox.Show("Некоторые файла/директории были изменены/удалены и т.д.");

            Invoker invoker = new Invoker(BindingData);
            Invoke(invoker);
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void StartForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
                this.Hide();
        }

        private void развернутьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void свернутьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
                this.Hide();
        }

        private void выйтиToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();//Application.Exit();
        }

        private void Add_AutoRun_btn_Click(object sender, EventArgs e)
        {
            Microsoft.Win32.RegistryKey regKey =
                Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

            regKey.SetValue("Birthday", Application.ExecutablePath);
            //Application.SturtupPath - путь к папке исполняемого файла

            if (MessageBox.Show("Для занесения приложения в автозагрузку следует перзагрузить компьютер. \n Перзагрузить сейчас?", 
                "Birthday - автозагрузка", MessageBoxButtons.OKCancel) == DialogResult.OK)
                RestartOS();
        }

        private void RejAutoRun_btn_Click(object sender, EventArgs e)
        {
            Microsoft.Win32.RegistryKey regKey =
                Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

            if (regKey.GetValue("Birthday") != null)
            {
                regKey.DeleteValue("Birthday");

                if (MessageBox.Show("Для удаления приложения из автозагрузки следует перзагрузить компьютер. \n Перзагрузить сейчас?",
                "Birthday - автозагрузка", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    RestartOS();
            }
        }

        private void RestartOS()
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "cmd.exe";
            proc.StartInfo.Arguments = "/r shurtdown -r -t 2";

            proc.Start();

            /*
            ProcessStartInfo psi = 
                new ProcessStartInfo("cmd", "/r shurtdown -r -t 2") { CreateNoWindow = true };
            Process.Start(psi);*/

            //Process.Start("cmd.exe", "/r shurtdown -r -t 2");
        }
    }
}
