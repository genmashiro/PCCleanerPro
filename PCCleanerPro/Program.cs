using System.Diagnostics;
using System.Management;
using System.Net;
using System.Security.Principal;
using Microsoft.Win32;

namespace PCCleanerPro
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (!IsRunAsAdmin())
            {
                Console.WriteLine("Please run the program as an administrator.");
                Console.ReadKey();
                return;
            }

            ShowOptions();

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        private static void ShowOptions()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            
            Console.WriteLine("Welcome to PCCleanerPro v0.0.1, " + Environment.UserName);
            Console.WriteLine("Coded by Gen.");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("You are using: " + Environment.OSVersion);
            Console.WriteLine("PC Name: " + Environment.MachineName);
            
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Please choose an option:");
            Console.WriteLine();
            Console.WriteLine("1. Clean temporary files");
            Console.WriteLine("2. Clean event log files");
            Console.WriteLine("3. Clean startup");
            Console.WriteLine("4. Uninstall programs");
            Console.WriteLine("5. Detect broken hard drives");
            Console.WriteLine("6. Clean browser data");
            Console.WriteLine("7. Exit");
            Console.WriteLine();

            ConsoleKeyInfo key = Console.ReadKey();

            switch (key.KeyChar)
            {
                case '1':
                    DeleteTempFiles();
                    break;
                case '2':
                    DeleteEventLogs();
                    break;
                case '3':
                    CleanStartup();
                    break;
                case '4':
                    UninstallPrograms();
                    break;
                case '5':
                    DetectBrokenHardDrives();
                    break;
                case '6':
                    BrowserDataCleaner.CleanBrowserData();
                    break;
                case '7':
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    ShowOptions();
                    break;
            }
        }

        private static void DeleteTempFiles()
        {
            Console.Clear();
            string tempFolderPath = Path.GetTempPath();
            DirectoryInfo directoryInfo = new DirectoryInfo(tempFolderPath);

            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                try
                {
                    file.Delete();
                    Console.WriteLine("Deleted file: " + file.FullName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting {file}: {ex.Message}");
                }
            }

            foreach (DirectoryInfo subdirectory in directoryInfo.GetDirectories())
            {
                try
                {
                    subdirectory.Delete(true);
                    Console.WriteLine("Deleted directory: " + subdirectory.FullName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting {subdirectory}: {ex.Message}");
                }
            }

            Console.WriteLine("\nTemporary files have been deleted.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            ShowOptions();
        }

        private static void DeleteEventLogs()
        {
            Console.Clear();
            try
            {
                EventLog installEventLog = new EventLog("Application");
                EventLog secEventLog = new EventLog("Security");
                EventLog sysEventLog = new EventLog("System");

                int maxKb = 64;
                installEventLog.MaximumKilobytes = maxKb;
                secEventLog.MaximumKilobytes = maxKb;
                sysEventLog.MaximumKilobytes = maxKb;

                installEventLog.Clear();
                secEventLog.Clear();
                sysEventLog.Clear();

                Console.WriteLine("\nAll Application Logs have been cleared.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                ShowOptions();
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nError deleting event log: " + ex.Message);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                ShowOptions();
            }
        }

        private static void CleanStartup()
        {
            Console.Clear();
            // Get the registry key for the current user's startup programs
            string startupFolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            // Get a list of all the startup programs and prompt the user to choose which ones to disable or delete
            Console.WriteLine("Active Startup Programs:");
            Console.WriteLine("------------------------");
            string[] startupPrograms = regKey.GetValueNames();
            foreach (string program in startupPrograms)
            {
                Console.WriteLine(program);
            }
            Console.WriteLine("\nWhich programs do you want to disable or delete? (Separate each program with a comma): (Type 'exit' to go back)");
            string input = Console.ReadLine();
            if (input.ToLower() == "exit")
            {
                ShowOptions();
                return;
            }
            string[] programsToDisable = Console.ReadLine().Split(',');

            // Disable or delete the selected startup programs
            foreach (string program in programsToDisable)
            {
                if (regKey.GetValue(program) != null)
                {
                    Console.WriteLine($"Do you want to disable or delete the startup program: {program}?");
                    Console.WriteLine("Enter D to delete:");
                    string userChoice = Console.ReadLine().ToUpper();

                    if (userChoice == "D")
                    {
                        // Delete the program from the registry
                        regKey.DeleteValue(program);
                        Console.WriteLine($"{program} has been deleted from the startup programs.");
                        ShowOptions();
                    }
                    else
                    {
                        Console.WriteLine(userChoice + " is not a valid option.");
                        ShowOptions();
                    }
                }
                else
                {
                    Console.WriteLine($"{program} is not an active startup program.");
                    ShowOptions();
                }
            }

            // Close the registry key
            regKey.Close();
        }

        private static void UninstallPrograms()
        {
            Console.Clear();
            Console.WriteLine("\nInstalled Programs:");

            RegistryKey uninstallKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall");

            var uninstallValues = uninstallKey?.GetSubKeyNames().Select(uninstallKey.OpenSubKey)?.ToList();

            foreach (var value in uninstallValues)
            {
                string displayName = value?.GetValue("DisplayName")?.ToString();
                string uninstallString = value?.GetValue("UninstallString")?.ToString();

                if (!string.IsNullOrEmpty(displayName))
                {
                    Console.WriteLine(displayName);
                }
            }

            Console.WriteLine();

            Console.WriteLine("Which program do you want to uninstall? (Type 'exit' to go back)" );
            string programName = Console.ReadLine();

            if (programName.ToLower() == "exit")
            {
                ShowOptions();
                return;
            }

            foreach (var value in uninstallValues)
            {
                string displayName = value?.GetValue("DisplayName")?.ToString();
                string uninstallString = value?.GetValue("UninstallString")?.ToString();

                if (!string.IsNullOrEmpty(displayName) && displayName.Contains(programName))
                {
                    Console.WriteLine($"Uninstalling {displayName}...");

                    var uninstallCommand = uninstallString.Trim('"');
                    var parametersStartIndex = uninstallCommand.IndexOf("/uninstall", StringComparison.OrdinalIgnoreCase);
                    string uninstallerPath = uninstallCommand.Substring(0, parametersStartIndex);
                    string uninstallerArguments = uninstallCommand.Substring(parametersStartIndex);

                    ProcessStartInfo uninstallerInfo = new ProcessStartInfo(uninstallerPath, uninstallerArguments);
                    uninstallerInfo.UseShellExecute = false;
                    uninstallerInfo.RedirectStandardOutput = true;

                    Process uninstallerProcess = new Process();
                    uninstallerProcess.StartInfo = uninstallerInfo;
                    uninstallerProcess.Start();

                    Console.WriteLine($"Successfully uninstalled {displayName}.");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    ShowOptions();
                    return;
                }
            }

            Console.WriteLine($"No such program found: {programName}.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            ShowOptions();
        }

        private static void DetectBrokenHardDrives()
        {
            bool detectedBrokenHardDrive = false;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive WHERE Status='Error'");
            foreach (ManagementObject drive in searcher.Get())
            {
                detectedBrokenHardDrive = true;
                Console.WriteLine("\nDetected Broken Hard Drive:");
                Console.WriteLine($"Model: {drive["Model"]}");
                Console.WriteLine($"Serial Number: {drive["SerialNumber"]}");
                Console.WriteLine();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            if (!detectedBrokenHardDrive)
            {
                Console.WriteLine("\nEverything is fine.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                ShowOptions();
            }
        }

        private static bool IsRunAsAdmin()
        {
            try
            {
                return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}