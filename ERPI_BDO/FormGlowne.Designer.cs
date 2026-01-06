namespace ERPI_BDO
{
    partial class FormGlowne
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            panel4 = new Panel();
            webViewLogin = new Microsoft.Web.WebView2.WinForms.WebView2();
            panel3 = new Panel();
            btnZaloguj = new Button();
            tabPage2 = new TabPage();
            panelEupTop = new Panel();
            dgvEup = new DataGridView();
            panelEupBottom = new Panel();
            btnWybierzEup = new Button();
            tabPage3 = new TabPage();
            dgvKpo = new DataGridView();
            tabPage4 = new TabPage();
            dgvKpoSender = new DataGridView();
            tabPage5 = new TabPage();
            dgvKpoTransport = new DataGridView();
            btnPobierzKpo = new Button();
            label2 = new Label();
            label1 = new Label();
            dtKpoDo = new DateTimePicker();
            dtKpoOd = new DateTimePicker();
            statusStrip1 = new StatusStrip();
            lblStatus = new ToolStripStatusLabel();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            lblWybranyEup = new ToolStripStatusLabel();
            toolStripStatusLabel2 = new ToolStripStatusLabel();
            toolStripStatusLabelKpo = new ToolStripStatusLabel();
            menuStrip1 = new MenuStrip();
            debugToolStripMenuItem1 = new ToolStripMenuItem();
            panel5 = new Panel();
            checkBoxDatyTransportu = new CheckBox();
            panel6 = new Panel();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)webViewLogin).BeginInit();
            panel3.SuspendLayout();
            tabPage2.SuspendLayout();
            panelEupTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvEup).BeginInit();
            panelEupBottom.SuspendLayout();
            tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvKpo).BeginInit();
            tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvKpoSender).BeginInit();
            tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvKpoTransport).BeginInit();
            statusStrip1.SuspendLayout();
            menuStrip1.SuspendLayout();
            panel5.SuspendLayout();
            panel6.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Controls.Add(tabPage4);
            tabControl1.Controls.Add(tabPage5);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(990, 478);
            tabControl1.TabIndex = 6;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(panel4);
            tabPage1.Controls.Add(panel3);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(982, 450);
            tabPage1.TabIndex = 2;
            tabPage1.Text = "Logowanie do BDO...";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // panel4
            // 
            panel4.BackColor = Color.Silver;
            panel4.Controls.Add(webViewLogin);
            panel4.Dock = DockStyle.Fill;
            panel4.Location = new Point(3, 3);
            panel4.Name = "panel4";
            panel4.Size = new Size(976, 386);
            panel4.TabIndex = 2;
            // 
            // webViewLogin
            // 
            webViewLogin.AllowExternalDrop = true;
            webViewLogin.BackColor = SystemColors.Control;
            webViewLogin.CreationProperties = null;
            webViewLogin.DefaultBackgroundColor = Color.White;
            webViewLogin.Dock = DockStyle.Fill;
            webViewLogin.Location = new Point(0, 0);
            webViewLogin.Name = "webViewLogin";
            webViewLogin.Size = new Size(976, 386);
            webViewLogin.TabIndex = 0;
            webViewLogin.ZoomFactor = 1D;
            // 
            // panel3
            // 
            panel3.Controls.Add(btnZaloguj);
            panel3.Dock = DockStyle.Bottom;
            panel3.Location = new Point(3, 389);
            panel3.Name = "panel3";
            panel3.Size = new Size(976, 58);
            panel3.TabIndex = 1;
            // 
            // btnZaloguj
            // 
            btnZaloguj.Location = new Point(5, 17);
            btnZaloguj.Name = "btnZaloguj";
            btnZaloguj.Size = new Size(106, 23);
            btnZaloguj.TabIndex = 9;
            btnZaloguj.Text = "Zaloguj do BDO";
            btnZaloguj.UseVisualStyleBackColor = true;
            btnZaloguj.Click += btnZaloguj_Click;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(panelEupTop);
            tabPage2.Controls.Add(panelEupBottom);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(982, 450);
            tabPage2.TabIndex = 0;
            tabPage2.Text = "EUP (wybór firmy / lokalizacji)";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // panelEupTop
            // 
            panelEupTop.Controls.Add(dgvEup);
            panelEupTop.Dock = DockStyle.Fill;
            panelEupTop.Location = new Point(3, 3);
            panelEupTop.Name = "panelEupTop";
            panelEupTop.Size = new Size(976, 384);
            panelEupTop.TabIndex = 11;
            // 
            // dgvEup
            // 
            dgvEup.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvEup.Dock = DockStyle.Fill;
            dgvEup.Location = new Point(0, 0);
            dgvEup.Name = "dgvEup";
            dgvEup.Size = new Size(976, 384);
            dgvEup.TabIndex = 6;
            dgvEup.CellDoubleClick += dgvEup_CellDoubleClick;
            // 
            // panelEupBottom
            // 
            panelEupBottom.Controls.Add(btnWybierzEup);
            panelEupBottom.Dock = DockStyle.Bottom;
            panelEupBottom.Location = new Point(3, 387);
            panelEupBottom.Name = "panelEupBottom";
            panelEupBottom.Size = new Size(976, 60);
            panelEupBottom.TabIndex = 10;
            // 
            // btnWybierzEup
            // 
            btnWybierzEup.Location = new Point(581, 21);
            btnWybierzEup.Name = "btnWybierzEup";
            btnWybierzEup.Size = new Size(136, 23);
            btnWybierzEup.TabIndex = 7;
            btnWybierzEup.Text = "Wybierz lokalizację";
            btnWybierzEup.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(dgvKpo);
            tabPage3.Location = new Point(4, 24);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(982, 450);
            tabPage3.TabIndex = 1;
            tabPage3.Text = "KPO przejmujący";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // dgvKpo
            // 
            dgvKpo.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvKpo.Dock = DockStyle.Fill;
            dgvKpo.Location = new Point(3, 3);
            dgvKpo.Name = "dgvKpo";
            dgvKpo.Size = new Size(976, 444);
            dgvKpo.TabIndex = 0;
            dgvKpo.CellDoubleClick += dgvKpo_CellDoubleClick;
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(dgvKpoSender);
            tabPage4.Location = new Point(4, 24);
            tabPage4.Name = "tabPage4";
            tabPage4.Padding = new Padding(3);
            tabPage4.Size = new Size(982, 450);
            tabPage4.TabIndex = 3;
            tabPage4.Text = "KPO przekazujący";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // dgvKpoSender
            // 
            dgvKpoSender.AllowUserToAddRows = false;
            dgvKpoSender.AllowUserToDeleteRows = false;
            dgvKpoSender.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvKpoSender.Dock = DockStyle.Fill;
            dgvKpoSender.Location = new Point(3, 3);
            dgvKpoSender.Name = "dgvKpoSender";
            dgvKpoSender.ReadOnly = true;
            dgvKpoSender.RowHeadersWidth = 51;
            dgvKpoSender.Size = new Size(976, 444);
            dgvKpoSender.TabIndex = 6;
            // 
            // tabPage5
            // 
            tabPage5.Controls.Add(dgvKpoTransport);
            tabPage5.Location = new Point(4, 24);
            tabPage5.Name = "tabPage5";
            tabPage5.Padding = new Padding(3);
            tabPage5.Size = new Size(982, 450);
            tabPage5.TabIndex = 4;
            tabPage5.Text = "KPO transportujący";
            tabPage5.UseVisualStyleBackColor = true;
            // 
            // dgvKpoTransport
            // 
            dgvKpoTransport.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvKpoTransport.Dock = DockStyle.Fill;
            dgvKpoTransport.Location = new Point(3, 3);
            dgvKpoTransport.Name = "dgvKpoTransport";
            dgvKpoTransport.Size = new Size(976, 444);
            dgvKpoTransport.TabIndex = 0;
            // 
            // btnPobierzKpo
            // 
            btnPobierzKpo.Location = new Point(741, 18);
            btnPobierzKpo.Name = "btnPobierzKpo";
            btnPobierzKpo.Size = new Size(121, 24);
            btnPobierzKpo.TabIndex = 4;
            btnPobierzKpo.Text = "Pobierz KPO";
            btnPobierzKpo.UseVisualStyleBackColor = true;
            btnPobierzKpo.Click += btnPobierzKpo_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(249, 23);
            label2.Name = "label2";
            label2.Size = new Size(27, 15);
            label2.TabIndex = 3;
            label2.Text = "do: ";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(7, 23);
            label1.Name = "label1";
            label1.Size = new Size(27, 15);
            label1.TabIndex = 2;
            label1.Text = "od: ";
            // 
            // dtKpoDo
            // 
            dtKpoDo.Location = new Point(282, 15);
            dtKpoDo.Name = "dtKpoDo";
            dtKpoDo.Size = new Size(223, 23);
            dtKpoDo.TabIndex = 1;
            // 
            // dtKpoOd
            // 
            dtKpoOd.Location = new Point(43, 15);
            dtKpoOd.Name = "dtKpoOd";
            dtKpoOd.Size = new Size(200, 23);
            dtKpoOd.TabIndex = 0;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { lblStatus, toolStripStatusLabel1, lblWybranyEup, toolStripStatusLabel2, toolStripStatusLabelKpo });
            statusStrip1.Location = new Point(0, 556);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(990, 22);
            statusStrip1.TabIndex = 7;
            statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(85, 17);
            lblStatus.Text = "Status aplikacji";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(16, 17);
            toolStripStatusLabel1.Text = " | ";
            // 
            // lblWybranyEup
            // 
            lblWybranyEup.Name = "lblWybranyEup";
            lblWybranyEup.Size = new Size(93, 17);
            lblWybranyEup.Text = "Nic nie wybrano";
            // 
            // toolStripStatusLabel2
            // 
            toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            toolStripStatusLabel2.Size = new Size(16, 17);
            toolStripStatusLabel2.Text = " | ";
            // 
            // toolStripStatusLabelKpo
            // 
            toolStripStatusLabelKpo.Name = "toolStripStatusLabelKpo";
            toolStripStatusLabelKpo.Size = new Size(12, 17);
            toolStripStatusLabelKpo.Text = "-";
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { debugToolStripMenuItem1 });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(990, 24);
            menuStrip1.TabIndex = 8;
            menuStrip1.Text = "menuStrip1";
            // 
            // debugToolStripMenuItem1
            // 
            debugToolStripMenuItem1.Name = "debugToolStripMenuItem1";
            debugToolStripMenuItem1.Size = new Size(54, 20);
            debugToolStripMenuItem1.Text = "Debug";
            debugToolStripMenuItem1.Click += debugToolStripMenuItem1_Click;
            // 
            // panel5
            // 
            panel5.Controls.Add(checkBoxDatyTransportu);
            panel5.Controls.Add(label2);
            panel5.Controls.Add(btnPobierzKpo);
            panel5.Controls.Add(label1);
            panel5.Controls.Add(dtKpoDo);
            panel5.Controls.Add(dtKpoOd);
            panel5.Dock = DockStyle.Top;
            panel5.Location = new Point(0, 24);
            panel5.Name = "panel5";
            panel5.Size = new Size(990, 54);
            panel5.TabIndex = 9;
            // 
            // checkBoxDatyTransportu
            // 
            checkBoxDatyTransportu.AutoSize = true;
            checkBoxDatyTransportu.Checked = true;
            checkBoxDatyTransportu.CheckState = CheckState.Checked;
            checkBoxDatyTransportu.Location = new Point(532, 22);
            checkBoxDatyTransportu.Name = "checkBoxDatyTransportu";
            checkBoxDatyTransportu.Size = new Size(189, 19);
            checkBoxDatyTransportu.TabIndex = 5;
            checkBoxDatyTransportu.Text = "daty transportu / zatwierdzenia";
            checkBoxDatyTransportu.UseVisualStyleBackColor = true;
            // 
            // panel6
            // 
            panel6.Controls.Add(tabControl1);
            panel6.Dock = DockStyle.Fill;
            panel6.Location = new Point(0, 78);
            panel6.Name = "panel6";
            panel6.Size = new Size(990, 478);
            panel6.TabIndex = 10;
            // 
            // FormGlowne
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(990, 578);
            Controls.Add(panel6);
            Controls.Add(panel5);
            Controls.Add(statusStrip1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "FormGlowne";
            Text = "ERPI BDO";
            Load += FormGlowne_Load;
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)webViewLogin).EndInit();
            panel3.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            panelEupTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvEup).EndInit();
            panelEupBottom.ResumeLayout(false);
            tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvKpo).EndInit();
            tabPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvKpoSender).EndInit();
            tabPage5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvKpoTransport).EndInit();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            panel5.ResumeLayout(false);
            panel5.PerformLayout();
            panel6.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TabControl tabControl1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private DataGridView dgvEup;
        private Button btnWybierzEup;
        
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel lblStatus;
        private Panel panelEupTop;
        private Panel panelEupBottom;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripStatusLabel lblWybranyEup;
        private DataGridView dgvKpo;
        private Button btnPobierzKpo;
        private Label label2;
        private Label label1;
        private DateTimePicker dtKpoDo;
        private DateTimePicker dtKpoOd;
        private TabPage tabPage1;
        private Microsoft.Web.WebView2.WinForms.WebView2 webViewLogin;
        private Panel panel3;
        private Panel panel4;
        private Button btnZaloguj;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem debugToolStripMenuItem1;
        private TabPage tabPage4;
        private DataGridView dgvKpoSender;
        private ToolStripStatusLabel toolStripStatusLabel2;
        private ToolStripStatusLabel toolStripStatusLabelKpo;
        private Panel panel5;
        private Panel panel6;
        private TabPage tabPage5;
        private DataGridView dgvKpoTransport;
        private CheckBox checkBoxDatyTransportu;
    }
}
