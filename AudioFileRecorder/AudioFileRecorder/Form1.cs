using System.Configuration;
using System.Collections.Specialized;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using NAudio.Wave;
using System.IO;

namespace AudioFileRecorder
{
    public partial class Form1 : Form
    { 
        public string defaultPath;
        public Form1()
        {
            InitializeComponent();

            
            if (Properties.Settings.Default.defaultFilePath == null || Properties.Settings.Default.defaultFilePath == "")
            {
                FolderBrowserDialog fbd1 = new FolderBrowserDialog();
                fbd1.RootFolder = Environment.SpecialFolder.MyComputer;
                fbd1.Description = "Select a folder to save to:";
                fbd1.ShowNewFolderButton = true;

                if (fbd1.ShowDialog() == DialogResult.OK)
                {
                    //Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    //config.AppSettings.Settings.Add("SaveFilePath", fbd1.SelectedPath);
                    //config.Save();
                    Properties.Settings.Default.defaultFilePath = fbd1.SelectedPath;
                    Properties.Settings.Default.Save();
                    //ConfigurationManager.RefreshSection("appSettings");

                    //System.Configuration.ConfigurationManager.AppSettings["SaveFilePath"] = fbd1.SelectedPath;
                }
            }

            for (int items = 8200; items < 8221; items++ )
            {
                listView2.Items.Add(items.ToString());
            }
            List<WaveInCapabilities> sources = Recording.SelectInput();

            listView1.Items.Clear();

            foreach (var source in sources)
            {
                ListViewItem item = new ListViewItem(source.ProductName);
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, source.Channels.ToString()));
                listView1.Items.Add(item);
            }
            //textBox1.Text = ConfigurationManager.AppSettings.Get("SaveFilePath");
            textBox1.Text = Properties.Settings.Default.defaultFilePath;
            defaultPath = Properties.Settings.Default.defaultFilePath;
        }

        string fileName = null;
        //Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        //string defaultPath = config.AppSettings.Settings["SaveFilePath"].Value;
        //string defaultPath = ConfigurationManager.AppSettings.Get("SaveFilePath");
        
        //string command = null;
        //string filePath = "C:\\Users\\bminster\\Desktop\\";
        string copyFileDestination;

        private void btnSource_Click(object sender, EventArgs e)
        {
            List<WaveInCapabilities> sources = Recording.SelectInput();

            listView1.Items.Clear();

            foreach (var source in sources)
            {
                ListViewItem item = new ListViewItem(source.ProductName);
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, source.Channels.ToString()));
                listView1.Items.Add(item);
            }
        }

        WaveIn sourceStream = null;
        DirectSoundOut waveOut = null;
        WaveFileWriter waveWriter = null;
        WaveFileReader waveReader = null;

        private void btnRec_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select an audio source to begin recording");
                return;
            }
            try
            {
                SaveFileDialog save = new SaveFileDialog();
                save.Filter = "Wave File (*.wav|*.wav;*";
                //save.InitialDirectory = "C:\\Users\\bminster\\Downloads\\ICG wav Files\\Prompts\\";
                save.FileName = "C:\\Users\\bminster\\Downloads\\ICG wav Files\\Prompts\\" + "0000TEMP.wav";
                //this.DialogResult = DialogResult.OK;
                //if (save.ShowDialog() != DialogResult.OK) return;
                fileName = save.FileName;
                textBox1.Text = fileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while saving your file. Please try again.");
                Console.WriteLine("------------------");
                Console.WriteLine("Message: " + ex.Message);
                Console.WriteLine("Source: " + ex.Source);
                Console.WriteLine("Stack Trace: " + ex.StackTrace);
                Console.WriteLine("------------------");
                return;
            }

            btnStopRecording.Visible = true;
            btnRec.Visible = false;

            int deviceNumber = listView1.SelectedItems[0].Index;
            DisposeWave();
            //waveWriter = null;
            //waveWriter.Dispose();

            try
            {
                sourceStream = new WaveIn();
                sourceStream.DeviceNumber = deviceNumber;
                sourceStream.WaveFormat = new WaveFormat(44100, WaveIn.GetCapabilities(deviceNumber).Channels);
                sourceStream.DataAvailable += new EventHandler<WaveInEventArgs>(sourceStream_DataAvailable);
                waveWriter = new WaveFileWriter(fileName, sourceStream.WaveFormat);

                sourceStream.StartRecording();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Console.WriteLine("------------------");
                Console.WriteLine("Message: " + ex.Message);
                Console.WriteLine("Source: " + ex.Source);
                Console.WriteLine("Stack Trace: " + ex.StackTrace);
                Console.WriteLine("------------------");
            }
        }

        private void btnStopRecording_Click(object sender, EventArgs e)
        {
            btnStopRecording.Visible = false;
            btnRec.Visible = true;
            if (waveWriter != null)
            {
                waveWriter.Dispose();
                waveWriter = null;
            }
        }


        private void sourceStream_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (waveWriter == null) return;
            waveWriter.Write(e.Buffer, 0, e.BytesRecorded);
            waveWriter.Flush();
        }

        //private void mediaPlayer(string command, string file)
        //{
        //    var myPlayer = new SoundPlayer();
        //    if (command == "PLAY")
        //    {
        //        myPlayer.SoundLocation = file;
        //        myPlayer.Play();
        //    }
        //    if (command == "STOP")
        //    {
        //        myPlayer.Stop();
        //    }
        //}

        //private void btnPlay_Click(object sender, EventArgs e)
        //{
        //    if (fileName == null) return;

        //    if (waveOut == null)
        //    {
        //        waveReader = new WaveFileReader(fileName);
        //        waveOut = new DirectSoundOut();
        //        waveOut.Init(new WaveChannel32(waveReader));
        //        waveOut.Play();
        //        waveOut.Stop();
        //    }
        //    else
        //    {
        //        waveOut.Play();
        //        waveOut.Stop();
        //    }
            
            //btnPlay.Visible = false;
            //btnPause.Visible = true;
            //if (waveReader. == WaveFileReader.EndRead) DisposeWave();
            //command = "PLAY";
            //if (fileName == null || fileName =="")
            //{
            //    MessageBox.Show("Please select an audio file to play");
            //    return;
            //}

            //mediaPlayer(command, fileName);
        //}

        //private void btnPause_Click(object sender, EventArgs e)
        //{
        //    waveOut.Pause();
        //    btnPause.Visible = false;
        //    btnPlay.Visible = true;
        //}

        private void btnStop_Click(object sender, EventArgs e)
        {
            //command = "STOP";
            //mediaPlayer(command, fileName);
            //if (mediaPlayer)
            //{

            //}
            DisposeWave();
            if (sourceStream != null)
            {
                sourceStream.StopRecording();
                sourceStream.Dispose();
                sourceStream = null;
            }
            //axWindowsMediaPlayer1.Dispose();
            //btnPause.Visible = false;
            //btnPlay.Visible = true;
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd1 = new OpenFileDialog();
            ofd1.Filter = "Wave File (*.wav|*.wav;*";
            if (ofd1.ShowDialog() == DialogResult.OK)
            {
                fileName = ofd1.FileName;
                textBox1.Text = fileName;
                //waveReader = new WaveFileReader(fileName);
                //waveOut = new DirectSoundOut();
                //waveOut.Init(new WaveChannel32(waveReader));
                axWindowsMediaPlayer1.URL = fileName;
                axWindowsMediaPlayer1.Ctlcontrols.play();
            }
            else
            {
                MessageBox.Show("Please select a valid file to play");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            string message = "Are you sure you want to delete " + fileName;
            DialogResult result = MessageBox.Show(message,"Delete Current Working File", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                DisposeWave();
                axWindowsMediaPlayer1.URL = null;
                System.IO.File.Delete(fileName);
                fileName = null;
            }
            else if (result == DialogResult.No) return;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            fileName = textBox1.Text;
            //try
            //{
            //    waveReader = new WaveFileReader(fileName);
            //    waveOut = new DirectSoundOut();
            //    waveOut.Init(new WaveChannel32(waveReader));
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
        }

        private void DisposeWave()
        {
            if (waveOut != null)
            {
                if (waveOut.PlaybackState == PlaybackState.Playing) waveOut.Stop();
                waveOut.Dispose();
                waveOut = null;
            }
            if (waveReader != null)
            {
                waveReader.Flush();
                waveReader.Dispose();
                waveReader = null;
            }
            if (waveWriter != null)
            {
                waveWriter.Flush();
                waveWriter.Dispose();
                waveWriter = null;
            }
            if (axWindowsMediaPlayer1.IsDisposed != true)
            {
                axWindowsMediaPlayer1.URL = null;
                axWindowsMediaPlayer1.currentPlaylist.clear();
                //axWindowsMediaPlayer1.Dispose();
                //axWindowsMediaPlayer1.Visible = true;
                AxWMPLib.AxWindowsMediaPlayer myMediaPlayer = new AxWMPLib.AxWindowsMediaPlayer();
                this.Controls.Add(myMediaPlayer);

            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DisposeWave();
        }

        private void axWindowsMediaPlayer1_Enter(object sender, EventArgs e)
        {
            //using (axWindowsMediaPlayer1 = new AxWMPLib.AxWindowsMediaPlayer())
            //{
            //    axWindowsMediaPlayer1.CreateControl();
            //    axWindowsMediaPlayer1.Ctlenabled.Equals(true);
                axWindowsMediaPlayer1.Ctlcontrols.play();
                axWindowsMediaPlayer1.Ctlcontrols.stop();
                axWindowsMediaPlayer1.Ctlcontrols.pause();
                axWindowsMediaPlayer1.URL = fileName;
            //}
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            //if (axWindowsMediaPlayer1.IsDisposed != true)
            //{
            //    axWindowsMediaPlayer1.URL = null;
            //    axWindowsMediaPlayer1.currentPlaylist.clear();
            //    axWindowsMediaPlayer1.Dispose();
                
            //    axWindowsMediaPlayer1.Show();
            //}
            //if (listView2.SelectedItems[0] == null)
            //{
            //    MessageBox.Show("Please select an account to save this wave file to");
            //    return;
            //}
            ListViewItem selected;
            try
            {
                selected = listView2.SelectedItems[0];
                copyFileDestination = selected.Text.ToString();
            }
            catch (ArgumentOutOfRangeException ex)
            {
                MessageBox.Show("Please select an account to save this wave file to");
                Console.WriteLine(ex);
                return;
            }
            DialogResult result;
            result = MessageBox.Show("Save new audio file to account " + selected.Text + "?", "Save file", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                if (Directory.Exists(defaultPath + "\\" + copyFileDestination + "\\"))
                {

                }
                else
                {
                    Directory.CreateDirectory(defaultPath + copyFileDestination + "\\");
                }
                if (File.Exists(defaultPath + "\\" + copyFileDestination + "\\" + copyFileDestination + ".wav"))
                {
                    File.Copy(fileName, defaultPath + "\\" + copyFileDestination + "\\" + copyFileDestination + ".wav", true);
                }
                else
                {
                    File.Copy(fileName, defaultPath + copyFileDestination + "\\" + copyFileDestination + ".wav");
                }
                fileName = defaultPath + "\\" + copyFileDestination + "\\" + copyFileDestination + ".wav";
                axWindowsMediaPlayer1.URL = fileName;
                textBox1.Text = fileName;

            }

            else return;
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnAddAccount_Click(object sender, EventArgs e)
        {
            using (var addAccount = new AddAccount())
            {
                var result = addAccount.ShowDialog();
                if (result == DialogResult.OK)
                {
                    listView2.Items.Add(addAccount.accountNumber);
                }
            }
            
        }

        private void btnSetPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd1 = new FolderBrowserDialog();
            fbd1.RootFolder = Environment.SpecialFolder.MyComputer;
            fbd1.Description = "Select a folder to save to:";
            fbd1.ShowNewFolderButton = true;

            if (fbd1.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.defaultFilePath = fbd1.SelectedPath;
                Properties.Settings.Default.Save();
                defaultPath = Properties.Settings.Default.defaultFilePath;
                MessageBox.Show("Save folder changed to: " + Properties.Settings.Default.defaultFilePath);
            }

        }





    }
}
