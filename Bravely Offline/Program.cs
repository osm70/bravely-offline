
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;
using System.Security.Policy;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Microsoft.VisualBasic.FileIO;
using System.Xml.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Bravely_Offline
{

    class Program
    {
        #region Trap application termination
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);
        static EventHandler _handler;

        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static bool Handler(CtrlType sig)
        {
            Console.WriteLine("Closed");



            Console.WriteLine("Cleaning up...");

           
           

            Process p = Process.GetProcessesByName("citra-qt").FirstOrDefault();
            p.Kill();

            now = DateTime.Now;
            nowS = now.ToString("MM_dd_yyyy_HH_mm_ss");
            using (StreamWriter sw = File.CreateText(saveFolder + @"\time.txt"))
            {
                sw.Write(nowS);
                sw.Close();
            }
            FileSystem.CopyDirectory(saveFolder, userDataFolder + @"Save\" + game + @"\" + slot + @"\Save\", true);
            FileSystem.CopyDirectory(saveFolder, userDataFolder + @"Save\" + game + @"\" + slot + @"\Backup\" + nowS);
            exitSystem = true;

            Environment.Exit(-1);

            return true;
        }
        #endregion

        static void Main(string[] args)
        {

            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);

            //start your multi threaded program here
            Program p = new Program();
            p.Start();

            while (!exitSystem)
            {
                Thread.Sleep(500);
            }
        }

        static void getCostumes()
        { 
        string filePath = saveFolder + @"\GAME0.sav";
            int offset = 0xB6F8;
            byte[] items = {0xc0, 0xe5, 0x4e, 0xe0, 0xe5, 0x4e, 0x00, 0xe6, 0x4e, 0x20, 0xe6, 0x4e, 0x40, 0xe6, 0x4e };
            Stream outStream = File.Open(filePath, FileMode.Open, FileAccess.Write);
            outStream.Seek(offset, SeekOrigin.Begin);
            for (int i = 0; i < items.Length; i++)
            {
                outStream.WriteByte(items[i]);
            }
            
            outStream.Flush();
            outStream.Close();

        }
        static void copyMove()
        {
            Console.WriteLine("");
            Console.WriteLine("Receiving the most recently sent move...");
            System.IO.DirectoryInfo di = new DirectoryInfo(streetpassFolder + @"\InBox___");
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            FileSystem.CopyDirectory(streetpassFolder + @"\OutBox__", streetpassFolder + @"\InBox___");

        }
        static void addDrinks(int drinks)
        {
            string filePath;
            if (game == "BD") filePath = saveFolder + @"\ECSV.sav";
            else filePath = saveFolder + @"\BD2ECSV.sav";
            int offset = 0x4;
            byte[] saveDrinks = new byte[1];
            using (FileStream str = File.OpenRead(filePath))
            {
                str.Seek(offset, SeekOrigin.Begin);
                str.Read(saveDrinks, 0, 1);
                str.Close();
            }
            int oldDrinks = saveDrinks[0];
            int newDrinks = oldDrinks + drinks;
            Console.WriteLine("");
            Console.WriteLine("Adding SP Drinks...");
            Console.WriteLine("Old: " + oldDrinks);
            Console.WriteLine("New: " + newDrinks);
            if (newDrinks > 31)
            {
                newDrinks = 31;
                Console.WriteLine("Exceeded the limit, resetting to 31");

            }
            saveDrinks[0] = (byte)newDrinks;

            Stream outStream = File.Open(filePath, FileMode.Open, FileAccess.Write);
            outStream.Seek(offset, SeekOrigin.Begin);
            outStream.WriteByte(saveDrinks[0]);
            outStream.Flush();
            outStream.Close();

        }
        static void addSp(int sp)
        {
            string filePath;
            if (game == "BD") filePath = saveFolder + @"\ECSV.sav";
            else filePath = saveFolder + @"\BD2ECSV.sav";
            int offset = 0x5;
            byte[] saveSp = new byte[1];
            using (FileStream str = File.OpenRead(filePath))
            {
                str.Seek(offset, SeekOrigin.Begin);
                str.Read(saveSp, 0, 1);
                str.Close();
            }
            int oldSp=0;
            switch (saveSp[0])
            {
                case 0x03: 
                oldSp = 3;
                break;
                case 0x02:
                oldSp = 2;
                break;
                case 0x01:
                oldSp = 1;
                break;
                case 0x00:
                oldSp = 0;
                break;
                case 0xFF:
                oldSp = -1;
                break;
                case 0xFE:
                oldSp = -2;
                break;
                case 0xFD:
                oldSp = -3;
                break;
                case 0xFC:
                oldSp = -4;
                break;

            }
            int newSp = oldSp + sp;
            Console.WriteLine("");
            Console.WriteLine("Adding SP...");
            Console.WriteLine("Old: " + oldSp);
            Console.WriteLine("New: " + newSp);
            if (newSp > 3)
            {
                newSp = 3;
                Console.WriteLine("Exceeded the limit, resetting to 3");

            }
            switch (newSp)
            {
                case 3:
                    saveSp[0] = 0x03;
                    break;
                case 2:
                    saveSp[0] = 0x02;
                    break;
                case 1:
                    saveSp[0] = 0x01;
                    break;
                case 0:
                    saveSp[0] = 0x00;
                    break;
                case -1:
                    saveSp[0] = 0xFF;
                    break;
                case -2:
                    saveSp[0] = 0xFE;
                    break;
                case -3:
                    saveSp[0] = 0xFD;
                    break;
                case -4:
                    saveSp[0] = 0xFC;
                    break;
            }
            Stream outStream = File.Open(filePath, FileMode.Open, FileAccess.Write);
            outStream.Seek(offset, SeekOrigin.Begin);
            outStream.WriteByte(saveSp[0]);
            outStream.Flush();
            outStream.Close();


        }
        static void addVillagers(int villagers)
        {
            string filePath;
            if (game == "BD") filePath = saveFolder + @"\COLONY0.sav";
            else filePath = saveFolder + @"\BD2COLONY0.sav";
            int villagerCountOffset = 0x4;
            byte[] saveFileVillagerCount = new byte[2], newVillagerCount = new byte[2];
            using (FileStream str = File.OpenRead(filePath))
            {
                str.Seek(villagerCountOffset, SeekOrigin.Begin);
                str.Read(saveFileVillagerCount, 0, 1);
                str.Read(saveFileVillagerCount, 1, 1);
                str.Close();
            }
            if (game.Equals("BD"))
            {

                int oldV = saveFileVillagerCount[0] + ((saveFileVillagerCount[1] / 0x10) * 128);
                int newV = oldV + villagers;
                Console.WriteLine("");
                Console.WriteLine("Adding population...");
                Console.WriteLine("Old: " + oldV);
                Console.WriteLine("New: " + newV);
                if (newV > 999)
                {
                    newV = 999;
                    Console.WriteLine("Exceeded the limit, resetting to 999");

                }
                byte byte2 = (byte)((newV / 128) * 0x10);
                byte byte1 = (byte)(newV - ((byte2 * 128) / 0x10));
                newVillagerCount[0] = byte1;
                newVillagerCount[1] = byte2;
                Stream outStream = File.Open(filePath, FileMode.Open, FileAccess.Write);
                outStream.Seek(villagerCountOffset, SeekOrigin.Begin);
                outStream.WriteByte(newVillagerCount[0]);
                outStream.WriteByte(newVillagerCount[1]);
                outStream.Flush();
                outStream.Close();
                newVillagerCount.CopyTo(saveFileVillagerCount, 0);
            }
            else 
            {
                Array.Reverse(saveFileVillagerCount);
                string hex = Convert.ToHexString(saveFileVillagerCount);
                
                int oldV = int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
                int newV = oldV + villagers;
                Console.WriteLine("");
                Console.WriteLine("Adding population...");
                Console.WriteLine("Old: " + oldV);
                Console.WriteLine("New: " + newV);
                if (newV > 9999)
                {
                    newV = 9999;
                    Console.WriteLine("Exceeded the limit, resetting to 9999");

                }
                string newHex = newV.ToString("X4");               
                byte byte1 = (byte) int.Parse(newHex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
                byte byte2 = (byte)int.Parse(newHex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                newVillagerCount[0] = byte1;
                newVillagerCount[1] = byte2;
                Stream outStream = File.Open(filePath, FileMode.Open, FileAccess.Write);
                outStream.Seek(villagerCountOffset, SeekOrigin.Begin);
                outStream.WriteByte(newVillagerCount[0]);
                outStream.WriteByte(newVillagerCount[1]);
                outStream.Flush();
                outStream.Close();
                newVillagerCount.CopyTo(saveFileVillagerCount, 0);
            }
        }
        static int ParseFile(string saveFolder, string fileName)
        {
            int value;
            if (!File.Exists(saveFolder + @"\"+fileName+".txt")) File.Create(saveFolder + @"\"+fileName+".txt").Dispose();


            using (StreamReader sr = File.OpenText(saveFolder + @"\" + fileName + ".txt"))
            {
                try
                {
                    value = int.Parse(sr.ReadToEnd());
                }
                catch (Exception)
                {
                    sr.Close();
                    value = 0;
                    using (StreamWriter sw = File.CreateText(saveFolder + @"\" + fileName + ".txt"))
                    {
                        sw.Write(value);
                        sw.Close();
                    }
                }
                sr.Close();

            }

            return value;
        }

        static void CreateFileWatcher(string path)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = path;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            if (game.Equals("BD")) watcher.Filter = "GAME0.sav";
            else watcher.Filter = "";

           
            watcher.Changed += new FileSystemEventHandler(OnChanged);


            
            watcher.EnableRaisingEvents = true;
        }

        private static void OnChanged(object source, FileSystemEventArgs e)
        {

            changed = true;
        }



        static DateTime now = DateTime.Now;
        static DateTime prev = DateTime.Now;
        static String nowS = now.ToString("MM_dd_yyyy_HH_mm_ss");
        static string userDataFolder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\UserData\";
        static string dataFolder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\Data\";
        static string romsFolder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\Roms\";
        static string saveFolder = "";
        static string titleID = "";
        static string streetpassFolder="";
        static string slot = "";
        static string game = "";
        static bool changed = false;
        static int status = 0;
        static int day = 0;
        static int nem = 0;
        static bool exitSystem = false;
        static bool costumes = false;
        static bool copy = false;


        public void Start()
        {

            
            System.IO.Directory.CreateDirectory(userDataFolder);
            Console.WriteLine("BRAVELY OFFLINE");
            Console.WriteLine("");
            string tokenFile = userDataFolder + @"\token.txt";
            if (!File.Exists(tokenFile))
            {
                Activate();
                using (StreamWriter sw = File.CreateText(tokenFile))
                {

                    var allChar = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz+-%=";
                    var random = new Random();
                    var resultToken = new string(
                       Enumerable.Repeat(allChar, 8192)
                       .Select(tmp => tmp[random.Next(tmp.Length)]).ToArray());

                    string token = resultToken.ToString();
                    sw.Write(token);
                }
                Console.WriteLine("Activated");
                Console.WriteLine("");

            }

            game = null;
            String mode = null;
            while
        (String.IsNullOrEmpty(mode))
            {
                Console.WriteLine("Select an option (Type the number and press enter)");
                Console.WriteLine("1 - Configure Citra - Do this first");
                Console.WriteLine("2 - Start playing");
                string tmp = Console.ReadLine();
                if (tmp == "1") mode = "1";
                else if (tmp == "2") mode = "2";

            }
            Console.WriteLine("");
            if (mode.Equals("1"))
            {

                using (Process p = new Process())
                {
                    p.StartInfo.FileName = dataFolder + @"Citra\citra-qt.exe";
                    String arg = "";
                    p.StartInfo.Arguments = arg;
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.Start();

                }

                Environment.Exit(0);
            }
            while
                    (String.IsNullOrEmpty(game))
            {
                Console.WriteLine("Select a game");
                Console.WriteLine("1 - Bravely Default");
                Console.WriteLine("2 - Bravely Second");
                string tmp = Console.ReadLine();
                if (tmp == "1")
                {
                    game = "BD";
                    saveFolder = dataFolder+ @"\Citra\user\sdmc\Nintendo 3DS\00000000000000000000000000000000\00000000000000000000000000000000\title\00040000\000fc500\data\00000001";
                    titleID = "000fc500";
                    streetpassFolder = dataFolder+ @"Citra\user\nand\data\00000000000000000000000000000000\sysdata\00010026\00000000\CEC\000db600";
                }
                else if (tmp == "2")
                {
                    game = "BS";
                    saveFolder = dataFolder + @"\Citra\user\sdmc\Nintendo 3DS\00000000000000000000000000000000\00000000000000000000000000000000\title\00040000\0017ba00\data\00000001";
                    titleID = "0017ba00";
                    streetpassFolder = dataFolder + @"Citra\user\nand\data\00000000000000000000000000000000\sysdata\00010026\00000000\CEC\0017ba00";
                } 
            }
            Console.WriteLine("");
            Console.WriteLine(game + " selected");
            if (!File.Exists(romsFolder + game + ".3ds"))
            {
                Console.WriteLine("");
                Console.WriteLine("Rom file missing");
                Console.WriteLine("Please place a valid US version decrypted Rom named " + game + ".3ds into the Roms folder.");
                Console.WriteLine("Press enter to exit");
                Console.ReadLine();
                Environment.Exit(0);
            }
            string output;
            using (Process p = new Process())
            {
                p.StartInfo.FileName = dataFolder + "rom_tool.exe";
                String arg = (" -i " + "'" + romsFolder + game + ".3ds'").Replace("'", "\"");
                p.StartInfo.Arguments = arg;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.Start();

                output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
            }
            if (!output.Contains(titleID))
            {
                Console.WriteLine("");
                Console.WriteLine("Invalid Rom");
                Console.WriteLine("Make sure you have the US version DECRYPTED Rom and try again.");
                Console.WriteLine("Press enter to exit");
                Console.ReadLine();
                Environment.Exit(0);
            }
            if (output.Contains("Update"))
            {
               
                using (Process p = new Process())
                {
                    p.StartInfo.FileName = dataFolder + "rom_tool.exe";
                    String arg = (" -s " + "'" + romsFolder + game + ".3ds'").Replace("'", "\"");
                    p.StartInfo.Arguments = arg;
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.Start();

                    output = p.StandardOutput.ReadToEnd();
                    p.WaitForExit();
                }
            }

            if (File.Exists(saveFolder+@"\slot.txt"))
            {

                using (StreamReader sr = File.OpenText(saveFolder + @"\slot.txt"))
                {
                    slot = sr.ReadToEnd();
                    sr.Close();
                    now = DateTime.Now;
                    nowS = now.ToString("MM_dd_yyyy_HH_mm_ss");
                    FileSystem.CopyDirectory(saveFolder, userDataFolder+@"Save\"+game+@"\"+slot+@"\Backup\"+nowS);
                    FileSystem.CopyDirectory(saveFolder, userDataFolder + @"Save\" + game + @"\" + slot + @"\Save\", true);



                }

            }
            System.IO.DirectoryInfo di = new DirectoryInfo(saveFolder);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            slot = "";
            Console.WriteLine("");
            Console.WriteLine("Select your save slot");
            Console.WriteLine("");
            for (int i = 0; i <= 9; i++)
            {
                string name = "NEW GAME";
                if (File.Exists(userDataFolder + @"Save\"+game+@"\" + i + @"\Save\name.txt"))
                {

                    using (StreamReader sr = File.OpenText(userDataFolder + @"Save\" + game + @"\" + i + @"\Save\name.txt"))
                    {
                         name = sr.ReadToEnd();
                        sr.Close();
                        
                    }
                }
                Console.WriteLine(i + " - " + name);
            }
            Console.WriteLine("");
            HashSet<String> nums = new HashSet<String>() {"0","1","2","3","4","5","6","7","8","9"};
            while (!nums.Contains(slot))
            {
                Console.WriteLine("Enter slot number");
                slot = Console.ReadLine();
            }
            System.IO.DirectoryInfo dir = Directory.CreateDirectory(userDataFolder + @"Save\" + game + @"\" + slot + @"\Save");

            using (StreamWriter sw = File.CreateText(userDataFolder + @"Save\" + game + @"\" + slot + @"\Save\slot.txt"))
            {

                sw.Write(slot);
            }

            Console.WriteLine("");



            if (!File.Exists(userDataFolder + @"Save\" + game + @"\" + slot + @"\Save\name.txt"))
            {
                if (game.Equals("BS"))
                {
                    

                    File.Create(userDataFolder + @"Save\" + game + @"\" + slot + @"\Save\transferSP.txt").Dispose();
                    File.Create(userDataFolder + @"Save\" + game + @"\" + slot + @"\Save\transferColony.txt").Dispose();

                    if (File.Exists(dataFolder + @"Citra\user\sdmc\Nintendo 3DS\00000000000000000000000000000000\00000000000000000000000000000000\extdata\00000000\00000FC5\user\ECSV.sav"))
                    {
                        string filePath = dataFolder + @"Citra\user\sdmc\Nintendo 3DS\00000000000000000000000000000000\00000000000000000000000000000000\extdata\00000000\00000FC5\user\ECSV.sav";
                        int offset = 0x4;
                        byte[] saveDrinks = new byte[1];
                        using (FileStream str = File.OpenRead(filePath))
                        {
                            str.Seek(offset, SeekOrigin.Begin);
                            str.Read(saveDrinks, 0, 1);
                            str.Close();
                        }
                        int transferSP = saveDrinks[0];

                        filePath = dataFolder + @"Citra\user\sdmc\Nintendo 3DS\00000000000000000000000000000000\00000000000000000000000000000000\extdata\00000000\00000FC5\user\COLONY0.sav";

                        int villagerCountOffset = 0x4;
                        byte[] saveFileVillagerCount = new byte[2], newVillagerCount = new byte[2];
                        using (FileStream str = File.OpenRead(filePath))
                        {
                            str.Seek(villagerCountOffset, SeekOrigin.Begin);
                            str.Read(saveFileVillagerCount, 0, 1);
                            str.Read(saveFileVillagerCount, 1, 1);
                            str.Close();
                        }

                        int transferColony = saveFileVillagerCount[0] + ((saveFileVillagerCount[1] / 0x10) * 128);

                        Console.WriteLine("The following will be transfered over from BD to BS:");
                        Console.WriteLine("SP drinks: "+transferSP);
                        Console.WriteLine("VIllagers: "+transferColony);

                        using (StreamWriter sw = File.CreateText(userDataFolder + @"Save\" + game + @"\" + slot + @"\Save\transferSP.txt"))
                        {

                            sw.Write(transferSP);
                        }

                        using (StreamWriter sw = File.CreateText(userDataFolder + @"Save\" + game + @"\" + slot + @"\Save\transferColony.txt"))
                        {

                            sw.Write(transferColony);
                        }

                        System.IO.DirectoryInfo diii = new DirectoryInfo(dataFolder + @"Citra\user\sdmc\Nintendo 3DS\00000000000000000000000000000000\00000000000000000000000000000000\extdata\00000000\00000FC5\user");
                        foreach (FileInfo file in diii.GetFiles())
                        {
                            file.Delete();
                        }
                        Console.WriteLine("You will get the transfered villagers and SP Drinks as soon as you unlock the Ba'al busting ship.");
                        Console.WriteLine("Press ENTER to continue");
                        Console.ReadLine();
                    }

                    else
                    {
                        
                        Console.WriteLine("Extra data not found");
                        Console.WriteLine("To transfer data from BD to BS, restart this app, start Bravely Default with your save and create Extra Data");
                        Console.WriteLine("Check the documentation for more info");
                        Console.WriteLine("If you do not wish to transfer, press ENTER three times");
                        Console.ReadLine();
                        Console.ReadLine();
                        Console.ReadLine();
                    }


                }
                Console.WriteLine("");
                Console.WriteLine("Creating new save file...");
                File.Create(userDataFolder + @"Save\" + game + @"\" + slot + @"\Save\time.txt").Dispose();
                File.Create(userDataFolder + @"Save\" + game + @"\" + slot + @"\Save\nextNem.txt").Dispose();
                File.Create(userDataFolder + @"Save\" + game + @"\" + slot + @"\Save\day.txt").Dispose();
                File.Create(userDataFolder + @"Save\" + game + @"\" + slot + @"\Save\status.txt").Dispose();                           
                Console.WriteLine("Enter the name for this save slot.");
                string name = Console.ReadLine();
                using (StreamWriter sw = File.CreateText(userDataFolder + @"Save\" + game + @"\" + slot + @"\Save\name.txt"))
                {

                    sw.Write(name);
                }
                Console.WriteLine("Remember to only use save file slot 1 in-game.");
                Console.WriteLine("Press ENTER to continue");
                Console.ReadLine();
                FileSystem.CopyDirectory(userDataFolder + @"Save\" + game + @"\" + slot + @"\Save\",saveFolder);
                if (game.Equals("BD")) {
                    costumes = true;
                }
               
            }

            else
            {
                FileSystem.CopyDirectory(userDataFolder + @"Save\" + game + @"\" + slot + @"\Save\", saveFolder);
                
                status = ParseFile(saveFolder, "status");
                DateTime lastSave;
                string lastSaveS;
                using (StreamReader sr = File.OpenText(userDataFolder + @"Save\" + game + @"\" + slot + @"\Save\time.txt"))
                {
                    lastSaveS = sr.ReadToEnd();
                    sr.Close();

                }
                lastSave = DateTime.ParseExact(lastSaveS, "MM_dd_yyyy_HH_mm_ss", null);
                now = DateTime.Now;
                nowS = now.ToString("MM_dd_yyyy_HH_mm_ss");
                TimeSpan diff = now - lastSave;
                int hours = (int)diff.TotalHours;
                nem = 0;
                if (hours < 0) { hours = 0; }
                
                
                    if (Directory.Exists(streetpassFolder))
                    {
                        
                        Console.WriteLine("Hours since last save: "+hours);
                        int villagers = (hours / 2) * 3;
                       if (villagers !=0) addVillagers(villagers);
                        int sp = hours / 6;
                        if (sp != 0 && game.Equals("BD")) addSp(sp);
                        if (hours >= 10)
                        {
                            Console.WriteLine("");
                            Console.WriteLine("More than 10 hours passed since your last save, a new update cycle has begun.");
                           if (game.Equals("BS")) addDrinks(1);
                            day = ParseFile(saveFolder, "day");
                            if (day == 0) day = 1;                         
                            if (day % 3 == 0) addDrinks(1);
                            if (day % 2 == 0) nem = 1;
                            day++;
                            using (StreamWriter sw = File.CreateText(saveFolder + @"\day.txt"))
                            {
                                sw.Write(day);
                                sw.Close();
                            }
                            copy = true;
                        }

                        if (status == 0)
                        {
                            bool tmp = false;
                            while (!tmp)
                            {
                                Console.WriteLine("");
                            if (game.Equals("BD")) Console.WriteLine("Have you unlocked Norende reconstructions?");
                            else Console.WriteLine("Have you unlocked Ba'al Busting? (Do you have a Ba'al Busting ship?)");
                                Console.WriteLine("1 - No");
                                Console.WriteLine("2 - Yes");
                                Console.WriteLine("Please answer honestly. Selecting Yes while you haven't can CORRUPT YOUR SAVE.");
                                string o = Console.ReadLine();
                                if (o.Equals("1")) tmp = true;
                                else if (o.Equals("2"))
                                {
                                    status = 1;
                                    using (StreamWriter sw = File.CreateText(saveFolder + @"\status.txt"))
                                    {
                                        sw.Write("1");
                                        sw.Close();
                                    }
                                    tmp = true;
                                    Console.WriteLine("");
                                    Console.WriteLine("You will now get your starting 100 villagers and 1 SP Drink.");
                                    addVillagers(100);
                                    addDrinks(1);
                                if (game.Equals("BS"))
                                {
                                    Console.WriteLine("");
                                    Console.WriteLine("You will now get the villagers and SP Drinks you transfered from BD.");
                                    int transferSP = ParseFile(saveFolder, "transferSP");
                                    int transferColony = ParseFile(saveFolder, "transferColony");
                                    if (transferSP != 0) addDrinks(transferSP);
                                    if (transferColony != 0) addVillagers(transferColony);
                                    Console.WriteLine("Press ENTER to continue");
                                    Console.ReadLine();
                                }
                                }

                            }
                        }
                        if (status == 1)
                            {
                                if (nem == 1)
                                {
                                    Console.WriteLine("");
                                    Console.WriteLine("You are about to receive a new Nemesis/Ba'al, as well as a copy of your sent move.");
                                    Console.WriteLine("Please follow these instructions after the game starts.");
                                    Console.WriteLine("1. Open your friend list (Menu - Tactics - Friends) and delete the DELIVERY friend if present. Also delete yourself.");
                                    Console.WriteLine("2. Switch focus back to this console app and press ENTER.");
                                    Console.WriteLine("3. Update Streetpass data");
                                    Console.WriteLine("4. Switch focus back to this console all and press ENTER for a second time");
                                    Console.WriteLine("5. Update Streetpass data again");
                                    Console.WriteLine("Do NOT New Game Plus or play normally in this state. Follow these instructions first.");
                                    Console.WriteLine("If you wish to New Game Plus, follow the instructions first, then save, return to title and do it then.");
                                    Console.WriteLine("After reading all this, press ENTER to start the game.");
                                    Console.ReadLine();
                                }
                            }

                        

                    }
                    else Console.WriteLine("Streetpass not registered, skipping straight to game launch.");

            
         }
            Console.WriteLine("");
            Console.WriteLine("Starting the game");

            if (costumes)
            {
                Console.WriteLine("Starting a new game");
                Console.WriteLine("As soon as you gain control of your character, select Emulation - Restart (in Citra) and load your newly created in-game file");
                Console.WriteLine("Press ENTER to continue");
                Console.ReadLine();
            }
            
            if (copy && (nem !=1 || status !=1)) 
            {
                Console.WriteLine("");
                Console.WriteLine("You are about to receive a copy of your received move, but not a Nemesis/Ba'al.");
                Console.WriteLine("Please follow these instructions after the game starts.");
                Console.WriteLine("1. Open your friend list (Menu - Tactics - Friends) and delete the friend with the name you used when configuring Citra if present (delete yourself).");
                Console.WriteLine("2. Switch focus back to this console app and press ENTER.");
                Console.WriteLine("3. Update Streetpass data");
                Console.WriteLine("Do NOT New Game Plus or play normally in this state. Follow these instructions first.");
                Console.WriteLine("If you wish to New Game Plus, follow the instructions first, then save, return to title and do it then.");
                Console.WriteLine("After reading all this, press ENTER to start the game.");
                Console.ReadLine();
                
            }

            using (Process p = new Process())
            {
                p.StartInfo.FileName = dataFolder + @"Citra\citra-qt.exe";
                String arg = ("'" + romsFolder + game + ".3ds'").Replace("'", "\"");
                p.StartInfo.Arguments = arg;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.Start();
                

            }
            if (copy)
            {
                Console.WriteLine("Delete yourself from your friend list, switch back here, press ENTER");
                Console.ReadLine();
                copyMove();
                Console.WriteLine("Update Streetpass");
            }
            if (status == 1 && nem == 1) 
            {
                int next = ParseFile(saveFolder, "nextNem");
                if (next == 0) next = 1;
                Console.WriteLine("Delete the DELIVERY friend, switch back here, press ENTER");
                Console.ReadLine();
                System.IO.DirectoryInfo dii = new DirectoryInfo(streetpassFolder + @"\InBox___");
                foreach (FileInfo file in dii.GetFiles())
                {
                    file.Delete();
                }
                FileSystem.CopyDirectory(dataFolder + game + @"\Streetpass\"+next, streetpassFolder + @"\InBox___");
                Console.WriteLine("Update Streetpass");
                next++;
                int nemCount = 7;
                if (game.Equals("BD")) nemCount = 10;
                if (next == nemCount+1) next = 1;
                using (StreamWriter sw = File.CreateText(saveFolder + @"\nextNem.txt"))
                {
                    sw.Write(next);
                    sw.Close();
                }

            }
            if (costumes && game.Equals("BD"))
            {
                while (!File.Exists(saveFolder + @"\GAME0.sav"))
                {
                    Thread.Sleep(500);
                }
                getCostumes();
            }
            Console.WriteLine("");
            Console.WriteLine("All systems running, switching to monitoring mode. Keep this console window open and play the game.");
            string save;
            if (game.Equals("BD")) save = saveFolder + "GAME0.sav";
            else save = saveFolder + "BD2GAME0.sav";
            
            while (Process.GetProcessesByName("citra-qt").Length > 0) 
            {
                CreateFileWatcher(saveFolder);
                Thread.Sleep(9500);
                if (changed)
                {
                    now = DateTime.Now;                   
                    nowS = now.ToString("MM_dd_yyyy_HH_mm_ss");
                    using (StreamWriter sw = File.CreateText(saveFolder + @"\time.txt"))
                    {
                        sw.Write(nowS);
                        sw.Close();
                    }
                    FileSystem.CopyDirectory(saveFolder, userDataFolder + @"Save\" + game + @"\" + slot + @"\Save\", true);
                    FileSystem.CopyDirectory(saveFolder, userDataFolder + @"Save\" + game + @"\" + slot + @"\Backup\" + nowS);
                    
                    changed = false;
                }
            }
            Console.WriteLine("Game closed, cleaning up...");
            now = DateTime.Now;
            nowS = now.ToString("MM_dd_yyyy_HH_mm_ss");
            using (StreamWriter sw = File.CreateText(saveFolder + @"\time.txt"))
            {
                sw.Write(nowS);
                sw.Close();
            }
            FileSystem.CopyDirectory(saveFolder, userDataFolder + @"Save\" + game + @"\" + slot + @"\Save\", true);
            FileSystem.CopyDirectory(saveFolder, userDataFolder + @"Save\" + game + @"\" + slot + @"\Backup\" + nowS);
            exitSystem = true;

        }

        private void Activate()
        {
            Console.WriteLine("Refer to the documentation for a guide on how to activate this app. Any incorrect answer will close the app.");
            Console.WriteLine("Enter activation code:");
            if(!Console.ReadLine().Equals("12108")) Environment.Exit(0);
            Console.WriteLine("What should you enter as the username when configuring Citra?");
            Console.WriteLine("1. Your name");
            Console.WriteLine("2. A name of your (fictional) friend");
            Console.WriteLine("Type the number and press ENTER to answer");
            if (!Console.ReadLine().Equals("2")) Environment.Exit(0);
            Console.WriteLine("Which save slot should you use in-game?");
            Console.WriteLine("1. Slot 1 only");
            Console.WriteLine("2. Doesn't matter");
            if (!Console.ReadLine().Equals("1")) Environment.Exit(0);
            Console.WriteLine("Which video should you watch at the start of Bravely Default?");
            Console.WriteLine("1. BD_AR");
            Console.WriteLine("2. Gyro");
            if (!Console.ReadLine().Equals("1")) Environment.Exit(0);
            Console.WriteLine("What should you do when starting a new game of Bravely Default?");
            Console.WriteLine("1. Wait on the start screen");
            Console.WriteLine("2. Reset when gaining control of the character");
            if (!Console.ReadLine().Equals("2")) Environment.Exit(0);
            Console.WriteLine("What should you do when starting a new game of Bravely Second?");
            Console.WriteLine("1. Wait on the start screen");
            Console.WriteLine("2. Reset when gaining control of the character");
            if (!Console.ReadLine().Equals("1")) Environment.Exit(0);
            Console.WriteLine("What should you do before updating Streetpass?");
            Console.WriteLine("1. Nothing, just update");
            Console.WriteLine("2. Delete the old friend data");
            if (!Console.ReadLine().Equals("2")) Environment.Exit(0);

        }
    }
}