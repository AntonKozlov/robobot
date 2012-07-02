﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace robobot_winphone.Model
{
    public class LogManager
    {
        public static void Log(string message)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(message);
#endif
        }
    }
}
