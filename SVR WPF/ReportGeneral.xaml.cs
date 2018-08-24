using System;
using System.Windows;
using System.Windows.Controls;
using Word = Microsoft.Office.Interop.Word;
using System.Data.SqlServerCe;
using System.Reflection;


namespace SVR_WPF
{
    /// <summary>
    /// Interaction logic for ReportGeneral.xaml
    /// </summary>
    public partial class ReportGeneral : Window
    {
        public string period; //check
        public string syFrom; //check
        public string syTo; //check
        public string violationType; //check
        public string residence; //check
        public string violationName;
        public string cmbAcad, cmbDepart, cmbInsti;
        int i = 1;
        int row = 1, column = 9;
        string[] genRep = new string[10];

        private void btnSaveReport_Click(object sender, RoutedEventArgs e)
        {
            SqlCeConnection conn = DBUtils.GetDBConnection();
            object oMissing = Missing.Value;
            object oEndOfDoc = "\\endofdoc"; /* \endofdoc is a predefined bookmark */

            //Start Word and create a new document.
            Word._Application oWord;
            Word._Document oDoc;
            oWord = new Word.Application();
            oWord.Visible = true;
            oDoc = oWord.Documents.Add(ref oMissing, ref oMissing,
            ref oMissing, ref oMissing);

            Word.Paragraph oPara;
            oPara = oDoc.Content.Paragraphs.Add(ref oMissing);
            oPara.Range.Text = "Adamson Computer Science Department";
            oPara.Range.Font.Size = 18;
            oPara.Range.Font.Bold = 1;
            oPara.Format.SpaceAfter = 1;    //1 pt spacing after paragraph.
            oPara.Range.InsertParagraphAfter();

            //Insert a paragraph at the beginning of the document.
            Word.Paragraph oPara1;
            oPara1 = oDoc.Content.Paragraphs.Add(ref oMissing);
            if (violationName == null || violationName == "")
            {
                oPara1.Range.Text = "Violation Type: " + violationType;

            }
            else
            {
                oPara1.Range.Text = "Violation Type: " + violationType + "\nViolation Name: " + violationName;

            }
            oPara1.Range.Font.Size = 15;
            oPara1.Range.Font.Bold = 1;
            oPara1.Format.SpaceAfter = 1;    //1 pt spacing after paragraph.
            oPara1.Range.InsertParagraphAfter();

            //Insert a paragraph at the end of the document.
            Word.Paragraph oPara2;
            object oRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oPara2 = oDoc.Content.Paragraphs.Add(ref oRng);
            oPara2.Range.Text = "Period: " + period + "\nSY: " + syFrom + "-" + syTo;
            oPara1.Range.Font.Bold = 0;
            oPara2.Range.Font.Size = 14;

            oPara2.Format.SpaceAfter = 1;
            oPara2.Range.InsertParagraphAfter();

            //Insert another paragraph.
            Word.Paragraph oPara3;
            oRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oPara3 = oDoc.Content.Paragraphs.Add(ref oRng);
            oPara3.Range.Text = "Residence Status: " + residence;
            oPara3.Format.SpaceAfter = 24;
            oPara3.Range.Font.Size = 11;
            oPara3.Range.InsertParagraphAfter();

            //Insert a row x column table, fill it with data, and make the first row bold.
            Word.Table oTable;
            Word.Range wrdRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oTable = oDoc.Tables.Add(wrdRng, row, column, ref oMissing, ref oMissing);

            oTable.Range.ParagraphFormat.SpaceAfter = 6;
            int r, c;
            string strText;
            oTable.Cell(1, 1).Range.Text = "Record No.";
            oTable.Cell(1, 2).Range.Text = "Student No.";
            oTable.Cell(1, 3).Range.Text = "Full Name";
            oTable.Cell(1, 4).Range.Text = "Residence";
            oTable.Cell(1, 5).Range.Text = "Date Committed";
            oTable.Cell(1, 6).Range.Text = "Violation Code";
            oTable.Cell(1, 7).Range.Text = "Violation Type";
            oTable.Cell(1, 8).Range.Text = "Violation Name";
            oTable.Cell(1, 9).Range.Text = "Remarks";

            conn.Open();
            if (violationType == "Academic" && violationName == "ALL" && residence == "ALL" && period == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (SYint between " + syFrom + " and " + syTo + ") AND (ViolationType = 'Academic')", conn))
                {
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        if (reader.HasRows)
                        {
                            for (r = 2; r <= row; r++)
                            {
                                int i = 0;
                                reader.Read();
                                for (c = 1; c <= column; c++)
                                {
                                    //1
                                    int recordNo = Convert.ToInt32(reader.GetValue(0));
                                    //2
                                    int studNoIndex = reader.GetOrdinal("StudentNo");
                                    int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                    //3
                                    int fullNameIndex = reader.GetOrdinal("Full Name");
                                    string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                    //4
                                    int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                    string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                    //5
                                    int dateIndex = reader.GetOrdinal("dateCommitted");
                                    DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                    string date = myDate.ToString("MM/dd/yyyy");
                                    //6
                                    int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                    int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                    //7
                                    int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                    string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                    //8
                                    int violationNameIndex = reader.GetOrdinal("ViolationName");
                                    string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                    //9
                                    int remarksIndex = reader.GetOrdinal("remarks");
                                    string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                    genRep[0] = recordNo.ToString();
                                    genRep[1] = studNo.ToString();
                                    genRep[2] = fullName;
                                    genRep[3] = residence;
                                    genRep[4] = date;
                                    genRep[5] = violationCode.ToString();
                                    genRep[6] = violationType;
                                    genRep[7] = violationName;
                                    genRep[8] = remarks;
                                    strText = genRep[i];
                                    oTable.Cell(r, c).Range.Text = strText;
                                    i++;
                                }
                            }
                        }
                    }
                }
            }
            else if (violationType == "Academic" && violationName == "ALL" && residence == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (rd.Period = '" + period + "') AND (SYint between " + syFrom + " and " + syTo + ")  AND (ViolationType = 'Academic')", conn))
                {
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            if (reader.HasRows)
                            {
                                for (r = 2; r <= row; r++)
                                {
                                    int i = 0;
                                    reader.Read();
                                    for (c = 1; c <= column; c++)
                                    {
                                        //1
                                        int recordNo = Convert.ToInt32(reader.GetValue(0));
                                        //2
                                        int studNoIndex = reader.GetOrdinal("StudentNo");
                                        int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                        //3
                                        int fullNameIndex = reader.GetOrdinal("Full Name");
                                        string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                        //4
                                        int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                        string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                        //5
                                        int dateIndex = reader.GetOrdinal("dateCommitted");
                                        DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                        string date = myDate.ToString("MM/dd/yyyy");
                                        //6
                                        int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                        int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                        //7
                                        int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                        string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                        //8
                                        int violationNameIndex = reader.GetOrdinal("ViolationName");
                                        string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                        //9
                                        int remarksIndex = reader.GetOrdinal("remarks");
                                        string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                        genRep[0] = recordNo.ToString();
                                        genRep[1] = studNo.ToString();
                                        genRep[2] = fullName;
                                        genRep[3] = residence;
                                        genRep[4] = date;
                                        genRep[5] = violationCode.ToString();
                                        genRep[6] = violationType;
                                        genRep[7] = violationName;
                                        genRep[8] = remarks;
                                        strText = genRep[i];
                                        oTable.Cell(r, c).Range.Text = strText;
                                        i++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (violationType == "Academic" && violationName == "ALL" && period == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (SYint between " + syFrom + " and " + syTo + ")  AND (ViolationType = 'Academic') AND (si.ResidenceStatus = '" + residence + "')", conn))
                {
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            if (reader.HasRows)
                            {
                                for (r = 2; r <= row; r++)
                                {
                                    int i = 0;
                                    reader.Read();
                                    for (c = 1; c <= column; c++)
                                    {
                                        //1
                                        int recordNo = Convert.ToInt32(reader.GetValue(0));
                                        //2
                                        int studNoIndex = reader.GetOrdinal("StudentNo");
                                        int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                        //3
                                        int fullNameIndex = reader.GetOrdinal("Full Name");
                                        string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                        //4
                                        int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                        string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                        //5
                                        int dateIndex = reader.GetOrdinal("dateCommitted");
                                        DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                        string date = myDate.ToString("MM/dd/yyyy");
                                        //6
                                        int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                        int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                        //7
                                        int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                        string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                        //8
                                        int violationNameIndex = reader.GetOrdinal("ViolationName");
                                        string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                        //9
                                        int remarksIndex = reader.GetOrdinal("remarks");
                                        string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                        genRep[0] = recordNo.ToString();
                                        genRep[1] = studNo.ToString();
                                        genRep[2] = fullName;
                                        genRep[3] = residence;
                                        genRep[4] = date;
                                        genRep[5] = violationCode.ToString();
                                        genRep[6] = violationType;
                                        genRep[7] = violationName;
                                        genRep[8] = remarks;
                                        strText = genRep[i];
                                        oTable.Cell(r, c).Range.Text = strText;
                                        i++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (violationType == "Academic" && violationName == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (rd.Period = '" + period + "') AND (SYint between " + syFrom + " and " + syTo + ") AND (si.ResidenceStatus = '" + residence + "') AND (ViolationType = 'Academic')", conn))
                {
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            if (reader.HasRows)
                            {
                                for (r = 2; r <= row; r++)
                                {
                                    int i = 0;
                                    reader.Read();
                                    for (c = 1; c <= column; c++)
                                    {
                                        //1
                                        int recordNo = Convert.ToInt32(reader.GetValue(0));
                                        //2
                                        int studNoIndex = reader.GetOrdinal("StudentNo");
                                        int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                        //3
                                        int fullNameIndex = reader.GetOrdinal("Full Name");
                                        string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                        //4
                                        int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                        string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                        //5
                                        int dateIndex = reader.GetOrdinal("dateCommitted");
                                        DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                        string date = myDate.ToString("MM/dd/yyyy");
                                        //6
                                        int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                        int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                        //7
                                        int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                        string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                        //8
                                        int violationNameIndex = reader.GetOrdinal("ViolationName");
                                        string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                        //9
                                        int remarksIndex = reader.GetOrdinal("remarks");
                                        string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                        genRep[0] = recordNo.ToString();
                                        genRep[1] = studNo.ToString();
                                        genRep[2] = fullName;
                                        genRep[3] = residence;
                                        genRep[4] = date;
                                        genRep[5] = violationCode.ToString();
                                        genRep[6] = violationType;
                                        genRep[7] = violationName;
                                        genRep[8] = remarks;
                                        strText = genRep[i];
                                        oTable.Cell(r, c).Range.Text = strText;
                                        i++;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            else if (violationType == "Institutional" && violationName == "ALL" && residence == "ALL" && period == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (SYint between " + syFrom + " and " + syTo + ")  AND (ViolationType = 'Institutional')", conn))
                {
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            if (reader.HasRows)
                            {
                                for (r = 2; r <= row; r++)
                                {
                                    int i = 0;
                                    reader.Read();
                                    for (c = 1; c <= column; c++)
                                    {
                                        //1
                                        int recordNo = Convert.ToInt32(reader.GetValue(0));
                                        //2
                                        int studNoIndex = reader.GetOrdinal("StudentNo");
                                        int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                        //3
                                        int fullNameIndex = reader.GetOrdinal("Full Name");
                                        string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                        //4
                                        int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                        string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                        //5
                                        int dateIndex = reader.GetOrdinal("dateCommitted");
                                        DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                        string date = myDate.ToString("MM/dd/yyyy");
                                        //6
                                        int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                        int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                        //7
                                        int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                        string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                        //8
                                        int violationNameIndex = reader.GetOrdinal("ViolationName");
                                        string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                        //9
                                        int remarksIndex = reader.GetOrdinal("remarks");
                                        string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                        genRep[0] = recordNo.ToString();
                                        genRep[1] = studNo.ToString();
                                        genRep[2] = fullName;
                                        genRep[3] = residence;
                                        genRep[4] = date;
                                        genRep[5] = violationCode.ToString();
                                        genRep[6] = violationType;
                                        genRep[7] = violationName;
                                        genRep[8] = remarks;
                                        strText = genRep[i];
                                        oTable.Cell(r, c).Range.Text = strText;
                                        i++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (violationType == "Institutional" && violationName == "ALL" && residence == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (rd.Period = '" + period + "') AND (SYint between " + syFrom + " and " + syTo + ")  AND (ViolationType = 'Institutional')", conn))
                {
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            if (reader.HasRows)
                            {
                                for (r = 2; r <= row; r++)
                                {
                                    int i = 0;
                                    reader.Read();
                                    for (c = 1; c <= column; c++)
                                    {
                                        //1
                                        int recordNo = Convert.ToInt32(reader.GetValue(0));
                                        //2
                                        int studNoIndex = reader.GetOrdinal("StudentNo");
                                        int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                        //3
                                        int fullNameIndex = reader.GetOrdinal("Full Name");
                                        string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                        //4
                                        int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                        string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                        //5
                                        int dateIndex = reader.GetOrdinal("dateCommitted");
                                        DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                        string date = myDate.ToString("MM/dd/yyyy");
                                        //6
                                        int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                        int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                        //7
                                        int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                        string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                        //8
                                        int violationNameIndex = reader.GetOrdinal("ViolationName");
                                        string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                        //9
                                        int remarksIndex = reader.GetOrdinal("remarks");
                                        string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                        genRep[0] = recordNo.ToString();
                                        genRep[1] = studNo.ToString();
                                        genRep[2] = fullName;
                                        genRep[3] = residence;
                                        genRep[4] = date;
                                        genRep[5] = violationCode.ToString();
                                        genRep[6] = violationType;
                                        genRep[7] = violationName;
                                        genRep[8] = remarks;
                                        strText = genRep[i];
                                        oTable.Cell(r, c).Range.Text = strText;
                                        i++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (violationType == "Institutional" && violationName == "ALL" && period == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (SYint between " + syFrom + " and " + syTo + ")  AND (ViolationType = 'Institutional') AND (si.ResidenceStatus = '" + residence + "')", conn))
                {
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            if (reader.HasRows)
                            {
                                for (r = 2; r <= row; r++)
                                {
                                    int i = 0;
                                    reader.Read();
                                    for (c = 1; c <= column; c++)
                                    {
                                        //1
                                        int recordNo = Convert.ToInt32(reader.GetValue(0));
                                        //2
                                        int studNoIndex = reader.GetOrdinal("StudentNo");
                                        int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                        //3
                                        int fullNameIndex = reader.GetOrdinal("Full Name");
                                        string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                        //4
                                        int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                        string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                        //5
                                        int dateIndex = reader.GetOrdinal("dateCommitted");
                                        DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                        string date = myDate.ToString("MM/dd/yyyy");
                                        //6
                                        int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                        int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                        //7
                                        int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                        string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                        //8
                                        int violationNameIndex = reader.GetOrdinal("ViolationName");
                                        string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                        //9
                                        int remarksIndex = reader.GetOrdinal("remarks");
                                        string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                        genRep[0] = recordNo.ToString();
                                        genRep[1] = studNo.ToString();
                                        genRep[2] = fullName;
                                        genRep[3] = residence;
                                        genRep[4] = date;
                                        genRep[5] = violationCode.ToString();
                                        genRep[6] = violationType;
                                        genRep[7] = violationName;
                                        genRep[8] = remarks;
                                        strText = genRep[i];
                                        oTable.Cell(r, c).Range.Text = strText;
                                        i++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (violationType == "Institutional" && violationName == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (rd.Period = '" + period + "') AND (SYint between " + syFrom + " and " + syTo + ") AND (si.ResidenceStatus = '" + residence + "') AND (ViolationType = 'Institutional')", conn))
                {
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            if (reader.HasRows)
                            {
                                for (r = 2; r <= row; r++)
                                {
                                    int i = 0;
                                    reader.Read();
                                    for (c = 1; c <= column; c++)
                                    {
                                        //1
                                        int recordNo = Convert.ToInt32(reader.GetValue(0));
                                        //2
                                        int studNoIndex = reader.GetOrdinal("StudentNo");
                                        int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                        //3
                                        int fullNameIndex = reader.GetOrdinal("Full Name");
                                        string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                        //4
                                        int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                        string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                        //5
                                        int dateIndex = reader.GetOrdinal("dateCommitted");
                                        DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                        string date = myDate.ToString("MM/dd/yyyy");
                                        //6
                                        int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                        int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                        //7
                                        int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                        string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                        //8
                                        int violationNameIndex = reader.GetOrdinal("ViolationName");
                                        string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                        //9
                                        int remarksIndex = reader.GetOrdinal("remarks");
                                        string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                        genRep[0] = recordNo.ToString();
                                        genRep[1] = studNo.ToString();
                                        genRep[2] = fullName;
                                        genRep[3] = residence;
                                        genRep[4] = date;
                                        genRep[5] = violationCode.ToString();
                                        genRep[6] = violationType;
                                        genRep[7] = violationName;
                                        genRep[8] = remarks;
                                        strText = genRep[i];
                                        oTable.Cell(r, c).Range.Text = strText;
                                        i++;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            else if (violationType == "Departmental" && violationName == "ALL" && residence == "ALL" && period == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (SYint between 2015 and 2019)  AND (ViolationType = 'Departmental')", conn))
                {
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            if (reader.HasRows)
                            {
                                for (r = 2; r <= row; r++)
                                {
                                    int i = 0;
                                    reader.Read();
                                    for (c = 1; c <= column; c++)
                                    {
                                        //1
                                        int recordNo = Convert.ToInt32(reader.GetValue(0));
                                        //2
                                        int studNoIndex = reader.GetOrdinal("StudentNo");
                                        int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                        //3
                                        int fullNameIndex = reader.GetOrdinal("Full Name");
                                        string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                        //4
                                        int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                        string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                        //5
                                        int dateIndex = reader.GetOrdinal("dateCommitted");
                                        DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                        string date = myDate.ToString("MM/dd/yyyy");
                                        //6
                                        int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                        int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                        //7
                                        int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                        string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                        //8
                                        int violationNameIndex = reader.GetOrdinal("ViolationName");
                                        string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                        //9
                                        int remarksIndex = reader.GetOrdinal("remarks");
                                        string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                        genRep[0] = recordNo.ToString();
                                        genRep[1] = studNo.ToString();
                                        genRep[2] = fullName;
                                        genRep[3] = residence;
                                        genRep[4] = date;
                                        genRep[5] = violationCode.ToString();
                                        genRep[6] = violationType;
                                        genRep[7] = violationName;
                                        genRep[8] = remarks;
                                        strText = genRep[i];
                                        oTable.Cell(r, c).Range.Text = strText;
                                        i++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (violationType == "Departmental" && violationName == "ALL" && residence == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (rd.Period = '" + period + "') AND (SYint between " + syFrom + " and " + syTo + ")  AND (ViolationType = 'Departmental')", conn))
                {
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            if (reader.HasRows)
                            {
                                for (r = 2; r <= row; r++)
                                {
                                    int i = 0;
                                    reader.Read();
                                    for (c = 1; c <= column; c++)
                                    {
                                        //1
                                        int recordNo = Convert.ToInt32(reader.GetValue(0));
                                        //2
                                        int studNoIndex = reader.GetOrdinal("StudentNo");
                                        int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                        //3
                                        int fullNameIndex = reader.GetOrdinal("Full Name");
                                        string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                        //4
                                        int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                        string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                        //5
                                        int dateIndex = reader.GetOrdinal("dateCommitted");
                                        DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                        string date = myDate.ToString("MM/dd/yyyy");
                                        //6
                                        int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                        int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                        //7
                                        int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                        string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                        //8
                                        int violationNameIndex = reader.GetOrdinal("ViolationName");
                                        string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                        //9
                                        int remarksIndex = reader.GetOrdinal("remarks");
                                        string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                        genRep[0] = recordNo.ToString();
                                        genRep[1] = studNo.ToString();
                                        genRep[2] = fullName;
                                        genRep[3] = residence;
                                        genRep[4] = date;
                                        genRep[5] = violationCode.ToString();
                                        genRep[6] = violationType;
                                        genRep[7] = violationName;
                                        genRep[8] = remarks;
                                        strText = genRep[i];
                                        oTable.Cell(r, c).Range.Text = strText;
                                        i++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (violationType == "Departmental" && violationName == "ALL" && period == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (SYint between " + syFrom + " and " + syTo + ")  AND (ViolationType = 'Departmental') AND (si.ResidenceStatus = '" + residence + "')", conn))
                {
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {

                        for (r = 2; r <= row; r++)
                        {
                            int i = 0;
                            reader.Read();
                            for (c = 1; c <= column; c++)
                            {
                                //1
                                int recordNo = Convert.ToInt32(reader.GetValue(0));
                                //2
                                int studNoIndex = reader.GetOrdinal("StudentNo");
                                int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                //3
                                int fullNameIndex = reader.GetOrdinal("Full Name");
                                string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                //4
                                int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                //5
                                int dateIndex = reader.GetOrdinal("dateCommitted");
                                DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                string date = myDate.ToString("MM/dd/yyyy");
                                //6
                                int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                //7
                                int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                //8
                                int violationNameIndex = reader.GetOrdinal("ViolationName");
                                string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                //9
                                int remarksIndex = reader.GetOrdinal("remarks");
                                string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                genRep[0] = recordNo.ToString();
                                genRep[1] = studNo.ToString();
                                genRep[2] = fullName;
                                genRep[3] = residence;
                                genRep[4] = date;
                                genRep[5] = violationCode.ToString();
                                genRep[6] = violationType;
                                genRep[7] = violationName;
                                genRep[8] = remarks;
                                strText = genRep[i];
                                oTable.Cell(r, c).Range.Text = strText;
                                i++;
                            }
                        }
                    }

                }
            }
            else if (violationType == "Departmental" && violationName == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (rd.Period = '" + period + "') AND (SYint between " + syFrom + " and " + syTo + ") AND (si.ResidenceStatus = '" + residence + "') AND (ViolationType = 'Departmental')", conn))
                {
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            for (r = 2; r <= row; r++)
                            {
                                int i = 0;
                                reader.Read();
                                for (c = 1; c <= column; c++)
                                {
                                    //1
                                    int recordNo = Convert.ToInt32(reader.GetValue(0));
                                    //2
                                    int studNoIndex = reader.GetOrdinal("StudentNo");
                                    int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                    //3
                                    int fullNameIndex = reader.GetOrdinal("Full Name");
                                    string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                    //4
                                    int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                    string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                    //5
                                    int dateIndex = reader.GetOrdinal("dateCommitted");
                                    DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                    string date = myDate.ToString("MM/dd/yyyy");
                                    //6
                                    int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                    int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                    //7
                                    int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                    string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                    //8
                                    int violationNameIndex = reader.GetOrdinal("ViolationName");
                                    string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                    //9
                                    int remarksIndex = reader.GetOrdinal("remarks");
                                    string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                    genRep[0] = recordNo.ToString();
                                    genRep[1] = studNo.ToString();
                                    genRep[2] = fullName;
                                    genRep[3] = residence;
                                    genRep[4] = date;
                                    genRep[5] = violationCode.ToString();
                                    genRep[6] = violationType;
                                    genRep[7] = violationName;
                                    genRep[8] = remarks;
                                    strText = genRep[i];
                                    oTable.Cell(r, c).Range.Text = strText;
                                    i++;
                                }
                            }
                        }
                    }
                }
            }

            else if (period == "ALL" && violationType == "ALL" && residence == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (SYint between " + syFrom + " and " + syTo + ")", conn))
                {
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            for (r = 2; r <= row; r++)
                            {
                                int i = 0;
                                reader.Read();
                                for (c = 1; c <= column; c++)
                                {
                                    //1
                                    int recordNo = Convert.ToInt32(reader.GetValue(0));
                                    //2
                                    int studNoIndex = reader.GetOrdinal("StudentNo");
                                    int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                    //3
                                    int fullNameIndex = reader.GetOrdinal("Full Name");
                                    string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                    //4
                                    int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                    string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                    //5
                                    int dateIndex = reader.GetOrdinal("dateCommitted");
                                    DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                    string date = myDate.ToString("MM/dd/yyyy");
                                    //6
                                    int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                    int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                    //7
                                    int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                    string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                    //8
                                    int violationNameIndex = reader.GetOrdinal("ViolationName");
                                    string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                    //9
                                    int remarksIndex = reader.GetOrdinal("remarks");
                                    string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                    genRep[0] = recordNo.ToString();
                                    genRep[1] = studNo.ToString();
                                    genRep[2] = fullName;
                                    genRep[3] = residence;
                                    genRep[4] = date;
                                    genRep[5] = violationCode.ToString();
                                    genRep[6] = violationType;
                                    genRep[7] = violationName;
                                    genRep[8] = remarks;
                                    strText = genRep[i];
                                    oTable.Cell(r, c).Range.Text = strText;
                                    i++;
                                }
                            }
                        }
                    }
                }
            }
            else if (period == "ALL" && violationType == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (SYint between " + syFrom + " and " + syTo + ") AND (si.ResidenceStatus = '" + residence + "')", conn))
                {
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            for (r = 2; r <= row; r++)
                            {
                                int i = 0;
                                reader.Read();
                                for (c = 1; c <= column; c++)
                                {
                                    //1
                                    int recordNo = Convert.ToInt32(reader.GetValue(0));
                                    //2
                                    int studNoIndex = reader.GetOrdinal("StudentNo");
                                    int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                    //3
                                    int fullNameIndex = reader.GetOrdinal("Full Name");
                                    string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                    //4
                                    int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                    string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                    //5
                                    int dateIndex = reader.GetOrdinal("dateCommitted");
                                    DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                    string date = myDate.ToString("MM/dd/yyyy");
                                    //6
                                    int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                    int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                    //7
                                    int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                    string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                    //8
                                    int violationNameIndex = reader.GetOrdinal("ViolationName");
                                    string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                    //9
                                    int remarksIndex = reader.GetOrdinal("remarks");
                                    string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                    genRep[0] = recordNo.ToString();
                                    genRep[1] = studNo.ToString();
                                    genRep[2] = fullName;
                                    genRep[3] = residence;
                                    genRep[4] = date;
                                    genRep[5] = violationCode.ToString();
                                    genRep[6] = violationType;
                                    genRep[7] = violationName;
                                    genRep[8] = remarks;
                                    strText = genRep[i];
                                    oTable.Cell(r, c).Range.Text = strText;
                                    i++;
                                }

                            }
                        }
                    }
                }
            }
            else if (period == "ALL" && residence == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (vd.ViolationType = '" + violationType + "') AND (vd.ViolationName = '" + violationName + "') AND (SYint between " + syFrom + " and " + syTo + ")", conn))
                {
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        for (r = 2; r <= row; r++)
                        {
                            int i = 0;
                            reader.Read();
                            for (c = 1; c <= column; c++)
                            {
                                //1
                                int recordNo = Convert.ToInt32(reader.GetValue(0));
                                //2
                                int studNoIndex = reader.GetOrdinal("StudentNo");
                                int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                //3
                                int fullNameIndex = reader.GetOrdinal("Full Name");
                                string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                //4
                                int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                //5
                                int dateIndex = reader.GetOrdinal("dateCommitted");
                                DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                string date = myDate.ToString("MM/dd/yyyy");
                                //6
                                int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                //7
                                int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                //8
                                int violationNameIndex = reader.GetOrdinal("ViolationName");
                                string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                //9
                                int remarksIndex = reader.GetOrdinal("remarks");
                                string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                genRep[0] = recordNo.ToString();
                                genRep[1] = studNo.ToString();
                                genRep[2] = fullName;
                                genRep[3] = residence;
                                genRep[4] = date;
                                genRep[5] = violationCode.ToString();
                                genRep[6] = violationType;
                                genRep[7] = violationName;
                                genRep[8] = remarks;
                                strText = genRep[i];
                                oTable.Cell(r, c).Range.Text = strText;
                                i++;
                            }

                        }
                    }
                }
            } //

            else if (violationType == "ALL" && residence == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (rd.Period = '" + period + "') AND (SYint between " + syFrom + " and " + syTo + ")", conn))
                {
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        for (r = 2; r <= row; r++)
                        {
                            int i = 0;
                            reader.Read();
                            for (c = 1; c <= column; c++)
                            {
                                //1
                                int recordNo = Convert.ToInt32(reader.GetValue(0));
                                //2
                                int studNoIndex = reader.GetOrdinal("StudentNo");
                                int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                //3
                                int fullNameIndex = reader.GetOrdinal("Full Name");
                                string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                //4
                                int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                //5
                                int dateIndex = reader.GetOrdinal("dateCommitted");
                                DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                string date = myDate.ToString("MM/dd/yyyy");
                                //6
                                int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                //7
                                int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                //8
                                int violationNameIndex = reader.GetOrdinal("ViolationName");
                                string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                //9
                                int remarksIndex = reader.GetOrdinal("remarks");
                                string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                genRep[0] = recordNo.ToString();
                                genRep[1] = studNo.ToString();
                                genRep[2] = fullName;
                                genRep[3] = residence;
                                genRep[4] = date;
                                genRep[5] = violationCode.ToString();
                                genRep[6] = violationType;
                                genRep[7] = violationName;
                                genRep[8] = remarks;
                                strText = genRep[i];
                                oTable.Cell(r, c).Range.Text = strText;
                                i++;
                            }
                        }
                    }
                }
            }
            else if (violationType == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (rd.Period = '" + period + "') AND (SYint between " + syFrom + " and " + syTo + ") AND (si.ResidenceStatus = '" + residence + "')", conn))
                {
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        for (r = 2; r <= row; r++)
                        {
                            int i = 0;
                            reader.Read();
                            for (c = 1; c <= column; c++)
                            {
                                //1
                                int recordNo = Convert.ToInt32(reader.GetValue(0));
                                //2
                                int studNoIndex = reader.GetOrdinal("StudentNo");
                                int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                //3
                                int fullNameIndex = reader.GetOrdinal("Full Name");
                                string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                //4
                                int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                //5
                                int dateIndex = reader.GetOrdinal("dateCommitted");
                                DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                string date = myDate.ToString("MM/dd/yyyy");
                                //6
                                int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                //7
                                int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                //8
                                int violationNameIndex = reader.GetOrdinal("ViolationName");
                                string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                //9
                                int remarksIndex = reader.GetOrdinal("remarks");
                                string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                genRep[0] = recordNo.ToString();
                                genRep[1] = studNo.ToString();
                                genRep[2] = fullName;
                                genRep[3] = residence;
                                genRep[4] = date;
                                genRep[5] = violationCode.ToString();
                                genRep[6] = violationType;
                                genRep[7] = violationName;
                                genRep[8] = remarks;
                                strText = genRep[i];
                                oTable.Cell(r, c).Range.Text = strText;
                                i++;
                            }
                        }
                    }
                }
            }
            else if (period == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (vd.ViolationType = '" + violationType + "') AND (vd.ViolationName = '" + violationName + "') AND (SYint between " + syFrom + " and " + syTo + ") AND (si.ResidenceStatus = '" + residence + "')", conn))
                {
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        for (r = 2; r <= row; r++)
                        {
                            int i = 0;
                            reader.Read();
                            for (c = 1; c <= column; c++)
                            {
                                //1
                                int recordNo = Convert.ToInt32(reader.GetValue(0));
                                //2
                                int studNoIndex = reader.GetOrdinal("StudentNo");
                                int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                //3
                                int fullNameIndex = reader.GetOrdinal("Full Name");
                                string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                //4
                                int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                //5
                                int dateIndex = reader.GetOrdinal("dateCommitted");
                                DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                string date = myDate.ToString("MM/dd/yyyy");
                                //6
                                int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                //7
                                int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                //8
                                int violationNameIndex = reader.GetOrdinal("ViolationName");
                                string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                //9
                                int remarksIndex = reader.GetOrdinal("remarks");
                                string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                genRep[0] = recordNo.ToString();
                                genRep[1] = studNo.ToString();
                                genRep[2] = fullName;
                                genRep[3] = residence;
                                genRep[4] = date;
                                genRep[5] = violationCode.ToString();
                                genRep[6] = violationType;
                                genRep[7] = violationName;
                                genRep[8] = remarks;
                                strText = genRep[i];
                                oTable.Cell(r, c).Range.Text = strText;
                                i++;
                            }
                        }
                    }
                }
            } //
            else if (residence == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (vd.ViolationType = '" + violationType + "') AND (rd.Period = '" + period + "') AND (SYint between " + syFrom + " and " + syTo + ")", conn))
                {
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        for (r = 2; r <= row; r++)
                        {
                            int i = 0;
                            reader.Read();
                            for (c = 1; c <= column; c++)
                            {
                                //1
                                int recordNo = Convert.ToInt32(reader.GetValue(0));
                                //2
                                int studNoIndex = reader.GetOrdinal("StudentNo");
                                int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                //3
                                int fullNameIndex = reader.GetOrdinal("Full Name");
                                string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                //4
                                int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                //5
                                int dateIndex = reader.GetOrdinal("dateCommitted");
                                DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                string date = myDate.ToString("MM/dd/yyyy");
                                //6
                                int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                //7
                                int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                //8
                                int violationNameIndex = reader.GetOrdinal("ViolationName");
                                string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                //9
                                int remarksIndex = reader.GetOrdinal("remarks");
                                string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                genRep[0] = recordNo.ToString();
                                genRep[1] = studNo.ToString();
                                genRep[2] = fullName;
                                genRep[3] = residence;
                                genRep[4] = date;
                                genRep[5] = violationCode.ToString();
                                genRep[6] = violationType;
                                genRep[7] = violationName;
                                genRep[8] = remarks;
                                strText = genRep[i];
                                oTable.Cell(r, c).Range.Text = strText;
                                i++;
                            }
                        }
                    }
                }
            }
            else
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (vd.ViolationType = '" + violationType + "') AND (vd.ViolationName = '" + violationName + "') AND (rd.Period = '" + period + "') AND (SYint between " + syFrom + " and " + syTo + ") AND (si.ResidenceStatus = '" + residence + "')", conn))
                {
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        for (r = 2; r <= row; r++)
                        {
                            int i = 0;
                            reader.Read();
                            for (c = 1; c <= column; c++)
                            {
                                //1
                                int recordNo = Convert.ToInt32(reader.GetValue(0));
                                //2
                                int studNoIndex = reader.GetOrdinal("StudentNo");
                                int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                //3
                                int fullNameIndex = reader.GetOrdinal("Full Name");
                                string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                //4
                                int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                //5
                                int dateIndex = reader.GetOrdinal("dateCommitted");
                                DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                string date = myDate.ToString("MM/dd/yyyy");
                                //6
                                int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                //7
                                int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                //8
                                int violationNameIndex = reader.GetOrdinal("ViolationName");
                                string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                //9
                                int remarksIndex = reader.GetOrdinal("remarks");
                                string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                genRep[0] = recordNo.ToString();
                                genRep[1] = studNo.ToString();
                                genRep[2] = fullName;
                                genRep[3] = residence;
                                genRep[4] = date;
                                genRep[5] = violationCode.ToString();
                                genRep[6] = violationType;
                                genRep[7] = violationName;
                                genRep[8] = remarks;
                                strText = genRep[i];
                                oTable.Cell(r, c).Range.Text = strText;
                                i++;

                            }
                        }
                    }
                }
            }
            oTable.Rows[1].Range.Font.Bold = 1;
            conn.Close();
            wrdRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;

            object noReset = false;
            object password = System.String.Empty;
            object useIRM = false;
            object enforceStyleLock = false;
            oDoc.Protect(Word.WdProtectionType.wdAllowOnlyReading, ref noReset, ref password, ref useIRM, ref enforceStyleLock);
            oDoc.Save();
        }

        public ReportGeneral(string period, string syFrom, string syTo, string violationName, string violationType, string residence)
        {
            this.period = period;
            this.syFrom = syFrom;
            this.syTo = syTo;
            this.violationName = violationName;
            this.violationType = violationType;
            this.residence = residence;
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            lblSem.Content = period;
            lblyrStart.Content = syFrom;
            lblyrEnd.Content = syTo;
            updateListView();
        }

        private void updateListView()
        {
            SqlCeConnection conn = DBUtils.GetDBConnection();
            conn.Open();
            if (violationType == "Academic" && violationName == "ALL" && residence == "ALL" && period == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (SYint between " + syFrom + " and " + syTo + ") AND (ViolationType = 'Academic')", conn))
                {
                    lvGenReport.Items.Clear();
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            while (reader.Read())
                            {
                                //1
                                int recordNo = Convert.ToInt32(reader.GetValue(0));
                                //2
                                int studNoIndex = reader.GetOrdinal("StudentNo");
                                int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                //3
                                int fullNameIndex = reader.GetOrdinal("Full Name");
                                string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                //4
                                int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                //5
                                int dateIndex = reader.GetOrdinal("dateCommitted");
                                DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                string date = myDate.ToString("MM/dd/yyyy");
                                //6
                                int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                //7
                                int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                //8
                                int violationNameIndex = reader.GetOrdinal("ViolationName");
                                string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                //9
                                int remarksIndex = reader.GetOrdinal("remarks");
                                string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                genRep[0] = i.ToString();
                                genRep[1] = recordNo.ToString();
                                genRep[2] = studNo.ToString();
                                genRep[3] = fullName;
                                genRep[4] = residence;
                                genRep[5] = date;
                                genRep[6] = violationCode.ToString();
                                genRep[7] = violationType;
                                genRep[8] = violationName;
                                genRep[9] = remarks;
                                lvGenReport.Items.Add(new ListViewGenReport
                                {
                                    i = this.i,
                                    recordNo = int.Parse(genRep[1]),
                                    studNo = genRep[2],
                                    fullName = genRep[3],
                                    residence = genRep[4],
                                    dateCommitted = genRep[5],
                                    violationCode = genRep[6],
                                    violationType = genRep[7],
                                    violationName = genRep[8],
                                    remarks = genRep[9]
                                });
                                row++;
                                i++;
                            }
                        }
                    }
                }
            }
            else if (violationType == "Academic" && violationName == "ALL" && residence == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (rd.Period = '" + period + "') AND (SYint between " + syFrom + " and " + syTo + ")  AND (ViolationType = 'Academic')", conn))
                {
                    lvGenReport.Items.Clear();
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            while (reader.Read())
                            {
                                //1
                                int recordNo = Convert.ToInt32(reader.GetValue(0));
                                //2
                                int studNoIndex = reader.GetOrdinal("StudentNo");
                                int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                //3
                                int fullNameIndex = reader.GetOrdinal("Full Name");
                                string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                //4
                                int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                //5
                                int dateIndex = reader.GetOrdinal("dateCommitted");
                                DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                string date = myDate.ToString("MM/dd/yyyy");
                                //6
                                int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                //7
                                int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                //8
                                int violationNameIndex = reader.GetOrdinal("ViolationName");
                                string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                //9
                                int remarksIndex = reader.GetOrdinal("remarks");
                                string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                genRep[0] = i.ToString();
                                genRep[1] = recordNo.ToString();
                                genRep[2] = studNo.ToString();
                                genRep[3] = fullName;
                                genRep[4] = residence;
                                genRep[5] = date;
                                genRep[6] = violationCode.ToString();
                                genRep[7] = violationType;
                                genRep[8] = violationName;
                                genRep[9] = remarks;
                                lvGenReport.Items.Add(new ListViewGenReport
                                {
                                    i = this.i,
                                    recordNo = int.Parse(genRep[1]),
                                    studNo = genRep[2],
                                    fullName = genRep[3],
                                    residence = genRep[4],
                                    dateCommitted = genRep[5],
                                    violationCode = genRep[6],
                                    violationType = genRep[7],
                                    violationName = genRep[8],
                                    remarks = genRep[9]
                                });
                                row++;
                                i++;
                            }
                        }
                    }
                }
            }
            else if (violationType == "Academic" && violationName == "ALL" && period == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (SYint between " + syFrom + " and " + syTo + ")  AND (ViolationType = 'Academic') AND (si.ResidenceStatus = '" + residence + "')", conn))
                {
                    lvGenReport.Items.Clear();
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            while (reader.Read())
                            {
                                //1
                                int recordNo = Convert.ToInt32(reader.GetValue(0));
                                //2
                                int studNoIndex = reader.GetOrdinal("StudentNo");
                                int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                //3
                                int fullNameIndex = reader.GetOrdinal("Full Name");
                                string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                //4
                                int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                //5
                                int dateIndex = reader.GetOrdinal("dateCommitted");
                                DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                string date = myDate.ToString("MM/dd/yyyy");
                                //6
                                int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                //7
                                int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                //8
                                int violationNameIndex = reader.GetOrdinal("ViolationName");
                                string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                //9
                                int remarksIndex = reader.GetOrdinal("remarks");
                                string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                genRep[0] = i.ToString();
                                genRep[1] = recordNo.ToString();
                                genRep[2] = studNo.ToString();
                                genRep[3] = fullName;
                                genRep[4] = residence;
                                genRep[5] = date;
                                genRep[6] = violationCode.ToString();
                                genRep[7] = violationType;
                                genRep[8] = violationName;
                                genRep[9] = remarks;
                                lvGenReport.Items.Add(new ListViewGenReport
                                {
                                    i = this.i,
                                    recordNo = int.Parse(genRep[1]),
                                    studNo = genRep[2],
                                    fullName = genRep[3],
                                    residence = genRep[4],
                                    dateCommitted = genRep[5],
                                    violationCode = genRep[6],
                                    violationType = genRep[7],
                                    violationName = genRep[8],
                                    remarks = genRep[9]
                                });
                                row++;
                                i++;
                            }
                        }
                    }
                }
            }
            else if (violationType == "Academic" && violationName == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (rd.Period = '" + period + "') AND (SYint between " + syFrom + " and " + syTo + ") AND (si.ResidenceStatus = '" + residence + "') AND (ViolationType = 'Academic')", conn))
                {
                    lvGenReport.Items.Clear();
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            while (reader.Read())
                            {
                                //1
                                int recordNo = Convert.ToInt32(reader.GetValue(0));
                                //2
                                int studNoIndex = reader.GetOrdinal("StudentNo");
                                int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                //3
                                int fullNameIndex = reader.GetOrdinal("Full Name");
                                string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                //4
                                int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                //5
                                int dateIndex = reader.GetOrdinal("dateCommitted");
                                DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                string date = myDate.ToString("MM/dd/yyyy");
                                //6
                                int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                //7
                                int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                //8
                                int violationNameIndex = reader.GetOrdinal("ViolationName");
                                string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                //9
                                int remarksIndex = reader.GetOrdinal("remarks");
                                string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                genRep[0] = i.ToString();
                                genRep[1] = recordNo.ToString();
                                genRep[2] = studNo.ToString();
                                genRep[3] = fullName;
                                genRep[4] = residence;
                                genRep[5] = date;
                                genRep[6] = violationCode.ToString();
                                genRep[7] = violationType;
                                genRep[8] = violationName;
                                genRep[9] = remarks;
                                lvGenReport.Items.Add(new ListViewGenReport
                                {
                                    i = this.i,
                                    recordNo = int.Parse(genRep[1]),
                                    studNo = genRep[2],
                                    fullName = genRep[3],
                                    residence = genRep[4],
                                    dateCommitted = genRep[5],
                                    violationCode = genRep[6],
                                    violationType = genRep[7],
                                    violationName = genRep[8],
                                    remarks = genRep[9]
                                });
                                row++;
                                i++;
                            }
                        }
                    }
                }
            }

            else if (violationType == "Institutional" && violationName == "ALL" && residence == "ALL" && period == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (SYint between " + syFrom + " and " + syTo + ")  AND (ViolationType = 'Institutional')", conn))
                {
                    lvGenReport.Items.Clear();
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            while (reader.Read())
                            {
                                //1
                                int recordNo = Convert.ToInt32(reader.GetValue(0));
                                //2
                                int studNoIndex = reader.GetOrdinal("StudentNo");
                                int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                //3
                                int fullNameIndex = reader.GetOrdinal("Full Name");
                                string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                //4
                                int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                //5
                                int dateIndex = reader.GetOrdinal("dateCommitted");
                                DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                string date = myDate.ToString("MM/dd/yyyy");
                                //6
                                int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                //7
                                int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                //8
                                int violationNameIndex = reader.GetOrdinal("ViolationName");
                                string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                //9
                                int remarksIndex = reader.GetOrdinal("remarks");
                                string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                genRep[0] = i.ToString();
                                genRep[1] = recordNo.ToString();
                                genRep[2] = studNo.ToString();
                                genRep[3] = fullName;
                                genRep[4] = residence;
                                genRep[5] = date;
                                genRep[6] = violationCode.ToString();
                                genRep[7] = violationType;
                                genRep[8] = violationName;
                                genRep[9] = remarks;
                                lvGenReport.Items.Add(new ListViewGenReport
                                {
                                    i = this.i,
                                    recordNo = int.Parse(genRep[1]),
                                    studNo = genRep[2],
                                    fullName = genRep[3],
                                    residence = genRep[4],
                                    dateCommitted = genRep[5],
                                    violationCode = genRep[6],
                                    violationType = genRep[7],
                                    violationName = genRep[8],
                                    remarks = genRep[9]
                                });
                                row++;
                                i++;
                            }
                        }
                    }
                }
            }
            else if (violationType == "Institutional" && violationName == "ALL" && residence == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (rd.Period = '" + period + "') AND (SYint between " + syFrom + " and " + syTo + ")  AND (ViolationType = 'Institutional')", conn))
                {
                    lvGenReport.Items.Clear();
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            while (reader.Read())
                            {
                                //1
                                int recordNo = Convert.ToInt32(reader.GetValue(0));
                                //2
                                int studNoIndex = reader.GetOrdinal("StudentNo");
                                int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                //3
                                int fullNameIndex = reader.GetOrdinal("Full Name");
                                string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                //4
                                int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                //5
                                int dateIndex = reader.GetOrdinal("dateCommitted");
                                DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                string date = myDate.ToString("MM/dd/yyyy");
                                //6
                                int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                //7
                                int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                //8
                                int violationNameIndex = reader.GetOrdinal("ViolationName");
                                string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                //9
                                int remarksIndex = reader.GetOrdinal("remarks");
                                string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                genRep[0] = i.ToString();
                                genRep[1] = recordNo.ToString();
                                genRep[2] = studNo.ToString();
                                genRep[3] = fullName;
                                genRep[4] = residence;
                                genRep[5] = date;
                                genRep[6] = violationCode.ToString();
                                genRep[7] = violationType;
                                genRep[8] = violationName;
                                genRep[9] = remarks;
                                lvGenReport.Items.Add(new ListViewGenReport
                                {
                                    i = this.i,
                                    recordNo = int.Parse(genRep[1]),
                                    studNo = genRep[2],
                                    fullName = genRep[3],
                                    residence = genRep[4],
                                    dateCommitted = genRep[5],
                                    violationCode = genRep[6],
                                    violationType = genRep[7],
                                    violationName = genRep[8],
                                    remarks = genRep[9]
                                });
                                row++;
                                i++;
                            }
                        }
                    }
                }
            }
            else if (violationType == "Institutional" && violationName == "ALL" && period == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (SYint between " + syFrom + " and " + syTo + ")  AND (ViolationType = 'Institutional') AND (si.ResidenceStatus = '" + residence + "')", conn))
                {
                    lvGenReport.Items.Clear();
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            while (reader.Read())
                            {
                                //1
                                int recordNo = Convert.ToInt32(reader.GetValue(0));
                                //2
                                int studNoIndex = reader.GetOrdinal("StudentNo");
                                int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                //3
                                int fullNameIndex = reader.GetOrdinal("Full Name");
                                string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                //4
                                int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                //5
                                int dateIndex = reader.GetOrdinal("dateCommitted");
                                DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                string date = myDate.ToString("MM/dd/yyyy");
                                //6
                                int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                //7
                                int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                //8
                                int violationNameIndex = reader.GetOrdinal("ViolationName");
                                string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                //9
                                int remarksIndex = reader.GetOrdinal("remarks");
                                string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                genRep[0] = i.ToString();
                                genRep[1] = recordNo.ToString();
                                genRep[2] = studNo.ToString();
                                genRep[3] = fullName;
                                genRep[4] = residence;
                                genRep[5] = date;
                                genRep[6] = violationCode.ToString();
                                genRep[7] = violationType;
                                genRep[8] = violationName;
                                genRep[9] = remarks;
                                lvGenReport.Items.Add(new ListViewGenReport
                                {
                                    i = this.i,
                                    recordNo = int.Parse(genRep[1]),
                                    studNo = genRep[2],
                                    fullName = genRep[3],
                                    residence = genRep[4],
                                    dateCommitted = genRep[5],
                                    violationCode = genRep[6],
                                    violationType = genRep[7],
                                    violationName = genRep[8],
                                    remarks = genRep[9]
                                });
                                row++;
                                i++;
                            }
                        }
                    }
                }
            }
            else if (violationType == "Institutional" && violationName == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (rd.Period = '" + period + "') AND (SYint between " + syFrom + " and " + syTo + ") AND (si.ResidenceStatus = '" + residence + "') AND (ViolationType = 'Institutional')", conn))
                {
                    lvGenReport.Items.Clear();
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        int i = 1;

                        {
                            while (reader.Read())
                            {
                                //1
                                int recordNo = Convert.ToInt32(reader.GetValue(0));
                                //2
                                int studNoIndex = reader.GetOrdinal("StudentNo");
                                int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                //3
                                int fullNameIndex = reader.GetOrdinal("Full Name");
                                string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                //4
                                int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                //5
                                int dateIndex = reader.GetOrdinal("dateCommitted");
                                DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                string date = myDate.ToString("MM/dd/yyyy");
                                //6
                                int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                //7
                                int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                //8
                                int violationNameIndex = reader.GetOrdinal("ViolationName");
                                string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                //9
                                int remarksIndex = reader.GetOrdinal("remarks");
                                string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                genRep[0] = i.ToString();
                                genRep[1] = recordNo.ToString();
                                genRep[2] = studNo.ToString();
                                genRep[3] = fullName;
                                genRep[4] = residence;
                                genRep[5] = date;
                                genRep[6] = violationCode.ToString();
                                genRep[7] = violationType;
                                genRep[8] = violationName;
                                genRep[9] = remarks;
                                lvGenReport.Items.Add(new ListViewGenReport
                                {
                                    i = this.i,
                                    recordNo = int.Parse(genRep[1]),
                                    studNo = genRep[2],
                                    fullName = genRep[3],
                                    residence = genRep[4],
                                    dateCommitted = genRep[5],
                                    violationCode = genRep[6],
                                    violationType = genRep[7],
                                    violationName = genRep[8],
                                    remarks = genRep[9]
                                });
                                row++;
                                i++;
                            }
                        }
                    }
                }
            }

            else if (violationType == "Departmental" && violationName == "ALL" && residence == "ALL" && period == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (SYint between 2015 and 2019)  AND (ViolationType = 'Departmental')", conn))
                {
                    lvGenReport.Items.Clear();
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            while (reader.Read())
                            {
                                //1
                                int recordNo = Convert.ToInt32(reader.GetValue(0));
                                //2
                                int studNoIndex = reader.GetOrdinal("StudentNo");
                                int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                //3
                                int fullNameIndex = reader.GetOrdinal("Full Name");
                                string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                //4
                                int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                //5
                                int dateIndex = reader.GetOrdinal("dateCommitted");
                                DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                string date = myDate.ToString("MM/dd/yyyy");
                                //6
                                int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                //7
                                int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                //8
                                int violationNameIndex = reader.GetOrdinal("ViolationName");
                                string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                //9
                                int remarksIndex = reader.GetOrdinal("remarks");
                                string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                genRep[0] = i.ToString();
                                genRep[1] = recordNo.ToString();
                                genRep[2] = studNo.ToString();
                                genRep[3] = fullName;
                                genRep[4] = residence;
                                genRep[5] = date;
                                genRep[6] = violationCode.ToString();
                                genRep[7] = violationType;
                                genRep[8] = violationName;
                                genRep[9] = remarks;
                                lvGenReport.Items.Add(new ListViewGenReport
                                {
                                    i = this.i,
                                    recordNo = int.Parse(genRep[1]),
                                    studNo = genRep[2],
                                    fullName = genRep[3],
                                    residence = genRep[4],
                                    dateCommitted = genRep[5],
                                    violationCode = genRep[6],
                                    violationType = genRep[7],
                                    violationName = genRep[8],
                                    remarks = genRep[9]
                                });
                                row++;
                                i++;
                            }
                        }
                    }
                }
            }
            else if (violationType == "Departmental" && violationName == "ALL" && residence == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (rd.Period = '" + period + "') AND (SYint between " + syFrom + " and " + syTo + ")  AND (ViolationType = 'Departmental')", conn))
                {
                    lvGenReport.Items.Clear();
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            while (reader.Read())
                            {
                                //1
                                int recordNo = Convert.ToInt32(reader.GetValue(0));
                                //2
                                int studNoIndex = reader.GetOrdinal("StudentNo");
                                int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                //3
                                int fullNameIndex = reader.GetOrdinal("Full Name");
                                string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                //4
                                int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                //5
                                int dateIndex = reader.GetOrdinal("dateCommitted");
                                DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                string date = myDate.ToString("MM/dd/yyyy");
                                //6
                                int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                //7
                                int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                //8
                                int violationNameIndex = reader.GetOrdinal("ViolationName");
                                string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                //9
                                int remarksIndex = reader.GetOrdinal("remarks");
                                string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                genRep[0] = i.ToString();
                                genRep[1] = recordNo.ToString();
                                genRep[2] = studNo.ToString();
                                genRep[3] = fullName;
                                genRep[4] = residence;
                                genRep[5] = date;
                                genRep[6] = violationCode.ToString();
                                genRep[7] = violationType;
                                genRep[8] = violationName;
                                genRep[9] = remarks;
                                lvGenReport.Items.Add(new ListViewGenReport
                                {
                                    i = this.i,
                                    recordNo = int.Parse(genRep[1]),
                                    studNo = genRep[2],
                                    fullName = genRep[3],
                                    residence = genRep[4],
                                    dateCommitted = genRep[5],
                                    violationCode = genRep[6],
                                    violationType = genRep[7],
                                    violationName = genRep[8],
                                    remarks = genRep[9]
                                });
                                row++;
                                i++;
                            }
                        }
                    }
                }
            }
            else if (violationType == "Departmental" && violationName == "ALL" && period == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (SYint between " + syFrom + " and " + syTo + ")  AND (ViolationType = 'Departmental') AND (si.ResidenceStatus = '" + residence + "')", conn))
                {
                    lvGenReport.Items.Clear();
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            while (reader.Read())
                            {
                                //1
                                int recordNo = Convert.ToInt32(reader.GetValue(0));
                                //2
                                int studNoIndex = reader.GetOrdinal("StudentNo");
                                int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                //3
                                int fullNameIndex = reader.GetOrdinal("Full Name");
                                string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                //4
                                int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                //5
                                int dateIndex = reader.GetOrdinal("dateCommitted");
                                DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                string date = myDate.ToString("MM/dd/yyyy");
                                //6
                                int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                //7
                                int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                //8
                                int violationNameIndex = reader.GetOrdinal("ViolationName");
                                string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                //9
                                int remarksIndex = reader.GetOrdinal("remarks");
                                string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                genRep[0] = i.ToString();
                                genRep[1] = recordNo.ToString();
                                genRep[2] = studNo.ToString();
                                genRep[3] = fullName;
                                genRep[4] = residence;
                                genRep[5] = date;
                                genRep[6] = violationCode.ToString();
                                genRep[7] = violationType;
                                genRep[8] = violationName;
                                genRep[9] = remarks;
                                lvGenReport.Items.Add(new ListViewGenReport
                                {
                                    i = this.i,
                                    recordNo = int.Parse(genRep[1]),
                                    studNo = genRep[2],
                                    fullName = genRep[3],
                                    residence = genRep[4],
                                    dateCommitted = genRep[5],
                                    violationCode = genRep[6],
                                    violationType = genRep[7],
                                    violationName = genRep[8],
                                    remarks = genRep[9]
                                });
                                row++;
                                i++;
                            }
                        }
                    }
                }
            }
            else if (violationType == "Departmental" && violationName == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (rd.Period = '" + period + "') AND (SYint between " + syFrom + " and " + syTo + ") AND (si.ResidenceStatus = '" + residence + "') AND (ViolationType = 'Departmental')", conn))
                {
                    lvGenReport.Items.Clear();
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            while (reader.Read())
                            {
                                //1
                                int recordNo = Convert.ToInt32(reader.GetValue(0));
                                //2
                                int studNoIndex = reader.GetOrdinal("StudentNo");
                                int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                //3
                                int fullNameIndex = reader.GetOrdinal("Full Name");
                                string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                //4
                                int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                //5
                                int dateIndex = reader.GetOrdinal("dateCommitted");
                                DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                string date = myDate.ToString("MM/dd/yyyy");
                                //6
                                int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                //7
                                int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                //8
                                int violationNameIndex = reader.GetOrdinal("ViolationName");
                                string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                //9
                                int remarksIndex = reader.GetOrdinal("remarks");
                                string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                genRep[0] = i.ToString();
                                genRep[1] = recordNo.ToString();
                                genRep[2] = studNo.ToString();
                                genRep[3] = fullName;
                                genRep[4] = residence;
                                genRep[5] = date;
                                genRep[6] = violationCode.ToString();
                                genRep[7] = violationType;
                                genRep[8] = violationName;
                                genRep[9] = remarks;
                                lvGenReport.Items.Add(new ListViewGenReport
                                {
                                    i = this.i,
                                    recordNo = int.Parse(genRep[1]),
                                    studNo = genRep[2],
                                    fullName = genRep[3],
                                    residence = genRep[4],
                                    dateCommitted = genRep[5],
                                    violationCode = genRep[6],
                                    violationType = genRep[7],
                                    violationName = genRep[8],
                                    remarks = genRep[9]
                                });
                                row++;
                                i++;
                            }
                        }
                    }
                }
            }

            else if (period == "ALL" && violationType == "ALL" && residence == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (SYint between " + syFrom + " and " + syTo + ")", conn))
                {
                    lvGenReport.Items.Clear();
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            while (reader.Read())
                            {
                                //1
                                int recordNo = Convert.ToInt32(reader.GetValue(0));
                                //2
                                int studNoIndex = reader.GetOrdinal("StudentNo");
                                int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                //3
                                int fullNameIndex = reader.GetOrdinal("Full Name");
                                string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                //4
                                int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                //5
                                int dateIndex = reader.GetOrdinal("dateCommitted");
                                DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                string date = myDate.ToString("MM/dd/yyyy");
                                //6
                                int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                //7
                                int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                //8
                                int violationNameIndex = reader.GetOrdinal("ViolationName");
                                string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                //9
                                int remarksIndex = reader.GetOrdinal("remarks");
                                string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                genRep[1] = recordNo.ToString();
                                genRep[2] = studNo.ToString();
                                genRep[3] = fullName;
                                genRep[4] = residence;
                                genRep[5] = date;
                                genRep[6] = violationCode.ToString();
                                genRep[7] = violationType;
                                genRep[8] = violationName;
                                genRep[9] = remarks;
                                lvGenReport.Items.Add(new ListViewGenReport
                                {
                                    i = this.i,
                                    recordNo = int.Parse(genRep[1]),
                                    studNo = genRep[2],
                                    fullName = genRep[3],
                                    residence = genRep[4],
                                    dateCommitted = genRep[5],
                                    violationCode = genRep[6],
                                    violationType = genRep[7],
                                    violationName = genRep[8],
                                    remarks = genRep[9]
                                });
                                row++;
                                i++;
                            }
                        }
                    }
                }
            }
            else if (period == "ALL" && violationType == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (SYint between " + syFrom + " and " + syTo + ") AND (si.ResidenceStatus = '" + residence + "')", conn))
                {
                    lvGenReport.Items.Clear();
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            while (reader.Read())
                            {
                                //1
                                int recordNo = Convert.ToInt32(reader.GetValue(0));
                                //2
                                int studNoIndex = reader.GetOrdinal("StudentNo");
                                int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                //3
                                int fullNameIndex = reader.GetOrdinal("Full Name");
                                string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                //4
                                int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                //5
                                int dateIndex = reader.GetOrdinal("dateCommitted");
                                DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                string date = myDate.ToString("MM/dd/yyyy");
                                //6
                                int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                //7
                                int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                //8
                                int violationNameIndex = reader.GetOrdinal("ViolationName");
                                string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                //9
                                int remarksIndex = reader.GetOrdinal("remarks");
                                string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                genRep[0] = i.ToString();
                                genRep[1] = recordNo.ToString();
                                genRep[2] = studNo.ToString();
                                genRep[3] = fullName;
                                genRep[4] = residence;
                                genRep[5] = date;
                                genRep[6] = violationCode.ToString();
                                genRep[7] = violationType;
                                genRep[8] = violationName;
                                genRep[9] = remarks;
                                lvGenReport.Items.Add(new ListViewGenReport
                                {
                                    i = this.i,
                                    recordNo = int.Parse(genRep[1]),
                                    studNo = genRep[2],
                                    fullName = genRep[3],
                                    residence = genRep[4],
                                    dateCommitted = genRep[5],
                                    violationCode = genRep[6],
                                    violationType = genRep[7],
                                    violationName = genRep[8],
                                    remarks = genRep[9]
                                });
                                row++;
                                i++;
                            }
                        }
                    }
                }
            }
            else if (period == "ALL" && residence == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (vd.ViolationType = '" + violationType + "') AND (vd.ViolationName = '" + violationName + "') AND (SYint between " + syFrom + " and " + syTo + ")", conn))
                {
                    lvGenReport.Items.Clear();
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            while (reader.Read())
                            {
                                //1
                                int recordNo = Convert.ToInt32(reader.GetValue(0));
                                //2
                                int studNoIndex = reader.GetOrdinal("StudentNo");
                                int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                //3
                                int fullNameIndex = reader.GetOrdinal("Full Name");
                                string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                //4
                                int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                //5
                                int dateIndex = reader.GetOrdinal("dateCommitted");
                                DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                string date = myDate.ToString("MM/dd/yyyy");
                                //6
                                int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                //7
                                int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                //8
                                int violationNameIndex = reader.GetOrdinal("ViolationName");
                                string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                //9
                                int remarksIndex = reader.GetOrdinal("remarks");
                                string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                genRep[0] = i.ToString();
                                genRep[1] = recordNo.ToString();
                                genRep[2] = studNo.ToString();
                                genRep[3] = fullName;
                                genRep[4] = residence;
                                genRep[5] = date;
                                genRep[6] = violationCode.ToString();
                                genRep[7] = violationType;
                                genRep[8] = violationName;
                                genRep[9] = remarks;
                                lvGenReport.Items.Add(new ListViewGenReport
                                {
                                    i = this.i,
                                    recordNo = int.Parse(genRep[1]),
                                    studNo = genRep[2],
                                    fullName = genRep[3],
                                    residence = genRep[4],
                                    dateCommitted = genRep[5],
                                    violationCode = genRep[6],
                                    violationType = genRep[7],
                                    violationName = genRep[8],
                                    remarks = genRep[9]
                                });
                                row++;
                                i++;
                            }
                        }
                    }
                }
            } //

            else if (violationType == "ALL" && residence == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (rd.Period = '" + period + "') AND (SYint between " + syFrom + " and " + syTo + ")", conn))
                {
                    lvGenReport.Items.Clear();
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            while (reader.Read())
                            {
                                //1
                                int recordNo = Convert.ToInt32(reader.GetValue(0));
                                //2
                                int studNoIndex = reader.GetOrdinal("StudentNo");
                                int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                //3
                                int fullNameIndex = reader.GetOrdinal("Full Name");
                                string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                //4
                                int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                //5
                                int dateIndex = reader.GetOrdinal("dateCommitted");
                                DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                string date = myDate.ToString("MM/dd/yyyy");
                                //6
                                int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                //7
                                int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                //8
                                int violationNameIndex = reader.GetOrdinal("ViolationName");
                                string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                //9
                                int remarksIndex = reader.GetOrdinal("remarks");
                                string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                genRep[0] = i.ToString();
                                genRep[1] = recordNo.ToString();
                                genRep[2] = studNo.ToString();
                                genRep[3] = fullName;
                                genRep[4] = residence;
                                genRep[5] = date;
                                genRep[6] = violationCode.ToString();
                                genRep[7] = violationType;
                                genRep[8] = violationName;
                                genRep[9] = remarks;
                                lvGenReport.Items.Add(new ListViewGenReport
                                {
                                    i = this.i,
                                    recordNo = int.Parse(genRep[1]),
                                    studNo = genRep[2],
                                    fullName = genRep[3],
                                    residence = genRep[4],
                                    dateCommitted = genRep[5],
                                    violationCode = genRep[6],
                                    violationType = genRep[7],
                                    violationName = genRep[8],
                                    remarks = genRep[9]
                                });
                                row++;
                                i++;
                            }
                        }
                    }
                }
            }
            else if (violationType == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (rd.Period = '" + period + "') AND (SYint between " + syFrom + " and " + syTo + ") AND (si.ResidenceStatus = '" + residence + "')", conn))
                {
                    lvGenReport.Items.Clear();
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            while (reader.Read())
                            {
                                //1
                                int recordNo = Convert.ToInt32(reader.GetValue(0));
                                //2
                                int studNoIndex = reader.GetOrdinal("StudentNo");
                                int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                //3
                                int fullNameIndex = reader.GetOrdinal("Full Name");
                                string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                //4
                                int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                //5
                                int dateIndex = reader.GetOrdinal("dateCommitted");
                                DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                string date = myDate.ToString("MM/dd/yyyy");
                                //6
                                int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                //7
                                int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                //8
                                int violationNameIndex = reader.GetOrdinal("ViolationName");
                                string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                //9
                                int remarksIndex = reader.GetOrdinal("remarks");
                                string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                genRep[0] = i.ToString();
                                genRep[1] = recordNo.ToString();
                                genRep[2] = studNo.ToString();
                                genRep[3] = fullName;
                                genRep[4] = residence;
                                genRep[5] = date;
                                genRep[6] = violationCode.ToString();
                                genRep[7] = violationType;
                                genRep[8] = violationName;
                                genRep[9] = remarks;
                                lvGenReport.Items.Add(new ListViewGenReport
                                {
                                    i = this.i,
                                    recordNo = int.Parse(genRep[1]),
                                    studNo = genRep[2],
                                    fullName = genRep[3],
                                    residence = genRep[4],
                                    dateCommitted = genRep[5],
                                    violationCode = genRep[6],
                                    violationType = genRep[7],
                                    violationName = genRep[8],
                                    remarks = genRep[9]
                                });
                                row++;
                                i++;
                            }
                        }
                    }
                }
            }
            else if (period == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (vd.ViolationType = '" + violationType + "') AND (vd.ViolationName = '" + violationName + "') AND (SYint between " + syFrom + " and " + syTo + ") AND (si.ResidenceStatus = '" + residence + "')", conn))
                {
                    lvGenReport.Items.Clear();
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            while (reader.Read())
                            {
                                //1
                                int recordNo = Convert.ToInt32(reader.GetValue(0));
                                //2
                                int studNoIndex = reader.GetOrdinal("StudentNo");
                                int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                //3
                                int fullNameIndex = reader.GetOrdinal("Full Name");
                                string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                //4
                                int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                //5
                                int dateIndex = reader.GetOrdinal("dateCommitted");
                                DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                string date = myDate.ToString("MM/dd/yyyy");
                                //6
                                int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                //7
                                int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                //8
                                int violationNameIndex = reader.GetOrdinal("ViolationName");
                                string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                //9
                                int remarksIndex = reader.GetOrdinal("remarks");
                                string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                genRep[0] = i.ToString();
                                genRep[1] = recordNo.ToString();
                                genRep[2] = studNo.ToString();
                                genRep[3] = fullName;
                                genRep[4] = residence;
                                genRep[5] = date;
                                genRep[6] = violationCode.ToString();
                                genRep[7] = violationType;
                                genRep[8] = violationName;
                                genRep[9] = remarks;
                                lvGenReport.Items.Add(new ListViewGenReport
                                {
                                    i = this.i,
                                    recordNo = int.Parse(genRep[1]),
                                    studNo = genRep[2],
                                    fullName = genRep[3],
                                    residence = genRep[4],
                                    dateCommitted = genRep[5],
                                    violationCode = genRep[6],
                                    violationType = genRep[7],
                                    violationName = genRep[8],
                                    remarks = genRep[9]
                                });
                                row++;
                                i++;
                            }
                        }
                    }
                }
            } //
            else if (residence == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (vd.ViolationType = '" + violationType + "') AND (rd.Period = '" + period + "') AND (SYint between " + syFrom + " and " + syTo + ")", conn))
                {
                    lvGenReport.Items.Clear();
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            while (reader.Read())
                            {
                                //1
                                int recordNo = Convert.ToInt32(reader.GetValue(0));
                                //2
                                int studNoIndex = reader.GetOrdinal("StudentNo");
                                int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                //3
                                int fullNameIndex = reader.GetOrdinal("Full Name");
                                string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                //4
                                int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                //5
                                int dateIndex = reader.GetOrdinal("dateCommitted");
                                DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                string date = myDate.ToString("MM/dd/yyyy");
                                //6
                                int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                //7
                                int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                //8
                                int violationNameIndex = reader.GetOrdinal("ViolationName");
                                string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                //9
                                int remarksIndex = reader.GetOrdinal("remarks");
                                string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                genRep[0] = i.ToString();
                                genRep[1] = recordNo.ToString();
                                genRep[2] = studNo.ToString();
                                genRep[3] = fullName;
                                genRep[4] = residence;
                                genRep[5] = date;
                                genRep[6] = violationCode.ToString();
                                genRep[7] = violationType;
                                genRep[8] = violationName;
                                genRep[9] = remarks;
                                lvGenReport.Items.Add(new ListViewGenReport
                                {
                                    i = this.i,
                                    recordNo = int.Parse(genRep[1]),
                                    studNo = genRep[2],
                                    fullName = genRep[3],
                                    residence = genRep[4],
                                    dateCommitted = genRep[5],
                                    violationCode = genRep[6],
                                    violationType = genRep[7],
                                    violationName = genRep[8],
                                    remarks = genRep[9]
                                });
                                row++;
                                i++;
                            }
                        }
                    }
                }
            }
            else
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (vd.ViolationType = '" + violationType + "') AND (vd.ViolationName = '" + violationName + "') AND (rd.Period = '" + period + "') AND (SYint between " + syFrom + " and " + syTo + ") AND (si.ResidenceStatus = '" + residence + "')", conn))
                {
                    lvGenReport.Items.Clear();
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            while (reader.Read())
                            {
                                //1
                                int recordNo = Convert.ToInt32(reader.GetValue(0));
                                //2
                                int studNoIndex = reader.GetOrdinal("StudentNo");
                                int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                //3
                                int fullNameIndex = reader.GetOrdinal("Full Name");
                                string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                //4
                                int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                //5
                                int dateIndex = reader.GetOrdinal("dateCommitted");
                                DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                string date = myDate.ToString("MM/dd/yyyy");
                                //6
                                int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                //7
                                int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                //8
                                int violationNameIndex = reader.GetOrdinal("ViolationName");
                                string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                //9
                                int remarksIndex = reader.GetOrdinal("remarks");
                                string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                genRep[0] = i.ToString();
                                genRep[1] = recordNo.ToString();
                                genRep[2] = studNo.ToString();
                                genRep[3] = fullName;
                                genRep[4] = residence;
                                genRep[5] = date;
                                genRep[6] = violationCode.ToString();
                                genRep[7] = violationType;
                                genRep[8] = violationName;
                                genRep[9] = remarks;
                                lvGenReport.Items.Add(new ListViewGenReport
                                {
                                    i = this.i,
                                    recordNo = int.Parse(genRep[1]),
                                    studNo = genRep[2],
                                    fullName = genRep[3],
                                    residence = genRep[4],
                                    dateCommitted = genRep[5],
                                    violationCode = genRep[6],
                                    violationType = genRep[7],
                                    violationName = genRep[8],
                                    remarks = genRep[9]
                                });
                                row++;
                                i++;
                            }
                        }
                    }
                }
            }
            conn.Close();
        }

        private void btnPrintReport_Click(object sender, RoutedEventArgs e)
        {
            SqlCeConnection conn = DBUtils.GetDBConnection();
            object oMissing = Missing.Value;
            object oEndOfDoc = "\\endofdoc"; /* \endofdoc is a predefined bookmark */

            //Start Word and create a new document.
            Word._Application oWord;
            Word._Document oDoc;
            oWord = new Word.Application();
            oWord.Visible = true;
            oDoc = oWord.Documents.Add(ref oMissing, ref oMissing,
            ref oMissing, ref oMissing);

            Word.Paragraph oPara;
            oPara = oDoc.Content.Paragraphs.Add(ref oMissing);
            oPara.Range.Text = "Adamson Computer Science Department";
            oPara.Range.Font.Size = 18;
            oPara.Range.Font.Bold = 1;
            oPara.Format.SpaceAfter = 1;    //1 pt spacing after paragraph.
            oPara.Range.InsertParagraphAfter();

            //Insert a paragraph at the beginning of the document.
            Word.Paragraph oPara1;
            oPara1 = oDoc.Content.Paragraphs.Add(ref oMissing);
            if (violationName == null || violationName == "")
            {
                oPara1.Range.Text = "Violation Type: " + violationType;

            }
            else
            {
                oPara1.Range.Text = "Violation Type: " + violationType + "\nViolation Name: " + violationName;

            }
            oPara1.Range.Font.Size = 15;
            oPara1.Range.Font.Bold = 1;
            oPara1.Format.SpaceAfter = 1;    //1 pt spacing after paragraph.
            oPara1.Range.InsertParagraphAfter();

            //Insert a paragraph at the end of the document.
            Word.Paragraph oPara2;
            object oRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oPara2 = oDoc.Content.Paragraphs.Add(ref oRng);
            oPara2.Range.Text = "Period: " + period + "\nSY: " + syFrom + "-" + syTo;
            oPara1.Range.Font.Bold = 0;
            oPara2.Range.Font.Size = 14;

            oPara2.Format.SpaceAfter = 1;
            oPara2.Range.InsertParagraphAfter();

            //Insert another paragraph.
            Word.Paragraph oPara3;
            oRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oPara3 = oDoc.Content.Paragraphs.Add(ref oRng);
            oPara3.Range.Text = "Residence Status: " + residence;
            oPara3.Format.SpaceAfter = 24;
            oPara3.Range.Font.Size = 11;
            oPara3.Range.InsertParagraphAfter();

            //Insert a row x column table, fill it with data, and make the first row bold.
            Word.Table oTable;
            Word.Range wrdRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oTable = oDoc.Tables.Add(wrdRng, row, column, ref oMissing, ref oMissing);

            oTable.Range.ParagraphFormat.SpaceAfter = 6;
            int r, c;
            string strText;
            oTable.Cell(1, 1).Range.Text = "Record No.";
            oTable.Cell(1, 2).Range.Text = "Student No.";
            oTable.Cell(1, 3).Range.Text = "Full Name";
            oTable.Cell(1, 4).Range.Text = "Residence";
            oTable.Cell(1, 5).Range.Text = "Date Committed";
            oTable.Cell(1, 6).Range.Text = "Violation Code";
            oTable.Cell(1, 7).Range.Text = "Violation Type";
            oTable.Cell(1, 8).Range.Text = "Violation Name";
            oTable.Cell(1, 9).Range.Text = "Remarks";

            conn.Open();
            if (violationType == "Academic" && violationName == "ALL" && residence == "ALL" && period == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (SYint between " + syFrom + " and " + syTo + ") AND (ViolationType = 'Academic')", conn))
                {
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        if (reader.HasRows)
                        {
                            for (r = 2; r <= row; r++)
                            {
                                int i = 0;
                                reader.Read();
                                for (c = 1; c <= column; c++)
                                {
                                    //1
                                    int recordNo = Convert.ToInt32(reader.GetValue(0));
                                    //2
                                    int studNoIndex = reader.GetOrdinal("StudentNo");
                                    int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                    //3
                                    int fullNameIndex = reader.GetOrdinal("Full Name");
                                    string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                    //4
                                    int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                    string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                    //5
                                    int dateIndex = reader.GetOrdinal("dateCommitted");
                                    DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                    string date = myDate.ToString("MM/dd/yyyy");
                                    //6
                                    int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                    int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                    //7
                                    int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                    string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                    //8
                                    int violationNameIndex = reader.GetOrdinal("ViolationName");
                                    string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                    //9
                                    int remarksIndex = reader.GetOrdinal("remarks");
                                    string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                    genRep[0] = recordNo.ToString();
                                    genRep[1] = studNo.ToString();
                                    genRep[2] = fullName;
                                    genRep[3] = residence;
                                    genRep[4] = date;
                                    genRep[5] = violationCode.ToString();
                                    genRep[6] = violationType;
                                    genRep[7] = violationName;
                                    genRep[8] = remarks;
                                    strText = genRep[i];
                                    oTable.Cell(r, c).Range.Text = strText;
                                    i++;
                                }
                            }
                        }
                    }
                }
            }
            else if (violationType == "Academic" && violationName == "ALL" && residence == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (rd.Period = '" + period + "') AND (SYint between " + syFrom + " and " + syTo + ")  AND (ViolationType = 'Academic')", conn))
                {
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            if (reader.HasRows)
                            {
                                for (r = 2; r <= row; r++)
                                {
                                    int i = 0;
                                    reader.Read();
                                    for (c = 1; c <= column; c++)
                                    {
                                        //1
                                        int recordNo = Convert.ToInt32(reader.GetValue(0));
                                        //2
                                        int studNoIndex = reader.GetOrdinal("StudentNo");
                                        int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                        //3
                                        int fullNameIndex = reader.GetOrdinal("Full Name");
                                        string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                        //4
                                        int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                        string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                        //5
                                        int dateIndex = reader.GetOrdinal("dateCommitted");
                                        DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                        string date = myDate.ToString("MM/dd/yyyy");
                                        //6
                                        int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                        int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                        //7
                                        int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                        string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                        //8
                                        int violationNameIndex = reader.GetOrdinal("ViolationName");
                                        string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                        //9
                                        int remarksIndex = reader.GetOrdinal("remarks");
                                        string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                        genRep[0] = recordNo.ToString();
                                        genRep[1] = studNo.ToString();
                                        genRep[2] = fullName;
                                        genRep[3] = residence;
                                        genRep[4] = date;
                                        genRep[5] = violationCode.ToString();
                                        genRep[6] = violationType;
                                        genRep[7] = violationName;
                                        genRep[8] = remarks;
                                        strText = genRep[i];
                                        oTable.Cell(r, c).Range.Text = strText;
                                        i++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (violationType == "Academic" && violationName == "ALL" && period == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (SYint between " + syFrom + " and " + syTo + ")  AND (ViolationType = 'Academic') AND (si.ResidenceStatus = '" + residence + "')", conn))
                {
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            if (reader.HasRows)
                            {
                                for (r = 2; r <= row; r++)
                                {
                                    int i = 0;
                                    reader.Read();
                                    for (c = 1; c <= column; c++)
                                    {
                                        //1
                                        int recordNo = Convert.ToInt32(reader.GetValue(0));
                                        //2
                                        int studNoIndex = reader.GetOrdinal("StudentNo");
                                        int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                        //3
                                        int fullNameIndex = reader.GetOrdinal("Full Name");
                                        string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                        //4
                                        int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                        string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                        //5
                                        int dateIndex = reader.GetOrdinal("dateCommitted");
                                        DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                        string date = myDate.ToString("MM/dd/yyyy");
                                        //6
                                        int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                        int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                        //7
                                        int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                        string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                        //8
                                        int violationNameIndex = reader.GetOrdinal("ViolationName");
                                        string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                        //9
                                        int remarksIndex = reader.GetOrdinal("remarks");
                                        string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                        genRep[0] = recordNo.ToString();
                                        genRep[1] = studNo.ToString();
                                        genRep[2] = fullName;
                                        genRep[3] = residence;
                                        genRep[4] = date;
                                        genRep[5] = violationCode.ToString();
                                        genRep[6] = violationType;
                                        genRep[7] = violationName;
                                        genRep[8] = remarks;
                                        strText = genRep[i];
                                        oTable.Cell(r, c).Range.Text = strText;
                                        i++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (violationType == "Academic" && violationName == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (rd.Period = '" + period + "') AND (SYint between " + syFrom + " and " + syTo + ") AND (si.ResidenceStatus = '" + residence + "') AND (ViolationType = 'Academic')", conn))
                {
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            if (reader.HasRows)
                            {
                                for (r = 2; r <= row; r++)
                                {
                                    int i = 0;
                                    reader.Read();
                                    for (c = 1; c <= column; c++)
                                    {
                                        //1
                                        int recordNo = Convert.ToInt32(reader.GetValue(0));
                                        //2
                                        int studNoIndex = reader.GetOrdinal("StudentNo");
                                        int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                        //3
                                        int fullNameIndex = reader.GetOrdinal("Full Name");
                                        string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                        //4
                                        int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                        string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                        //5
                                        int dateIndex = reader.GetOrdinal("dateCommitted");
                                        DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                        string date = myDate.ToString("MM/dd/yyyy");
                                        //6
                                        int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                        int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                        //7
                                        int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                        string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                        //8
                                        int violationNameIndex = reader.GetOrdinal("ViolationName");
                                        string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                        //9
                                        int remarksIndex = reader.GetOrdinal("remarks");
                                        string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                        genRep[0] = recordNo.ToString();
                                        genRep[1] = studNo.ToString();
                                        genRep[2] = fullName;
                                        genRep[3] = residence;
                                        genRep[4] = date;
                                        genRep[5] = violationCode.ToString();
                                        genRep[6] = violationType;
                                        genRep[7] = violationName;
                                        genRep[8] = remarks;
                                        strText = genRep[i];
                                        oTable.Cell(r, c).Range.Text = strText;
                                        i++;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            else if (violationType == "Institutional" && violationName == "ALL" && residence == "ALL" && period == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (SYint between " + syFrom + " and " + syTo + ")  AND (ViolationType = 'Institutional')", conn))
                {
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            if (reader.HasRows)
                            {
                                for (r = 2; r <= row; r++)
                                {
                                    int i = 0;
                                    reader.Read();
                                    for (c = 1; c <= column; c++)
                                    {
                                        //1
                                        int recordNo = Convert.ToInt32(reader.GetValue(0));
                                        //2
                                        int studNoIndex = reader.GetOrdinal("StudentNo");
                                        int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                        //3
                                        int fullNameIndex = reader.GetOrdinal("Full Name");
                                        string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                        //4
                                        int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                        string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                        //5
                                        int dateIndex = reader.GetOrdinal("dateCommitted");
                                        DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                        string date = myDate.ToString("MM/dd/yyyy");
                                        //6
                                        int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                        int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                        //7
                                        int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                        string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                        //8
                                        int violationNameIndex = reader.GetOrdinal("ViolationName");
                                        string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                        //9
                                        int remarksIndex = reader.GetOrdinal("remarks");
                                        string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                        genRep[0] = recordNo.ToString();
                                        genRep[1] = studNo.ToString();
                                        genRep[2] = fullName;
                                        genRep[3] = residence;
                                        genRep[4] = date;
                                        genRep[5] = violationCode.ToString();
                                        genRep[6] = violationType;
                                        genRep[7] = violationName;
                                        genRep[8] = remarks;
                                        strText = genRep[i];
                                        oTable.Cell(r, c).Range.Text = strText;
                                        i++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (violationType == "Institutional" && violationName == "ALL" && residence == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (rd.Period = '" + period + "') AND (SYint between " + syFrom + " and " + syTo + ")  AND (ViolationType = 'Institutional')", conn))
                {
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            if (reader.HasRows)
                            {
                                for (r = 2; r <= row; r++)
                                {
                                    int i = 0;
                                    reader.Read();
                                    for (c = 1; c <= column; c++)
                                    {
                                        //1
                                        int recordNo = Convert.ToInt32(reader.GetValue(0));
                                        //2
                                        int studNoIndex = reader.GetOrdinal("StudentNo");
                                        int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                        //3
                                        int fullNameIndex = reader.GetOrdinal("Full Name");
                                        string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                        //4
                                        int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                        string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                        //5
                                        int dateIndex = reader.GetOrdinal("dateCommitted");
                                        DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                        string date = myDate.ToString("MM/dd/yyyy");
                                        //6
                                        int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                        int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                        //7
                                        int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                        string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                        //8
                                        int violationNameIndex = reader.GetOrdinal("ViolationName");
                                        string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                        //9
                                        int remarksIndex = reader.GetOrdinal("remarks");
                                        string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                        genRep[0] = recordNo.ToString();
                                        genRep[1] = studNo.ToString();
                                        genRep[2] = fullName;
                                        genRep[3] = residence;
                                        genRep[4] = date;
                                        genRep[5] = violationCode.ToString();
                                        genRep[6] = violationType;
                                        genRep[7] = violationName;
                                        genRep[8] = remarks;
                                        strText = genRep[i];
                                        oTable.Cell(r, c).Range.Text = strText;
                                        i++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (violationType == "Institutional" && violationName == "ALL" && period == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (SYint between " + syFrom + " and " + syTo + ")  AND (ViolationType = 'Institutional') AND (si.ResidenceStatus = '" + residence + "')", conn))
                {
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            if (reader.HasRows)
                            {
                                for (r = 2; r <= row; r++)
                                {
                                    int i = 0;
                                    reader.Read();
                                    for (c = 1; c <= column; c++)
                                    {
                                        //1
                                        int recordNo = Convert.ToInt32(reader.GetValue(0));
                                        //2
                                        int studNoIndex = reader.GetOrdinal("StudentNo");
                                        int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                        //3
                                        int fullNameIndex = reader.GetOrdinal("Full Name");
                                        string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                        //4
                                        int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                        string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                        //5
                                        int dateIndex = reader.GetOrdinal("dateCommitted");
                                        DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                        string date = myDate.ToString("MM/dd/yyyy");
                                        //6
                                        int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                        int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                        //7
                                        int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                        string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                        //8
                                        int violationNameIndex = reader.GetOrdinal("ViolationName");
                                        string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                        //9
                                        int remarksIndex = reader.GetOrdinal("remarks");
                                        string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                        genRep[0] = recordNo.ToString();
                                        genRep[1] = studNo.ToString();
                                        genRep[2] = fullName;
                                        genRep[3] = residence;
                                        genRep[4] = date;
                                        genRep[5] = violationCode.ToString();
                                        genRep[6] = violationType;
                                        genRep[7] = violationName;
                                        genRep[8] = remarks;
                                        strText = genRep[i];
                                        oTable.Cell(r, c).Range.Text = strText;
                                        i++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (violationType == "Institutional" && violationName == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (rd.Period = '" + period + "') AND (SYint between " + syFrom + " and " + syTo + ") AND (si.ResidenceStatus = '" + residence + "') AND (ViolationType = 'Institutional')", conn))
                {
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            if (reader.HasRows)
                            {
                                for (r = 2; r <= row; r++)
                                {
                                    int i = 0;
                                    reader.Read();
                                    for (c = 1; c <= column; c++)
                                    {
                                        //1
                                        int recordNo = Convert.ToInt32(reader.GetValue(0));
                                        //2
                                        int studNoIndex = reader.GetOrdinal("StudentNo");
                                        int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                        //3
                                        int fullNameIndex = reader.GetOrdinal("Full Name");
                                        string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                        //4
                                        int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                        string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                        //5
                                        int dateIndex = reader.GetOrdinal("dateCommitted");
                                        DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                        string date = myDate.ToString("MM/dd/yyyy");
                                        //6
                                        int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                        int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                        //7
                                        int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                        string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                        //8
                                        int violationNameIndex = reader.GetOrdinal("ViolationName");
                                        string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                        //9
                                        int remarksIndex = reader.GetOrdinal("remarks");
                                        string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                        genRep[0] = recordNo.ToString();
                                        genRep[1] = studNo.ToString();
                                        genRep[2] = fullName;
                                        genRep[3] = residence;
                                        genRep[4] = date;
                                        genRep[5] = violationCode.ToString();
                                        genRep[6] = violationType;
                                        genRep[7] = violationName;
                                        genRep[8] = remarks;
                                        strText = genRep[i];
                                        oTable.Cell(r, c).Range.Text = strText;
                                        i++;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            else if (violationType == "Departmental" && violationName == "ALL" && residence == "ALL" && period == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (SYint between 2015 and 2019)  AND (ViolationType = 'Departmental')", conn))
                {
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            if (reader.HasRows)
                            {
                                for (r = 2; r <= row; r++)
                                {
                                    int i = 0;
                                    reader.Read();
                                    for (c = 1; c <= column; c++)
                                    {
                                        //1
                                        int recordNo = Convert.ToInt32(reader.GetValue(0));
                                        //2
                                        int studNoIndex = reader.GetOrdinal("StudentNo");
                                        int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                        //3
                                        int fullNameIndex = reader.GetOrdinal("Full Name");
                                        string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                        //4
                                        int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                        string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                        //5
                                        int dateIndex = reader.GetOrdinal("dateCommitted");
                                        DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                        string date = myDate.ToString("MM/dd/yyyy");
                                        //6
                                        int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                        int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                        //7
                                        int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                        string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                        //8
                                        int violationNameIndex = reader.GetOrdinal("ViolationName");
                                        string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                        //9
                                        int remarksIndex = reader.GetOrdinal("remarks");
                                        string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                        genRep[0] = recordNo.ToString();
                                        genRep[1] = studNo.ToString();
                                        genRep[2] = fullName;
                                        genRep[3] = residence;
                                        genRep[4] = date;
                                        genRep[5] = violationCode.ToString();
                                        genRep[6] = violationType;
                                        genRep[7] = violationName;
                                        genRep[8] = remarks;
                                        strText = genRep[i];
                                        oTable.Cell(r, c).Range.Text = strText;
                                        i++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (violationType == "Departmental" && violationName == "ALL" && residence == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (rd.Period = '" + period + "') AND (SYint between " + syFrom + " and " + syTo + ")  AND (ViolationType = 'Departmental')", conn))
                {
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            if (reader.HasRows)
                            {
                                for (r = 2; r <= row; r++)
                                {
                                    int i = 0;
                                    reader.Read();
                                    for (c = 1; c <= column; c++)
                                    {
                                        //1
                                        int recordNo = Convert.ToInt32(reader.GetValue(0));
                                        //2
                                        int studNoIndex = reader.GetOrdinal("StudentNo");
                                        int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                        //3
                                        int fullNameIndex = reader.GetOrdinal("Full Name");
                                        string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                        //4
                                        int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                        string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                        //5
                                        int dateIndex = reader.GetOrdinal("dateCommitted");
                                        DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                        string date = myDate.ToString("MM/dd/yyyy");
                                        //6
                                        int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                        int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                        //7
                                        int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                        string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                        //8
                                        int violationNameIndex = reader.GetOrdinal("ViolationName");
                                        string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                        //9
                                        int remarksIndex = reader.GetOrdinal("remarks");
                                        string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                        genRep[0] = recordNo.ToString();
                                        genRep[1] = studNo.ToString();
                                        genRep[2] = fullName;
                                        genRep[3] = residence;
                                        genRep[4] = date;
                                        genRep[5] = violationCode.ToString();
                                        genRep[6] = violationType;
                                        genRep[7] = violationName;
                                        genRep[8] = remarks;
                                        strText = genRep[i];
                                        oTable.Cell(r, c).Range.Text = strText;
                                        i++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (violationType == "Departmental" && violationName == "ALL" && period == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (SYint between " + syFrom + " and " + syTo + ")  AND (ViolationType = 'Departmental') AND (si.ResidenceStatus = '" + residence + "')", conn))
                {
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {

                        for (r = 2; r <= row; r++)
                        {
                            int i = 0;
                            reader.Read();
                            for (c = 1; c <= column; c++)
                            {
                                //1
                                int recordNo = Convert.ToInt32(reader.GetValue(0));
                                //2
                                int studNoIndex = reader.GetOrdinal("StudentNo");
                                int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                //3
                                int fullNameIndex = reader.GetOrdinal("Full Name");
                                string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                //4
                                int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                //5
                                int dateIndex = reader.GetOrdinal("dateCommitted");
                                DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                string date = myDate.ToString("MM/dd/yyyy");
                                //6
                                int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                //7
                                int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                //8
                                int violationNameIndex = reader.GetOrdinal("ViolationName");
                                string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                //9
                                int remarksIndex = reader.GetOrdinal("remarks");
                                string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                genRep[0] = recordNo.ToString();
                                genRep[1] = studNo.ToString();
                                genRep[2] = fullName;
                                genRep[3] = residence;
                                genRep[4] = date;
                                genRep[5] = violationCode.ToString();
                                genRep[6] = violationType;
                                genRep[7] = violationName;
                                genRep[8] = remarks;
                                strText = genRep[i];
                                oTable.Cell(r, c).Range.Text = strText;
                                i++;
                            }
                        }
                    }

                }
            }
            else if (violationType == "Departmental" && violationName == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (rd.Period = '" + period + "') AND (SYint between " + syFrom + " and " + syTo + ") AND (si.ResidenceStatus = '" + residence + "') AND (ViolationType = 'Departmental')", conn))
                {
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            for (r = 2; r <= row; r++)
                            {
                                int i = 0;
                                reader.Read();
                                for (c = 1; c <= column; c++)
                                {
                                    //1
                                    int recordNo = Convert.ToInt32(reader.GetValue(0));
                                    //2
                                    int studNoIndex = reader.GetOrdinal("StudentNo");
                                    int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                    //3
                                    int fullNameIndex = reader.GetOrdinal("Full Name");
                                    string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                    //4
                                    int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                    string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                    //5
                                    int dateIndex = reader.GetOrdinal("dateCommitted");
                                    DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                    string date = myDate.ToString("MM/dd/yyyy");
                                    //6
                                    int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                    int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                    //7
                                    int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                    string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                    //8
                                    int violationNameIndex = reader.GetOrdinal("ViolationName");
                                    string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                    //9
                                    int remarksIndex = reader.GetOrdinal("remarks");
                                    string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                    genRep[0] = recordNo.ToString();
                                    genRep[1] = studNo.ToString();
                                    genRep[2] = fullName;
                                    genRep[3] = residence;
                                    genRep[4] = date;
                                    genRep[5] = violationCode.ToString();
                                    genRep[6] = violationType;
                                    genRep[7] = violationName;
                                    genRep[8] = remarks;
                                    strText = genRep[i];
                                    oTable.Cell(r, c).Range.Text = strText;
                                    i++;
                                }
                            }
                        }
                    }
                }
            }

            else if (period == "ALL" && violationType == "ALL" && residence == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (SYint between " + syFrom + " and " + syTo + ")", conn))
                {
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            for (r = 2; r <= row; r++)
                            {
                                int i = 0;
                                reader.Read();
                                for (c = 1; c <= column; c++)
                                {
                                    //1
                                    int recordNo = Convert.ToInt32(reader.GetValue(0));
                                    //2
                                    int studNoIndex = reader.GetOrdinal("StudentNo");
                                    int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                    //3
                                    int fullNameIndex = reader.GetOrdinal("Full Name");
                                    string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                    //4
                                    int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                    string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                    //5
                                    int dateIndex = reader.GetOrdinal("dateCommitted");
                                    DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                    string date = myDate.ToString("MM/dd/yyyy");
                                    //6
                                    int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                    int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                    //7
                                    int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                    string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                    //8
                                    int violationNameIndex = reader.GetOrdinal("ViolationName");
                                    string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                    //9
                                    int remarksIndex = reader.GetOrdinal("remarks");
                                    string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                    genRep[0] = recordNo.ToString();
                                    genRep[1] = studNo.ToString();
                                    genRep[2] = fullName;
                                    genRep[3] = residence;
                                    genRep[4] = date;
                                    genRep[5] = violationCode.ToString();
                                    genRep[6] = violationType;
                                    genRep[7] = violationName;
                                    genRep[8] = remarks;
                                    strText = genRep[i];
                                    oTable.Cell(r, c).Range.Text = strText;
                                    i++;
                                }
                            }
                        }
                    }
                }
            }
            else if (period == "ALL" && violationType == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (SYint between " + syFrom + " and " + syTo + ") AND (si.ResidenceStatus = '" + residence + "')", conn))
                {
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        {
                            for (r = 2; r <= row; r++)
                            {
                                int i = 0;
                                reader.Read();
                                for (c = 1; c <= column; c++)
                                {
                                    //1
                                    int recordNo = Convert.ToInt32(reader.GetValue(0));
                                    //2
                                    int studNoIndex = reader.GetOrdinal("StudentNo");
                                    int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                    //3
                                    int fullNameIndex = reader.GetOrdinal("Full Name");
                                    string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                    //4
                                    int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                    string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                    //5
                                    int dateIndex = reader.GetOrdinal("dateCommitted");
                                    DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                    string date = myDate.ToString("MM/dd/yyyy");
                                    //6
                                    int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                    int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                    //7
                                    int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                    string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                    //8
                                    int violationNameIndex = reader.GetOrdinal("ViolationName");
                                    string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                    //9
                                    int remarksIndex = reader.GetOrdinal("remarks");
                                    string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                    genRep[0] = recordNo.ToString();
                                    genRep[1] = studNo.ToString();
                                    genRep[2] = fullName;
                                    genRep[3] = residence;
                                    genRep[4] = date;
                                    genRep[5] = violationCode.ToString();
                                    genRep[6] = violationType;
                                    genRep[7] = violationName;
                                    genRep[8] = remarks;
                                    strText = genRep[i];
                                    oTable.Cell(r, c).Range.Text = strText;
                                    i++;
                                }

                            }
                        }
                    }
                }
            }
            else if (period == "ALL" && residence == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (vd.ViolationType = '" + violationType + "') AND (vd.ViolationName = '" + violationName + "') AND (SYint between " + syFrom + " and " + syTo + ")", conn))
                {
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        for (r = 2; r <= row; r++)
                        {
                            int i = 0;
                            reader.Read();
                            for (c = 1; c <= column; c++)
                            {
                                //1
                                int recordNo = Convert.ToInt32(reader.GetValue(0));
                                //2
                                int studNoIndex = reader.GetOrdinal("StudentNo");
                                int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                //3
                                int fullNameIndex = reader.GetOrdinal("Full Name");
                                string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                //4
                                int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                //5
                                int dateIndex = reader.GetOrdinal("dateCommitted");
                                DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                string date = myDate.ToString("MM/dd/yyyy");
                                //6
                                int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                //7
                                int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                //8
                                int violationNameIndex = reader.GetOrdinal("ViolationName");
                                string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                //9
                                int remarksIndex = reader.GetOrdinal("remarks");
                                string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                genRep[0] = recordNo.ToString();
                                genRep[1] = studNo.ToString();
                                genRep[2] = fullName;
                                genRep[3] = residence;
                                genRep[4] = date;
                                genRep[5] = violationCode.ToString();
                                genRep[6] = violationType;
                                genRep[7] = violationName;
                                genRep[8] = remarks;
                                strText = genRep[i];
                                oTable.Cell(r, c).Range.Text = strText;
                                i++;
                            }
                            
                        }
                    }
                }
            } //

            else if (violationType == "ALL" && residence == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (rd.Period = '" + period + "') AND (SYint between " + syFrom + " and " + syTo + ")", conn))
                {
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        for (r = 2; r <= row; r++)
                        {
                            int i = 0;
                            reader.Read();
                            for (c = 1; c <= column; c++)
                            {
                                //1
                                int recordNo = Convert.ToInt32(reader.GetValue(0));
                                //2
                                int studNoIndex = reader.GetOrdinal("StudentNo");
                                int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                //3
                                int fullNameIndex = reader.GetOrdinal("Full Name");
                                string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                //4
                                int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                //5
                                int dateIndex = reader.GetOrdinal("dateCommitted");
                                DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                string date = myDate.ToString("MM/dd/yyyy");
                                //6
                                int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                //7
                                int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                //8
                                int violationNameIndex = reader.GetOrdinal("ViolationName");
                                string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                //9
                                int remarksIndex = reader.GetOrdinal("remarks");
                                string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                genRep[0] = recordNo.ToString();
                                genRep[1] = studNo.ToString();
                                genRep[2] = fullName;
                                genRep[3] = residence;
                                genRep[4] = date;
                                genRep[5] = violationCode.ToString();
                                genRep[6] = violationType;
                                genRep[7] = violationName;
                                genRep[8] = remarks;
                                strText = genRep[i];
                                oTable.Cell(r, c).Range.Text = strText;
                                i++;
                            }
                        }
                    }
                }
            }
            else if (violationType == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (rd.Period = '" + period + "') AND (SYint between " + syFrom + " and " + syTo + ") AND (si.ResidenceStatus = '" + residence + "')", conn))
                {
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        for (r = 2; r <= row; r++)
                        {
                            int i = 0;
                            reader.Read();
                            for (c = 1; c <= column; c++)
                            {
                                //1
                                int recordNo = Convert.ToInt32(reader.GetValue(0));
                                //2
                                int studNoIndex = reader.GetOrdinal("StudentNo");
                                int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                //3
                                int fullNameIndex = reader.GetOrdinal("Full Name");
                                string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                //4
                                int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                //5
                                int dateIndex = reader.GetOrdinal("dateCommitted");
                                DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                string date = myDate.ToString("MM/dd/yyyy");
                                //6
                                int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                //7
                                int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                //8
                                int violationNameIndex = reader.GetOrdinal("ViolationName");
                                string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                //9
                                int remarksIndex = reader.GetOrdinal("remarks");
                                string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                genRep[0] = recordNo.ToString();
                                genRep[1] = studNo.ToString();
                                genRep[2] = fullName;
                                genRep[3] = residence;
                                genRep[4] = date;
                                genRep[5] = violationCode.ToString();
                                genRep[6] = violationType;
                                genRep[7] = violationName;
                                genRep[8] = remarks;
                                strText = genRep[i];
                                oTable.Cell(r, c).Range.Text = strText;
                                i++;
                            }
                        }
                    }
                }
            }
            else if (period == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (vd.ViolationType = '" + violationType + "') AND (vd.ViolationName = '" + violationName + "') AND (SYint between " + syFrom + " and " + syTo + ") AND (si.ResidenceStatus = '" + residence + "')", conn))
                {
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        for (r = 2; r <= row; r++)
                        {
                            int i = 0;
                            reader.Read();
                            for (c = 1; c <= column; c++)
                            {
                                //1
                                int recordNo = Convert.ToInt32(reader.GetValue(0));
                                //2
                                int studNoIndex = reader.GetOrdinal("StudentNo");
                                int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                //3
                                int fullNameIndex = reader.GetOrdinal("Full Name");
                                string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                //4
                                int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                //5
                                int dateIndex = reader.GetOrdinal("dateCommitted");
                                DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                string date = myDate.ToString("MM/dd/yyyy");
                                //6
                                int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                //7
                                int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                //8
                                int violationNameIndex = reader.GetOrdinal("ViolationName");
                                string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                //9
                                int remarksIndex = reader.GetOrdinal("remarks");
                                string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                genRep[0] = recordNo.ToString();
                                genRep[1] = studNo.ToString();
                                genRep[2] = fullName;
                                genRep[3] = residence;
                                genRep[4] = date;
                                genRep[5] = violationCode.ToString();
                                genRep[6] = violationType;
                                genRep[7] = violationName;
                                genRep[8] = remarks;
                                strText = genRep[i];
                                oTable.Cell(r, c).Range.Text = strText;
                                i++;
                            }
                        }
                    }
                }
            } //
            else if (residence == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (vd.ViolationType = '" + violationType + "') AND (rd.Period = '" + period + "') AND (SYint between " + syFrom + " and " + syTo + ")", conn))
                {
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        for (r = 2; r <= row; r++)
                        {
                            int i = 0;
                            reader.Read();
                            for (c = 1; c <= column; c++)
                            {
                                //1
                                int recordNo = Convert.ToInt32(reader.GetValue(0));
                                //2
                                int studNoIndex = reader.GetOrdinal("StudentNo");
                                int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                //3
                                int fullNameIndex = reader.GetOrdinal("Full Name");
                                string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                //4
                                int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                //5
                                int dateIndex = reader.GetOrdinal("dateCommitted");
                                DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                string date = myDate.ToString("MM/dd/yyyy");
                                //6
                                int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                //7
                                int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                //8
                                int violationNameIndex = reader.GetOrdinal("ViolationName");
                                string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                //9
                                int remarksIndex = reader.GetOrdinal("remarks");
                                string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                genRep[0] = recordNo.ToString();
                                genRep[1] = studNo.ToString();
                                genRep[2] = fullName;
                                genRep[3] = residence;
                                genRep[4] = date;
                                genRep[5] = violationCode.ToString();
                                genRep[6] = violationType;
                                genRep[7] = violationName;
                                genRep[8] = remarks;
                                strText = genRep[i];
                                oTable.Cell(r, c).Range.Text = strText;
                                i++;
                            }
                        }
                    }
                }
            }
            else
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.firstName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SYint as SYint, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (vd.ViolationType = '" + violationType + "') AND (vd.ViolationName = '" + violationName + "') AND (rd.Period = '" + period + "') AND (SYint between " + syFrom + " and " + syTo + ") AND (si.ResidenceStatus = '" + residence + "')", conn))
                {
                    using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                    {
                        for (r = 2; r <= row; r++)
                        {
                            int i = 0;
                            reader.Read();
                            for (c = 1; c <= column; c++)
                            {
                                //1
                                int recordNo = Convert.ToInt32(reader.GetValue(0));
                                //2
                                int studNoIndex = reader.GetOrdinal("StudentNo");
                                int studNo = Convert.ToInt32(reader.GetValue(studNoIndex));
                                //3
                                int fullNameIndex = reader.GetOrdinal("Full Name");
                                string fullName = Convert.ToString(reader.GetValue(fullNameIndex));
                                //4
                                int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                                string residence = Convert.ToString(reader.GetValue(residenceIndex));
                                //5
                                int dateIndex = reader.GetOrdinal("dateCommitted");
                                DateTime myDate = Convert.ToDateTime(reader.GetValue(dateIndex));
                                string date = myDate.ToString("MM/dd/yyyy");
                                //6
                                int violationCodeIndex = reader.GetOrdinal("ViolationCode");
                                int violationCode = Convert.ToInt32(reader.GetValue(violationCodeIndex));
                                //7
                                int violationTypeIndex = reader.GetOrdinal("ViolationType");
                                string violationType = Convert.ToString(reader.GetValue(violationTypeIndex));
                                //8
                                int violationNameIndex = reader.GetOrdinal("ViolationName");
                                string violationName = Convert.ToString(reader.GetValue(violationNameIndex));
                                //9
                                int remarksIndex = reader.GetOrdinal("remarks");
                                string remarks = Convert.ToString(reader.GetValue(remarksIndex));
                                genRep[0] = recordNo.ToString();
                                genRep[1] = studNo.ToString();
                                genRep[2] = fullName;
                                genRep[3] = residence;
                                genRep[4] = date;
                                genRep[5] = violationCode.ToString();
                                genRep[6] = violationType;
                                genRep[7] = violationName;
                                genRep[8] = remarks;
                                strText = genRep[i];
                                oTable.Cell(r, c).Range.Text = strText;
                                i++;

                            }
                        }
                    }
                }
            }
            oTable.Rows[1].Range.Font.Bold = 1;
            conn.Close();
            wrdRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;

            object noReset = false;
            object password = System.String.Empty;
            object useIRM = false;
            object enforceStyleLock = false;
            oDoc.Protect(Word.WdProtectionType.wdAllowOnlyReading, ref noReset, ref password, ref useIRM, ref enforceStyleLock);
            oDoc.PrintOut();
        }
    }
}
