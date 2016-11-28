using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace MainLibrary
{
    public class UpdateApplication
    {
        #region Properties and Fileds
        /// <summary>
        /// Lưu trữ danh sách đường dẫn tuyệt đối đến các file.
        /// </summary>
        private string[] listPathOfFile;
        /// <summary>
        /// Lưu trữ danh sách tên của các file.xml . 
        /// </summary>
        private string[] listNameOfFile;

        /// <summary>
        /// Kiểm tra xem phương thức showListOfFile() được thực thi.
        /// </summary>
        private int flagOfShowList;
        /// <summary>
        /// Độ dài số lượng file trong thư mục.
        /// </summary>
        private int lengthOfListFile;
        /// <summary>
        /// Đường dẫn của thư mục lưu trữ các file.
        /// </summary>
        public static string pathStore;
        /// <summary>
        /// Kiểm tra tính đúng đắn các hàm trong class <see cref="UpdateApplication"/>.
        /// </summary>
        private int flagOfMain;
        #endregion

        #region Methods
        /// <summary>
        /// Hiển thị ra danh sách các file bên trong thư mục có đường dẫn pathStore.
        /// </summary>
        public void showListOfFile()
        {
            // Thư mục được truyền vào có đường dẫn pathOfDirectory
            Console.Write("Nhap vao duong dan luu tru cac file can CAP NHAT: ");
            pathStore = Console.ReadLine();
            try
            {
                // Lưu trữ danh sách đường dẫn trong thư mục pathOfDirectory.
                listPathOfFile = Directory.GetFiles(pathStore);

                if (lengthOfListFile == 0) // Nếu số file trong thư mục bằng 0.
                {
                    lengthOfListFile = listPathOfFile.Length; // Lấy về số file trong thư mục
                    // Sau khi lấy về số file, nếu số file vẫn bằng 0 thì dừng lại
                    if (lengthOfListFile == 0)
                    {
                        Console.WriteLine("Khong ton tai file trong thuc muc {0}", pathStore);
                        return;
                    }

                    flagOfMain = 1; // Ghi nhận tồn tại file trong thư mục

                    // Khởi tạo mảng danh sách lưu trữ tên của File
                    listNameOfFile = new string[lengthOfListFile];

                    int count = 0; // biến đếm file.xml trong thư mục pathOfDirectory
                    for (int i = 0; i < lengthOfListFile; i++)
                    {
                        /* Kiểm tra đường dẫn trong danh sách chỉ đến file có đuôi .xml
                         * Nếu là file.xml, lưu tên các file vào mảng listNameOfFile
                         */
                        if (Regex.IsMatch(listPathOfFile[i], @".xml"))
                        {
                            listNameOfFile[count++] += Path.GetFileName(listPathOfFile[i]);
                        }
                    }

                    // Lấy về số lượng file.xml
                    lengthOfListFile = count;
                    if (lengthOfListFile == 0) // nếu số file.xml bằng 0
                    {
                        Console.WriteLine("Khong ton tai file.xml trong thu muc {0}.", pathStore);
                        return;
                    }

                    // In ra danh sách các file.xml trong thư mục
                    Console.WriteLine("........................................................................");
                    Console.WriteLine("Danh sach ten cac file .xml gan voi cac phan mem trong thu muc:");
                    for (int i = 0; i < lengthOfListFile; i++)
                    {
                        Console.WriteLine("\t{0}.\t{1}", i + 1, listNameOfFile[i]);
                    }
                    Console.WriteLine("........................................................................");

                    // Ghi nhận phương thức showListOfFile đã thực thi hoàn thành
                    flagOfShowList = 1;
                }
                else
                {
                    // In ra danh sách các file.xml trong thư mục khi đã thực thi trước đó
                    Console.WriteLine("........................................................................");
                    Console.WriteLine("Danh sach ten cac file .xml gan voi cac phan mem trong thu muc:");
                    for (int i = 0; i < lengthOfListFile; i++)
                    {
                        Console.WriteLine("\t{0}.\t{1}", i + 1, listNameOfFile[i]);
                    }
                    Console.WriteLine("........................................................................");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Khong ton tai thu muc co duong dan {0}.", pathStore);
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Tiến hành kiểm tra phiên bản khác của file từ host để phát hiện cập nhật.
        /// </summary>
        public void checkUpdateFile()
        {
            Console.WriteLine("-------------------------------------------------------------------------------");
            Console.WriteLine("KIEM TRA CAP NHAT CAC UNG DUNG");
            Console.WriteLine("Vui long chon 1 phan mem de kiem tra cap nhat.\n" +
                              "Luu y: Kiem tra cap nhat dua tren thong so trong file tai lieu .xml gan voi cac phan mem");
            chooseFileToCheckUpdate();
            if (flagOfMain == 0)
            {
                Execute();
            }
            Console.Write("\n\tCAC TUY CHON KIEM TRA CAP NHAT VA CAI DAT PHIEN BAN MOI" +
                              "\n1. Chi kiem tra trang thai cap nhat" +
                              "\n2. Kiem tra trang thai cap nhat va tai ve ( nhung khong cai dat)" +
                              "\n3. Kiem tra trang thai cap nhat, tai ve va cai dat thay the ban cu" +
                              "\n4. Tuy chon khac(khac 1,2,3) => Thoat khoi chuong trinh" +
                              "\nLua chon:");
            int keyChoose = 0;
            try
            {
                keyChoose = int.Parse(Console.ReadLine()); // Nhập tùy chọn từ bàn phím
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ky tu khong hop le: " + ex.ToString());
                Console.WriteLine("Dang thoat ra khoi chuong trinh. Nhan Enter.");
                return;
            }

            switch (keyChoose)
            {
                case 1:
                    {
                        try
                        {
                            /* Khởi tạo một đối tượng kiểu ProcessUpdate và bắt đầu thực thi kiểm tra
                             * StartMonitoring() chỉ kiểm tra file được chọn có bản cập nhật hay không
                             * không tải về hay cập nhật.
                             */
                            Log.Console = true;
                            var obj = new ProcessUpdate();
                            obj.StartSimpleMornitoring();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("loi: " + ex.ToString());
                        }
                        break;
                    }
                case 2:
                    {
                        try
                        {
                            /* Khởi tạo một đối tượng kiểu ProcessUpdate và bắt đầu thực thi kiểm tra
                             * StartMornitoringNormal() cho phép kiểm tra file được chọn có bản cập nhật mới hay không
                             * nếu có bản cập nhật mới thì chỉ tải bản cập nhật về để trong thư mục download
                             * mà không cài đặt bản mới đó.
                             */
                            Log.Console = true;
                            var obj = new ProcessUpdate();
                            obj.StartNormalMornitoring();
                            Console.ReadKey();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("loi: " + ex.ToString());
                        }
                        break;
                    }
                case 3:
                    {
                        try
                        {
                            /* Khởi tạo một đối tượng kiểu ProcessUpdate và bắt đầu thực thi kiểm tra
                             * StartMornitoringFull() cho phép kiểm tra file được chọn có bản cập nhật mới hay không
                             * nếu có bản cập nhật mới thì tự động tải về và cài đặt luôn bản mới đó bằng cách 
                             * ghi đè lên file cũ.
                             * Nhưng để đơn giản, chương trình sẽ xóa file cũ và thay thế file mới vào chỗ đó.
                             */
                            Log.Console = true;
                            var obj = new ProcessUpdate();
                            obj.StartFullMornitoring();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("loi: " + ex.ToString());
                        }
                        break;
                    }
                default:
                    {
                        Console.WriteLine("thoat");
                        break;
                    }
            }
        }

        /// <summary>
        /// Chọn một file từ thư mục để kiểm tra số phiên bản trên host.
        /// </summary>
        public void chooseFileToCheckUpdate()
        {
            try
            {
                if (flagOfShowList == 0) // Kiểm tra trạng thái của showListOfFile() đã được thực thi hoàn thành chưa
                {
                    // Nếu chưa thực thi, thì bắt đầu thực thi
                    showListOfFile();
                }
                else
                {
                    // Nếu thực thi rồi thì chỉ việc in ra danh sách các file.xml có trong thư mục (nếu có)
                    Console.WriteLine("........................................................................");
                    for (int i = 0; i < lengthOfListFile; i++)
                    {
                        Console.WriteLine("\t{0}. {1}", i + 1, listNameOfFile[i]);
                    }
                    Console.WriteLine("........................................................................");
                }

                int choose = 0;
                if (lengthOfListFile > 0) // Nếu có file.xml trong thư mục với đường dẫn được nhập vào
                {
                    do
                    {
                        // Chọn file.xml để tiến hành kiểm tra trạng thái cập nhật
                        Console.Write("Lua chon: ");
                        try
                        {
                            choose = int.Parse(Console.ReadLine());
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Ky tu khong hop le: " + ex.ToString());
                        }
                        if (choose <= 0 || choose > lengthOfListFile)
                            Console.WriteLine("Lua chon khong hop le. Nhap lai.");
                    } while (choose <= 0 || choose > lengthOfListFile);

                    // Sau khi lựa chọn được file, tiến hành cài đặt các thông số cần thiết để kiểm tra
                    Console.WriteLine("Dang kiem tra App: {0}", listNameOfFile[choose - 1]);

                    // Lấy về đường dẫn tuyệt đối đến file được chọn
                    ProcessUpdate.localConfigFileUri = pathStore + @"\" + listNameOfFile[choose - 1];
                    // Tên file .xml được chọn
                    ProcessUpdate.nameOfFileCheck = listNameOfFile[choose - 1];
                    // Chỉ số của file trong mảng danh sách tên file .xml
                    ProcessUpdate.index = choose - 1;
                }
                else
                {
                    // Nếu không có file .xml nào trong thư mục
                    Console.WriteLine("Khong co App nao trong thu muc hoac duong dan khong hop le.");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Co loi xay ra trong qua trinh chon 1 file de cap nhat." + ex.ToString());
            }
        }

        /// <summary>
        /// Hàm thực thi giao diện chính của chương trình.
        /// </summary>
        public void Execute()
        {
            Console.WriteLine("-------------------------------------------------------------------------------");
            Console.WriteLine("\t\t\tCHAO MUNG DEN VOI AUTO UPDATE APPLICATION");
            Console.Write("Vui long chon cac tuy chon:"
                                + "\n\t1. Danh sach cac ung dung cua ban"
                                + "\n\t2. Kiem tra cap nhat"
                                + "\n\t3. Thoat chuong trinh"
                                + "\nNhap lua chon:");
            int keyChoose = 0;
            do
            {
                try
                {
                    keyChoose = int.Parse(Console.ReadLine());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ky tu khong hop le: " + ex.ToString());
                }
                if ((keyChoose <= 0) || (keyChoose >= 4))
                    Console.Write("Lua chon khong hop le.\nNhap lua chon:");
            } while ((keyChoose <= 0) || (keyChoose >= 4));

            switch (keyChoose)
            {
                case 1:
                    {
                        try
                        {
                            /* In ra danh sách các file bên trong thư mục có đường dẫn pathStore (nếu có) 
                             * Nếu đường dẫn không hợp lệ hoặc không tồn tại file thì thông báo
                             */
                            showListOfFile();
                            // Quay về màn hình chính
                            Execute();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Loi trong qua trinh show file: " + ex.ToString());
                        }
                        break;
                    }
                case 2:
                    {
                        try
                        {
                            /* Lựa chọn một file .xml(nếu có) trong thư mục được nhâp vào và tiến hành kiểm tra
                             * trạng thái cập nhật với các tùy chọn bên trong.
                             *  */
                            checkUpdateFile();
                            break;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Loi xay ra: " + ex.ToString());
                            return;
                        }
                    }
                case 3:
                    {
                        // Thoát ra khỏi chương trình.
                        Console.WriteLine("Dang thoat chuong trinh.");
                        break;
                    }

            }
        }
    }

    #endregion
}
