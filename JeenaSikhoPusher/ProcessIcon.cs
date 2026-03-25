using System;
using System.Diagnostics;
using System.Windows.Forms;
using JeenaSikhoPusher.Properties;
using System.Drawing;
using System.IO;

namespace JeenaSikhoPusher
{
	/// <summary>
	/// 
	/// </summary>
	class ProcessIcon : IDisposable
    {
        private Icon[] AnimIconList;
		/// <summary>
		/// The NotifyIcon object.
		/// </summary>
        NotifyIcon ni;
        Timer timer1 = new Timer();
        bool tick = true;
    	/// <summary>
		/// Initializes a new instance of the <see cref="ProcessIcon"/> class.
		/// </summary>
		public ProcessIcon()
		{
            try
            {
                //Instantiate the NotifyIcon object.
                ni=new NotifyIcon();
                object[] objValues = new object[2];
                objValues[0] = Convert.ToString("report_sync_1.ico");
                objValues[1] = Convert.ToString("report_sync_2.ico");
                SetIconRange(objValues);
                timer1.Tick += new EventHandler(timer1_Tick);
                timer1.Start();
            }
            catch (Exception ex) {  }
		}

        public void SetIconRange(object[] IconList)
        {
            System.Type tp = IconList[0].GetType();
            if (tp.Name == "String")
            {
                AnimIconList = new Icon[IconList.Length];
                for (int i = 0; i < IconList.Length; ++i)
                {
                    AnimIconList[i] = new Icon(IconList[i].ToString());
                }

            }
            if (tp.Name == "Icon")
            {
                AnimIconList = new Icon[IconList.Length];
                for (int i = 0; i < IconList.Length; ++i)
                {
                    AnimIconList[i] = (Icon)IconList[i];
                }
            }
            if(IconList.Length > 0)
            ni.Icon = AnimIconList[0];
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Interval =300;
            if (tick)
            {

                ni.Icon = AnimIconList[0];
                tick = false;
            }
            else
            {
                ni.Icon = AnimIconList[1];
                tick = true;
            }

        }
        /// <summary>
		/// Displays the icon in the system tray.
		/// </summary>
		public void Display()
		{
			// Put the icon in the system tray and allow it react to mouse clicks.			
			ni.MouseClick += new MouseEventHandler(ni_MouseClick);
            ni.Icon = new Icon(Path.Combine(Application.StartupPath, "report_sync_1.ico"));
            ni.Text = "Jeena Sikho Patient Data Send.";
			ni.Visible = true;

			// Attach a context menu.
			ni.ContextMenuStrip = new ContextMenus().Create();
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		public void Dispose()
		{
			// When the application closes, this will remove the icon from the system tray immediately.
			ni.Dispose();
		}

		/// <summary>
		/// Handles the MouseClick event of the ni control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
		void ni_MouseClick(object sender, MouseEventArgs e)
		{
			// Handle mouse button clicks.
			if (e.Button == MouseButtons.Left)
			{
				//Start Windows Explorer.
				//Process.Start("explorer", null);
			}
		}
	}
}