using System;

namespace PCCleanerPro
{
    public class BrowserDataCleaner
    {
        public static void CleanBrowserData()
        {
            Console.Clear();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Which browser data do you want to delete?");
            Console.WriteLine();
            Console.WriteLine("1. Cache");
            Console.WriteLine("2. Cookies");
            Console.WriteLine("3. Both");
            Console.WriteLine();

            ConsoleKeyInfo key = Console.ReadKey();

            switch (key.KeyChar)
            {
                case '1':
                    DeleteCache();
                    break;
                case '2':
                    DeleteCookies();
                    break;
                case '3':
                    DeleteCache();
                    DeleteCookies();
                    break;
                default:
                    Console.Clear();
                    Console.WriteLine("Invalid option.");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    CleanBrowserData();
                    break;
            }
        }

        private static void DeleteCache()
        {
            Console.Clear();

            // Code to delete browser cache

            // Code to delete Firefox cache
            string firefoxCacheFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Mozilla\Firefox\Profiles";
            if (Directory.Exists(firefoxCacheFolder)) 
            {
                DirectoryInfo firefoxProfilesFolder = new DirectoryInfo(firefoxCacheFolder);
                foreach (var dir in firefoxProfilesFolder.GetDirectories())
                {
                    DirectoryInfo cacheFolder = new DirectoryInfo(dir.FullName + @"\cache2");
                    if (cacheFolder.Exists)
                    {
                        cacheFolder.Delete(true);
                    }
                    Console.WriteLine("Firefox cache have been deleted");
                }
            }

            // Code to delete Edge cache
            string edgeCacheFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Packages\Microsoft.MicrosoftEdge_xxxxxx\AC\";
            if (Directory.Exists(edgeCacheFolder))
            {
                DirectoryInfo edgeCacheDataFolder = new DirectoryInfo(edgeCacheFolder);
                DirectoryInfo cacheFolder = new DirectoryInfo(edgeCacheDataFolder.FullName + @"\MicrosoftEdge\Cache\");
                if (cacheFolder.Exists)
                {
                    cacheFolder.Delete(true);
                    Console.WriteLine("Edge cache deleted.");
                }
            }

            // Code to delete Chrome cache
            string chromeCacheFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome\User Data\Default\Cache\";
            if (Directory.Exists(chromeCacheFolder))
            {
                DirectoryInfo cacheFolder = new DirectoryInfo(chromeCacheFolder);
                if (cacheFolder.Exists)
                {
                    foreach (FileInfo file in cacheFolder.GetFiles())
                    {
                        file.Delete();
                    }
                    foreach (DirectoryInfo dir in cacheFolder.GetDirectories())
                    {
                        dir.Delete(true);
                    }
                    Console.WriteLine("Chrome cache deleted.");
                }
            }

            // Code to delete Opera cache
            string operaCacheFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Opera Software\Opera Stable\Cache\";
            if (Directory.Exists(operaCacheFolder))
            {
                DirectoryInfo cacheFolder = new DirectoryInfo(operaCacheFolder);
                if (cacheFolder.Exists)
                {
                    foreach (FileInfo file in cacheFolder.GetFiles())
                    {
                        file.Delete();
                    }
                    foreach (DirectoryInfo dir in cacheFolder.GetDirectories())
                    {
                        dir.Delete(true);
                    }
                    Console.WriteLine("Opera cache deleted.");
                }
            }
        }

        private static void DeleteCookies()
        {
            // Code to delete browser cache
            // Firefox
            string firefoxCookiesPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                        @"\Mozilla\Firefox\Profiles\";
            if (Directory.Exists(firefoxCookiesPath))
            {
                string[] profiles = Directory.GetDirectories(firefoxCookiesPath);
                foreach (string profile in profiles)
                {
                    string cookieFile = profile + "\\cookies.sqlite";
                    if (File.Exists(cookieFile))
                    {
                        File.Delete(cookieFile);
                    }
                }
            }

            // Edge
            string edgeCookiesPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                                     @"\Packages\Microsoft.MicrosoftEdge_8wekyb3d8bbwe\AC\MicrosoftEdge\User\Default\Cookies";
            if (File.Exists(edgeCookiesPath))
            {
                File.Delete(edgeCookiesPath);
            }

            // Chrome
            string chromeCookiesPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                                       @"\Google\Chrome\User Data\Default\Cookies";
            if (File.Exists(chromeCookiesPath))
            {
                File.Delete(chromeCookiesPath);
            }

            // Opera
            string operaCookiesPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                      @"\Opera Software\Opera Stable\Cookies";
            if (File.Exists(operaCookiesPath))
            {
                File.Delete(operaCookiesPath);
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}