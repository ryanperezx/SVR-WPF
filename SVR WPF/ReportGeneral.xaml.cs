using System;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlServerCe;


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
        string[] genRep = new string[10];
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
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.GivenName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SY as SY, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (SY between " + syFrom + " and " + syTo + ") AND (ViolationType = 'Academic')", conn))
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
                                i++;
                            }
                        }
                    }
                }
            }
            else if (violationType == "Academic" && violationName == "ALL" && residence == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.GivenName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SY as SY, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (rd.Period = '" + period + "') AND (SY between " + syFrom + " and " + syTo + ")  AND (ViolationType = 'Academic')", conn))
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
                                i++;
                            }
                        }
                    }
                }
            }
            else if (violationType == "Academic" && violationName == "ALL" && period == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.GivenName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SY as SY, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (SY between " + syFrom + " and " + syTo + ")  AND (ViolationType = 'Academic') AND (si.ResidenceStatus = '" + residence + "')", conn))
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
                                i++;
                            }
                        }
                    }
                }
            }
            else if (violationType == "Academic" && violationName == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.GivenName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SY as SY, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (rd.Period = '" + period + "') AND (SY between " + syFrom + " and " + syTo + ") AND (si.ResidenceStatus = '" + residence + "') AND (ViolationType = 'Academic')", conn))
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
                                i++;
                            }
                        }
                    }
                }
            }

            else if (violationType == "Institutional" && violationName == "ALL" && residence == "ALL" && period == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.GivenName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SY as SY, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (SY between " + syFrom + " and " + syTo + ")  AND (ViolationType = 'Institutional')", conn))
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
                                i++;
                            }
                        }
                    }
                }
            }
            else if (violationType == "Institutional" && violationName == "ALL" && residence == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.GivenName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SY as SY, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (rd.Period = '" + period + "') AND (SY between " + syFrom + " and " + syTo + ")  AND (ViolationType = 'Institutional')", conn))
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
                                i++;
                            }
                        }
                    }
                }
            }
            else if (violationType == "Institutional" && violationName == "ALL" && period == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.GivenName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SY as SY, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (SY between " + syFrom + " and " + syTo + ")  AND (ViolationType = 'Institutional') AND (si.ResidenceStatus = '" + residence + "')", conn))
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
                                i++;
                            }
                        }
                    }
                }
            }
            else if (violationType == "Institutional" && violationName == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.GivenName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SY as SY, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (rd.Period = '" + period + "') AND (SY between " + syFrom + " and " + syTo + ") AND (si.ResidenceStatus = '" + residence + "') AND (ViolationType = 'Institutional')", conn))
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
                                i++;
                            }
                        }
                    }
                }
            }

            else if (violationType == "Departmental" && violationName == "ALL" && residence == "ALL" && period == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.GivenName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SY as SY, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (SY between 2015 and 2019)  AND (ViolationType = 'Departmental')", conn))
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
                                i++;
                            }
                        }
                    }
                }
            }
            else if (violationType == "Departmental" && violationName == "ALL" && residence == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.GivenName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SY as SY, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (rd.Period = '" + period + "') AND (SY between " + syFrom + " and " + syTo + ")  AND (ViolationType = 'Departmental')", conn))
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
                                i++;
                            }
                        }
                    }
                }
            }
            else if (violationType == "Departmental" && violationName == "ALL" && period == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.GivenName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SY as SY, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (SY between " + syFrom + " and " + syTo + ")  AND (ViolationType = 'Departmental') AND (si.ResidenceStatus = '" + residence + "')", conn))
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
                                i++;
                            }
                        }
                    }
                }
            }
            else if (violationType == "Departmental" && violationName == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.GivenName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SY as SY, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (rd.Period = '" + period + "') AND (SY between " + syFrom + " and " + syTo + ") AND (si.ResidenceStatus = '" + residence + "') AND (ViolationType = 'Departmental')", conn))
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
                                i++;
                            }
                        }
                    }
                }
            }

            else if (period == "ALL" && violationType == "ALL" && residence == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.GivenName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SY as SY, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (SY between " + syFrom + " and " + syTo + ")", conn))
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
                                i++;
                            }
                        }
                    }
                }
            }
            else if (period == "ALL" && violationType == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.GivenName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SY as SY, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (SY between " + syFrom + " and " + syTo + ") AND (si.ResidenceStatus = '" + residence + "')", conn))
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
                                i++;
                            }
                        }
                    }
                }
            }
            else if (period == "ALL" && residence == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.GivenName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SY as SY, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (vd.ViolationType = '" + violationType + "') AND (vd.ViolationName = '" + violationName + "') AND (SY between " + syFrom + " and " + syTo + ")", conn))
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
                                i++;
                            }
                        }
                    }
                }
            } //

            else if (violationType == "ALL" && residence == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.GivenName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SY as SY, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (rd.Period = '" + period + "') AND (SY between " + syFrom + " and " + syTo + ")", conn))
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
                                i++;
                            }
                        }
                    }
                }
            }
            else if (violationType == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.GivenName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SY as SY, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (rd.Period = '" + period + "') AND (SY between " + syFrom + " and " + syTo + ") AND (si.ResidenceStatus = '" + residence + "')", conn))
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
                                i++;
                            }
                        }
                    }
                }
            }
            else if (period == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.GivenName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SY as SY, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (vd.ViolationType = '" + violationType + "') AND (vd.ViolationName = '" + violationName + "') AND (SY between " + syFrom + " and " + syTo + ") AND (si.ResidenceStatus = '" + residence + "')", conn))
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
                                i++;
                            }
                        }
                    }
                }
            } //
            else if (residence == "ALL")
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.GivenName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SY as SY, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (vd.ViolationType = '" + violationType + "') AND (rd.Period = '" + period + "') AND (SY between " + syFrom + " and " + syTo + ")", conn))
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
                                i++;
                            }
                        }
                    }
                }
            }
            else
            {
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT rd.RecordNo as RecordNo, rd.StudentNo as StudentNo, rd.ViolationCode as ViolationCode, rd.DateCommitted as DateCommitted, si.LastName + ', ' + si.GivenName + ' ' + COALESCE(si.MiddleName, '') AS [Full Name], si.ResidenceStatus as ResidenceStatus, rd.Period as Period, rd.SY as SY, rd.Remarks as Remarks, vd.ViolationType as ViolationType, vd.ViolationName FROM RecordDetails AS rd INNER JOIN StudentInfo AS si ON rd.StudentNo = si.StudentNo INNER JOIN ViolationDetails AS vd ON rd.ViolationCode = vd.ViolationCode WHERE (vd.ViolationType = '" + violationType + "') AND (vd.ViolationName = '" + violationName + "') AND (rd.Period = '" + period + "') AND (SY between " + syFrom + " and " + syTo + ") AND (si.ResidenceStatus = '" + residence + "')", conn))
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
                                i++;
                            }
                        }
                    }
                }
            } //
            conn.Close();
        }
    }
}
