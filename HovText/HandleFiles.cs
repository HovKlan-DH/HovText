﻿/*
##################################################################################################
HANDLEFILES
-----------

This will handle most of the file activity. At least everything related to
the "data", "index" and "favorite" files.

##################################################################################################
*/

using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;


namespace HovText
{
    public class HandleFiles
    {
        // ###########################################################################################
        // Class variables
        // ########################################################################################### 
        private static SortedDictionary<int, string> entriesApplicationLoad = new SortedDictionary<int, string>();
        private static SortedDictionary<int, Image> entriesApplicationIconLoad = new SortedDictionary<int, Image>();
        private static SortedDictionary<int, bool> entriesIsFavoriteLoad = new SortedDictionary<int, bool>();
        private static SortedDictionary<int, bool> entriesIsTransparentLoad = new SortedDictionary<int, bool>();
        private static SortedDictionary<int, string> entriesChecksumLoad = new SortedDictionary<int, string>();
        private static SortedList<int, Dictionary<string, object>> entriesOriginalLoad = new SortedList<int, Dictionary<string, object>>();
        private static List<int> entriesOrderLoad = new List<int>();
        public static bool saveIndexAndFavoriteFiles = false;

        // Below 3 booleans will be set to "true", if we are not loading any files
        public static bool onLoadAllEntriesInClipboardQueue = false; // "true" = all entries have been loaded from data file and to the clipboard queue - no content yet in the clipboard list, as the queue has not been processed yet
        public static bool onLoadAllEntriesProcessedInClipboardQueue = false; // "true" = all cliopboard entries have been processed and the clipboard list nbow shows everything - nothing has been saved yet
        public static bool onLoadAllEntriesSavedFromQueue = false; // "true" = all entries have been saved - if no new stuff copied and everything should be visible/listed, then this should be a 1:1 copy from previous files    


        // ###########################################################################################
        // Save the (one) newest clipboard entry to a data file
        // (will just append to the existing file)
        // ###########################################################################################   

        public static void SaveClipboardEntryToFile(int indexToSave)
        {
            Logging.Log($"Saving entry index [{indexToSave}] to encrypted \"data\" file [{Settings.pathAndData}]");

            // Check if file exists, if not, create it and insert version name
            if (!File.Exists(Settings.pathAndData))
            {
                using (var fileStream = new FileStream(Settings.pathAndData, FileMode.Create))
                using (var writer = new StreamWriter(fileStream))
                {
                    writer.WriteLine($"HovText {Settings.appVer}");
                    writer.Flush();
                }
            }

            var entriesOriginalLoad = new SortedList<int, Dictionary<string, object>>();
            var entriesApplicationLoad = new SortedDictionary<int, string>();
            var entriesApplicationIconLoad = new SortedDictionary<int, Image>();
            var entriesIsTransparentLoad = new SortedDictionary<int, bool>();
            var entriesChecksumLoad = new SortedDictionary<int, string>();

            entriesOriginalLoad.Add(indexToSave, Settings.entriesOriginal[indexToSave]);
            entriesApplicationLoad.Add(indexToSave, Settings.entriesApplication[indexToSave]);
            entriesApplicationIconLoad.Add(indexToSave, Settings.entriesApplicationIcon[indexToSave]);
            entriesIsTransparentLoad.Add(indexToSave, Settings.entriesIsTransparent[indexToSave]);
            entriesChecksumLoad.Add(indexToSave, Settings.entriesChecksum[indexToSave]);

            foreach (var format in entriesOriginalLoad)
            {
                if (format.Value == null)
                {
                    Logging.Log($"Format=[{format.Value}]");
                }
            }

            // Update the Tuple type declaration to include the new dictionary
            var dataToSerialize = new Tuple<
                SortedList<int, Dictionary<string, object>>,
                SortedDictionary<int, string>,
                SortedDictionary<int, Image>,
                SortedDictionary<int, bool>,
                SortedDictionary<int, string>
            >(entriesOriginalLoad, entriesApplicationLoad, entriesApplicationIconLoad, entriesIsTransparentLoad, entriesChecksumLoad);

            // Serialization process
            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream())
            {
                // Serialize the data to the MemoryStream
                binaryFormatter.Serialize(memoryStream, dataToSerialize);
                memoryStream.Position = 0; // Reset the position after serialization

                // Compress the serialized data
                using (var compressedStream = new MemoryStream())
                {
                    using (var compressor = new GZipStream(compressedStream, CompressionMode.Compress))
                    {
                        memoryStream.CopyTo(compressor);
                    } // Compressor needs to be closed/disposed to finish writing

                    var compressedData = compressedStream.ToArray();

                    // Encrypt the compressed data
                    var encryptedData = Encryption.EncryptStringToBytes_Aes(compressedData, Settings.encryptionKey, Settings.encryptionInitializationVector);

                    // Write the encrypted data to a file
                    using (var fileStream = new FileStream(Settings.pathAndData, FileMode.Append))
                    {
                        // Write the length of the encrypted data
                        byte[] encryptedDataLengthBytes = BitConverter.GetBytes(encryptedData.Length);
                        if (BitConverter.IsLittleEndian)
                        {
                            Array.Reverse(encryptedDataLengthBytes);
                        }
                        fileStream.Write(encryptedDataLengthBytes, 0, encryptedDataLengthBytes.Length);

                        // Write the encrypted data
                        fileStream.Write(encryptedData, 0, encryptedData.Length);
                    }
                }
            }
        }


