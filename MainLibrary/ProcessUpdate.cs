using System;
using System.Threading;
using System.IO;
using FTPMethodConnect;
using System.Text.RegularExpressions;
using System.IO.Compression;

namespace MainLibrary
{
    public class ProcessUpdate
    {
        #region Fields
        /// <summary>
        /// Đường dẫn đến cấu hình file hiện tại được chọn.
        /// </summary>
        public static string localConfigFileUri;

        /// <summary>
        /// Tên của file được chọn.
        /// </summary>
        public static string nameOfFileCheck;

        /// <summary>
        /// Chỉ số của file được chọn trong danh sách.
        /// </summary>
        public static int index;

        /// <summary>
        /// Thời gian nghỉ giữa 2 lần kiểm tra.
        /// </summary>
        private const int CheckInterval = 900;

        /// <summary>
        /// Biến ghi nhận trạng thái Update.
        /// </summary>
        public static int flagForUpdate = 1;
        #endregion

        #region Fields
        /// <summary>
        /// Thời gian thực thi chương trình.
        /// </summary>
        private Timer _timer;

        /// <summary>
        /// Ghi nhận quá trình kiểm tra nhập hoàn thành hay chưa.
        /// </summary>
        private volatile bool _updating;

        /// <summary>
        /// Đối tượng kiểu <see cref="Manifest"/> lưu trữ thông tin của cấu hình file hiện tại.
        /// </summary>
        private readonly Manifest _localConfig;

        /// <summary>
        /// Đối tượng kiểu <see cref="Manifest" /> lưu trữ thông tin của cấu hình file từ xa.
        /// </summary>
        private Manifest _remoteConfig;

        /// <summary>
        /// Cấu hình file hiện tại.
        /// </summary>
        private readonly FileInfo _localConfigFile;
        #endregion

        #region initialization

        /// <summary>
        /// Khởi tạo một đối tượng kiểu <see cref="ProcessUpdate" />.
        /// </summary>
        public ProcessUpdate() : this(new FileInfo(localConfigFileUri))
        {

        }


        /// <summary>
        /// Khởi tạo một đối tượng kiểu <see cref="ProcessUpdate"/>.
        /// </summary>
        /// <param name="configFile">The configuration file.</param>
        public ProcessUpdate(FileInfo configFile)
        {
            Log.Debug = true;

            // Tham chiếu đến cấu hình file được truyền vào
            _localConfigFile = configFile;

            // In ra file được chọn để kiểm tra cập nhật
            Console.WriteLine("Dang tai, vui long cho...");
            Console.WriteLine("Khoi tao su dung file: {0}", configFile.FullName);

            if (!configFile.Exists) // Nếu file không tồn tại
            {
                Console.WriteLine("Cau hinh File {0} khong ton tai... Dang dung lai...", configFile.FullName);
                return;
            }

            // Đọc toàn bộ dữ liệu từ file được chọn
            string data = File.ReadAllText(configFile.FullName);
            // Khởi tạo đối tượng kiểu Manifest với dữ liệu đã được truyền vào
            this._localConfig = new Manifest(data);
        }
        #endregion

        #region 


        /// <summary>
        ///  Thực thi một cách đơn giản.
        /// </summary>
        public void StartSimpleMornitoring()
        {
            Console.WriteLine("Chuan bi... Vui long cho...");
            _timer = new Timer(CheckSimpleUpdate, null, 5000, 0);
        }


        /// <summary>
        /// Thực thi một cách bình thường.
        /// </summary>
        public void StartNormalMornitoring()
        {
            Console.WriteLine("Chuan bi kiem tra cap nhat va tien hanh cap nhat(neu co). Vui long cho...");
            _timer = new Timer(CheckNormalUpdate, null, 5000, 0);
        }


        /// <summary>
        /// Thực thi một cách đầy đủ.
        /// </summary>
        public void StartFullMornitoring()
        {
            Console.WriteLine("Chuan bi kiem tra cap nhat va tien hanh cap nhat(neu co). Vui long cho...");
            _timer = new Timer(CheckFullUpdate, null, 5000, 0);
        }


        /// <summary>
        /// Dừng lại màn hình.
        /// </summary>
        public void StopMornitoring()
        {
            Log.Write("Dang dung lai");

            if (_timer == null)
            {
                Log.Write("Da ket thuc!");
                return;
            }
            _timer.Dispose();
        }


