﻿using PastPaperHelper.Core.Tools;
using PastPaperHelper.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace PastPaperHelper.Views
{
    /// <summary>
    /// PracticeView.xaml 的交互逻辑
    /// </summary>
    public partial class PracticeView : UserControl
    {
        public PracticeView()
        {
            InitializeComponent();
        }

        private void view_qp_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            var vm = btn.DataContext as MistakeViewModel;
            Process.Start(PastPaperHelperCore.LocalFiles[vm.QuestionPaper]);
        }

        private void view_ms_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            var vm = btn.DataContext as MistakeViewModel;
            Process.Start(PastPaperHelperCore.LocalFiles[vm.QuestionPaper.Replace("qp", "ms")]);
        }
    }
}