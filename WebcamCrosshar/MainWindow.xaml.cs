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
using System.Drawing.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Drawing;
using System.IO;
using System.ComponentModel; 
using System.Threading;


namespace WebcamCrosshar
{
    public partial class MainWindow : Window
    {
        public VideoCaptureDevice LocalWebCam;
        public FilterInfoCollection LocalWebCamCollection;
        
        private void Cam_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                BitmapImage bi;
                using (var bitmap = (Bitmap)eventArgs.Frame.Clone())
                {
                    bi = new BitmapImage();
                    bi.BeginInit();
                    MemoryStream ms = new MemoryStream();
                    bitmap.Save(ms, ImageFormat.Bmp);
                    bi.StreamSource = ms;
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.EndInit();
                }
                bi.Freeze();
                Dispatcher.BeginInvoke(new ThreadStart(delegate { frameHolder.Source = bi; }));
            }
            catch (Exception ex) { }
        }

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;   
        }
        
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LocalWebCamCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            
            var videoCaptureDevice = findBoreScope(LocalWebCamCollection);
            
            LocalWebCam = new VideoCaptureDevice(videoCaptureDevice);
            LocalWebCam.NewFrame += new NewFrameEventHandler(Cam_NewFrame);
            LocalWebCam.Start();
        }

        string findBoreScope(FilterInfoCollection collection)
        {
            //VideoCaptureDevice videoCaptureDevice = null;
            foreach(FilterInfo videoCaptureDevice in collection)
            {
                if(videoCaptureDevice.Name=="Teslong Camera")
                {
                    return videoCaptureDevice.MonikerString;
                };
            }
            return null;
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            LocalWebCam.Stop();
        }
    }
}
