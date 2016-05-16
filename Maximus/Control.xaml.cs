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
    public sealed partial class Control : Page
    {
        private MqttClient client;
        byte[] message;
        double temperature;
        public Control()
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
            
            this.client.Subscribe(new string[] { "/lightsStatus" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            this.client.Subscribe(new string[] { "/heaterStatus" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            this.client.Subscribe(new string[] { "/doorStatus" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            this.client.Subscribe(new string[] { "/blindsStatus" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

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
                if (e.Topic == "/lightsStatus")
                {
                    
                    if (parsingMsg.ToString()=="off")
                    {
                        Lights.IsOn = false;
                    }
                    else if (parsingMsg.ToString() == "on")
                    {
                        Lights.IsOn = true;
                    }
                    
                }
                else if (e.Topic == "/heaterStatus")
                {
                    if (parsingMsg.ToString() == "off")
                    {
                        Heater.IsOn = false;
                    }
                    else if (parsingMsg.ToString() == "on")
                    {
                        Heater.IsOn = true;
                    }
                }
                else if (e.Topic == "/doorStatus")
                {
                    if (parsingMsg.ToString() == "off")
                    {
                        Door.IsOn = false;
                    }
                    else if (parsingMsg.ToString() == "on")
                    {
                        Door.IsOn = true;
                    }
                }
                else if (e.Topic == "/blindsStatus")
                {
                    if (parsingMsg.ToString() == "off")
                    {
                        Blinds.IsOn = false;
                    }
                    else if (parsingMsg.ToString() == "on")
                    {
                        Blinds.IsOn = true;
                    }
                }
                //this.subscribeTxt.Text = parsingMsg.ToString();

            });
            //FinalMsg(parsingMsg);



        }


        //private void toggleSwitch_Toggled(object sender, RoutedEventArgs e)
        //{

        //}

        //private void sosButton_Checked(object sender, RoutedEventArgs e)
        //{
        //            }

        //private void Lights_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
            
        //}

        private void Lights_Toggled(object sender, RoutedEventArgs e)
        {
            if (Lights.IsOn == true)
            {
                string status = "10";
                this.client.Publish("/iotControl", Encoding.UTF8.GetBytes(status));

            }
            else if (Lights.IsOn == false)
            {
                string status = "11";
                this.client.Publish("/iotControl", Encoding.UTF8.GetBytes(status));
            }
        }
        private void Heater_Toggled(object sender, RoutedEventArgs e)
        {
            if (Heater.IsOn == true)
            {
                string status = "20";
                this.client.Publish("/iotControl", Encoding.UTF8.GetBytes(status));
            }
            else if (Heater.IsOn == false)
            {
                string status = "21";
                this.client.Publish("/iotControl", Encoding.UTF8.GetBytes(status));
            }
        }
        private void Door_Toggled(object sender, RoutedEventArgs e)
        {
            if (Door.IsOn == true)
            {
                string status = "30";
                this.client.Publish("/iotControl", Encoding.UTF8.GetBytes(status));
            }
            else if (Door.IsOn == false)
            {
                string status = "31";
                this.client.Publish("/iotControl", Encoding.UTF8.GetBytes(status));
            }
        }
        private void Blinds_Toggled(object sender, RoutedEventArgs e)
        {
            if (Blinds.IsOn == true)
            {
                string status = "40";
                this.client.Publish("/iotControl", Encoding.UTF8.GetBytes(status));
            }
            else if (Blinds.IsOn == false)
            {
                string status = "41";
                this.client.Publish("/iotControl", Encoding.UTF8.GetBytes(status));
            }
        }

       
    }
}
