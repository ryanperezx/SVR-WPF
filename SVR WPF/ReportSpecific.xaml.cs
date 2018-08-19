using System;
using System.Windows;
using Word = Microsoft.Office.Interop.Word;
using System.Reflection;
using System.Data.SqlServerCe;

namespace SVR_WPF
{

    public partial class ReportSpecific : Window
    {
        public int studNo;
        int institutionalCount, departmentalCount, academicCount, probiCount, lastChanceCount;
        string residence, fullName;
        int i = 1;
        int row = 1, column = 8;
        string[] records = new string[8];
        SqlCeConnection conn = DBUtils.GetDBConnection();

        public ReportSpecific(int studNo)
        {
            InitializeComponent();
            this.studNo = studNo;
            conn.Open();
            using (SqlCeCommand cmd = new SqlCeCommand("SELECT LastName + ', ' + GivenName + ' ' + COALESCE(MiddleName, '') AS [Full Name], ResidenceStatus, CounterLastChance, CounterProbi FROM StudentInfo WHERE StudentNo = @studentNo", conn))
            {
                cmd.Parameters.AddWithValue("@studentNo", studNo);
                using (SqlCeDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                {
                    if (reader.Read())
                    {
                        int fullNameIndex = reader.GetOrdinal("Full Name");
                        fullName = Convert.ToString(reader.GetValue(fullNameIndex));

                        int residenceIndex = reader.GetOrdinal("ResidenceStatus");
                        residence = Convert.ToString(reader.GetValue(residenceIndex));

                        int probiCountIndex = reader.GetOrdinal("CounterProbi");
                        probiCount = Convert.ToInt32(reader.GetValue(probiCountIndex));

                        int lastChanceIndex = reader.GetOrdinal("CounterLastChance");
                        lastChanceCount = Convert.ToInt32(reader.GetValue(lastChanceIndex));

                    }
                }
            }
            txtStudNo.Text = studNo.ToString();
            txtResidence.Text = residence;
            txtFullName.Text = fullName;
            txtLC.Text = lastChanceCount.ToString();
            txtProb.Text = probiCount.ToString();
            updateListView();
        }

        private void updateListView()
        {
            using (SqlCeCommand cmd1 = new SqlCeCommand("SELECT * FROM RecordDetails INNER JOIN ViolationDetails ON ViolationDetails.ViolationCode = RecordDetails.ViolationCode WHERE RecordDetails.StudentNo = @studNo", conn))
            {
                lvSpeReport.Items.Clear();
                cmd1.Parameters.AddWithValue("@studNo", studNo);
                using (SqlCeDataReader dr = cmd1.ExecuteResultSet(ResultSetOptions.Scrollable))
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            int recordNo = Convert.ToInt32(dr.GetValue(0));

                            int dateIndex = dr.GetOrdinal("DateCommitted");
                            DateTime myDate = dr.GetDateTime(dateIndex);
                            string date = myDate.ToString("MM/dd/yyyy");

                            int periodIndex = dr.GetOrdinal("Period");
                            string period = Convert.ToString(dr.GetValue(periodIndex));

                            int syIndex = dr.GetOrdinal("SY");
                            string sy = Convert.ToString(dr.GetValue(syIndex));

                            int violationCode = Convert.ToInt32(dr.GetValue(2));

                            int violationTypeIndex = dr.GetOrdinal("ViolationType");
                            string violationType = Convert.ToString(dr.GetValue(violationTypeIndex));

                            if (violationType.Equals("Institutional"))
                                institutionalCount += 1;
                            else if (violationType.Equals("Departmental"))
                                departmentalCount += 1;
                            else if (violationType.Equals("Academic"))
                                academicCount += 1;

                            int violationNameIndex = dr.GetOrdinal("ViolationName");
                            string violationName = Convert.ToString(dr.GetValue(violationNameIndex));

                            int remarksIndex = dr.GetOrdinal("Remarks");
                            string remarks = Convert.ToString(dr.GetValue(remarksIndex));

                            records[0] = recordNo.ToString();
                            records[1] = date;
                            records[2] = period;
                            records[3] = sy;
                            records[4] = violationCode.ToString();
                            records[5] = violationType;
                            records[6] = violationName;
                            records[7] = remarks;

                            lvSpeReport.Items.Add(new ListViewSpeReport
                            {
                                i = this.i,
                                recordNo = int.Parse(records[0]),
                                dateCommitted= records[1],
                                period = records[2],
                                schoolYear = records[3],
                                violationCode = records[4],
                                violationType = records[5],
                                violationName = records[6],
                                remarks = records[7]
                            });
                            i++;
                            txtDepartViolation.Text = departmentalCount.ToString();
                            txtInstiViolation.Text = institutionalCount.ToString();
                            txtAcademicViolation.Text = academicCount.ToString();
                            row++;
                        }
                    }
                }
            }
        }

        private void btnGenReport_Click(object sender, RoutedEventArgs e)
        {
            object oMissing = Missing.Value;
            object oEndOfDoc = "\\endofdoc"; /* \endofdoc is a predefined bookmark */

            //Start Word and create a new document.
            Word._Application oWord;
            Word._Document oDoc;
            oWord = new Word.Application();
            oWord.Visible = true;
            oDoc = oWord.Documents.Add(ref oMissing, ref oMissing,
            ref oMissing, ref oMissing);

            //Insert a paragraph at the beginning of the document.
            Word.Paragraph oPara1;
            oPara1 = oDoc.Content.Paragraphs.Add(ref oMissing);
            oPara1.Range.Text = fullName;
            oPara1.Range.Font.Bold = 1;
            oPara1.Format.SpaceAfter = 1;    //1 pt spacing after paragraph.
            oPara1.Range.InsertParagraphAfter();

            //Insert a paragraph at the end of the document.
            Word.Paragraph oPara2;
            object oRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oPara2 = oDoc.Content.Paragraphs.Add(ref oRng);
            oPara2.Range.Text = "Student no: " + studNo.ToString();
            oPara1.Range.Font.Bold = 0;
            oPara2.Format.SpaceAfter = 1;
            oPara2.Range.InsertParagraphAfter();

            //Insert another paragraph.
            Word.Paragraph oPara3;
            oRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oPara3 = oDoc.Content.Paragraphs.Add(ref oRng);
            oPara3.Range.Text = "Residence Status: " + residence;
            oPara3.Range.Font.Bold = 0;
            oPara3.Format.SpaceAfter = 24;
            oPara3.Range.InsertParagraphAfter();

            //Insert another paragraph.
            Word.Paragraph oPara4;
            oRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oPara4 = oDoc.Content.Paragraphs.Add(ref oRng);
            oPara4.Range.Text = "Violations: ";
            oPara4.Range.Font.Bold = 1;
            oPara4.Range.Font.Size = 14;
            oPara4.Format.SpaceAfter = 1;
            oPara4.Range.InsertParagraphAfter();

            //Insert another paragraph.
            Word.Paragraph oPara5;
            oRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oPara5 = oDoc.Content.Paragraphs.Add(ref oRng);
            oPara5.Range.Text = "No. of Institutional Violation: " + institutionalCount.ToString();
            oPara5.Range.Font.Size = 11;
            oPara5.Range.Font.Bold = 0;
            oPara5.Format.SpaceAfter = 1;
            oPara5.Range.InsertParagraphAfter();

            //Insert another paragraph.
            Word.Paragraph oPara6;
            oRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oPara6 = oDoc.Content.Paragraphs.Add(ref oRng);
            oPara6.Range.Text = "No. of Academic Violation: " + academicCount.ToString();
            oPara6.Format.SpaceAfter = 1;
            oPara6.Range.InsertParagraphAfter();

            //Insert another paragraph.
            Word.Paragraph oPara7;
            oRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oPara7 = oDoc.Content.Paragraphs.Add(ref oRng);
            oPara7.Range.Text = "No. of Departmental Violation: " + departmentalCount.ToString();
            oPara7.Format.SpaceAfter = 24;
            oPara7.Range.InsertParagraphAfter();

            //Insert a row x column table, fill it with data, and make the first row bold.
            Word.Table oTable;
            Word.Range wrdRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oTable = oDoc.Tables.Add(wrdRng, row, column, ref oMissing, ref oMissing);
            oTable.Range.ParagraphFormat.SpaceAfter = 6;
            int r, c;
            string strText;
            oTable.Cell(1, 1).Range.Text = "Record No.";
            oTable.Cell(1, 2).Range.Text = "Date Committed";
            oTable.Cell(1, 3).Range.Text = "Semester";
            oTable.Cell(1, 4).Range.Text = "School Year";
            oTable.Cell(1, 5).Range.Text = "Violation Code";
            oTable.Cell(1, 6).Range.Text = "Violation Type";
            oTable.Cell(1, 7).Range.Text = "Violation Name";
            oTable.Cell(1, 8).Range.Text = "Remarks";
            using (SqlCeCommand cmd1 = new SqlCeCommand("SELECT * FROM RecordDetails INNER JOIN ViolationDetails ON ViolationDetails.ViolationCode = RecordDetails.ViolationCode WHERE RecordDetails.StudentNo = @studNo", conn))
            {
                cmd1.Parameters.AddWithValue("@studNo", studNo);
                using (SqlCeDataReader dr = cmd1.ExecuteResultSet(ResultSetOptions.Scrollable))
                {
                    if (dr.HasRows)
                    {
                        for (r = 2; r <= row; r++)
                        {
                            int i = 0;
                            dr.Read();
                            for (c = 1; c <= column; c++)
                            {
                                int recordNo = Convert.ToInt32(dr.GetValue(0));

                                int dateIndex = dr.GetOrdinal("DateCommitted");
                                DateTime myDate = dr.GetDateTime(dateIndex);
                                string date = myDate.ToString("MM/dd/yyyy");

                                int periodIndex = dr.GetOrdinal("Period");
                                string period = Convert.ToString(dr.GetValue(periodIndex));

                                int syIndex = dr.GetOrdinal("SY");
                                string sy = Convert.ToString(dr.GetValue(syIndex));

                                int violationCode = Convert.ToInt32(dr.GetValue(2));

                                int violationTypeIndex = dr.GetOrdinal("ViolationType");
                                string violationType = Convert.ToString(dr.GetValue(violationTypeIndex));

                                if (violationType.Equals("Institutional"))
                                    institutionalCount += 1;
                                else if (violationType.Equals("Departmental"))
                                    departmentalCount += 1;

                                int violationNameIndex = dr.GetOrdinal("ViolationName");
                                string violationName = Convert.ToString(dr.GetValue(violationNameIndex));

                                int remarksIndex = dr.GetOrdinal("Remarks");
                                string remarks = Convert.ToString(dr.GetValue(remarksIndex));

                                records[0] = recordNo.ToString();
                                records[1] = date;
                                records[2] = period;
                                records[3] = sy;
                                records[4] = violationCode.ToString();
                                records[5] = violationType;
                                records[6] = violationName;
                                records[7] = remarks;
                                strText = records[i];
                                oTable.Cell(r, c).Range.Text = strText;
                                i++;
                            }
                        }
                    }
                }
            }
            oTable.Rows[1].Range.Font.Bold = 1;

            //Add text after the chart.
            wrdRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;


        }
    }
}
