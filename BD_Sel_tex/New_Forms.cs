using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BD_Sel_tex
{
    public partial class New_Forms : Form
    {
      
        private string context;

        public New_Forms(string context)
        {
            InitializeComponent();
            this.context = context;
            this.StartPosition = FormStartPosition.CenterScreen;
            GenerateDynamicControls();
        }

        private void CreateLabelAndTextBox(string name, string labelText, ref int currentHeight, int controlHeight, int horizontalSpacing, ref int maxWidth, bool isReadOnly = false)
        {
            Label lbl = new Label
            {
                Text = labelText,
                Font = new Font("Times New Roman", 14),
                AutoSize = true,
                Location = new Point(horizontalSpacing, currentHeight)
            };

            TextBox txt = new TextBox
            {
                Name = "txt" + name,
                Font = new Font("Times New Roman", 14),
                Width = 300,
                Location = new Point(lbl.Left + 200, currentHeight),
                ReadOnly = isReadOnly
            };

            Controls.Add(lbl);
            Controls.Add(txt);
            currentHeight += controlHeight + 20;
            maxWidth = Math.Max(maxWidth, txt.Right + horizontalSpacing);
        }

        private void GenerateDynamicControls()
        {
            int controlHeight = 30;
            int verticalSpacing = 20;
            int horizontalSpacing = 20;

            int maxWidth = 0;
            int currentHeight = 10;

            switch (context)
            {
                case "Manufacturers":
                    CreateLabelAndTextBox("ManufacturersNAME", "Название производителя", ref currentHeight, controlHeight, horizontalSpacing, ref maxWidth);
                    CreateLabelAndTextBox("Country", "Страна", ref currentHeight, controlHeight, horizontalSpacing, ref maxWidth);
                    break;

                case "SpareParts":
                    CreateLabelAndTextBox("SparePartsNAME", "Название запчасти", ref currentHeight, controlHeight, horizontalSpacing, ref maxWidth);
                    CreateLabelAndTextBox("Compatible", "Совместимость", ref currentHeight, controlHeight, horizontalSpacing, ref maxWidth);
                    break;

                case "AgriculturalWorks":
                    CreateLabelAndTextBox("WorkTYPE", "Тип работы", ref currentHeight, controlHeight, horizontalSpacing, ref maxWidth);
                    CreateLabelAndTextBox("DateCompleted", "Дата выполнения", ref currentHeight, controlHeight, horizontalSpacing, ref maxWidth);
                    break;

                case "Models":
                    CreateLabelAndTextBox("ModelNAME", "Название модели", ref currentHeight, controlHeight, horizontalSpacing, ref maxWidth);
                    CreateLabelAndTextBox("Specifications", "Характеристики", ref currentHeight, controlHeight, horizontalSpacing, ref maxWidth);
                    CreateLabelAndTextBox("SparePartsID", "Идентификатор запчасти", ref currentHeight, controlHeight, horizontalSpacing, ref maxWidth);
                    CreateLabelAndTextBox("WorkID", "Идентификатор работы", ref currentHeight, controlHeight, horizontalSpacing, ref maxWidth);
                    break;

                case "Techniques":
                    CreateLabelAndTextBox("TechniqueNAME", "Название техники", ref currentHeight, controlHeight, horizontalSpacing, ref maxWidth);
                    CreateLabelAndTextBox("TechniqueTypeOfFuel", "Тип топлива", ref currentHeight, controlHeight, horizontalSpacing, ref maxWidth);
                    CreateLabelAndTextBox("ManufacturersID", "Идентификатор производителя", ref currentHeight, controlHeight, horizontalSpacing, ref maxWidth);
                    CreateLabelAndTextBox("ModelID", "Идентификатор модели", ref currentHeight, controlHeight, horizontalSpacing, ref maxWidth);
                    break;
            }

            Button btnSave = new Button
            {
                Text = "Сохранить",
                Font = new Font("Times New Roman", 14),
                Size = new Size(150, 40),
                Location = new Point((maxWidth - 150) / 2, currentHeight + verticalSpacing)
            };
            btnSave.Click += Btn_Save_Click;

            Controls.Add(btnSave);
            this.ClientSize = new Size(maxWidth + horizontalSpacing * 2, btnSave.Bottom + verticalSpacing * 2);
        }

        private bool CheckForDuplicates(string tableName, Dictionary<string, string> data)
        {
            string whereClause = string.Join(" AND ", data.Select(d => $"{d.Key} = '{d.Value}'"));
            string query = $"SELECT COUNT(*) FROM {tableName} WHERE {whereClause}";

            BDConnect.sqlCommand.CommandText = query;
            int count = (int)BDConnect.sqlCommand.ExecuteScalar();

            return count > 0;
        }

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            foreach (Control ctrl in Controls)
            {
                if (ctrl is TextBox txt)
                {
                    string fieldKey = txt.Name.Replace("txt", "");
                    data[fieldKey] = txt.Text;
                }
            }

            if (data.Values.Any(value => string.IsNullOrEmpty(value)))
            {
                MessageBox.Show("Все поля должны быть заполнены.");
                return;
            }

            if (CheckForDuplicates(context, data))
            {
                MessageBox.Show("Запись с такими данными уже существует.");
                return;
            }

            InsertDataToDatabase(context, data);
            MessageBox.Show("Запись успешно добавлена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

            DialogResult = DialogResult.OK;
            Close();
        }

        private void InsertDataToDatabase(string context, Dictionary<string, string> data)
        {
            BDConnect.sqlCommand.CommandText = $"INSERT INTO {context} ({string.Join(",", data.Keys)}) VALUES ({string.Join(",", data.Keys.Select(k => "@" + k))})";
            BDConnect.sqlCommand.Parameters.Clear();

            foreach (var item in data)
            {
                object value = string.IsNullOrEmpty(item.Value) ? (object)DBNull.Value : item.Value;
                BDConnect.sqlCommand.Parameters.AddWithValue("@" + item.Key, value);
            }
            BDConnect.sqlCommand.ExecuteNonQuery();
        }
        private void Backssss_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
