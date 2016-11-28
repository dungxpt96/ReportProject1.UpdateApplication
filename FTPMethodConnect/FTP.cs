using System;
using System.IO;
using System.Net;
using MainLibrary;

namespace FTPMethodConnect
{
    /// <summary>
    /// Class FTP.
    /// </summary>
    public class FTP
    {
        #region Fields
        /// <summary>
        /// Địa chỉ ftp.
        /// </summary>
        private string host = @"ftp://31.220.16.3";

        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        private string userName = @"u272976158";

        /// <summary>
        /// Mật khẩu.
        /// </summary>
        private string passWord = @"dung5196";

        /// <summary>
        /// Yêu cầu đến FTP.
        /// </summary>
        private FtpWebRequest ftpRequest;

        /// <summary>
        /// Phản hồi từ FTP.
        /// </summary>
        private FtpWebResponse ftpResponse;

        /// <summary>
        /// Dòng tin FTP.
        /// </summary>
        private Stream ftpStream;


        /// <summary>
        /// Bộ đệm.
        /// </summary>
        private int sizeBuffer = 2048;
        #endregion

        #region Methods
        /// <summary>
        /// Tải xuống file.
        /// </summary>
        /// <param name="remoteFile">Remote file.</param>
        /// <param name="localFile">Lhe local file.</param>
        public void download(string remoteFile, string localFile)
        {
            try
            {
                /* Tạo một yêu cầu đến FTP server */
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + remoteFile);

                /* Xác định loại yêu cầu là Download */
                ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;

                /* Đăng nhập vào FTP server */
                ftpRequest.Credentials = new NetworkCredential(userName, passWord);

                /* Sử dụng tùy chọn khi có nghi vấn */
                ftpRequest.UseBinary = true; // Giá trị xác định thể loại dữ liệu cho truyền file
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;

                /* Thiết lập các truyền thông trả về từ FTP server */
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();

                /* Nhận các dòng phản hồi từ FTP server */
                ftpStream = ftpResponse.GetResponseStream();

                /* Mở một luồng File để ghi lại file được tải xuống */
                FileStream localFileStream = new FileStream(localFile, FileMode.Create);

                /* Sử dụng bộ đệm để lưu trữ dữ liệu được tải xuống */
                byte[] byteBuffer = new byte[sizeBuffer];
                int byteRead = ftpStream.Read(byteBuffer, 0, sizeBuffer);

                /* Tải file bằng cách ghi dữ liệu từ bộ đệm cho đến khi việc chuyển dịch là hoàn thành */
                try
                {
                    while (byteRead > 0)
                    {
                        localFileStream.Write(byteBuffer, 0, byteRead);
                        byteRead = ftpStream.Read(byteBuffer, 0, sizeBuffer);
                    }

                    Console.WriteLine("Tai xuong thanh cong!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Tai xuong khong thanh cong!");
                    Console.WriteLine("Loi: " + ex.Message);
                    return;
                }

                /* Giải phóng bộ nhớ */
                ftpResponse.Close();
                localFileStream.Close();
                ftpStream.Close();
                ftpRequest = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Da co loi xay ra trong download " + ex.Message);
                //ProcessUpdate.flagForUpdate = 0;

            }
        }

