using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Windows;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SVR_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        static string[] Scopes = { DriveService.Scope.Drive,DriveService.Scope.DriveFile };

        static string ApplicationName = "SVR Database";

        protected void Application_Startup(object sender, StartupEventArgs e)
        {
            UserCredential credential;
            credential = GetCredentials();


            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            uploadFile(folder + "\\Student Violation Records\\StudentViolationRecords.sdf", service);



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

            MessageBox.Show("File ID: " + file.Id);
        }

        private static UserCredential GetCredentials()
        {
            UserCredential credential;
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            using (var stream = new FileStream( folder + "\\Student Violation Records\\credentials.json", FileMode.Open, FileAccess.Read))
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
    }
}
