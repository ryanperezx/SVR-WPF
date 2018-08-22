using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Windows;
using System.IO;
using System.Threading;
using System.Data.SqlServerCe;
using System.Data.Common;
using System.Net.NetworkInformation;
namespace SVR_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        static string[] Scopes = { DriveService.Scope.Drive, DriveService.Scope.DriveFile };

        static string ApplicationName = "SVR Database";
        bool check;

        protected void Application_Startup(object sender, StartupEventArgs e)
        {
            CheckConnection(check);
            if (check == true)
            {
                UserCredential credential;
                credential = GetCredentials();
                MessageBox.Show("1");

                DateTime dt = new DateTime();
                DateTime dy = new DateTime();
                dt = DateTime.Today;

                var service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });

                string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                using (SqlCeConnection conn = DBUtils.GetDBConnection())
                {
                    conn.Open();
                    using (SqlCeCommand cmd = new SqlCeCommand("SELECT TOP 1 * from BackupDate ORDER BY No DESC", conn))
                    {
                        using (DbDataReader reader = cmd.ExecuteResultSet(ResultSetOptions.Scrollable))
                        {
                            reader.Read();

                            int dateIndex = reader.GetOrdinal("backupDate");
                            DateTime myDate = reader.GetDateTime(dateIndex);
                            dy = myDate;

                        }
                    }
                    if (dt.Date > dy.Date)
                    {
                        using (SqlCeCommand cmd1 = new SqlCeCommand("INSERT INTO BackupDate (backupDate) VALUES (@backUpDate)", conn))
                        {
                            cmd1.Parameters.AddWithValue("@backUpDate", dt);
                            cmd1.ExecuteNonQuery();
                        }
                    }
                }
                if (dt.Date > dy.Date)
                {
                    uploadFile(folder + "\\Student Violation Records\\StudentViolationRecords.sdf", service);
                }
            }
            else
            {
                MessageBox.Show("2");
            }
            MainWindow mw = new MainWindow();
            mw.Show();
        }

        private static void uploadFile(string path, DriveService service)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File();
            fileMetadata.Name = Path.GetFileName(path);
            fileMetadata.MimeType = "application/unknown";
            FilesResource.CreateMediaUpload request;
            using (var stream = new System.IO.FileStream(path, System.IO.FileMode.Open))
            {
                request = service.Files.Create(fileMetadata, stream, "application/unknown");
                request.Fields = "id";
                request.Upload();
            }

            var file = request.ResponseBody;
        }

        private static UserCredential GetCredentials()
        {
            UserCredential credential;
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            using (var stream = new FileStream(folder + "\\Student Violation Records\\credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

                credPath = Path.Combine(credPath, ".credentials/drive-dotnet-quickstart.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            return credential;
        }

        private bool CheckConnection(bool check)
        {
            try
            {
                Ping myPing = new Ping();
                String host = "google.com";
                byte[] buffer = new byte[32];
                int timeout = 2000;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                if (reply.Status == IPStatus.Success)
                {
                    check = true;
                }
                else
                {
                    check = false;
                }
                return check;
            }
            catch (Exception)
            {
                return check = false;
            }
        }
    }
}
