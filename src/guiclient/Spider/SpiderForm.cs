using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;

using System.IO;

namespace Spider
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class frmSpider : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.TabPage tabPageId;
		private System.Windows.Forms.TabPage tabPageCreate;
		private System.Windows.Forms.TabPage tabPageManage;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnIdFile;
		private System.Windows.Forms.Button btnChangeId;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btnCreateTask;
		private System.Windows.Forms.TextBox txtTaskNameCreate;
		private System.Windows.Forms.TextBox txtUrl;
		private System.Windows.Forms.TextBox txtDeepth;
		private System.Windows.Forms.TextBox txtTaskManage;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button btnStatus;
		private System.Windows.Forms.Button btnResult;
		private System.Windows.Forms.Button btnDelete;
		public System.Windows.Forms.ListBox lstTasks;
		private System.Windows.Forms.Button btnList;
		private System.Windows.Forms.MenuItem mnuExit;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem mnuAbout;
		private System.Windows.Forms.TextBox txtStatus;
		private System.Windows.Forms.TextBox txtId;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.CheckBox chkSiteOnly;
		private System.Windows.Forms.Button btnView;

		private SClient client;

		public frmSpider()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			WaitDialog wd = new WaitDialog();	
			try 
			{
				wd.Show();
				wd.Refresh();
				client = new SClient(this);
				Directory.CreateDirectory(SClient.IdDir);
				Directory.CreateDirectory(SClient.ResultDir);
				//AppendStatusLine("Id-s dir: " + SClient.IdDir);
				//AppendStatusLine("Results-s dir: " + SClient.ResultDir);
				txtId.Text = client.Connect();
			} 
			finally {
				wd.Close();
				wd = null;
			}			
			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmSpider));
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPageId = new System.Windows.Forms.TabPage();
			this.btnChangeId = new System.Windows.Forms.Button();
			this.btnIdFile = new System.Windows.Forms.Button();
			this.txtId = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.tabPageCreate = new System.Windows.Forms.TabPage();
			this.txtDeepth = new System.Windows.Forms.TextBox();
			this.txtUrl = new System.Windows.Forms.TextBox();
			this.txtTaskNameCreate = new System.Windows.Forms.TextBox();
			this.btnCreateTask = new System.Windows.Forms.Button();
			this.chkSiteOnly = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.tabPageManage = new System.Windows.Forms.TabPage();
			this.btnList = new System.Windows.Forms.Button();
			this.lstTasks = new System.Windows.Forms.ListBox();
			this.btnDelete = new System.Windows.Forms.Button();
			this.btnResult = new System.Windows.Forms.Button();
			this.btnStatus = new System.Windows.Forms.Button();
			this.txtTaskManage = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.mainMenu = new System.Windows.Forms.MainMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.mnuExit = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.mnuAbout = new System.Windows.Forms.MenuItem();
			this.txtStatus = new System.Windows.Forms.TextBox();
			this.btnView = new System.Windows.Forms.Button();
			this.tabControl1.SuspendLayout();
			this.tabPageId.SuspendLayout();
			this.tabPageCreate.SuspendLayout();
			this.tabPageManage.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.tabControl1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					  this.tabPageId,
																					  this.tabPageCreate,
																					  this.tabPageManage});
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(440, 144);
			this.tabControl1.TabIndex = 0;
			// 
			// tabPageId
			// 
			this.tabPageId.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.btnChangeId,
																					this.btnIdFile,
																					this.txtId,
																					this.label1});
			this.tabPageId.Location = new System.Drawing.Point(4, 22);
			this.tabPageId.Name = "tabPageId";
			this.tabPageId.Size = new System.Drawing.Size(432, 118);
			this.tabPageId.TabIndex = 0;
			this.tabPageId.Text = "Client ID";
			// 
			// btnChangeId
			// 
			this.btnChangeId.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.btnChangeId.Location = new System.Drawing.Point(8, 88);
			this.btnChangeId.Name = "btnChangeId";
			this.btnChangeId.Size = new System.Drawing.Size(64, 24);
			this.btnChangeId.TabIndex = 3;
			this.btnChangeId.Text = "Change";
			this.btnChangeId.Click += new System.EventHandler(this.btnChangeId_Click);
			// 
			// btnIdFile
			// 
			this.btnIdFile.Location = new System.Drawing.Point(248, 8);
			this.btnIdFile.Name = "btnIdFile";
			this.btnIdFile.Size = new System.Drawing.Size(64, 24);
			this.btnIdFile.TabIndex = 2;
			this.btnIdFile.Text = "Id File";
			this.btnIdFile.Click += new System.EventHandler(this.btnIdFile_Click);
			// 
			// txtId
			// 
			this.txtId.Location = new System.Drawing.Point(64, 8);
			this.txtId.Name = "txtId";
			this.txtId.Size = new System.Drawing.Size(176, 20);
			this.txtId.TabIndex = 1;
			this.txtId.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(24, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "Id:";
			// 
			// tabPageCreate
			// 
			this.tabPageCreate.Controls.AddRange(new System.Windows.Forms.Control[] {
																						this.txtDeepth,
																						this.txtUrl,
																						this.txtTaskNameCreate,
																						this.btnCreateTask,
																						this.chkSiteOnly,
																						this.label4,
																						this.label3,
																						this.label2});
			this.tabPageCreate.Location = new System.Drawing.Point(4, 22);
			this.tabPageCreate.Name = "tabPageCreate";
			this.tabPageCreate.Size = new System.Drawing.Size(432, 118);
			this.tabPageCreate.TabIndex = 1;
			this.tabPageCreate.Text = "Create Task";
			// 
			// txtDeepth
			// 
			this.txtDeepth.Location = new System.Drawing.Point(64, 56);
			this.txtDeepth.Name = "txtDeepth";
			this.txtDeepth.Size = new System.Drawing.Size(40, 20);
			this.txtDeepth.TabIndex = 7;
			this.txtDeepth.Text = "1";
			this.txtDeepth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// txtUrl
			// 
			this.txtUrl.Location = new System.Drawing.Point(64, 32);
			this.txtUrl.Name = "txtUrl";
			this.txtUrl.Size = new System.Drawing.Size(344, 20);
			this.txtUrl.TabIndex = 6;
			this.txtUrl.Text = "http://";
			// 
			// txtTaskNameCreate
			// 
			this.txtTaskNameCreate.Location = new System.Drawing.Point(64, 8);
			this.txtTaskNameCreate.Name = "txtTaskNameCreate";
			this.txtTaskNameCreate.Size = new System.Drawing.Size(176, 20);
			this.txtTaskNameCreate.TabIndex = 5;
			this.txtTaskNameCreate.Text = "task";
			// 
			// btnCreateTask
			// 
			this.btnCreateTask.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.btnCreateTask.Location = new System.Drawing.Point(8, 88);
			this.btnCreateTask.Name = "btnCreateTask";
			this.btnCreateTask.Size = new System.Drawing.Size(64, 24);
			this.btnCreateTask.TabIndex = 4;
			this.btnCreateTask.Text = "Create";
			this.btnCreateTask.Click += new System.EventHandler(this.btnCreateTask_Click);
			// 
			// chkSiteOnly
			// 
			this.chkSiteOnly.Checked = true;
			this.chkSiteOnly.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkSiteOnly.Location = new System.Drawing.Point(224, 56);
			this.chkSiteOnly.Name = "chkSiteOnly";
			this.chkSiteOnly.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.chkSiteOnly.Size = new System.Drawing.Size(184, 24);
			this.chkSiteOnly.TabIndex = 3;
			this.chkSiteOnly.Text = "Follow links inside this site only";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(8, 56);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(80, 16);
			this.label4.TabIndex = 2;
			this.label4.Text = "Deepth:";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 32);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(48, 16);
			this.label3.TabIndex = 1;
			this.label3.Text = "Url:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 8);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(40, 16);
			this.label2.TabIndex = 0;
			this.label2.Text = "Name:";
			// 
			// tabPageManage
			// 
			this.tabPageManage.Controls.AddRange(new System.Windows.Forms.Control[] {
																						this.btnView,
																						this.btnList,
																						this.lstTasks,
																						this.btnDelete,
																						this.btnResult,
																						this.btnStatus,
																						this.txtTaskManage,
																						this.label5});
			this.tabPageManage.Location = new System.Drawing.Point(4, 22);
			this.tabPageManage.Name = "tabPageManage";
			this.tabPageManage.Size = new System.Drawing.Size(432, 118);
			this.tabPageManage.TabIndex = 2;
			this.tabPageManage.Text = "Manage Task";
			// 
			// btnList
			// 
			this.btnList.Location = new System.Drawing.Point(248, 8);
			this.btnList.Name = "btnList";
			this.btnList.Size = new System.Drawing.Size(64, 24);
			this.btnList.TabIndex = 12;
			this.btnList.Text = "List Tasks";
			this.btnList.Click += new System.EventHandler(this.btnList_Click);
			// 
			// lstTasks
			// 
			this.lstTasks.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.lstTasks.Items.AddRange(new object[] {
														  "click List Tasks to fill this list"});
			this.lstTasks.Location = new System.Drawing.Point(8, 40);
			this.lstTasks.Name = "lstTasks";
			this.lstTasks.Size = new System.Drawing.Size(304, 69);
			this.lstTasks.TabIndex = 11;
			this.lstTasks.SelectedIndexChanged += new System.EventHandler(this.lstTasks_SelectedIndexChanged);
			// 
			// btnDelete
			// 
			this.btnDelete.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			this.btnDelete.Location = new System.Drawing.Point(336, 88);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Size = new System.Drawing.Size(88, 24);
			this.btnDelete.TabIndex = 10;
			this.btnDelete.Text = "Delete Task";
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			// 
			// btnResult
			// 
			this.btnResult.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			this.btnResult.Location = new System.Drawing.Point(336, 40);
			this.btnResult.Name = "btnResult";
			this.btnResult.Size = new System.Drawing.Size(88, 24);
			this.btnResult.TabIndex = 9;
			this.btnResult.Text = "Get Result";
			this.btnResult.Click += new System.EventHandler(this.btnResult_Click);
			// 
			// btnStatus
			// 
			this.btnStatus.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			this.btnStatus.Location = new System.Drawing.Point(336, 16);
			this.btnStatus.Name = "btnStatus";
			this.btnStatus.Size = new System.Drawing.Size(88, 24);
			this.btnStatus.TabIndex = 8;
			this.btnStatus.Text = "Current Status";
			this.btnStatus.Click += new System.EventHandler(this.btnStatus_Click);
			// 
			// txtTaskManage
			// 
			this.txtTaskManage.Location = new System.Drawing.Point(64, 8);
			this.txtTaskManage.Name = "txtTaskManage";
			this.txtTaskManage.Size = new System.Drawing.Size(176, 20);
			this.txtTaskManage.TabIndex = 7;
			this.txtTaskManage.Text = "task";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(8, 8);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(40, 16);
			this.label5.TabIndex = 6;
			this.label5.Text = "Name:";
			// 
			// mainMenu
			// 
			this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.menuItem1,
																					 this.menuItem3});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.mnuExit});
			this.menuItem1.Text = "&File";
			// 
			// mnuExit
			// 
			this.mnuExit.Index = 0;
			this.mnuExit.Text = "E&xit";
			this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 1;
			this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.mnuAbout});
			this.menuItem3.Text = "&Help";
			// 
			// mnuAbout
			// 
			this.mnuAbout.Index = 0;
			this.mnuAbout.Text = "&About";
			this.mnuAbout.Click += new System.EventHandler(this.mnuAbout_Click);
			// 
			// txtStatus
			// 
			this.txtStatus.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.txtStatus.Location = new System.Drawing.Point(0, 152);
			this.txtStatus.Multiline = true;
			this.txtStatus.Name = "txtStatus";
			this.txtStatus.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtStatus.Size = new System.Drawing.Size(440, 120);
			this.txtStatus.TabIndex = 1;
			this.txtStatus.Text = "";
			// 
			// btnView
			// 
			this.btnView.Location = new System.Drawing.Point(336, 64);
			this.btnView.Name = "btnView";
			this.btnView.Size = new System.Drawing.Size(88, 24);
			this.btnView.TabIndex = 13;
			this.btnView.Text = "View Result";
			this.btnView.Click += new System.EventHandler(this.btnView_Click);
			// 
			// frmSpider
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(440, 273);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.txtStatus,
																		  this.tabControl1});
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Menu = this.mainMenu;
			this.Name = "frmSpider";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Spider Client";
			this.tabControl1.ResumeLayout(false);
			this.tabPageId.ResumeLayout(false);
			this.tabPageCreate.ResumeLayout(false);
			this.tabPageManage.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			
			try 
			{
				frmSpider sp = new frmSpider();
				Application.Run(sp);
			} 
			catch(Exception e1){
				ShowError(e1.Message);
			}
		}

		private void mnuExit_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void btnIdFile_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.InitialDirectory = SClient.IdDir;
			ofd.Filter = "Id files (*.txt)|*.txt|All files (*.*)|*.*";
			if (ofd.ShowDialog () == DialogResult.OK) 
			{
				String FileName = ofd.FileName;
				if (FileName.Length != 0) 
				{
					StreamReader id = null;
					try 
					{
						id = new StreamReader(FileName);
						txtId.Text = id.ReadLine();
					}
					catch (Exception e1) 
					{
						ShowError(e1.Message);
					} 
					finally {
						if(id != null) id.Close();
					}
				}
			}
			ofd.Dispose ();
		}

		private void ViewResultFile(){
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.InitialDirectory = SClient.ResultDir;
			ofd.Filter = "Result files (*.txt)|*.txt|Temporary result files (*.tmp)|*.tmp|All files (*.*)|*.*";
			if (ofd.ShowDialog () == DialogResult.OK) 
			{
				String FileName = ofd.FileName;
				if (FileName.Length != 0) 
				{
						StreamReader reader = null;
						try
						{
							reader = new StreamReader(FileName);
							string data = reader.ReadToEnd();
							reader.Close();
							reader = null;
							ResultForm rs = new ResultForm();
							rs.Text = rs.Text + " - " + FileName;
							rs.SetText(data);
							rs.Show();
							rs.Refresh();
						} 
						catch (Exception e)
						{
							ShowError(e.Message);
						} 
						finally 
						{
							if(reader != null) reader.Close();
						}
					}
				}
			
			ofd.Dispose ();
		}

		private void btnChangeId_Click(object sender, System.EventArgs e)
		{	
			try 
			{
				client.ChangeId(Convert.ToInt64(txtId.Text));
			} 
			catch (Exception e1){
				ShowError(e1.Message);
			}
		}

		public static void ShowError(string msg)
		{
			MessageBox.Show (String.Format ("Error:\n\n{0} ", msg), "Spider Client Error",
				MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		public void AppendStatus(string text)
		{
			lock(txtStatus){
				txtStatus.AppendText(text);
			}
		}

		public void AppendStatusLine(string text)
		{
			lock(txtStatus)
			{
				txtStatus.AppendText(text + "\r\n");
			}
		}

		private void btnCreateTask_Click(object sender, System.EventArgs e)
		{
			try
			{
				client.CreateTask(txtTaskNameCreate.Text.Trim(),
					txtUrl.Text.Trim(),
					Convert.ToInt32(txtDeepth.Text.Trim()),
					chkSiteOnly.Checked);
			} 
			catch(Exception e1){
				ShowError(e1.Message);
			}
		}

		private void btnDelete_Click(object sender, System.EventArgs e)
		{
			try
			{
				client.RemoveTask(txtTaskManage.Text.Trim());
			}
			catch(Exception e1){
				ShowError(e1.Message);
			}
		}

		private void btnStatus_Click(object sender, System.EventArgs e)
		{
			
			try
			{
				client.GetStatus(txtTaskManage.Text.Trim());
			}
			catch(Exception e1)
			{
				ShowError(e1.Message);
			}
		}

		private void btnList_Click(object sender, System.EventArgs e)
		{
			try
			{
				client.ListTasks();
			}
			catch(Exception e1)
			{
				ShowError(e1.Message);
			}
		}

		private void lstTasks_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			string task = (string)lstTasks.SelectedItem;
			if(task == null) return;
			int pos = task.IndexOf(' ');
			if(pos > 0) task = task.Substring(0, pos);
			else task = "";
			txtTaskManage.Text = task;
		}

		private void btnResult_Click(object sender, System.EventArgs e)
		{
			try
			{
				client.Download(txtTaskManage.Text.Trim(), false);
			}
			catch(Exception e1)
			{
				ShowError(e1.Message);
			}
		}

		private void mnuAbout_Click(object sender, System.EventArgs e)
		{
			About about = new About();
			about.ShowDialog(this);
		}

		private void btnView_Click(object sender, System.EventArgs e)
		{
			ViewResultFile();
		}
	}
}
