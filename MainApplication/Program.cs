using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainLibrary;

namespace MainApplication
{
    /// <summary>
    /// Class Program.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Thực thi chương trình Update.
        /// </summary>
        /// <param name="args">Arguments.</param>
        public static void Main(string[] args)
        {
            var objectExecute = new UpdateApplication();
            objectExecute.Execute();

            Console.ReadKey();
        }
    }
}
