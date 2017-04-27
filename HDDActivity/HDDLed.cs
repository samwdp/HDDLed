using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.Management.Instrumentation;
using System.Collections.Specialized;
using System.Threading;

namespace HDDActivity
{
    public partial class HDDLed : Form
    {
        NotifyIcon hddLedIcon;
        Icon active;
        Icon inactive;
        Thread hddLedWorker;

        public HDDLed()
        {

            active = new Icon("active.ico");
            inactive = new Icon("inactive.ico");
            hddLedIcon = new NotifyIcon();
            hddLedIcon.Icon = inactive;
            hddLedIcon.Visible = true;

            MenuItem quitMenuItem = new MenuItem("Quit");
            ContextMenu contextMenu = new ContextMenu();
            contextMenu.MenuItems.Add(quitMenuItem);

            hddLedIcon.ContextMenu = contextMenu;

            quitMenuItem.Click += QuitMenuItem_Click;

            InitializeComponent();
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;

            hddLedWorker = new Thread(new ThreadStart(HDDLedThread));
            hddLedWorker.Start();

        }

        private void QuitMenuItem_Click(object sender, EventArgs e)
        {
            hddLedWorker.Abort();
            hddLedIcon.Dispose();
            this.Close();
        }

        public void HDDLedThread()
        {
            ManagementClass driveDataClass = new ManagementClass("Win32_PerfFormattedData_PerfDisk_PhysicalDisk");

            try
            {

                while (true)
                {
                    ManagementObjectCollection driveCollection = driveDataClass.GetInstances();
                    foreach(ManagementObject obj in driveCollection)
                    {
                        if(obj["Name"].ToString() == "_Total")
                        {
                            if(Convert.ToUInt64(obj["DiskBytesPersec"]) > 0)
                            {
                                hddLedIcon.Icon = active;
                            }
                            else
                            {
                                hddLedIcon.Icon = inactive;
                            }
                        }
                    }

                    Thread.Sleep(100);
                }
            }catch (ThreadAbortException tae)
            {
                driveDataClass.Dispose();
            }
            
        }
    }
}
