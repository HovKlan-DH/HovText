﻿/*
##################################################################################################
HANDLECLIPBOARD (CLASS)
-----------------------

This will handle all clipboard activities.

Clipboard capturing (getting UPDATE event + fetching data) will not
be done in a thread - it will run as normal.
Processing the data, dequeuing and getting text/image etc. will be done 
in a thread.

##################################################################################################
*/

using static HovText.Program;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace HovText
{
    public class HandleClipboard
    {

        // ###########################################################################################
        // Class variables
        // ###########################################################################################   
        public static ConcurrentQueue<(int, Dictionary<string, object>, string, Image, bool)> clipboardQueue = new ConcurrentQueue<(int, Dictionary<string, object>, string, Image, bool)>();
        public static IntPtr originatingHandle = IntPtr.Zero;
        private Form _formSettings;
        public static int threadSafeIndex = 0; // this number will be sequential increased via a thread-safe method - it will show the number that can be used for inserting to array, so a number of "1" means that one entry is inserted already
        private readonly object clipboardData_lock = new object();


        // ###########################################################################################
        // Called whenever there is a clipboard [UPDATE] event.
        // Called in a parallized thread.
        // Will store relevant clipboard data in a tuple queue (thread-safe, nice).
        // ###########################################################################################   

        public void GetClipboardData(string whoUpdatedClipboardName, uint thisProcessId)
        {
            Logging.Log($"Capturing clipboard data");

            // Get if the clipboard contains either text or an image
            bool containsText = false;
            bool containsImage = false;
            try
            {
                containsText = Clipboard.ContainsText();
                containsImage = Clipboard.ContainsImage();
            }
            catch (Exception ex)
            {
                Logging.Log($"Error: " + ex.Message);
            }
                
            if (containsImage && !Settings.isCopyImages)
            {
                Logging.Log($"Clipboard is an image and we should not capture images - ignoring it");
            }
            else if (containsText || containsImage) // only add to the queue, if it contains either a text or an image
            {
                // Lock clipboard variables (not the clipboard itself)
                lock (clipboardData_lock)
                {

                    // Find the last key from the data arrays and then add one (if this is not the first entry)
                    int insertIndex = Interlocked.Increment(ref threadSafeIndex) - 1; // thread-safe incremental of a sequence number

                    // Define an object that will contain all the formats we want to handle
                    Dictionary<string, object> clipboardObject = new Dictionary<string, object>();

                    // Get clipboard data formats and add this to the clipboard object
                    try
                    {
                        // Make a snapshot of the clipboard
                        IDataObject clipboardIDataObject = Clipboard.GetDataObject();

                        var formats = clipboardIDataObject.GetFormats(false);
                        //Logging.Log($"index=[{clipboardQueueIndex}] Formats available on clipboard:");
                        Logging.Log($"index=[{insertIndex}] Formats available on clipboard:");
                        foreach (var format in formats)
                        {
                            if (
                                format.Contains("Text")
                                || format.Contains("HTML")
                                || format.Contains("Csv")
                                || format.Contains("Link")
                                || format.Contains("Hyperlink")
                                || format.Contains("Bitmap")
                                || format.Contains("PNG") // including transparent layer of PNG
                                || format.Contains("Recipient") // Outlook recipient
                                || format.Contains("Format17") // picture format
                                || format.Contains("GIF")
                                || format.Contains("JFIF")
                                || format.Contains("Office Drawing Shape Format")
                                || format.Contains("Preferred DropEffect") // used in e.g. animated GIF
                                || format.Contains("Shell IDList Array") // used in e.g. animated GIF
                                || format.Contains("FileDrop") // used in e.g. animated GIF
                                || format.Contains("FileContents") // used in e.g. animated GIF
                                || format.Contains("FileGroupDescriptorW") // used in e.g. animated GIF
                                || format.Contains("Image") // seen on "CorelPHOTOPAINT.Image.20" and "CorelPhotoPaint.Image.9"
                                || format.Contains("Color") // seen on "Corel.Color.20"
                            )
                            {
                                var data = clipboardIDataObject.GetData(format);
                                clipboardObject.Add(format, data);
                                Logging.Log($"index=[{insertIndex}]    Adding format [{format}]");
                                
                                for (int i=1; i <= 5; i++)
                                {
                                    if (clipboardObject.TryGetValue(format, out object value))
                                    {
                                        if (value == null || string.IsNullOrEmpty(value.ToString()))
                                        {
                                            data = clipboardIDataObject.GetData(format);
                                            clipboardObject.Remove(format);
                                            clipboardObject.Add(format, data);
                                            Logging.Log($"index=[{insertIndex}]    Adding format [{format}] - retry [{i}/5]");
                                        } else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Logging.Log($"index=[{insertIndex}]    Discarding format [{format}]");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.Log($"index=[{insertIndex}] Error: " + ex.Message);
                    }

                    // Find the process icon - if possible. Note that applications running with
                    // higher priveledges cannot be queried for the icon
                    Image whoUpdatedClipboardIcon = null;
                    try
                    {
                        Process process = null;
                        process = Process.GetProcessById((int)thisProcessId);
                        if (process != null && !process.HasExited)
                        {
                            string processFilePath = process.MainModule.FileName;
                            using (Icon appIconTmp = Icon.ExtractAssociatedIcon(processFilePath))
                            {
                                whoUpdatedClipboardIcon = appIconTmp.ToBitmap();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.Log($"index=[{insertIndex}] Warning: Cannot get application icon: {ex.Message}");
                        //Logging.LogException(ex);
                    }

                    // Thread-safe adding to the tuple queue
                    clipboardQueue.Enqueue((
                        insertIndex,
                        clipboardObject,
                        whoUpdatedClipboardName,
                        whoUpdatedClipboardIcon,
                        false
                    ));
                    Logging.Log($"index=[{insertIndex}] Enqueued");
                }
            }
        }


        // ###########################################################################################
        // Called from timer that will process the queue coming from either clipboard or loading.
        // Called in a parallized thread.
        // ###########################################################################################   

        public void ReleaseClipboardQueue()
        {
            int loadQueueCounter = clipboardQueue.Count; // used when loading files, to validate how many entries is processed

            // Dequeue oldest element
            while (clipboardQueue.TryDequeue(out var clipboard))
            {
                // Set the tuple variables
                int index = clipboard.Item1;
                Dictionary<string, object> clipboardObject = clipboard.Item2;
                string whoUpdatedClipboardName = clipboard.Item3;
                Image whoUpdatedClipboardIcon = clipboard.Item4;
                bool isFavorite = clipboard.Item5;

                Logging.Log($"index=[{index}] Releasing clipboard data");

                // Make sure that we set the "threadSafeIndex" to the highest possible number, in case this is coming from "loading file"
                if (index >= threadSafeIndex)
                {
                    threadSafeIndex = index + 1;
                }

                // Get a boolean if this is either TEXT or an IMAGE
                bool isClipboardText = clipboardObject.ContainsKey(DataFormats.Text) ||
                    clipboardObject.ContainsKey(DataFormats.UnicodeText) ||
                    clipboardObject.ContainsKey(DataFormats.Rtf);
                bool isClipboardImage = clipboardObject.ContainsKey(DataFormats.Bitmap);

                // Get TEXT clipboard content
                string clipboardText = ""; // should not be NULL, as we are filtering on text-values later
                if(isClipboardText)
                {
                    if (clipboardObject.ContainsKey(DataFormats.UnicodeText))
                    {
                        clipboardText = clipboardObject[DataFormats.UnicodeText] as string;
                        Logging.Log($"index=[{index}] Found text format [UnicodeText]");
                    }
                    else if (clipboardObject.ContainsKey(DataFormats.Rtf))
                    {
                        clipboardText = clipboardObject[DataFormats.Rtf] as string;
                        Logging.Log($"index=[{index}] Found text format [Rtf]");
                    }
                    else if (clipboardObject.ContainsKey(DataFormats.Text))
                    {
                        clipboardText = clipboardObject[DataFormats.Text] as string;
                        Logging.Log($"index=[{index}] Found text format [Text]");
                    }
                    else
                    {
                        clipboardText = ""; // no supported text format found
                        Logging.Log($"index=[{index}] Error: no supported text formats found!?");
                        isClipboardText = false;
                    }
                }

                Image clipboardImage = null;
                if (isClipboardImage)
                {
                    clipboardImage = clipboardObject[DataFormats.Bitmap] as Image;
                    Logging.Log($"index=[{index}] Found image format [Bitmap]");
                }

                // Set default values
                string checksum = null;
                bool isClipboardImageTransparent = false;
                bool isAlreadyInDataArray = false;
                bool skipRest = false; // "true" if we should skip the rest and return at end of function

                // TEXT - is clipboard text - also triggers when copying whitespaces only
                if (isClipboardText)
                {
                    // Trim the text for whitespaces and empty new-lines
                    if (clipboardText == null || clipboardText.Trim().Length == 0) {
                        skipRest = true;
                    }

                    if (skipRest)
                    {
                        Logging.Log($"index=[{index}] Error: Retrieved text is empty - ignoring clipboard entry - is copying/processing going too fast???");
                    } else { 
                        checksum = GetStringHash(clipboardText.Trim());

                        Logging.Log($"index=[{index}] Text checksum: [{checksum}]");

                        // Check if we already have the clipboard in the clipboard list - if so, reuse this
                        isAlreadyInDataArray = IsClipboardContentAlreadyInArrays(index, isClipboardText, isClipboardImage, checksum);
                    }
                }

                // IMAGE - is clipboard an image AND only if we include images in the clipboard list also
                Image transparentImage = null;
                if (isClipboardImage && Settings.isCopyImages)
                {
                    // Get hash value of picture in clipboard
                    checksum = GetImageHash(clipboardImage);

                    Logging.Log($"index=[{index}] Image checksum: [{checksum}]");

                    isAlreadyInDataArray = IsClipboardContentAlreadyInArrays(index, isClipboardText, isClipboardImage, checksum);

                    if (!isAlreadyInDataArray)
                    {
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();

                        transparentImage = GetTransparentImageFromClipboard(clipboardObject, index);
                        if(transparentImage != null)
                        {
                            isClipboardImageTransparent = IsImageTransparent(transparentImage);
                        } else
                        {
                            isClipboardImageTransparent = false;
                        }
                        
                        stopwatch.Stop();
                        Logging.Log($"index=[{index}] Execution time for transparency check: {stopwatch.Elapsed.TotalSeconds} seconds");
                    }
                }

                // Only add the entry, if it is not already present in list
                if (!isAlreadyInDataArray && !skipRest && (isClipboardText || isClipboardImage))
                {
                    AddClipboardToList(
                        index,
                        clipboardObject,
                        isClipboardText,
                        isClipboardImage,
                        isClipboardImageTransparent,
                        clipboardText,
                        clipboardImage,
                        transparentImage,
                        whoUpdatedClipboardName,
                        whoUpdatedClipboardIcon,
                        checksum,
                        isFavorite
                    );

                    // Set the clipboard, if we have some text
                    // AND we should not always paste the original text
                    // AND it is not set to paste on hotkey only
                    if (isClipboardText)
                    {
                        if (HandleFiles.onLoadAllEntriesProcessedInClipboardQueue)
                        {
                            if (!string.IsNullOrEmpty(clipboardText) && !Settings.isEnabledAlwaysPasteOriginal && !Settings.isEnabledPasteOnHotkey)
                            {
                                _formSettings.Invoke(new MethodInvoker(() =>
                                {
                                    if (clipboardQueue.Count == 0 && !Settings.isProcessingClipboard)
                                    {
                                        SetClipboard(index);
                                    }
                                    else
                                    {
                                        Logging.Log($"index=[{index}] Warning: Skipping setting clipboard as new relevant clipboard UPDATE event detected");
                                    }
                                }));
                            }
                        }
                    }
                }
            }

            Settings.isProcessingClipboardQueue = false;
            if (Settings.entriesOrder.Count == loadQueueCounter)
            {
                HandleFiles.onLoadAllEntriesProcessedInClipboardQueue = true;
            }
        }


        // ###########################################################################################
        // Check if the data is already present in one of the arrays (text or image)
        // ###########################################################################################   

        private bool IsClipboardContentAlreadyInArrays (
            int index,
            bool isClipboardText,
            bool isClipboardImage,
            string checksum)
        {
            if (Settings.entriesTextTrimmed.Count > 0)
            {
                for (int i = 0; i < Settings.entriesTextTrimmed.Count; i++)
                {

                    if (isClipboardText)
                    {
                        // If the two data are identical then move the entry to the top
                        if (Settings.entriesChecksum.ElementAt(i).Value == checksum)
                        {
                            Logging.Log($"index=[{index}] Text is already in data array - reusing it");
                            int moveIndex = Settings.entriesTextTrimmed.ElementAt(i).Key;
                            Settings.MoveEntryToTop(moveIndex);
                            return true;
                        }
                    }
                    else if (isClipboardImage)
                    {
                        if (Settings.entriesChecksum.ElementAt(i).Value == checksum)
                        {
                            Logging.Log($"index=[{index}] Image is already in data array - reusing it");
                            int moveIndex = Settings.entriesTextTrimmed.ElementAt(i).Key;
                            Settings.MoveEntryToTop(moveIndex);
                            return true;
                        }
                    }
                }
            }
            return false; // we did not find any identical data
        }


        // ###########################################################################################
        // Calculate the hash value of an ímage or a text string
        // ###########################################################################################

        private string GetImageHash(Image img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                // Save image to the stream
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

                // Compute hash from the stream
                using (var sha256 = System.Security.Cryptography.SHA256.Create())
                {
                    stream.Position = 0; // Reset the stream position to the beginning
                    byte[] hash = sha256.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        private string GetStringHash(string text)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                // Convert the string to a byte array
                byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(text);

                // Compute the hash of the byte array
                byte[] hashBytes = sha256.ComputeHash(textBytes);

                // Convert the byte array to a hexadecimal string
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }


        // ###########################################################################################
        // Add clipboard data to the various arrays
        // ###########################################################################################   

        private void AddClipboardToList(
            int index,
            Dictionary<string, object> clipboardObject,
            bool isClipboardText,
            bool isClipboardImage,
            bool isClipboardImageTransparent,
            string clipboardText,
            Image clipboardImage,
            Image transparentImage,
            string whoUpdatedClipboardName,
            Image whoUpdatedClipboardIcon,
            string checksum,
            bool isFavorite
        )
        {
            // Log if this is a TEXT or IMAGE clipboard
            if (isClipboardText)
            {
                Logging.Log($"index=[{index}] Adding new [TEXT] entry from application [" + whoUpdatedClipboardName + "] to clipboard list");
            }
            else
            {
                Logging.Log($"index=[{index}] Adding new [IMAGE] entry from application [" + whoUpdatedClipboardName + "] to clipboard list");
            }

            // clipboardText
            Settings.entriesText.Add(index, clipboardText);

            // clipboardTextTrimmed
            Settings.entriesTextTrimmed.Add(index, clipboardText.Trim());

            // entriesImage
            if (clipboardImage != null)
            {
                // Resize image, if needed
                Bitmap bmp = new Bitmap(clipboardImage);
                Bitmap resizedBmp = ResizeImage(bmp, 400, 800); // width, height
                Settings.entriesImage.Add(index, resizedBmp);
            }
            else
            {
                Settings.entriesImage.Add(index, null);
            }

            // entriesImageTrans
            // entriesIsTransparent
            if (clipboardImage != null && isClipboardImageTransparent)
            {
                Bitmap bmp = new Bitmap(transparentImage);
                Bitmap resizedBmp = ResizeImage(bmp, 200, 400); // width, height
                Settings.entriesImageTrans.Add(index, (Image)resizedBmp);
                Settings.entriesIsTransparent.Add(index, true);
            }
            else
            {
                Settings.entriesImageTrans.Add(index, null);
                Settings.entriesIsTransparent.Add(index, false);
            }

            // entriesChecksum
            Settings.entriesChecksum.Add(index, checksum);

            // entriesIsFavorite
            if(Settings.isEnabledFavorites)
            {
                Settings.entriesIsFavorite.Add(index, isFavorite);
            } else
            {
                Settings.entriesIsFavorite.Add(index, false);
            }            

            // entriesIsUrl
            bool isUrl = Uri.TryCreate(clipboardText.Trim(), UriKind.Absolute, out Uri myUri) && (myUri.Scheme == Uri.UriSchemeHttp || myUri.Scheme == Uri.UriSchemeHttps || myUri.Scheme == Uri.UriSchemeFtp || myUri.Scheme == "ws" || myUri.Scheme == "wss");
            if (isUrl)
            {
                Settings.entriesIsUrl.Add(index, true);
            }
            else
            {
                Settings.entriesIsUrl.Add(index, false);
            }

            // entriesIsEmail
            bool isEmail = false;
            if (isClipboardText)
            {
                isEmail = Regex.IsMatch(clipboardText.Trim(), @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$");
            }
            Settings.entriesIsEmail.Add(index, isEmail);

            // entriesIsImage
            if (isClipboardImage)
            {
                Settings.entriesIsImage.Add(index, true);
            }
            else
            {
                Settings.entriesIsImage.Add(index, false);
            }

            // entriesOriginal
            Settings.entriesOriginal.Add(index, clipboardObject);

            // entriesApplication
            // entriesApplicationIcon
            Settings.entriesApplication.Add(index, whoUpdatedClipboardName);
            Settings.entriesApplicationIcon.Add(index, whoUpdatedClipboardIcon);

            // Append the (one) newly processed clipboard data to the data file
            Settings.clipboardSaveQueue.Add(index);

            // Add the original index to the "order" array (we only save the clipboard once)
            Settings.entriesOrder.Add(index, index);

            // Check if we have too many entries in the clipboard list - if so, remove the oldest one
            if (Settings.entriesOrder.Count > Settings.clipboardEntriesToSave)
            {
                foreach (var key in Settings.entriesOrder.Keys.ToList())
                {
                    // Exit the loop once the number of entries is within the limit
                    if (Settings.entriesOrder.Count <= Settings.clipboardEntriesToSave)
                    {
                        break;
                    }

                    // Check if the entry is marked as favorite
                    if (Settings.entriesIsFavorite.TryGetValue(key, out bool thisIsFavorite) && thisIsFavorite)
                    {
                        continue;
                    }

                    Logging.Log($"index=[{index}] Max entries of [{Settings.clipboardEntriesToSave}] has been reached in the clipboard list - removing entry index [{key}] from clipboard list");

                    // Remove the chosen entry, as it's not a favorite
                    Settings.RemoveEntryFromLists(key);
                }
            }

            Logging.Log($"index=[{index}] Entries in clipboard list is now [" + Settings.entriesTextTrimmed.Count + "]");
        }


        // ###########################################################################################
        // Check if the image contains any transparency
        // https://stackoverflow.com/a/2570002/2028935 but modified later by ChatGPT
        // ###########################################################################################

        private Image GetTransparentImageFromClipboard(Dictionary<string, object> clipboardObject, int index)
        {
            // This one here is a little weird, but 
            try
            {
                if (clipboardObject[DataFormats.Dib] == null)
                {
                    return null;
                }
            } catch {
                return null;
            }            

            // Get the "Dib" format as a byte array, but sometimes this fails (not sure why!?)
            byte[] dib = null;
            if (clipboardObject.TryGetValue(DataFormats.Dib, out var dataObject))
            {
                if (dataObject is MemoryStream memoryStream)
                {
                    dib = memoryStream.ToArray();
                }
                else if (dataObject is byte[] byteArray)
                {
                    dib = byteArray;
                }
                else
                {
                    Logging.Log($"index=[{index}] Error: Image is not in correct format!?");
                }
            }

            var width = BitConverter.ToInt32(dib, 4);
            var height = BitConverter.ToInt32(dib, 8);
            var bpp = BitConverter.ToInt16(dib, 14);
            if (bpp == 32)
            {
                var gch = GCHandle.Alloc(dib, GCHandleType.Pinned);
                Bitmap bmp = null;
                try
                {
                    var ptr = new IntPtr((long)gch.AddrOfPinnedObject() + 40);
                    bmp = new Bitmap(width, height, width * 4, System.Drawing.Imaging.PixelFormat.Format32bppArgb, ptr);

                    // Create a new bitmap to flip the original vertically
                    Bitmap flippedBmp = new Bitmap(width, height);
                    using (Graphics g = Graphics.FromImage(flippedBmp))
                    {
                        // Set transformation for vertical flip
                        g.ScaleTransform(1, -1);
                        g.TranslateTransform(0, -height);
                        // Draw the original bitmap onto the new bitmap with the transformation
                        g.DrawImage(bmp, new Rectangle(0, 0, width, height));
                    }

                    return flippedBmp;
                }
                finally
                {
                    gch.Free();
                    bmp?.Dispose();
                }
            }

            return null;
        }


        // ###########################################################################################
        // Check if the image contains any transparency (alpha)?
        // https://stackoverflow.com/a/2570002/2028935
        // ###########################################################################################

        private static bool IsImageTransparent(Image image)
        {
            Bitmap img = new Bitmap(image);
            for (int y = 0; y < img.Height; ++y)
            {
                for (int x = 0; x < img.Width; ++x)
                {
                    if (img.GetPixel(x, y).A != 255)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        // ###########################################################################################
        // Place data in the clipboard, based on the entry index
        // ###########################################################################################

        public static void SetClipboard(int index)
        {
            string entryText;
            Image entryImage;
            bool isEntryText;
            bool isEntryImage;

            if (threadSafeIndex > 0)
            {
                if(Settings.isEnabledTrimWhitespacing)
                {
                    entryText = Settings.entriesTextTrimmed[index];
                } else
                {
                    entryText = Settings.entriesText[index];
                }
                entryImage = Settings.entriesImage[index];
                isEntryText = !string.IsNullOrEmpty(entryText);
                isEntryImage = entryImage != null;

                // Put text to the clipboard
                if (isEntryText)
                {
                    try
                    {
                        if ((Settings.isEnabledPasteOnHotkey && !Settings.pasteOnHotkeySetCleartext) || Settings.isEnabledAlwaysPasteOriginal)
                        {
                            RestoreOriginal(index);
                        }
                        else
                        {
                            Clipboard.SetText(entryText, TextDataFormat.UnicodeText); // https://stackoverflow.com/a/14255608/2028935
                            Logging.Log($"index=[{index}] Set clipboard [SetText]");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.LogException(ex);
                    }
                }
                else if (isEntryImage) // Put an image to the clipboard
                {
                    try
                    {
                        RestoreOriginal(index);
                    }
                    catch (Exception ex)
                    {
                        Logging.LogException(ex);
                    }
                }
                else
                {
                    Logging.Log("index=[{index}] Exception raised:");
                    Logging.Log("index=[{index}]    \"Set clipboard\" triggered but is neither [TEXT] nor [IMAGE]!?");
                }
            }
        }


        // ###########################################################################################
        // Restore the original clipboard (as original as I can get it at least)
        // ###########################################################################################

        public static void RestoreOriginal(int index)
        {
            Logging.Log($"[{index}] Restoring original content to clipboard:");

            try
            {
                // Snippet directly taken from legacy HovText - more or less the only thing? :-)
                // ---
                DataObject data = new DataObject();
                foreach (KeyValuePair<string, object> kvp in Settings.entriesOriginal[index])
                {
                    if (kvp.Value != null)
                    {
                        data.SetData(kvp.Key, kvp.Value);
                        Logging.Log("index=[{index}]    Adding format to clipboard, [" + kvp.Key + "]");
                    }
                }
                Clipboard.SetDataObject(data, true);
                Logging.Log($"index=[{index}] Set clipboard [SetDataObject]");
                // ---
            }
            catch (Exception ex)
            {
                Logging.LogException(ex);
            }
        }


        // ###########################################################################################
        // Resize (if needed) the image
        // ###########################################################################################

        public Bitmap ResizeImage(Image originalImage, int maxWidth, int maxHeight)
        {
            Size newSize = CalculateMaxSizeDimensions(originalImage.Size, maxWidth, maxHeight);

            Bitmap resizedImage = new Bitmap(newSize.Width, newSize.Height);
            using (Graphics graphics = Graphics.FromImage(resizedImage))
            {
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.DrawImage(originalImage, 0, 0, newSize.Width, newSize.Height);
            }

            return resizedImage;
        }


        // ###########################################################################################
        // Recalculate image size, with same ratio aspect
        // ###########################################################################################

        private Size CalculateMaxSizeDimensions(Size originalSize, int maxWidth, int maxHeight)
        {
            // Check if the original size is within the maximum dimensions
            if (originalSize.Width <= maxWidth && originalSize.Height <= maxHeight)
            {
                // No resizing needed, if width and height are smaller than max
                return originalSize;
            }

            float widthRatio = (float)maxWidth / originalSize.Width;
            float heightRatio = (float)maxHeight / originalSize.Height;
            float scaleRatio = Math.Min(widthRatio, heightRatio);

            int newWidth = (int)(originalSize.Width * scaleRatio);
            int newHeight = (int)(originalSize.Height * scaleRatio);

            return new Size(newWidth, newHeight);
        }


        // ###########################################################################################
        // Get process info for active application (the one where the hotkey is pressed)
        // ###########################################################################################

        public static string GetActiveApplicationName()
        {
            // Get the active application
            originatingHandle = NativeMethods.GetForegroundWindow();

            // Get the process ID and find the name for that ID
            uint threadId = NativeMethods.GetWindowThreadProcessId(originatingHandle, out uint processId);
            string appProcessName = Process.GetProcessById((int)processId).ProcessName;
            return appProcessName;
        }


        // ###########################################################################################
        // Instantiate the "Settings Form"
        // ###########################################################################################

        public HandleClipboard(Form formSettings)
        {
            _formSettings = formSettings;
        }


        // ###########################################################################################
    }
}
