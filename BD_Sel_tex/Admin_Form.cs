using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.VariantTypes;
using DocumentFormat.OpenXml.Wordprocessing;
using OfficeOpenXml;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Xml.Linq;


namespace BD_Sel_tex
{
    public partial class Admin_Form : Form
    {
        public Admin_Form()
        {
            InitializeComponent();
        }
        int selectRow;
        enum RowState
        {
            Existed,
            New,
            Modified,
            ModifiedNew,
            Delete
        }
        private void ReadSingleRow(DataGridView dgv, IDataRecord record, RowState state)
        {
            var values = new object[record.FieldCount + 1];
            record.GetValues(values);
            values[values.Length - 1] = state;
            dgv.Rows.Add(values);
        }
        private void RefreshDataGrid(DataGridView dgv, string query)
        {
            try
            {
                dgv.Rows.Clear();
                BDConnect.sqlCommand.CommandText = query;
                using (var reader = BDConnect.sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ReadSingleRow(dgv, reader, RowState.Existed);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении таблицы: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CreateColumns(DataGridView dgv, params (string name, string header)[] columns)
        {
            dgv.Columns.Clear();

            foreach (var (name, header) in columns)
            {
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                column.Name = name;
                column.HeaderText = header;
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgv.Columns.Add(column);
            }

        }
        private void Admin_Form_Load(object sender, EventArgs e)
        {
            Btn_add_pr.Click += button_new_Click;
            Btn_add_model.Click += button_new_Click;
            Btn_add_zap.Click += button_new_Click;
            Btn_add_sel.Click += button_new_Click;

            Btn_edit_pr.Click += Btn_edit_Click;
            Btn_edit_model.Click += Btn_edit_Click;
            Btn_edit_zap.Click += Btn_edit_Click;
            Btn_edit_sel.Click += Btn_edit_Click;

            btnDelete_pr.Click += btnDelete_Click;
            btnDelete_model.Click += btnDelete_Click;
            btnDelete_zap.Click += btnDelete_Click;
            btnDelete_sel.Click += btnDelete_Click;

            Btn_refresh_pr.Click += Btn_refresh_Click;
            Btn_refresh_model.Click += Btn_refresh_Click;
            Btn_refresh_zap.Click += Btn_refresh_Click;
            Btn_refresh_sel.Click += Btn_refresh_Click;

            CreateColumnsForTechniques();
            RefreshDataGrid(dataGridView_tex, "SELECT TechniqueID, TechniqueNAME, TechniqueTypeOfFuel, ManufacturersID, ModelID FROM Techniques");

            CreateColumnsForManufacturers();
            RefreshDataGrid(dataGridView_Manufacturers, "SELECT ManufacturersID, ManufacturersNAME, Country FROM Manufacturers");

            CreateColumnsForModal();
            RefreshDataGrid(dataGridView_Model, "SELECT ModelID, ModelNAME, Specifications, SparePartsID, WorkID FROM Models");

            CreateColumnsForSpareParts();
            RefreshDataGrid(dataGridView_SpareParts, "SELECT SparePartsID, SparePartsNAME, Compatible FROM SpareParts");

            CreateColumnsForAgriculturalWorks();
            RefreshDataGrid(dataGridView_AgriculturalWorks, "SELECT WorkID, WorkTYPE, DateCompleted FROM AgriculturalWorks");
        }
        private void Change()
        {
            string tableName = "";
            DataGridView selectedGrid = null;
            List<string> columnValues = new List<string>();
            if (tabControl1.SelectedTab == tabPage1)
            {
                tableName = "Techniques";
                selectedGrid = dataGridView_tex;

                columnValues.Add(Txt_TechniqueId.Text);
                columnValues.Add(Txt_TechniqueNAM.Text);
                columnValues.Add(Txt_TechniqueTypeOfFuel.Text);
                columnValues.Add(Txt_ManufacturersID.Text);
                columnValues.Add(Txt_ModelID.Text);
            }
            else if (tabControl1.SelectedTab == tabPage2)
            {
                tableName = "Manufacturers";
                selectedGrid = dataGridView_Manufacturers;

                columnValues.Add(Txt_ManufacturerID.Text);
                columnValues.Add(Txt_Manufacturersname.Text);
                columnValues.Add(Txt_Countr.Text);
            }
            else if (tabControl1.SelectedTab == tabPage3)
            {
                tableName = "Models";
                selectedGrid = dataGridView_Model;

                columnValues.Add(Txt_ModelID.Text);
                columnValues.Add(Txt_ModelNAME.Text);
                columnValues.Add(Txt_Specifications.Text);
                columnValues.Add(Txt_SparePartID.Text);
                columnValues.Add(Txt_WorkID.Text);
            }
            else if (tabControl1.SelectedTab == tabPage4)
            {
                tableName = "SpareParts";
                selectedGrid = dataGridView_SpareParts;

                columnValues.Add(Txt_SparePartID.Text);
                columnValues.Add(Txt_SparePartsNAME.Text);
                columnValues.Add(Txt_Compatible.Text);
            }
            else if (tabControl1.SelectedTab == tabPage5)
            {
                tableName = "AgriculturalWorks";
                selectedGrid = dataGridView_AgriculturalWorks;

                columnValues.Add(Txt_WorkID.Text);
                columnValues.Add(Txt_WorkTYPE.Text);
                columnValues.Add(dateTimePicker1.Text);
            }

            if (string.IsNullOrEmpty(tableName) || selectedGrid == null)
            {
                MessageBox.Show("Не удалось определить текущую вкладку.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var selectedRowIndex = selectedGrid.CurrentCell?.RowIndex;
            if (selectedRowIndex == null || selectedRowIndex < 0)
            {
                MessageBox.Show("Не выбрана строка для редактирования.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(columnValues[0]))
            {
                MessageBox.Show("Ошибка: отсутствует значение первичного ключа.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            for (int i = 0; i < columnValues.Count; i++)
            {
                if (string.IsNullOrEmpty(columnValues[i]))
                {
                    columnValues[i] = selectedGrid.Rows[(int)selectedRowIndex].Cells[i].Value?.ToString() ?? string.Empty;
                }
            }
            string query = "UPDATE " + tableName + " SET ";
            for (int i = 1; i < columnValues.Count; i++)
            {
                query += $"{selectedGrid.Columns[i].Name} = @param{i}, ";
            }

            query = query.TrimEnd(',', ' ');
            query += " WHERE " + selectedGrid.Columns[0].Name + " = @param0";

            try
            {
                BDConnect.sqlCommand.CommandText = query;
                BDConnect.sqlCommand.Parameters.Clear();
                for (int i = 0; i < columnValues.Count; i++)
                {
                    if (int.TryParse(columnValues[i], out int intValue))
                    {
                        BDConnect.sqlCommand.Parameters.AddWithValue($"@param{i}", intValue);
                    }
                    else if (DateTime.TryParse(columnValues[i], out DateTime dateValue))
                    {
                        BDConnect.sqlCommand.Parameters.AddWithValue($"@param{i}", dateValue);
                    }
                    else
                    {
                        BDConnect.sqlCommand.Parameters.AddWithValue($"@param{i}", columnValues[i]);
                    }
                }

                BDConnect.sqlCommand.ExecuteNonQuery();

                MessageBox.Show("Запись успешно обновлена в базе данных.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления базы данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private string GetForeignKeyValue(string tableName, string columnName)
        {
            BDConnect.sqlCommand.CommandText = $"SELECT TOP 1 {columnName} FROM {tableName} ORDER BY {columnName} DESC";

            BDConnect.sqlCommand.Parameters.Clear();

            var result = BDConnect.sqlCommand.ExecuteScalar();

            return result != null ? result.ToString() : "NULL";
        }
        private Dictionary<string, string> GetFieldsForContext(string context)
        {
            if (context == "Manufacturers")
            {
                return new Dictionary<string, string>
        {
            { "ManufacturersID", Guid.NewGuid().ToString() }, // Замени на INT, если нужен реальный идентификатор
            { "ManufacturersNAME", "Название производителя" },
            { "Country", "Страна расположения производителя" }
        };
            }
            else if (context == "SpareParts")
            {
                return new Dictionary<string, string>
        {
            { "SparePartsID", Guid.NewGuid().ToString() }, // Замени на INT, если нужен реальный идентификатор
            { "SparePartsNAME", "Название запчасти" },
            { "Compatible", "Совместимость запчастей" }
        };
            }
            else if (context == "AgriculturalWorks")
            {
                return new Dictionary<string, string>
        {
            { "WorkID", Guid.NewGuid().ToString() }, // Замени на INT, если нужен реальный идентификатор
            { "WorkTYPE", "Тип работы" },
            { "DateCompleted", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }
        };
            }
            else if (context == "Models")
            {
                return new Dictionary<string, string>
        {
            { "ModelID", Guid.NewGuid().ToString() }, // Замени на INT, если нужен реальный идентификатор
            { "ModelNAME", "Название модели" },
            { "Specifications", "Характеристики модели" },
            { "SparePartsID", GetForeignKeyValue("SpareParts", "SparePartsID") },
            { "WorkID", GetForeignKeyValue("AgriculturalWorks", "WorkID") }
        };
            }
            else if (context == "Techniques")
            {
                return new Dictionary<string, string>
        {
            { "TechniqueID", Guid.NewGuid().ToString() }, // Замени на INT, если нужен реальный идентификатор
            { "TechniqueNAME", "Название техники" },
            { "TechniqueTypeOfFuel", "Тип топлива" },
            { "ManufacturersID", GetForeignKeyValue("Manufacturers", "ManufacturersID") },
            { "ModelID", GetForeignKeyValue("Models", "ModelID") }
        };
            }
            else
            {
                return new Dictionary<string, string>();
            }
        }
        private void RefreshDataGridForContext(string context)
        {
            string query = GetQueryForContext(context);

            if (string.IsNullOrEmpty(query))
            {
                MessageBox.Show("Неизвестный контекст");
                return;
            }

            DataGridView dgv = GetDataGridForContext(context);
            RefreshDataGrid(dgv, query);
        }
        private DataGridView GetDataGridForContext(string context)
        {
            switch (context)
            {
                case "Manufacturers":
                    return dataGridView_Manufacturers;
                case "SpareParts":
                    return dataGridView_SpareParts;
                case "AgriculturalWorks":
                    return dataGridView_AgriculturalWorks;
                case "Models":
                    return dataGridView_Model;
                case "Techniques":
                    return dataGridView_tex;
                default:
                    throw new InvalidOperationException("Неизвестный контекст для DataGridView");
            }
        }

        private string GetQueryForContext(string context)
        {
            switch (context)
            {
                case "Manufacturers":
                    return "SELECT * FROM Manufacturers";
                case "SpareParts":
                    return "SELECT * FROM SpareParts";
                case "AgriculturalWorks":
                    return "SELECT * FROM AgriculturalWorks";
                case "Models":
                    return "SELECT * FROM Models";
                case "Techniques":
                    return "SELECT * FROM Techniques";
                default:
                    return null;
            }
        }

        //-----------------Создание колонок-------------------------
        private void CreateColumnsForTechniques()
        {
            CreateColumns(dataGridView_tex,
                ("TechniqueID", "TechniqueID"),
                ("TechniqueNAME", "Название техники"),
                ("TechniqueTypeOfFuel", "Тип техники подачи топлива"),
                ("ManufacturersID", "ManufacturersID"),
                ("ModelID", "ModelID"));
        }
        private void CreateColumnsForManufacturers()
        {
            CreateColumns(dataGridView_Manufacturers,
                ("ManufacturersID", "ManufacturersID"),
                ("ManufacturersNAME", "Название производителя"),
                ("Country", "Город"));
        }
        private void CreateColumnsForModal()
        {
            CreateColumns(dataGridView_Model,
                ("ModelID", "ModelID"),
                ("ModelNAME", "Название модели"),
                ("Specifications", "Спецификкация"),
                ("SparePartsID", "SparePartsID"),
                ("WorkID", "WorkID"));
        }
        private void CreateColumnsForSpareParts()
        {
            CreateColumns(dataGridView_SpareParts,
                ("SparePartsID", "SparePartsID"),
                ("SparePartsNAME", "Название запчастей"),
                ("Compatible", "Совместимость"));
        }
        private void CreateColumnsForAgriculturalWorks()
        {
            CreateColumns(dataGridView_AgriculturalWorks,
                ("WorkID", "WorkID"),
                ("WorkTYPE", "Тип работы"),
                ("DateCompleted", "Дата выполнения"));
        }
        //----------------------------------------------------------
        private void dataGridView_tex_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView_tex.Rows[e.RowIndex];
                Txt_TechniqueId.Text = row.Cells["TechniqueID"].Value.ToString();
                Txt_TechniqueNAM.Text = row.Cells["TechniqueNAME"].Value.ToString();
                Txt_TechniqueTypeOfFuel.Text = row.Cells["TechniqueTypeOfFuel"].Value.ToString();
                Txt_ManufacturersID.Text = row.Cells["ManufacturersID"].Value.ToString();
                Txt_ModelID.Text = row.Cells["ModelID"].Value.ToString();

            }
        }
        private void dataGridView_Manufacturers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView_Manufacturers.Rows[e.RowIndex];
                Txt_ManufacturerID.Text = row.Cells["ManufacturersID"].Value.ToString();
                Txt_Manufacturersname.Text = row.Cells["ManufacturersNAME"].Value.ToString();
                Txt_Countr.Text = row.Cells["Country"].Value.ToString();
            }
        }
        private void dataGridView_Model_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView_Model.Rows[e.RowIndex];
                Txt_ModeID.Text = row.Cells["ModelID"].Value.ToString();
                Txt_ModelNAME.Text = row.Cells["ModelNAME"].Value.ToString();
                Txt_Specifications.Text = row.Cells["Specifications"].Value.ToString();
                Txt_SparePartsID.Text = row.Cells["SparePartsID"].Value.ToString();
                Txt_WorkID.Text = row.Cells["WorkID"].Value.ToString();
            }
        }
        private void dataGridView_SpareParts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView_SpareParts.Rows[e.RowIndex];
                Txt_SparePartID.Text = row.Cells["SparePartsID"].Value.ToString();
                Txt_SparePartsNAME.Text = row.Cells["SparePartsNAME"].Value.ToString();
                Txt_Compatible.Text = row.Cells["Compatible"].Value.ToString();
            }
        }
        private void dataGridView_AgriculturalWorks_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView_AgriculturalWorks.Rows[e.RowIndex];
                Txt_WorksID.Text = row.Cells["WorkID"].Value.ToString();
                Txt_WorkTYPE.Text = row.Cells["WorkTYPE"].Value.ToString();
                dateTimePicker1.Text = row.Cells["DateCompleted"].Value.ToString();
            }
        }
        //-----------------------------------------------------------------
        private void DeleteRow()
        {
            string context = "";
            DataGridView selectedGrid = null;

            if (tabControl1.SelectedTab == tabPage1)
            {
                context = "Techniques";
                selectedGrid = dataGridView_tex;
            }
            else if (tabControl1.SelectedTab == tabPage2)
            {
                context = "Manufacturers";
                selectedGrid = dataGridView_Manufacturers;
            }
            else if (tabControl1.SelectedTab == tabPage3)
            {
                context = "Models";
                selectedGrid = dataGridView_Model;
            }
            else if (tabControl1.SelectedTab == tabPage4)
            {
                context = "SpareParts";
                selectedGrid = dataGridView_SpareParts;
            }
            else if (tabControl1.SelectedTab == tabPage5)
            {
                context = "AgriculturalWorks";
                selectedGrid = dataGridView_AgriculturalWorks;
            }

            if (selectedGrid == null)
            {
                MessageBox.Show("Выбранный DataGridView отсутствует.");
                return;
            }

            if (selectedGrid.Rows.Count == 0)
            {
                MessageBox.Show("В таблице нет строк для удаления.");
                return;
            }

            if (selectedGrid.CurrentCell == null)
            {
                MessageBox.Show("Нет выделенной строки для удаления.");
                return;
            }

            int index = selectedGrid.CurrentCell.RowIndex;

            if (index < 0 || index >= selectedGrid.Rows.Count || selectedGrid.Rows[index].IsNewRow)
            {
                MessageBox.Show("Некорректный индекс строки или выбрана пустая строка.");
                return;
            }

            if (selectedGrid.Columns.Count == 0)
            {
                MessageBox.Show("В таблице нет столбцов.");
                return;
            }

            if (selectedGrid.Rows[index].Cells[0].Value == null || string.IsNullOrEmpty(selectedGrid.Rows[index].Cells[0].Value.ToString()))
            {
                MessageBox.Show("Значение в первой ячейке отсутствует.");
                return;
            }
            int intCellValue = Convert.ToInt32(selectedGrid.Rows[index].Cells[0].Value);
            if (selectedGrid.Columns.Count > 6)
            {
                selectedGrid.Rows[index].Cells[6].Value = RowState.Delete;
            }

            selectedGrid.Rows[index].Visible = false;
            DeleteRecordFromTable(context, intCellValue);

        }
        private void DeleteRecordFromTable(string tableName, int recordId)
        {

            Dictionary<string, string> tablePrimaryKeys = new Dictionary<string, string>
            {
                { "Techniques", "TechniqueID" },
                { "Manufacturers", "ManufacturerID" },
                { "Models", "ModelID" },
                { "SpareParts", "SparePartID" },
                { "AgriculturalWorks", "WorkID" }
            };

            if (recordId == -1)
            {
                MessageBox.Show("Запись не выбрана для удаления.");
                return;
            }
            if (!tablePrimaryKeys.ContainsKey(tableName))
            {
                MessageBox.Show($"Не найден первичный ключ для таблицы {tableName}.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string primaryKeyColumn = tablePrimaryKeys[tableName];

            try
            {
                string deleteQuery = $"DELETE FROM {tableName} WHERE {primaryKeyColumn} = {recordId}";
                BDConnect.sqlCommand.CommandText = deleteQuery;
                BDConnect.sqlCommand.ExecuteNonQuery();

                MessageBox.Show("Запись успешно удалена.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении записи: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Search(DataGridView dwg, string tableName, string searchText)
        {
            dwg.Rows.Clear();

            try
            {
                string query;
                if (string.IsNullOrWhiteSpace(searchText))
                {
                    query = $"SELECT * FROM {tableName}";
                }
                else
                {
                    var searchConditions = new List<string>();
                    foreach (DataGridViewColumn column in dwg.Columns)
                    {
                        searchConditions.Add($"CAST([{column.Name}] AS NVARCHAR(MAX)) LIKE @searchText");
                    }

                    query = $"SELECT * FROM {tableName} WHERE {string.Join(" OR ", searchConditions)}";
                }
                BDConnect.sqlCommand.CommandText = query;
                BDConnect.sqlCommand.Parameters.Clear();


                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    BDConnect.sqlCommand.Parameters.AddWithValue("@searchText", $"%{searchText}%");
                }
                using (SqlDataReader reader = BDConnect.sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int rowIndex = dwg.Rows.Add();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            dwg.Rows[rowIndex].Cells[i].Value = reader.GetValue(i);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка поиска: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void ImportFromExcelAndUpdateDB(DataGridView grid, string tableName)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Excel Files|*.xlsx;*.xls"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (var package = new ExcelPackage(new FileInfo(openFileDialog.FileName)))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        int rowCount = worksheet.Dimension.Rows;
                        int colCount = worksheet.Dimension.Columns;

                        grid.Rows.Clear(); 

                        for (int row = 2; row <= rowCount; row++)
                        {
                            int gridRowIndex = grid.Rows.Add();
                            List<string> values = new List<string>();

                            for (int col = 1; col <= colCount; col++)
                            {
                                object cellValue = worksheet.Cells[row, col].Value ?? string.Empty;
                                grid.Rows[gridRowIndex].Cells[col - 1].Value = cellValue.ToString();
                                values.Add(cellValue.ToString());
                            }

                            UpdateDatabaseRecord(tableName, values);
                        }

                        MessageBox.Show("Импорт завершён успешно!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка импорта: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private readonly Dictionary<string, string> tablePrimaryKeys = new Dictionary<string, string>
        {
            { "Techniques", "TechniqueID" },
            { "Manufacturers", "ManufacturerID" },
            { "Models", "ModelID" },
            { "SpareParts", "SparePartID" },
            { "AgriculturalWorks", "WorkID" }
        };
        private readonly Dictionary<string, List<string>> tableColumns = new Dictionary<string, List<string>>
        {
           { "Techniques", new List<string> { "TechniqueID", "TechniqueName", "TypeOfFuel", "ManufacturerID", "ModelID", "Year", "Condition" } },
           { "Manufacturers", new List<string> { "ManufacturerID", "ManufacturerName", "Country", "EstablishedYear", "Specialization" } },
           { "Models", new List<string> { "ModelID", "ModelName", "Specifications", "SparePartID", "WorkID", "ReleaseYear" } },
           { "SpareParts", new List<string> { "SparePartID", "SparePartName", "Compatibility", "Stock", "Price" } },
           { "AgriculturalWorks", new List<string> { "WorkID", "WorkType", "StartDate", "EndDate", "EquipmentID", "Description" } }

        };
        private string GenerateUpdateQuery(string tableName, List<string> values)
        {
            string primaryKey = tablePrimaryKeys[tableName];
            List<string> columns = tableColumns[tableName];

            string setClause = string.Join(", ", columns.Skip(1).Select((col, i) => $"{col} = @param{i + 1}"));

            return $"UPDATE {tableName} SET {setClause} WHERE {primaryKey} = @param0";
        }

        private string GenerateInsertQuery(string tableName, List<string> values)
        {
            List<string> columns = tableColumns[tableName];
            string columnNames = string.Join(", ", columns);
            string parameters = string.Join(", ", values.Select((_, i) => $"@param{i}"));

            return $"INSERT INTO {tableName} ({columnNames}) VALUES ({parameters})";
        }
        private void UpdateDatabaseRecord(string tableName, List<string> values)
        {
            try
            {
                string primaryKey = tablePrimaryKeys[tableName];
                string checkQuery = $"SELECT COUNT(*) FROM {tableName} WHERE {primaryKey} = @primaryKey";

                BDConnect.sqlCommand.CommandText = checkQuery;
                BDConnect.sqlCommand.Parameters.Clear();
                BDConnect.sqlCommand.Parameters.AddWithValue("@primaryKey", values[0]);

                int exists = (int)BDConnect.sqlCommand.ExecuteScalar();
                string query = exists > 0
                    ? GenerateUpdateQuery(tableName, values)
                    : GenerateInsertQuery(tableName, values);

                Console.WriteLine("Generated Query: " + query);

                BDConnect.sqlCommand.CommandText = query;
                BDConnect.sqlCommand.Parameters.Clear();

                int paramIndex = 0;

                for (int i = 0; i < values.Count; i++)
                {
                    if (values[i] != null && values[i] != string.Empty)
                    {
                        string paramName = $"@param{paramIndex}";
                        Console.WriteLine($"Adding Parameter: {paramName} = {values[i]}");
                        BDConnect.sqlCommand.Parameters.AddWithValue(paramName, values[i]);
                        paramIndex++;
                    }
                    else
                    {
                        string paramName = $"@param{paramIndex}";
                        Console.WriteLine($"Adding Parameter: {paramName} = NULL");
                        BDConnect.sqlCommand.Parameters.AddWithValue(paramName, DBNull.Value);
                        paramIndex++;
                    }
                }

                Console.WriteLine("Parameters:");
                foreach (SqlParameter param in BDConnect.sqlCommand.Parameters)
                {
                    Console.WriteLine($"{param.ParameterName} = {param.Value}");
                }

                BDConnect.sqlCommand.ExecuteNonQuery();

                MessageBox.Show(
                    exists > 0
                        ? "Запись успешно обновлена в базе данных."
                        : "Запись успешно добавлена в базу данных.",
                    "Успех",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления базы данных: {ex.Message}\n\n{ex.StackTrace}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataGridView GetSelectedGrid(TabControl tabControl)
        {
            if (tabControl.SelectedTab == tabPage1) return dataGridView_tex;
            if (tabControl.SelectedTab == tabPage2) return dataGridView_Manufacturers;
            if (tabControl.SelectedTab == tabPage3) return dataGridView_Model;
            if (tabControl.SelectedTab == tabPage4) return dataGridView_SpareParts;
            if (tabControl.SelectedTab == tabPage5) return dataGridView_AgriculturalWorks;
          
            return null; 
        }

        private string GetTableNameForTab(TabPage tabPage)
        {
            if (tabPage == tabPage1) return "TechniqueID";
            if (tabPage == tabPage2) return "Manufacturers";
            if (tabPage == tabPage3) return "Models";
            if (tabPage == tabPage4) return "SpareParts";
            if (tabPage == tabPage5) return "AgriculturalWorks";
            return string.Empty;
        }
        private void BackupDatabase(string databaseName, string backupFilePath)
        {
            try
            {

                string backupQuery = $@"
                BACKUP DATABASE [{databaseName}]
                TO DISK = @backupFilePath
                WITH FORMAT, INIT, NAME = 'Full Backup of {databaseName}', SKIP, NOREWIND, NOUNLOAD, STATS = 10";

                BDConnect.sqlCommand.CommandText = backupQuery;
                BDConnect.sqlCommand.Parameters.Clear();
                BDConnect.sqlCommand.Parameters.AddWithValue("@backupFilePath", backupFilePath);

                BDConnect.sqlCommand.ExecuteNonQuery();

                MessageBox.Show($"Резервное копирование базы данных '{databaseName}' успешно выполнено!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка резервного копирования: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //----------------------------------------------------------

        private void button_new_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                string context = "";

                if (clickedButton == Btn_add_pr)
                {
                    context = "Manufacturers";
                }
                else if (clickedButton == Btn_add_model)
                {
                    context = "Models";
                }
                else if (clickedButton == Btn_add_zap)
                {
                    context = "SpareParts";
                }
                else if (clickedButton == Btn_add_sel)
                {
                    context = "AgriculturalWorks";
                }
                else if (clickedButton == button_new)
                {

                    context = "Techniques";
                }


                Dictionary<string, string> fields = GetFieldsForContext(context);

                New_Forms fm = new New_Forms(context);
                if (fm.ShowDialog() == DialogResult.OK)
                {
                    RefreshDataGridForContext(context);
                }
            }


        }
        private void Btn_edit_Click(object sender, EventArgs e)
        {
            Change();
        }
        private void Btn_refresh_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                string query = "";
                DataGridView targetDataGridView = null;

                if (clickedButton == Btn_refresh)
                {
                    targetDataGridView = dataGridView_tex;
                    query = "SELECT * FROM Techniques";
                }
                else if (clickedButton == Btn_refresh_zap)
                {
                    targetDataGridView = dataGridView_Manufacturers;
                    query = "SELECT * FROM Manufacturers";
                }
                else if (clickedButton == Btn_refresh_model)
                {
                    targetDataGridView = dataGridView_Model;
                    query = "SELECT * FROM Models";
                }
                else if (clickedButton == Btn_refresh_pr)
                {
                    targetDataGridView = dataGridView_SpareParts;
                    query = "SELECT * FROM SpareParts";
                }
                else if (clickedButton == Btn_refresh_sel)
                {
                    targetDataGridView = dataGridView_AgriculturalWorks;
                    query = "SELECT * FROM AgriculturalWorks";
                }

                if (targetDataGridView != null && !string.IsNullOrEmpty(query))
                {
                    RefreshDataGrid(targetDataGridView, query);
                }
            }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteRow();
        }

        private void Txt_seach_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox == Txt_seach)
                Search(dataGridView_tex, "Techniques", textBox.Text.Trim());
            else if (textBox == Txt_seach_pr)
                Search(dataGridView_Manufacturers, "Manufacturers", textBox.Text.Trim());
            else if (textBox == Txt_seach_model)
                Search(dataGridView_Model, "Models", textBox.Text.Trim());
            else if (textBox == Txt_seach_zap)
                Search(dataGridView_SpareParts, "SpareParts", textBox.Text.Trim());
            else if (textBox == Txt_seach_sel)
                Search(dataGridView_AgriculturalWorks, "AgriculturalWorks", textBox.Text.Trim());
        }

        private void Btn_exp_Click(object sender, EventArgs e)
        {
            var dataOperations = new DataOperations();

            DataGridView selectedGrid = GetSelectedGrid(tabControl1);
            if (selectedGrid != null)
            {
                dataOperations.ExportToExcel(selectedGrid, GetTableNameForTab(tabControl1.SelectedTab));
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите вкладку с данными для экспорта.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_imp_Click(object sender, EventArgs e)
        {
            var dataOperations = new DataOperations();
            DataGridView selectedGrid = GetSelectedGrid(tabControl1);
            if (selectedGrid != null)
            {
                dataOperations.ImportFromExcel(selectedGrid, GetTableNameForTab(tabControl1.SelectedTab));
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите вкладку с данными для импорта.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_rez_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Backup files (*.bak)|*.bak",
                Title = "Сохранить резервную копию"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string backupPath = saveFileDialog.FileName;
                BackupDatabase("BD_Sel_tex", backupPath);
            }
        }
    }
}