        /// <summary>
        /// Kiểm tra cập nhật một cách đơn giản.
        /// </summary>
        /// <param name="state">State.</param>
        private void CheckSimpleUpdate(object state)
        {
            try
            {
                Log.Write("Dang kiem tra...");

                if (_updating) // nếu _updating != null, việc kiểm tra cập nhật đã hoàn thành.
                {
                    Log.Write("Kiem tra cap nhat da duoc hoan thanh.");
                    Log.Write("Ket thuc kiem tra");
                }

                // Lấy đường dẫn của thư mục chứa file xml từ xa.
                var remoteUri = new Uri(this._localConfig.BaseUri);
                // In ra đường dẫn phiên bản hiện tại đang sử dụng
                Log.Write("Nhan du lieu o dia chi: '{0}'.", remoteUri.AbsoluteUri);

                // Khởi tạo một đối tượng kiểu FTP
                var objFTP = new FTP();
                // Lấy về nội dung của file trên host để so sánh kiểm tra cập nhật với file đã cài.
                string data = objFTP.getContentOfFile(nameOfFileCheck);
                // Khởi tạo đối tượng kiểu Manifest lưu trữ dữ liệu file trên host.
                _remoteConfig = new Manifest(data);

                if (_remoteConfig == null) // Nếu việc khởi tạo là không thành công.
                {
                    Log.Write("Du lieu khong duoc tim thay, dung lai...");
                    return;
                }

                // So sánh mã bảo mật của file đã cài và file trên host
                if (_remoteConfig.CodeSecurity != _localConfig.CodeSecurity)
                {
                    Log.Write("Ma bao mat khong trung nhau... Khong hop le. Khong cap nhat. Dang thoat...");
                    Log.Write("Nhan Enter.");
                    return;
                }

                // In ra thông tin số phiên bản của các phiên bản cũ và mới
                Log.Write("Cau hinh tu xa la kha dung.");
                Log.Write("Phien ban hien tai: {0}", _localConfig.Version);
                Log.Write("Phien ban tu xa: {0}", _remoteConfig.Version);

                // Tiến hành so sánh 2 sô phiên bản
                int compare = _localConfig.Version.CompareTo(_remoteConfig.Version);
                if (compare >= 0) // Phiên bản đang cài đặt là mới nhất
                {
                    Log.Write("Phien ban hien tai la moi nhat. Kiem tra ket thuc");
                    return;
                }
                // Ngược lại, có phiên bản mới hơn.
                Log.Write("Co phien ban moi hon. Ban co the xem xet cap nhat luc sau. Ket thuc kiem tra.");
            }
            catch (Exception ex)
            {
                Log.Write("Co loi xay ra: " + ex.ToString());
                return;
            }
        }


        /// <summary>
        /// Kiểm tra cập nhật một cách bình thường.
        /// </summary>
        /// <param name="state">State.</param>
        private void CheckNormalUpdate(object state)
        {
            try
            {
                Log.Write("Dang kiem tra...");

                if (_updating) // nếu _updating != null, việc kiểm tra cập nhật đã hoàn thành.
                {
                    Log.Write("Kiem tra cap nhat da duoc hoan thanh.");
                    Log.Write("Ket thuc kiem tra");
                }

                // Lấy đường dẫn của thư mục chứa file xml từ xa.
                var remoteUri = new Uri(this._localConfig.BaseUri);
                // In ra đường dẫn phiên bản hiện tại đang sử dụng
                Log.Write("Nhan du lieu o dia chi: '{0}'.", remoteUri.AbsoluteUri);

                // Khởi tạo một đối tượng kiểu FTP
                var objFTP = new FTP();
                // Lấy về nội dung của file trên host để so sánh kiểm tra cập nhật với file đã cài.
                string data = objFTP.getContentOfFile(nameOfFileCheck);
                // Khởi tạo đối tượng kiểu Manifest lưu trữ dữ liệu file trên host.
                _remoteConfig = new Manifest(data);

                if (_remoteConfig == null) // Nếu việc khởi tạo là không thành công.
                {
                    Log.Write("Du lieu khong duoc tim thay, dung lai...");
                    return;
                }

                // So sánh mã bảo mật của file đã cài và file trên host
                if (_remoteConfig.CodeSecurity != _localConfig.CodeSecurity)
                {
                    Log.Write("Ma bao mat khong trung nhau... Khong hop le. Khong cap nhat. Dang thoat...");
                    Log.Write("Nhan Enter.");
                    return;
                }

                // In ra thông tin số phiên bản của các phiên bản cũ và mới
                Log.Write("Cau hinh tu xa la kha dung.");
                Log.Write("Phien ban hien tai: {0}", _localConfig.Version);
                Log.Write("Phien ban tu xa: {0}", _remoteConfig.Version);

                // Tiến hành so sánh 2 sô phiên bản
                int compare = _localConfig.Version.CompareTo(_remoteConfig.Version);
                if (compare >= 0) // Phiên bản đang cài đặt là mới nhất
                {
                    Log.Write("Phien ban hien tai la moi nhat. Kiem tra ket thuc");
                    return;
                }
                // Ngược lại, có phiên bản mới hơn.
                Log.Write("Co phien ban moi hon. Dang tai phien ban moi ve thu muc download");

                // Tiến hành tải về File trên host
                _updating = true;
                Update();

                Log.Write("Ket thuc...");
                _updating = false;
            }
            catch (Exception ex)
            {
                Log.Write("Co loi xay ra trong qua trinh cap nhat: " + ex.ToString());
                return;
            }
        }


