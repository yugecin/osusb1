namespace osusb1 {
	partial class form {
	/// <summary>
	/// Required designer variable.
	/// </summary>
	private System.ComponentModel.IContainer components = null;

	/// <summary>
	/// Clean up any resources being used.
	/// </summary>
	/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
	protected override void Dispose(bool disposing) {
		if (disposing && (components != null)) {
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	#region Windows Form Designer generated code

	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.panel1 = new System.Windows.Forms.PictureBox();
			this.nuptime = new System.Windows.Forms.NumericUpDown();
			this.button1 = new System.Windows.Forms.Button();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.btnexport = new System.Windows.Forms.Button();
			this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
			this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
			this.trackBar1 = new System.Windows.Forms.TrackBar();
			this.trackBar2 = new System.Windows.Forms.TrackBar();
			this.trackBar3 = new System.Windows.Forms.TrackBar();
			this.trackBar4 = new System.Windows.Forms.TrackBar();
			this.trackBar5 = new System.Windows.Forms.TrackBar();
			this.trackBar6 = new System.Windows.Forms.TrackBar();
			this.trackBar7 = new System.Windows.Forms.TrackBar();
			this.trackBar8 = new System.Windows.Forms.TrackBar();
			this.udata0 = new System.Windows.Forms.Label();
			this.udata1 = new System.Windows.Forms.Label();
			this.udata2 = new System.Windows.Forms.Label();
			this.udata3 = new System.Windows.Forms.Label();
			this.udata4 = new System.Windows.Forms.Label();
			this.udata5 = new System.Windows.Forms.Label();
			this.udata6 = new System.Windows.Forms.Label();
			this.udata7 = new System.Windows.Forms.Label();
			this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.panel1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nuptime)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar6)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar7)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar8)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Location = new System.Drawing.Point(12, 12);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(640, 480);
			this.panel1.TabIndex = 0;
			this.panel1.TabStop = false;
			this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
			this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
			this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
			this.panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseUp);
			// 
			// nuptime
			// 
			this.nuptime.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.nuptime.Location = new System.Drawing.Point(12, 498);
			this.nuptime.Maximum = new decimal(new int[] {
            500000,
            0,
            0,
            0});
			this.nuptime.Name = "nuptime";
			this.nuptime.Size = new System.Drawing.Size(105, 20);
			this.nuptime.TabIndex = 1;
			this.nuptime.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.nuptime.ValueChanged += new System.EventHandler(this.nuptime_ValueChanged);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(577, 495);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 2;
			this.button1.Text = "auto";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// timer1
			// 
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(671, 61);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(21, 13);
			this.label3.TabIndex = 13;
			this.label3.Text = "fps";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(671, 35);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(16, 13);
			this.label2.TabIndex = 12;
			this.label2.Text = "to";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(671, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(27, 13);
			this.label1.TabIndex = 11;
			this.label1.Text = "from";
			// 
			// btnexport
			// 
			this.btnexport.Location = new System.Drawing.Point(735, 85);
			this.btnexport.Name = "btnexport";
			this.btnexport.Size = new System.Drawing.Size(120, 23);
			this.btnexport.TabIndex = 10;
			this.btnexport.Text = "export";
			this.btnexport.UseVisualStyleBackColor = true;
			this.btnexport.Click += new System.EventHandler(this.UI_ExportRequest);
			// 
			// numericUpDown2
			// 
			this.numericUpDown2.Location = new System.Drawing.Point(735, 33);
			this.numericUpDown2.Maximum = new decimal(new int[] {
            500000,
            0,
            0,
            0});
			this.numericUpDown2.Name = "numericUpDown2";
			this.numericUpDown2.Size = new System.Drawing.Size(120, 20);
			this.numericUpDown2.TabIndex = 8;
			this.numericUpDown2.Value = new decimal(new int[] {
            500000,
            0,
            0,
            0});
			// 
			// numericUpDown1
			// 
			this.numericUpDown1.Location = new System.Drawing.Point(735, 7);
			this.numericUpDown1.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
			this.numericUpDown1.Name = "numericUpDown1";
			this.numericUpDown1.Size = new System.Drawing.Size(120, 20);
			this.numericUpDown1.TabIndex = 7;
			// 
			// trackBar1
			// 
			this.trackBar1.Location = new System.Drawing.Point(674, 125);
			this.trackBar1.Maximum = 20;
			this.trackBar1.Minimum = -20;
			this.trackBar1.Name = "trackBar1";
			this.trackBar1.Size = new System.Drawing.Size(181, 42);
			this.trackBar1.TabIndex = 14;
			this.trackBar1.Tag = "0";
			// 
			// trackBar2
			// 
			this.trackBar2.Location = new System.Drawing.Point(674, 173);
			this.trackBar2.Maximum = 20;
			this.trackBar2.Minimum = -20;
			this.trackBar2.Name = "trackBar2";
			this.trackBar2.Size = new System.Drawing.Size(181, 42);
			this.trackBar2.TabIndex = 15;
			this.trackBar2.Tag = "1";
			// 
			// trackBar3
			// 
			this.trackBar3.Location = new System.Drawing.Point(674, 221);
			this.trackBar3.Maximum = 20;
			this.trackBar3.Minimum = -20;
			this.trackBar3.Name = "trackBar3";
			this.trackBar3.Size = new System.Drawing.Size(181, 42);
			this.trackBar3.TabIndex = 16;
			this.trackBar3.Tag = "2";
			// 
			// trackBar4
			// 
			this.trackBar4.Location = new System.Drawing.Point(674, 269);
			this.trackBar4.Maximum = 20;
			this.trackBar4.Minimum = -20;
			this.trackBar4.Name = "trackBar4";
			this.trackBar4.Size = new System.Drawing.Size(181, 42);
			this.trackBar4.TabIndex = 17;
			this.trackBar4.Tag = "3";
			// 
			// trackBar5
			// 
			this.trackBar5.Location = new System.Drawing.Point(674, 317);
			this.trackBar5.Maximum = 20;
			this.trackBar5.Minimum = -20;
			this.trackBar5.Name = "trackBar5";
			this.trackBar5.Size = new System.Drawing.Size(181, 42);
			this.trackBar5.TabIndex = 18;
			this.trackBar5.Tag = "4";
			// 
			// trackBar6
			// 
			this.trackBar6.Location = new System.Drawing.Point(674, 365);
			this.trackBar6.Maximum = 20;
			this.trackBar6.Minimum = -20;
			this.trackBar6.Name = "trackBar6";
			this.trackBar6.Size = new System.Drawing.Size(181, 42);
			this.trackBar6.TabIndex = 19;
			this.trackBar6.Tag = "5";
			// 
			// trackBar7
			// 
			this.trackBar7.Location = new System.Drawing.Point(674, 413);
			this.trackBar7.Maximum = 20;
			this.trackBar7.Minimum = -20;
			this.trackBar7.Name = "trackBar7";
			this.trackBar7.Size = new System.Drawing.Size(181, 42);
			this.trackBar7.TabIndex = 20;
			this.trackBar7.Tag = "6";
			// 
			// trackBar8
			// 
			this.trackBar8.Location = new System.Drawing.Point(674, 461);
			this.trackBar8.Maximum = 20;
			this.trackBar8.Minimum = -20;
			this.trackBar8.Name = "trackBar8";
			this.trackBar8.Size = new System.Drawing.Size(181, 42);
			this.trackBar8.TabIndex = 21;
			this.trackBar8.Tag = "7";
			// 
			// udata0
			// 
			this.udata0.AutoSize = true;
			this.udata0.Location = new System.Drawing.Point(831, 154);
			this.udata0.Name = "udata0";
			this.udata0.Size = new System.Drawing.Size(13, 13);
			this.udata0.TabIndex = 22;
			this.udata0.Text = "0";
			this.udata0.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// udata1
			// 
			this.udata1.AutoSize = true;
			this.udata1.Location = new System.Drawing.Point(831, 205);
			this.udata1.Name = "udata1";
			this.udata1.Size = new System.Drawing.Size(13, 13);
			this.udata1.TabIndex = 23;
			this.udata1.Text = "0";
			this.udata1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// udata2
			// 
			this.udata2.AutoSize = true;
			this.udata2.Location = new System.Drawing.Point(831, 250);
			this.udata2.Name = "udata2";
			this.udata2.Size = new System.Drawing.Size(13, 13);
			this.udata2.TabIndex = 24;
			this.udata2.Text = "0";
			this.udata2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// udata3
			// 
			this.udata3.AutoSize = true;
			this.udata3.Location = new System.Drawing.Point(831, 298);
			this.udata3.Name = "udata3";
			this.udata3.Size = new System.Drawing.Size(13, 13);
			this.udata3.TabIndex = 25;
			this.udata3.Text = "0";
			this.udata3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// udata4
			// 
			this.udata4.AutoSize = true;
			this.udata4.Location = new System.Drawing.Point(831, 346);
			this.udata4.Name = "udata4";
			this.udata4.Size = new System.Drawing.Size(13, 13);
			this.udata4.TabIndex = 26;
			this.udata4.Text = "0";
			this.udata4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// udata5
			// 
			this.udata5.AutoSize = true;
			this.udata5.Location = new System.Drawing.Point(831, 394);
			this.udata5.Name = "udata5";
			this.udata5.Size = new System.Drawing.Size(13, 13);
			this.udata5.TabIndex = 27;
			this.udata5.Text = "0";
			this.udata5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// udata6
			// 
			this.udata6.AutoSize = true;
			this.udata6.Location = new System.Drawing.Point(831, 442);
			this.udata6.Name = "udata6";
			this.udata6.Size = new System.Drawing.Size(13, 13);
			this.udata6.TabIndex = 28;
			this.udata6.Text = "0";
			this.udata6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// udata7
			// 
			this.udata7.AutoSize = true;
			this.udata7.Location = new System.Drawing.Point(831, 490);
			this.udata7.Name = "udata7";
			this.udata7.Size = new System.Drawing.Size(13, 13);
			this.udata7.TabIndex = 29;
			this.udata7.Text = "0";
			this.udata7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// numericUpDown3
			// 
			this.numericUpDown3.Location = new System.Drawing.Point(735, 59);
			this.numericUpDown3.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.numericUpDown3.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDown3.Name = "numericUpDown3";
			this.numericUpDown3.Size = new System.Drawing.Size(120, 20);
			this.numericUpDown3.TabIndex = 9;
			this.numericUpDown3.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
			// 
			// form
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(867, 548);
			this.Controls.Add(this.udata7);
			this.Controls.Add(this.udata6);
			this.Controls.Add(this.udata5);
			this.Controls.Add(this.udata4);
			this.Controls.Add(this.udata3);
			this.Controls.Add(this.udata2);
			this.Controls.Add(this.udata1);
			this.Controls.Add(this.udata0);
			this.Controls.Add(this.trackBar8);
			this.Controls.Add(this.trackBar7);
			this.Controls.Add(this.trackBar6);
			this.Controls.Add(this.trackBar5);
			this.Controls.Add(this.trackBar4);
			this.Controls.Add(this.trackBar3);
			this.Controls.Add(this.trackBar2);
			this.Controls.Add(this.trackBar1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnexport);
			this.Controls.Add(this.numericUpDown3);
			this.Controls.Add(this.numericUpDown2);
			this.Controls.Add(this.numericUpDown1);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.nuptime);
			this.Controls.Add(this.panel1);
			this.DoubleBuffered = true;
			this.Name = "form";
			this.Text = "Form1";
			((System.ComponentModel.ISupportInitialize)(this.panel1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nuptime)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar6)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar7)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar8)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

	}

	#endregion

	private System.Windows.Forms.PictureBox panel1;
	private System.Windows.Forms.NumericUpDown nuptime;
	private System.Windows.Forms.Button button1;
	private System.Windows.Forms.Timer timer1;
	private System.Windows.Forms.Label label3;
	private System.Windows.Forms.Label label2;
	private System.Windows.Forms.Label label1;
	private System.Windows.Forms.Button btnexport;
	private System.Windows.Forms.NumericUpDown numericUpDown2;
	private System.Windows.Forms.NumericUpDown numericUpDown1;
	private System.Windows.Forms.TrackBar trackBar1;
	private System.Windows.Forms.TrackBar trackBar2;
	private System.Windows.Forms.TrackBar trackBar3;
	private System.Windows.Forms.TrackBar trackBar4;
	private System.Windows.Forms.TrackBar trackBar5;
	private System.Windows.Forms.TrackBar trackBar6;
	private System.Windows.Forms.TrackBar trackBar7;
	private System.Windows.Forms.TrackBar trackBar8;
	private System.Windows.Forms.Label udata0;
	private System.Windows.Forms.Label udata1;
	private System.Windows.Forms.Label udata2;
	private System.Windows.Forms.Label udata3;
	private System.Windows.Forms.Label udata4;
	private System.Windows.Forms.Label udata5;
	private System.Windows.Forms.Label udata6;
	private System.Windows.Forms.Label udata7;
	private System.Windows.Forms.NumericUpDown numericUpDown3;
}
}

