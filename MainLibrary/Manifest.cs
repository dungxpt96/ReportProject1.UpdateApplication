using System;
using System.Xml.Linq;
using System.Linq;
using System.IO;

namespace MainLibrary
{
    internal class Manifest
    {
        #region Field
        /// <summary>
        /// Lưu trữ dữ liệu đọc được từ 1 file.
        /// </summary>
        private string _data;
        #endregion

        #region Initialization
        /// <summary>
        /// Khởi tạo một đối tượng kiểu <see cref="Manifest"/>.
        /// </summary>
        /// <param name="data"></param>
        public Manifest(string data)
        {
            Load(data);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Lấy về số phiên bản của phần mềm.
        /// </summary>
        /// <value>Version của phần mềm</value>
        public string Version { get; private set; }

        /// <summary>
        /// Lấy về tên phần mềm.
        /// </summary>
        /// <value> Tên của phần mềm. </value>
        public string TenPhanmem { get; private set; }

        /// <summary>
        /// Lấy về ngày sửa đổi (ngày cập nhật) cuối của phần mềm.
        /// </summary>
        /// <value> Ngày cập nhật phần mềm.</value>
        public string NgayCapNhat { get; private set; }

        /// <summary>
        /// Lấy về nội dung chức năng của phần mềm.
        /// </summary>
        /// <value> Chức năng của phần mềm.</value>
        public string Function { get; private set; }

        /// <summary>
        /// Lấy về tên người phát hành phần mềm.
        /// </summary>
        /// <value> Người sở hữu phần mềm. </value>
        public string Author { get; private set; }

        /// <summary>
        /// Lấy về mã bảo mật của phần mềm.
        /// </summary>
        /// <value> Mã bảo mật </value>
        public string CodeSecurity { get; private set; }

        /// <summary>
        /// Lấy về đường dẫn đến thư mục chứa phần mềm.
        /// </summary>
        /// <value> URI.</value>
        public string BaseUri { get; private set; }

        /// <summary>
        /// Tên của file.zip chứa các file của bản cập nhật mới.
        /// </summary>
        /// <value>file.zip</value>
        public string ZipFiles { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Tải dữ liệu và lấy về những thông tin trên dữ liệu đó.
        /// </summary>
        /// <param name="data"></param>
        private void Load(string data)
        {
            _data = data;

            try
            {
                // Khởi tạo 1 đối tượng kiểu XDocument là file .xml
                var xml = XDocument.Parse(data);

                /* Tiến hành đọc file xml để lấy dữ liệu.
                 * Đọc từng Node, rồi trong mỗi Node lấy về nội dung của các thuộc tính và các phần tử.
                 *  */

                // Khai báo biến chứa nội dung của 1 Node trong file xml có tên là Basic
                var basic = xml.Descendants("Basic");
                // Với mỗi phần tử thuộc Node "Basic"
                foreach (var e in basic)
                {
                    // Lấy về nội dung của các thuộc tính và phần tử trong Node "Basic"
                    Version = e.Attribute("Version").Value;
                    TenPhanmem = e.Element("TenPhanMem").Value;
                    NgayCapNhat = e.Element("NgayCapNhat").Value;
                    Function = e.Element("Function").Value;
                    Author = e.Element("Author").Value;
                }

                // Khai báo biến chứa nội dung của 1 Node trong file xml có tên là Manifest
                var manifest = xml.Descendants("Manifest");
                // Vói mỗi phần tử thuộc Node "Manifest"
                foreach (var e in manifest)
                {
                    // Lấy về nội dung của các thuộc tính và phần tử trong Node "Manifest"
                    CodeSecurity = e.Element("CodeSecurity").Value;
                    BaseUri = e.Element("BaseUri").Value;
                    ZipFiles = e.Element("ZipFile").Value;
                }

            }
            catch (Exception ex) // Có lỗi trong quá trình đọc file xml
            {
                Log.Write("(Manifest load) Da co loi xay ra: {0}", ex.ToString());
                return;
            }
        }

        /// <summary>
        /// Ghi dữ liệu vừa đọc được ra đường dẫn được truyền vào
        /// </summary>
        /// <param name="path"></param>
        public void Write(string path)
        {
            File.WriteAllText(path, _data);
        }
        #endregion
    }
}
