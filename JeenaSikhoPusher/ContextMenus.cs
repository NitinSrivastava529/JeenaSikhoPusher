using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using JeenaSikhoPusher.Properties;
using Microsoft.Win32;
using System.Security.Permissions;
namespace JeenaSikhoPusher
{
    class ContextMenus
    {/// <summary>
        /// Is the About box displayed?
        /// </summary>
        bool isAboutLoaded = false;
        RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        public static List<SyncLog> SyncLogList = new List<SyncLog>();
        public static string lastrun = string.Empty;
        public static string lastmessage = string.Empty;
        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>ContextMenuStrip</returns>
        public ContextMenuStrip Create()
        {
            // Add the default menu options.
        
            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem item;
            ToolStripSeparator sep;

            // Windows Explorer.
            item = new ToolStripMenuItem();
            item.Text = "ENABLE ON BOOT";
            item.Click += new EventHandler(ENABLE_BOOT_Click);
            //item.Image = Resources.Explorer;
            menu.Items.Add(item);

            item = new ToolStripMenuItem();
            item.Text = "DISABLE ON BOOT";
            item.Click += new EventHandler(DISABLE_BOOT_Click);
            //item.Image = Resources.Explorer;
            menu.Items.Add(item);

            // About.
            item = new ToolStripMenuItem();
            item.Text = "Running Status";
            item.Click += new EventHandler(About_Click);
            //item.Image = Resources.About;
            menu.Items.Add(item);

            // Separator.
            sep = new ToolStripSeparator();
            menu.Items.Add(sep);

            // Exit.
            item = new ToolStripMenuItem();
            item.Text = "Exit";
            item.Click += new System.EventHandler(Exit_Click);
            //item.Image = Resources.Exit;
            menu.Items.Add(item);

            return menu;
        }

        /// <summary>
        /// Handles the Click event of the Explorer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Explorer_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }
        void ENABLE_BOOT_Click(object sender, EventArgs e)
        {
            RegistryKey rkey = Registry.LocalMachine;
            RegistryPermission f = new RegistryPermission(RegistryPermissionAccess.Write | RegistryPermissionAccess.Read, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
            /**********************/
            /* set registry keys  */
            /**********************/
            RegistryKey wtaKey = rkey.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run\");
            try
            {
                wtaKey.SetValue("JeenaSikhoPusher", Application.ExecutablePath.ToString());
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
   
        }
        void DISABLE_BOOT_Click(object sender, EventArgs e)
        {
            RegistryKey rkey = Registry.LocalMachine;
            RegistryPermission f = new RegistryPermission(RegistryPermissionAccess.Write | RegistryPermissionAccess.Read, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
            /**********************/
            /* delete registry keys  */
            /**********************/
            RegistryKey wtaKey = rkey.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run\");
            try
            {
                wtaKey.DeleteValue("JeenaSikhoPusher", false);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

        }
        /// <summary>
        /// Handles the Click event of the About control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void About_Click(object sender, EventArgs e)
        {
            if (!isAboutLoaded)
            {
                isAboutLoaded = true;
                new  LogForm().ShowDialog();
                isAboutLoaded = false;
            }
        }

        /// <summary>
        /// Processes a menu item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Exit_Click(object sender, EventArgs e)
        {
            // Quit without further ado.
            Application.Exit();
        }
    }
}
