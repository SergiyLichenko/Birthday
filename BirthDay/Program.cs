using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace BIRTHDAY
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string paramethers = String.Empty;
            if (args != null && args.Length > 0)
            {
                foreach (string par in args)
                    paramethers += par + " ";

                MessageBox.Show(paramethers);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new StartForm());
        }
    }
}
