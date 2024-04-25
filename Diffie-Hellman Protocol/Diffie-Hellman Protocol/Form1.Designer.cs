namespace Diffie_Hellman_Protocol
{
    partial class Form1
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
            this.InputA = new System.Windows.Forms.RichTextBox();
            this.InputB = new System.Windows.Forms.RichTextBox();
            this.InputG = new System.Windows.Forms.RichTextBox();
            this.InputP = new System.Windows.Forms.RichTextBox();
            this.Generate = new System.Windows.Forms.Button();
            this.OutputA = new System.Windows.Forms.RichTextBox();
            this.OutputB = new System.Windows.Forms.RichTextBox();
            this.OutputKa = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.Count = new System.Windows.Forms.Button();
            this.OutputKb = new System.Windows.Forms.RichTextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // InputA
            // 
            this.InputA.Location = new System.Drawing.Point(12, 34);
            this.InputA.Name = "InputA";
            this.InputA.Size = new System.Drawing.Size(384, 41);
            this.InputA.TabIndex = 0;
            this.InputA.Text = "";
            // 
            // InputB
            // 
            this.InputB.Location = new System.Drawing.Point(12, 102);
            this.InputB.Name = "InputB";
            this.InputB.Size = new System.Drawing.Size(384, 41);
            this.InputB.TabIndex = 1;
            this.InputB.Text = "";
            // 
            // InputG
            // 
            this.InputG.Location = new System.Drawing.Point(12, 167);
            this.InputG.Name = "InputG";
            this.InputG.Size = new System.Drawing.Size(384, 41);
            this.InputG.TabIndex = 2;
            this.InputG.Text = "";
            // 
            // InputP
            // 
            this.InputP.Location = new System.Drawing.Point(12, 224);
            this.InputP.Name = "InputP";
            this.InputP.Size = new System.Drawing.Size(384, 41);
            this.InputP.TabIndex = 3;
            this.InputP.Text = "";
            // 
            // Generate
            // 
            this.Generate.Location = new System.Drawing.Point(315, 271);
            this.Generate.Name = "Generate";
            this.Generate.Size = new System.Drawing.Size(81, 23);
            this.Generate.TabIndex = 4;
            this.Generate.Text = "Generate";
            this.Generate.UseVisualStyleBackColor = true;
            this.Generate.Click += new System.EventHandler(this.Generate_Click);
            // 
            // OutputA
            // 
            this.OutputA.Location = new System.Drawing.Point(12, 302);
            this.OutputA.Name = "OutputA";
            this.OutputA.Size = new System.Drawing.Size(384, 41);
            this.OutputA.TabIndex = 5;
            this.OutputA.Text = "";
            // 
            // OutputB
            // 
            this.OutputB.Location = new System.Drawing.Point(12, 349);
            this.OutputB.Name = "OutputB";
            this.OutputB.Size = new System.Drawing.Size(384, 41);
            this.OutputB.TabIndex = 6;
            this.OutputB.Text = "";
            // 
            // OutputKa
            // 
            this.OutputKa.Location = new System.Drawing.Point(12, 397);
            this.OutputKa.Name = "OutputKa";
            this.OutputKa.Size = new System.Drawing.Size(384, 41);
            this.OutputKa.TabIndex = 7;
            this.OutputKa.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(401, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(15, 16);
            this.label1.TabIndex = 8;
            this.label1.Text = "a";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(401, 116);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(15, 16);
            this.label2.TabIndex = 9;
            this.label2.Text = "b";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(401, 181);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(15, 16);
            this.label3.TabIndex = 10;
            this.label3.Text = "g";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(401, 239);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(15, 16);
            this.label4.TabIndex = 11;
            this.label4.Text = "p";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(402, 317);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(16, 16);
            this.label5.TabIndex = 12;
            this.label5.Text = "A";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(403, 363);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(16, 16);
            this.label6.TabIndex = 13;
            this.label6.Text = "B";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(403, 413);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(23, 16);
            this.label7.TabIndex = 14;
            this.label7.Text = "Ka";
            // 
            // Count
            // 
            this.Count.Location = new System.Drawing.Point(315, 548);
            this.Count.Name = "Count";
            this.Count.Size = new System.Drawing.Size(81, 23);
            this.Count.TabIndex = 15;
            this.Count.Text = "Count";
            this.Count.UseVisualStyleBackColor = true;
            this.Count.Click += new System.EventHandler(this.Count_Click);
            // 
            // OutputKb
            // 
            this.OutputKb.Location = new System.Drawing.Point(12, 444);
            this.OutputKb.Name = "OutputKb";
            this.OutputKb.Size = new System.Drawing.Size(384, 41);
            this.OutputKb.TabIndex = 16;
            this.OutputKb.Text = "";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(404, 456);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(23, 16);
            this.label8.TabIndex = 17;
            this.label8.Text = "Kb";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(12, 491);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(384, 41);
            this.richTextBox1.TabIndex = 18;
            this.richTextBox1.Text = "";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(401, 503);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(38, 16);
            this.label9.TabIndex = 19;
            this.label9.Text = "Time";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(445, 593);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.OutputKb);
            this.Controls.Add(this.Count);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.OutputKa);
            this.Controls.Add(this.OutputB);
            this.Controls.Add(this.OutputA);
            this.Controls.Add(this.Generate);
            this.Controls.Add(this.InputP);
            this.Controls.Add(this.InputG);
            this.Controls.Add(this.InputB);
            this.Controls.Add(this.InputA);
            this.Name = "Form1";
            this.Text = "Diffie-Hellman Protocol";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox InputA;
        private System.Windows.Forms.RichTextBox InputB;
        private System.Windows.Forms.RichTextBox InputG;
        private System.Windows.Forms.RichTextBox InputP;
        private System.Windows.Forms.Button Generate;
        private System.Windows.Forms.RichTextBox OutputA;
        private System.Windows.Forms.RichTextBox OutputB;
        private System.Windows.Forms.RichTextBox OutputKa;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button Count;
        private System.Windows.Forms.RichTextBox OutputKb;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label label9;
    }
}