        /// <summary>
        /// Kiểm tra cập nhật một cách đầy đủ.
        /// </summary>
        /// <param name="state">State.</param>
        private void CheckFullUpdate(object state)
        {
            try
            {
                Log.Write("Dang kiem tra...");

                if (_updating) // nếu _updating != null, việc kiểm tra cập nhật đã hoàn thành.
                {
                    Log.Write("Kiem tra cap nhat da duoc hoan thanh.");
                    Log.Write("Ket thuc kiem tra");
                }

                // Lấy đường dẫn của thư mục chứa file xml từ xa.
                var remoteUri = new Uri(this._localConfig.BaseUri);
                // In ra đường dẫn phiên bản hiện tại đang sử dụng
                Log.Write("Nhan du lieu o dia chi: '{0}'.", remoteUri.AbsoluteUri);

                // Khởi tạo một đối tượng kiểu FTP
                var objFTP = new FTP();
                // Lấy về nội dung của file trên host để so sánh kiểm tra cập nhật với file đã cài.
                string data = objFTP.getContentOfFile(nameOfFileCheck);
                // Khởi tạo đối tượng kiểu Manifest lưu trữ dữ liệu file trên host.
                _remoteConfig = new Manifest(data);

                if (_remoteConfig == null) // Nếu việc khởi tạo là không thành công.
                {
                    Log.Write("Du lieu khong duoc tim thay, dung lai...");
                    return;
                }

                // So sánh mã bảo mật của file đã cài và file trên host
                if (_remoteConfig.CodeSecurity != _localConfig.CodeSecurity)
                {
                    Log.Write("Ma bao mat khong trung nhau... Khong hop le. Khong cap nhat. Dang thoat...");
                    Log.Write("Nhan Enter.");
                    return;
                }

                // In ra thông tin số phiên bản của các phiên bản cũ và mới
                Log.Write("Cau hinh tu xa la kha dung.");
                Log.Write("Phien ban hien tai: {0}", _localConfig.Version);
                Log.Write("Phien ban tu xa: {0}", _remoteConfig.Version);

                // Tiến hành so sánh 2 sô phiên bản
                int compare = _localConfig.Version.CompareTo(_remoteConfig.Version);
                if (compare >= 0) // Phiên bản đang cài đặt là mới nhất
                {
                    Log.Write("Phien ban hien tai la moi nhat. Kiem tra ket thuc");
                    return;
                }
                // Ngược lại, có phiên bản mới hơn.
                Log.Write("Co phien ban moi hon. Dang tai phien ban moi ve thu muc download va tien hanh cai dat.");

                // Tiến hành tải về bản cập nhật mới trên host
                _updating = true;
                Update();
                // Cài đặt bản cập nhật mới thay thế bản cũ.
                installFile();
                _updating = false;
                Log.Write("Cai dat thanh cong. Ket thuc...");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Co loi xay ra: " + ex.ToString());
                return;
            }
        }


