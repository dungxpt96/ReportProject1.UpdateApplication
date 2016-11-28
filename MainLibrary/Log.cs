using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainLibrary
{
    public static class Log
    {
        #region 
        /// <summary>
        /// Khởi tạo một đối tượng kiểu <see cref="Log"/>.
        /// </summary>
        static Log()
        {
            Prefix = "[Update]";
        }
        #endregion

        #region Properties
        /// <summary>
        /// Lấy về và đặt giá trị cho Console là true hoặc false.
        /// </summary>
        /// <value><c>true</c> if console; còn lại, <c>false</c>.</value>
        public static bool Console { get; set; }

        /// <summary>
        /// Lấy về và đặt giá trị cho Debug là true hoặc false.
        /// </summary>
        /// <value><c>true</c> if debug; còn lại, <c>false</c>.</value>
        public static bool Debug { get; set; }

        /// <summary>
        /// Lấy về và đặt nội dung cho thông điệp.
        /// </summary>
        /// <value>Prefix.</value>
        public static string Prefix { get; set; }
        #endregion

        #region Event
        /// <summary>
        /// Tạo một sự kiện kiểu <see cref="LogEventArgs" />.
        /// </summary>
        public static event EventHandler<LogEventArgs> Event;

        /// <summary>
        /// Xảy ra khi sự kiện Event xảy ra.
        /// </summary>
        /// <param name="message"></param>
        private static void OnEvent(string message)
        {
            if (Event != null)
            {
                Event(null, new LogEventArgs(message));
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Hiển thị một định dạng xác định
        /// </summary>
        /// <param name="format">Format.</param>
        /// <param name="args">Arguments.</param>
        public static void Write(string format, params object[] args)
        {
            // Định dạng lại chuỗi.
            string message = string.Format(format, args);

            // Kích hoạt sự kiện
            OnEvent(message);

            if (Console) // In ra màn hình thông điệp
            {
                System.Console.WriteLine(message);
            }

            if (Debug) // Chuẩn đoán lỗi và thông báo
            {
                System.Diagnostics.Debug.WriteLine(Debug);
            }
        }
        #endregion
    }
}
