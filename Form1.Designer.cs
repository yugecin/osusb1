﻿namespace osusb1 {
	partial class Form1 {
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.nuptime = new System.Windows.Forms.NumericUpDown();
			this.button1 = new System.Windows.Forms.Button();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			((System.ComponentModel.ISupportInitialize)(this.nuptime)).BeginInit();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Location = new System.Drawing.Point(12, 12);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(640, 480);
			this.panel1.TabIndex = 0;
			this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
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
            10000,
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
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(669, 548);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.nuptime);
			this.Controls.Add(this.panel1);
			this.Name = "Form1";
			this.Text = "Form1";
			((System.ComponentModel.ISupportInitialize)(this.nuptime)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.NumericUpDown nuptime;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Timer timer1;
	}
}

