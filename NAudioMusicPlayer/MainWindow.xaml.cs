using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Sample_NAudio;
using System.IO;
using System.ComponentModel;

namespace NAudioMusicPlayer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            NAudioEngine soundEngine = NAudioEngine.Instance;
            soundEngine.PropertyChanged += NAudioEngine_PropertyChanged;
            UIHelper.Bind(soundEngine, "CanStop", Stop, Button.IsEnabledProperty);
            UIHelper.Bind(soundEngine, "CanPlay", Play, Button.IsEnabledProperty);
            UIHelper.Bind(soundEngine, "CanPause", Pause, Button.IsEnabledProperty);
            spectrumAnalyzer.RegisterSoundPlayer(soundEngine);
            waveformTimeline.RegisterSoundPlayer(soundEngine);
        }
        #region Events
        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (NAudioEngine.Instance.CanPlay)
                NAudioEngine.Instance.Play();
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            if (NAudioEngine.Instance.CanPause)
                NAudioEngine.Instance.Pause();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            if (NAudioEngine.Instance.CanStop)
                NAudioEngine.Instance.Stop();
            NAudioEngine.Instance.ActiveStream.CurrentTime = TimeSpan.FromSeconds(0);
        }

        private void OpenFile()
        {
            Microsoft.Win32.OpenFileDialog openDialog = new Microsoft.Win32.OpenFileDialog();
            openDialog.Filter = "All Files (*.*)|*.*";
            openDialog.Multiselect = true;
            if (openDialog.ShowDialog() == true)
            {
                NAudioEngine.Instance.OpenFile(openDialog.FileName);
                textBox.Text = openDialog.FileName;
            }
        }

        private void NAudioEngine_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NAudioEngine engine = NAudioEngine.Instance;
            switch (e.PropertyName)
            {
                case "FileTag":
                    if (engine.FileTag != null)
                    {
                        TagLib.Tag tag = engine.FileTag.Tag;
                        if (tag.Title != null)
                        { 
                            this.Title = "MusicPlayer | " + tag.Album;
                        }
                        if( tag.Title == null)
                        {
                            this.Title = "Music Player";
                        }
                        if (tag.Pictures.Length > 0)
                        {
                            using (MemoryStream albumArtworkMemStream = new MemoryStream(tag.Pictures[0].Data.Data))
                            {
                                try
                                {
                                    BitmapImage albumImage = new BitmapImage();
                                    albumImage.BeginInit();
                                    albumImage.CacheOption = BitmapCacheOption.OnLoad;
                                    albumImage.StreamSource = albumArtworkMemStream;
                                    albumImage.EndInit();
                                    albumArtPanel.AlbumArtImage = albumImage;
                                    
                                }
                                catch (NotSupportedException)
                                {
                                    albumArtPanel.AlbumArtImage = null;
                                    // System.NotSupportedException:
                                    // No imaging component suitable to complete this operation was found.
                                }
                                albumArtworkMemStream.Close();
                            }
                        }
                        else
                        {
                            albumArtPanel.AlbumArtImage = null;
                        }
                    }
                    else
                    {
                        albumArtPanel.AlbumArtImage = null;
                    }
                    break;
                case "ChannelPosition":
                    digitalClock.Time = TimeSpan.FromSeconds(engine.ChannelPosition);
                    break;
                default:
                    // Do Nothing
                    break;
            }
        }
        #endregion

        private void button_Click(object sender, RoutedEventArgs e)
        {
            OpenFile();
        }

    }
}