        /// <summary>
        /// Tải lên file.
        /// </summary>
        /// <param name="remoteFile">Remote file.</param>
        /// <param name="localFile">Local file.</param>
        public void upload(string remoteFile, string localFile)
        {
            try
            {
                /* Mở file để tải lên */
                FileInfo objLocalFile = new FileInfo(localFile);
                /* Tạo một yêu cầu đến FTP server */
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + remoteFile);
                /* Xác đinh thể loại yêu cầu là Upload */
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                /* Đăng nhập vào hệ thống FTP server */
                ftpRequest.Credentials = new NetworkCredential(userName, passWord);
                /* Sử dụng tùy chọn khi có vấn đề */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;

                /* Thiết lập các truyền thông trả về từ FTP server */
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                /* Nhận các dòng phản hồi từ FTP server */
                ftpStream = ftpRequest.GetRequestStream();
                /* Mở một luồng file để đọc file được tải lên */
                FileStream remoteFileStream = objLocalFile.OpenRead();

                /* Tạo một bộ đệm lưu giữ dữ liệu chuẩn bị tải lên */
                byte[] byteBuffer = new byte[sizeBuffer];
                int byteSent = remoteFileStream.Read(byteBuffer, 0, sizeBuffer);

                /* Tải file lên bằng cách gửi các bộ dữ liệu đệm cho đến khi chuyển dịch là hoàn thành */
                try
                {
                    while (byteSent != 0)
                    {
                        ftpStream.Write(byteBuffer, 0, byteSent);
                        byteSent = remoteFileStream.Read(byteBuffer, 0, sizeBuffer);
                    }

                    Console.WriteLine("Uploaded!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Tai len khong thanh cong!");
                    Console.WriteLine("Loi: " + ex.Message);
                    return;
                }

                /* Giải phóng bộ nhớ */
                ftpResponse.Close();
                remoteFileStream.Close();
                ftpStream.Close();
                ftpRequest = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Da co loi xay ra: " + ex.Message);
            }
        }

        /// <summary>
        /// Lấy về độ dài file.
        /// </summary>
        /// <param name="remoteFile">Remote file.</param>
        /// <returns>Độ dài của File.</returns>
        public int getSizeOfFile(string remoteFile)
        {
            int sizeOfFile = 0;

            try
            {
                /* Tạo một yêu cầu đến FTP server */
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + remoteFile);
                /* Xác định thể loại yêu cầu là lấy về độ dài File */
                ftpRequest.Method = WebRequestMethods.Ftp.GetFileSize;
                /* Đăng nhập vào hệ thống FTP server */
                ftpRequest.Credentials = new NetworkCredential(userName, passWord);
                /* Sử dụng tùy chọn khi có vấn đề */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;

                /* Thiết lập các truyền thông trả về từ FTP server */
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                /* Nhận các dòng phản hồi từ FTP server */
                ftpStream = ftpResponse.GetResponseStream();

                /* Lưu trữ các phản hồi */
                sizeOfFile = (int)ftpResponse.ContentLength;

                /* Giải phóng bộ nhớ */
                ftpStream.Close();
                ftpResponse.Close();
                ftpRequest = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Da co loi xay ra: " + ex.Message);
            }

            /* Trả về độ dài File */
            return sizeOfFile;
        }

        /// <summary>
        /// Đổi tên file.
        /// </summary>
        /// <param name="remoteFile">Remote file.</param>
        /// <param name="newName">Tên mới</param>
        public void rename(string remoteFile, string newName)
        {
            try
            {
                /* Tạo một yêu cầu đến FTP server */
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + remoteFile);
                /* Xác định thể loại yêu cầu là đổi tên File */
                ftpRequest.Method = WebRequestMethods.Ftp.Rename;
                /* Đăng nhập vào hệ thống FTP server */
                ftpRequest.Credentials = new NetworkCredential(userName, passWord);
                /* Sử dụng tùy chọn khi có vấn đề */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;

                /* Thực hiện đổi tên File */
                ftpRequest.RenameTo = newName;

                /* Thiết lập các truyền thông trả về từ FTP server */
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();

                /* Giải phóng bộ nhớ */
                ftpResponse.Close();
                ftpRequest = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Da co loi xay ra: " + ex.Message);
            }
        }

        /// <summary>
        /// Xóa file.
        /// </summary>
        /// <param name="remoteFile">The remote file.</param>
        public void delete(string remoteFile)
        {
            try
            {
                /* Tạo một yêu cầu đến FTP server */
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + remoteFile);
                /* Đăng nhập vào hệ thống FTP server */
                ftpRequest.Credentials = new NetworkCredential(userName, passWord);
                /* Sử dụng tùy chọn khi có vấn đề */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                /* Xác định thể loại yêu cầu là xóa File */
                ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                /* Thiết lập các truyền thông trả về từ FTP server */
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                /* Giải phóng bộ nhớ */
                ftpResponse.Close();
                ftpRequest = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Da xay ra loi: " + ex.Message);
            }
        }

        /// <summary>
        /// Đọc nội dung của file.
        /// </summary>
        /// <param name="remoteFile">Remote file.</param>
        /// <returns>Nội dung của File.</returns>
        public string getContentOfFile(string remoteFile)
        {
            string contentOfFile = null;
            try
            {
                sizeBuffer = getSizeOfFile(remoteFile);
                /* Tạo một yêu cầu đến FTP server */
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + remoteFile);
                /* Đăng nhập vào hệ thống FTP server */
                ftpRequest.Credentials = new NetworkCredential(userName, passWord);
                /* Sử dụng tùy chọn khi có vấn đề */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;

                /* Thiết lập các truyền thông trả về từ FTP server */
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                /* Nhận các dòng phản hồi từ FTP server */
                ftpStream = ftpResponse.GetResponseStream();

                /* Bộ đệm lưu trữ nội dung file */
                byte[] byteBuffer = new byte[sizeBuffer];
                int byteRead = ftpStream.Read(byteBuffer, 0, sizeBuffer);
                /* Đọc toàn bộ File và lấy về nội dung File */
                try
                {
                    while (byteRead > 0)
                    {
                        contentOfFile += System.Text.Encoding.UTF8.GetString(byteBuffer);
                        byteRead = ftpStream.Read(byteBuffer, 0, sizeBuffer);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Da co loi xay ra: " + ex.Message);
                }
                /* Giải phóng bộ nhớ */
                ftpStream.Close();
                ftpResponse.Close();
                ftpRequest = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Da co loi xay ra: " + ex.Message);
            }

            /* Trả về nội dung File */
            return contentOfFile;
        }

        /// <summary>
        /// Tạo thư mục.
        /// </summary>
        /// <param name="newDirectoryName">Tạo thư mục.</param>
        public void creatDirectory(string newDirectoryName)
        {
            try
            {
                /* Tạo một yêu cầu đến FTP server */
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + newDirectoryName);
                /* Đăng nhập vào hệ thống FTP server */
                ftpRequest.Credentials = new NetworkCredential(userName, passWord);
                /* Sử dụng tùy chọn khi có vấn đề */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                /* Xác đinh thể loại yêu cầu là tạo một thư mục */
                ftpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
                /* Thiết lập các truyền thông trả về từ FTP server */
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                /* Giải phóng bộ nhớ */
                ftpResponse.Close();
                ftpRequest = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Da co loi xay ra: " + ex.ToString());
            }
        }

        /// <summary>
        /// Lần cuối sửa đồi file.
        /// </summary>
        /// <param name="remoteFile">Thời gian cuối sửa đổi.</param>
        public DateTime getTimeModified(string remoteFile)
        {
            DateTime timeModifiedFile = DateTime.Now;
            try
            {
                /* Tạo một yêu cầu đến FTP server */
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + remoteFile);
                /* Đăng nhập vào hệ thống FTP server */
                ftpRequest.Credentials = new NetworkCredential(userName, passWord);
                /* Sử dụng tùy chọn khi có vấn đề */
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                /* Xác định thể loại yêu cầu là lấy về thời gian tạo File */
                ftpRequest.Method = WebRequestMethods.Ftp.GetDateTimestamp;
                /* Thiết lập các truyền thông trả về từ FTP server */
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                /* Lây về thời gian thay đổi file */
                timeModifiedFile = ftpResponse.LastModified;

                Console.WriteLine("Thoi gian sua doi gan nhat: " + timeModifiedFile);
                ftpResponse.Close();
                ftpRequest = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Da co loi xay ra: " + ex.ToString());
            }
            return timeModifiedFile;
        }
        #endregion
    }
}
