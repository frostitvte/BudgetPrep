using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.UI.WebControls;

namespace BudgetPrep.Classes
{
    public class ReportHelper
    {
        public ReportHelper()
        {
            rowLimit = Convert.ToInt32(WebConfigurationManager.AppSettings["ExcelRowsPerSheet"]);
        }

        private static int rowLimit = 60000;

        public DataTable ToDataTable<T>(IEnumerable<T> items)
        {
            // Create the result table, and gather all properties of a T        
            DataTable table = new DataTable(typeof(T).Name);
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Add the properties as columns to the datatable
            foreach (var prop in props)
            {
                Type propType = prop.PropertyType;

                // Is it a nullable type? Get the underlying type 
                if (propType.IsGenericType && propType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                    propType = new NullableConverter(propType).UnderlyingType;
                try
                {
                    table.Columns.Add(prop.Name, propType);
                }
                catch
                {
                    table.Columns.Add(prop.Name, typeof(string));
                }
            }

            // Add the property values per T as rows to the datatable
            foreach (var item in items)
            {
                var values = new object[props.Length];
                for (var i = 0; i < props.Length; i++)
                    values[i] = props[i].GetValue(item, null);

                table.Rows.Add(values);
            }

            return table;
        }

        public void DownloadSample<T>(ref HttpResponse response)
        {
            DataTable table = new DataTable(typeof(T).Name);
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                Type propType = prop.PropertyType;

                if (propType.IsGenericType && propType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                    propType = new NullableConverter(propType).UnderlyingType;

                table.Columns.Add(prop.Name, typeof(string));
            }

            var values = new object[props.Length];
            for (int i = 0; i < props.Count(); i++)
            {
                values[i] = string.Empty;
            }
            table.Rows.Add(values);

            ToExcel(table, ref response);
        }

        public DataSet ExcelToDataSet(string FilePath, string Extension)
        {
            string isHDR = "Yes";
            string conStr = "";
            switch (Extension)
            {
                case ".xls": conStr = ConfigurationManager.ConnectionStrings["ExcelConString1"].ConnectionString; //Excel 97-03
                    break;
                case ".xlsx": conStr = ConfigurationManager.ConnectionStrings["ExcelConString2"].ConnectionString; //Excel 07
                    break;
                case ".csv": conStr = ConfigurationManager.ConnectionStrings["CSVConString"].ConnectionString;  //.CSV
                    break;
            }

            if (Extension == ".csv")
            {
                string FolderPath = WebConfigurationManager.AppSettings["BudgetUploadFolderPath"];
                conStr = String.Format(conStr, FolderPath, isHDR);
            }
            else
            {
                conStr = String.Format(conStr, FilePath, isHDR);
            }

            OleDbConnection connExcel = new OleDbConnection(conStr);
            OleDbCommand cmdExcel = new OleDbCommand();
            OleDbDataAdapter oda = new OleDbDataAdapter();
            cmdExcel.Connection = connExcel;

            ////string ConnectionString = String.Format(@"provider=Microsoft.ACE.OLEDB.12.0;data source={0};Extended Properties=""Excel 12.0 Xml; IMEX=1; HDR=Yes;""", FilePath);
            ////OleDbConnection connExcel1 = new OleDbConnection(ConnectionString);
            ////connExcel1.Open();

            //Get the name of First Sheet
            connExcel.Open();
            DataTable dtExcelSchema;
            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

            DataSet ds = new DataSet();
            for (int i = 0; i < dtExcelSchema.Rows.Count; i++)
            {
                DataTable dt = new DataTable();
                string SheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                cmdExcel.CommandText = "SELECT * From [" + Path.GetFileName(FilePath) + "]";
                oda.SelectCommand = cmdExcel;
                oda.Fill(dt);
                ds.Tables.Add(dt);
            }
            connExcel.Close();

            return ds;
        }

        public DataTable Validate<T>(DataSet dsInput, String ShapeFormat, ref List<string> ListErrors)
        {
            //Validating Empty Columns
            DataTable dt = dsInput.Tables[0];
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            for (int i = dt.Columns.Count - 1; i >= 0; i--)
            {
                if (dt.Columns[i].ColumnName == string.Empty)
                {
                    dt.Columns.RemoveAt(i);
                }
                else if (props.Where(x => x.Name == dt.Columns[i].ColumnName).Count() == 0)
                {
                    return dt;
                }
            }

            //Validating values
            T obj = (T)Activator.CreateInstance(typeof(T));

            for (int c = 0; c < dt.Columns.Count; c++)
            {
                Type proptype = props.Where(x => x.Name == dt.Columns[c].ColumnName).FirstOrDefault().PropertyType;

                for (int r = 0; r < dt.Rows.Count; r++)
                {
                    if (typeof(T).Name.Contains("SegmentDetailImport")) //SegmentDetailImport
                    {
                        //validate empty row
                        if (!(proptype.IsGenericType && proptype.GetGenericTypeDefinition().Equals(typeof(Nullable<>))) && string.IsNullOrWhiteSpace(dt.Rows[r][c].ToString()))
                            ListErrors.Add("Cell value is Empty at Row:" + (r + 1).ToString() + ", Column:" + (c + 1).ToString());

                        //validate column = DetailCode
                        if (props[c].Name == "DetailCode")
                        {
                            if (!System.Text.RegularExpressions.Regex.IsMatch(dt.Rows[r][c].ToString(), @"^[a-zA-Z0-9]+$"))
                            {
                                ListErrors.Add("Invalid " + proptype.ToString() + " format at Row:" + (r + 1).ToString() + ", Column:" + (c + 1).ToString());
                            }
                            else
                            {
                                int MaxLenght = ShapeFormat.ToString().Length;

                                if (dt.Rows[r][c].ToString().Length > MaxLenght)
                                {
                                    ListErrors.Add("Length format exceeded at Row:" + (r + 1).ToString() + ", Column:" + (c + 1).ToString() + 
                                        ". Please check shape format from Segment Setup");
                                }
                            }
                        }
                    }
                    else if (typeof(T).Name.Contains("AccountCodeImport")) //AccountCodeImport
                    {
                        //validate empty row
                        if (props[c].Name != "UpperLevel")
                        {
                            if (!(proptype.IsGenericType && proptype.GetGenericTypeDefinition().Equals(typeof(Nullable<>))) && string.IsNullOrWhiteSpace(dt.Rows[r][c].ToString()))
                                ListErrors.Add("Cell value is Empty at Row:" + (r + 1).ToString() + ", Column:" + (c + 1).ToString());
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(dt.Rows[r][c].ToString()) | !System.Text.RegularExpressions.Regex.IsMatch(dt.Rows[r][c].ToString(), @"^[0-9]+$"))
                            {
                                dt.Rows[r][c] = "0";
                            }
                        }

                        //validate column = AccountCode
                        if (props[c].Name == "AccountCode")
                        {
                            if (!System.Text.RegularExpressions.Regex.IsMatch(dt.Rows[r][c].ToString(), @"^[a-zA-Z0-9]+$"))
                            {
                                ListErrors.Add("Invalid " + proptype.ToString() + " format at Row:" + (r + 1).ToString() + ", Column:" + (c + 1).ToString());
                            }
                        }

                        //validate column = AccountDesc
                        if (props[c].Name == "AccountDesc")
                        {
                            if (!System.Text.RegularExpressions.Regex.IsMatch(dt.Rows[r][c].ToString(), @"^[a-zA-Z\s]+$"))
                            {
                                ListErrors.Add("Invalid " + proptype.ToString() + " format at Row:" + (r + 1).ToString() + ", Column:" + (c + 1).ToString());
                            }
                        }
                    }
                    else if (typeof(T).Name.Contains("ServiceCodeImport")) //ServiceCodeImport
                    {
                        //validate empty row
                        if (props[c].Name != "UpperLevel")
                        {
                            if (!(proptype.IsGenericType && proptype.GetGenericTypeDefinition().Equals(typeof(Nullable<>))) && string.IsNullOrWhiteSpace(dt.Rows[r][c].ToString()))
                                ListErrors.Add("Cell value is Empty at Row:" + (r + 1).ToString() + ", Column:" + (c + 1).ToString());
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(dt.Rows[r][c].ToString()) | !System.Text.RegularExpressions.Regex.IsMatch(dt.Rows[r][c].ToString(), @"^[a-zA-Z0-9]+$"))
                            {
                                dt.Rows[r][c] = "0";
                            }
                        }

                        //validate column = ServiceCode
                        if (props[c].Name == "ServiceCode")
                        {
                            if (!System.Text.RegularExpressions.Regex.IsMatch(dt.Rows[r][c].ToString(), @"^[a-zA-Z0-9]+$"))
                            {
                                ListErrors.Add("Invalid " + proptype.ToString() + " format at Row:" + (r + 1).ToString() + ", Column:" + (c + 1).ToString());
                            }
                        }

                        //validate column = ServiceDesc
                        if (props[c].Name == "ServiceDesc")
                        {
                            if (!System.Text.RegularExpressions.Regex.IsMatch(dt.Rows[r][c].ToString(), @"^[a-zA-Z\s]+$"))
                            {
                                ListErrors.Add("Invalid " + proptype.ToString() + " format at Row:" + (r + 1).ToString() + ", Column:" + (c + 1).ToString());
                            }
                        }
                    }

                    //validate column Status - start
                    if (props[c].Name == "Status")
                    {
                        if (System.Text.RegularExpressions.Regex.IsMatch(dt.Rows[r][c].ToString(), @"^[0-9]+$"))
                        {
                            ListErrors.Add("Invalid " + proptype.ToString() + " format at Row:" + (r + 1).ToString() + ", Column:" + (c + 1).ToString());
                        }
                        else
                        {
                            if (dt.Rows[r][c].ToString() != "Active" && dt.Rows[r][c].ToString() != "Inactive")
                            {
                                ListErrors.Add("Please check Status at Row:" + (r + 1).ToString() + ", Column:" + (c + 1).ToString());
                            }
                        }
                    }
                    //validate column Status - end
                }
            }

