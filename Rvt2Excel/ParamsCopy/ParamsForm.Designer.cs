namespace Rvt2Excel.ParamsCopy
{
    partial class ParamsForm
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
            this.ok = new System.Windows.Forms.Button();
            this.from = new System.Windows.Forms.Label();
            this.to = new System.Windows.Forms.Label();
            this.arrow = new System.Windows.Forms.Label();
            this.cancel = new System.Windows.Forms.Button();
            this.toBox = new System.Windows.Forms.ComboBox();
            this.fromBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // ok
            // 
            this.ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ok.Location = new System.Drawing.Point(132, 77);
            this.ok.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(75, 30);
            this.ok.TabIndex = 2;
            this.ok.Text = "确认";
            this.ok.UseVisualStyleBackColor = true;
            // 
            // from
            // 
            this.from.AutoSize = true;
            this.from.Location = new System.Drawing.Point(12, 19);
            this.from.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.from.Name = "from";
            this.from.Size = new System.Drawing.Size(29, 12);
            this.from.TabIndex = 3;
            this.from.Text = "从：";
            // 
            // to
            // 
            this.to.AutoSize = true;
            this.to.Location = new System.Drawing.Point(166, 19);
            this.to.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.to.Name = "to";
            this.to.Size = new System.Drawing.Size(29, 12);
            this.to.TabIndex = 4;
            this.to.Text = "到：";
            // 
            // arrow
            // 
            this.arrow.AutoSize = true;
            this.arrow.Location = new System.Drawing.Point(139, 47);
            this.arrow.Name = "arrow";
            this.arrow.Size = new System.Drawing.Size(23, 12);
            this.arrow.TabIndex = 5;
            this.arrow.Text = "<->";
            // 
            // cancel
            // 
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.Location = new System.Drawing.Point(213, 77);
            this.cancel.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 30);
            this.cancel.TabIndex = 6;
            this.cancel.Text = "取消";
            this.cancel.UseVisualStyleBackColor = true;
            // 
            // toBox
            // 
            this.toBox.FormattingEnabled = true;
            this.toBox.Location = new System.Drawing.Point(168, 44);
            this.toBox.Name = "toBox";
            this.toBox.Size = new System.Drawing.Size(121, 20);
            this.toBox.TabIndex = 7;
            // 
            // fromBox
            // 
            this.fromBox.FormattingEnabled = true;
            this.fromBox.Location = new System.Drawing.Point(12, 44);
            this.fromBox.Name = "fromBox";
            this.fromBox.Size = new System.Drawing.Size(121, 20);
            this.fromBox.TabIndex = 8;
            // 
            // ParamsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 120);
            this.ControlBox = false;
            this.Controls.Add(this.fromBox);
            this.Controls.Add(this.toBox);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.arrow);
            this.Controls.Add(this.to);
            this.Controls.Add(this.from);
            this.Controls.Add(this.ok);
            this.Name = "ParamsForm";
            this.Text = "设置窗体";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button ok;
        private System.Windows.Forms.Label from;
        private System.Windows.Forms.Label to;
        private System.Windows.Forms.Label arrow;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.ComboBox toBox;
        private System.Windows.Forms.ComboBox fromBox;
    }
}