﻿#pragma checksum "D:\Projects\整技科技\SVN\Source\iTec_uwp\iTec_uwp\Sensor2_Page.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "A2EDEC4D9D77E5D01AE8BF46D8E022A5"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace iTec_uwp
{
    partial class Sensor2_Page : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.17.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 2: // Sensor2_Page.xaml line 12
                {
                    this.textBox_Copy = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                }
                break;
            case 3: // Sensor2_Page.xaml line 13
                {
                    this.textBox_Copy1 = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                }
                break;
            case 4: // Sensor2_Page.xaml line 14
                {
                    this.textBox_Copy2 = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                }
                break;
            case 5: // Sensor2_Page.xaml line 40
                {
                    this.btnExit = (global::Windows.UI.Xaml.Controls.Button)(target);
                    ((global::Windows.UI.Xaml.Controls.Button)this.btnExit).Click += this.btnExit_Click;
                }
                break;
            case 6: // Sensor2_Page.xaml line 34
                {
                    this.rpcResistance = (global::Microsoft.Toolkit.Uwp.UI.Controls.RadialProgressBar)(target);
                }
                break;
            case 7: // Sensor2_Page.xaml line 37
                {
                    this.textBox_Copy8 = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                }
                break;
            case 8: // Sensor2_Page.xaml line 38
                {
                    this.txtResistanceValue = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                }
                break;
            case 9: // Sensor2_Page.xaml line 27
                {
                    this.rpcCadence = (global::Microsoft.Toolkit.Uwp.UI.Controls.RadialProgressBar)(target);
                }
                break;
            case 10: // Sensor2_Page.xaml line 30
                {
                    this.textCadenceValue = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                }
                break;
            case 11: // Sensor2_Page.xaml line 31
                {
                    this.txtCadenceValue = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                }
                break;
            case 12: // Sensor2_Page.xaml line 16
                {
                    this.rpcPower = (global::Microsoft.Toolkit.Uwp.UI.Controls.RadialProgressBar)(target);
                }
                break;
            case 13: // Sensor2_Page.xaml line 19
                {
                    this.textBox_Copy4 = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                }
                break;
            case 14: // Sensor2_Page.xaml line 20
                {
                    this.txtPowerValue = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        /// <summary>
        /// GetBindingConnector(int connectionId, object target)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.17.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

