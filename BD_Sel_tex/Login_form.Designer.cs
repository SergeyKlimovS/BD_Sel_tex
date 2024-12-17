namespace BD_Sel_tex
{
    partial class Login_form
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.Btn_logIn = new System.Windows.Forms.Button();
            this.Text_login = new System.Windows.Forms.TextBox();
            this.Text_passwd = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // Btn_logIn
            // 
            this.Btn_logIn.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Btn_logIn.Location = new System.Drawing.Point(84, 343);
            this.Btn_logIn.Name = "Btn_logIn";
            this.Btn_logIn.Size = new System.Drawing.Size(176, 45);
            this.Btn_logIn.TabIndex = 0;
            this.Btn_logIn.Text = "Зайти";
            this.Btn_logIn.UseVisualStyleBackColor = true;
            this.Btn_logIn.Click += new System.EventHandler(this.Btn_logIn_Click);
            // 
            // Text_login
            // 
            this.Text_login.BackColor = System.Drawing.Color.White;
            this.Text_login.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Text_login.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Text_login.Location = new System.Drawing.Point(57, 170);
            this.Text_login.Multiline = true;
            this.Text_login.Name = "Text_login";
            this.Text_login.Size = new System.Drawing.Size(229, 53);
            this.Text_login.TabIndex = 1;
            // 
            // Text_passwd
            // 
            this.Text_passwd.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Text_passwd.Location = new System.Drawing.Point(57, 246);
            this.Text_passwd.Multiline = true;
            this.Text_passwd.Name = "Text_passwd";
            this.Text_passwd.Size = new System.Drawing.Size(229, 53);
            this.Text_passwd.TabIndex = 2;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.pictureBox1.Location = new System.Drawing.Point(103, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(122, 108);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // Login_form
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Silver;
            this.ClientSize = new System.Drawing.Size(336, 457);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.Text_passwd);
            this.Controls.Add(this.Text_login);
            this.Controls.Add(this.Btn_logIn);
            this.MaximumSize = new System.Drawing.Size(352, 496);
            this.MinimumSize = new System.Drawing.Size(352, 496);
            this.Name = "Login_form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Авторизация";
            this.Load += new System.EventHandler(this.Login_form_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Btn_logIn;
        private System.Windows.Forms.TextBox Text_login;
        private System.Windows.Forms.TextBox Text_passwd;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