        /// <summary>
        /// Cập nhật phần mềm.
        /// </summary>
        private void Update()
        {
            try
            {
                // Tạo đường dẫn cho thư mục download để lưu trữ các file
                // Lưu ý, thư mục download này nằm trong đường dẫn mà bạn nhập vào
                string directoryStore = UpdateApplication.pathStore + @"\download";
                Log.Write("Thu muc luu giu ban cap nhat moi: " + directoryStore);

                if (!Directory.Exists(directoryStore)) // Nếu thư mục download chưa tồn tại, tạo thư mục
                {
                    Log.Write("Dang tao thu muc luu tru file tai ve.");
                    Directory.CreateDirectory(directoryStore);
                }

                // Biến kiểu FileInfo là biến lưu trữ đường dẫn cho file chuẩn bị được tải về
                var info = new FileInfo(Path.Combine(directoryStore, this._remoteConfig.ZipFiles));
                // Lấy ra đường dẫn cho file được tải về
                string urlStore = info.FullName;
                // Tạo thư mục nằm trong thư mục download chứa file tải về để vói đường dẫn được tạo 
                Directory.CreateDirectory(info.DirectoryName);

                // Khai báo đối tượng kiểu FTP
                var ftp = new FTP();
                if (_remoteConfig.ZipFiles.CompareTo("") == 0) // Xem file thư mục chứa bản cập nhật là .zip hay thường
                {
                    // Nếu là thư mục thường
                    ftp.download(_remoteConfig.TenPhanmem, urlStore + @"\" + _remoteConfig.TenPhanmem);
                }
                else
                    // Nếu là thư mục .zip
                    ftp.download(_remoteConfig.ZipFiles, urlStore);
            }
            catch (Exception ex)
            {
                Log.Write("Da xay ra loi: " + ex.ToString());
                flagForUpdate = 0;
                return;
            }
        }


        /// <summary>
        /// Cài đặt phần mềm.
        /// </summary>
        private void installFile()
        {
            if (flagForUpdate == 1) // Ghi nhận việc tải về là thành công.
            {
                Log.Write("Tien hanh cai dat thay the phien ban cu:");
                // Kiểm tra bản được tải về có định dạng là zip hay thường
                if (Regex.IsMatch(_remoteConfig.ZipFiles, @".zip"))
                {
                    Console.WriteLine("Co file .zip");

                    // Lấy về đường dẫn đến thư mục download
                    string directoryStore = UpdateApplication.pathStore + @"\download";
                    // Tạo thư mục download
                    var dir = Directory.CreateDirectory(directoryStore);
                    // Lấy đường dẫn đến thư mục .zip
                    string zipPath = Path.Combine(directoryStore, this._remoteConfig.ZipFiles);
                    // Extract các file trong thư mục .zip ra thư mục download
                    ZipFile.ExtractToDirectory(zipPath, directoryStore);
                    // Xóa thư mục .zip
                    File.Delete(zipPath);

                    // Lấy về danh sách các file thường trong thư mục downloas
                    var files = dir.GetFiles("*.*", SearchOption.AllDirectories);
                    foreach (FileInfo file in files)
                    {
                        // Xóa file cùng tên với file trong thư mục download ở thư mục cha của thư mục download
                        string des = UpdateApplication.pathStore + @"\" + file.ToString();
                        if (File.Exists(des))
                            File.Delete(des);
                        // Copy file từ thư mục download ra ngoài
                        file.CopyTo(des, true);
                        // Xóa file trong thư mục download
                        file.Delete();
                    }
                }
                else
                {
                    // Ngược lại, nếu bản tải về là file thường

                    // Lấy về đường dẫn đến thư mục download
                    string directoryStore = UpdateApplication.pathStore + @"\download";
                    var dir = Directory.CreateDirectory(directoryStore);
                    // Lấy về danh sách các file thường trong thư mục downloas
                    var files = dir.GetFiles("*.*", SearchOption.AllDirectories);
                    foreach (FileInfo file in files)
                    {
                        // Xóa file cùng tên với file trong thư mục download ở thư mục cha của thư mục download
                        string des = UpdateApplication.pathStore + @"\" + file.ToString();
                        if (File.Exists(des))
                            File.Delete(des);
                        // Copy file từ thư mục download ra ngoài
                        file.CopyTo(des, true);
                        // Xóa file trong thư mục download
                        file.Delete();
                    }
                }
            }
        }
        #endregion
    }
}
