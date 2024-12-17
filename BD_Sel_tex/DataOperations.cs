using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using System.Windows.Forms;

namespace BD_Sel_tex
{
    public class DataOperations
    {
        public void ExportToExcel(DataGridView dataGridView, string tableName)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
                saveFileDialog.FileName = $"{tableName}_Export.xlsx";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var workbook = new XLWorkbook();
                    var worksheet = workbook.Worksheets.Add(tableName);

                    // Добавляем заголовки
                    for (int col = 0; col < dataGridView.Columns.Count; col++)
                    {
                        worksheet.Cell(1, col + 1).Value = dataGridView.Columns[col].HeaderText;
                    }
                    for (int row = 0; row < dataGridView.Rows.Count; row++)
                    {
                        for (int col = 0; col < dataGridView.Columns.Count; col++)
                        {
                            worksheet.Cell(row + 2, col + 1).Value = dataGridView.Rows[row].Cells[col].Value.ToString();
                        }
                    }

                    workbook.SaveAs(saveFileDialog.FileName);
                    MessageBox.Show("Данные успешно экспортированы!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void ImportFromExcel(DataGridView dataGridView, string tableName)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var workbook = new XLWorkbook(openFileDialog.FileName);
                    var worksheet = workbook.Worksheet(1);
                    var dataTable = worksheet.RangeUsed().AsTable().AsNativeDataTable();

                    dataGridView.Rows.Clear();
                    foreach (DataRow row in dataTable.Rows)
                    {
                        int rowIndex = dataGridView.Rows.Add();
                        for (int col = 0; col < dataGridView.Columns.Count; col++)
                        {
                            dataGridView.Rows[rowIndex].Cells[col].Value = row[col].ToString();
                        }
                    }

                    MessageBox.Show("Данные успешно импортированы!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при импорте данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
