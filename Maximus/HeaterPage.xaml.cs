using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using GIS = GHIElectronics.UWP.Shields;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Maximus
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class heaterPage : Page
    {
        int xvar = 0;
        int yvar = -800;
        int xNew,x2New;
        Double yNew,y2New;
        Point startPoint = new Point();
        double degrees;

        Point newPoint = new Point();
        Point lightsPoint = new Point();
        PointCollection myPointCollection2 = new PointCollection();
        PointCollection myPointCollection3 = new PointCollection();
        //*****
        private MqttClient client;
        byte[] message;
        double temperature;
        public heaterPage()
        {
            this.InitializeComponent();
            
            try
            {
                this.client = new MqttClient("broker.mqttdashboard.com");
                this.client.Connect(Guid.NewGuid().ToString());
                Debug.WriteLine("using broker.mqttdashboard.com");

            }
            catch (Exception)
            {
                try
                {
                    this.client = new MqttClient("iot.eclipse.org");
                    this.client.Connect(Guid.NewGuid().ToString());
                    Debug.WriteLine("using iot.eclipse.org");
                }
                catch (Exception)
                {
                    Debug.WriteLine("BROKER DOWN");
                }
            }
            
            //byte[] qosLevels = { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE };
            this.client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
            this.client.Subscribe(new string[] { "/TempSensor" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            //this.client.Subscribe(new string[] { "/LightSensor" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

            //*** drawing 
            myPolyline.StrokeThickness = 2;
            myPolyline.FillRule = FillRule.EvenOdd;
            startPoint.X = 0;
            startPoint.Y = -400;

            newPoint.X = 0;
            newPoint.Y = (0 - 400) * (-1);
            myPointCollection2.Add(newPoint);
            
            
        }


        async void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {

            message = e.Message;
            StringBuilder parsingMsg = new StringBuilder();
            foreach (var value in message)
            {
                parsingMsg.Append(Char.ConvertFromUtf32(value));
            }
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                tempData.Text = parsingMsg.ToString();

                yNew = Double.Parse(tempData.Text);
                xNew = xNew + 1;
                //xNew = Int32.Parse(xPoint.Text);
                //yNew = Int32.Parse(yPoint.Text);
                newPoint.X = xNew;
                newPoint.Y = ((1.5 * yNew) - 200) * (-1);//(yNew - 400) * (-1);
                myPointCollection2.Add(newPoint);
                myPolyline.Points = myPointCollection2;

            });
        }

    private void tempData_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            
        }

        //private void LightData_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        //{

        //}
    }
}
  