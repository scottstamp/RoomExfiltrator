namespace RoomExfiltrator
{
    partial class frmMain
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
            this.txtFurni = new System.Windows.Forms.RichTextBox();
            this.txtOwnerId = new System.Windows.Forms.TextBox();
            this.txtRoomId = new System.Windows.Forms.TextBox();
            this.lblOwnerId = new System.Windows.Forms.Label();
            this.lblRoomId = new System.Windows.Forms.Label();
            this.chkSqlOnly = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // txtFurni
            // 
            this.txtFurni.Location = new System.Drawing.Point(0, 38);
            this.txtFurni.Name = "txtFurni";
            this.txtFurni.Size = new System.Drawing.Size(958, 385);
            this.txtFurni.TabIndex = 0;
            this.txtFurni.Text = "";
            // 
            // txtOwnerId
            // 
            this.txtOwnerId.Location = new System.Drawing.Point(73, 12);
            this.txtOwnerId.Name = "txtOwnerId";
            this.txtOwnerId.Size = new System.Drawing.Size(41, 20);
            this.txtOwnerId.TabIndex = 1;
            this.txtOwnerId.Text = "8";
            // 
            // txtRoomId
            // 
            this.txtRoomId.Location = new System.Drawing.Point(178, 12);
            this.txtRoomId.Name = "txtRoomId";
            this.txtRoomId.Size = new System.Drawing.Size(41, 20);
            this.txtRoomId.TabIndex = 2;
            // 
            // lblOwnerId
            // 
            this.lblOwnerId.AutoSize = true;
            this.lblOwnerId.Location = new System.Drawing.Point(12, 15);
            this.lblOwnerId.Name = "lblOwnerId";
            this.lblOwnerId.Size = new System.Drawing.Size(55, 13);
            this.lblOwnerId.TabIndex = 3;
            this.lblOwnerId.Text = "Owner ID:";
            // 
            // lblRoomId
            // 
            this.lblRoomId.AutoSize = true;
            this.lblRoomId.Location = new System.Drawing.Point(120, 15);
            this.lblRoomId.Name = "lblRoomId";
            this.lblRoomId.Size = new System.Drawing.Size(52, 13);
            this.lblRoomId.TabIndex = 4;
            this.lblRoomId.Text = "Room ID:";
            // 
            // chkSqlOnly
            // 
            this.chkSqlOnly.AutoSize = true;
            this.chkSqlOnly.Location = new System.Drawing.Point(225, 14);
            this.chkSqlOnly.Name = "chkSqlOnly";
            this.chkSqlOnly.Size = new System.Drawing.Size(71, 17);
            this.chkSqlOnly.TabIndex = 5;
            this.chkSqlOnly.Text = "SQL Only";
            this.chkSqlOnly.UseVisualStyleBackColor = true;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(958, 423);
            this.Controls.Add(this.chkSqlOnly);
            this.Controls.Add(this.lblRoomId);
            this.Controls.Add(this.lblOwnerId);
            this.Controls.Add(this.txtRoomId);
            this.Controls.Add(this.txtOwnerId);
            this.Controls.Add(this.txtFurni);
            this.Name = "frmMain";
            this.Text = "RoomExfiltrator";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox txtFurni;
        private System.Windows.Forms.TextBox txtOwnerId;
        private System.Windows.Forms.TextBox txtRoomId;
        private System.Windows.Forms.Label lblOwnerId;
        private System.Windows.Forms.Label lblRoomId;
        private System.Windows.Forms.CheckBox chkSqlOnly;
    }
}

