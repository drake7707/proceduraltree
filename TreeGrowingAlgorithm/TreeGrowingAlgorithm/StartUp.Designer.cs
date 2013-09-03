namespace TreeGrowingAlgorithm
{
    partial class StartUp
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn2D = new System.Windows.Forms.Button();
            this.btn3D = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn2D
            // 
            this.btn2D.Location = new System.Drawing.Point(25, 42);
            this.btn2D.Name = "btn2D";
            this.btn2D.Size = new System.Drawing.Size(120, 23);
            this.btn2D.TabIndex = 0;
            this.btn2D.Text = "2D";
            this.btn2D.UseVisualStyleBackColor = true;
            this.btn2D.Click += new System.EventHandler(this.btn2D_Click);
            // 
            // btn3D
            // 
            this.btn3D.Location = new System.Drawing.Point(206, 42);
            this.btn3D.Name = "btn3D";
            this.btn3D.Size = new System.Drawing.Size(116, 23);
            this.btn3D.TabIndex = 1;
            this.btn3D.Text = "3D";
            this.btn3D.UseVisualStyleBackColor = true;
            this.btn3D.Click += new System.EventHandler(this.btn3D_Click);
            // 
            // StartUp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(350, 122);
            this.Controls.Add(this.btn3D);
            this.Controls.Add(this.btn2D);
            this.Name = "StartUp";
            this.Text = "Tree algorithm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn2D;
        private System.Windows.Forms.Button btn3D;
    }
}