            return dt;
        }

        //validation for Budget Mengurus&Perjawatan&Projek
        public DataTable ValidateBudgetImport<T>(DataSet dsInput, GridView gridviewitem, ref List<string> ListErrors)
        {
            DataTable dt = dsInput.Tables[0];
            Type objT = typeof(T);

            //assign table name - start
            if (objT.Name == "BudgetMengurusSetup")
                dt.TableName = "BudgetMengurus_CSV-File";
            else if (objT.Name == "BudgetProjekSetup")
                dt.TableName = "BudgetProjek_CSV-File";
            else if (objT.Name == "BudgetPerjawatanSetup")
                dt.TableName = "BudgetPerjawatan_CSV-File";
            //assign table name - end

            for (int c = 0; c < dt.Columns.Count; c++)
            {
                if (dt.Columns[c].ColumnName == string.Empty)
                {
                    ListErrors.Add("Column is Empty at Column:" + (c + 1).ToString());
                }
                else
                {
                    for (int r = 0; r < dt.Rows.Count; r++)
                    {
                        //validate empty row - start
                        if (string.IsNullOrWhiteSpace(dt.Rows[r][c].ToString()))
                            ListErrors.Add("Cell value is Empty at Row:" + (r + 1).ToString() + ", Column:" + (c + 1).ToString());
                        //validate empty row - end

                        //BudgetMengurusSetup && BudgetProjekSetup - AD
                        if (objT.Name == "BudgetMengurusSetup" || objT.Name == "BudgetProjekSetup")
                        {
                            //validate Numeric only
                            if (dt.Columns[c].ColumnName.Contains("Anggaran Dipohon"))
                            {
                                if (!System.Text.RegularExpressions.Regex.IsMatch(dt.Rows[r][c].ToString(), @"^-*[0-9,\.]+$"))
                                {
                                    ListErrors.Add("Invalid format at Row:" + (r + 1).ToString() + ", Column:" + (c + 1).ToString());
                                }
                            }
                        }
                        //BudgetPerjawatanSetup - BP
                        else if (objT.Name == "BudgetPerjawatanSetup")
                        {
                            //validate Numeric only
                            if (dt.Columns[c].ColumnName.Contains("Bil Perjawatan"))
                            {
                                if (!System.Text.RegularExpressions.Regex.IsMatch(dt.Rows[r][c].ToString(), @"^-*[0-9,\.]+$"))
                                {
                                    ListErrors.Add("Invalid format at Row:" + (r + 1).ToString() + ", Column:" + (c + 1).ToString());
                                }
                            }
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                }
            }

            //validate uploaded csv column with GridView column; need exacty match - start
            int index = 0;
            List<string> item = new List<string>();
            var result = dt.Columns.Cast<DataColumn>().Select(x => x.ToString()).ToList();

            foreach (DataControlField col in gridviewitem.Columns)
            {
                if (col.HeaderText == "Object")
                {
                    if (objT.Name == "BudgetMengurusSetup" || objT.Name == "BudgetProjekSetup")
                    {
                        if (result[index].ToString() != col.HeaderText && result[index].ToString() != "AccountCode")
                        {
                            ListErrors.Add("Invalid format at Column: " + (index + 1).ToString() + " - " + result[index].ToString());
                        }
                    }
                    else if (objT.Name == "BudgetPerjawatanSetup")
                    {
                        if (result[index].ToString() != col.HeaderText && result[index].ToString() != "ServiceCode")
                        {
                            ListErrors.Add("Invalid format at Column: " + (index + 1).ToString() + " - " + result[index].ToString());
                        }
                    }
                }
                else if (col.HeaderText == "Description")
                {
                    if (result[index].ToString() != col.HeaderText)
                    {
                        ListErrors.Add("Invalid format at Column: " + (index + 1).ToString() + " - " + result[index].ToString());
                    }
                }
                else
                {
                    if (col.HeaderText.Contains("Anggaran Dipohon"))
                    {
                        item.Add(col.HeaderText);
                    }
                    else if (col.HeaderText.Contains("Bil Perjawatan"))
                    {
                        string comparer = col.HeaderText;

                        if (!comparer.Contains("Kontrak") && !comparer.Contains("Tetap"))
                            item.Add(col.HeaderText);
                    }
                }

                index++;
            }

            //validate anggaran di pohon
            if (result.Count > 2)
            {
                if (item.Count > 0)
                {
                    int idx = 2; int colNum = 3;

                    foreach (string name in item)
                    {
                        if (name != result[idx].ToString())
                        {
                            ListErrors.Add("Invalid format at Column: " + (colNum).ToString() + " - " + result[idx].ToString());
                        }

                        if ((result.Count - 1) > idx)
                        {
                            idx++;
                        }
                        else if ((result.Count - 1) == idx)
                        {
                            break;
                        }
                        colNum++;
                    }
                }
            }

            HttpContext.Current.Session["BudgetCol"] = item;
            return dt;
        }

        public List<T> DataTableToList<T>(DataTable dtInput)
        {
            List<T> data = new List<T>();  

            foreach (DataRow row in dtInput.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }

            return data;
        }

        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }
 
        //Excel Helper Funtions
        public void ToExcel(DataTable dtInput, ref HttpResponse response)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(dtInput);
            ToExcel(ds, dtInput.TableName + ".xls", ref response);
        }

