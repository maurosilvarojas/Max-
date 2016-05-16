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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Maximus
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Lights : Page
    {
        private MqttClient client;
        byte[] message;
        

        public Lights()
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

            this.client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

            this.client.Subscribe(new string[] { "/redlightsStatus" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            this.client.Subscribe(new string[] { "/greenlightsStatus" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            this.client.Subscribe(new string[] { "/bluelightsStatus" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
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
            });
         }

        

        private void redSwitch_Toggled(object sender, RoutedEventArgs e)
        {
//            App.illuminanceSensor=
            if (redSwitch.IsOn == true)
            {
                string status = "51";
                this.client.Publish("/iotControl", Encoding.UTF8.GetBytes(status));

            }
            else if (redSwitch.IsOn == false)
            {
                string status = "50";
                this.client.Publish("/iotControl", Encoding.UTF8.GetBytes(status));
            }
        }
        
        
        private void greenSwitch_Toggled_1(object sender, RoutedEventArgs e)
        {
            if (greenSwitch.IsOn == true)
            {
                string status = "61";
                this.client.Publish("/iotControl", Encoding.UTF8.GetBytes(status));

            }
            else if (greenSwitch.IsOn == false)
            {
                string status = "60";
                this.client.Publish("/iotControl", Encoding.UTF8.GetBytes(status));
            }
        }

        private void blueSwitch_Toggled_1(object sender, RoutedEventArgs e)
        {
            if (blueSwitch.IsOn == true)
            {
                string status = "71";
                this.client.Publish("/iotControl", Encoding.UTF8.GetBytes(status));

            }
            else if (blueSwitch.IsOn == false)
            {
                string status = "70";
                this.client.Publish("/iotControl", Encoding.UTF8.GetBytes(status));
            }
        }
    }
}
