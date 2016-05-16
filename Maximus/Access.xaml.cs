using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Media.FaceAnalysis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Media.SpeechRecognition;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Maximus
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Access : Page
    {
        public Access()
        {
            this.InitializeComponent();
            this.myCameraControl.SetFaceProcessor(this.ProcessVideoFrame);
        }

        

                    async Task ProcessVideoFrame(SoftwareBitmap bitmap)
        {
            if (this.faceDetector == null)
            {
                this.faceDetector = await FaceDetector.CreateAsync();

            }
            var results = await this.faceDetector.DetectFacesAsync(bitmap);

            var showVideo = results?.Count > 0; //? = DIFFERENT THAN NULL

            this.myCameraControl.ShowCamera(true);
            if (showVideo)
            {
                this.myCameraControl.HighlightFace(results[0].FaceBox);
            }


        }
        FaceDetector faceDetector;

        private void button_Click(object sender, RoutedEventArgs e)
        {
            //    this.myCameraControl.ShowCamera(true);
            //    //if (showVideo)
            //    //{
            //    //    this.myCameraControl.HighlightFace(results[0].FaceBox);
            //    //}
        }
    }
}
