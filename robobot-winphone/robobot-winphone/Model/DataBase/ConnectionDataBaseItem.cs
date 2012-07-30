﻿using System;
using System.Data.Linq.Mapping;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace robobot_winphone.Model.DataBase
{
    [Table]
    public class ConnectionDataBaseItem
    {
        [Column(IsPrimaryKey = true)]
        public string Name { get; set; }
        [Column()]
        public string Ip { get; set; }
        [Column()]
        public int Port  { get; set; }
    }
}
