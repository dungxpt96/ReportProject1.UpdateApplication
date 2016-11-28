using System;

namespace MainLibrary
{
    public class LogEventArgs : EventArgs
    {
        #region Initialization

        /// <summary>
        /// Khởi tạo một đối tượng kiểu <see cref="LogEventArgs"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        public LogEventArgs(string message)
        {
            Message = message;
            TimeStamp = DateTime.Now;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Lấy về thời gian thực.
        /// </summary>
        /// <value>Thời gian thực.</value>
        public static DateTime TimeStamp { get; private set; }

        /// <summary>
        /// Lấy về thông điệp.
        /// </summary>
        /// <value>Thông điệp.</value>
        public static string Message { get; private set; }
        #endregion
    }
}
