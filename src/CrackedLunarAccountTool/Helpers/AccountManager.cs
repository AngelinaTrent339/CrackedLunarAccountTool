using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.IO;
using System.Threading;

namespace CrackedLunarAccountTool.Helpers
{
    internal class AccountManager
    {
        private static JObject json;
        public static void CreateAccount(string username, string uuid)
        {
            // Check if json is null
            if (json == null)
            {
                ConsoleHelpers.PrintLine("ERROR", "JSON data is not loaded. Cannot create account.", Color.FromArgb(224, 17, 95));
                return;
            }

            // Ensure accounts object exists
            if (json["accounts"] == null)
            {
                json["accounts"] = new JObject();
            }

            // the account data to be added
            JObject newAccount = new JObject
            {
                ["accessToken"] = uuid,
                ["accessTokenExpiresAt"] = "2050-07-02T10:56:30.717167800Z",
                ["eligibleForMigration"] = false,
                ["hasMultipleProfiles"] = false,
                ["legacy"] = true,
                ["persistent"] = true,
                ["userProperites"] = new JArray(),
                ["localId"] = uuid,
                ["minecraftProfile"] = new JObject
                {
                    ["id"] = uuid,
                    ["name"] = username
                },
                ["remoteId"] = uuid,
                ["type"] = "Xbox",
                ["username"] = username
            };

            // add the new account to the existing accounts
            JObject accounts = (JObject)json["accounts"];
            accounts[uuid] = newAccount;

            ConsoleHelpers.PrintLine("SUCCESS", "Your account has successfully been created.", Color.FromArgb(135, 145, 216));
        }

        public static void RemoveAllAccounts()
        {
            if (json == null)
            {
                ConsoleHelpers.PrintLine("ERROR", "JSON data is not loaded. Cannot remove accounts.", Color.FromArgb(224, 17, 95));
                return;
            }

            json["accounts"] = new JObject();
            ConsoleHelpers.PrintLine("SUCCESS", "All accounts have been successfully removed.", Color.FromArgb(135, 145, 216));
        }

        public static void RemoveCrackedAccounts()
        {
            if (json == null || json["accounts"] == null)
            {
                ConsoleHelpers.PrintLine("ERROR", "JSON data is not loaded or accounts section is missing. Cannot remove accounts.", Color.FromArgb(224, 17, 95));
                return;
            }

            JArray accountsToRemove = new JArray();
            JObject accounts = (JObject)json["accounts"];

            foreach (var account in accounts)
            {
                if (Validate.IsValidUUID((string)account.Value["accessToken"]))
                {
                    accountsToRemove.Add(account.Key);
                }
            }

            foreach (var key in accountsToRemove)
            {
                accounts.Remove(key.ToString());
            }

            ConsoleHelpers.PrintLine("SUCCESS", "Cracked accounts have been successfully removed.", Color.FromArgb(135, 145, 216));
        }

        public static void RemovePremiumAccounts()
        {
            if (json == null || json["accounts"] == null)
            {
                ConsoleHelpers.PrintLine("ERROR", "JSON data is not loaded or accounts section is missing. Cannot remove accounts.", Color.FromArgb(224, 17, 95));
                return;
            }

            JArray accountsToRemove = new JArray();
            JObject accounts = (JObject)json["accounts"];

            foreach (var account in accounts)
            {
                if (!Validate.IsValidUUID((string)account.Value["accessToken"]))
                {
                    accountsToRemove.Add(account.Key);
                }
            }

            foreach (var key in accountsToRemove)
            {
                accounts.Remove(key.ToString());
            }

            ConsoleHelpers.PrintLine("SUCCESS", "Premium accounts have been successfully removed.", Color.FromArgb(135, 145, 216));
        }

        public static void ViewInstalledAccounts()
        {
            if (json == null || json["accounts"] == null)
            {
                ConsoleHelpers.PrintLine("ERROR", "JSON data is not loaded or accounts section is missing. Cannot view accounts.", Color.FromArgb(224, 17, 95));
                return;
            }

            ConsoleHelpers.PrintLine("INFO", "Installed Accounts:", Color.FromArgb(135, 145, 216));
            JObject accounts = (JObject)json["accounts"];
            
            if (accounts.Count == 0)
            {
                ConsoleHelpers.PrintLine("INFO", "No accounts found.", Color.FromArgb(135, 145, 216));
                return;
            }

            foreach (var account in accounts)
            {
                ConsoleHelpers.PrintLine("ACCOUNT", account.Key + ": " + account.Value["username"], Color.FromArgb(135, 145, 216));
            }
        }

        public static void LoadJson()
        {
            try
            {
                if (File.Exists(Program.lunarAcccountsPath))
                {
                    string jsonContent = File.ReadAllText(Program.lunarAcccountsPath);
                    if (string.IsNullOrWhiteSpace(jsonContent))
                    {
                        // If file is empty, create default structure
                        json = new JObject { ["accounts"] = new JObject() };
                    }
                    else
                    {
                        json = JObject.Parse(jsonContent);
                        // Ensure accounts object exists
                        if (json["accounts"] == null)
                        {
                            json["accounts"] = new JObject();
                        }
                    }
                }
                else
                {
                    // Create directory if it doesn't exist
                    string directory = Path.GetDirectoryName(Program.lunarAcccountsPath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    
                    json = new JObject { ["accounts"] = new JObject() };
                    
                    // Save the default structure to file
                    SaveJson();
                }
            }
            catch (Exception e)
            {
                ConsoleHelpers.PrintLine("ERROR", "Failed to load accounts file: " + e.Message, Color.FromArgb(224, 17, 95));
                ConsoleHelpers.PrintLine("NOTICE", "Please check that you have write permissions to the Lunar Client directory.", Color.FromArgb(224, 17, 95));
                ConsoleHelpers.PrintLine("NOTICE", "Exiting in 3 seconds...", Color.FromArgb(242, 140, 40));
                Thread.Sleep(3000);
                Environment.Exit(1);
            }
        }

        public static void SaveJson()
        {
            try
            {
                if (json == null)
                {
                    ConsoleHelpers.PrintLine("ERROR", "JSON data is null. Cannot save accounts file.", Color.FromArgb(224, 17, 95));
                    return;
                }

                // Create directory if it doesn't exist
                string directory = Path.GetDirectoryName(Program.lunarAcccountsPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(Program.lunarAcccountsPath, json.ToString());
            }
            catch (Exception e)
            {
                ConsoleHelpers.PrintLine("ERROR", "Failed to save accounts file: " + e.Message, Color.FromArgb(224, 17, 95));
            }
        }
    }
}
