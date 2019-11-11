namespace AehnlichLib_UnitTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DirectoryDiffTests
    {
        /// <summary>
        /// This test determines whether the DirectoryDiff.GetBasePath() method determines
        /// the correct relative base path for left and right merged directories or not.
        /// </summary>
        [TestMethod]
        public void TestGetBasePath()
        {
            string[ ] t = { "Y:\\FILES\\MP3", null,        // params A
                            "X:\\", "X:\\00",              // params B
                            "00"                           // Expected output
                          };

            string[] t1 = { "Y:\\FILES\\MP3", "Y:\\FILES\\MP3\\01_mp3_mix",  // params A
                            "X:\\", "X:\\01_mp3_mix",                        // params B
                            "01_mp3_mix"                                     // Expected output
                          };

            string[] t2 = { "Y:\\DOWNLOADS\\CLEAN3_5", "Y:\\DOWNLOADS\\CLEAN3_5\\Aehnlich",  // params A
                            "X:\\DOWNLOADS\\CLEAN3_6", "Y:\\DOWNLOADS\\CLEAN3_6\\Aehnlich",  // params B
                            "Aehnlich"                                                       // Expected output
                          };

            string[] t3 = { "Y:\\DOWNLOADS\\CLEAN3_5", "Y:\\DOWNLOADS\\CLEAN3_5\\Aehnlich\\Props",  // params A
                            "X:\\DOWNLOADS\\CLEAN3_6", "Y:\\DOWNLOADS\\CLEAN3_6\\Aehnlich\\Props",  // params B
                            "Aehnlich\\Props"                                                       // Expected output
                          };

            string[][] tList = new string[][] { t, t1, t2, t3 };

            foreach (var item in tList)
            {
                string result = AehnlichLib.Dir.DirectoryDiff.GetBasePath(
                                                    item[0], new Mock_IFileSystemInfo.FileSystemInfoImpl(item[1]),
                                                    item[2], new Mock_IFileSystemInfo.FileSystemInfoImpl(item[3]));

                // Check if output meets expectations
                Assert.AreEqual(item[4], result);
            }
        }
    }
}