        // ###########################################################################################
        // Load the full list of clipboard entries from a data file.
        // Get the form instance needed for UI updates.
        // ###########################################################################################   

        public static void LoadDataFile()
        {
            Logging.Log($"Reading encrypted \"data\" file [{Settings.pathAndDataLoad}]");

            try
            {
                // Read the version name from the first line of the file
                string version;
                using (var tempStream = new FileStream(Settings.pathAndDataLoad, FileMode.Open))
                using (var reader = new StreamReader(tempStream))
                {
                    version = reader.ReadLine();
                    Logging.Log($"Version saved to \"data\" file is [{version}]");
                }

                int counter = 0;
                using (var fileStream = new FileStream(Settings.pathAndDataLoad, FileMode.Open))
                {
                    // Skip the first line as this is the "version" line
                    fileStream.Position = Encoding.UTF8.GetBytes(version + Environment.NewLine).Length;

                    bool breakWhile = false; // "true" when we have reached the amount of clipboards we want

                    while (fileStream.Position < fileStream.Length && !breakWhile)
                    {
                        // Read the length of the encrypted data
                        byte[] encryptedDataLengthBytes = new byte[sizeof(int)];
                        fileStream.Read(encryptedDataLengthBytes, 0, encryptedDataLengthBytes.Length);

                        // Reverse the byte order if the system is little-endian
                        if (BitConverter.IsLittleEndian)
                        {
                            Array.Reverse(encryptedDataLengthBytes);
                        }

                        int encryptedDataLength = BitConverter.ToInt32(encryptedDataLengthBytes, 0);

                        // Read the encrypted data from the file
                        byte[] encryptedData = new byte[encryptedDataLength];
                        fileStream.Read(encryptedData, 0, encryptedData.Length);

                        // Decrypt the data
                        var decryptedData = Encryption.DecryptStringFromBytes_Aes(encryptedData, Settings.encryptionKey, Settings.encryptionInitializationVector);

                        // Check if decryption was successful
                        if (decryptedData == null)
                        {
                            Logging.Log("Encryption mismatches and \"data\" file cannot be read - will be overwritten");
                            break; // Break out of the loop if decryption fails
                        }

                        // Decompress the decrypted data
                        byte[] decompressedData;
                        using (var decompressedStream = new MemoryStream())
                        {
                            using (var memoryStream = new MemoryStream(decryptedData))
                            {
                                using (var decompressor = new GZipStream(memoryStream, CompressionMode.Decompress))
                                {
                                    decompressor.CopyTo(decompressedStream);
                                }
                            }
                            decompressedData = decompressedStream.ToArray();
                        }

                        try
                        {
                            // Deserialize the decompressed data
                            using (var memoryStream = new MemoryStream(decompressedData))
                            {
                                var binaryFormatter = new BinaryFormatter();
                                var loadedData = (Tuple<
                                    SortedList<int, Dictionary<string, object>>,
                                    SortedDictionary<int, string>,
                                    SortedDictionary<int, Image>,
                                    SortedDictionary<int, bool>,
                                    SortedDictionary<int, string>
                                >)binaryFormatter.Deserialize(memoryStream);

                                // Extract the dictionaries from the tuple into temporary dictionaries
                                entriesOriginalLoad = loadedData.Item1;
                                entriesApplicationLoad = loadedData.Item2;
                                entriesApplicationIconLoad = loadedData.Item3;
                                entriesIsTransparentLoad = loadedData.Item4;
                                entriesChecksumLoad = loadedData.Item5;
                            }
                        }
                        catch (SerializationException ex)
                        {
                            Logging.Log($"Error during deserialization: {ex.Message}");
                            entriesOriginalLoad = null;
                            entriesIsTransparentLoad = null;
                            entriesApplicationLoad = null;
                            entriesApplicationIconLoad = null;
                            entriesChecksumLoad = null;
                            break;
                        }

                        // Process entries if decryption and deserialization were successful
                        if (entriesOriginalLoad != null)
                        {
                            int numEntries = entriesOrderLoad.Count;
                            Logging.Log("---");
                            foreach (var entry in entriesOriginalLoad)
                            {
                                try
                                {
                                    int orderNum = entriesOrderLoad.IndexOf(entry.Key);

                                    Dictionary<string, object> clipboardObject = entry.Value;
                                    entriesApplicationLoad.TryGetValue(entry.Key, out string whoUpdatedClipboardName);
                                    entriesApplicationIconLoad.TryGetValue(entry.Key, out Image whoUpdatedClipboardIcon);
                                    entriesIsFavoriteLoad.TryGetValue(entry.Key, out bool isFavorite);

                                    // "orderNum" will be -1, if the original entry should NOT be used again
                                    if (orderNum != -1)
                                    {
                                        counter++;

                                        Logging.Log($"Processing clipboard entry [{counter}] with entry.Key [{entry.Key}] and entryIndex [{Settings.entryIndex}] with [{clipboardObject.Count}] formats");
                                                                                
                                        // Thread-safe adding to the tuple queue
                                        HandleClipboard.clipboardQueue.Enqueue((
                                            orderNum,
                                            clipboardObject,
                                            whoUpdatedClipboardName,
                                            whoUpdatedClipboardIcon,
                                            isFavorite
                                        ));
                                    }
                                    else
                                    {
                                        Logging.Log($"Skipping clipboard entry [{counter}] with entry.Key [{entry.Key}] and entryIndex [{Settings.entryIndex}] with [{clipboardObject.Count}] formats");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logging.Log($"Error processing entry {entry.Key}: {ex.Message}");
                                }
                                
                                // Break from loop, if we have reached the amount we want to process
                                if (counter == numEntries)
                                {
                                    breakWhile = true;
                                }
                            }

                            // Clear temporary dictionaries for the next iteration
                            entriesOriginalLoad = null;
                            entriesIsTransparentLoad = null;
                            entriesApplicationLoad = null;
                            entriesApplicationIconLoad = null;
                            entriesChecksumLoad = null;
                        }
                    }
                    Logging.Log("---");
                }

                onLoadAllEntriesInClipboardQueue = true;
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Logging.Log("Error loading data file: " + ex.Message);
            }
        }


        // ###########################################################################################
        // Save the full list of mapping between original saved clipboard entry
        // index (from data file) and the new ordered list entry index
        // ###########################################################################################   

        public static void SaveIndexesToFile()
        {

            Logging.Log($"Saving encrypted \"index\" file [{Settings.pathAndDataIndex}]");

            try
            {

                SortedDictionary<int, int> entriesOrderTmp = new SortedDictionary<int, int>();

                string save = Settings.GetRegistryKey(Settings.registryPath, "StorageSaveType");

                if (save == "Text")
                {
                    foreach (var entry in Settings.entriesOrder)
                    {
                        bool isImage = Settings.entriesIsImage[entry.Key];
                        if (!isImage)
                        {
                            entriesOrderTmp.Add(entry.Key, entry.Value);
                        }
                    }
                }

                if (save == "Favorites")
                {

                    foreach (var entry in Settings.entriesOrder)
                    {
                        bool isFavorite = Settings.entriesIsFavorite[entry.Key];
                        if (isFavorite)
                        {
                            entriesOrderTmp.Add(entry.Key, entry.Value);
                        }
                    }
                }

                // Get the list of indexes
                if (save == "Text" || save == "Favorites")
                {
                    entriesOrderLoad = entriesOrderTmp.Values.ToList();
                }
                else
                {
                    entriesOrderLoad = Settings.entriesOrder.Values.ToList();
                }

//                Settings.entriesOrder = entriesOrderLoad;

                // Overwrite file
                using (var fileStream = new FileStream(Settings.pathAndDataIndex, FileMode.Create))
                using (var writer = new StreamWriter(fileStream))
                {
                    writer.WriteLine($"HovText {Settings.appVer}");
                    writer.Flush();
                }

                // Convert the list to a byte array
                var serializedData = BitConverter.GetBytes(entriesOrderLoad.Count)
                    .Concat(entriesOrderLoad.Select(index => BitConverter.GetBytes(index))
                    .SelectMany(bytes => bytes))
                    .ToArray();

                // Encrypt the serialized data
                var encryptedData = Encryption.EncryptStringToBytes_Aes(serializedData, Settings.encryptionKey, Settings.encryptionInitializationVector);

                // Write the encrypted data to a file
                using (var fileStream = new FileStream(Settings.pathAndDataIndex, FileMode.Append))
                {
                    // Write the length of the encrypted data
                    byte[] encryptedDataLengthBytes = BitConverter.GetBytes(encryptedData.Length);
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(encryptedDataLengthBytes);
                    }
                    fileStream.Write(encryptedDataLengthBytes, 0, encryptedDataLengthBytes.Length);

                    // Write the encrypted data
                    fileStream.Write(encryptedData, 0, encryptedData.Length);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Logging.Log("Error saving index file: " + ex.Message);
            }
        }


        // ###########################################################################################
        // Load the full list of mapping between original saved clipboard entry
        // index (from data file) and the new ordered list entry index
        // ###########################################################################################   

        public static void LoadIndexesFromFile()
        {
            entriesOrderLoad = new List<int>();
            Logging.Log($"Reading encrypted \"index\" file [{Settings.pathAndDataIndexLoad}]");

            try
            {
                // Read the version name from the first line of the file
                string version;
                using (var tempStream = new FileStream(Settings.pathAndDataIndexLoad, FileMode.Open))
                using (var reader = new StreamReader(tempStream))
                {
                    version = reader.ReadLine();
                    Logging.Log($"Version info from \"index\" file is [{version}]");
                }

                using (var fileStream = new FileStream(Settings.pathAndDataIndexLoad, FileMode.Open, FileAccess.Read))
                {
                    // Skip the first line as this is the "version" line
                    fileStream.Position = Encoding.UTF8.GetBytes(version + Environment.NewLine).Length;

                    while (fileStream.Position < fileStream.Length)
                    {
                        byte[] encryptedDataLengthBytes = new byte[sizeof(int)];
                        fileStream.Read(encryptedDataLengthBytes, 0, encryptedDataLengthBytes.Length);

                        if (BitConverter.IsLittleEndian)
                        {
                            Array.Reverse(encryptedDataLengthBytes);
                        }

                        int encryptedDataLength = BitConverter.ToInt32(encryptedDataLengthBytes, 0);

                        byte[] encryptedData = new byte[encryptedDataLength];
                        fileStream.Read(encryptedData, 0, encryptedData.Length);

                        var decryptedData = Encryption.DecryptStringFromBytes_Aes(encryptedData, Settings.encryptionKey, Settings.encryptionInitializationVector);

                        if (decryptedData == null)
                        {
                            Logging.Log("Encryption mismatches and \"index\" file cannot be read - will be overwritten");
                        }
                        else
                        {
                            int indexCount = BitConverter.ToInt32(decryptedData, 0);
                            Logging.Log($"Number of \"indexes\" read: [{indexCount}]");

                            for (int i = sizeof(int); i < decryptedData.Length; i += sizeof(int))
                            {
                                int index = BitConverter.ToInt32(decryptedData, i);
                                entriesOrderLoad.Add(index);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Log("Error loading index file: " + ex.Message);
            }
        }

        
        // ###########################################################################################
        // Save the full list of "is favorite" booleans to a data file
        // ###########################################################################################   

        public static void SaveFavoritesToFile()
        {
            Logging.Log($"Saving encrypted \"favorite\" file [{Settings.pathAndDataFavorite}]");

            // Filter the entries to only include those with value true
            var trueFavorites = Settings.entriesIsFavorite.Where(kv => kv.Value).ToDictionary(kv => kv.Key, kv => kv.Value);

            Dictionary<int, int> mapping = new Dictionary<int, int>();
            Dictionary<int, bool> trueFavorites2 = new Dictionary<int, bool>();

            foreach (var entry in trueFavorites)
            {
                // Check if the key exists in entriesOrder
                if (Settings.entriesOrder.TryGetValue(entry.Key, out int value))
                {
                    mapping.Add(entry.Key, value);
                }
            }

            foreach (var pair in mapping)
            {
                trueFavorites2.Add(pair.Value, true);
            }

            // Overwrite file
            using (var fileStream = new FileStream(Settings.pathAndDataFavorite, FileMode.Create))
            using (var writer = new StreamWriter(fileStream))
            {
                writer.WriteLine($"HovText {Settings.appVer}");
                writer.Flush();
            }

            // Update the Tuple type declaration to use SortedDictionary
            var dataToSerialize = new Tuple<
                SortedDictionary<int, bool>
            >(new SortedDictionary<int, bool>(trueFavorites2));

            // Serialization process
            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, dataToSerialize);
                var serializedData = memoryStream.ToArray();

                // Encrypt the serialized data
                var encryptedData = Encryption.EncryptStringToBytes_Aes(serializedData, Settings.encryptionKey, Settings.encryptionInitializationVector);

                // Write the encrypted data to a file
                using (var fileStream = new FileStream(Settings.pathAndDataFavorite, FileMode.Append))
                {
                    // Write the length of the encrypted data
                    byte[] encryptedDataLengthBytes = BitConverter.GetBytes(encryptedData.Length);
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(encryptedDataLengthBytes);
                    }
                    fileStream.Write(encryptedDataLengthBytes, 0, encryptedDataLengthBytes.Length);

                    // Write the encrypted data
                    fileStream.Write(encryptedData, 0, encryptedData.Length);
                }
            }
        }


        // ###########################################################################################
        // Load the full list of "is favorite" booleans from a data file
        // ###########################################################################################   

        public static void LoadFavoritesFromFile()
        {
            if (File.Exists(Settings.pathAndDataFavoriteLoad))
            {
                Logging.Log("Reading encrypted \"favorite\" file [pathAndDataFavoriteLoad]");

                try
                {
                    // Read the version name from the first line of the file
                    string version;
                    using (var tempStream = new FileStream(Settings.pathAndDataFavoriteLoad, FileMode.Open))
                    using (var reader = new StreamReader(tempStream))
                    {
                        version = reader.ReadLine();
                        Logging.Log($"Version saved to \"favorite\" file is [{version}]");
                    }

                    int favoriteCount = 0;

                    using (var fileStream = new FileStream(Settings.pathAndDataFavoriteLoad, FileMode.Open))
                    {
                        // Skip the first line as this is the "version" line
                        fileStream.Position = Encoding.UTF8.GetBytes(version + Environment.NewLine).Length;

                        while (fileStream.Position < fileStream.Length)
                        {
                            int encryptedDataLength;

                            // Read the length of the encrypted data
                            byte[] encryptedDataLengthBytes = new byte[sizeof(int)];
                            fileStream.Read(encryptedDataLengthBytes, 0, encryptedDataLengthBytes.Length);

                            // Reverse the byte order if the system is little-endian
                            if (BitConverter.IsLittleEndian)
                            {
                                Array.Reverse(encryptedDataLengthBytes);
                            }

                            encryptedDataLength = BitConverter.ToInt32(encryptedDataLengthBytes, 0);

                            // Read the encrypted data from the file
                            byte[] encryptedData = new byte[encryptedDataLength];
                            fileStream.Read(encryptedData, 0, encryptedData.Length);

                            // Decrypt the data
                            var decryptedData = Encryption.DecryptStringFromBytes_Aes(encryptedData, Settings.encryptionKey, Settings.encryptionInitializationVector);

                            entriesIsFavoriteLoad = null;

                            // Check if decryption was successful
                            if (decryptedData == null)
                            {
                                Logging.Log("Encryption mismatches and \"favorite\" file cannot be read - will be overwritten");
                                break; // Break out of the loop if decryption fails
                            }
                            else
                            {

                                try
                                {
                                    // Deserialize the decrypted data
                                    using (var memoryStream = new MemoryStream(decryptedData))
                                    {
                                        var binaryFormatter = new BinaryFormatter();
                                        var loadedData = (Tuple<
                                            SortedDictionary<int, bool>
                                        >)binaryFormatter.Deserialize(memoryStream);

                                        // Extract the dictionaries from the tuple into temporary dictionaries
                                        entriesIsFavoriteLoad = loadedData.Item1;
                                        favoriteCount += entriesIsFavoriteLoad.Count; // Increment the counter
                                    }
                                }
                                catch (SerializationException ex)
                                {
                                    Logging.Log($"Error during deserialization: {ex.Message}");
                                    //                                entriesIsFavoriteLoad = null;
                                    break; // Break out of the loop if deserialization fails
                                }
                            }
                        }

                        Logging.Log($"Number of \"favorites\" read: [{favoriteCount}]"); // Log the total count
                    }
                }
                catch (Exception ex)
                {
                    // Handle other exceptions
                    Logging.Log("Error loading favorite file: " + ex.Message);
                }
            }
        }      


        // ###########################################################################################
        // Save both the "index" and the "favorite" file
        // ###########################################################################################   

        public static void SaveIndexAndFavoritesToFiles()
        {
            SaveIndexesToFile();
            SaveFavoritesToFile();
        }


        // ###########################################################################################
        // Check if we have write access to the folder where the executable file resides
        // ###########################################################################################   

        public static bool HasWriteAccess()
        {
            try
            {
                // Attempt to create a temporary file in the specified folder
                string tempFilePath = Path.Combine(Settings.baseDirectory, Path.GetRandomFileName());
                using (FileStream fs = File.Create(tempFilePath)) { }

                // If successful, delete the temporary file and return true
                try
                {
                    File.Delete(tempFilePath);
                }
                catch (Exception ex)
                {
                    Logging.Log("Error: " + ex.Message);
                }
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                // If an UnauthorizedAccessException occurs, it means no write access
                return false;
            }
        }


        // ###########################################################################################
        // MessageBox for showing "missing write access"
        // ###########################################################################################   

        public static void InformOnMissingWriteAccess()
        {
            MessageBox.Show("HovText cannot write to the path [" + Settings.baseDirectory + "]!\r\n\r\nDisabling \"Troubleshooting\" and \"Save clipboards\".\r\n\r\nPlease make sure you have write permission to this folder, if you want to use these functionalities.",
                 "HovText ERROR",
                MessageBoxButtons.OK,
                 MessageBoxIcon.Error);
        }


        // ###########################################################################################
        // Delete troubleshoot logfile and clipboard data file - called when doing a clean-up
        // ###########################################################################################

        public static bool DeleteOldFiles()
        {
            bool wasAnyFilesDeleted = false;

            // HovText-data-*.bin
            string fileMask = $"{Settings.dataName}*.bin";
            string fileName = HandleFiles.GetOldestFile(Settings.baseDirectory, fileMask);
            if (fileName != Settings.pathAndData)
            {
                if (File.Exists(fileName))
                {
                    try
                    {
                        File.Delete(@fileName);
                        wasAnyFilesDeleted = true;
                        Logging.Log($"Deleted clipboard data file [{fileName}]");
                    }
                    catch (Exception ex)
                    {
                        Logging.Log($"Could not delete clipboard data file [{fileName}] as it was locked by another process");
                        Logging.Log($"Exception: {ex.Message}");
                    }
                }
            }

            // HovText-index-*.bin
            fileMask = $"{Settings.dataIndexName}*.bin";
            fileName = HandleFiles.GetOldestFile(Settings.baseDirectory, fileMask);
            if (fileName != Settings.pathAndDataIndex)
            {
                if (File.Exists(fileName))
                {
                    try
                    {
                        File.Delete(@fileName);
                        wasAnyFilesDeleted = true;
                        Logging.Log($"Deleted clipboard data index file [{fileName}]");
                    }
                    catch (Exception ex)
                    {
                        Logging.Log($"Could not delete clipboard data index file [{fileName}] as it was locked by another process");
                        Logging.Log($"Exception: {ex.Message}");
                    }
                }
            }

            // HovText-favorite-*.bin
            fileMask = $"{Settings.dataFavoriteName}*.bin";
            fileName = HandleFiles.GetOldestFile(Settings.baseDirectory, fileMask);
            if (fileName != Settings.pathAndDataFavorite)
            {
                if (File.Exists(fileName))
                {
                    try
                    {
                        File.Delete(@fileName);
                        wasAnyFilesDeleted = true;
                        Logging.Log($"Deleted clipboard data is-favorite file [{fileName}]");
                    }
                    catch (Exception ex)
                    {
                        Logging.Log($"Could not delete clipboard data is-favorite file [{fileName}] as it was locked by another process");
                        Logging.Log($"Exception: {ex.Message}");
                    }
                }
            }

            // Delete the temporary updater file
            string tempExe = Path.Combine(Settings.baseDirectory, Settings.updaterExe);
            if (File.Exists(tempExe))
            {
                try
                {
                    File.Delete(@tempExe);
                    wasAnyFilesDeleted = true;
                    Logging.Log($"Deleted temporary updater file [{tempExe}]");
                }
                catch (Exception ex)
                {
                    Logging.Log($"Could not delete temporary updater file [{tempExe}] as it was locked by another process");
                    Logging.Log($"Exception: {ex.Message}");
                }
            }

            return wasAnyFilesDeleted;
        }


        // ###########################################################################################
        // Delete troubleshoot logfile and clipboard data file - called when doing a clean-up
        // ###########################################################################################

        public static void DeleteFilesOnCleanup()
        {
            try
            {
                // HovText-data-*.bin
                string fileMask = $"{Settings.dataName}*.bin";
                foreach (var filePath in Directory.GetFiles(Settings.baseDirectory, fileMask))
                {
                    File.Delete(filePath);
                }

                // HovText-index-*.bin
                fileMask = $"{Settings.dataIndexName}*.bin";
                foreach (var filePath in Directory.GetFiles(Settings.baseDirectory, fileMask))
                {
                    File.Delete(filePath);
                }

                // HovText-favorite-*.bin
                fileMask = $"{Settings.dataFavoriteName}*.bin";
                foreach (var filePath in Directory.GetFiles(Settings.baseDirectory, fileMask))
                {
                    File.Delete(filePath);
                }

                // HovText Updater.exe
                string tempExe = Path.Combine(Settings.baseDirectory, Settings.updaterExe);
                if (File.Exists(tempExe))
                {
                    File.Delete(@tempExe);
                }

                // HovText-troubleshooting.txt
                if (File.Exists(Settings.pathAndLog))
                {
                    File.Delete(@Settings.pathAndLog);
                }
            }
            catch (Exception ex)
            {
                Logging.Log("Error: " + ex.Message);
            }
        }


        // ###########################################################################################
        // Get the NEWEST file, based on file mask
        // ###########################################################################################   

        public static string GetNewestFile(string directoryPath, string fileMask)
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);

                var files = directoryInfo.GetFiles(fileMask)
                                         .OrderByDescending(f => f.LastWriteTime)
                                         .ToList();
                return files.FirstOrDefault()?.FullName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }


        // ###########################################################################################
        // Get the OLDEST file, based on file mask
        // ###########################################################################################   

        public static string GetOldestFile(string directoryPath, string fileMask)
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);

                var files = directoryInfo.GetFiles(fileMask)
                                         .OrderBy(f => f.LastWriteTime)
                                         .ToList();
                return files.FirstOrDefault()?.FullName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }

        // ###########################################################################################
    }
}