        public void ToExcel(DataSet dsInput, string filename, ref HttpResponse response)
        {
            string excelXml = GetExcelXml(dsInput, filename);
            response.Clear();
            //response.AppendHeader("Content-Type", "application/vnd.ms-excel");
            response.ContentType = "application/application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            response.AppendHeader("Content-disposition", ("attachment; filename=" + filename));

            response.Write(excelXml);
            response.Flush();
            response.End();
        }

        public string GetExcelXml(DataSet dsInput, string filename)
        {
            string excelTemplate = getWorkbookTemplate();
            string worksheets = getWorksheets(dsInput);
            string excelXml = string.Format(excelTemplate, worksheets);
            return excelXml;
        }

        private static string getWorkbookTemplate()
        {
            StringBuilder sb = new StringBuilder(818);
            sb.AppendFormat("<?xml version=\"1.0\"?>{0}", Environment.NewLine);
            sb.AppendFormat("<?mso-application progid=\"Excel.Sheet\"?>{0}", Environment.NewLine);
            sb.AppendFormat("<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"{0}", Environment.NewLine);
            sb.AppendFormat(" xmlns:o=\"urn:schemas-microsoft-com:office:office\"{0}", Environment.NewLine);
            sb.AppendFormat(" xmlns:x=\"urn:schemas-microsoft-com:office:excel\"{0}", Environment.NewLine);
            sb.AppendFormat(" xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\"{0}", Environment.NewLine);
            sb.AppendFormat(" xmlns:html=\"http://www.w3.org/TR/REC-html40\">{0}", Environment.NewLine);
            sb.AppendFormat(" <Styles>{0}", Environment.NewLine);
            sb.AppendFormat("  <Style ss:ID=\"Default\" ss:Name=\"Normal\">{0}", Environment.NewLine);
            sb.AppendFormat("   <Alignment ss:Vertical=\"Bottom\"/>{0}", Environment.NewLine);
            sb.AppendFormat("   <Borders/>{0}", Environment.NewLine);
            sb.AppendFormat("   <Font ss:FontName=\"Arial\" x:Family=\"Swiss\" ss:Size=\"8\" ss:Color=\"#000000\"/>{0}", Environment.NewLine);
            sb.AppendFormat("   <Interior/>{0}", Environment.NewLine);
            sb.AppendFormat("   <NumberFormat/>{0}", Environment.NewLine);
            sb.AppendFormat("   <Protection/>{0}", Environment.NewLine);
            sb.AppendFormat("  </Style>{0}", Environment.NewLine);
            sb.AppendFormat("  <Style ss:ID=\"s62\">{0}", Environment.NewLine);
            sb.AppendFormat("   <Font ss:FontName=\"Arial\" x:Family=\"Swiss\" ss:Size=\"8\" ss:Color=\"#000000\"{0}", Environment.NewLine);
            sb.AppendFormat("    ss:Bold=\"1\"/>{0}", Environment.NewLine);
            sb.AppendFormat("  </Style>{0}", Environment.NewLine);
            sb.AppendFormat("  <Style ss:ID=\"sBlue\">{0}", Environment.NewLine);
            sb.AppendFormat("   <Font ss:FontName=\"Arial\" x:Family=\"Swiss\" ss:Size=\"8\" ss:Color=\"#000000\"{0}", Environment.NewLine);
            sb.AppendFormat("    ss:Bold=\"1\"/>{0}", Environment.NewLine);
            sb.AppendFormat("   <Interior ss:Color=\"#77ddea\" ss:Pattern=\"Solid\"/>{0}", Environment.NewLine);
            sb.AppendFormat("  </Style>{0}", Environment.NewLine);
            sb.AppendFormat("  <Style ss:ID=\"s63\">{0}", Environment.NewLine);
            sb.AppendFormat("   <NumberFormat ss:Format=\"Short Date\"/>{0}", Environment.NewLine);
            sb.AppendFormat("  </Style>{0}", Environment.NewLine);
            sb.AppendFormat(" </Styles>{0}", Environment.NewLine);
            sb.Append("{0}\\r\\n</Workbook>");
            return sb.ToString();
        }

        private static string getWorksheets(DataSet source)
        {
            DataColumn dc = null;
            StringWriter sw = new StringWriter();
            if ((source == null) || source.Tables.Count == 0)
            {
                sw.Write("<Worksheet ss:Name=\"Sheet1\"><Table><Row><Cell><Data ss:Type=\"String\"></Data></Cell></Row></Table></Worksheet>");
                return sw.ToString();
            }
            foreach (DataTable dt in source.Tables)
            {
                if ((dt.Rows.Count == 0))
                {
                    sw.Write("<Worksheet ss:Name=\"" + replaceXmlChar(dt.TableName) + "\"><Table><Row><Cell  ss:StyleID=\"s62\"><Data ss:Type=\"String\"></Data></Cell></Row></Table></Worksheet>");
                }
                //write each row data                
                int sheetCount = 0;
                int i = 0;
                for (i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    if (i % rowLimit == 0)
                    {
                        if ((i / rowLimit) > sheetCount)
                        {
                            sw.Write("{0}</Table>{0}</Worksheet>", Environment.NewLine);
                            sheetCount = (i / rowLimit);
                        }
                        if ((i / rowLimit) == 0)
                        {
                            sw.Write("{0}<Worksheet ss:Name=\"" + replaceXmlChar(dt.TableName) + "\">{0}<Table>", Environment.NewLine);
                        }
                        else
                        {
                            sw.Write("{0}<Worksheet ss:Name=\"" + replaceXmlChar(dt.TableName) + (i / rowLimit).ToString() + "\">{0}<Table>", Environment.NewLine);
                        }
                        sw.Write("{0}<Row>", Environment.NewLine);
                        foreach (DataColumn dc_loopVariable in dt.Columns)
                        {
                            dc = dc_loopVariable; //ss:MergeAcross=\"2\"
                            sw.Write(string.Format("<Cell ss:StyleID=\"s62\"><Data ss:Type=\"String\">{0}</Data></Cell>", replaceXmlChar(dc.ColumnName)));
                        }
                        sw.Write("</Row>");
                    }
                    sw.Write("{0}<Row>", Environment.NewLine);
                    foreach (DataColumn dc_loopVariable in dt.Columns)
                    {
                        dc = dc_loopVariable;
                        sw.Write((getCell(dc.DataType, dt.Rows[i][dc.ColumnName])));
                    }
                    sw.Write("</Row>");
                }
                sw.Write("{0}</Table>{0}</Worksheet>", Environment.NewLine);
            }

            return sw.ToString();
        }

        private static string replaceXmlChar(string input)
        {
            input = input.Replace("&", "&amp");
            input = input.Replace("<", "&lt;");
            input = input.Replace(">", "&gt;");
            input = input.Replace("\\\"", "&quot;");
            input = input.Replace("'", "&apos;");
            return input;
        }

        private static string getCell(Type type, object cellData)
        {

            if (type.Name.Contains("Int") | type.Name.Contains("Double") | type.Name.Contains("Decimal"))
            {
                if (System.DBNull.Value.Equals(cellData) || string.IsNullOrEmpty(cellData.ToString()))
                {
                    return string.Format("<Cell><Data ss:Type=\"Number\">{0}</Data></Cell>", "0");
                }
                else
                {
                    return string.Format("<Cell><Data ss:Type=\"Number\">{0}</Data></Cell>", cellData.ToString());
                }
            }
            if (type.Name.Contains("Date"))
            {
                if (System.DBNull.Value.Equals(cellData) || string.IsNullOrEmpty(cellData.ToString()))
                {
                    //return string.Format("<Cell ss:StyleID=\"s63\"><Data ss:Type=\"DateTime\">{0}</Data></Cell>", "9999-12-31");
                    return string.Format("<Cell ss:StyleID=\"s63\"><Data ss:Type=\"String\">{0}</Data></Cell>", " ");
                }
                else
                {
                    return string.Format("<Cell ss:StyleID=\"s63\"><Data ss:Type=\"DateTime\">{0}</Data></Cell>", Convert.ToDateTime(cellData).ToString("yyyy-MM-dd"));
                }
            }
            if (System.DBNull.Value.Equals(cellData))
            {
                return string.Format("<Cell><Data ss:Type=\"String\">{0}</Data></Cell>", "");
            }
            else
            {
                return string.Format("<Cell><Data ss:Type=\"String\">{0}</Data></Cell>", replaceXmlChar(cellData.ToString()));
            }

        }
    }
}