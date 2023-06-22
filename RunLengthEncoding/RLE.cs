/*
 * @file   RLE.cs
 * @author Dana
 *
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CompressionRLE
{
    public partial class MainWindow : Window
    {
        const byte BYTE_NOT_REPEATED = 0;
        const byte BYTE_REPEATED = 1;

        public MainWindow()
        {
            InitializeComponent();
        }

        // processing event "Archive"   
        private void ArchiveClick(object sender, RoutedEventArgs e)
        {
            Thread archiveThread = new Thread(ArchiveFunction);

            archiveThread.Start();            
            archiveThread.Join();

            statusBlock.Visibility = Visibility.Hidden;
            statusBlock.Text = "Compressing done";
            statusBlock.FontSize = 14;
            statusBlock.FontWeight = FontWeights.Bold;
            statusBlock.Foreground = Brushes.Green;
            statusBlock.Visibility = Visibility.Visible;
        }

        // creates .rle-file in the same folder
        private void ArchiveFunction()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            openFileDialog.Title = "Choose file for archiving...";
            openFileDialog.Filter = "All files|*.*";

            Nullable<bool> result = openFileDialog.ShowDialog();

            if (result == true) {
                string fileName = openFileDialog.FileName;

                if (File.Exists(fileName + ".rle")) {
                    File.Delete(fileName + ".rle");
                }

                ArchiveFile(fileName, fileName + ".rle");
            }
        }

        // compresses information
        private void ArchiveFile(string inputFileName, string outputFileName)
        {
            FileStream fileRead = new FileStream(inputFileName, FileMode.OpenOrCreate, FileAccess.Read);

            int currentByte = fileRead.ReadByte();
            int nextByte = fileRead.ReadByte();
            currentByte++;
            fileRead.Position = 0;

            do {
                if (nextByte == currentByte) {
                    // bytes are equals
                    currentByte = EqualBytes(fileRead, outputFileName);
                }
                else {
                    // bytes are different                
                    currentByte = DifferentBytes(fileRead, outputFileName);
                }
                nextByte = fileRead.ReadByte();
                fileRead.Position--;
            } 
            while (nextByte != -1);

            fileRead.Close();
        }

        // write same sequence of bytes
        private int EqualBytes(FileStream fileRead, string outputFileName)
        {
            byte[] buffer;
            int currentByte;
            int nextByte;
            byte count = 1;

            FileStream fileWrite = new FileStream(outputFileName, FileMode.Append, FileAccess.Write);

            do {
                count++;
                currentByte = fileRead.ReadByte();
                nextByte = fileRead.ReadByte();

                if (nextByte != -1) {
                    fileRead.Position--;
                }

                if (count == 255) {
                    buffer = new byte[3] { BYTE_REPEATED, count, (byte)currentByte };
                    fileWrite.Write(buffer, 0, 3);
                    count = 0;
                }
            }
            while (currentByte == nextByte);

            buffer = new byte[3] { BYTE_REPEATED, count, (byte)currentByte };
            fileWrite.Write(buffer, 0, 3);
            fileWrite.Close();

            return currentByte;
        }

        // write different sequence of bytes
        private int DifferentBytes(FileStream fileRead, string fileName)
        {
            MemoryStream tempBuffer = new MemoryStream();
            int currentByte;
            int nextByte;
            byte count = 1;

            do
            {
                count++;
                currentByte = fileRead.ReadByte();
                nextByte = fileRead.ReadByte();

                if (nextByte != -1) {
                    fileRead.Position--;
                }                

                if (currentByte != nextByte){
                    tempBuffer.WriteByte((byte)currentByte);
                }
            }
            while ((currentByte != nextByte) && (tempBuffer.Length < 255) && (nextByte != -1));

            FileStream fileWrite = new FileStream(fileName, FileMode.Append, FileAccess.Write);
            BinaryWriter binaryStreamWrite = new BinaryWriter(fileWrite);

            binaryStreamWrite.Write(BYTE_NOT_REPEATED);
            binaryStreamWrite.Write((byte)tempBuffer.Length);
            binaryStreamWrite.Write(tempBuffer.ToArray(), 0, (int)tempBuffer.Length);

            binaryStreamWrite.Close();
            fileWrite.Close();

            return currentByte;
        }

        // processing event "Extract"
        private void ExtractClick(object sender, RoutedEventArgs e)
        {
            Thread extractThread = new Thread(ExtractFunction);

            extractThread.Start();
            extractThread.Join();

            statusBlock.Text = "Extracting done";
            statusBlock.FontSize = 14;
            statusBlock.FontWeight = FontWeights.Bold;
            statusBlock.Foreground = Brushes.Green;
            statusBlock.Visibility = Visibility.Visible;
        }

        // unpack .rle-file into the chosen file
        private void ExtractFunction()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();

            openFileDialog.Title = "Choose file for extracting...";
            openFileDialog.Filter = "rle|*.rle";

            Nullable<bool> openResult = openFileDialog.ShowDialog();

            if (openResult == true) {
                saveFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
                saveFileDialog.FileName = openFileDialog.FileName;

                Nullable<bool> saveResult = saveFileDialog.ShowDialog();

                if (saveResult == true) {
                    ExtractFile(openFileDialog.FileName, saveFileDialog.FileName);
                }
            }
        }

        // restores the initial state of the file
        private void ExtractFile(string inputFileName, string outputFileName)
        {
            FileStream fileRead = new FileStream(inputFileName, FileMode.Open, FileAccess.Read);
            BinaryReader binaryStreamRead = new BinaryReader(fileRead);

            FileStream fileWrite = new FileStream(outputFileName, FileMode.Append, FileAccess.Write);
            BinaryWriter binaryStreamWrite = new BinaryWriter(fileWrite);

            byte currentByte;
            byte count;

            do {
                currentByte = binaryStreamRead.ReadByte();

                switch (currentByte)
                {
                    case BYTE_NOT_REPEATED:
                        count = binaryStreamRead.ReadByte();
                        for (int i = 0; i < count; i++) {
                            binaryStreamWrite.Write(binaryStreamRead.ReadByte());
                        }
                        break;

                    case BYTE_REPEATED:
                        count = binaryStreamRead.ReadByte();
                        currentByte = binaryStreamRead.ReadByte();
                        for (int i = 0; i < count; i++) {
                            binaryStreamWrite.Write(currentByte);
                        }
                        break;
                }
            }
            while (binaryStreamRead.PeekChar() != -1);

            binaryStreamRead.Close();
            fileRead.Close();

            binaryStreamWrite.Close();
            fileWrite.Close();
        }
    }
}
